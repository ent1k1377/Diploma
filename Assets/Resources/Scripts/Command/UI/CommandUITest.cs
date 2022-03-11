using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Resources.Scripts.Command.UI
{
    public class CommandUITest : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private GameObject _initialView;
        [SerializeField] private GameObject _endView;

        [SerializeField] private Transform _commandFieldContainer;
        
        private readonly Vector2 _offset = new(1,1);
        private bool _isNew = true;
        
        private Transform _container;

        private Canvas _mainCanvas;
        private CanvasGroup _canvasGroup;
        private RectTransform _rectTransform;

        private void Awake()
        {
            _mainCanvas = GetComponentInParent<Canvas>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _rectTransform = GetComponent<RectTransform>();
            _container = transform.parent;
        }

        private void Start()
        {
            Initialization();
        }
        
        private void Initialization()
        {
            _canvasGroup.blocksRaycasts = true;
            transform.localPosition = Vector3.zero;
            _rectTransform.sizeDelta = Vector2.zero;
            _initialView.SetActive(true);
            _endView.SetActive(false);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _canvasGroup.blocksRaycasts = false;
            transform.SetParent(_mainCanvas.transform);
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            _rectTransform.anchoredPosition += eventData.delta / _mainCanvas.scaleFactor;
            
            var position = transform.position - _container.transform.position;
            if (_isNew && (_offset.x < position.x || 
                           _offset.y < position.y || 
                           -_offset.x > position.x || 
                           -_offset.y > position.y))
            {
                _initialView.SetActive(false);
                _endView.SetActive(true);
                _isNew = false;
                Instantiate(this, _container);
            }
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            _canvasGroup.blocksRaycasts = true;
            if (_isNew)
            {
                transform.SetParent(_container);
                transform.localPosition = Vector3.zero;
            }
            else
            {
                if (eventData.pointerEnter is not null && eventData.pointerEnter.gameObject.TryGetComponent(out UICommandField uiCommandField))
                {
                    transform.SetParent(_commandFieldContainer);
                    //_rectTransform.sizeDelta = new Vector2(0, 100);
                    //_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100);
                }
                else
                    Destroy(gameObject);
            }
        }
    }
}
