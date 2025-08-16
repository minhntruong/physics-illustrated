using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace PhysicsIllustrated.Library.Managers;

public static class Graphics
{
    public static void Initialize(Game game, string fontName)
    {
        _game = game;
        _mid = new DrawImpl(game.GraphicsDevice, 10_000);
        _top = new DrawImpl(game.GraphicsDevice, 3_000);
        _fontName = fontName;

        OnLoadContent();

        game.Window.ClientSizeChanged += (sender, args) => OnViewportChange();
    }

    public static void Dispose()
    {
    }

    private static Game _game;
    private static DrawImpl _mid;
    private static DrawImpl _top;
    private static TextImpl _text;
    private static string _fontName;

    public static DrawImpl Mid => _mid;
    public static DrawImpl Top => _top;
    public static TextImpl Text => _text;

    public static void Draw()
    {
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