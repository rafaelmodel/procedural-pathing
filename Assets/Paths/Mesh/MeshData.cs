using System.Collections.Generic;
using UnityEngine;


namespace Paths.Meshes
{
    public class MeshData
    {
        public List<Vector3> vertices = new();
        public List<int> triangles = new();
        public List<Vector2> uvs = new();
        public void AddVertice(Vector3 vertice)
        {
            this.vertices.Add(vertice);
        }
        public void AddTriangles(int[] triangles)
        {
            this.triangles.AddRange(triangles);
        }
        public void AddUvs(Vector2[] uvs)
        {
            this.uvs.AddRange(uvs);
        }
    }
}