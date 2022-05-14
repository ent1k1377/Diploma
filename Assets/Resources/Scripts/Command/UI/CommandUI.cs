using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Resources.Scripts.Command.UI
{
    public class CommandUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private GameObject _transparentView;

        [SerializeField] [Range(0, 1)] private float _distanceBetweenCommand; 
        
        private Canvas _mainCanvas;
        private RectTransform _rectTransform;
        private Transform _container;
        
        private readonly Vector2 _offset = new(1,1);
        private bool _isOld = true;
        private Vector2 _commandSize;

        public bool IsOld => _isOld;

        public void SetIsOld(bool flag)
        {
            _isOld = flag;
        }
        
        private void Awake()
        {
            _mainCanvas = GetComponentInParent<Canvas>();
            _rectTransform = GetComponent<RectTransform>();
            _container = transform.parent;
            _commandSize = _rectTransform.sizeDelta;
        }

        private void Start()
        {
            Initialization();
        }
        
        private void Initialization()
        {
            _rectTransform.anchoredPosition = new Vector2(_commandSize.x / 2, _commandSize.y / -2);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            transform.SetParent(_mainCanvas.transform);
        }
        
        private void SetTransparentViewIndex(int index)
        {
            _transparentView.SetActive(true);
            _transparentView.GetComponent<RectTransform>().sizeDelta = _commandSize;
            _transparentView.transform.SetParent(UICommandManager.Instance.CommandFieldContainer);
            _transparentView.transform.SetSiblingIndex(index);
        }

        private void SetTransparentView()
        {
            Transform beforeCurrentCommand = null; 
            Transform afterCurrentCommand = null;
            
            for (var i = 0; i < UICommandManager.Instance.CommandFieldContainer.childCount; i++)
            {
                var otherTransform = UICommandManager.Instance.CommandFieldContainer.transform.GetChild(i);
                if (otherTransform.position.y - _distanceBetweenCommand >= transform.position.y)
                    beforeCurrentCommand = otherTransform;
                else if (otherTransform.position.y + _distanceBetweenCommand < transform.position.y && afterCurrentCommand is null)
                    afterCurrentCommand = otherTransform;

                if (beforeCurrentCommand is not null && afterCurrentCommand is not null)
                    SetTransparentViewIndex(beforeCurrentCommand.GetSiblingIndex() + 1);
                else if (beforeCurrentCommand is not null)
                    SetTransparentViewIndex(UICommandManager.Instance.CommandFieldContainer.childCount);
                else if (afterCurrentCommand is not null)
                    SetTransparentViewIndex(0);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            _rectTransform.anchoredPosition += eventData.delta / _mainCanvas.scaleFactor;

            var raycastResults = new List<RaycastResult>();
            UICommandManager.Instance.GraphicRaycaster.Raycast(eventData, raycastResults);

            if (raycastResults.Count(r => r.gameObject.TryGetComponent(out UICommandField _)) == 1)
                SetTransparentView();
            else
                _transparentView.transform.SetParent(transform);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_isOld)
            {
                transform.SetParent(_container);
                transform.localPosition = Vector3.zero;
                return;
            }
            if (eventData.pointerEnter is not null)
            {
                var raycastResults = new List<RaycastResult>();
                UICommandManager.Instance.GraphicRaycaster.Raycast(eventData, raycastResults);
                if (raycastResults.Count(r => r.gameObject.TryGetComponent(out UICommandField _)) == 1)
                {

                    foreach (var r in raycastResults)
                    {
                        if (r.gameObject.TryGetComponent(out UICommandField uiCommandField))
                            uiCommandField.GetChild().ForEach(Debug.Log);
                    }

                    transform.SetParent(UICommandManager.Instance.CommandFieldContainer);
                    _rectTransform.sizeDelta = _commandSize;
                    transform.SetSiblingIndex(_transparentView.transform.GetSiblingIndex());
                    _transparentView.SetActive(false);
                    _transparentView.transform.SetParent(transform);
                    return;
                }
            }
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            Destroy(_transparentView);
        }
    }
}
