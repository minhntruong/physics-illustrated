using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PhysicsIllustrated.Library.Managers;
using PhysicsIllustrated.Library.Physics;
using PhysicsIllustrated.Library.Physics.Constraints;
using PhysicsIllustrated.Library.Physics.Shapes;
using System.Collections.Generic;
using static PhysicsIllustrated.Library.Managers.GameExt;
using static PhysicsIllustrated.Library.Physics.Constants;
using I = PhysicsIllustrated.Library.Illustrators;

namespace L01_Circles;

public class Game1 : Game
{
    public Game1()
    {
        // Creation of GraphicsDeviceManager must be done in the constructor
        GameExt.Initialize(this);

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    private List<Body> _bodies = new List<Body>();
    private List<Contact> _contacts = new List<Contact>();
    private Body _circle1;
    private Body _circle2;
    private I.Circles _illustrator = new I.Circles();

    protected override void Initialize()
    {
        Configure(1600, 900);

        Graphics.Initialize(this, "Lib_Shared_Arial_24"); // "Proj_Arial_12");

        _circle1 = new Body(new CircleShape(150), Width() / 2, Height() / 2, 1.0f);
        _circle2 = new Body(new CircleShape(100), Width() - 100 - 10, 100 + 10, 1.0f);

        _bodies.Add(_circle1);
        _bodies.Add(_circle2);

        _illustrator.Add(_circle1);
        _illustrator.Add(_circle2);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        Graphics.OnLoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        if (Input.IsDefaultExitInput()) { Exit(); }

        Input.Update();

        StatesOnInputs();
        var dt = ProcessGameTime(gameTime);

        if (States.IsMouseEngaged)
        {
            _circle2.Position = Input.MousePosition();
        }

        //=== Check collisions =================================================

        _contacts.Clear();

        for (var i = 0; i < _bodies.Count; i++)
        {
            for (var j = i + 1; j < _bodies.Count; j++)
            {
                var a = _bodies[i];
                var b = _bodies[j];

                a.IsColliding = false;
                b.IsColliding = false;

                if (CollisionDetection.IsColliding(a, b, _contacts))
                {
                    a.IsColliding = true;
                    b.IsColliding = true;
                }
            }
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        //GraphicsDevice.Clear(Color.CornflowerBlue);
        States.Clear(GraphicsDevice);

        var s = @"
'M' to dis/engage the mouse
'P' to toggle the pointer
";
        Graphics.Text.Position(20, 20).Scale(0.75f).Text(s);

        // Draw all bodies
        foreach (var body in _bodies)
        {
            var color = Color.Cyan;

            color = body.IsColliding ? Color.Orange : color;

            //if (body.Shape.Type == ShapeType.Circle)
            if (body.Shape is CircleShape circleShape)
            {
                Graphics.Mid.DrawCircle(body.Position, circleShape.Radius, -1, color);
            }
            else if (body.Shape is PolygonShape polygonShape)
            {
                Graphics.Mid.DrawPolygon(body.Position, polygonShape.WorldVertices, color);
            }
        }

        _illustrator.Draw();

        Graphics.Draw();

        base.Draw(gameTime);
    }

    private void StatesOnInputs()
    {
        ToggleOnKeyClicked(Keys.M, ref States.IsMouseEngaged);
        ToggleOnKeyClicked(Keys.P, ref States.IsPaused);
        SetByKeyClicked(Keys.S, ref States.IsStepRequested);
    }

    private float ProcessGameTime(GameTime gameTime)
    {
        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (dt > SECS_PER_FRAME)
        {
            dt = SECS_PER_FRAME;
        }

        if (States.IsPaused && States.IsStepRequested == false)
        {
            dt = 0.0f;
        }

        if (States.IsPaused && States.IsStepRequested)
        {
            dt = SECS_PER_FRAME;
        }

        return dt;
    }
}
