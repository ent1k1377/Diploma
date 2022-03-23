namespace Resources.Scripts.Interpreter.TokenInfo
{
    public class Token
    {
        public TokenId Id { get; }
        public string Value { get; }
        public int Position { get; }
        
        public Token(TokenId type, string value, int position)
        {
            Id = type;
            Value = value;
            Position = position;
        }
        public override string ToString()
            => $"TOKEN [ Value: {Value}] Position: {Position}\n   Id: {Id} ";
    }
}
