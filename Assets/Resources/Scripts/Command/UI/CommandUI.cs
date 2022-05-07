using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Resources.Scripts.Command.UI
{
    public class CommandUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private GameObject _initialView;
        [SerializeField] private GameObject _endView;
        [SerializeField] private GameObject _transparentView;

        [SerializeField] [Range(0, 1)] private float _distanceBetweenCommand; 
        
        private Canvas _mainCanvas;
        private RectTransform _rectTransform;
        private Transform _container;
        
        private readonly Vector2 _offset = new(1,1);
        private bool _isOld = true;

        private void Awake()
        {
            _mainCanvas = GetComponentInParent<Canvas>();
            _rectTransform = GetComponent<RectTransform>();
            _container = transform.parent;
        }

        private void Start()
        {
            Initialization();
        }
        
        private void Initialization()
        {
            transform.localPosition = Vector3.zero;
            _rectTransform.sizeDelta = Vector2.zero;
            _initialView.SetActive(true);
            _endView.SetActive(false);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            transform.SetParent(_mainCanvas.transform); 
        }
        
        private void SetTransparentViewIndex(int index)
        {
            _transparentView.SetActive(true);
            _transparentView.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 100);
            _transparentView.transform.SetParent(CommandUIManager.Instance.CommandFieldContainer);
            _transparentView.transform.SetSiblingIndex(index);
        }

        private void SetTransparentView()
        {
            Transform beforeCurrentCommand = null; 
            Transform afterCurrentCommand = null;
            
            for (var i = 0; i < CommandUIManager.Instance.CommandFieldContainer.childCount; i++)
            {
                var otherTransform = CommandUIManager.Instance.CommandFieldContainer.transform.GetChild(i);
                if (otherTransform.position.y - _distanceBetweenCommand >= transform.position.y)
                    beforeCurrentCommand = otherTransform;
                else if (otherTransform.position.y + _distanceBetweenCommand < transform.position.y && afterCurrentCommand is null)
                    afterCurrentCommand = otherTransform;

                if (beforeCurrentCommand is not null && afterCurrentCommand is not null)
                    SetTransparentViewIndex(beforeCurrentCommand.GetSiblingIndex() + 1);
                else if (beforeCurrentCommand is not null)
                    SetTransparentViewIndex(CommandUIManager.Instance.CommandFieldContainer.childCount);
                else if (afterCurrentCommand is not null)
                    SetTransparentViewIndex(0);
            }
        }

        private void ChangeCommandView()
        {
            var position = transform.position - _container.transform.position;
            if (!_isOld || !(_offset.x < position.x) && !(_offset.y < position.y) && !(-_offset.x > position.x) &&
                !(-_offset.y > position.y)) return;
            _initialView.SetActive(false);
            _endView.SetActive(true);
            _isOld = false;
            Instantiate(this, _container);
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            _rectTransform.anchoredPosition += eventData.delta / _mainCanvas.scaleFactor;

            var raycastResults = new List<RaycastResult>();
            CommandUIManager.Instance.GraphicRaycaster.Raycast(eventData, raycastResults);

            if (raycastResults.Count(r => r.gameObject.TryGetComponent(out UICommandField _)) == 1)
                SetTransparentView();
            else
                _transparentView.transform.SetParent(transform);
            ChangeCommandView();
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
                CommandUIManager.Instance.GraphicRaycaster.Raycast(eventData, raycastResults);

                if (raycastResults.Count(r => r.gameObject.TryGetComponent(out UICommandField _)) == 1)
                {
                    transform.SetParent(CommandUIManager.Instance.CommandFieldContainer);
                    _rectTransform.sizeDelta = new Vector2(300, 100);
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
