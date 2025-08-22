using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShowPhysics.Library.Managers.Draw;
using ShowPhysics.Library.Managers.Text;
using static ShowPhysics.Library.Managers.Text.TextImpl;

namespace ShowPhysics.Library.Managers;

public static class Graphics
{
    public static void Initialize(Game game, string fontName)
    {
        _game = game;
        _fontName = fontName;
        _bot = new DrawImpl(game.GraphicsDevice, 3_000);
        _mid = new DrawImpl(game.GraphicsDevice, 10_000);
        _top = new DrawImpl(game.GraphicsDevice, 3_000);

        OnLoadContent();

        game.Window.ClientSizeChanged += (sender, args) => OnViewportChange();

    }

    private static Game _game;
    private static string _fontName;

    private static DrawImpl _top;
    private static DrawImpl _mid;
    private static DrawImpl _bot;
    
    private static TextImpl _text;
    private static TextBuilder _textBuilder;
    private static TextImpl _ui;
    private static TextBuilder _uiBuilder;

    private static float _zoom = 1.0f;
    private static Vector2 _origin = Vector2.Zero;

    //==========================================================================

    public static DrawImpl Top => _top;
    public static DrawImpl Mid => _mid;
    public static DrawImpl Bot => _bot;
    public static TextBuilder Text => _textBuilder;
    public static TextBuilder UI => _uiBuilder;

    //==========================================================================

    public static void Draw()
    {
        _bot.Draw();
        _mid.Draw();
        _top.Draw();
        _text.Draw();
        _ui.Draw();
    }

    //=== Game events ==========================================================

    public static void OnLoadContent()
    {
        var font = _game.Content.Load<SpriteFont>(_fontName);

        var spriteBatch1 = new SpriteBatch(_game.GraphicsDevice);

        _text = new TextImpl(spriteBatch1, font);
        _textBuilder = new TextBuilder(_text);
        _textBuilder.Anchor(TextAnchor.Center).Default();

        var spriteBatch2 = new SpriteBatch(_game.GraphicsDevice);

        _ui = new TextImpl(spriteBatch2, font);
        _uiBuilder = new TextBuilder(_ui, false);
        _uiBuilder.Anchor(TextAnchor.TopLeft).Scale(0.5f).Default();
    }

    public static void OnViewportChange()
    {
        _bot.OnViewportChange();
        _mid.OnViewportChange();
        _top.OnViewportChange();
    }
}
