using System;
using UnityEngine;

namespace Resources.Scripts.Command.UI
{
    public class CommandUIGotoLabel : MonoBehaviour
    {
        [SerializeField] private CommandUI _commandUI;
        [SerializeField] private CommandUIGoto _goto;
        [SerializeField] private Transform _point0;
        [SerializeField] private Transform _point1;

        [SerializeField] private CommandUIGoto _gotoCopy;

        public Transform Point0 => _point0;
        public Transform Point1 => _point1;

        private void Start()
        {
            _commandUI.SetIsOld(false);
        }

        public void SetGoto(CommandUIGoto commandUIGoto)
        {
            _gotoCopy = commandUIGoto;
        }
        
        private void OnDestroy()
        {
            if (_gotoCopy != null)
                Destroy(_gotoCopy.gameObject);
        }
    }
}
