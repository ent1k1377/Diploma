using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Resources.Scripts.Interpreter.Exceptions;
using Resources.Scripts.Interpreter.Symbols;
using Resources.Scripts.Interpreter.TokenInfo;
using Resources.Scripts.Interpreter.Types;
using UnityEngine;
using static Resources.Scripts.Interpreter.Types.TokenType;

namespace Resources.Scripts.Interpreter.Analyzers
{
    public class Parser
    {
        private readonly List<Token> _tokens;
        private readonly Player _player;
        private int _position;

        public Parser(List<Token> tokens, Player player) // Поменять в методах где возращаю bool, false на исключения всевохможные
        {
            _tokens = tokens;
            _player = player;
            _ = Parse();
        }

        private Token Check(List<TokenId> tokens)
        {
            if (tokens == null) 
                throw new ArgumentNullException(nameof(tokens));
            if (_position >= _tokens.Count) 
                return null;
            
            var token = _tokens[_position];
            var exp = tokens.FirstOrDefault(t => t.Type == token.Id.Type);
            if (exp == null) 
                return null;

            _position++;
            return token;
        }
        
        private Token Check(TokenId token)
        {
            if (_position >= _tokens.Count) 
                return null;
            
            var currentToken = _tokens[_position];
            if (token.Type != currentToken.Id.Type) 
                return null;
            
            _position++;
            return currentToken;
        }

        private async Task<bool> MethodProcessing()
        {
            var methodToken = Check(TypeList.GetMethodsTokens());
            if (methodToken is null) 
                return false;
            
            var methodArgumentToken = Check(TypeList.GetTokenBy(Direction));
            if (methodArgumentToken is null)
                throw new PassingArgumentException();
            
            await Run(new MethodOperation(methodToken, methodArgumentToken));
            return true;
        }

        private Symbol BooleanExpressionProcessing()
        {
            var operandLeft = Check(TypeList.GetLeftBooleanArgumentTokens());
            if (operandLeft is null)
                throw new Exception();
            
            var comparisonOperator = Check(TypeList.GetTokenBy(ComparisonOperator));
            if (comparisonOperator is null)
                throw new Exception();
            
            var operandRight = Check(TypeList.GetTokenBy(BooleanArgument));
            return operandRight is null ? throw new Exception() : new BooleanOperation(operandLeft, comparisonOperator, operandRight);
        }

        private async Task AddBooleanExpressions(ICollection<bool> booleanExpressions, ICollection<TokenType> booleanOperators)
        {
            Token booleanOperator;
            do
            {
                var booleanExpression = BooleanExpressionProcessing();
                var boolResult = await RunQ(booleanExpression);
                booleanExpressions.Add(boolResult);
                
                booleanOperator = Check(new List<TokenId>{TypeList.GetTokenBy(And), TypeList.GetTokenBy(Or)});
                if (booleanOperator != null)
                    booleanOperators.Add(booleanOperator.Id.Type);
            } while (booleanOperator != null);
        }
        
        private async Task<bool> CheckCondition()
        {
            List<bool> booleanExpressions = new();
            List<TokenType> booleanOperators = new();
            await AddBooleanExpressions(booleanExpressions, booleanOperators);
            
            var endConditions = Check(TypeList.GetTokenBy(EndBody));
            if (endConditions is null)
                throw new Exception();
            
            for (var i = 0; i < booleanOperators.Count; i++)
            {
                bool result;
                if (booleanOperators[i] == And)
                    result = booleanExpressions[i] && booleanExpressions[i + 1];
                else
                    result = booleanExpressions[i] || booleanExpressions[i + 1];
                booleanExpressions[i + 1] = result;
            }
            return booleanExpressions.Last();
        }
        
        private async Task<bool> ConditionalOperatorProcessing()
        {
            var conditionalToken = Check(TypeList.GetTokenBy(If));
            if (conditionalToken is null) 
                return false;
            
            var resultConditions = await CheckCondition();
            if (resultConditions)
            {
                Token endTokenIf;
                do
                {
                    await ExpressionProcessing();
                    endTokenIf = Check(new List<TokenId> {TypeList.GetTokenBy(Else), TypeList.GetTokenBy(EndIf)});
                    Debug.Log(endTokenIf is null);
                } while (endTokenIf is null);
                
                Debug.Log("Вышел из цикла");
                if (endTokenIf.Id.Type == EndIf) 
                    return true;

                while (true) // Пропускаем все токены до конца if'а
                {
                    var t = Check(TypeList.GetTokenBy(EndIf));
                    Debug.Log(t);
                    if (t is not null)
                        return true;
                }
                    
            }
            else
            {
                Token endTokenIf;
                while (true) // Пропускаем все токены пока не встретим Else
                {
                    endTokenIf = Check(new List<TokenId> {TypeList.GetTokenBy(Else), TypeList.GetTokenBy(EndIf)});
                    if (Check(TypeList.GetTokenBy(Else)) is not null)
                        break;
                }

                if (endTokenIf.Id.Type == EndIf)
                    return true;
                
                
                while (true) // Пропускаем все токены пока не встретим Else
                {
                    if (Check(TypeList.GetMethodsTokens()) is not null)
                        await MethodProcessing();
                    else if (Check(TypeList.GetTokenBy(If)) is not null)
                        await ConditionalOperatorProcessing();
                    else if (Check(TypeList.GetTokenBy(EndIf)) is not null)
                        return true;
                }
            }
        }
        
        private async Task ExpressionProcessing()
        {
            if (await MethodProcessing())
                return;
            if (await ConditionalOperatorProcessing())
                return;
        }

        private async Task Parse()
        {
            while (_position < _tokens.Count)
                await ExpressionProcessing();
        }

        private async Task<bool> RunQ(Symbol symbol)
        {
            switch (symbol)
            {
                case BooleanOperation b:
                {
                    return await _player.Check(b.LeftOperand, b.СomparisonOperator, b.RightOperand);
                }
            }
            throw new InterpreterRunException();
        }

        private async Task Run(Symbol symbol)
        {
            switch (symbol)
            {
                case MethodOperation m:
                {
                    var argument = m.Argument.Value;
                    switch (m.MethodType.Id.Type)
                    {
                        case Step:
                            await _player.Step(argument);
                            return;
                        case TakeFrom:
                            await _player.TakeFrom(argument);
                            return;
                        case GiveTo:
                            await _player.GiveTo(argument);
                            return;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            throw new InterpreterRunException();
        }
    }
}
