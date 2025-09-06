using Microsoft.Xna.Framework;
using ShowPhysics.Library.Managers;
using ShowPhysics.Library.Managers.Animation;
using ShowPhysics.Library.Physics.Shapes;
using System;
using System.Collections.Generic;

namespace ShowPhysics.Library.Physics.Steppables;

public class Step
{
    public string Name { get; set; }

    public bool? IsColliding { get; set; }
    
    public bool IsCompleted { get; set; }

    public Action Draw { get; set;  }
}

public static partial class CollisionDetectionSteppable
{
    public static IEnumerable<Step> IsCollidingCircles(Body a, Body b, List<Contact> contacts)
    {
        var circleA = (CircleShape)a.Shape;
        var circleB = (CircleShape)b.Shape;

        var drawDistance = (bool label = true) => { Graphics.DrawLabeledDistance(a, b, circleA.Radius + circleB.Radius, label); };

        yield return new Step
        {
            Name = "Measure the distance between the 2 circle centers",
            Draw = () => { drawDistance(); }
        };

        var ab = () => b.Position - a.Position;
        var radiusSum = circleA.Radius + circleB.Radius;

        var isColliding = () => ab().LengthSquared() <= radiusSum * radiusSum;

        if (!isColliding())
        {
            yield return new Step
            {
                Name = "No collision, the distance is greater than the sum of the radii",
                Draw = () => { drawDistance(); },
                IsColliding = false
            };

            yield return new Step
            {
                Name = "Check completed",
                IsCompleted = true,
            };

            yield break;
        }

        yield return new Step
        {
            Name = "Collision detected, the distance is less than the sum of the radii",
            Draw = () => { drawDistance(); },
            IsColliding = true
        };

        yield return new Step
        {
            Name = "Now we gather information about the collision that will be used to resolve it later",
            Draw = () => { drawDistance(false); }
        };

        var animB = Animations.AddFloat(0, circleB.Radius, 1);

        var contactNormal = () => Vector2.Normalize(ab());
        var contactPtFromB = (float current) => b.Position - contactNormal() * current;

        yield return new Step
        {
            Name = "Along the distance line, from the center, extending the entire radius, is 1 contact point",
            Draw = () =>
            {
                drawDistance(false);

                var point = () => contactPtFromB(animB.Current);

                Graphics.Mid.Vector().Start(b.Position).End(point()).Color(Color.Lime).ThicknessAbs(4).Stroke();
            }
        };

        var drawContactStart = () =>
        {
            var point = () => contactPtFromB(animB.Current);
            Graphics.DrawVertex(point(), Color.Lime, true);
        };

        yield return new Step
        {
            Name = "Mark this contact point",
            Draw = () =>
            {
                drawDistance(false);
                drawContactStart();
            }
        };

        var animA = Animations.AddFloat(0, circleA.Radius, 1);

        var contactPtFromA = (float current) => a.Position + contactNormal() * current;

        yield return new Step
        {
            Name = "From the other circle, extending the entire radius, is the 2nd contact point",
            Draw = () =>
            {
                drawDistance(false);
                drawContactStart();

                var point = () => contactPtFromA(animA.Current);

                Graphics.Mid.Vector().Start(a.Position).End(point()).Color(Color.Orange).ThicknessAbs(4).Stroke();
            }
        };

        var drawContactEnd = () =>
        {
            var point = () => contactPtFromA(animA.Current);
            Graphics.DrawVertex(point(), Color.Orange, true);
        };

        yield return new Step
        {
            Name = "Mark this contact point",
            Draw = () =>
            {
                drawDistance(false);

                drawContactStart();
                drawContactEnd();
            }
        };

        yield return new Step
        {
            Name = "Resulting contact information",
            Draw = () =>
            {
                drawContactStart();
                drawContactEnd();
            }
        };

        var contact = new Contact();

        contact.A = a;
        contact.B = b;

        contact.Normal = Vector2.Normalize(ab());

        contact.Start = b.Position - contact.Normal * circleB.Radius;
        contact.End = a.Position + contact.Normal * circleA.Radius;

        contact.Depth = (contact.End - contact.Start).Length();

        contacts.Add(contact);

        yield return new Step
        {
            Name = "Check completed",
            IsCompleted = true
        };
    }
}
