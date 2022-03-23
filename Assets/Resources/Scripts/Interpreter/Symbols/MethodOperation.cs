using Resources.Scripts.Interpreter.TokenInfo;

namespace Resources.Scripts.Interpreter.Symbols
{
    public class MethodOperation : Symbol
    {
        public Token MethodType { get; }
        public Token Argument { get; }
        
        public MethodOperation(Token method, Token argument)
        {
            MethodType = method;
            Argument = argument;
        }
    }
}