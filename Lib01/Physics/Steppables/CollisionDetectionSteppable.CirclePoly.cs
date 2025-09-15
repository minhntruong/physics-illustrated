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

        var step = new StepCirclePoly
        {
            Text = "First we find the closest facing edge to the circle center"
        };

        yield return step;

        var selectedEdge = -1;

        // Loop through all the edges of the polygon/box
        for (var i = 0; i < polyShape.WorldVertices.Length; i++)
        {
            selectedEdge = i;

            step.Reset();
            step.Text = $"Evaluationg edge {i}";
            step.AddDraw(() => (polyShape, i).DrawEdge());
            yield return step;

            step.Text = "Take the perpendicular (normal) to this edge";
            //step.AddDrawAnimatedFloat(1, (float animValue) => Graphics.DrawEdgeNormal(polyShape, i, animValue));
            step.AddAnim(1, (float animValue) => 
            {
                Coords.EdgeNormal(polyShape, selectedEdge).DrawEdgeNormalRef(animValue);
            });
            yield return step;

            step.Text = "Take the vector from the vertex to the circle center";
            step.AddAnim(1, (float animValue) => Coords.VertexToBody(polyShape, i, circle).DrawVector(Theme.Normals, animValue));
            yield return step;

            step.Text = $"Project the circle center vector onto the normal";
            //step.AddDraw(() => Graphics.DrawProjectionOnNormalFromBody(polyShape, i, circle));
            //step.AddDraw(() => Coords.EdgeNormalToBody(polyShape, i, circle).DrawProjection());
            step.AddAnim(1, (float animValue) => Coords.EdgeNormalToBody(polyShape, i, circle).DrawProjection(animValue));
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

                step.Text = "A positive projection was found, the circle center is outside the polygon";
                yield return step;

                step.Reset();
                step.Text = "Now we determine which region the circle center is in (A, B or C)";
                step.AddDraw(() => (polyShape, i).DrawEdge());
                yield return step;

                step.Text = "These are the regions";
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
        step.Text = "Check to see if the circle center is in region A";
        step.AddDraw(() => (polyShape, selectedEdge).DrawEdge());
        yield return step;

        step.Text = "First take the vector v1 from the vertex to cirle center";
        step.AddAnim(1, (float animValue) => Coords.VertexToBody(polyShape, selectedEdge, circle).DrawVector(Theme.Normals, animValue));
        //step.AddDraw(() => Graphics.DrawVectorFromVertexToBody(polyShape, selectedEdge, circle));
        yield return step;

        step.Text = "Then take the vector v2 along the edge";
        //step.AddDraw(() => Graphics.DrawVectorAlongEdge(polyShape, selectedEdge));
        step.AddAnim(1, (float animValue) => Coords.Edge(polyShape, selectedEdge).DrawVector(Theme.EdgeSelected, animValue, 50));
        yield return step;

        step.Text = "Then project v1 onto v2";
        step.AddDraw(() => Graphics.DrawProjectionOnEdgeFromBody(polyShape, selectedEdge, circle));
        yield return step;

        var v1 = circle.Position - minCurrVertex;
        var v2 = minNextVertex - minCurrVertex;

        if (Vector2.Dot(v1, v2) < 0)
        {
            step.Text = "The projection is negative, so we are in region A";
            yield return step;

            step.Reset();
            step.Text = "Now measure the distance from the circle center to the vertex";
            step.AddDraw(() => Graphics.DrawVertex(polyShape, selectedEdge));
            step.AddDraw(() => Graphics.DrawLabeledDistance(polyShape, selectedEdge, circle, circleShape.Radius));
            yield return step;

            if (v1.LengthSquared() > circleShape.Radius * circleShape.Radius)
            {
                // Distance from vertex to circle center is greater than radius, no collision
                step.Text = $"Distance is greater than radius ({circleShape.Radius}), no collision";
                yield return step;

                step.Reset();
                step.Text = "Check completed";
                step.IsColliding = false;
                step.IsCompleted = true;
                yield return step;

                yield break;
            }

            step.Text = $"Distance is less than radius ({circleShape.Radius}), collision detected";
            yield return step;

            step.Reset();
            step.Text = "Now, gather collision info for the response step";
            step.FacingEdgeIndex = -1; // Signal stop showing regions
            yield return step;

            step.Text = "Take the line from vertex to circle center";
            step.AddDraw(() => Graphics.DrawLineFromVertexToBody(polyShape, selectedEdge, circle, Theme.Normals));
            step.AddDraw(() => Graphics.DrawVertex(polyShape.WorldVertices[0]));
            var anim1 = step.AddAnim(circleShape.Radius, 1.0f, (float animValue) =>
            {
                Graphics.DrawVectorFromBodyToVertex(circle, polyShape, selectedEdge, Color.Lime, animValue);
            });

            yield return step;

            step.RemoveDraw(anim1);
            step.Text = "This is 1 contact point";
            step.AddDraw(() => Graphics.DrawContactFromCircleByVertex(circle, polyShape, selectedEdge, Theme.ContactStart));
            yield return step;

            step.Reset();
            step.Text = "This is the other contact point";
            step.AddDraw(() => Graphics.DrawContactFromCircleByVertex(circle, polyShape, selectedEdge, Theme.ContactStart));
            step.AddDraw(() => Graphics.DrawContact(polyShape, selectedEdge, Theme.ContactEnd));
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

            step.Text = "Check completed";
            step.Contact = contact;
            step.IsColliding = true;
            step.IsCompleted = true;
            yield return step;

            yield break;
        }
        else
        {
            step.Text = "The projection is positive, so we are not in region A";
            yield return step;
        }

        //=== Check if we are in region B
        step.Reset();

        step.Text = "Check to see if the circle center is in region B";
        step.AddDraw(() => (polyShape, selectedEdge).DrawEdge());
        yield return step;

        step.Text = "First take the vector v1 from the vertex to cirle center";
        step.AddAnim(1, (float animValue) => Coords.VertexToBody(polyShape, polyShape.NextVertexIndex(selectedEdge), circle).DrawVector(Theme.Normals, animValue));
        //step.AddDraw(() => Graphics.DrawVectorFromVertexToBody(polyShape, polyShape.NextVertexIndex(selectedEdge), circle));
        yield return step;

        step.Text = "Then take the vector v2 backward along the edge";
        step.AddDraw(() => Graphics.DrawVectorAlongEdge(polyShape, selectedEdge, true));
        yield return step;

        step.Text = "Then project v1 onto v2";
        step.AddDraw(() => Graphics.DrawProjectionOnEdgeFromBody(polyShape, selectedEdge, circle, true));
        yield return step;

        v1 = circle.Position - minNextVertex;  // vector from next nearest vertex to circle center
        v2 = minCurrVertex - minNextVertex;    // the nearest edge

        if (Vector2.Dot(v1, v2) < 0)
        {
            step.Text = "The projection is negative, so we are in region B";
            yield return step;

            var nextVertexIndex = polyShape.NextVertexIndex(selectedEdge);

            step.Reset();
            step.Text = "Now measure the distance from the circle center to the vertex";
            step.AddDraw(() => Graphics.DrawVertex(polyShape, nextVertexIndex));
            step.AddDraw(() => Graphics.DrawLabeledDistance(polyShape, nextVertexIndex, circle, circleShape.Radius));

            yield return step;

            if (v1.LengthSquared() > circleShape.Radius * circleShape.Radius)
            {
                // Distance from vertex to circle center is greater than radius, no collision
                step.Text = $"Distance is greater than radius ({circleShape.Radius}), no collision";
                yield return step;

                step.Reset();
                step.Text = "Check completed";
                step.IsColliding = false;
                step.IsCompleted = true;
                yield return step;

                yield break;
            }

            step.Text = $"Distance is less than radius ({circleShape.Radius}), collision detected";
            yield return step;

            step.Reset();
            step.Text = "Now, gather collision info for the response step";
            step.FacingEdgeIndex = -1; // Signal stop showing regions
            yield return step;

            step.Text = "Take the line from vertex to circle center";
            step.AddDraw(() => Graphics.DrawLineFromVertexToBody(polyShape, nextVertexIndex, circle, Theme.Normals));
            step.AddDraw(() => Graphics.DrawVertex(polyShape.WorldVertices[nextVertexIndex]));
            step.AddAnim(circleShape.Radius, 1.0f, (float animValue) =>
            {
                Graphics.DrawVectorFromBodyToVertex(circle, polyShape, nextVertexIndex, Color.Lime, animValue);
            });

            yield return step;

            step.ClearDrawAnimations();
            step.Text = "This is 1 contact point";
            step.AddDraw(() => Graphics.DrawContactFromCircleByVertex(circle, polyShape, nextVertexIndex, Theme.ContactStart));
            yield return step;

            step.Reset();
            step.Text = "This is the other contact point";
            step.AddDraw(() => Graphics.DrawContactFromCircleByVertex(circle, polyShape, nextVertexIndex, Theme.ContactStart));
            step.AddDraw(() => Graphics.DrawContact(polyShape, nextVertexIndex, Theme.ContactEnd));
            yield return step;

            contact = new Contact();
            contacts.Add(contact);

            // Detected collision in region B
            contact.A = poly;
            contact.B = circle;
            contact.Depth = circleShape.Radius - v1.Length();
            contact.Normal = Vector2.Normalize(v1);
            contact.Start = circle.Position + (contact.Normal * -circleShape.Radius);
            contact.End = contact.Start + contact.Normal * contact.Depth;

            step.Text = "Check completed";
            step.Contact = contact;
            step.IsColliding = true;
            step.IsCompleted = true;
            yield return step;

            yield break;
        }

        // We are in region C

        step.Reset();
        step.Text = "The only option left is region C";
        step.AddDraw(() => (polyShape, selectedEdge).DrawEdge());
        yield return step;

        step.Text = "For region C, the distance from the circle center to the edge is the projection we found earlier";
        step.AddDraw(() => Coords.EdgeNormal(polyShape, selectedEdge).DrawEdgeNormalRef()); // Graphics.DrawEdgeNormal(polyShape, selectedEdge));
        step.AddAnim(1, (float animValue) => Coords.VertexToBody(polyShape, selectedEdge, circle).DrawVector(Theme.Normals, animValue));
        //step.AddDraw(() => Graphics.DrawVectorFromVertexToBody(polyShape, selectedEdge, circle));
        step.AddDraw(() => Graphics.DrawProjectionOnNormalFromBody(polyShape, selectedEdge, circle, circleShape.Radius));
        yield return step;

        if (distanceCircleEdge > circleShape.Radius)
        {
            // Distance from edge to circle center is greater than radius, no collision
            step.Text = $"Distance from edge to circle center is greater than radius {circleShape.Radius}, no collision";
            yield return step;

            step.Reset();
            step.Text = "Check completed";
            step.IsColliding = false;
            step.IsCompleted = false;
            yield return step;

            yield break;
        }

        step.Reset();
        step.Text = "Calculating contact info";
        yield return step;

        step.Text = "Consider the perpendicular of the edge from the circle center";
        step.AddDraw(() => Graphics.DrawEdgeNormalFromBody(polyShape, selectedEdge, circle));
        yield return step;

        step.Text = "Extend to the radius of the circle";
        var anim2 = step.AddAnim(circleShape.Radius, 1, (float animValue) => Graphics.DrawEdgeNormalVectorFromBody(circle, polyShape, selectedEdge, Color.Lime, animValue));
        yield return step;

        step.RemoveDraw(anim2);
        step.Text = "This is 1 contact point";
        step.AddDraw(() => Graphics.DrawContactFromCircleByEdgeNormal(circle, polyShape, selectedEdge, Theme.ContactStart));
        yield return step;

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