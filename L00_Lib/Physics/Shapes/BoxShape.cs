using System;
using System.Numerics;

namespace PhysicsIllustrated.Library.Physics.Shapes;

public class BoxShape : PolygonShape
{
    public BoxShape(float width, float height)
        : base(
            // Clock-wise winding
            // These algorithms need clockwise winding to work correctly
            (-width / 2, -height / 2), // TL
            (+width / 2, -height / 2), // TR
            (+width / 2, +height / 2), // BR
            (-width / 2, +height / 2)  // BL

            // The below won't work with the current algorithms
            // Count-clock-wise winding
            //(+width / 2, -height / 2), // TR
            //(-width / 2, -height / 2), // TL
            //(-width / 2, +height / 2),  // BL
            //(+width / 2, +height / 2) // BR
        )
    {
        if (width <= 0 || height <= 0)
        {
            throw new ArgumentOutOfRangeException("Width and height must be greater than zero.");
        }

        Width = width;
        Height = height;
    }

    public override ShapeType Type => ShapeType.Box; // Change to Box when implemented

    public override float MomentOfInertiaFactor => 0.083333f * (Width * Width + Height * Height); // 0.083... is 1/12

    public float Width { get; set; }

    public float Height { get; set; }
}

