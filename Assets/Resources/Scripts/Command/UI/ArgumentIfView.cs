using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Resources.Scripts.Command.UI
{
    public class ArgumentIfView : MonoBehaviour
    {
        [SerializeField] private ArgumentIcon[] _argumentIcons;
        [SerializeField] private Image _mainIcon;
        
        public void SetIcon(string name)
        {
            var sprite = _argumentIcons.First(a => a.Name == name).Icon;
            _mainIcon.sprite = sprite;
        }
    }

    [Serializable]
    public class ArgumentIcon
    {
        public string Name;
        public Sprite Icon;
    }
}
