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

public class ShowCirclePolygonVelcro : ShowBase
{
    public ShowCirclePolygonVelcro(GraphicsDevice graphicsDevice) : base(graphicsDevice)
    {
        Name = "VELCRO / Circle-to-Polygon Collision Detection";

        //_box = new Body(new BoxShape(250, 250), Width() / 2, Height() / 2, 1.0f);
        _box = new Body(PolygonShape.Create(250, 8), Width() / 2, Height() / 2, 1.0f);
        _box.Rotation = 0;
        _movable = new Body(new CircleShape(80), Width() * 0.8f, 100, 1.0f);

        Bodies.Add(_box);
        Bodies.Add(_movable);

        _fileName = $"view_{GetType().Name}.txt";

        Menu += " | V = save view";
    }

    private Body _box;
    private Body _movable;
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

        ProcessStepDraws();

        // Handle show-side drawings
    }

    //==========================================================================

    protected override void InitializeSteps()
    {
        _steps = CollisionDetectionSteppableVelcro.IsCollidingPolygonCircle(_box, _movable, _contacts).GetEnumerator();
        base.InitializeSteps();
    }

    protected override void OnStepAdvanced()
    {
        if (_currentStep == null) { return; }

        Console(_currentStep.Text);

        if (_currentStep.IsCompleted)
        {
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
}
