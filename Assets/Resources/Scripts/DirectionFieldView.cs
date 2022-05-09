using System;
using System.Collections.Generic;
using DG.Tweening;
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

        private readonly Vector3 _minScaleDirectionField = new(0.1f, 0.1f, 0.1f);
        private readonly float _durationScale = 0.15f;

        public void Activate()
        {
            _directionField.transform.localScale = _minScaleDirectionField;
            _directionField.SetActive(true);
            _directionField.transform.DOScale(new Vector3(1, 1, 1), _durationScale);
        }

        public void Deactivate()
        {
            _directionField.transform.DOScale(_minScaleDirectionField, _durationScale).OnComplete(() => _directionField.SetActive(false));
        }

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
