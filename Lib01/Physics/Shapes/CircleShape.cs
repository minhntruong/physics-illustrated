using Microsoft.Xna.Framework;
using System;

namespace ShowPhysics.Library.Physics.Shapes;

public class CircleShape : Shape 
{
    public CircleShape(float radius)
    {
        if (radius <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(radius), "Radius must be greater than zero.");
        }

        Radius = radius;
    }

    public override ShapeType Type => ShapeType.Circle;

    // This times mass = circle's moment of enertia
    public override float MomentOfInertiaFactor => 0.5f * Radius * Radius;

    public override void UpdateVertices(float rotation, Vector2 position)
    {
    }

    public float Radius { get; set; }
}
