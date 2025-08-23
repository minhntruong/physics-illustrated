using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PhysicsIllustrated.Library.Illustrators;
using PhysicsIllustrated.Library.Managers;
using PhysicsIllustrated.Library.Physics;
using PhysicsIllustrated.Library.Physics.Shapes;
using static PhysicsIllustrated.Library.Managers.GameExt;
using static PhysicsIllustrated.Library.Illustrators.CirclePolyIllustrator.ModeEnum;
using System.Diagnostics;

namespace L02_Circle_Polygon;

public class Game1 : Game
{
    public Game1()
    {
        // Creation of GraphicsDeviceManager must be done in the constructor
        GameExt.Initialize(this);

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    private Body _box;
    private Body _movable;
    private CirclePolyIllustrator _illustrator;

    private bool _showMenu = true;
    private bool _isPanning = false;
    private Vector2 _panStartMouse;
    private Vector2 _panStartOrigin;

    private bool _isDragging;
    private Vector2 _dragStartMouse;
    private Vector2 _dragStartOrigin;

    private string _menuText = 
        "Mouse press to move\r\n" + 
        "Mouse wheel to rotate\r\n" + 
        "'.' to toggle menu\r\n" +
        "'M' to cycle the modes\r\n" +
        "'S' to step through the process\r\n" + 
        "'X' to reset the process\r\n" + 
        "'C' to clear text\r\n";

    private string _consoleText = "";

    protected override void Initialize()
    {
        Configure(1600, 900);

        Graphics.Initialize(this, "Lib_Shared_Arial_24");

        _illustrator = new CirclePolyIllustrator(GraphicsDevice);
        
        _box = new Body(new BoxShape(250, 250), Width() / 2, Height() / 2, 1.0f);
        _box.Rotation = 0.5f;
        _movable = new Body(new CircleShape(80), Width() * 0.8f, 100, 1.0f);

        _illustrator.Bodies.Add(_box);
        _illustrator.Bodies.Add(_movable);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        Graphics.OnLoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        //=== UI logic =========================================================

        Input.Update();

        bool isCtrlDown = Input.IsKeyDown(Keys.LeftControl) || Input.IsKeyDown(Keys.RightControl);
        var mousePos = Input.MousePosition();

        // --- Panning logic start ---
        if (isCtrlDown && Input.MouseLeftButtonPressed())
        {
            if (!_isPanning)
            {
                _isPanning = true;
                _panStartMouse = mousePos;
                _panStartOrigin = Graphics.Origin;
            }
            else
            {
                var delta = mousePos - _panStartMouse;
                Graphics.Origin = _panStartOrigin - delta;
            }
        }
        else
        {
            _isPanning = false;
        }

        //--- Dragging object logic start ---
        if (!isCtrlDown && Input.MouseLeftButtonPressed())
        {
            if (!_isDragging)
            {
                _isDragging = true;
                _dragStartMouse = mousePos;
                _dragStartOrigin = _movable.Position;
            }
            else
            {
                var delta = mousePos - _dragStartMouse;
                _movable.Position = _dragStartOrigin + delta / Graphics.Zoom;
            }
        }
        else
        {
            _isDragging = false;
        }

        IsMouseVisible = !_isDragging;

        var wheelDelta = Input.MouseScrollWheelDelta();

        if (wheelDelta != 0)
        {
            if (isCtrlDown)
            {
                // Wheel rotation with Ctrl
                Graphics.SetZoom(wheelDelta * 0.001f, Input.MousePosition());

                Debug.WriteLine("Zoom: " + Graphics.Zoom);
            }
            else
            {
                // Free wheel rotation
                _box.Rotation -= Input.MouseScrollWheelDelta() * 0.001f;
                if (_box.Rotation < 0)
                {
                    _box.Rotation += MathF.PI * 2;
                }
                else if (_box.Rotation > MathF.PI * 2)
                {
                    _box.Rotation -= MathF.PI * 2;
                }
            }
        }

        if (_illustrator.Mode == RunAllSteps)
        {
            if (Input.IsKeyClicked(Keys.S) || Input.MouseRightButtonClicked())
            {
                var s = _illustrator.StepProcess();
                if (s != null)
                {
                    _consoleText += "\r\n" + s.Step;

                    if (s.CollisionDetected.HasValue)
                    {
                        _consoleText +=
                            "\r\n" +
                            "Collision result: " + s.CollisionDetected.Value;
                    }

                    if (s.ProcessEnded)
                    {
                        _illustrator.ProcessReset();
                    }
                }
            }
        }
        else if (_illustrator.Mode == StopOnEdgeWithDraw ||
                 _illustrator.Mode == StopOnEdgeOnly)
        {
            _illustrator.EdgeProcess();
        }

        if (Input.IsKeyClicked(Keys.X))
        {
            _illustrator.Mode = RunAllSteps;
            _illustrator.ProcessReset();
        }

        if (Input.IsKeyClicked(Keys.C))
        {
            _consoleText = "";
        }

        if (Input.IsKeyClicked(Keys.M))
        {
            _illustrator.CycleMode();
        }

        if (Input.IsKeyClicked(Keys.OemPeriod))
        {
            _showMenu = !_showMenu;
        }

        //=== Illusatrator logic ===============================================

        _illustrator.Update(gameTime);

        //=== Base logic =======================================================

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        Graphics.UI.Position(20, 20).Text(
            (_showMenu ? _menuText +
            "Mode: " + _illustrator.Mode.ToString() + "\r\n"
            : "") +
            _consoleText);

        _illustrator.PreDraw();
        _illustrator.Draw();

        Graphics.Draw();

        base.Draw(gameTime);
    }

    //======================================================================

}
