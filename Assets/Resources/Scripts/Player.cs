using System.Collections.Generic;
using System.Threading.Tasks;
using Resources.Scripts.Interpreter.TokenInfo;
using Resources.Scripts.Interpreter.Types;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Resources.Scripts
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private Tilemap _map;
        [SerializeField] private Tilemap _mapObstacle;

        private readonly Vector3 _offset = new(0.5f, 0.5f, 0);

        private void Start()
        {
            var sourceCode =
                @"
                like ::= 1; 
                subscribe ::= like;
                Print 2 + 2;
                Print2;
                ";

            var sourceCode2 =
                @"
                TakeFrom S
                Step SW
                TakeFrom S
                GiveTo S
                GiveTo E
                ";
            
            var sourceCode3 =
                @"
                TakeFrom W
                TakeFrom E
                if S == something:
                    TakeFrom S
                    TakeFrom E
                    TakeFrom W
                    GiveTo S
                    GiveTo E
                else:
                    TakeFrom E
                endif
                ";
            
            var interpreter = new Interpreter.Interpreter();
            interpreter.Run(sourceCode3, this);
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

        public async Task Step(string direction)
        {
            await Task.Delay(5000);
            Debug.Log($"Step: {direction}");
        }
        
        public async Task TakeFrom(string direction)
        {
            await Task.Delay(5000);
            Debug.Log($"TakeFrom: {direction}");
        }
        
        public async Task GiveTo(string direction)
        {
            await Task.Delay(5000);
            Debug.Log($"GiveTo: {direction}");

        }

        public async Task<bool> Check(Token leftOperand, Token comparisonOperator, Token rightOperand)
        {
            Debug.Log(comparisonOperator.Value == "==");
            await Task.Delay(1000);
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
