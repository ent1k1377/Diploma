using Resources.Scripts.Interpreter.TokenInfo;

namespace Resources.Scripts.Interpreter.Symbols
{
    public class Unary : ArithmeticOperation
    {
        public Token Operator { get; }
        public Symbol Operand { get; }
        public Unary(Token op, Symbol operand)
        {
            Operator = op;
            Operand = operand;
        }
    }
}
