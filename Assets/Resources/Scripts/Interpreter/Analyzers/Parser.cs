using System;
using System.Collections.Generic;
using System.Linq;
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
        private int _tokenPosition;

        public Parser(List<Token> tokens, Player player)
        {
            _tokens = tokens;
            _player = player;
            Parse();
        }

        private Token Check(IEnumerable<TokenId> tokens)
        {
            if (_tokenPosition >= _tokens.Count) 
                return null;
            
            var currentToken = _tokens[_tokenPosition];
            if (tokens.FirstOrDefault(t => t.Type == currentToken.Id.Type) is null) 
                return null;

            _tokenPosition++;
            return currentToken;
        }
        
        private Token Check(TokenId token)
        {
            if (_tokenPosition >= _tokens.Count) 
                return null;
            
            var currentToken = _tokens[_tokenPosition];
            if (token.Type != currentToken.Id.Type) 
                return null;
            
            _tokenPosition++;
            return currentToken;
        }

        private void MethodProcessing()
        {
            var method = Check(TypeList.GetMethodsTokens());
            if (method is null) 
                return;
            
            var argument = Check(TypeList.GetTokenBy(Direction));
            if (argument is null)
                throw new MissingMethodArgument();

            ExternalProcessing(new MethodOperation(method, argument));
        }

        private Symbol BooleanExpressionProcessing()
        {
            var leftOperand = Check(TypeList.GetLeftBooleanArgumentTokens());
            if (leftOperand is null)
                throw new BooleanExpressionProcessingException();

            var comparisonOperator = Check(TypeList.GetTokenBy(ComparisonOperator));
            if (comparisonOperator is null)
                throw new BooleanExpressionProcessingException();
            
            var rightOperand = Check(TypeList.GetTokenBy(BooleanArgument));
            if (rightOperand is null)
                throw new BooleanExpressionProcessingException();
            
            return new BooleanOperation(leftOperand, comparisonOperator, rightOperand);
        }

        private void ReadBooleanExpressions(ICollection<bool> booleanExpressions, ICollection<TokenType> booleanOperators)
        {
            Token booleanOperator;
            do
            {
                var booleanExpression = BooleanExpressionProcessing();
                var boolResult = ExternalLogicProcessing(booleanExpression);
                booleanExpressions.Add(boolResult);
                
                booleanOperator = Check(new List<TokenId>{TypeList.GetTokenBy(And), TypeList.GetTokenBy(Or)});
                if (booleanOperator is not null)
                    booleanOperators.Add(booleanOperator.Id.Type);
            } while (booleanOperator != null);

            if (Check(TypeList.GetTokenBy(EndBody)) is null)
                throw new MissingMatchingTokenException(_tokens[_tokenPosition]);
        }

        private static bool WriteBooleanExpressionResult(IList<bool> booleanExpressions, IReadOnlyList<TokenType> booleanOperators)
        {
            for (var i = 0; i < booleanOperators.Count; i++)
            {
                var result = booleanOperators[i] switch
                {
                    And => booleanExpressions[i] && booleanExpressions[i + 1],
                    Or => booleanExpressions[i] || booleanExpressions[i + 1],
                    _ => true
                };
                booleanExpressions[i + 1] = result;
            }
            return booleanExpressions.Last();
        }
        
        private bool CheckCondition()
        {
            List<bool> booleanExpressions = new();
            List<TokenType> booleanOperators = new();
            ReadBooleanExpressions(booleanExpressions, booleanOperators);

            return WriteBooleanExpressionResult(booleanExpressions, booleanOperators);
        }
        
        private void ConditionalOperatorProcessing()
        {
            var conditionalToken = Check(TypeList.GetTokenBy(If));
            if (conditionalToken is null) 
                return;

            var resultConditions = CheckCondition();
            if (resultConditions)
            {
                Token newInstructions;
                do
                {
                    ExpressionProcessing();
                    newInstructions = Check(new List<TokenId> {TypeList.GetTokenBy(Else), TypeList.GetTokenBy(EndIf)});
                } while (newInstructions is null);
                
                if (newInstructions.Id.Type == EndIf) 
                    return;
                
                #region PassTokens
                
                var numberTokensIf = 1;
                while (true)
                {
                    if (Check(TypeList.GetTokenBy(If)) is not null)
                        numberTokensIf++;

                    if (Check(TypeList.GetTokenBy(EndIf)) is not null)
                    {
                        numberTokensIf--;
                        if (numberTokensIf == 0)
                            return;
                    }
                    _tokenPosition++;
                }
                
                #endregion
            }
            else
            {
                #region PassTokens

                Token endTokenIf;
                var numberTokensIf = 1;
                while (true) // Пропускаем все токены пока не встретим Else или EndIf
                {
                    if (Check(TypeList.GetTokenBy(If)) is not null)
                        numberTokensIf++;
                    endTokenIf = Check(new List<TokenId> {TypeList.GetTokenBy(Else), TypeList.GetTokenBy(EndIf)});
                    if (endTokenIf is not null)
                    {
                        if (endTokenIf.Id.Type == EndIf || numberTokensIf == 1)
                            numberTokensIf--;
                        if (numberTokensIf == 0)
                            break;
                    }
                    _tokenPosition++;
                }

                #endregion

                if (endTokenIf.Id.Type == EndIf)
                    return;
                
                while (true) 
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
            while (_tokenPosition < _tokens.Count)
                ExpressionProcessing();
        }

        private bool ExternalLogicProcessing(Symbol symbol)
        {
            switch (symbol)
            {
                case BooleanOperation b:
                {
                    return _player.Check(b.LeftOperand, b.СomparisonOperator, b.RightOperand);
                }
                default:
                    throw new MissingSymbolClassException();
            }
        }

        private void ExternalProcessing(Symbol symbol)
        {
            switch (symbol)
            {
                case MethodOperation m:
                {
                    var argument = m.Argument.Value;
                    if (m.MethodType.Id.Type == Step)
                        _player.Step(argument);
                    else if (m.MethodType.Id.Type == TakeFrom)
                        _player.TakeFrom(argument);
                    else if (m.MethodType.Id.Type == GiveTo) 
                        _player.GiveTo(argument);

                    break;
                }
                default:
                    throw new MissingSymbolClassException();
            }
        }
    }
}
