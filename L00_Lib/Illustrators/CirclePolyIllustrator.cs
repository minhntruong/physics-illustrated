using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhysicsIllustrated.Library.Managers;
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

    private Func<Vector2> _minCurrVertex = null;
    private Func<Vector2> _minNextVertex = null;

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    public override void Draw()
    {
        // Draw the shapes first
        base.Draw();

        //if (_currentStep == null)
        //{
        //    return;
        //}

        _currentStep?.Draw?.Invoke();
    
        if (_minCurrVertex != null)
            Graphics.DrawVertexHighlighted(_minCurrVertex());

        if (_minNextVertex != null)
            Graphics.DrawVertexHighlighted(_minNextVertex());
    }

    public CollisionStepResult StepProcess()
    {
        if (!_started)
        {
            InitializeProcess();
            _started = true;
        }

        if (_steps.MoveNext())
        {
            _currentStep = _steps.Current;

            if (_currentStep.MinCurrVertex != null)
            {
                _minCurrVertex = _currentStep.MinCurrVertex;
            }

            if (_currentStep.MinNextVertex != null)
            {
                _minNextVertex = _currentStep.MinNextVertex;
            }
        }
        else
        {
            _started = false;
            _currentStep = null;
        }

        return _currentStep;
    }

    public void StepProcessEnd()
    {
        if (_started)
        {
            _currentStep = null;
            _steps.Dispose();
            _steps = null;
            _started = false;
        }
    }

    public CollisionStepResult EdgeProcess()
    {
        InitializeProcess();

        var contToRun = false;
        do
        {
            contToRun = _steps.MoveNext();

            if (contToRun)
            {
                var currentStep = _steps.Current;
                if (currentStep.MinCurrVertex != null)
                {
                    _minCurrVertex = currentStep.MinCurrVertex;
                    _minNextVertex = currentStep.MinNextVertex;

                    _currentStep = currentStep;
                }
            }
        }
        while (contToRun);

        return _currentStep;
    }

    //==========================================================================

    private void InitializeProcess()
    {
        _contacts.Clear();

        _minCurrVertex = null;
        _minNextVertex = null;

        var (poly, circle) = GetBodies();
        _steps = CollisionDetectionSteppable.IsCollidingPolygonCircle(poly, circle, _contacts).GetEnumerator();
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