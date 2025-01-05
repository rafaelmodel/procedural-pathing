using Paths.Meshes;
using Spline;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Paths.Vehicle
{
    [CreateAssetMenu(fileName = "Road", menuName = "Paths/Road")]
    public class RoadPathSO : BasePathSO
    {
        public readonly float laneWidth = 2;
        public readonly float sidewalkWidth = 1;
        public readonly float middleWidth = 0.5f;
        public readonly float middleHeight = 0.4f;

        public int lanes = 1;
        public bool hasSidewalk = true;
        public bool hasMiddle = true;

        new public float MaxWidth
        {
            get { return lanes * laneWidth + 2 * sidewalkWidth + middleWidth; }
        }

        public override void SetupMesh()
        {
            int vertexCount = 4 + (hasSidewalk ? sidewalkShape.VertexCount : 0) + (hasMiddle ? middleShape.VertexCount : 0);
            rightPathShape = CreateInstance<Mesh2D>();

            Mesh2D.Vertex[] rightVertices = new Mesh2D.Vertex[vertexCount];
            int[] rightLines = new int[rightVertices.Length];

            rightVertices[^1] = new()
            {
                point = new Vector2(0, hasMiddle ? middleHeight : 0)
            };
            rightVertices[^2] = new()
            {
                point = new Vector2(0, hasMiddle ? middleHeight : 0)
            };
            rightVertices[^3] = new()
            {
                point = new(0, -0.25f)
            };
            rightVertices[^4] = new()
            {
                point = new(0, -0.25f)
            };


            int index = 0;
            if (hasMiddle)
            {
                for (int i = 0; i < middleShape.VertexCount; i++)
                {
                    rightVertices[index] = new()
                    {
                        point = middleShape.vertices[i].point
                    };
                    if (index > 0)
                        rightLines[index] = index - 1;
                    index++;
                }
            }
            if (hasSidewalk)
            {
                for (int i = 0; i < sidewalkShape.VertexCount; i++)
                {
                    // Instantiate the Vertex objects
                    rightVertices[index] = new()
                    {
                        point = sidewalkShape.vertices[i].point + new Vector2(lanes * laneWidth + (hasSidewalk ? sidewalkWidth : 0), 0)
                    };
                    if (index > 0)
                        rightLines[index] = index - 1;
                    index++;
                }
            }

            rightLines[0] = rightVertices.Length - 1;
            rightLines[^4] = rightVertices.Length - 5;
            rightLines[^3] = rightVertices.Length - 4;
            rightLines[^2] = rightVertices.Length - 3;
            rightLines[^1] = rightVertices.Length - 2;


            rightPathShape.vertices = rightVertices;
            rightPathShape.lines = rightLines;

            leftPathShape = CreateInstance<Mesh2D>();
            Mesh2D.Vertex[] leftVertices = new Mesh2D.Vertex[rightVertices.Length];
            for (int i = 0; i < rightVertices.Length; i++)
            {
                leftVertices[i] = new ()
                {
                    point = new Vector2(-rightVertices[i].point.x, rightVertices[i].point.y)
                };
            }

            leftPathShape.vertices = leftVertices;
            // The order we evaluate the lines/vertices is important
            // It will determine the direction of the normals
            leftPathShape.lines = rightLines.Reverse().ToArray();
        }

        public override PathSegment PlacePath(
            PathNode startNode,
            PathNode endNode,
            Vector3 controlPosition,
            float length,
            Transform parent
        )
        {
            Transform segmentTransform = new GameObject("Segment").transform;
            segmentTransform.localPosition = controlPosition;
            segmentTransform.SetParent(parent);
            PathSegment segment = segmentTransform.gameObject.AddComponent<PathSegment>();

            segment.CreateSpline(startNode, endNode, controlPosition, length, this);
            return segment;
        }

        public override OrientedPoint[] HandleNodeGeometry(
            Vector3 splineGeometryStart,
            Vector3 splineGeometryEnd,
            Vector3 controlNodePosition,
            Vector3 geometryOffset,
            Mesh mesh,
            Mesh2D shape
        )
        {
            Vector3 startControlNodeOrientation = controlNodePosition - splineGeometryStart;
            Vector3 endControlNodeOrientation = controlNodePosition - splineGeometryEnd;

            Vector3 startControlNodePosition = (controlNodeRatio * startControlNodeOrientation) + splineGeometryStart;
            Vector3 endControlNodePosition = (controlNodeRatio * endControlNodeOrientation) + splineGeometryEnd;

            // NOTE: Numbers refer to how many loops the spline will have, We need a even number so that we don't
            // get any gaps on the geometry. That happens because we must take half of the loops each time to
            // avoid duplicating geometry. 
            OrientedPoint[] spline = Bezier.CalculateSplineOP(
                splineGeometryStart,
                Vector3.up,
                startControlNodePosition,
                endControlNodePosition,
                splineGeometryEnd,
                Vector3.up,
                21
            ).Take(11).ToArray();

            ExtrudePathGeometry(mesh, spline, geometryOffset, 0, 10, shape);
            return spline;
        }

        public override void CreatePathGeometry(
            Mesh mesh,
            OrientedPoint[] path,
            Vector3 geometryOffset,
            int startGeometry,
            int endGeometry
        )
        {
            SetupMesh();
            List<CombineInstance> meshes = new();
            Mesh leftMesh = new();
            Mesh rightMesh = new();

            ExtrudePathGeometry(leftMesh, path, geometryOffset, startGeometry, endGeometry, leftPathShape);
            meshes.Add(new CombineInstance { mesh = leftMesh });

            ExtrudePathGeometry(rightMesh, path, geometryOffset, startGeometry, endGeometry, rightPathShape);
            meshes.Add(new CombineInstance { mesh = rightMesh });

            mesh.CombineMeshes(meshes.ToArray(), true, false);
        }


        public override void ExtrudePathGeometry(
            Mesh mesh, 
            OrientedPoint[] path, 
            Vector3 geometryOffset, 
            int startGeometry, 
            int endGeometry, 
            Mesh2D shape
        )
        {
            //Debug.Log("ExtrudePathGeometry");

            int vertsInShape = shape.VertexCount;
            int edgeLoops = endGeometry - startGeometry + 1;
            int segments = edgeLoops - 1;
            int vertCount = vertsInShape * edgeLoops;
            int triCount = shape.LineCount * segments;
            int triIndexCount = triCount * 3;
            float uSpam = shape.CalculateUspan();

            int[] triangleIndices = new int[triIndexCount];
            Vector3[] vertices = new Vector3[vertCount];
            Vector3[] normals = new Vector3[vertCount];
            Vector2[] uvs = new Vector2[vertCount];

            /* Generation code goes here */

            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangleIndices;
            mesh.normals = normals;
            mesh.uv = uvs;

            // Vertices
            for (int i = 0; i < edgeLoops; i++)
            {
                int offset = i * vertsInShape;
                for (int j = 0; j < vertsInShape; j++)
                {
                    int id = offset + j;
                    float t = i / (float)segments;

                    vertices[id] = path[i + startGeometry].LocalToWorldPosition(shape.vertices[j].point) - geometryOffset;
                    normals[id] = path[i + startGeometry].LocalToWorldDirection(shape.vertices[j].normal);
                    //uvs[id] = new Vector2(shape.ver tices[j].uCoord, t * Bezier.GetAproxLength(path) / uSpam);
                }
            }

            // Triangles
            int ti = 0;
            for (int i = 0; i < segments; i++)
            {
                int offset = i * vertsInShape;
                for (int l = 0; l < shape.LineCount; l += 2)
                {
                    int a = offset + shape.lines[l] + vertsInShape;
                    int b = offset + shape.lines[l];
                    int c = offset + shape.lines[l + 1];
                    int d = offset + shape.lines[l + 1] + vertsInShape;

                    triangleIndices[ti] = a; ti++;
                    triangleIndices[ti] = d; ti++;
                    triangleIndices[ti] = c; ti++;
                    triangleIndices[ti] = c; ti++;
                    triangleIndices[ti] = b; ti++;
                    triangleIndices[ti] = a; ti++;
                }
            }

            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangleIndices, 0);
            mesh.SetNormals(normals);
            mesh.SetUVs(0, uvs);
        }
    }
}