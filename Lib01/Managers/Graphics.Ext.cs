using System;
using Microsoft.Xna.Framework;

namespace ShowPhysics.Library.Managers;

public static partial class Graphics
{
    public static void DrawVertex(Vector2 position)
    {
        DrawVertex(position, Color.White);
    }

    public static void DrawVertex(Vector2 position, Color color, bool highlight = false)
    {
        if (highlight)
        {
            Top.Rect()
                .Center(position)
                .Width(16)
                .Height(16)
                .SizeAbs()
                .Color(color)
                .Fill();

            Top.Rect()
                .Center(position)
                .Width(10)
                .Height(10)
                .SizeAbs()
                .Color(Color.White)
                .Fill();
        }
        else
        {
            Top.Rect()
                .Center(position)
                .Width(6)
                .Height(6)
                .SizeAbs()
                .Color(color)
                .Fill();
        }
    }
}
