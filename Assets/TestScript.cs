using Paths.Vehicle;
using UnityEngine;

public class TestScript : MonoBehaviour
{

    public RoadPathSO roadSO;

    private void OnDrawGizmos()
    {
        roadSO.SetupMesh();
        Mesh2D leftShape = roadSO.leftPathShape;
        Mesh2D rightShape = roadSO.rightPathShape;

        Gizmos.color = Color.blue;
        for (int i = 0; i < rightShape.LineCount; i += 2)
        {
            Vector2 a = rightShape.vertices[rightShape.lines[i]].point;
            Vector2 b = rightShape.vertices[rightShape.lines[i + 1]].point;
            Gizmos.DrawLine(a, b);
            Gizmos.DrawSphere(a, 0.05f);
        }

        Gizmos.color = Color.red;
        for (int i = 0; i < leftShape.LineCount; i += 2)
        {
            Vector2 a = leftShape.vertices[leftShape.lines[i]].point;
            Vector2 b = leftShape.vertices[leftShape.lines[i + 1]].point;
            Gizmos.DrawLine(a, b);
        }

    }
}
