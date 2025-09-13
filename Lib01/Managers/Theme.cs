using Microsoft.Xna.Framework;
using System;

namespace ShowPhysics.Library.Managers;

public static class Theme
{
    public static Color Bg { get; } = new Color(69, 102, 160);

    public static Color BgAnnotations { get; } = new Color(80, 118, 183);

    public static float ShapeLineThicknessAbs = 2;

    public static float ShapeOverlayLineThicknessAbs = 4;

    public static Color Shape { get; } = Color.Cyan;

    public static Color EdgeSelected { get; } = Color.HotPink;

    public static Color Projection { get; } = Color.Orange;

    public static Color Normals { get; } = Color.CornflowerBlue;

    public static Color ContactStart { get; } = Color.GreenYellow;

    public static Color ContactEnd { get; } = Color.MonoGameOrange;

    public static Color ContactDepth { get; } = Color.CornflowerBlue;

    public static Color ContactDistance { get; } = Color.Orange;

    public static Color ContactDistanceThreshold { get; } = Color.MonoGameOrange;

    public static Color Label { get; } = Color.White;

    public static float LabelScale { get; } = 0.8f;
}