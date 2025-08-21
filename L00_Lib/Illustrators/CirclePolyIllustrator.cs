using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhysicsIllustrated.Library.Managers;
using PhysicsIllustrated.Library.Physics;
using PhysicsIllustrated.Library.Physics.Mathematics;
using PhysicsIllustrated.Library.Physics.Shapes;

namespace PhysicsIllustrated.Library.Illustrators;

public class CirclePolyIllustrator : IllustratorBase
{
    public enum ModeEnum { RunAllSteps, StopOnEdgeWithDraw, StopOnEdgeOnly }

    public CirclePolyIllustrator(GraphicsDevice graphicsDevice) : base(graphicsDevice)
    {
    }

    private bool _started = false;
    private IEnumerator<CollisionStepResult> _steps;
    private CollisionStepResult _currentStep;

    private Func<Vector2> _minCurrVertex = null;
    private Func<Vector2> _minNextVertex = null;
    private bool _minEdgeFound = false;
    private bool _showRegions = true;
    private bool _isOutside = false;

    public ModeEnum Mode { get; set; } = ModeEnum.RunAllSteps;

    public void CycleMode()
    {
        // Get all enum values
        var values = (ModeEnum[])Enum.GetValues(typeof(ModeEnum));
        // Find the next index, wrapping around
        int next = (Array.IndexOf(values, Mode) + 1) % values.Length;
        Mode = values[next];
    }

    public void CycleRegion()
    {
        _showRegions = !_showRegions;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    private void DrawRegions()
    {
        if (_showRegions &&
         _minCurrVertex != null &&
         _minNextVertex != null &&
         _minEdgeFound &&
         _isOutside)
        {
            // Draw the region between the two vertices
            var minCurr = _minCurrVertex();
            var minNext = _minNextVertex();

            var edge = minNext - minCurr;
            var edgeUnit = Vector2.Normalize(edge);
            var edgeNormal = edge.RightUnitNormal();

            Graphics.Bot.Color(Theme.BgSubtle).Width(2).Default();

            // A base
            Graphics.Bot
                .P0(minCurr)
                .P1((minCurr - minNext) * 1000)
                .DrawLine();

            Graphics.Text.Color(Theme.ShapeLolite).RotationOf(minCurr, minNext).Default();

            Graphics.Text
                .Position(minCurr - edgeUnit * 100 + edgeNormal * 40)
                .Text("A");

            // A vertical
            Graphics.Bot.P0(minCurr).P1(minCurr + edgeNormal * 1000).DrawLine();

            // B base
            Graphics.Bot
                .P0(minNext)
                .P1((minNext - minCurr) * 1000)
                .DrawLine();

            Graphics.Text
                .Position(minNext + edgeUnit * 100 + edgeNormal * 40)
                .Text("B");

            // B vertical
            Graphics.Bot.P0(minNext).P1(minNext + edgeNormal * 1000).DrawLine();

            Graphics.Text
                .Position(minCurr + edge * 0.5f + edgeNormal * 40)
                .Text("C");
        }
    }

    public override void Draw()
    {
        // Draw the shapes first
        base.Draw();


        if (_minCurrVertex != null && _minNextVertex != null)
        {
            if (_minEdgeFound)
            {
                Graphics.Mid.P0(_minNextVertex()).P1(_minCurrVertex()).Color(Theme.ShapeHilite).Width(2).DrawLine();
            }

            Graphics.DrawVertexHighlighted(_minCurrVertex());
            Graphics.DrawVertexHighlighted(_minNextVertex());
        }

        DrawRegions();     

        if (Mode != ModeEnum.StopOnEdgeOnly)
        {
            _currentStep?.Draw?.Invoke();
        }
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
                _minEdgeFound = _currentStep.MinEdgeFound;
            }

            if (_currentStep.MinNextVertex != null)
            {
                _minNextVertex = _currentStep.MinNextVertex;
            }

            if (_currentStep.IsOutside.HasValue)
            {
                _isOutside = _currentStep.IsOutside.Value;
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

            _minCurrVertex = null;
            _minNextVertex = null;
            _minEdgeFound = false;
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