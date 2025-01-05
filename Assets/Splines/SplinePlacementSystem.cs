using UnityEngine;
using TMPro;
using Utils;

namespace Spline.Management
{
    public class SplinePlacementSystem : MonoBehaviour
    {
        public enum BuildingState
        {
            StartNode,
            ControlNode,
            EndNode,
        }

        public enum BuildingMode
        {
            Straight,
            Curved,
        }

        protected SplineManager splineManager;

        protected GameObject nodeGFX;
        protected Vector3 controlPosition;
        [SerializeField] 
        protected Transform parentTransform;

        protected TextMeshPro smallerAngleText;
        protected TextMeshPro biggerAngleText;

        protected BuildingState buildingState;

        protected void Start()
        {
            splineManager = SplineManager.Instance;
            buildingState = BuildingState.StartNode;

            smallerAngleText = CreateText("Smaller angle text");
            biggerAngleText = CreateText("Bigger angle text");
            smallerAngleText.enabled = false;
            biggerAngleText.enabled = false;
        }

        private TextMeshPro CreateText(string textName)
        {
            GameObject textGO = new (textName);
            TextMeshPro text = textGO.AddComponent<TextMeshPro>();
            text.fontSize = 20;
            text.color = Color.black;
            text.alignment = TextAlignmentOptions.Center;
            text.transform.SetParent(transform);
            return text;
        }

        protected void HandleNodePositioning()
        {
            throw new System.NotImplementedException();
        }

        protected void HandleNodePlacement(Vector3 position, float length)
        {
            throw new System.NotImplementedException();
        }

        protected void PlaceSpline(Node startNode, Node endNode)
        {
            throw new System.NotImplementedException();
        }

        protected T GetOrCreateNodeAt<T>(Vector3 position) where T : Node
        {
            if (splineManager.TryGetNodeAt(position, out Node existingNode))
                return existingNode as T;

            // Create a new GameObject and add the specified type of Node
            T newNode = new GameObject(typeof(T).Name).AddComponent<T>();
            newNode.transform.position = position;
            newNode.transform.SetParent(parentTransform);

            return newNode;
        }

        protected void HandleSplinePreview(
            Vector3 start,
            Vector3 end,
            out float lenth,
            int resolution = 30,
            float controlNodeRatio = 0.552f,
            float maxWidth = 14f)
        {
            Vector3 controlNodePosition = (start + end) / 2;

            Vector3 startControlNodeOrientation = controlNodePosition - start;
            Vector3 endControlNodeOrientation = controlNodePosition - end;

            Vector3 startControlNodePosition = (controlNodeRatio * startControlNodeOrientation) + start;
            Vector3 endControlNodePosition = (controlNodeRatio * endControlNodeOrientation) + end;

            Vector3 leftGeometry = Vector3.left * maxWidth / 2;
            Vector3 rightGeometry = Vector3.right * maxWidth / 2;

            lenth = Bezier.GetLenth(
                start,
                startControlNodePosition,
                endControlNodePosition,
                end);

            // Segment
            OrientedPoint[] spline = DisplaySplineGeometry(
                start,
                end,
                startControlNodePosition,
                endControlNodePosition,
                leftGeometry,
                rightGeometry,
                resolution
            );

            // Node
            DisplayNodeGeometry(rightGeometry, spline[0]);
            DisplayNodeGeometry(leftGeometry, spline[^1]);
        }

        private void DisplayNodeGeometry(Vector3 geometrySide, OrientedPoint positionInSpline)
        {
            Vector3 yOffset = new(0, 0.1f, 0);
            Vector3 geometryStartPosition = positionInSpline.LocalToWorldPosition(geometrySide);

            OrientedPoint[] points = RafaelUtils.GetPointsArround(
                geometryStartPosition + yOffset,
                positionInSpline.position + yOffset,
                180,
                10
            );

            for (int i = 0; i < points.Length - 1; i++)
            {
                Vector3 position = points[i].position;
                Debug.DrawLine(points[i + 1].position, position);
            }
        }

        private OrientedPoint[] DisplaySplineGeometry(
            Vector3 start, 
            Vector3 end, 
            Vector3 startControlNodePosition, 
            Vector3 endControlNodePosition, 
            Vector3 leftGeometry, 
            Vector3 rightGeometry,
            int resolution
        )
        {
            OrientedPoint[] spline = Bezier.CalculateSplineOP(
                start,
                Vector3.up,
                startControlNodePosition,
                endControlNodePosition,
                end,
                Vector3.up,
                resolution
            );
            for (int i = 0; i < spline.Length - 1; i++)
            {
                Debug.DrawLine(spline[i + 1].position, spline[i].position);
            }
            for (int i = 0; i < spline.Length - 1; i++)
            {
                Debug.DrawLine(spline[i + 1].LocalToWorldPosition(leftGeometry), spline[i].LocalToWorldPosition(leftGeometry));
            }
            for (int i = 0; i < spline.Length - 1; i++)
            {
                Debug.DrawLine(spline[i + 1].LocalToWorldPosition(rightGeometry), spline[i].LocalToWorldPosition(rightGeometry));
            }
            return spline;
        }
    }
}