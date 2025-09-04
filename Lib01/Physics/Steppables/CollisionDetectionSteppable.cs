using Microsoft.Xna.Framework;
using ShowPhysics.Library.Managers;
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
        var circleA = (CircleShape) a.Shape;
        var circleB = (CircleShape) b.Shape;

        var drawDistance = () => { Graphics.Distance(a, b, circleA.Radius + circleB.Radius); };

        yield return new Step
        {
            Name = "Measure the distance between the 2 circle centers",
            Draw = () => { drawDistance(); }
        };

        var ab = b.Position - a.Position;
        var radiusSum = circleA.Radius + circleB.Radius;

        var isColliding = ab.LengthSquared() <= radiusSum * radiusSum;

        if (!isColliding)
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

        var contact = new Contact();

        contact.A = a;
        contact.B = b;

        contact.Normal = Vector2.Normalize(ab);

        contact.Start = b.Position - contact.Normal * circleB.Radius;
        contact.End = a.Position + contact.Normal * circleA.Radius;

        contact.Depth = (contact.End - contact.Start).Length();

        contacts.Add(contact);

    }
}
