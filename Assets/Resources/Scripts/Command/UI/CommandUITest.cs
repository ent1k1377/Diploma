using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Resources.Scripts.Command.UI
{
    public class CommandUITest : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private GameObject _initialView;
        [SerializeField] private GameObject _endView;
        [SerializeField] private GameObject _transparentView;

        [SerializeField] private Transform _commandFieldContainer;
        [SerializeField] private GraphicRaycaster _graphicRaycaster;
        [SerializeField] private GameObject _plug;

        [SerializeField] [Range(1, 10)] private float _distanceBetweenCommand; 
        
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
            Destroy(_plug);
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
        
        public void OnDrag(PointerEventData eventData)
        {
            _rectTransform.anchoredPosition += eventData.delta / _mainCanvas.scaleFactor;

            Transform beforeCurrentCommand = null; 
            Transform afterCurrentCommand = null;

            var raycastResults = new List<RaycastResult>();
            _graphicRaycaster.Raycast(eventData, raycastResults);
            if (raycastResults.Count(r => r.gameObject.TryGetComponent(out UICommandField _)) == 1)
            {
                for (var i = 0; i < _commandFieldContainer.childCount; i++)
                {
                    var otherTransform = _commandFieldContainer.transform.GetChild(i);
                    if (otherTransform.position.y >= transform.position.y)
                        beforeCurrentCommand = otherTransform;
                    else if (afterCurrentCommand is null && otherTransform.position.y < transform.position.y)
                        afterCurrentCommand = otherTransform;

                    if (beforeCurrentCommand is not null && afterCurrentCommand is not null)
                    {
                        _transparentView.SetActive(true);
                        _transparentView.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 100);
                        _transparentView.transform.SetParent(_commandFieldContainer);
                        _transparentView.transform.SetSiblingIndex(beforeCurrentCommand.GetSiblingIndex() + 1);
                    }
                    else if (beforeCurrentCommand is not null)
                    {
                        _transparentView.SetActive(true);
                        _transparentView.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 100);
                        _transparentView.transform.SetParent(_commandFieldContainer);
                        _transparentView.transform.SetSiblingIndex(_commandFieldContainer.childCount);
                    }
                    else if (afterCurrentCommand is not null)
                    {
                        _transparentView.SetActive(true);
                        _transparentView.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 100);
                        _transparentView.transform.SetParent(_commandFieldContainer);
                        _transparentView.transform.SetSiblingIndex(0);
                    }
                }
            }
            else
            {
                var position = transform.position - _container.transform.position;
                if (!_isOld || !(_offset.x < position.x) && !(_offset.y < position.y) && !(-_offset.x > position.x) &&
                                !(-_offset.y > position.y)) return;
                _initialView.SetActive(false);
                _endView.SetActive(true);
                _isOld = false;
                Instantiate(this, _container);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_isOld)
            {
                transform.SetParent(_container);
                transform.localPosition = Vector3.zero;
            }
            else
            {
                if (eventData.pointerEnter is not null)
                {
                    var raycastResults = new List<RaycastResult>();
                    _graphicRaycaster.Raycast(eventData, raycastResults);

                    if (raycastResults.Count(r => r.gameObject.TryGetComponent(out UICommandField _)) == 1)
                    {
                        transform.SetParent(_commandFieldContainer);
                        _rectTransform.sizeDelta = new Vector2(300, 100);
                        transform.SetSiblingIndex(_transparentView.transform.GetSiblingIndex());
                        _transparentView.SetActive(false);
                        _transparentView.transform.SetParent(transform);
                        return;
                    }
                }
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            Destroy(_transparentView);
        }
    }
}
