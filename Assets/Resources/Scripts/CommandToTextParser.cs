using Resources.Scripts.Command.UI;
using Resources.Scripts.Interpreter.Analyzers;
using UnityEngine;

namespace Resources.Scripts
{
    public class CommandToTextParser : MonoBehaviour
    {
        [SerializeField] private Player _player;
        
        public void OnStart()
        {
            Parse();
        }

        private void Parse()
        {
            var valueLabels = "qwertyuiopasdfghjklzxcvbnm".ToCharArray();
            var indexValueLabels = 0;
            
            var text = "";
            var commands = transform.GetComponentsInChildren<Command.Command>();
            
            //_commands.ToList().ForEach(Debug.Log);
            foreach (var command in commands)
            {
                if (command.TryGetComponent(out CommandUIGoto commandUIGoto))
                {
                    if (commandUIGoto.GetLabelValue() == '`')
                        commandUIGoto.SetLabelValue(valueLabels[indexValueLabels]);
                    indexValueLabels++;
                }
                else if (command.TryGetComponent(out CommandUIGotoLabel commandUIGotoLabel))
                {
                    if (commandUIGotoLabel.GetLabelValue() == '`')
                        commandUIGotoLabel.SetLabelValue(valueLabels[indexValueLabels]);
                    indexValueLabels++;
                }
                    


                text += command.GetText();
                
            }
            Debug.Log(text);
            Run(text, _player);
        }

        private void Run(string code, Player player)
        {
            Tokenizer lexer = new(code);
            lexer.Analysis();
            _ = new Parser(lexer.Tokens, player);
        }
        
        private void FileWrite()
        {
        }
    }
}
