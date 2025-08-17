using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhysicsIllustrated.Library.Managers;
using PhysicsIllustrated.Library.Physics.Shapes;

namespace PhysicsIllustrated.Library.Illustrators
{
    public class CirclesIllustrator : IllustratorBase
    {
        public CirclesIllustrator(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {
        }

        private bool _showRadii = true;

        public void ShowRadii(bool value)
        {
            _showRadii = value;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw()
        {
            for (var i = 0; i < Bodies.Count; i++)
            {
                for (var j = i + 1; j < Bodies.Count; j++)
                {
                    var bodyA = Bodies[i]; var shapeA = bodyA.Shape as CircleShape;
                    var bodyB = Bodies[j]; var shapeB = bodyB.Shape as CircleShape;

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

            base.Draw();
        }
    }
}
