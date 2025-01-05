using System.Collections.Generic;
using UnityEngine;

namespace Spline.Management
{

    public class SplineManager : MonoBehaviour
    {

        public static SplineManager Instance { get; private set; }

        private readonly Dictionary<Vector3, Node> nodesDict = new();
        private readonly List<Segment> segmentsList = new();

        private void Awake()
        {
            Instance = this;
        }
        public void AddNode(Node node)
        {
            if (!HasNode(node))
                nodesDict.Add(node.Position, node);
        }
        public void AddSegment(Segment segment)
        {
            if (!segmentsList.Contains(segment))
            {
                segmentsList.Add(segment);
                segment.transform.name = $"Segment {segmentsList.Count}";
            }
        }
        public void RemoveNode(Node node)
        {
            if (HasNode(node))
            {
                nodesDict.Remove(node.Position);
                Destroy(node.gameObject);
            }
        }
        private bool HasNode(Node node) => nodesDict.ContainsValue(node);
        public bool HasNode(Vector3 position) => nodesDict.ContainsKey(position);
        public bool TryGetNodeAt(Vector3 position, out Node node)
        {
            node = null;
            if (HasNode(position))
            {
                node = nodesDict.GetValueOrDefault(position);
                return true;
            }
            return false;
        }
    }
}