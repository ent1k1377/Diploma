using Resources.Scripts.Command.UI;

namespace Resources.Scripts.Command
{
    public class CommandGoto : Command
    {
        public override string GetText()
        {
            return $"goto {GetComponent<CommandUIGoto>().GetLabelValue()}:\n";
        }
    }
}