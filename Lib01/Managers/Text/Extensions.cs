using Microsoft.Xna.Framework;
using System;
using static ShowPhysics.Library.Managers.Text.TextImpl;

namespace ShowPhysics.Library.Managers.Text;

public static class Extensions
{
    public static TextBuilder LengthBetween(this TextBuilder builder, Vector2 a, Vector2 b, bool showLength = true)
    {
        var pos = (a + b) * 0.5f;

        var length = Vector2.Distance(a, b);

        builder = builder.Position(pos);

        if (showLength == false) return builder;

        return builder.Text(length.ToString("0.0"));
    }

    public static TextBuilder LengthBetween(this TextBuilder builder, Func<Vector2> a, Func<Vector2> b)
    {
        return builder.LengthBetween(a(), b());
    }
}
