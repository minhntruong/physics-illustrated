using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShowPhysics.Library.Managers;
using ShowPhysics.Library.Physics;
using ShowPhysics.Library.Physics.Math;
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
    private int? _facingEdgeIndex = null;
    private bool? _isCircleOutside = null;
    private bool _showRegions = true;

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

        // Handle draw commands
        if (_currentStep.Commands?.Count > 0)
        {
            foreach (var cmd in _currentStep.Commands)
            {
                cmd.Draw();
            }
        }

        // Handle show-side drawings
        CheckDrawRegions();
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

        if (_currentStep is StepCirclePoly stepCP)
        {
            if (stepCP.FacingEdgeIndex.HasValue)
            {
                _facingEdgeIndex = stepCP.FacingEdgeIndex.Value;
            }

            if (stepCP.IsCircleOutside.HasValue)
            {
                _isCircleOutside = stepCP.IsCircleOutside.Value;
            }
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

    private void CheckDrawRegions()
    {
        if (!_showRegions || !_facingEdgeIndex.HasValue || !_isCircleOutside.HasValue)
        {
            return;
        }

        // Draw the region between the two vertices
        var polyShape = (PolygonShape)_box.Shape;

        var v1 = polyShape.WorldVertices[_facingEdgeIndex.Value];
        var v2 = polyShape.WorldVertexAfter(_facingEdgeIndex.Value);

        var edge = v2 - v1;
        var edgeUnit = Vector2.Normalize(edge);
        var edgeNormal = edge.RightUnitNormal();

        Graphics.Bot.States().Color(Theme.BgAnnotations).ThicknessAbs(Theme.ShapeLineThicknessAbs).Default();

        // A base
        Graphics.Bot
            .Line()
            .Start(v1)
            .End((v1 - v2) * 1000)
            .Stroke();

        Graphics.Text.Color(Theme.Normals).RotationOf(v1, v2).Default();

        Graphics.Text
            .Position(v1 - edgeUnit * 100 + edgeNormal * 40)
            .Text("A");

        // A vertical
        Graphics.Bot.Line().Start(v1).End(v1 + edgeNormal * 1000).Stroke();

        // B base
        Graphics.Bot.Line()
            .Start(v2)
            .End((v2 - v1) * 1000)
            .Stroke();

        Graphics.Text
            .Position(v2 + edgeUnit * 100 + edgeNormal * 40)
            .Text("B");

        // B vertical
        Graphics.Bot.Line()
            .Start(v2)
            .End(v2 + edgeNormal * 1000)
            .Stroke();

        Graphics.Text
            .Position(v1 + edge * 0.5f + edgeNormal * 40)
            .Text("C");
    }
}
