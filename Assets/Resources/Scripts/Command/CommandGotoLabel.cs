using Resources.Scripts.Command.UI;

namespace Resources.Scripts.Command
{
    public class CommandGotoLabel : Command
    {
        public override string GetText()
        {
            return $"{GetComponent<CommandUIGotoLabel>().GetLabelValue()}:\n";
        }
    }
}