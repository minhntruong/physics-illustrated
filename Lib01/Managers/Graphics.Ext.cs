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
        Text.LengthBetween(a.Position, b.Position).Color(Theme.ContactDistance).Scale(0.8f);
    }
}
