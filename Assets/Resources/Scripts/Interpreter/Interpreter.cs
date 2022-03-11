using Resources.Scripts.Interpreter.Analyzers;
using UnityEngine;

namespace Resources.Scripts.Interpreter
{
    public class Interpreter : MonoBehaviour
    {
        private void Start()
        {
            var sourceCode =
                @"
                like ::= 1; 
                subscribe ::= like;
                Print [like + subscribe] * 2 + 2;
                ";
            
            Tokenizer lexer = new(sourceCode);
            lexer.Analysis();
            
            var parser = new Parser(lexer.Tokens);
            foreach (var item in parser.Res) Debug.Log($"{item}");
        }
    }
}
