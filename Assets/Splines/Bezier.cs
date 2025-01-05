using UnityEngine;
namespace Spline
{
    public static class Bezier
    {
        public static OrientedPoint GetOP(
           Transform startNode,
           Vector3 startControlNode,
           Transform endNode,
           Vector3 endControlNode,
           float t)
        {
            Vector3 a = Vector3.Lerp(startNode.position, startControlNode, t);
            Vector3 b = Vector3.Lerp(startControlNode, endControlNode, t);
            Vector3 c = Vector3.Lerp(endControlNode, endNode.position, t);

            Vector3 d = Vector3.Lerp(a, b, t);
            Vector3 e = Vector3.Lerp(b, c, t);

            Vector3 position = Vector3.Lerp(d, e, t);

            Vector3 tangent = (e - d).normalized;

            Vector3 up = Vector3.Lerp(startNode.up, endNode.up, t).normalized;
            Quaternion orientation = Quaternion.LookRotation(tangent, up);

            return new OrientedPoint(position, orientation);
        }

        public static OrientedPoint GetOP(
           Vector3 startNodePosition,
           Vector3 startNodeUp,
           Vector3 startControlNode,
           Vector3 endNodePosition,
           Vector3 endNodeUp,
           Vector3 endControlNode,
           float t)
        {
            // NOTE: Tangent seeems to be always zero.
            Vector3 a = Vector3.Lerp(startNodePosition, startControlNode, t);
            Vector3 b = Vector3.Lerp(startControlNode, endControlNode, t);
            Vector3 c = Vector3.Lerp(endControlNode, endNodePosition, t);

            Vector3 d = Vector3.Lerp(a, b, t);
            Vector3 e = Vector3.Lerp(b, c, t);

            Vector3 position = Vector3.Lerp(d, e, t);

            Vector3 tangent = (e - d).normalized;

            Vector3 up = Vector3.Lerp(startNodeUp, endNodeUp, t).normalized;

            if (up == Vector3.zero)
            {
                up = Vector3.up;
                Debug.LogWarning("Up is zero");
            }
            Quaternion orientation;
            if (tangent == Vector3.zero)
                orientation = Quaternion.identity;
            else
                orientation = Quaternion.LookRotation(tangent, up);

            return new OrientedPoint(position, orientation);
        }

        public static float GetLenth(
            Vector3 startNodePosition,
            Vector3 startControlNode,
            Vector3 endControlNode,
            Vector3 endNodePosition)
        {
            float t = 0;
            float dist = 0;
            Vector3 previousPosition = Vector3.zero;
            while (t <= 1)
            {
                t += 0.01f;
                Vector3 a = Vector3.Lerp(startNodePosition, startControlNode, t);
                Vector3 b = Vector3.Lerp(startControlNode, endControlNode, t);
                Vector3 c = Vector3.Lerp(endControlNode, endNodePosition, t);

                Vector3 d = Vector3.Lerp(a, b, t);
                Vector3 e = Vector3.Lerp(b, c, t);

                Vector3 position = Vector3.Lerp(d, e, t);
                if (previousPosition == Vector3.zero)
                    previousPosition = position;

                dist += Vector3.Distance(position, previousPosition);

                previousPosition = position;
            }
            return dist;
        }

        public static float GetAproxLength(OrientedPoint[] spline)
        {
            float dist = 0;
            for (int i = 0; i < spline.Length - 1; i++)
            {
                Vector3 a = spline[i].position;
                Vector3 b = spline[i + 1].position;
                dist += Vector3.Distance(a, b);
            }

            return dist;
        }


        public static OrientedPoint[] CalculateSplineOP(
            Transform startNodeTransform,
            Vector3 startControlNodePosition,
            Vector3 endControlNodePosition,
            Transform endNodeTransform,
            int resolution)
        {
            OrientedPoint[] path = new OrientedPoint[resolution];
            for (int i = 0; i < resolution; i++)
            {
                float t = i / (float)(resolution - 1);
                path[i] = GetOP(
                    startNodeTransform,
                    startControlNodePosition,
                    endNodeTransform,
                    endControlNodePosition,
                    t);
            }
            return path;
        }

        public static OrientedPoint[] CalculateSplineOP(
            Vector3 startNodePosition,
            Vector3 startNodeUp,
            Vector3 startControlNodePosition,
            Vector3 endControlNodePosition,
            Vector3 endNodePosition,
            Vector3 endNodeUp,
            int resolution)
        {
            OrientedPoint[] path = new OrientedPoint[resolution];
            for (int i = 0; i < resolution; i++)
            {
                float t = i / (float)(resolution - 1);
                path[i] = GetOP(
                    startNodePosition,
                    startNodeUp,
                    startControlNodePosition,
                    endNodePosition,
                    endNodeUp,
                    endControlNodePosition,
                    t);
            }
            return path;
        }

        public static Vector3 GetClosestPointTo(
            Vector3 startNodePosition,
            Vector3 startControlNode,
            Vector3 endControlNode,
            Vector3 endNodePosition, 
            Vector3 targetPosition)
        {
            float t = 0;
            float minDistanceToSegment = Mathf.Infinity;
            Vector3 position = Vector3.positiveInfinity;
            while (t <= 1)
            {
                t += 0.01f;
                Vector3 a = Vector3.Lerp(startNodePosition, startControlNode, t);
                Vector3 b = Vector3.Lerp(startControlNode, endControlNode, t);
                Vector3 c = Vector3.Lerp(endControlNode, endNodePosition, t);

                Vector3 d = Vector3.Lerp(a, b, t);
                Vector3 e = Vector3.Lerp(b, c, t);

                position = Vector3.Lerp(d, e, t);

                float dist = Vector3.Distance(position, targetPosition);
                if (dist < minDistanceToSegment) 
                    minDistanceToSegment = dist;
                else
                    break;
            }
            if (position == Vector3.positiveInfinity)
                Debug.LogError("Closest point is infinity");
            return position;
        }

    }
}