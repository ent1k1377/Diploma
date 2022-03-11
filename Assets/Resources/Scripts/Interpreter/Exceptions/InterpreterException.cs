using System;

namespace Resources.Scripts.Interpreter.Exceptions
{
    public class InterpreterException : Exception
    {
        public int ErrorPosition { get; set; }
    }
}
