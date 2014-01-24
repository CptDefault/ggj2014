using UnityEngine;

public static class VectorExtensions
{
    public static Vector3 ClampMagnitude(this Vector3 vec, float maxLength)
    {
        if (vec.sqrMagnitude < maxLength*maxLength)
            return vec;
        return vec/vec.magnitude*maxLength;
    }

    public static Vector3 XZ(this Vector2 vec, float y = 0)
    {
        return new Vector3(vec.x, y, vec.y);
    }
    public static Vector3 YZ(this Vector2 vec, float x = 0)
    {
        return new Vector3(x, vec.x, vec.y);
    }
}
