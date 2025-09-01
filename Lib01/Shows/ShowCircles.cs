using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShowPhysics.Library.Managers;
using ShowPhysics.Library.Physics;
using ShowPhysics.Library.Physics.Shapes;
using System;

namespace ShowPhysics.Library.Shows;

public class ShowCircles : ShowBase
{
    public ShowCircles(GraphicsDevice graphicsDevice) : base(graphicsDevice)
    {
        var circle = new Body(new CircleShape(200), Width() / 2, Height() / 2, 1.0f);
        _movable = new Body(new CircleShape(100), Width() - 100 - 10, 100 + 10, 1.0f);

        Bodies.Add(circle);
        Bodies.Add(_movable);
    }

    private Body _movable;

    public override void Update(GameTime gameTime)
    {
        CheckMovableObject();

        base.Update(gameTime);
    }

    //==========================================================================

    private void CheckMovableObject()
    {
        Input.CheckObjectDraggingStart(_movable.Position);

        if (Input.IsObjectDragging(out var objPos))
        {
            _movable.Position = objPos;
        }
    }
}
