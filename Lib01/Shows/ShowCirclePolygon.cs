using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShowPhysics.Library.Managers;
using ShowPhysics.Library.Physics;
using ShowPhysics.Library.Physics.Shapes;
using ShowPhysics.Library.Physics.Steppables;
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

    //==========================================================================

    public override void Update(GameTime gameTime)
    {
        CheckMovableObject();

        base.Update(gameTime);
    }
    
    public override void Draw()
    {
        base.Draw();

        if (_currentStep == null) { return; }

        // Handle step-specific drawing
        var step = _currentStep as StepCirclePoly;
        if (step.SelectedEdge.HasValue)
        {
            var edge = step.SelectedEdge.Value;
            var shape = _box.Shape as PolygonShape;

            var v1 = shape.WorldVertices[edge];
            var v2 = shape.WorldVertexAfter(edge);

            Graphics.Mid.Line().Start(v1).End(v2).Color(Theme.EdgeSelected).Thickness(2).Stroke();
        }
    }

    //==========================================================================

    protected override void InitializeSteps()
    {
        _steps = CollisionDetectionSteppable.IsCollidingPolygonCircle(_box, _movable, _contacts).GetEnumerator();
        base.InitializeSteps();
    }

    protected override void OnStepAdvanced()
    {
        if (_currentStep == null) { return; }

        Console(_currentStep.Name);
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
