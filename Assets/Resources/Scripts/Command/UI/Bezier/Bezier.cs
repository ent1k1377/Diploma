using UnityEngine;

namespace Resources.Scripts.Command.UI.Bezier
{
    public static class Bezier
    {
        public static Vector2 GetPoint(Vector2 point0, Vector2 point1, Vector2 point2, Vector2 point3, float t)
        {
            t = Mathf.Clamp01(t);
            var oneMinusT = 1f - t;
            var point = oneMinusT * oneMinusT * oneMinusT * point0 +
                        3f * oneMinusT * oneMinusT * t * point1 +
                        3f * oneMinusT * t * t * point2 +
                        t * t * t * point3;
            return point;
        }
        
        public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
            t = Mathf.Clamp01(t);
            var oneMinusT = 1f - t;
            var firstDerivative = 3f * oneMinusT * oneMinusT * (p1 - p0) +
                                  6f * oneMinusT * t * (p2 - p1) +
                                  3f * t * t * (p3 - p2);
            return firstDerivative;
        }
    }
}
