using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
            Parse();
        }

        private Token Check(IEnumerable<TokenId> tokens)
        {
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

        private void MethodProcessing()
        {
            var methodToken = Check(TypeList.GetMethodsTokens());
            if (methodToken is null) 
                return;
            
            var methodArgumentToken = Check(TypeList.GetTokenBy(Direction));
            if (methodArgumentToken is null)
                Stop(new Exception("Отсутствует аргумент у метода"));

            Run(new MethodOperation(methodToken, methodArgumentToken));
        }

        private Symbol BooleanExpressionProcessing()
        {
            var operandLeft = Check(TypeList.GetLeftBooleanArgumentTokens());
            if (operandLeft is null)
            {
                Stop(new Exception("Отсутствует левый операнд"));
                return null;
            }

            var comparisonOperator = Check(TypeList.GetTokenBy(ComparisonOperator));
            if (comparisonOperator is null)
            {
                Stop(new Exception("Отсутствует оператор"));
                return null;
            }
            
            var operandRight = Check(TypeList.GetTokenBy(BooleanArgument));
            if (operandRight is null)
            {
                Stop(new Exception("Отсутствует правый операнд"));
                return null;
            }
            
            return new BooleanOperation(operandLeft, comparisonOperator, operandRight);
        }

        private void AddBooleanExpressions(ICollection<bool> booleanExpressions, ICollection<TokenType> booleanOperators)
        {
            Token booleanOperator;
            do
            {
                var booleanExpression = BooleanExpressionProcessing();
                var boolResult = RunQ(booleanExpression);
                booleanExpressions.Add(boolResult);
                
                booleanOperator = Check(new List<TokenId>{TypeList.GetTokenBy(And), TypeList.GetTokenBy(Or)});
                if (booleanOperator != null)
                    booleanOperators.Add(booleanOperator.Id.Type);
            } while (booleanOperator != null);
        }
        
        private bool CheckCondition()
        {
            List<bool> booleanExpressions = new();
            List<TokenType> booleanOperators = new();
            AddBooleanExpressions(booleanExpressions, booleanOperators);
            
            var endConditions = Check(TypeList.GetTokenBy(EndBody));
            if (endConditions is null)
            {
                Stop(new Exception("Отсутствует конец блока If"));
                return false;
            }

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
        
        private void ConditionalOperatorProcessing()
        {
            var conditionalToken = Check(TypeList.GetTokenBy(If));
            if (conditionalToken is null) 
                return;

            var resultConditions = CheckCondition();
            if (resultConditions)
            {
                // Блок IF
                Token endTokenIf;
                do
                {
                    ExpressionProcessing();
                    endTokenIf = Check(new List<TokenId> {TypeList.GetTokenBy(Else), TypeList.GetTokenBy(EndIf)});
                } while (endTokenIf is null);
                
                if (endTokenIf.Id.Type == EndIf) 
                    return;
                // Блок Else
                var countTokenIf = 1;
                while (true) // Пропускаем все токены до конца else'а
                {
                    if (Check(TypeList.GetTokenBy(If)) is not null)
                        countTokenIf++;
                    var t = Check(TypeList.GetTokenBy(EndIf));
                    if (t is not null)
                    {
                        countTokenIf--;
                        if (countTokenIf == 0)
                            return;
                    }
                    _position++;
                }
            }
            else
            {
                // Блок If
                Token endTokenIf;
                var countTokenIf = 1;
                while (true) // Пропускаем все токены пока не встретим Else или EndIf
                {
                    if (Check(TypeList.GetTokenBy(If)) is not null)
                        countTokenIf++;
                    endTokenIf = Check(new List<TokenId> {TypeList.GetTokenBy(Else), TypeList.GetTokenBy(EndIf)});
                    if (endTokenIf is not null)
                    {
                        if (endTokenIf.Id.Type == EndIf || countTokenIf == 1)
                            countTokenIf--;
                        if (countTokenIf == 0)
                            break;
                    }
                    _position++;
                }

                if (endTokenIf.Id.Type == EndIf)
                    return;
                
                if (Check(TypeList.GetTokenBy(EndBody)) is null)
                {
                    Stop(new Exception("Отсутствует конец блока else"));
                    return;
                }
                // Блок Else
                while (true) // Пропускаем все токены пока не встретим EndIf
                {
                    ExpressionProcessing();
                    if (Check(TypeList.GetTokenBy(EndIf)) is not null)
                        return;
                }
            }
        }
        
        private void ExpressionProcessing()
        {
            MethodProcessing();
            ConditionalOperatorProcessing();
        }

        private void Parse()
        {
            while (_position < _tokens.Count)
                ExpressionProcessing();
        }

        private bool RunQ(Symbol symbol)
        {
            switch (symbol)
            {
                case BooleanOperation b:
                {
                    return _player.Check(b.LeftOperand, b.СomparisonOperator, b.RightOperand);
                }
                default:
                {
                    Debug.LogException(new InterpreterRunException());
                    throw new InterpreterRunException();
                }
            }
            
        }

        private void Run(Symbol symbol)
        {
            switch (symbol)
            {
                case MethodOperation m:
                {
                    var argument = m.Argument.Value;
                    switch (m.MethodType.Id.Type)
                    {
                        case Step:
                            _player.Step(argument);
                            return;
                        case TakeFrom:
                            _player.TakeFrom(argument);
                            return;
                        case GiveTo:
                            _player.GiveTo(argument);
                            return;
                    }
                    break;
                }
            }
        }

        private void Stop(Exception exception)
        {
            Debug.LogException(exception);
            throw exception;
        }
    }
}
