using System;
using Microsoft.Xna.Framework;

namespace ShowPhysics.Library.Managers;

public class GameExt
{
    private static Game _game;
    private static GraphicsDeviceManager _graphics;

    public static void Construct(Game game)
    {
        if (game as IGameExt == null)
            throw new ArgumentException("Game must implement IGameExt interface.", nameof(game));

        _game = game;
        _graphics = new GraphicsDeviceManager(game);
    }

    public static void Initialize(int width, int height, Action configure)
    {
        if (_game == null) 
            throw new InvalidOperationException("Game not initialized. Call Initialize first.");

        _graphics.PreferredBackBufferWidth = width;
        _graphics.PreferredBackBufferHeight = height;
        _graphics.ApplyChanges();

        configure?.Invoke();

        (_game as IGameExt).RaiseWindowClientSizeChanged();
    }

    public static int Width()
    {
        return _game.GraphicsDevice.Viewport.Width;
    }

    public static int Height()
    {
        return _game.GraphicsDevice.Viewport.Height;
    }

    public static void StartPan(Vector2 screenPos)
    {
        _panStartMouse = screenPos;
        _panStartOrigin = Camera.Origin;
    }

    private static Vector2 _panStartMouse;
    private static Vector2 _panStartOrigin;

    public static void DoPan(Vector2 screenPos)
    {
        var delta = screenPos - _panStartMouse;
        Camera.Origin = _panStartOrigin + delta;
    }

    public static void EndPan()
    {
    }
}
