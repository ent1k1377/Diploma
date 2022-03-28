using System;
using Resources.Scripts.Interpreter.TokenInfo;

namespace Resources.Scripts.Interpreter.Exceptions
{
    public class MissingMatchingTokenException : Exception
    {
        public MissingMatchingTokenException(Token token) : base($"Token: [ {token.Id.Type} ], Position: [ {token.Position} ]")
        {
        }
    }
}