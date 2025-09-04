using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShowPhysics.Library.Managers;
using ShowPhysics.Library.Physics;
using ShowPhysics.Library.Physics.Shapes;
using ShowPhysics.Library.Physics.Steppables;
using System;
using System.Collections.Generic;

namespace ShowPhysics.Library.Shows;

public class ShowBase
{
    public ShowBase(GraphicsDevice graphicsDevice)
    {
        GraphicsDevice = graphicsDevice;    
    }

    protected GraphicsDevice GraphicsDevice { get; private set; }

    protected bool _started = false;
    protected IEnumerator<Step> _steps;
    protected Step _currentStep;

    public List<Body> Bodies { get; } = new List<Body>();

    protected List<Contact> _contacts = new List<Contact>();

    protected bool _menuVisible = true;
    protected string _menuText =
        ". = toggle menu | " +
        "R = reset camera | " +
        "S = step";

    protected string _consoleText = "";

    public int Width()
    {
        return GraphicsDevice.Viewport.Width;
    }

    public int Height()
    {
        return GraphicsDevice.Viewport.Height;
    }

    public virtual void PreUpdate()
    {
        Input.Update();

        Input.CheckMousePanCamera();
        Input.CheckMouseZoomCamera();

        if (Input.IsKeyClicked(Keys.R))
        {
            Camera.Zoom = 1f;
            Camera.Origin = Vector2.Zero;
        }

        if (Input.IsKeyClicked(Keys.OemPeriod))
        {
            _menuVisible = !_menuVisible;
        }

        if (Input.IsKeyClicked(Keys.S) || Input.IsMouseRightButtonClickedInside())
        {
            if (_started == false)
            {
                _started = true;
                InitializeSteps();
            }

            if (_steps.MoveNext())
            {
                _currentStep = _steps.Current;
            }
            else
            {
                _started = false;
                _currentStep = null;
            }

            if (_currentStep != null)
            {
                OnStepAdvanced();
            }
        }
    }

    public virtual void OnStepAdvanced()
    {

    }

    public virtual void Update(GameTime gameTime)
    {
        foreach (Body body in Bodies)
        {
            body.Shape.UpdateVertices(body.Rotation, body.Position);
        }

        //=== Check collisions =================================================

        _contacts.Clear();

        for (var i = 0; i < Bodies.Count; i++)
        {
            for (var j = i + 1; j < Bodies.Count; j++)
            {
                var a = Bodies[i];
                var b = Bodies[j];

                a.IsColliding = false;
                b.IsColliding = false;

                if (CollisionDetection.IsColliding(a, b, _contacts))
                {
                    a.IsColliding = true;
                    b.IsColliding = true;
                }
            }
        }
    }

    public virtual void PreDraw()
    {
        GraphicsDevice.Clear(Theme.Bg);
    }

    public virtual void Draw()
    {
        // Draw all bodies

        Graphics.Mid.States().ThicknessAbs(2).Default();

        foreach (var body in Bodies)
        {
            var color = Theme.Shape;

            //color = body.IsColliding ? Color.Orange : color;

            if (body.Shape is CircleShape circleShape)
            {
                Graphics.Mid.Circle()
                    .Center(body.Position)
                    .Color(color)
                    .Radius(circleShape.Radius)
                    .Stroke();

                //Graphics.DrawVertex(body.Position, color);
            }
            else if (body.Shape is PolygonShape polygonShape)
            {
                Graphics.Mid.Poly()
                    .Center(body.Position)
                    .Color(color)
                    .Coordinates(polygonShape.WorldVertices);

                //Graphics.DrawVertex(body.Position, color);
            }

            Graphics.Mid.Rect()
                .Center(body.Position)
                .Width(6)
                .Height(6)
                .SizeAbs()
                .Color(Color.White)
                .Fill();
        }

        if (_menuVisible)
        {
            Graphics.UI.Position(10, 10).Text(_menuText);
        }

        Graphics.UI.Position(10, 40).Text(_consoleText);
    }

    public virtual void InitializeSteps()
    {
        _contacts.Clear();
    }

    protected void Console(string text)
    {
        _consoleText += text + "\r\n";
    }
}