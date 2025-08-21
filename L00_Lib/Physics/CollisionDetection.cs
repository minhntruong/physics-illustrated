using PhysicsIllustrated.Library.Managers;
using PhysicsIllustrated.Library.Physics.Shapes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using PhysicsIllustrated.Library.Physics.Mathematics;

namespace PhysicsIllustrated.Library.Physics;

public static class CollisionDetection
{
    public static bool IsColliding(Body a, Body b, List<Contact> contacts)
    {
        var aIsCircle = a.Shape.Type == ShapeType.Circle;
        var bIsCircle = b.Shape.Type == ShapeType.Circle;

        var aIsPolygon = a.Shape.Type == ShapeType.Polygon || a.Shape.Type == ShapeType.Box;
        var bIsPolygon = b.Shape.Type == ShapeType.Polygon || b.Shape.Type == ShapeType.Box;

        if (aIsCircle && bIsCircle)
        {
            return IsCollidingCircles(a, b, contacts);
        }

        if (aIsPolygon && bIsPolygon)
        {
            return IsCollidingPolygons(a, b, contacts);
        }

        if (aIsPolygon && bIsCircle)
        {
            return IsCollidingPolygonCircle(a, b, contacts);
        }

        if (aIsCircle && bIsPolygon)
        {
            return IsCollidingPolygonCircle(b, a, contacts);
        }

        return false;
    }

    public static bool IsCollidingPolygons(Body a, Body b, List<Contact> contacts)
    {
        var aPolygonShape = (PolygonShape)a.Shape;
        var bPolygonShape = (PolygonShape)b.Shape;

        int aIndexReferenceEdge = 0, bIndexReferenceEdge = 0;
        Vector2 aSupportPoint = Vector2.Zero, bSupportPoint = Vector2.Zero;

        float abSeparation = aPolygonShape.FindMinSeparation(bPolygonShape, ref aIndexReferenceEdge, ref aSupportPoint);
        if (abSeparation >= 0)
        {
            return false;
        }

        float baSeparation = bPolygonShape.FindMinSeparation(aPolygonShape, ref bIndexReferenceEdge, ref bSupportPoint);
        if (baSeparation >= 0)
        {
            return false;
        }

        PolygonShape referenceShape;
        PolygonShape incidentShape;

        int indexReferenceEdge;

        if (abSeparation > baSeparation)
        {
            referenceShape = aPolygonShape;
            incidentShape = bPolygonShape;
            indexReferenceEdge = aIndexReferenceEdge;
        }
        else
        {
            referenceShape = bPolygonShape;
            incidentShape = aPolygonShape;
            indexReferenceEdge = bIndexReferenceEdge;
        }

        // Find the reference edge based on the index that returned from the function
        Vector2 referenceEdge = referenceShape.WorldEdgeAt(indexReferenceEdge);

        ///////////////////////////////////// 
        // Clipping 
        /////////////////////////////////////
        var refEdgeNorm = referenceEdge.RightUnitNormal();
        int incidentIndex = incidentShape.FindIncidentEdge(refEdgeNorm);

        //int incidentNextIndex = incidentIndex + 1 >= incidentShape.WorldVertices.Length
        //    ? 0 // Wrap around to the first vertex if we are at the last one
        //    : incidentIndex + 1;


        Vector2 v0 = incidentShape.WorldVertices[incidentIndex];
        Vector2 v1 = incidentShape.WorldVertexAfter(incidentIndex); // WorldVertices[incidentNextIndex];

        var contactPoints = new List<Vector2>{v0, v1};
        var clippedPoints = new List<Vector2>(contactPoints);

        for (int i = 0; i < referenceShape.WorldVertices.Length; i++)
        {
            if (i == indexReferenceEdge) { continue; }

            Vector2 c0 = referenceShape.WorldVertices[i];

            //var nextIndex = i + 1 < referenceShape.WorldVertices.Length ? i + 1 : 0;
            Vector2 c1 = referenceShape.WorldVertexAfter(i); // s[nextIndex];

            int numClipped = referenceShape.ClipSegmentToLine(contactPoints, clippedPoints, c0, c1);
            if (numClipped < 2)
            {
                break;
            }

            //contactPoints = clippedPoints; // make the next contact points the ones that were just clipped
            contactPoints.Clear(); // clear the list for the next iteration
            contactPoints.AddRange(clippedPoints); // copy the clipped points to the contact points
        }

        var vref = referenceShape.WorldVertices[indexReferenceEdge];

        // Loop all clipped points, but only consider those where separation is negative (objects are penetrating each other)
        foreach (var vclip in clippedPoints)
        {
            float separation = Vector2.Dot(vclip - vref, referenceEdge.RightUnitNormal());

            if (separation <= 0)
            {
                Contact contact = new Contact();
                contact.A = a;
                contact.B = b;
                contact.Normal = referenceEdge.RightUnitNormal();
                contact.Start = vclip;
                contact.End = vclip + contact.Normal * -separation;

                if (baSeparation >= abSeparation)
                {
                    // the start-end points are always from "a" to "b"
                    var temp = contact.Start;
                    contact.Start = contact.End;
                    contact.End = temp; // swap start and end points

                    // the collision normal is always from "a" to "b"
                    contact.Normal *= -1.0f;
                }

                contacts.Add(contact);
            }
        }

        return true;
    }

    public static bool IsCollidingCircles(Body a, Body b, List<Contact> contacts)
    {
        var aCircle = (CircleShape) a.Shape;
        var bCircle = (CircleShape) b.Shape;

        var ab = b.Position - a.Position;
        var radiusSum = aCircle.Radius + bCircle.Radius;

        var isColliding = ab.LengthSquared() <= radiusSum * radiusSum;

        if (!isColliding)
        {
            return false;
        }

        var contact = new Contact();

        contact.A = a;
        contact.B = b;

        contact.Normal = Vector2.Normalize(ab);

        contact.Start = b.Position - contact.Normal * bCircle.Radius;
        contact.End = a.Position + contact.Normal * aCircle.Radius;

        contact.Depth = (contact.End - contact.Start).Length();

        contacts.Add(contact);

        return true;
    }

    public static bool IsCollidingPolygonCircle(Body polygon, Body circle, List<Contact> contacts)
    {
        var polygonShape = (PolygonShape) polygon.Shape;
        var circleShape = (CircleShape) circle.Shape;

        var isOutside = false;

        var minCurrVertex = Vector2.Zero;
        var minNextVertex = Vector2.Zero;

        var distanceCircleEdge = float.MinValue;

        // Loop through all the edges of the polygon/box
        for (var i = 0; i < polygonShape.WorldVertices.Length; i++)
        {
            //var nextIndex = i + 1;
            //if (nextIndex >= polygonShape.WorldVertices.Length)
            //{
            //    nextIndex = 0; // Wrap around to the first vertex
            //}

            var edge = polygonShape.WorldEdgeAt(i);
            var normal = edge.RightUnitNormal();

            // Compare the circle center with the poly vertex
            var circleCenter = circle.Position - polygonShape.WorldVertices[i];

            // Project the circle center onto the edge normal
            var projection = Vector2.Dot(circleCenter, normal);

            //=== DEBUG: Draw the edge's normal

            var missedColor = Color.DodgerBlue;
            var defaultColor = Color.White;

#if true
            var drawDebugs = false; // polygon.Name == "new-poly" && circle.Name == "center-circle";

            if (drawDebugs)
            {
                #region DRAW DEBUGS

                // Draw the normal line
                Graphics.Top
                    .P0(polygonShape.WorldVertices[i] + edge * 0.0f)
                    .P1(polygonShape.WorldVertices[i] + edge * 0.0f + normal * 45)
                    .Color(projection > 0 ? defaultColor : missedColor)
                    .Thickness(2)
                    .DrawLine();

                //Graphics.Top.DrawLine(
                //    polygonShape.WorldVertices[i] + edge * 0.0f,
                //    polygonShape.WorldVertices[i] + edge * 0.0f + normal * 45,
                //    projection > 0 ? defaultColor : missedColor,
                //    2);

                // Draw the line from vertex to the circle center
                Graphics.Top
                    .P0(polygonShape.WorldVertices[i])
                    .P1(polygonShape.WorldVertices[i] + circleCenter)
                    .Color(projection > 0 ? defaultColor : missedColor)
                    .Thickness(2)
                    .DrawLine();

                //Graphics.Top.DrawLine(
                //    polygonShape.WorldVertices[i],
                //    polygonShape.WorldVertices[i] + circleCenter,
                //    projection > 0 ? defaultColor : missedColor,
                //    2);

                //=== Draw refs
                var refPoint = new Vector2(400, 400);

                if (projection > 0)
                {
                    // Circle center is found outside of one of polygon's edges

                    //=== DEBUG Positive Projection
                    Graphics.Top
                        .P0(refPoint)
                        .P1(refPoint + normal * projection)
                        .Color(Color.Red)
                        .Thickness(6)
                        .DrawLine();

                    //Graphics.Top.DrawLine(
                    //    refPoint,
                    //    refPoint + normal * projection,
                    //    Color.Red,
                    //    6);

                    // Positive Normal
                    Graphics.Top
                        .P0(refPoint)
                        .P1(refPoint + circleCenter)
                        .Color(defaultColor)
                        .Thickness(2)
                        .DrawLine();

                    //Graphics.Top.DrawLine(
                    //    refPoint,
                    //    refPoint + circleCenter,
                    //    defaultColor,
                    //    2);
                }
                else
                {
                    // Negative Projection
                    Graphics.Top
                        .P0(refPoint)
                        .P1(refPoint + normal * projection)
                        .Color(Color.DarkRed)
                        .Thickness(4)
                        .DrawLine();

                    //Graphics.Top.DrawLine(
                    //    refPoint,
                    //    refPoint + normal * projection,
                    //    Color.DarkRed,
                    //    4);

                    // Negative Normal
                    Graphics.Top
                        .P0(refPoint)
                        .P1(refPoint + circleCenter)
                        .Color(missedColor)
                        .Thickness(2)
                        .DrawLine();

                    //Graphics.Top.DrawLine(
                    //    refPoint,
                    //    refPoint + circleCenter,
                    //    missedColor,
                    //    2);
                }

                // Draw normal line
                Graphics.Top
                    .P0(refPoint)
                    .P1(refPoint + normal * 150)
                    .Color(projection > 0 ? defaultColor : missedColor)
                    .Thickness(2)
                    .DrawLine();

                //Graphics.Top.DrawLine(
                //    refPoint,
                //    refPoint + normal * 150,
                //    projection > 0 ? defaultColor : missedColor,
                //    2);

                //=== End draw refs
                #endregion
            }
#endif

            if (projection > 0)
            {
                // If we found a dot product projection that is in the positive side of the normal
                // it means that the circle center is outside of the polygon edge

                distanceCircleEdge = projection;

                minCurrVertex = polygonShape.WorldVertices[i];

                minNextVertex = polygonShape.WorldVertexAfter(i); // s[nextIndex];

#if false
                // Debug draws
                Drawer.DrawFillRect(minCurrVertex.X, minCurrVertex.Y, 8, 8, Color.Orange);
                Drawer.DrawFillRect(minNextVertex.X, minNextVertex.Y, 8, 8, Color.Orange);
#endif
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
                    minCurrVertex = polygonShape.WorldVertices[i];
                    minNextVertex = polygonShape.WorldVertexAfter(i); // s[nextIndex];
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
                    return false;
                }

                contact = new Contact();
                contacts.Add(contact);

                // Detected collision in region A
                contact.A = polygon;
                contact.B = circle;
                contact.Depth = circleShape.Radius - v1.Length();
                contact.Normal = Vector2.Normalize(v1);
                contact.Start = circle.Position + (contact.Normal * -circleShape.Radius);
                contact.End = contact.Start + contact.Normal * contact.Depth;

                return true;
            }

            // Check if we are in region B
            v1 = circle.Position - minNextVertex;  // vector from next nearest vertex to circle center
            v2 = minCurrVertex - minNextVertex;    // the nearest edge

            if (Vector2.Dot(v1, v2) < 0)
            {
                if (v1.LengthSquared() > circleShape.Radius * circleShape.Radius)
                {
                    // Distance from vertex to circle center is greater than radius, no collision
                    return false;
                }

                contact = new Contact();
                contacts.Add(contact);

                // Detected collision in region B
                contact.A = polygon;
                contact.B = circle;
                contact.Depth = circleShape.Radius - v1.Length();
                contact.Normal = Vector2.Normalize(v1);
                contact.Start = circle.Position + (contact.Normal * -circleShape.Radius);
                contact.End = contact.Start + contact.Normal * contact.Depth;

                return true;
            }

            // We are in region C

            if (distanceCircleEdge > circleShape.Radius)
            {
                // Distance from edge to circle center is greater than radius, no collision
                return false;
            }

            contact = new Contact();
            contacts.Add(contact);

            contact.A = polygon;
            contact.B = circle;
            contact.Depth = circleShape.Radius - distanceCircleEdge;
            contact.Normal = (minNextVertex - minCurrVertex).RightUnitNormal();
            contact.Start = circle.Position - (contact.Normal * circleShape.Radius);
            contact.End = contact.Start + contact.Normal * contact.Depth;

            return true;
        }
        else
        {
            contact = new Contact();
            contacts.Add(contact);

            // Circle center is inside the polygon
            contact.A = polygon;
            contact.B = circle;
            contact.Depth = circleShape.Radius - distanceCircleEdge;
            contact.Normal = (minNextVertex - minCurrVertex).RightUnitNormal();
            contact.Start = circle.Position - (contact.Normal * circleShape.Radius);
            contact.End = contact.Start + contact.Normal * contact.Depth;

            return true;
        }
    }
}
