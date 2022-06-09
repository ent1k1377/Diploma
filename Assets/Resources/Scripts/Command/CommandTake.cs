using Resources.Scripts.Command.UI;
using UnityEngine;

namespace Resources.Scripts.Command
{
    public class CommandTake : Command
    {
        [SerializeField] private DirectionFieldView _directionFieldView;
        
        public override string GetText()
        {
            return $"TakeFrom {_directionFieldView.GetIndexActiveDirection()}\n";
        }
    }
}
