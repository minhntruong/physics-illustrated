using System;
using Microsoft.Xna.Framework;
using PhysicsIllustrated.Library.Managers;
using PhysicsIllustrated.Library.Physics;
using PhysicsIllustrated.Library.Physics.Shapes;

namespace PhysicsIllustrated.Library.Illustrators
{
    public class Circles
    {
        private List<Body> _bodies = new List<Body>();
        private bool _showRadii = true;

        public void ShowRadii(bool value)
        {
            _showRadii = value;
        }

        public void Add(Body body)
        {
            _bodies.Add(body);
        }

        public void Draw()
        {
            for (var i = 0; i < _bodies.Count; i++)
            {
                for (var j = i + 1; j < _bodies.Count; j++)
                {
                    var bodyA = _bodies[i]; var shapeA = bodyA.Shape as CircleShape;
                    var bodyB = _bodies[j]; var shapeB = bodyB.Shape as CircleShape;

                    var aToB = bodyB.Position - bodyA.Position;
                    var aToBNorm = Vector2.Normalize(aToB);
                    
                    if (_showRadii)
                    {
                        // Radius A
                        Graphics.Mid.DrawLine(
                            bodyA.Position,
                            //bodyA.Position + -aToBNorm * shapeA.Radius,
                            bodyA.Position + new Vector2(1, 0) * shapeA.Radius,
                            Color.MediumSlateBlue);

                        Graphics.Text
                            .Anchor(TextAnchor.Center)
                            //.Position(bodyA.Position + -aToBNorm * shapeA.Radius * 0.5f)
                            .Position(bodyA.Position + new Vector2(1, 0) * shapeA.Radius * 0.5f)
                            .Scale(0.5f)
                            .Text(shapeA.Radius);

                        // Radius B
                        Graphics.Mid.DrawLine(
                            bodyB.Position,
                            //bodyB.Position + aToBNorm * (bodyB.Shape as CircleShape).Radius,
                            bodyB.Position + new Vector2(1, 0) * shapeB.Radius,
                            Color.MediumSlateBlue);

                        Graphics.Text
                            .Anchor(TextAnchor.Center)
                            //.Position(bodyB.Position + aToBNorm * shapeB.Radius * 0.5f)
                            .Position(bodyB.Position + new Vector2(1, 0) * shapeB.Radius * 0.5f)
                            .Scale(0.5f)
                            .Text(shapeB.Radius);
                    }

                    // Distance line
                    Graphics.Mid.DrawLine(
                        bodyA.Position, 
                        bodyB.Position, 
                        Color.DarkCyan);

                    var len = aToB.Length();

                    Graphics.Text
                        .Anchor(TextAnchor.Center)
                        .Position(bodyA.Position + aToBNorm * len * 0.5f)
                        .Scale(0.5f)
                        .Text(len.ToString("F1"));

                    Graphics.Mid.DrawFillRect(bodyA.Position.X, bodyA.Position.Y, 4, 4, Color.Cyan);
                    Graphics.Mid.DrawFillRect(bodyB.Position.X, bodyB.Position.Y, 4, 4, Color.Cyan);
                }
            }
        }
    }
}
