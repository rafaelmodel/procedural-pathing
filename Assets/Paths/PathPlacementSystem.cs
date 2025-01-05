using Spline;
using Spline.Management;
using UnityEngine;
using Utils;

namespace Paths.Management
{
    public class PathPlacementSystem : SplinePlacementSystem
    {
        public static PathPlacementSystem Instance { get; private set; }
        [SerializeField] private BasePathSO pathSO;
        private PathNode startNode;

        private void Awake()
        {
            Instance = this;
        }

        new private void Start()
        {
            base.Start();
        }
        private void Update()
        {
            HandleNodePositioning();
        }

        new private void HandleNodePositioning()
        {
            Vector3 hitPosition = new Vector3(0, 0.01f, 0);
            float length = 0;

            if (!RafaelUtils.TryRaycastObject(out RaycastHit hit))
                return;

            if (hit.transform.CompareTag("Terrain"))
                hitPosition += hit.point;

            else if (hit.transform.TryGetComponent<PathNode>(out PathNode node))
                hitPosition = node.Position;

            else
                return;

            Debug.Log(hit.transform.name);

            if (buildingState == BuildingState.EndNode)
            {
                HandleSplinePreview(
                    startNode.transform.position,
                    hitPosition,
                    out length,
                    controlNodeRatio: pathSO.controlNodeRatio,
                    maxWidth: pathSO.MaxWidth);
            }

            if (Input.GetMouseButtonDown(0))
            {
                HandleNodePlacement(hitPosition, length);
            }

            if (Input.GetMouseButtonDown(1))
            {
                buildingState = BuildingState.StartNode;
                if (startNode != null && !startNode.HasConnection)
                {
                    Destroy(startNode.gameObject);
                    startNode = null;
                }
            }
        }

        new private void HandleNodePlacement(Vector3 position, float length)
        {
            switch (buildingState)
            {
                case BuildingState.StartNode:
                    startNode = GetOrCreateNodeAt<PathNode>(position);
                    buildingState = BuildingState.EndNode;
                    break;
                case BuildingState.ControlNode:
                    break;
                case BuildingState.EndNode:
                    PathNode endNode = GetOrCreateNodeAt<PathNode>(position);
                    Vector3 controlPosition = (startNode.Position + endNode.Position) / 2;
                    PlacePath(startNode, endNode, controlPosition, length);
                    startNode = endNode;
                    break;
                default:
                    break;
            }
        }

        private void PlacePath(PathNode startNode, PathNode endNode, Vector3 controlPosition, float lenth)
        {
            PathSegment segment = pathSO.PlacePath(startNode, endNode, controlPosition, lenth, parentTransform);

            splineManager.AddSegment(segment);
            splineManager.AddNode(startNode);
            splineManager.AddNode(endNode);

            foreach (Connection connection in startNode.ConnectionsList)
            {
                connection.segment.SetPathMesh();
                if (!startNode.HasIntersection) continue;
                startNode.SetNodeMesh(connection);
            }
            foreach (Connection connection in endNode.ConnectionsList)
            {
                connection.segment.SetPathMesh();
                //Debug.Log("Is this working?");
                //if (!endNode.HasIntersection) continue;
                //Debug.Log("End node has intersection");
                //endNode.SetNodeMesh(connection);
            }
        }
    }
}