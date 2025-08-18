using Microsoft.Xna.Framework;
using PhysicsIllustrated.Library.Managers;
using PhysicsIllustrated.Library.Physics.Mathematics;
using PhysicsIllustrated.Library.Physics.Shapes;
using System;

namespace PhysicsIllustrated.Library.Physics;

public class CollisionStepResult
{
    public string Step { get; set; }
    public bool? IsOutside { get; set; }
    public Vector2? MinCurrVertex { get; set; }
    public Vector2? MinNextVertex { get; set; }
    public float? DistanceCircleEdge { get; set; }
    public Contact Contact { get; set; }
    public bool? CollisionDetected { get; set; }

    public Action Draw;
}

public static class CollisionDetectionSteppable
{
    private static Color _shapeHili = Color.LightCyan;
    private static Color _line = Color.CornflowerBlue;
    private static Color _proj = Color.Pink;

    public static IEnumerable<CollisionStepResult> IsCollidingPolygonCircle(
        Body polygon,
        Body circle,
        List<Contact> contacts)
    {
        var polygonShape = (PolygonShape)polygon.Shape;
        var circleShape = (CircleShape)circle.Shape;

        var isOutside = false;

        var minCurrVertex = Vector2.Zero;
        var minNextVertex = Vector2.Zero;

        var distanceCircleEdge = float.MinValue;

        yield return new CollisionStepResult
        {
            Step = "Looping through each edge of the polygon",
        };

        // Loop through all the edges of the polygon/box
        for (var i = 0; i < polygonShape.WorldVertices.Length; i++)
        {
            var edge = () => polygonShape.WorldEdgeAt(i);

            var v0 = () => polygonShape.WorldVertices[i];

            var drawEdge = () =>
            {
                var p2 = v0() + edge();

                Graphics.Top.DrawLine(
                    v0(),
                    p2,
                    _shapeHili,
                    2
                );

                Graphics.DrawVertex(v0());
                Graphics.DrawVertex(p2);
            };

            yield return new CollisionStepResult
            {
                Step = $"Edge {i}",
                Draw = drawEdge
            };

            var vertexToCircle = () => circle.Position - v0();

            var drawVertexToCircle = () =>
            {
                Graphics.Mid.P0(v0()).P1(circle.Position).Color(_line).Thickness(2).DrawVector();
                Graphics.DrawVertex(circle.Position);
            };

            yield return new CollisionStepResult
            {
                Step = $"Take a line from the 1st vertex to the circle center",
                Draw = () =>
                {
                    drawEdge();
                    drawVertexToCircle();
                }
            };

            var normal = () => edge().RightUnitNormal();
            var projection = () => Vector2.Dot(vertexToCircle(), normal());

            var drawNormal = () =>
            {
                if (projection() > 0)
                {
                    // Draw the normal longer than the projection so it's clear
                    var nLength = projection() + 50;
                    if (nLength < 50) nLength = 50;

                    Graphics.Mid.P0(v0()).P1(v0() + normal() * nLength).Color(_line).Thickness(2).DrawVector();
                }
                else
                {
                    Graphics.Mid.P0(v0()).P1(v0() + normal() * 50).Color(_line).Thickness(2).DrawVector();
                }
            };

            yield return new CollisionStepResult
            {
                Step = $"Take the normal of the edge",
                Draw = () =>
                {
                    drawEdge();
                    drawVertexToCircle();
                    drawNormal();
                }
            };


            var drawProj = () =>
            {
                var p2 = v0() + normal() * projection();

                Graphics.Mid.P0(v0()).P1(p2).Color(_proj).Thickness(2).DrawVector();
            };

            yield return new CollisionStepResult
            {
                Step = $"Project the circle center onto the edge normal: {projection():F2}",

                Draw = () =>
                {
                    drawEdge();
                    drawVertexToCircle();
                    drawNormal();
                    drawProj();
                }
            };

            if (projection() > 0)
            {

            }
            else
            {

            }
        }

        Contact contact = null;

        if (isOutside)
        {
        }
        else
        {

        }

        // TEST
        yield return new CollisionStepResult
        {
            Step = "TEST method called",
            CollisionDetected = false
        };

        yield break;
    }
}
