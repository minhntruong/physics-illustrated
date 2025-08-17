using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PhysicsIllustrated.Library.Managers;

public class GameExt
{
    private static Game _game;
    private static GraphicsDeviceManager _graphics;
    private static InputMgr _input;

    public static void Initialize(Game game)
    {
        _game = game;
        _graphics = new GraphicsDeviceManager(game);
    }

    public static void Configure(int width, int height)
    {
        if (_game == null) throw new InvalidOperationException("Game not initialized. Call Initialize first.");

        _graphics.PreferredBackBufferWidth = width;
        _graphics.PreferredBackBufferHeight = height;
        _graphics.ApplyChanges();

        _input = new InputMgr(_game, Width(), Height());
    }

    public static InputMgr Input => _input;

    public static int Width()
    {
        return _game.GraphicsDevice.Viewport.Width;
    }

    public static int Height()
    {
        return _game.GraphicsDevice.Viewport.Height;
    }

    public static void ToggleOnKeyClicked(Keys key, ref bool state)
    {
        if (_input.IsKeyClicked(key))
        {
            state = !state;
        }
    }

    public static void SetByKeyClicked(Keys key, ref bool state)
    {
        state = _input.IsKeyClicked(key);
    }

    public static void SetByMouseLeftButtonPressed(ref bool state)
    {
        state = _input.MouseLeftButtonPressed();
    }

}
