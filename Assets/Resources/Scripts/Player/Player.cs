using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
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
        
        private readonly Vector3 _offset = new(0.5f, 0.5f, 0);

        public event UnityAction Destroyed;

        private void OnDestroy()
        {
            Destroyed?.Invoke();
        }

        private async UniTask Move(Vector3Int target)
        {
            var playerCell = _map.WorldToCell(transform.position);
            var newPosition = playerCell + target;
            if (CheckObstacle(newPosition))
            {
                var waitMoving = true;
                transform.DOMove(_map.CellToWorld(playerCell) + target + _offset, 2f).OnComplete(() => waitMoving = false);
                await UniTask.Delay(2000);
                // while (waitMoving)
                // {
                //     Debug.Log(899);
                //     await UniTask.DelayFrame(1);
                // }
            }
        }

        public async UniTask Step(string direction)
        {
            await UniTask.WhenAll(Move(GetDirection(direction)));
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
