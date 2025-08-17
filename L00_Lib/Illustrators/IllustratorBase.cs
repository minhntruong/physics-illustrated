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
        GraphicsDevice.Clear(new Color(69, 102, 160));
    }

    public virtual void Draw()
    {
        // Draw all bodies
        foreach (var body in Bodies)
        {
            var color = Color.Cyan;

            color = body.IsColliding ? Color.Orange : color;

            if (body.Shape is CircleShape circleShape)
            {
                Graphics.Mid.DrawCircle(body.Position, circleShape.Radius, -1, color);
            }
            else if (body.Shape is PolygonShape polygonShape)
            {
                Graphics.Mid.DrawPolygon(body.Position, polygonShape.WorldVertices, color);
            }
        }

    }
}