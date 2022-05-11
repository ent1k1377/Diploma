using System.Collections.Generic;
using System.Linq;
using Resources.Scripts.Command.UI.Bezier;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Resources.Scripts.Command.UI
{
    public class CommandUIGoto : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        [SerializeField] private CommandUI _commandUI;
        [SerializeField] private CommandUIGotoLabel _gotoLabel;

        [SerializeField] private Transform _point0;
        [SerializeField] private Transform _point1;

        [SerializeField] private CommandUIGotoLabel _gotoLabelCopy;
        private BezierPath _bezierPath;
        private bool _isOld = true;
        private Transform _container;
        
        public Transform Point0 => _point0;
        public Transform Point1 => _point1;
        
        private void Awake()
        {
            _bezierPath = GetComponent<BezierPath>();
            _container = transform.parent;
        }

        private void ChangeCommandView()
        {
            var offset = new Vector2(1, 1);
            var position = transform.position - _container.transform.position;
            if (!_commandUI.IsOld || !(offset.x < position.x) && !(offset.y < position.y) && !(-offset.x > position.x) &&
                !(-offset.y > position.y)) return;

            _commandUI.SetIsOld(false);
            Instantiate(this, _container);
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            ChangeCommandView();
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            if (_isOld)
            {
                var raycastResults = new List<RaycastResult>();
                UICommandManager.Instance.GraphicRaycaster.Raycast(eventData, raycastResults);
                if (raycastResults.Count(r => r.gameObject.TryGetComponent(out UICommandField _)) == 1)
                {
                    _gotoLabelCopy = Instantiate(_gotoLabel, transform.parent);
                    _gotoLabelCopy.SetGoto(this);
                    _bezierPath.SetPointsGotoLabel(_gotoLabelCopy.Point0, _gotoLabelCopy.Point1);
                    _isOld = false;
                    _bezierPath.enabled = true;
                    _gotoLabelCopy.transform.SetParent(UICommandManager.Instance.CommandFieldContainer);
                    _gotoLabelCopy.gameObject.SetActive(true);
                    _gotoLabelCopy.transform.SetSiblingIndex(transform.GetSiblingIndex());
                }
            }
        }
        
        private void OnDestroy()
        {
            if (_gotoLabelCopy != null)
                Destroy(_gotoLabelCopy.gameObject);
        }
    }
}
