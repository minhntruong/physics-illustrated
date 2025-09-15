using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShowPhysics.Library.Managers;
using ShowPhysics.Library.Physics;
using ShowPhysics.Library.Physics.Math;
using ShowPhysics.Library.Physics.Shapes;
using ShowPhysics.Library.Physics.Steppables;
using static ShowPhysics.Library.Physics.Math.Extensions;

namespace ShowPhysics.Library.Shows;

public class ShowCirclePolygon : ShowBase
{
    public ShowCirclePolygon(GraphicsDevice graphicsDevice) : base(graphicsDevice)
    {
        Name = "Circle-to-Polygon Collision Detection";

        //_box = new Body(new BoxShape(250, 250), Width() / 2, Height() / 2, 1.0f);
        _box = new Body(PolygonShape.Create(250, 8), Width() / 2, Height() / 2, 1.0f);
        _box.Rotation = 0;
        _movable = new Body(new CircleShape(80), Width() * 0.8f, 100, 1.0f);

        Bodies.Add(_box);
        Bodies.Add(_movable);

        _fileName = $"view_{GetType().Name}.txt";

        _menu += " | V = save view";
        UpdateTitle();
    }

    private Body _box;
    private Body _movable;
    private int? _facingEdgeIndex = null;
    private bool _showRegions = true;
    private string _fileName;

    //==========================================================================

    public override void LoadContent()
    {
        if (File.Exists(_fileName))
        {
            var lines = File.ReadAllLines(_fileName);
            
            foreach (var line in lines)
            {
                var parts = line.Split('=');
                if (parts.Length != 2) { return; }
                var key = parts[0].Trim();
                var value = parts[1].Trim();

                if (key == "Origin" && Vector2TryParse(value, out var origin))
                {
                    Camera.Origin = origin;
                }
                else if (key == "Scale" && float.TryParse(value, out var scale))
                {
                    Camera.Zoom = scale;
                }
                else if (key == "PolyRotation" && float.TryParse(value, out var rotation))
                {
                    _box.Rotation = rotation;
                }
                else if (key == "CirclePosition" && Vector2TryParse(value, out var position))
                {
                    _movable.Position = position;
                }
            }
        }
    }

    public override void Update(GameTime gameTime)
    {
        CheckMovableObject();

        if (Input.IsKeyClicked(Keys.V))
        {
            File.WriteAllText(_fileName, $"Origin = {Camera.Origin}\nScale = {Camera.Zoom}\nPolyRotation = {_box.Rotation}\nCirclePosition = {_movable.Position}");
        }

        base.Update(gameTime);
    }
    
    public override void Draw()
    {
        base.Draw();

        if (_currentStep == null) { return; }

        // Handle draws
        if (_currentStep.Draws?.Count > 0)
        {
            foreach (var draw in _currentStep.Draws)
            {
                draw();
            }
        }

        // Handle show-side drawings
        CheckDrawRegions();

        //DrawAnimations();
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

        Console(_currentStep.Text);

        if (_currentStep is StepCirclePoly stepCP)
        {
            if (stepCP.FacingEdgeIndex.HasValue)
            {
                _facingEdgeIndex = stepCP.FacingEdgeIndex.Value;
            }
        }

        if (_currentStep.IsCompleted)
        {
            // Stop drawing regions when the step is completed
            _facingEdgeIndex = null;
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
        if (!_showRegions || !_facingEdgeIndex.HasValue || _facingEdgeIndex.Value < 0) { return; }

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
