using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Resources.Scripts
{
    public class DirectionFieldView : MonoBehaviour
    {
        [SerializeField] private Image _directionIcon;
        [SerializeField] private List<Sprite> _directionIcons;
        [SerializeField] private List<DirectionIcon> _directions;
        [SerializeField] private GameObject _directionField;
        
        public void Activate() => _directionField.SetActive(true);

        public void Deactivate() => _directionField.SetActive(false);

        public void SetCurrentActiveDirection(int index)
        {
            DeactivateDirections();
            _directions[index].Image.sprite = _directions[index].Active;
            _directionIcon.sprite = _directionIcons[index];
            Deactivate();
        }

        private void DeactivateDirections()
        {
            foreach (var direction in _directions)
                direction.Image.sprite = direction.NotActive;
        }
    }

    [Serializable]
    public class DirectionIcon
    {
        public Image Image;
        public Sprite Active;
        public Sprite NotActive;
    }
}
