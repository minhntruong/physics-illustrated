using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ShowPhysics.Library.Managers;
using ShowPhysics.Library.Managers.Animation;
using ShowPhysics.Library.Physics.Math;
using ShowPhysics.Library.Physics.Shapes;

namespace ShowPhysics.Library.Physics.Steppables;

public static partial class CollisionDetectionSteppable
{
    public static IEnumerable<Step> IsCollidingPolygonCircle(Body poly, Body circle, List<Contact> contacts)
    {
        var polyShape = (PolygonShape)poly.Shape;
        var circleShape = (CircleShape)circle.Shape;

        var isOutside = false;

        var minCurrVertex = Vector2.Zero;
        var minNextVertex = Vector2.Zero;

        var distanceCircleEdge = float.MinValue;

        yield return new Step
        {
            Name = "First we find the closest facing edge to the circle center"
        };

        var selectedEdge = -1;

        var step = new StepCirclePoly();

        // Loop through all the edges of the polygon/box
        for (var i = 0; i < polyShape.WorldVertices.Length; i++)
        {
            selectedEdge = i;

            step.Reset();
            step.Name = $"Evaluationg edge {i}";
            step.AddDraw(() => Graphics.DrawSelectedEdge(polyShape, i));
            yield return step;

            step.Name = "Take the perpendicular (normal) to this edge";
            step.AddDraw(() => Graphics.DrawNormal(polyShape, i));
            yield return step;

            step.Name = "Take the vector from the vertex to the circle center";
            step.AddDraw(() => Graphics.DrawVectorToBody(polyShape, i, circle));
            yield return step;

            step.Name = $"Project the circle center vector onto the normal";
            step.AddDraw(() => Graphics.DrawProjectionOnNormalFromBody(polyShape, i, circle));
            yield return step;

            var edge = polyShape.WorldEdgeAt(i);
            var normal = edge.RightUnitNormal();

            // Compare the circle center with the poly vertex
            var circleCenter = circle.Position - polyShape.WorldVertices[i];

            // Project the circle center onto the edge normal
            var projection = Vector2.Dot(circleCenter, normal);

            if (projection > 0)
            {
                // If we found a dot product projection that is in the positive side of the normal
                // it means that the circle center is outside of the polygon edge

                distanceCircleEdge = projection;

                minCurrVertex = polyShape.WorldVertices[i];

                minNextVertex = polyShape.WorldVertexAfter(i); // s[nextIndex];

                isOutside = true;

                step.Name = "A positive projection was found, the circle center is outside the polygon";
                yield return step;

                step.Reset();
                step.Name = "Now we determine which region the circle center is in (A, B or C)";
                step.AddDraw(() => Graphics.DrawSelectedEdge(polyShape, i));
                yield return step;

                step.Name = "These are the regions";
                step.FacingEdgeIndex = i;
                yield return step;

                break;
            }
            else
            {
                // Not yet found a positive projection, so track the biggest negative projection (closest to zero)
                // So in case a positive projection is not found, we can use the biggest value
                if (projection > distanceCircleEdge)
                {
                    distanceCircleEdge = projection;
                    minCurrVertex = polyShape.WorldVertices[i];
                    minNextVertex = polyShape.WorldVertexAfter(i); // s[nextIndex];
                }
            }
        }

        Contact contact = null;

        if (isOutside == false)
        {
            contact = new Contact();
            contacts.Add(contact);

            // Circle center is inside the polygon
            contact.A = poly;
            contact.B = circle;
            contact.Depth = circleShape.Radius - distanceCircleEdge;
            contact.Normal = (minNextVertex - minCurrVertex).RightUnitNormal();
            contact.Start = circle.Position - (contact.Normal * circleShape.Radius);
            contact.End = contact.Start + contact.Normal * contact.Depth;

            yield return new Step
            {
                IsColliding = true,
                IsCompleted = true,
            };
        }

        //=== Check if we are in region A

        step.Reset();
        step.Name = "Check to see if the circle center is in region A";
        step.AddDraw(() => Graphics.DrawSelectedEdge(polyShape, selectedEdge));
        yield return step;

        step.Name = "First take the vector v1 from the vertex to cirle center";
        step.AddDraw(() => Graphics.DrawVectorToBody(polyShape, selectedEdge, circle));
        yield return step;

        step.Name = "Then take the vector v2 along the edge";
        step.AddDraw(() => Graphics.DrawVectorAlongEdge(polyShape, selectedEdge));
        yield return step;

        step.Name = "Then project v1 onto v2";
        step.AddDraw(() => Graphics.DrawProjectionOnEdgeFromBody(polyShape, selectedEdge, circle));
        yield return step;

        var v1 = circle.Position - minCurrVertex;
        var v2 = minNextVertex - minCurrVertex;

        if (Vector2.Dot(v1, v2) < 0)
        {
            step.Name = "The projection is negative, so we are in region A";
            yield return step;

            step.Reset();
            step.Name = "Now measure the distance from the circle center to the vertex";
            step.AddDraw(() => Graphics.DrawLabeledDistance(polyShape, selectedEdge, circle, circleShape.Radius));
            yield return step;

            if (v1.LengthSquared() > circleShape.Radius * circleShape.Radius)
            {
                // Distance from vertex to circle center is greater than radius, no collision
                step.Name = $"Distance is greater than radius ({circleShape.Radius}), no collision";
                yield return step;

                step.Reset();
                step.Name = "Check completed";
                step.IsColliding = false;
                step.IsCompleted = true;
                yield return step;

                yield break;
            }

            step.Name = $"Distance is less than radius ({circleShape.Radius}), collision detected";
            yield return step;

            step.Reset();
            step.Name = "Now, gather collision info for the response step";
            step.FacingEdgeIndex = -1; // Signal stop showing regions
            yield return step;

            step.Name = "Take the line from vertex to circle center";
            step.AddDraw(() => Graphics.DrawLineToBody(polyShape, selectedEdge, circle, Theme.Normals));
            step.AddDraw(() => Graphics.DrawVertex(polyShape.WorldVertices[0]));
            yield return step;

            contact = new Contact();
            contacts.Add(contact);

            // Detected collision in region A
            contact.A = poly;
            contact.B = circle;
            contact.Depth = circleShape.Radius - v1.Length();
            contact.Normal = Vector2.Normalize(v1);
            contact.Start = circle.Position + (contact.Normal * -circleShape.Radius);
            contact.End = contact.Start + contact.Normal * contact.Depth;

            yield return new Step
            {
                IsColliding = true,
                IsCompleted = true,
            };
        }

        // Check if we are in region B
        v1 = circle.Position - minNextVertex;  // vector from next nearest vertex to circle center
        v2 = minCurrVertex - minNextVertex;    // the nearest edge

        if (Vector2.Dot(v1, v2) < 0)
        {
            if (v1.LengthSquared() > circleShape.Radius * circleShape.Radius)
            {
                // Distance from vertex to circle center is greater than radius, no collision
                yield return new Step
                {
                    IsColliding = false,
                    IsCompleted = true,
                };
            }

            contact = new Contact();
            contacts.Add(contact);

            // Detected collision in region B
            contact.A = poly;
            contact.B = circle;
            contact.Depth = circleShape.Radius - v1.Length();
            contact.Normal = Vector2.Normalize(v1);
            contact.Start = circle.Position + (contact.Normal * -circleShape.Radius);
            contact.End = contact.Start + contact.Normal * contact.Depth;

            yield return new Step
            {
                IsColliding = true,
                IsCompleted = true,
            };
        }

        // We are in region C

        if (distanceCircleEdge > circleShape.Radius)
        {
            // Distance from edge to circle center is greater than radius, no collision
            yield return new Step
            {
                IsColliding = false,
                IsCompleted = true,
            };
        }

        contact = new Contact();
        contacts.Add(contact);

        contact.A = poly;
        contact.B = circle;
        contact.Depth = circleShape.Radius - distanceCircleEdge;
        contact.Normal = (minNextVertex - minCurrVertex).RightUnitNormal();
        contact.Start = circle.Position - (contact.Normal * circleShape.Radius);
        contact.End = contact.Start + contact.Normal * contact.Depth;

        yield return new Step
        {
            IsColliding = true,
            IsCompleted = true,
        };
    }
}