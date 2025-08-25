using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhysicsIllustrated.Library.Managers;
using PhysicsIllustrated.Library.Physics;
using PhysicsIllustrated.Library.Physics.Shapes;

namespace PhysicsIllustrated.Library.Illustrators;

public class IllustratorBase
{
    public IllustratorBase(GraphicsDevice graphicsDevice)
    {
        GraphicsDevice = graphicsDevice;    
    }

    protected GraphicsDevice GraphicsDevice { get; private set; }
    
    public List<Body> Bodies { get; } = new List<Body>();

    protected List<Contact> _contacts = new List<Contact>();

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

        Graphics.Mid.Thickness(2).Default();

        foreach (var body in Bodies)
        {
            var color = Theme.Shape;

            //color = body.IsColliding ? Color.Orange : color;

            if (body.Shape is CircleShape circleShape)
            {
                Graphics.Mid
                    .P0(body.Position)
                    .Color(color)
                    .Radius(circleShape.Radius)
                    .DrawCircle();

                Graphics.DrawVertex(body.Position, color);
            }
            else if (body.Shape is PolygonShape polygonShape)
            {
                Graphics.Mid
                    .P0(body.Position)
                    .Color(color)
                    .DrawPolygon(polygonShape.WorldVertices);

                Graphics.DrawVertex(body.Position, color);
            }
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
}