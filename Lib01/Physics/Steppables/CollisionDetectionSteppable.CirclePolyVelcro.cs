using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ShowPhysics.Library.Managers;
using ShowPhysics.Library.Physics.Math;
using ShowPhysics.Library.Physics.Shapes;

namespace ShowPhysics.Library.Physics.Steppables;

public class CollisionDetectionSteppableVelcro
{
    public static IEnumerable<Step> IsCollidingPolygonCircle(Body poly, Body circle, List<Contact> contacts)
    {
        var polyShape = (PolygonShape)poly.Shape;
        var circleShape = (CircleShape)circle.Shape;

        var maxSeparation = float.MinValue;
        var indexReferenceEdge = -1;

        var step = new Step();
        step.Text = "VELCRO Collision Detection between Polygon and Circle";
        yield return step;

        for (var i = 0; i < polyShape.WorldVertices.Length; i++)
        {
            step.Reset();

            if (indexReferenceEdge != -1)
            {
                step.AddDraw(() => Coords.Vertex(polyShape, indexReferenceEdge).DrawVertex(Color.Pink, true));
            }

            step.Text = $"Evaluating edge {i}";
            step.AddDraw(() => (polyShape, i).DrawEdge());
            yield return step;

            step.Text = "Vertex/edge normal";
            step.AddDraw(() => Coords.EdgeNormal(polyShape, i).DrawRefLineWithNormal());
            yield return step;

            step.Text = "Vertex to circle center";
            step.AddDraw(() => Coords.VertexToBody(polyShape, i, circle).DrawVector(Theme.Normals));
            yield return step;

            var proj = 0f;
            step.Text = "Project onto normal";
            step.AddDraw(() => proj = Coords.EdgeNormalToBody(polyShape, i, circle).DrawProjection());
            yield return step;

            if (proj > maxSeparation)
            {
                if (maxSeparation == float.MinValue)
                {
                    step.Text = $"First value of {proj}";
                }
                else
                {
                    step.Text = $"New value of {proj} is greater than {maxSeparation}";
                }

                maxSeparation = proj;
                indexReferenceEdge = i;
            }
            else
            {
                step.Text = $"Current value {proj} is not greather than max of {maxSeparation}";
            }

            yield return step;
        }

        step.Reset();

        if (maxSeparation < 0)
        {
            // Inside
            step.Text = $"Circle is inside polygon (max separation {maxSeparation})";
            yield return step;

            yield break;
        }


    }
}
