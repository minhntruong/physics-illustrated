using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShowPhysics.Library.Managers;
using ShowPhysics.Library.Physics;
using ShowPhysics.Library.Physics.Shapes;
using ShowPhysics.Library.Physics.Steppables;

namespace ShowPhysics.Library.Shows;

public class ShowCircles : ShowBase
{
    public ShowCircles(GraphicsDevice graphicsDevice) : base(graphicsDevice)
    {
        Name = "Circle-to-Circle Collision Detection";

        var circle = new Body(new CircleShape(200), Width() / 2, Height() / 2, 1.0f);
        _movable = new Body(new CircleShape(100), Width() - 100 - 10, 100 + 10, 1.0f);

        Bodies.Add(circle);
        Bodies.Add(_movable);

        Menu += " | I = toggle radii";
    }

    private Body _movable;
    private bool _showRadii = true;

    //==========================================================================

    public override void Update(GameTime gameTime)
    {
        if (Input.IsKeyClicked(Keys.I))
        {
            _showRadii = !_showRadii;
        }

        CheckMovableObject();

        base.Update(gameTime);
    }

    public override void Draw()
    {
        base.Draw();

        if (_showRadii)
        {
            Graphics.DrawRadius(Bodies[0]);
            Graphics.DrawRadius(_movable);
        }

        ProcessStepDraws();

        // TODO: remove
        _currentStep?.Draw?.Invoke();

    }

    //==========================================================================

    protected override void InitializeSteps()
    {
        _steps = CollisionDetectionSteppable.IsCollidingCircles(Bodies[0], _movable, _contacts).GetEnumerator();
        base.InitializeSteps();
    }

    protected override void OnStepAdvanced()
    {
        if (_currentStep == null) { return; }

        Console(_currentStep.Text);

        if (_currentStep.IsColliding.HasValue)
        {
            _showRadii = false;
        }
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
