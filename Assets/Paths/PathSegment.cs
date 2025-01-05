using Spline;
using UnityEngine;

namespace Paths
{
    public class PathSegment : Segment
    {
        public BasePathSO pathSO;
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private MeshCollider meshCollider;
        private Mesh mesh;

        public float MaxWidth => pathSO.MaxWidth;

        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            meshCollider = GetComponent<MeshCollider>();
            mesh = new Mesh {name = "Segment"};
            meshFilter.sharedMesh = mesh;
        }
        private void SetColliderMesh()
        {
            meshCollider.sharedMesh = mesh;
            meshCollider.includeLayers = LayerMask.GetMask("Default");
        }

        public void CreateSpline(
            PathNode startNode,
            PathNode endNode,
            Vector3 controlPosition, 
            float length,
            BasePathSO pathSO)
        {
            this.pathSO = pathSO;
            float controlNodeRatio = pathSO.controlNodeRatio;
            int segmentsPerUnitLength = pathSO.segmentsPerhUnitLength;
            base.CreateSpline(startNode, endNode, controlPosition, controlNodeRatio, length, segmentsPerUnitLength);

            startNode.RefreshConnections();
            endNode.RefreshConnections();
            startNode.ClearMesh();
            endNode.ClearMesh();
        }

        public override void SetPathMesh()
        {
            int startGeometry = startNode.GetConnectionFor(this).splineGeometryStartIndex;
            int endGeometry = endNode.GetConnectionFor(this).splineGeometryStartIndex;
            
            pathSO.CreatePathGeometry(mesh, spline, this.transform.position, startGeometry, endGeometry);
            meshRenderer.material = pathSO.material;
            mesh.RecalculateBounds();
            SetColliderMesh();
        }
    }
}