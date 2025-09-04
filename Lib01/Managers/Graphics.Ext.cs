using System;
using Microsoft.Xna.Framework;
using ShowPhysics.Library.Managers.Text;
using ShowPhysics.Library.Physics;
using ShowPhysics.Library.Physics.Shapes;
using ShowPhysics.Library.Shows;

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
}
