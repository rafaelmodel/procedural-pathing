using Paths.Meshes;
using Spline;
using System.Collections.Generic;
using UnityEngine;

namespace Paths
{
    public class PathNode : Node
    {
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private MeshCollider meshCollider;

        List<CombineInstance> meshes = new();
        private Mesh mesh;

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

        public override void RefreshConnections()
        {
            foreach (Connection connection in connectionsList)
            {
                RefreshConnection(connection);
            }
        }

        public void RefreshConnection(PathSegment segment)
        {
            Connection connection = GetConnectionFor(segment);
            if (connection == null)
            {
                print("Connection is null");
                return;
            }
            RefreshConnection(connection);
        }


        public void RefreshConnection(Connection connection)
        {
            PathSegment segment = (PathSegment)connection.segment;
            Vector3 segmentDirection = GetConnectionDirection(connection);
            OrientedPoint[] segmentSpline = segment.SplinePath;
            int splineGeometryStartIndex;

            if (!HasIntersection)
            {
                splineGeometryStartIndex = 0;
                if (!connection.isStartNode)
                    splineGeometryStartIndex = segmentSpline.Length - 1;

                connection.splineGeometryStartIndex = splineGeometryStartIndex;
                return;
            }

            Connection[] adjacentConnections = GetAdjacentConnectionsTo(segmentDirection);
            Connection adjacentConnection = adjacentConnections[0];

            Vector3 adjacentConnectionDirection;

            adjacentConnectionDirection = GetConnectionDirection(adjacentConnection);

            float angle = Vector3.Angle(segmentDirection, adjacentConnectionDirection);
            float geometryStartOffset = segment.MaxWidth * 0.6f;

            if (angle < 90)
                geometryStartOffset = 63.33f * Mathf.Exp(-0.04f * angle) + segment.MaxWidth * 0.6f;

            float segmentCoveredDistance = 0;

            for (int i = 1; i < segmentSpline.Length - 2; i++)
            {
                int segmentGeometryStartIndex = i;
                if (!connection.isStartNode)
                    segmentGeometryStartIndex = segmentSpline.Length - 1 - i;

                segmentCoveredDistance += Vector3.Distance(segmentSpline[i + 1].position, segmentSpline[i].position);

                if (segmentCoveredDistance >= geometryStartOffset)
                {
                    connection.splineGeometryStartIndex = segmentGeometryStartIndex;
                    break;
                }
            }
        }

        public void ClearMesh()
        {
            mesh.Clear();
            meshes.Clear();
        }

        public override void SetNodeMesh(Connection connection)
        {
            PathSegment segment = (PathSegment)connection.segment;
            Vector3 connectionDireciton = connection.node.GetConnectionDirection(connection);
            Connection[] connections = connection.node.GetAdjacentConnectionsTo(connectionDireciton);
            OrientedPoint splineGeometryStart = connection.node.GetSplinePoint(connection, connection.splineGeometryStartIndex);
            OrientedPoint controlPoint = segment.SplinePath[0];
            Vector3 forward = Vector3.forward;
            Vector3 geometryOffset = transform.position;

            if (!connection.isStartNode)
            {
                forward = -Vector3.forward;
                controlPoint = connection.segment.SplinePath[^1];
            }

            Vector3 segmentDirection = splineGeometryStart.LocalToWorldDirection(forward);
            float smallestAngle = 0;

            for (int i = 0; i < connections.Length; i++)
            {
                MeshData meshData = new();
                Mesh newMesh = new();
                OrientedPoint[] midleSpline;


                Connection adjacentConnection = connections[i];
                if (adjacentConnection == null) continue;
                OrientedPoint connectionGeometryStart = adjacentConnection.node.GetSplinePoint(adjacentConnection, adjacentConnection.splineGeometryStartIndex);

                forward = Vector3.forward;

                if (!adjacentConnection.isStartNode)
                    forward = -Vector3.forward;

                Vector3 adjacentSegmentDirection = connectionGeometryStart.LocalToWorldDirection(forward);
                float angle = Vector3.SignedAngle(segmentDirection, adjacentSegmentDirection, Vector3.up);

                // This will be the smaller angle segment to one side
                // We don't know which side is the segment, but here it doesn't matter
                if (i == 0)
                {
                    smallestAngle = angle;
                    if (angle < 0)
                    {
                        midleSpline = segment.pathSO.HandleNodeGeometry(
                            splineGeometryStart.position,
                            connectionGeometryStart.position,
                            controlPoint.position,
                            geometryOffset,
                            newMesh,
                            segment.pathSO.rightPathShape
                        );
                        for (int j = 0; j < midleSpline.Length - 1; j++)
                        {
                            meshData.AddVertice(midleSpline[j].position - geometryOffset);
                            meshData.AddVertice(controlPoint.position - geometryOffset);
                            meshData.AddVertice(midleSpline[j + 1].position - geometryOffset);
                        }
                    }
                    else
                    {
                        midleSpline = segment.pathSO.HandleNodeGeometry(
                            splineGeometryStart.position,
                            connectionGeometryStart.position,
                            controlPoint.position,
                            geometryOffset,
                            newMesh,
                            segment.pathSO.leftPathShape
                        );
                        for (int j = 0; j < midleSpline.Length - 1; j++)
                        {
                            meshData.AddVertice(midleSpline[j + 1].position - geometryOffset);
                            meshData.AddVertice(controlPoint.position - geometryOffset);
                            meshData.AddVertice(midleSpline[j].position - geometryOffset);
                        }
                    }
                }
                // This will be the smaller angle to the other side
                else
                {
                    if ((angle < 0 && smallestAngle < 0) || ( angle >= 0 && smallestAngle < 0))
                    {
                        midleSpline = segment.pathSO.HandleNodeGeometry(
                            splineGeometryStart.position,
                            connectionGeometryStart.position,
                            controlPoint.position,
                            geometryOffset,
                            newMesh,
                            segment.pathSO.leftPathShape
                        );
                        for (int j = 0; j < midleSpline.Length - 1; j++)
                        {
                            meshData.AddVertice(midleSpline[j + 1].position - geometryOffset);
                            meshData.AddVertice(controlPoint.position - geometryOffset);
                            meshData.AddVertice(midleSpline[j].position - geometryOffset);
                        }
                    }
                    else if ((angle < 0 && smallestAngle >= 0) || (angle >= 0 && smallestAngle >= 0))
                    {
                        midleSpline = segment.pathSO.HandleNodeGeometry(
                            splineGeometryStart.position,
                            connectionGeometryStart.position,
                            controlPoint.position,
                            geometryOffset,
                            newMesh,
                            segment.pathSO.rightPathShape
                        );
                        for (int j = 0; j < midleSpline.Length - 1; j++)
                        {
                            meshData.AddVertice(midleSpline[j].position - geometryOffset);
                            meshData.AddVertice(controlPoint.position - geometryOffset);
                            meshData.AddVertice(midleSpline[j + 1].position - geometryOffset);
                        }
                    }
                }

                // NOTE: If we need to cap the bottom part of the road we can just duplicate
                // the midleMesh but offsetted by the height of the road.
                if (HasMultipleIntersections)
                {
                    MeshUtilities.PopulateMeshTriangles(meshData);
                    MeshUtilities.PopulateMeshUvs(meshData);
                    Mesh midleMesh = MeshUtilities.LoadMesh(meshData);
                    meshes.Add(new CombineInstance { mesh = midleMesh });
                }

                meshes.Add(new CombineInstance { mesh = newMesh });
            }

            mesh.CombineMeshes(meshes.ToArray(), true, false);
            meshRenderer.material = segment.pathSO.material;
            mesh.RecalculateBounds();
            SetColliderMesh();
        }

    }
}