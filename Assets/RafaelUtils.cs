
using UnityEngine.InputSystem;
using UnityEngine;

namespace Utils
{
    public static class RafaelUtils
    {
        public static bool LineIntersection(Vector3 vec1Start, Vector3 vec1Dir, Vector3 vec2Start, Vector3 vec2Dir, out Vector3 intersection)
        {
            intersection = Vector3.zero;

            // Solve for t1 and t2 using the cross-product method
            Vector3 crossD1D2 = Vector3.Cross(vec1Dir, vec2Dir);
            float determinant = crossD1D2.sqrMagnitude;

            // Check if lines are parallel (determinant close to 0)
            if (determinant < Mathf.Epsilon)
                return false; // No intersection, lines are parallel

            // Calculate the parameter t1
            Vector3 diff = vec2Start - vec1Start;
            float t1 = Vector3.Dot(Vector3.Cross(diff, vec2Dir), crossD1D2) / determinant;

            // Compute the intersection point
            intersection = vec1Start + t1 * vec1Dir;
            return true;
        }

        public static bool TryRaycastObject(out RaycastHit hit)
        {
            Vector3 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                return true;

            return false;
        }

        public static GameObject CreateSphere(Vector3 position, string name, Transform parent = null, float scale = .25f)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localScale = scale * Vector3.one;
            sphere.transform.position = position;
            sphere.transform.name = name;
            sphere.transform.parent = parent;
            return sphere;
        }
        public static float Vector3ToAngle360(Vector3 from, Vector3 to)
        {
            float angle = Vector3.Angle(from, to);
            Vector3 cross = Vector3.Cross(from, to);
            return cross.y > 0 ? angle : 360 - angle;
        }
        public static OrientedPoint[] GetPointsArround(Vector3 point, Vector3 origin, float endAngle, int pointCount)
        {
            OrientedPoint[] points = new OrientedPoint[pointCount];
            Vector3 vector = point - origin;
            float angleStep = endAngle / (pointCount - 1); // Step size

            for (int i = 0; i < pointCount; i++)
            {
                if (i != 0)
                {
                    // Only apply rotation if it's not the first point
                    Quaternion rotation = Quaternion.AngleAxis(angleStep, Vector3.up);
                    vector = rotation * vector;
                }

                Vector3 tangent = Vector3.Cross(vector, Vector3.up);
                Quaternion orientation = Quaternion.LookRotation(tangent, Vector3.up);

                points[i] = new OrientedPoint(vector + origin, orientation);
            }

            return points;
        }
    }

}
