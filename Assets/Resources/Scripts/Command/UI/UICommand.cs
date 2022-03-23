using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Resources.Scripts.Command.UI
{
    public class UICommand : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private UIContainerCommand _uiContainerCommand;
        [SerializeField] private Transform _commandSet;
        
        private Canvas _mainCanvas;
        private CanvasGroup _canvasGroup;
        private RectTransform _rectTransform;

        private bool we = true;
        private void Awake()
        {
            _mainCanvas = GetComponentInParent<Canvas>();
            _canvasGroup = GetComponentInParent<CanvasGroup>();
            _rectTransform = GetComponent<RectTransform>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _canvasGroup.blocksRaycasts = false;
            
            if (we)
            {
                transform.SetParent(_commandSet);
                _uiContainerCommand.gameObject.SetActive(false);
            }
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            _rectTransform.anchoredPosition += eventData.delta / _mainCanvas.scaleFactor;
            int x = 60;
            if (we && (x < transform.localPosition.x || x < transform.localPosition.z || 
                       -x > transform.localPosition.x || -x > transform.localPosition.z))
            {
                we = false;
                _uiContainerCommand.gameObject.SetActive(true);
            }
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            _uiContainerCommand.SpawnUICommand();
            if (eventData.pointerEnter.gameObject.TryGetComponent(out UICommandField uiCommandField))
            {
                _canvasGroup.blocksRaycasts = true;
            }
            else
            {
                Destroy(gameObject);
            }

        }
    }
}
