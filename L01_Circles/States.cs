using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace L01_Circles;

public class States
{
    public static bool IsPaused = false;

    public static bool IsStepRequested = false;

    public static Color _bg = Color.CornflowerBlue;

    public static Color _pausedBg = new Color(69, 102, 160);

    public static void Clear(GraphicsDevice graphicsDevice)
    {
        graphicsDevice.Clear(!IsPaused ? _bg : _pausedBg);
    }
}
