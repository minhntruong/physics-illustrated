using Microsoft.Xna.Framework;
using ShowPhysics.Library.Managers;
using ShowPhysics.Library.Managers.Animation;
using System;
using System.Collections.Generic;
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

        yield return new StepCirclePoly
        {
            Name = "First we find the closest facing edge to the circle center"
        };

        // Loop through all the edges of the polygon/box
        for (var i = 0; i < polyShape.WorldVertices.Length; i++)
        {
            yield return new StepCirclePoly
            {
                Name = $"Evaluationg edge {i}",
                SelectedEdge = i,
            };

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

        if (isOutside)
        {
            //=== Check if we are in region A
            var v1 = circle.Position - minCurrVertex;
            var v2 = minNextVertex - minCurrVertex;

            if (Vector2.Dot(v1, v2) < 0)
            {
                if (v1.LengthSquared() > circleShape.Radius * circleShape.Radius)
                {
                    // Distance from vertex to circle center is greater than radius, no collision
                    yield return new StepCirclePoly
                    {
                        IsColliding = false,
                        IsCompleted = true,
                    };
                }

                contact = new Contact();
                contacts.Add(contact);

                // Detected collision in region A
                contact.A = poly;
                contact.B = circle;
                contact.Depth = circleShape.Radius - v1.Length();
                contact.Normal = Vector2.Normalize(v1);
                contact.Start = circle.Position + (contact.Normal * -circleShape.Radius);
                contact.End = contact.Start + contact.Normal * contact.Depth;

                yield return new StepCirclePoly
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
                    yield return new StepCirclePoly
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

                yield return new StepCirclePoly
                {
                    IsColliding = true,
                    IsCompleted = true,
                };
            }

            // We are in region C

            if (distanceCircleEdge > circleShape.Radius)
            {
                // Distance from edge to circle center is greater than radius, no collision
                yield return new StepCirclePoly
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

            yield return new StepCirclePoly
            {
                IsColliding = true,
                IsCompleted = true,
            };
        }
        else
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

            yield return new StepCirclePoly
            {
                IsColliding = true,
                IsCompleted = true,
            };
        }
    }
}