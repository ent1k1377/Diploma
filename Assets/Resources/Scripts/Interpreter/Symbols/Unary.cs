using Resources.Scripts.Interpreter.TokenInfo;

namespace LanguageInterpreterLibrary.Symbols
{
    public class Unary : ArithmeticOperation
    {
        public Token Operator { get; set; }
        public Symbol Operand { get; set; }
        public Unary(Token op, Symbol operand)
        {
            Operator = op;
            Operand = operand;
        }
    }
}
