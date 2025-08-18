using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhysicsIllustrated.Library.Physics;
using PhysicsIllustrated.Library.Physics.Shapes;
using System;

namespace PhysicsIllustrated.Library.Illustrators;

public class CirclePolyIllustrator : IllustratorBase
{
    public CirclePolyIllustrator(GraphicsDevice graphicsDevice) : base(graphicsDevice)
    {
    }

    private bool _started = false;
    private IEnumerator<CollisionStepResult> _steps;
    private CollisionStepResult _currentStep;

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    public override void Draw()
    {
        // Draw the shapes first
        base.Draw();

        if (_currentStep != null)
        {
            if (_currentStep.Draw != null)
            {
                _currentStep.Draw();
            }
        }

    }

    public CollisionStepResult StepProcess()
    {
        if (!_started)
        {
            _contacts.Clear();

            var (poly, circle) = GetBodies();
            _steps = CollisionDetectionSteppable.IsCollidingPolygonCircle(poly, circle, _contacts).GetEnumerator();

            _started = true;
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

        return _currentStep;
    }

    public void EndProcess()
    {
        if (_started)
        {
            _currentStep = null;
            _steps.Dispose();
            _steps = null;
            _started = false;
        }
    }

    private (Body Poly, Body Circle) GetBodies()
    {
        if (Bodies.Count < 2)
        {
            throw new InvalidOperationException("There must be at least two bodies to process collision steps.");
        }

        var poly = Bodies.Where(x => x.Shape is PolygonShape).FirstOrDefault();
        var circle = Bodies.Where(x => x.Shape is CircleShape).FirstOrDefault(); ;

        if (poly == null ||circle == null)
        {
            throw new InvalidOperationException("Polygon and/or circle not found");
        }

        return (poly, circle);
    }
}