using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace ShowPhysics.Library.Managers;

public class InputMgr
{
    public InputMgr(Game game, int width, int height)
    {
        _game = game;

        game.Window.ClientSizeChanged += (sender, args) => OnWindowSizeChanged();

        _width = width;
        _height = height;

        _curMouseState = Mouse.GetState();
        _curKbState = Keyboard.GetState();
    }

    private Game _game;
    private int _width, _height;
    private bool _exitHandler = true;

    private MouseState _prvMouseState, _curMouseState;
    private KeyboardState _prvKbState, _curKbState;
    private Vector2 _validMousePos = Vector2.Zero;

    public void Update()
    {
        _prvMouseState = _curMouseState;
        _curMouseState = Mouse.GetState();
        _prvKbState = _curKbState;
        _curKbState = Keyboard.GetState();

        if (_exitHandler && IsDefaultExitInput()) { _game.Exit(); }
    }

    public bool IsKeyDown(Keys key)
    {
        return _curKbState.IsKeyDown(key);
    }

    public bool IsKeyClicked(Keys key)
    {
        return _curKbState.IsKeyDown(key) && !_prvKbState.IsKeyDown(key);
    }

    //==========================================================================

    private void OnWindowSizeChanged()
    {
        _width = _game.GraphicsDevice.Viewport.Width;
        _height = _game.GraphicsDevice.Viewport.Height;
    }

    private bool IsDefaultExitInput()
    {
        var isIt = GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape);
        return isIt;
    }

    private bool MouseIsValid()
    {
        return
            _curMouseState.X >= 0 && _curMouseState.X < _width &&
            _curMouseState.Y >= 0 && _curMouseState.Y < _height;
    }

}
