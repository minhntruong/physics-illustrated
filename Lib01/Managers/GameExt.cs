using Microsoft.Xna.Framework;
using System;

namespace ShowPhysics.Library.Managers;

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

}
