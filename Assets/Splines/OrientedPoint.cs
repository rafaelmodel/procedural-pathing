using UnityEngine;

public struct OrientedPoint
{
    public Vector3 position;
    public Quaternion orientation;

    public OrientedPoint(Vector3 position, Quaternion orientation)
    {
        this.position = position;
        this.orientation = orientation.normalized;
    }

    public OrientedPoint(Vector3 position, Vector3 forward)
    {
        this.position = position;
        this.orientation = Quaternion.LookRotation(forward);
    }

    public readonly Vector3 LocalToWorldPosition(Vector3 point)
    {
        return position + orientation * point;
    }

    public readonly Vector3 WorldToLocalPosition(Vector3 point)
    {
        return Quaternion.Inverse(orientation) * (point - position);
    }

    public readonly Vector3 LocalToWorldDirection(Vector3 dir)
    {
        return orientation * dir;
    }
}