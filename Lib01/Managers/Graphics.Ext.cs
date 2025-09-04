using System;
using Microsoft.Xna.Framework;
using ShowPhysics.Library.Managers.Text;
using ShowPhysics.Library.Physics;
using ShowPhysics.Library.Physics.Shapes;

namespace ShowPhysics.Library.Managers;

public static partial class Graphics
{
    public static void Distance(Body a, Body b)
    {
        Bot.Line().Start(a.Position).End(b.Position).Color(Theme.ContactDistance).ThicknessAbs(4).Stroke();
        Text.LengthBetween(a.Position, b.Position).Color(Theme.Label).Scale(Theme.LabelScale);
    }

    public static void Distance(Body a, Body b, float threshold)
    {
        var color = Theme.ContactDistance;

        if ((b.Position - a.Position).Length() < threshold)
        {
            color = Theme.ContactDistanceThreshold;
        }

        Bot.Line().Start(a.Position).End(b.Position).Color(color).ThicknessAbs(4).Stroke();
        Text.LengthBetween(a.Position, b.Position).Color(Theme.Label).Scale(Theme.LabelScale);
    }

    public static void Radius(Body a)
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
}
