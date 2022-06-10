using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Resources.Scripts.Interpreter.TokenInfo;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

namespace Resources.Scripts.Player
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private Tilemap _map;
        [SerializeField] private Tilemap _mapObstacle;
        
        [SerializeField] private Vector3 _offset;

        public event UnityAction Destroyed;

        private void Start()
        {
            var position = _map.WorldToCell(transform.position);
            transform.position = _map.CellToWorld(position) + _offset;
        }

        private void OnDestroy()
        {
            Destroyed?.Invoke();
            transform.DOKill();
        }

        private async Task Move(Vector3Int target)
        {
            var playerCell = _map.WorldToCell(transform.position);
            var newPosition = playerCell + target;
            if (!CheckObstacle(newPosition)) return;
            
            var waitMoving = true;
            transform.DOMove(_map.CellToWorld(playerCell) + target + _offset, 1f).OnComplete(() => waitMoving = false);
            while (waitMoving)
                await Task.Yield();
        }

        public async Task Step(string direction)
        {
            await Move(GetDirection(direction));
        }
        
        private bool CheckObstacle(Vector3Int target)
        {
            return _mapObstacle.GetTile(target) is null;
        }

        public void TakeFrom(string direction)
        {
            Debug.Log($"TakeFrom: {direction}");
        }
        
        public void GiveTo(string direction)
        {
            Debug.Log($"GiveTo: {direction}");
        }

        public bool Check(Token leftOperand, Token comparisonOperator, Token rightOperand)
        {
            return comparisonOperator.Value == "==";
        }

        private Vector3Int GetDirection(string direction)
        {
            Dictionary<char, int> directionQ = new()
            {
                {'N', 1},
                {'E', 1},
                {'S', -1},
                {'W', -1},
                {'C', 0}
            };
            return new Vector3Int(directionQ[direction[1]], directionQ[direction[0]], 0);
        }
    }
}
