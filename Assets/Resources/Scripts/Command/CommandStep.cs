using UnityEngine;

namespace Resources.Scripts.Command
{
    public class CommandStep : Command
    {
        public void Step(string direction)
        {
            Debug.Log($"Step: {direction}");
        }
    }
}
