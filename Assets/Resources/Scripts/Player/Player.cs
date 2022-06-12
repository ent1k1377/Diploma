using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Resources.Scripts.InteractiveObjects;
using Resources.Scripts.Interpreter.TokenInfo;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

namespace Resources.Scripts.Player
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private Sprite _heroIdle;
        [SerializeField] private Sprite _heroUpHands;
        
        [SerializeField] private Tilemap _map;
        [SerializeField] private Tilemap _mapObstacle;
        
        [SerializeField] private Vector3 _offset;

        private InformationBlock _informationBlock;
        private bool _isTaken;

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
            transform.DOMove(_map.CellToWorld(playerCell + target) + _offset, 1f).OnComplete(() => waitMoving = false);
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

        private void Update()
        {
            var position = _map.WorldToCell(transform.position);
            Debug.Log(position);
        }

        public async Task TakeFrom(string direction)
        {
            if (_isTaken) return;
            
            var hit = Physics2D.Raycast(transform.position, (Vector3)GetDirection(direction));
            if (hit.collider != null && hit.transform.TryGetComponent(out InformationBlock informationBlock))
            {
                if (informationBlock.GetPosition() == _map.WorldToCell(transform.position) + GetDirection(direction))
                {
                    _isTaken = true;
                    _informationBlock = informationBlock;
                    GetComponent<SpriteRenderer>().sprite = _heroUpHands;
                    var waitMoving = true;
                    informationBlock.transform.DOMove(transform.position, 0.4f).OnComplete(() => waitMoving = false);
                    while (waitMoving)
                        await Task.Yield();
                    informationBlock.transform.SetParent(transform);
                }
            }
        }
        
        public async Task GiveTo(string direction)
        {
            if (!_isTaken) return;
            
            var hit = Physics2D.RaycastAll(transform.position, (Vector3)GetDirection(direction));
            if (hit.Length == 1 && CheckObstacle(_map.WorldToCell(transform.position) + GetDirection(direction)))
            {
                _isTaken = false;
                GetComponent<SpriteRenderer>().sprite = _heroIdle;
                var waitMoving = true;
                
                var position = _map.WorldToCell(transform.position) + GetDirection(direction);
                var q = _map.CellToWorld(position) + _informationBlock.GetOffset;
                
                _informationBlock.transform.DOMove(q, 0.4f).OnComplete(() => waitMoving = false);
                
                while (waitMoving)
                    await Task.Yield();
                _informationBlock.transform.SetParent(null);
            }
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
