using System;
using Microsoft.Xna.Framework;

namespace PhysicsIllustrated.Library.Physics.Mathematics;

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


}
