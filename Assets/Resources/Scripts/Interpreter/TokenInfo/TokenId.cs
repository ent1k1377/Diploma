namespace Resources.Scripts.Interpreter.TokenInfo
{
    public class TokenId
    {
        public string Identifier { get; }
        public string SearchPattern { get; }
        public TokenId(string id, string pattern)
        {
            Identifier = id;
            SearchPattern = pattern;
        }
        public override string ToString() => $"TokenType [ Identifier: {Identifier}]";
    }
}
