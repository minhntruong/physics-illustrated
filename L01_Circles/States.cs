using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace L01_Circles;

public class States
{
    public static bool IsPaused = false;

    public static bool IsStepRequested = false;

    public static bool IsMouseEngaged = false;

    public static Color _bg = new Color(69, 102, 160);

    public static Color _pausedBg = Color.CornflowerBlue;

    public static void Clear(GraphicsDevice graphicsDevice)
    {
        graphicsDevice.Clear(!IsPaused ? _bg : _pausedBg);
    }
}
