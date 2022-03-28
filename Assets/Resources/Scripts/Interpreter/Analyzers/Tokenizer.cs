using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Resources.Scripts.Interpreter.Exceptions;
using Resources.Scripts.Interpreter.TokenInfo;
using Resources.Scripts.Interpreter.Types;
using static System.String;
using static Resources.Scripts.Interpreter.Types.TokenType;

namespace Resources.Scripts.Interpreter.Analyzers
{
    public class Tokenizer
    {
        private readonly string _sourceCode;
        private int _cursorPosition;
        public List<Token> Tokens { get; private set; }
        public Tokenizer(string code)
        {
            _cursorPosition = 0;
            _sourceCode = code;
            Tokens = new List<Token>();

            #region filteringCommentents

            _sourceCode = Join('\n',
                code.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(e =>
                    {
                        var pos = e.IndexOf("//", StringComparison.Ordinal);
                        return pos == -1 ? e : e[..pos];
                    }));

            #endregion
        }
        public void Analysis()
        {
            while (GetToken()) { }
            Tokens = Tokens
                .Where(t => t.Id.Type != TypeList.GetTokenBy(Comment).Type)
                .Where(t => t.Id.Type != TypeList.GetTokenBy(EmptyOperator).Type)
                .ToList();
        }

        private bool GetToken()
        {
            if (_cursorPosition >= _sourceCode.Length) 
                return false;
            
            foreach (var tokenType in TypeList.Types)
            {
                Regex regex = new($"^{tokenType.SearchPattern}");
                var result = regex.Match(_sourceCode[_cursorPosition..]).Value;

                if (IsNullOrEmpty(result)) 
                    continue;
                Token token = new(tokenType, result, _cursorPosition);
                _cursorPosition += result.Length;
                Tokens.Add(token);
                return true;
            }
            throw new GetTokenException();
        }
    }
}
