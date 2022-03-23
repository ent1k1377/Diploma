using Resources.Scripts.Interpreter.TokenInfo;

namespace Resources.Scripts.Interpreter.Symbols
{
    public class Integer : Symbol
    {
        public Token Number { get; set; }
        public Integer(Token number) => this.Number = number;
    }
}
