using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Resources.Scripts.InteractiveObjects
{
    public class InformationBlock : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private int number;

        private void Start()
        {
            _text.text = number.ToString();
        }
    }
}
