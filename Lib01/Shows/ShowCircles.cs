using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShowPhysics.Library.Managers;
using ShowPhysics.Library.Managers.Text;
using ShowPhysics.Library.Physics;
using ShowPhysics.Library.Physics.Shapes;
using ShowPhysics.Library.Physics.Steppables;
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

    public override void Draw()
    {
        Graphics.Mid.Line().Start(_movable.Position).End(Bodies[0].Position).Color(Theme.ContactDistance).ThicknessAbs(4).Stroke();

        Graphics.Text
            .LengthBetween(_movable.Position, Bodies[0].Position);

        base.Draw();
    }

    //==========================================================================

    public override void InitializeSteps()
    {
        _steps = CollisionDetectionSteppable.IsCollidingCircles(Bodies[0], _movable, _contacts).GetEnumerator();
        base.InitializeSteps();
    }

    private void CheckMovableObject()
    {
        Input.CheckObjectDraggingStart(_movable.Position);

        if (Input.IsObjectDragging(out var objPos))
        {
            _movable.Position = objPos;
        }
    }
}
