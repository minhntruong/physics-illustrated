using System;
using Microsoft.Xna.Framework;

namespace ShowPhysics.Library.Managers.Draw;

internal static class Coordinates
{
    public static void Rectangle(Vector2 center, float width, float height, Span<Vector2> data, float scale = 1)
    {
        if (data.Length != 4)
            throw new ArgumentOutOfRangeException(nameof(data), "Data length must be 4");

        width = width * scale;
        height = height * scale;

        var halfWidth = width / 2;
        var halfHeight = height / 2;

        data[0] = new Vector2(center.X - halfWidth, center.Y - halfHeight); // TopLeft
        data[1] = new Vector2(center.X + halfWidth, center.Y - halfHeight); // TopRight
        data[2] = new Vector2(center.X + halfWidth, center.Y + halfHeight); // BottomRight
        data[3] = new Vector2(center.X - halfWidth, center.Y + halfHeight); // BottomLeft
    }

    public static void Circle(Vector2 center, float radius, int segments, Span<Vector2> data, float scale = 1)
    {
        if (data.Length != segments)
            throw new ArgumentOutOfRangeException(nameof(data), "Data length must match segments");

        radius = radius * scale;

        for (var v = 0; v < data.Length; v++)
        {
            var angle = (float) v / data.Length * MathHelper.TwoPi;
            var x = center.X + MathF.Cos(angle) * radius;
            var y = center.Y + MathF.Sin(angle) * radius;

            data[v] = new Vector2(x, y);
        }
    }
}
