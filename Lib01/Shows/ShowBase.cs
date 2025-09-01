using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShowPhysics.Library.Managers;
using ShowPhysics.Library.Physics;
using ShowPhysics.Library.Physics.Shapes;
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
    
    public List<Body> Bodies { get; } = new List<Body>();

    protected List<Contact> _contacts = new List<Contact>();

    protected bool _menuVisible = true;
    protected string _menuText =
        ". = toggle menu";

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
        }

        if (_menuVisible)
        {
            Graphics.UI.Position(20, 20).Text(_menuText);
        }
    }
}

public static class Theme
{
    public static Color Bg { get; } = new Color(69, 102, 160);

    public static Color BgAnnotations { get; } = new Color(80, 118, 183);

    public static Color Shape { get; } = Color.Cyan;

    public static Color EdgeSelected { get; } = Color.HotPink;

    public static Color Projection { get; } = Color.Orange;

    public static Color Normals { get; } = Color.CornflowerBlue;

    public static Color ContactStart { get; } = Color.GreenYellow;

    public static Color ContactEnd { get; } = Color.MonoGameOrange;

    public static Color ContactDepth { get; } = Color.CornflowerBlue;

    public static Color ContactDistance { get; } = Color.Orange;
}