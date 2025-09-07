using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShowPhysics.Library.Managers;
using ShowPhysics.Library.Physics;
using ShowPhysics.Library.Physics.Shapes;
using System;

namespace ShowPhysics.Library.Shows;

public class ShowCirclePolygon : ShowBase
{
    public ShowCirclePolygon(GraphicsDevice graphicsDevice) : base(graphicsDevice)
    {
        Name = "Circle-to-Polygon Collision Detection";

        _box = new Body(new BoxShape(250, 250), Width() / 2, Height() / 2, 1.0f);
        _box.Rotation = 0.5f;
        _movable = new Body(new CircleShape(80), Width() * 0.8f, 100, 1.0f);

        Bodies.Add(_box);
        Bodies.Add(_movable);
    }

    private Body _box;
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

        var wheelDelta = Input.MouseScrollWheelDelta();
        var isCtrlDown = Input.IsKeyDown(Keys.LeftControl) || Input.IsKeyDown(Keys.RightControl);

        if (wheelDelta != 0 && isCtrlDown == false)
        {
            // Free wheel rotation
            var r = _box.Rotation - wheelDelta * 0.001f;

            if (r < 0)
            {
                r += MathF.PI * 2;
            }
            else if (r > MathF.PI * 2)
            {
                r -= MathF.PI * 2;
            }

            _box.Rotation = r;
        }
    }
}
