using System;
using Microsoft.Xna.Framework;

namespace ShowPhysics.Library.Managers.Draw;

public struct DrawStates
{
    public DrawStates()
    {
    }

    private float _thickness = 1.0f;

    public Color Color { get; set; } = Color.White;

    public float Thickness 
    { 
        get => ThicknessAbs ? _thickness / Math.Max(1.0f, Camera.Zoom) : _thickness;
        set => _thickness = value;
    }

    public bool ThicknessAbs { get; set; } = false;  // Whether to back out Zoom when drawing -- this causes the stroke to be the same number of pixels regardless of zoom level
}
