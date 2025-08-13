using System;
using Microsoft.Xna.Framework;

namespace PhysicsIllustrated.Library.Physics;

public class Contact : IId
{
    public int Id { get; set; }

    public Body A { get; set; }

    public Body B { get; set; }

    public Vector2 Start { get; set; } // Start point of the contact

    public Vector2 End { get; set; }   // End point of the contact

    public Vector2 Normal { get; set; } // Normal vector of the contact

    public float Depth { get; set; }
}
