using Resources.Scripts.Interpreter.Types;

namespace Resources.Scripts.Interpreter.TokenInfo
{
    public class TokenId
    {
        public TokenType Type { get; }
        public string SearchPattern { get; }
        public TokenId(TokenType id, string pattern)
        {
            Type = id;
            SearchPattern = pattern;
        }
        public override string ToString() => $"TokenType [ Identifier: {Type} ]";
    }
}
