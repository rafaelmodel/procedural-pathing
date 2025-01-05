using UnityEngine;
using System;
using Spline;

namespace Paths
{
    public class BasePathSO : ScriptableObject
    {
        public enum PathType
        {
            Road,
            Rail,
        }

        [SerializeField]
        private PathType pathType;

        public readonly float minAllowedAngle = 30f;
        public readonly float maxAllowedAngle;

        public readonly int segmentsPerhUnitLength = 1;

        public readonly float minLenth = 14;
        public readonly float controlNodeRatio = 0.552f;

        public Material material;

        public Mesh2D sidewalkShape;
        public Mesh2D middleShape;

        [HideInInspector]
        public Mesh2D leftPathShape;
        [HideInInspector]
        public Mesh2D rightPathShape;

        public float MaxWidth
        {
            get { return 15; }
        }

        public virtual PathSegment PlacePath(
            PathNode startNode,
            PathNode endNode,
            Vector3 controlPosition,
            float length,
            Transform parent)
        {
            throw new NotImplementedException();
        }

        public virtual void SetupMesh()
        {
            throw new NotImplementedException();
        }

        public virtual void ExtrudePathGeometry(
            Mesh mesh,
            OrientedPoint[] path, 
            Vector3 geometryOffset, 
            int startGeometry, 
            int endGeometry,
            Mesh2D shape
        )
        {
            throw new NotImplementedException();
        }
        public virtual void CreatePathGeometry(
            Mesh mesh,
            OrientedPoint[] path,
            Vector3 geometryOffset,
            int startGeometry,
            int endGeometry
        )
        {
            throw new NotImplementedException();
        }

        public virtual void CreateIntersectionGeometry(Connection connection, Mesh mesh)
        {
            throw new NotImplementedException();
        }

        public virtual OrientedPoint[] HandleNodeGeometry(
            Vector3 splineGeometryStart,
            Vector3 splineGeometryEnd,
            Vector3 controlNodePosition,
            Vector3 geometryOffset,
            Mesh mesh,
            Mesh2D shape
        )
        {
            throw new NotImplementedException();
        }
    }
}