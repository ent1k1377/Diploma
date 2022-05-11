using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Resources.Scripts.Command.UI
{
    public class CommandUI2QWE : MonoBehaviour, IDragHandler
    {
        [SerializeField] private CommandUI _commandUI;
        [SerializeField] private GameObject _initialView;
        [SerializeField] private GameObject _endView;

        private Transform _container;
        
        private void Awake()
        {
            _container = transform.parent;
        }

        private void Start()
        {
            _initialView.SetActive(true);
            _endView.SetActive(false);
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            ChangeCommandView();
        }
        
        private void ChangeCommandView()
        {
            var offset = new Vector2(1, 1);
            var position = transform.position - _container.transform.position;
            if (!_commandUI.IsOld || !(offset.x < position.x) && !(offset.y < position.y) && !(-offset.x > position.x) &&
                !(-offset.y > position.y)) return;
            _initialView.SetActive(false);
            _endView.SetActive(true);
            _commandUI.SetIsOld(false);
            Instantiate(this, _container);
        }
        
        
    }
}
