using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LanguageInterpreterLibrary.Symbols;
using Resources.Scripts.Interpreter.Exceptions;
using Resources.Scripts.Interpreter.TokenInfo;
using Resources.Scripts.Interpreter.Types;

namespace Resources.Scripts.Interpreter.Analyzers
{
    using static Convert;
    public class Parser
    {
        private List<Token> Tokens { get; }
        private int _position;
        private readonly Dictionary<string, string> _fields = new();
        
        public readonly ObservableCollection<string> Res = new();

        public Parser(List<Token> tokens)
        {
            Tokens = tokens;
            _position = 0;
            var expressionNode = Parse();
            Res.Clear();
            Start(expressionNode);
        }

        private Token Check(List<TokenId> token)
        {
            if (_position < Tokens.Count)
            {
                Token t = Tokens[_position];
                TokenId exp = token.FirstOrDefault(e => e.Identifier == t.Id.Identifier);
                if (exp == null) return null;
                _position++;
                return t;
            }
            return null;
        }

        private Token Take(List<TokenId> tokens)
        {
            var t = Check(tokens);
            return t ?? throw new TakeTokenException();
        }

        private Symbol TerminalProcessing()
        {
            var token = Check(new List<TokenId> { TypeList.GetTokenBy("Terminal") });
            if (token != null) return new Unary(token, ArithmeticExpressionProcessing());
            throw new TerminalProcessingException();
        }

        private Symbol VariableOrIntegerProcessing()
        {
            var token = Check(new List<TokenId> { TypeList.GetTokenBy("Integer") });
            if (token != null) return new Integer(token);
            token = Check(new List<TokenId> { TypeList.GetTokenBy("Variable") });
            if (token != null) return new Variable(token);
            throw new VariableOrIntegerProcessingException();
        }

        private Symbol BracketsProcessing()
        {
            if (Check(new List<TokenId> {TypeList.GetTokenBy("LeftBracket")}) == null) return VariableOrIntegerProcessing();
            
            Symbol n = ArithmeticExpressionProcessing();
            Take(new List<TokenId> { TypeList.GetTokenBy("RightBracket") });
            return n;
        }

        private Symbol ArithmeticExpressionProcessing()
        {
            Symbol left = BracketsProcessing();
            Token op = Check(new List<TokenId>
                {
                TypeList.GetTokenBy("Subtraction"),
                TypeList.GetTokenBy("Addition"),
                TypeList.GetTokenBy("Multiplication"),
                TypeList.GetTokenBy("Division")
            }
            );

            while (op != null)
            {
                Symbol right = BracketsProcessing();
                left = new Binary(left, op, right);
                op = Check(new List<TokenId>
                {
                    TypeList.GetTokenBy("Subtraction"),
                    TypeList.GetTokenBy("Addition"),
                    TypeList.GetTokenBy("Multiplication"),
                    TypeList.GetTokenBy("Division")
                });
            }
            return left;
        }

        private Symbol ExpressionProcessing()
        {
            var tokens = new List<TokenId> {TypeList.GetTokenBy("Variable")};
            if (Check(tokens) == null)
                return TerminalProcessing();
            _position--;
            var symbol = VariableOrIntegerProcessing();
            var token = Check(new List<TokenId> { TypeList.GetTokenBy("AssignmentOperator") });
            if (token != null)
            {
                Symbol right = ArithmeticExpressionProcessing();
                var binary = new Binary(symbol, token, right);
                return binary;
            }
            throw new ExpressionProcessingException();
        }

        private Symbol Parse()
        {
            AvailableSymbol an = new();
            while (_position < Tokens.Count)
            {
                var symbol = ExpressionProcessing();
                Take(new List<TokenId> { TypeList.GetTokenBy("EndLine") });
                an.Add(symbol);
            }
            return an;
        }

        private int? Start(Symbol symbol)
        {
            Console.WriteLine(symbol);
            if (symbol is Integer i) { return int.Parse(i.Number.Value); }
            if (symbol is Unary u)
            {
                switch (u.Operator.Id.Identifier)
                {
                    case "Terminal":
                        Res.Add($">  {Start(u.Operand)}");
                        return Start(u.Operand);
                }
            }
            if (symbol is Binary b)
            {
                switch (b.Operator.Id.Identifier)
                {
                    case "Addition": return ToInt32(Start(b.Left)) + ToInt32(Start(b.Right));
                    case "Subtraction": return ToInt32(Start(b.Left)) - ToInt32(Start(b.Right));
                    case "Multiplication": return ToInt32(Start(b.Left)) * ToInt32(Start(b.Right));
                    case "Division": return ToInt32(Start(b.Left)) / ToInt32(Start(b.Right));

                    case "AssignmentOperator":
                        int? result = Start(b.Right);
                        Variable variableNode = (Variable)b.Left;
                        _fields[variableNode.Field.Value] = result.ToString();
                        return result;
                }
            }
            if (symbol is Variable v)
            {
                if (!_fields.ContainsKey(v.Field.Value)) throw new VariableNotFoundException();
                return ToInt32(_fields[v.Field.Value]);
            }

            if (symbol is not AvailableSymbol a) throw new InterpreterException();
            
            a.CodeLine.ForEach(codeString => Start(codeString));
            return null;
        }
    }
}
