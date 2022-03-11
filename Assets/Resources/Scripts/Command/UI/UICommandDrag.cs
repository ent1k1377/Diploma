using UnityEngine;
using UnityEngine.EventSystems;

namespace Resources.Scripts.Command.UI
{
    public class UICommandDrag : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform currentTransform;
        
        private GameObject _mainContent;
        private Vector3 _currentPosition;

        private int _totalChild;

        public void OnPointerDown(PointerEventData eventData)
        {
            _currentPosition = currentTransform.position;
            _mainContent = currentTransform.parent.gameObject;
            _totalChild = _mainContent.transform.childCount;
        }

        public void OnDrag(PointerEventData eventData)
        {
            currentTransform.position =
                new Vector3(currentTransform.position.x, eventData.position.y, currentTransform.position.z);

            for (var i = 0; i < _totalChild; i++)
            {
                if (i != currentTransform.GetSiblingIndex())
                {
                    Transform otherTransform = _mainContent.transform.GetChild(i);
                    var distance = (int) Vector3.Distance(currentTransform.position,
                        otherTransform.position);
                    if (distance <= 10)
                    {
                        Vector3 otherTransformOldPosition = otherTransform.position;
                        otherTransform.position = new Vector3(otherTransform.position.x, _currentPosition.y,
                            otherTransform.position.z);
                        currentTransform.position = new Vector3(currentTransform.position.x, otherTransformOldPosition.y,
                            currentTransform.position.z);
                        currentTransform.SetSiblingIndex(otherTransform.GetSiblingIndex());
                        _currentPosition = currentTransform.position;
                    }
                }
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            currentTransform.position = _currentPosition;
        }
    }
}