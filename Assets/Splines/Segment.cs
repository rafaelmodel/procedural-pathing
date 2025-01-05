using UnityEngine;
using Utils;

namespace Spline
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    public class Segment : MonoBehaviour
    {
        protected Node startNode, endNode;
        public Node StartNode => startNode;
        public Node EndNode => endNode;

        protected Transform controlNodeTransform;
        protected float length;

        protected OrientedPoint[] spline;
        public OrientedPoint[] SplinePath => spline;

        public virtual void CreateSpline(
            Node startNode, 
            Node endNode,
            Vector3 controlPosition,
            float controlNodeRatio,
            float length,
            int segmentsPerhUnitLength)
        {
            this.startNode = startNode;
            this.endNode = endNode;
            this.length = length;

            controlNodeTransform = RafaelUtils.CreateSphere(
                controlPosition,
                "Control Node",
                transform,
                0.5f
            ).transform;

            controlNodeTransform.position = controlPosition;
            CreateSplinePoints(segmentsPerhUnitLength, controlNodeRatio);

            this.startNode.AddSegment(this, true);
            this.endNode.AddSegment(this, false);
        }

        private void CreateSplinePoints(int segmentsPerhUnitLength, float controlNodeRatio)
        {            
            int resolution = Mathf.FloorToInt(length * segmentsPerhUnitLength);
            Vector3 offset = new(0, transform.parent.position.y, 0);

            Vector3 startPosition = startNode.Position + offset;
            Vector3 endPosition = endNode.Position + offset;

            controlNodeTransform.position = (startPosition + endPosition) / 2 + offset;

            Vector3 startControlNodeOrientation = controlNodeTransform.position - startPosition;
            Vector3 endControlNodeOrientation = controlNodeTransform.position - endPosition;

            Vector3 startControlNodePosition = (controlNodeRatio * startControlNodeOrientation) + startPosition;
            Vector3 endControlNodePosition = (controlNodeRatio * endControlNodeOrientation) + endPosition;

            spline = Bezier.CalculateSplineOP(
                startNode.transform,
                startControlNodePosition,
                endControlNodePosition,
                endNode.transform,
                resolution
            );
        }

        public virtual void SetPathMesh()
        {
            throw new System.NotImplementedException();
        }
    }
}