using System;
using UnityEngine;

namespace Resources.Scripts.Command.UI
{
    public class CommandUIIf : MonoBehaviour
    {
        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = transform.GetComponent<RectTransform>();
        }

        private void Start()
        {
            Initialization();
        }

        private void Initialization()
        {
            _rectTransform.anchoredPosition = new Vector2(0, _rectTransform.position.y / -2);
        }
    }
}
