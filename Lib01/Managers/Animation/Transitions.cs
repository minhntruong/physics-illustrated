using Microsoft.Xna.Framework;
using System;

namespace ShowPhysics.Library.Managers.Animation;

public static class Transitions
{
    public static float Lerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }
}
