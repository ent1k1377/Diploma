using Resources.Scripts.Interpreter.TokenInfo;

namespace Resources.Scripts.Interpreter.Symbols
{
    public class BooleanOperation : Symbol
    {
        public Token LeftOperand { get; }
        public Token СomparisonOperator { get; }
        public Token RightOperand { get; }
        
        public BooleanOperation(Token leftOperand, Token comparisonOperator, Token rightOperand)
        {
            LeftOperand = leftOperand;
            СomparisonOperator = comparisonOperator;
            RightOperand = rightOperand;
        }
    }
}