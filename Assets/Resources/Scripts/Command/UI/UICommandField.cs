using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Resources.Scripts.Command.UI
{
    public class UICommandField : MonoBehaviour
    {
        public List<CommandUI> GetChild()
        {
            return transform.GetComponentsInChildren<CommandUI>().ToList();
        }
    }
}
