using Microsoft.Xna.Framework;
using System;

namespace ShowPhysics.Library.Managers.Draw.Builders;

public class VectorBuilder : LineBuilderBase<VectorBuilder>
{
    public VectorBuilder(DrawImpl drawImpl) : base(drawImpl)
    {
    }

    public VectorBuilder Stroke()
    {
        // Draw the main line
        _drawImpl.CreateLine(_start, _end, _states.Color, _states.Thickness);

        // Arrowhead parameters
        float arrowLength = 16f; // Length of the arrowhead lines
        float arrowAngle = MathF.PI / 7f; // Angle between the arrowhead lines and the main line

        // Direction from P0 to P1
        var direction = _end - _start;
        if (direction.LengthSquared() < 1e-6f)
            return this; // Ignore zero-length vectors

        direction = Vector2.Normalize(direction);

        // Calculate the two arrowhead points
        float theta = MathF.Atan2(direction.Y, direction.X);

        float angle1 = theta + MathF.PI - arrowAngle;
        float angle2 = theta + MathF.PI + arrowAngle;

        Vector2 arrowPoint1 = _end + new Vector2(MathF.Cos(angle1), MathF.Sin(angle1)) * arrowLength;
        Vector2 arrowPoint2 = _end + new Vector2(MathF.Cos(angle2), MathF.Sin(angle2)) * arrowLength;

        // Draw the two arrowhead lines
        _drawImpl.CreateLine(_end, arrowPoint1, _states.Color, _states.Thickness);
        _drawImpl.CreateLine(_end, arrowPoint2, _states.Color, _states.Thickness);

        return this;
    }
}
