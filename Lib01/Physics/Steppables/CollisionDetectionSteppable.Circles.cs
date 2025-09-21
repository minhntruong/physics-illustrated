using System;
using Microsoft.Xna.Framework;
using ShowPhysics.Library.Managers;
using ShowPhysics.Library.Physics.Shapes;
using System.Collections.Generic;

namespace ShowPhysics.Library.Physics.Steppables;

public static partial class CollisionDetectionSteppable
{
    public static IEnumerable<Step> IsCollidingCircles(Body a, Body b, List<Contact> contacts)
    {
        var circleA = (CircleShape)a.Shape;
        var circleB = (CircleShape)b.Shape;

        var step = new Step();

        step.Text = "Measure the distance between the 2 circle centers";
        step.AddAnim((float animValue) =>
        {
            Coords.Line(a, b).DrawContactDistance(circleA.Radius + circleB.Radius, true, animValue);
        });
        yield return step;

        var ab = () => b.Position - a.Position;
        var radiusSum = circleA.Radius + circleB.Radius;

        var isColliding = () => ab().Length() <= radiusSum;

        if (!isColliding())
        {
            step.Text = "No collision, the distance is greater than the sum of the radii";
            yield return step;

            step.Text = "Check completed";
            step.IsCompleted = true;
            step.IsColliding = false;
            yield return step;

            yield break;
        }

        step.Text = "Collision detected, the distance is less than the sum of the radii";
        yield return step;

        step.Reset();
        step.Text = "Now we gather information about the collision that will be used to resolve it later";
        step.AddDraw(() => Coords.Line(a, b).DrawLine(Theme.BgAnnotations));
        yield return step;

        step.Text = "Along the distance line, from the center, extending the entire radius, is 1 contact point";
        step.AddAnim((float animValue) => Coords.BodyExtentToBody(b, a, circleB.Radius).DrawVector(Theme.ContactStart, animValue));
        yield return step;

        step.RemoveLastDraw();
        step.Text = "This is the first contact point";
        step.AddDraw(() => Coords.BodyExtentToBody(b, a, circleB.Radius).End.DrawContact(Theme.ContactStart));
        yield return step;

        step.Text = "From the other circle, extending the entire radius, is the 2nd contact point";
        step.AddAnim((float animValue) => Coords.BodyExtentToBody(a, b, circleA.Radius).DrawVector(Theme.ContactEnd, animValue));
        yield return step;

        step.RemoveLastDraw();
        step.Text = "This is the 2nd contact point";
        step.AddDraw(() => Coords.BodyExtentToBody(a, b, circleA.Radius).End.DrawContact(Theme.ContactEnd));
        yield return step;

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
            Text = "Check completed",
            IsCompleted = true
        };
    }
}
