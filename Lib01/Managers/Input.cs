using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ShowPhysics.Library.Managers;

public static class Input
{
    public static void Initialize(Game game)
    {
        _game = game;

        (game as IGameExt).WindowClientSizeChanged += (sender, args) => OnWindowSizeChanged();

        _curMouseState = Mouse.GetState();
        _curKbState = Keyboard.GetState();
    }

    private static Game _game;
    private static int _width, _height;
    private static bool _exitHandler = true;

    private static bool _isPanning = false;
    private static Vector2 _panStartMouse;
    private static Vector2 _panStartOrigin;

    private static bool _isObjDragging = false;
    private static Vector2 _objDraggingStartWorld = Vector2.Zero;
    private static Vector2 _objDraggingDeltaWorld = Vector2.Zero;

    private static MouseState _prvMouseState, _curMouseState;
    private static KeyboardState _prvKbState, _curKbState;
    private static Vector2 _validMousePos = Vector2.Zero;

    public static void Update()
    {
        _prvMouseState = _curMouseState;
        _curMouseState = Mouse.GetState();
        _prvKbState = _curKbState;
        _curKbState = Keyboard.GetState();

        if (_exitHandler && IsDefaultExitInput()) { _game.Exit(); }
    }

    public static bool IsKeyDown(Keys key)
    {
        return _curKbState.IsKeyDown(key);
    }

    public static bool IsKeyClicked(Keys key)
    {
        return _curKbState.IsKeyDown(key) && !_prvKbState.IsKeyDown(key);
    }

    public static void CheckMousePanCamera()
    {
        bool isCtrlDown = IsKeyDown(Keys.LeftControl) || IsKeyDown(Keys.RightControl);

        if (_isPanning == false)
        {
            if (isCtrlDown && IsMouseLeftButtonPressedInside())
            {
                _isPanning = true;
                _panStartMouse = MousePosition();
                _panStartOrigin = Camera.Origin;
            }
        }
        else
        {
            // Panning in progress
            if (isCtrlDown && IsMouseLeftButtonPressed())
            {
                var mousePos = MousePosition();
                var delta = (mousePos - _panStartMouse) / Camera.Zoom;
                Camera.Origin = _panStartOrigin - delta;
            }
            else
            {
                _isPanning = false;
            }
        }
    }

    public static void CheckObjectDraggingStart(Vector2 objPosWorld)
    {
        if (_isObjDragging) { return; }

        bool isCtrlDown = IsKeyDown(Keys.LeftControl) || IsKeyDown(Keys.RightControl);

        if (isCtrlDown) { return; }

        if (IsMouseLeftButtonPressedInside() == false) { return; }

        var mousePos = MousePosition();
        var mousePosWorld = (mousePos + Camera.Origin) / Camera.Zoom  ;

        _isObjDragging = true;
        _objDraggingStartWorld = objPosWorld;
        _objDraggingDeltaWorld = mousePosWorld - _objDraggingStartWorld;

        _game.IsMouseVisible = false;
    }

    public static bool IsObjectDragging(out Vector2 objPosWorld)
    {
        objPosWorld = Vector2.Zero;

        if (_isObjDragging == false) { return false; }

        if (IsMouseLeftButtonPressed() == false)
        {
            _isObjDragging = false;

            _game.IsMouseVisible = true;
            return false;
        }

        var mousePos = MousePosition();
        var mousePosWorld = (mousePos + Camera.Origin) / Camera.Zoom;

        objPosWorld = (mousePosWorld - _objDraggingDeltaWorld);

        return true;
    }

    public static void CheckMouseZoomCamera()
    {
        var wheelDelta = MouseScrollWheelDelta();

        if (wheelDelta == 0) { return; }

        bool isCtrlDown = IsKeyDown(Keys.LeftControl) || IsKeyDown(Keys.RightControl);

        if (!isCtrlDown) { return; }

        Camera.SetZoomFocus(wheelDelta * 0.001f, MousePosition());
    }

    //=== Mouse ================================================================

    public static Vector2 MousePosition()
    {
        return new Vector2(_curMouseState.X, _curMouseState.Y);
    }

    public static bool IsMouseLeftButtonPressed()
    {
        return _curMouseState.LeftButton == ButtonState.Pressed;
    }

    public static bool IsMouseLeftButtonPressedInside()
    {
        if (!IsMouseInside()) { return false; }

        return _curMouseState.LeftButton == ButtonState.Pressed;
    }

    public static bool IsMouseRightButtonClickedInside()
    {
        if (!IsMouseInside()) { return false; }

        return _prvMouseState.RightButton == ButtonState.Released &&
               _curMouseState.RightButton == ButtonState.Pressed;
    }

    //==========================================================================

    private static void OnWindowSizeChanged()
    {
        _width = _game.GraphicsDevice.Viewport.Width;
        _height = _game.GraphicsDevice.Viewport.Height;
    }

    private static bool IsDefaultExitInput()
    {
        var isIt = GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape);
        return isIt;
    }

    //=== Mouse ================================================================

    private static int MouseScrollWheelDelta()
    {
        return _curMouseState.ScrollWheelValue - _prvMouseState.ScrollWheelValue;
    }

    private static bool IsMouseInside()
    {
        return
            _curMouseState.X >= 0 && _curMouseState.X < _width &&
            _curMouseState.Y >= 0 && _curMouseState.Y < _height;
    }
}
