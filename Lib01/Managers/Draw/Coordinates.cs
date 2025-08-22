using System;
using Microsoft.Xna.Framework;

namespace ShowPhysics.Library.Managers.Draw;

internal static class Coordinates
{
    private static float _zoom = 1.0f;
    private static Vector2 _origin = Vector2.Zero;

    public static void SetZoom(float zoomInc, Vector2 screenPos)
    {
        var unzoomedWorldPos = (screenPos + Origin) / Zoom;

        Zoom += zoomInc;

        // Adjust Origin so the world position under the cursor stays fixed
        Origin = (unzoomedWorldPos * Zoom) - screenPos;
    }

    public static float Zoom
    {
        get { return _zoom; }
        set
        {
            _zoom = value;
            _zoom = Math.Clamp(_zoom, 0.1f, 5.0f);
        }
    }

    // Origin already has zoom applied
    public static Vector2 Origin
    {
        get { return _origin; }
        set
        {
            _origin = value;
        }
    }

    public static Vector2 Transform(Vector2 point)
    {
        return TransformZoomAndPan(point);
    }

    public static void Rectangle(Vector2 center, float width, float height, Span<Vector2> data)
    {
        if (data.Length != 4)
            throw new ArgumentOutOfRangeException(nameof(data), "Data length must be 4");

        center = TransformZoomAndPan(center);
        width = TransformZoom(width);
        height = TransformZoom(height);

        var halfWidth = width / 2;
        var halfHeight = height / 2;

        data[0] = new Vector2(center.X - halfWidth, center.Y - halfHeight); // TopLeft
        data[1] = new Vector2(center.X + halfWidth, center.Y - halfHeight); // TopRight
        data[2] = new Vector2(center.X + halfWidth, center.Y + halfHeight); // BottomRight
        data[3] = new Vector2(center.X - halfWidth, center.Y + halfHeight); // BottomLeft
    }

    public static void Circle(Vector2 center, float radius, int segments, Span<Vector2> data)
    {
        if (data.Length != segments)
            throw new ArgumentOutOfRangeException(nameof(data), "Data length must match segments");

        center = TransformZoomAndPan(center);
        radius = TransformZoom(radius);

        for (var v = 0; v < data.Length; v++)
        {
            var angle = (float) v / data.Length * MathHelper.TwoPi;
            var x = center.X + MathF.Cos(angle) * radius;
            var y = center.Y + MathF.Sin(angle) * radius;

            data[v] = new Vector2(x, y);
        }
    }

    //==========================================================================

    private static Vector2 TransformZoomAndPan(Vector2 point)
    {
        return (point - Origin) * Zoom;
    }

    private static float TransformZoom(float value)
    {
        return value * Zoom;
    }
}
