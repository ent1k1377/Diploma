using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Resources.Scripts.Interpreter.TokenInfo;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

namespace Resources.Scripts
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private Tilemap _map;
        [SerializeField] private Tilemap _mapObstacle;

        private Interpreter.Interpreter _interpreter;
        private readonly Vector3 _offset = new(0.5f, 0.5f, 0);

        public event UnityAction Destroyed;

        private void OnDestroy()
        {
            Destroyed?.Invoke();
        }
        
        private void Start()
        {
            var sourceCode3 =
                @"
                if S != something:
                    if S == something or S == something:
                        GiveTo W
                    else:
                        GiveTo S
                    endif
                    GiveTo E
                else:
                    if S != something:
                        GiveTo W
                    else:
                        if S == something:
                            GiveTo W
                        endif   
                        GiveTo E
                    endif
                    GiveTo E
                endif
                ";
            
            var sourceCode4 =
                @"
                GiveTo S
                goto a:
                
                GiveTo E
                GiveTo W
                a:
                GiveTo N
                ";
            
            
            var sourceCode5 =
                @"
                b:
                if S == something:
                    GiveTo W
                    goto b:
                else:
                    GiveTo S
                endif
                
                GiveTo N
                ";

            _interpreter = new Interpreter.Interpreter();
            _interpreter.Run(sourceCode4, this);
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

        public void Step(string direction)
        {
            Debug.Log($"Step: {direction}");
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

        private Vector2Int GetDirection(string direction)
        {
            Dictionary<char, int> directionQ = new()
            {
                {'N', 1},
                {'E', 1},
                {'S', -1},
                {'W', -1},
                {'C', 0}
            };
            return new Vector2Int(directionQ[direction[0]], directionQ[direction[1]]);
        }
    }
}
