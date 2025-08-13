using System;
using Microsoft.Xna.Framework;

namespace PhysicsIllustrated.Library.Physics.Shapes;

public enum ShapeType
{
    Circle, Polygon, Box
}

public abstract class Shape
{
    public abstract ShapeType Type { get; }

    public abstract float MomentOfInertiaFactor { get; }

    public abstract void UpdateVertices(float rotation, Vector2 position);
}