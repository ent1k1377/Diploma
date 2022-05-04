using UnityEngine;
using UnityEngine.EventSystems;

namespace Resources.Scripts.Command.UI
{
    public class UICommandDrag : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        private GameObject _mainContent;
        private Vector3 _currentPosition;

        private int _totalChild;

        public void OnPointerDown(PointerEventData eventData)
        {
            _currentPosition = transform.position;
            _mainContent = transform.parent.gameObject;
            _totalChild = _mainContent.transform.childCount;
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = new Vector3(transform.position.x, eventData.position.y, transform.position.z);

            for (var i = 0; i < _totalChild; i++)
            {
                if (i != transform.GetSiblingIndex())
                {
                    var otherTransform = _mainContent.transform.GetChild(i);
                    var distance = (int) Vector3.Distance(transform.position,
                        otherTransform.position);
                    if (distance <= 10)
                    {
                        var otherTransformOldPosition = otherTransform.position;
                        otherTransform.position = new Vector3(otherTransform.position.x, _currentPosition.y, otherTransform.position.z);
                        transform.position = new Vector3(transform.position.x, otherTransformOldPosition.y, transform.position.z);
                        transform.SetSiblingIndex(otherTransform.GetSiblingIndex());
                        _currentPosition = transform.position;
                    }
                }
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            transform.position = _currentPosition;
        }
    }
}