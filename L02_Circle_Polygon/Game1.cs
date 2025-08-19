using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PhysicsIllustrated.Library.Illustrators;
using PhysicsIllustrated.Library.Managers;
using PhysicsIllustrated.Library.Physics;
using PhysicsIllustrated.Library.Physics.Shapes;
using System;
using static PhysicsIllustrated.Library.Managers.GameExt;

namespace L02_Circle_Polygon;

public class Game1 : Game
{
    public enum Mode { RunAllSteps, EndOnEdgeFound }

    public Game1()
    {
        // Creation of GraphicsDeviceManager must be done in the constructor
        GameExt.Initialize(this);

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    private Mode _mode = Mode.RunAllSteps;
    private Body _box;
    private Body _movable;
    private CirclePolyIllustrator _illustrator;

    private string _menuText = 
        "Mouse press to move\r\n" + 
        "Mouse wheel to rotate\r\n" + 
        "'S' to step through the process\r\n" + 
        "'X' to end the process\r\n" + 
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

        var isMouseEngaged = Input.MouseLeftButtonPressed();
        if (isMouseEngaged)
        {
            _movable.Position = Input.MousePosition();
        }

        IsMouseVisible = !isMouseEngaged;

        _box.Rotation -= Input.MouseScrollWheelDelta() * 0.001f;
        if (_box.Rotation < 0)
        {
            _box.Rotation += MathF.PI * 2;
        }
        else if (_box.Rotation > MathF.PI * 2)
        {
            _box.Rotation -= MathF.PI * 2;
        }

        if (_mode == Mode.RunAllSteps)
        {
            if (Input.IsKeyClicked(Keys.S) || Input.MouseRightButtonClicked())
            {
                var s = _illustrator.StepProcess();
                if (s != null)
                {
                    _consoleText += "\r\n" + s.Step;
                }
            }

            if (Input.IsKeyClicked(Keys.X))
            {
                _illustrator.StepProcessEnd();
            }
        }
        else if (_mode == Mode.EndOnEdgeFound)
        {
            _illustrator.EdgeProcess();
        }

        if (Input.IsKeyClicked(Keys.C))
        {
            _consoleText = "";
        }

        if (Input.IsKeyClicked(Keys.M))
        {
            // Get all enum values
            var values = (Mode[])Enum.GetValues(typeof(Mode));
            // Find the next index, wrapping around
            int next = (Array.IndexOf(values, _mode) + 1) % values.Length;
            _mode = values[next];
        }


        //=== Illusatrator logic ===============================================

        _illustrator.Update(gameTime);

        //=== Base logic =======================================================

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        Graphics.Text.Position(20, 20).Scale(0.75f).Text(
            _menuText + 
            "Mode: " + _mode.ToString() + "\r\n" +
            _consoleText);

        _illustrator.PreDraw();
        _illustrator.Draw();

        Graphics.Draw();

        base.Draw(gameTime);
    }

    //======================================================================

}
