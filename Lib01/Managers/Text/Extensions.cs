using Microsoft.Xna.Framework;
using System;
using static ShowPhysics.Library.Managers.Text.TextImpl;

namespace ShowPhysics.Library.Managers.Text;

public static class Extensions
{
    public static TextBuilder LengthBetween(this TextBuilder builder, Vector2 a, Vector2 b)
    {
        var pos = (a + b) * 0.5f;

        var length = Vector2.Distance(a, b);
        
        return builder.Position(pos).Text(length.ToString("0.0"));
    }
}
