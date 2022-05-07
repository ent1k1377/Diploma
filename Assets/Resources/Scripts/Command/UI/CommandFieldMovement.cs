using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Resources.Scripts.Command.UI
{
    public class CommandFieldMovement : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
    {
        [SerializeField] [Range(1, 10)] private float _speed;
        
        private List<RaycastResult> _raycastResults = new ();
        private bool _isDrag;
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            _isDrag = true;
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            _raycastResults = new List<RaycastResult>();
            CommandUIManager.Instance.GraphicRaycaster.Raycast(eventData, _raycastResults);
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            _isDrag = false;
        }

        private void Update()
        {
            Move();
        }

        private void Move()
        {
            if (!_isDrag) return;
            
            if (_raycastResults.Count(r => r.gameObject == CommandUIManager.Instance.Content) != 1) return;
            var yPosition = CommandUIManager.Instance.Camera.WorldToViewportPoint(transform.position).y;
            var rectTransform = CommandUIManager.Instance.Content.GetComponent<RectTransform>();

            if (yPosition < 0.25f && Screen.height <= rectTransform.sizeDelta.y - rectTransform.anchoredPosition.y)
                rectTransform.position += new Vector3(0, _speed / 100, 0);
            else if (yPosition > 0.75f && rectTransform.anchoredPosition.y > 1)
                rectTransform.position += new Vector3(0, -_speed / 100, 0);
        }
    }
}
