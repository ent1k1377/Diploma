using UnityEngine;

namespace Resources.Scripts.Command
{
    public class Command : MonoBehaviour
    {
        public virtual string GetText()
        {
            return "";
        }
    }
}
