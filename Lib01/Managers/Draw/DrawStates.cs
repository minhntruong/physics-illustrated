using System;
using Microsoft.Xna.Framework;

namespace ShowPhysics.Library.Managers.Draw;

public struct DrawStates
{
    public DrawStates()
    {
    }

    public Color Color { get; set; } = Color.White;
    public float Thickness { get; set; } = 1.0f;
}
