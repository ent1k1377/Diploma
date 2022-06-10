using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Resources.Scripts.InteractiveObjects
{
    public class InformationBlock : MonoBehaviour
    {
        [SerializeField] private Tilemap _map;
        [SerializeField] private Vector3 _offset;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private int number;

        private void Start()
        {
            _text.text = number.ToString();
            var position = _map.WorldToCell(transform.position);
            transform.position = _map.CellToWorld(position) + _offset;
        }
    }
}
