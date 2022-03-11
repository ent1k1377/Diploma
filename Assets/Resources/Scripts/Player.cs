using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Resources.Scripts
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private Tilemap _map;
        [SerializeField] private Tilemap _mapObstacle;

        private Camera _camera;
        private readonly Vector3 _offset = new Vector3(0.5f, 0.5f, 0);
        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            InputKeyboard();
        }

        private void InputKeyboard()
        {
            if (Input.GetKeyDown(KeyCode.A))
                Move(new Vector3Int(-1, 0, 0));
            else if (Input.GetKeyDown(KeyCode.D))
                Move(new Vector3Int(1, 0, 0));
            else if (Input.GetKeyDown(KeyCode.W))
                Move(new Vector3Int(0, 1, 0));
            else if (Input.GetKeyDown(KeyCode.S))
                Move(new Vector3Int(0, -1, 0));
        }

        private void Move(Vector3Int target)
        {
            var playerCell = _map.WorldToCell(transform.position);
            var newPosition = playerCell + target;
            if (CheckObstacle(newPosition))
                transform.position = _map.CellToWorld(playerCell) + target + _offset;

        }

        private bool CheckObstacle(Vector3Int target)
        {
            return _mapObstacle.GetTile(target) is null;
        }
    }
}
