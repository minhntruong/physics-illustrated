using System;
using Microsoft.Xna.Framework;

namespace ShowPhysics.Library.Managers.Text;

public enum TextAnchor { TopLeft, Center };

public struct TextStates
{
    public TextStates()
    {
    }

    public Color Color { get; set; } = Color.White;
    public TextAnchor Anchor { get; set; } = TextAnchor.TopLeft;
    public float Scale { get; set; } = 1;
    public float Rotation { get; set; } = 0;
}