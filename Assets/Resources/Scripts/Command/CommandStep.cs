using Resources.Scripts.Command.UI;
using UnityEngine;

namespace Resources.Scripts.Command
{
    public class CommandStep : Command
    {
        [SerializeField] private DirectionFieldView _directionFieldView;
        
        public override string GetText()
        {
            return $"Step {_directionFieldView.GetIndexActiveDirection()}\n";
        }
    }
}
