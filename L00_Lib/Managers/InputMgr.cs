using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PhysicsIllustrated.Library.Managers;

public class InputMgr
{
    public InputMgr(Game game, int width, int height)
    {
        _game = game;

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

    public Vector2 MousePosition()
    {
        var isValid = MouseIsValid();

        if (isValid)
        {
            _validMousePos = new Vector2(_curMouseState.X, _curMouseState.Y);
            return _validMousePos;
        }
        else
        {
            return _validMousePos;

        }
    }

    public Vector2 MousePositionAbs()
    {
        return new Vector2(_curMouseState.X, _curMouseState.Y);
    }

    public float MouseScrollWheelDelta()
    {
        return _curMouseState.ScrollWheelValue - _prvMouseState.ScrollWheelValue;
    }

    public bool MouseIsValid(out Vector2 mousePos)
    {
        // Monogame will give us mouse coordinates outside the window

        var isValid = MouseIsValid();

        if (isValid)
        {
            mousePos.X = _curMouseState.X;
            mousePos.Y = _curMouseState.Y;
        }
        else
        {
            mousePos = Vector2.Zero;
        }

        return isValid;
    }

    //==========================================================================

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

    public bool MouseLeftButtonClicked()
    {
        if (!MouseIsValid()) { return false; }

        return _curMouseState.LeftButton == ButtonState.Pressed && 
               _prvMouseState.LeftButton == ButtonState.Released;
    }

    public bool MouseLeftButtonPressed()
    {
        if (!MouseIsValid()) { return false; }

        return _curMouseState.LeftButton == ButtonState.Pressed;
    }

    public bool MouseRightButtonClicked()
    {
        if (!MouseIsValid()) { return false; }

        return _curMouseState.RightButton == ButtonState.Pressed &&
               _prvMouseState.RightButton == ButtonState.Released;
    }

    public bool MouseRightButtonPressed()
    {
        if (!MouseIsValid()) { return false; }

        return _curMouseState.RightButton == ButtonState.Pressed;
    }

}
