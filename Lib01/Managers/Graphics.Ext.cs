using System;
using Microsoft.Xna.Framework;
using ShowPhysics.Library.Managers.Text;
using ShowPhysics.Library.Physics;
using ShowPhysics.Library.Physics.Shapes;

namespace ShowPhysics.Library.Managers;

public static partial class Graphics
{
    public static void DrawLabeledDistance(Body a, Body b, bool label = true)
    {
        Bot.Line().Start(a.Position).End(b.Position).Color(Theme.ContactDistance).ThicknessAbs(4).Stroke();

        if (label)
            Text.LengthBetween(a.Position, b.Position).Color(Theme.Label).Scale(Theme.LabelScale);
    }

    public static void DrawLabeledDistance(Body a, Body b, float threshold, bool label = true)
    {
        var color = Theme.ContactDistance;

        if ((b.Position - a.Position).Length() < threshold)
        {
            color = Theme.ContactDistanceThreshold;
        }

        Bot.Line().Start(a.Position).End(b.Position).Color(color).ThicknessAbs(4).Stroke();

        if (label)
            Text.LengthBetween(a.Position, b.Position).Color(Theme.Label).Scale(Theme.LabelScale);
    }

    public static void DrawRadius(Body a)
    {
        if (a.Shape is CircleShape circle)
        {
            var end = a.Position + new Vector2(-circle.Radius, 0);

            Bot.Line()
                .Start(a.Position)
                .End(end)
                .Color(Theme.Shape)
                .ThicknessAbs(2)
                .Stroke();

            Text.LengthBetween(a.Position, end).Color(Theme.Label).Scale(Theme.LabelScale);
        }
    }

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
