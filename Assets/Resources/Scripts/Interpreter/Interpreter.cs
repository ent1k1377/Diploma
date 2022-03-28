using Resources.Scripts.Interpreter.Analyzers;
using UnityEngine;

namespace Resources.Scripts.Interpreter
{
    public class Interpreter
    {
        public void Run(string code, Player player)
        {
            Tokenizer lexer = new(code);
            lexer.Analysis();
            //lexer.Tokens.ForEach(Debug.Log);
            _ = new Parser(lexer.Tokens, player);
        }
    }
}
