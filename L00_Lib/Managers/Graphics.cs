using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace PhysicsIllustrated.Library.Managers;

public static class Graphics
{
    public static void Initialize(Game game, string fontName)
    {
        _game = game;
        _bot = new DrawImpl(game.GraphicsDevice, 3_000);
        _mid = new DrawImpl(game.GraphicsDevice, 10_000);
        _top = new DrawImpl(game.GraphicsDevice, 3_000);
        _fontName = fontName;

        OnLoadContent();

        game.Window.ClientSizeChanged += (sender, args) => OnViewportChange();
    }

    private static float _zoom = 1.0f;
    private static Vector2 _origin = Vector2.Zero;

    public static void SetZoom(float zoomInc, Vector2 screenPos)
    {
        var unzoomedWorldPos = (screenPos + Origin) / Zoom;

        Zoom += zoomInc;

        // Adjust Origin so the world position under the cursor stays fixed
        Origin = (unzoomedWorldPos * Zoom) - screenPos;
    }

    public static float Zoom
    {
        get { return _zoom; }
        set
        {
            _zoom = value;

            _zoom = Math.Clamp(_zoom, 0.1f, 5.0f);

            _bot.Zoom = value;
            _mid.Zoom = value;
            _top.Zoom = value;
        }
    }

    // Origin already has zoom applied
    public static Vector2 Origin
    {
        get { return _origin; }
        set
        {
            _origin = value;
            _bot.Origin = value;
            _mid.Origin = value;
            _top.Origin = value;
        }
    }

    public static void Dispose()
    {
    }

    private static Game _game;
    private static DrawImpl _bot;
    private static DrawImpl _mid;
    private static DrawImpl _top;
    private static TextImpl _text;
    private static string _fontName;

    public static DrawImpl Bot => _bot;
    public static DrawImpl Mid => _mid;
    public static DrawImpl Top => _top;
    public static TextImpl Text => _text;

    public static void DrawVertex(Vector2 position)
    {
        _top.P0(position).Color(Color.White).Width(6).Filled().DrawSquare();
    }

    public static void DrawVertexHighlighted(Vector2 position)
    {
        _top.P0(position).Color(Color.White).Width(10).Thickness(2).Filled(false).DrawSquare();
    }

    public static void DrawVertex(Vector2 position, Color color)
    {
        _top.P0(position).Color(color).Width(6).Filled().DrawSquare();
    }

    public static void DrawVectorAbs(Vector2 p0, Vector2 p1, Color color)
    {
        _mid.Color(color).P0(p0).P1(p1).DrawVector();
    }

    public static void DrawVectorRel(Vector2 position, Vector2 vector, Color color)
    {
        _mid.Color(color).P0(position).P1(position + vector).DrawVector();
    }

    public static void Draw()
    {
        _bot.Draw();
        _mid.Draw();
        _top.Draw();
        _text.Draw();
    }

    public static void OnViewportChange()
    {
        _mid.OnViewportChange();
        _top.OnViewportChange();
    }

    public static void OnLoadContent()
    {
        var spriteBatch = new SpriteBatch(_game.GraphicsDevice);
        var font = _game.Content.Load<SpriteFont>(_fontName);

        _text = new TextImpl(spriteBatch, font);
    }
}