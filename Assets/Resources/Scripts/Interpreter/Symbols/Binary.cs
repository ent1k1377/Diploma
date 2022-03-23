using Resources.Scripts.Interpreter.TokenInfo;

namespace Resources.Scripts.Interpreter.Symbols
{
    public class Binary : ArithmeticOperation
    {
        public Token Operator { get; }
        public Symbol Left { get; }
        public Symbol Right { get; }
        public Binary(Symbol left, Token op, Symbol right)
        {
            Operator = op;
            Left = left;
            Right = right;
        }
    }
}
