using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace ShowPhysics.Library.Managers;

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

}
