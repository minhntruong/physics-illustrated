using System;
using Microsoft.Xna.Framework;

namespace ShowPhysics.Library.Physics.Math;

public static class Extensions
{
    public static Vector2 RightUnitNormal(this Vector2 vector)
    {
        // Returns a vector that is perpendicular to the right of the given vector
        var normal = new Vector2(vector.Y, -vector.X);

        return Vector2.Normalize(normal);
    }

    public static float Cross(this Vector2 a, Vector2 b)
    {
        return a.X * b.Y - a.Y * b.X;
    }

    public static Vector2 Cross(this float s, Vector2 v)
    {
        return new Vector2(-s * v.Y, s * v.X);
    }

    public static Vector2 Cross(this Vector2 v, float s)
    {
        return new Vector2(s * v.Y, -s * v.X);
    }

    public static bool Vector2TryParse(string s, out Vector2 v)
    {
        v = Vector2.Zero;

        // {X:1.2 Y:3.4}

        var parts = s.Trim('{', '}').Split(' ', ':');

        if (parts.Length != 4) { return false; }

        if (float.TryParse(parts[1], out var x) && float.TryParse(parts[3], out var y))
        {
            v = new Vector2(x, y);
            return true;
        }

        return false;
    }
}
