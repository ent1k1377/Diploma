using Resources.Scripts.Interpreter;
using Resources.Scripts.Interpreter.TokenInfo;

namespace Resources.Scripts.Interpreter.Symbols
{
    public class Variable : Symbol
    {
        public Token Field { get; }
        public Variable(Token field) => Field = field;
    }
}

