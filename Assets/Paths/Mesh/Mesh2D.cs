using UnityEngine;


[CreateAssetMenu]
public class Mesh2D : ScriptableObject
{
    [System.Serializable]
    public class Vertex
    {
        public Vector2 point;
        [HideInInspector]
        public Vector2 normal;
        [HideInInspector]
        public float uCoord;
    }

    public Vertex[] vertices;
    [HideInInspector]
    public int[] lines;

    public int VertexCount => vertices.Length;
    public int LineCount => lines.Length;

    public float CalculateUspan()
    {
        float dist = 0f;
        for (int i = 0; i < LineCount; i+=2)
        {
            Vector2 a = vertices[lines[i]].point;
            Vector2 b = vertices[lines[i + 1]].point;

            dist += (a - b).magnitude;
        }
        PopulateMeshUvs();
        PopulateNormals();
        return dist;
    }

    private void PopulateNormals()
    {
        for (int i = 0; i < LineCount; i += 2)
        {
            Vector2 a = vertices[lines[i]].point;
            Vector2 b = vertices[lines[i + 1]].point;
            Vector2 dir = (b - a).normalized;
            Vector2 normal = new(-dir.y, dir.x);
            vertices[lines[i]].normal = normal;
            vertices[lines[i + 1]].normal = normal;
        }
    }

    private void PopulateMeshUvs()
    {
        int numUvs = VertexCount / 3;
        for (int i = 0; i < numUvs; i++)
        {
            float completionPercent = i / (float)(numUvs - 1);

            vertices[0].uCoord = completionPercent;
            vertices[1].uCoord = completionPercent;
            vertices[2].uCoord = completionPercent;
        }
    }
}
