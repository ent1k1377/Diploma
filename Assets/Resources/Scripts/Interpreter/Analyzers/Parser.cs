using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Resources.Scripts.Interpreter.Exceptions;
using Resources.Scripts.Interpreter.Symbols;
using Resources.Scripts.Interpreter.TokenInfo;
using Resources.Scripts.Interpreter.Types;
using static Resources.Scripts.Interpreter.Types.TokenType;

namespace Resources.Scripts.Interpreter.Analyzers
{
    public class Parser
    {
        private readonly Dictionary<string, int> _gotoStorage = new();
        private readonly List<Token> _tokens;
        private readonly Player.Player _player;
        private int _tokenPosition;

        private readonly CancellationTokenSource _tokenSource = new();
        
        public Parser(List<Token> tokens, Player.Player player)
        {
            
            _tokens = tokens;
            _player = player;
            _ = Parse();
            _player.Destroyed += OnDestroyed;
        }

        private void OnDestroyed()
        {
            _tokenSource.Cancel();
            _player.Destroyed -= OnDestroyed;
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

        private bool MethodProcessing()
        {
            var method = Check(TypeList.GetMethodsTokens());
            if (method is null)
                return false;

            var argument = Check(TypeList.GetTokenBy(Direction));
            if (argument is null)
                throw new MissingMethodArgument();

            ExternalProcessing(new MethodOperation(method, argument));
            return true;
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

        private void ReadBooleanExpressions(ICollection<bool> booleanExpressions,
            ICollection<TokenType> booleanOperators)
        {
            Token booleanOperator;
            do
            {
                var booleanExpression = BooleanExpressionProcessing();
                var boolResult = ExternalLogicProcessing(booleanExpression);
                booleanExpressions.Add(boolResult);

                booleanOperator = Check(new List<TokenId> {TypeList.GetTokenBy(And), TypeList.GetTokenBy(Or)});
                if (booleanOperator is not null)
                    booleanOperators.Add(booleanOperator.Id.Type);
            } while (booleanOperator != null);

            if (Check(TypeList.GetTokenBy(EndBody)) is null)
                throw new MissingMatchingTokenException(_tokens[_tokenPosition]);
        }

        private static bool WriteBooleanExpressionResult(IList<bool> booleanExpressions,
            IReadOnlyList<TokenType> booleanOperators)
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

        private bool ConditionalOperatorProcessing()
        {
            var conditionalToken = Check(TypeList.GetTokenBy(If));
            if (conditionalToken is null)
                return false;

            var resultConditions = CheckCondition();
            if (resultConditions)
            {
                Token newInstructions;
                do
                {
                    if (Check(TypeList.GetTokenBy(GoTo)) is not null)
                    {
                        _tokenPosition--;
                        return true;
                    }
                    ExpressionProcessing();
                    newInstructions = Check(new List<TokenId> {TypeList.GetTokenBy(Else), TypeList.GetTokenBy(EndIf)});
                } while (newInstructions is null);

                if (newInstructions.Id.Type == EndIf)
                    return true;

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
                            return true;
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
                while (true)
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
                    return true;

                while (true)
                {
                    ExpressionProcessing();
                    if (Check(TypeList.GetTokenBy(EndIf)) is not null)
                        return true;
                }
            }
        }

        private bool LabelProcessing()
        {
            var labelToken = Check(TypeList.GetTokenBy(Label));
            if (labelToken is null)
                return false;
            
            if (!_gotoStorage.ContainsKey(labelToken.Value))
                _gotoStorage.Add(labelToken.Value, --_tokenPosition);
            return true;
        }
        
        private bool GotoProcessing()
        {
            var gotoToken = Check(TypeList.GetTokenBy(GoTo));
            if (gotoToken is null)
                return false;
            
            var labelToken = Check(TypeList.GetTokenBy(Label));
            if (labelToken is null)
                return false;
            
            if (_gotoStorage.ContainsKey(labelToken.Value))
                _tokenPosition = _gotoStorage[labelToken.Value];
            else
            {
                while (true)
                {
                    var label = Check(TypeList.GetTokenBy(Label));
                    if (label is not null && label.Value == labelToken.Value)
                        break;
                    _tokenPosition++;
                }
            }
            return true;
        }

        private bool ExpressionProcessing()
        {
            if (MethodProcessing())
                return true;
            if (ConditionalOperatorProcessing())
                return true;
            if (GotoProcessing())
                return true;
            if (LabelProcessing())
                return true;
            
            return false;
        }

        private async Task Parse()
        {
            var token = _tokenSource.Token;
            while (_tokenPosition < _tokens.Count)
            {
                if (token.IsCancellationRequested)
                    break;
                ExpressionProcessing();
                await Task.Yield();
            }
        }

        private bool ExternalLogicProcessing(Symbol symbol)
        {
            switch (symbol)
            {
                case BooleanOperation b:
                    return _player.Check(b.LeftOperand, b.СomparisonOperator, b.RightOperand);
                default:
                    throw new MissingSymbolClassException();
            }
        }

        private async Task ExternalProcessing(Symbol symbol)
        {
            switch (symbol)
            {
                case MethodOperation m:
                {
                    var argument = m.Argument.Value;
                    if (m.MethodType.Id.Type == Step)
                        await _player.Step(argument);
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
