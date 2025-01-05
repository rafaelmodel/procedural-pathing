using Paths;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Spline
{
    public class Connection
    {
        public Connection(Segment segment, Node node, bool isStartNode, int splineGeometryStartIndex = 0)
        {
            this.segment = segment;
            this.node = node;
            this.isStartNode = isStartNode;
            this.splineGeometryStartIndex = splineGeometryStartIndex;
        }

        public Segment segment;
        public Node node;
        public bool isStartNode;
        public int splineGeometryStartIndex;
    }

    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    public class Node : MonoBehaviour
    {
        protected List<Connection> connectionsList = new();
        public List<Connection> ConnectionsList => connectionsList;

        public bool HasConnection => connectionsList.Count > 0;
        public bool HasIntersection => connectionsList.Count > 1;
        public bool HasMultipleIntersections => connectionsList.Count > 2;
        public Vector3 Position => transform.position;
        
        public void AddSegment(Segment segment, bool isStartNode)
        {
            bool isConnected = connectionsList.Any(connection => connection.segment.Equals(segment));
            if (!isConnected)
            MakeConnection(segment, isStartNode);
        }

        public Connection GetConnectionFor(Segment segment)
        {
            return connectionsList.FirstOrDefault(connection => connection.segment.Equals(segment));
        }

        protected Connection MakeConnection(Segment segment, bool isStartNode, int geometryStartIndex = 0)
        {
            Connection connection = new(segment, this, isStartNode, geometryStartIndex);
            connectionsList.Add(connection);
            return connection;
        }

        public Connection[] GetAdjacentConnectionsTo(Vector3 segmentDirection)
        {       
            Connection[] adjacentConnections = new Connection[2];
            if (!HasIntersection)
            {
                adjacentConnections[0] = connectionsList[0];
                adjacentConnections[1] = connectionsList[0];
                return adjacentConnections;
            }

            float smallestAngle = 360;
            float biggestAngle = 0;
            Vector3 connectionDirection;

            foreach (Connection connection in connectionsList)
            {
                connectionDirection = GetConnectionDirection(connection);

                float angle = RafaelUtils.Vector3ToAngle360(segmentDirection, connectionDirection);
                if (angle < 5 || angle > 355) continue;

                if (angle < smallestAngle)
                {
                    smallestAngle = angle;
                    adjacentConnections[0] = connection;
                }
                if (angle > biggestAngle)
                {
                    biggestAngle = angle;
                    adjacentConnections[1] = connection;
                }
            }

            // The idea is for the index 0 to be the smallest angle independent of left or right
            if (360 - biggestAngle < smallestAngle)
            {
                (adjacentConnections[1], adjacentConnections[0]) = (adjacentConnections[0], adjacentConnections[1]);
            }
            return adjacentConnections;
        }

        public Vector3 GetConnectionDirection(Connection connection)
        {
            Vector3 connectionDirection;
            if (connection.isStartNode)
                connectionDirection = connection.segment.SplinePath[0].LocalToWorldDirection(Vector3.forward);
            else
                connectionDirection = connection.segment.SplinePath[^1].LocalToWorldDirection(-Vector3.forward);
            return connectionDirection;
        }

        public OrientedPoint GetSplinePoint(Connection connection, int index)
        {
            return connection.segment.SplinePath[index];
        }

        public virtual void SetNodeMesh(Connection connection)
        {
            throw new System.NotImplementedException();
        }

        public virtual void RefreshConnections()
        {
            throw new System.NotImplementedException();
        }
    }
}