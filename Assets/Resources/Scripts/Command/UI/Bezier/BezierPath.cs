using UnityEngine;

namespace Resources.Scripts.Command.UI.Bezier
{
    public class BezierPath : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRenderer;

        public Transform _point0;
        public Transform _point1;
        public Transform _point2;
        public Transform _point3;

        private readonly int _segmentsNumber = 30;
        
        private void Update()
        {
            UpdateCountPoints();
        }

        public void SetPointsGotoLabel(Transform point2, Transform point3)
        {
            _point2 = point2;
            _point3 = point3;
        }
        
        private void UpdateCountPoints()
        {
            _lineRenderer.positionCount = _segmentsNumber;
            
            for (var i = 0; i < _segmentsNumber; i++) {
                var parameter = (float)(i + 1) / _segmentsNumber;
                Vector3 point = Bezier.GetPoint(_point0.position, _point1.position, _point2.position, _point3.position, parameter);
                _lineRenderer.SetPosition(i, point);
            }
        }
        
        private void OnDrawGizmos() 
        {
            var previousPoint = _point0.position;

            Gizmos.color = Color.green;
            Gizmos.DrawLine(_point0.position, _point1.position);
            Gizmos.DrawLine(_point2.position, _point3.position);
            Gizmos.color = Color.red;
            
            for (var i = 0; i < _segmentsNumber + 1; i++) {
                var parameter = (float)i / _segmentsNumber;
                Vector3 point = Bezier.GetPoint(_point0.position, _point1.position, _point2.position, _point3.position, parameter);
                Gizmos.DrawLine(previousPoint, point);
                previousPoint = point;
            }
        }
    }
}
