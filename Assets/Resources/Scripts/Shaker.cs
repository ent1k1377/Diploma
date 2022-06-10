using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Resources.Scripts
{
    public class Shaker : MonoBehaviour
    {
        [SerializeField] private Vector2 _offset;
        [Range(1, 10)] [SerializeField] private float _duration;
        
        private Vector3 _startingPosition;

        private void Start()
        {
            _startingPosition = transform.position;
            _ = Shake();
        }

        private async Task Shake()
        {
            while (true)
            {
                var nextPosition = _startingPosition + new Vector3(_offset.x * Random.Range(-1f, 1f), _offset.y * Random.Range(-1f, 1f), 0);
                var waitShaking = true;
                transform.DOMove(nextPosition, _duration).OnComplete(() => waitShaking = false);
                while (waitShaking)
                    await Task.Yield();
            }
        }

        private void OnDisable()
        {
            transform.DOKill();
        }
    }
}
