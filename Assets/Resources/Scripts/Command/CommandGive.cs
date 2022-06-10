using Resources.Scripts.Command.UI;
using UnityEngine;

namespace Resources.Scripts.Command
{
    public class CommandGive : Command
    {
        [SerializeField] private DirectionFieldView _directionFieldView;
        
        public override string GetText()
        {
            return $"GiveTo {_directionFieldView.GetIndexActiveDirection()}\n";
        }
    }
}
