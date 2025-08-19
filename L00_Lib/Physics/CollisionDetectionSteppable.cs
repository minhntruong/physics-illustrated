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
       
    public Func<Vector2> MinCurrVertex { get; set; }
    
    public Func<Vector2> MinNextVertex { get; set; }

    public float? DistanceCircleEdge { get; set; }
    public Contact Contact { get; set; }
    public bool? CollisionDetected { get; set; }

    public Action Draw;
}

public static class CollisionDetectionSteppable
{
    private static Color _highlite = Color.HotPink;
    private static Color _line = Color.CornflowerBlue;
    private static Color _proj = Color.Pink;

    public static IEnumerable<CollisionStepResult> IsCollidingPolygonCircle(
        Body polygon,
        Body circle,
        List<Contact> contacts)
    {
        var polygonShape = (PolygonShape)polygon.Shape;
        var circleShape = (CircleShape)circle.Shape;

        var circleCenterIsOutside = false;

        Func<Vector2> minCurrVertex = null;
        Func<Vector2> minNextVertex = null;

        var distanceCircleEdge = float.MinValue;

        yield return new CollisionStepResult
        {
            Step = "Looping through each edge of the polygon",
        };

        // Loop through all the edges of the polygon/box
        for (var i = 0; i < polygonShape.WorldVertices.Length; i++)
        {
            var iSaved = i;
            var edge = () => polygonShape.WorldEdgeAt(iSaved);

            var v0 = () => polygonShape.WorldVertices[iSaved];

            //=== STEP HIGHLIGHT EDGE ==========================================

            var drawEdge = () =>
            {
                var p2 = v0() + edge();

                Graphics.Top.DrawLine(
                    v0(),
                    p2,
                    _highlite,
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

            //=== STEP NORMAL ==================================================

            var normal = () => edge().RightUnitNormal();
            var drawNormal = () =>
            {
                Graphics.Mid.Color(_line).Thickness(2).Default();
                Graphics.Mid.P0(v0()).P1(v0() + normal() * 2000).DrawLine();
                Graphics.Mid.P0(v0()).P1(v0() + normal() * 50).DrawVector();
            };
            
            yield return new CollisionStepResult
            {
                Step = $"Take the normal of the edge",
                Draw = () =>
                {
                    drawEdge();
                    drawNormal();
                }
            };

            //=== STEP VERTEX TO CIRCLE ========================================

            var vertexToCircle = () => circle.Position - v0();

            var drawVertexToCircleCenter = () =>
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
                    drawNormal();
                    drawVertexToCircleCenter();
                }
            };

            //=== STEP PROJECTION ==============================================

            var projection = () => Vector2.Dot(vertexToCircle(), normal());

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
                    drawVertexToCircleCenter();
                    drawNormal();
                    drawProj();
                }
            };

            //=== DECISION # 1A ================================================

            if (projection() > 0)
            {
                distanceCircleEdge = projection();

                var index = i;
                minCurrVertex = () => polygonShape.WorldVertices[index];
                minNextVertex = () => polygonShape.WorldVertexAfter(index);

                circleCenterIsOutside = true;

                yield return new CollisionStepResult
                {
                    Step = $"Positive projection found: {projection():F2}, meaning circle center is outside of polygon",
                    MinCurrVertex = minCurrVertex,
                    MinNextVertex = minNextVertex,
                    Draw = () =>
                    {
                        drawEdge();
                        drawVertexToCircleCenter();
                        drawNormal();
                        drawProj();
                    }
                };

                break;
            }
            //=== DECISION # 1B ================================================
            else
            {
                if (projection() > distanceCircleEdge)
                {
                    var temp = distanceCircleEdge;

                    distanceCircleEdge = projection();

                    var index = i;
                    minCurrVertex = () => polygonShape.WorldVertices[index];
                    minNextVertex = () => polygonShape.WorldVertexAfter(index);

                    yield return new CollisionStepResult
                    {
                        Step = $"Positive projection not found, but value {distanceCircleEdge} is larger than prev {temp}",
                        MinCurrVertex = minCurrVertex,
                        MinNextVertex = minNextVertex,
                        Draw = () =>
                        {
                            drawEdge();
                            drawVertexToCircleCenter();
                            drawNormal();
                            drawProj();
                        }
                    };
                }
            }
        } // Each edge of the polygon

        Contact contact = null;

        if (circleCenterIsOutside)
        {
            //=== DECISION # 2A ================================================
            yield return new CollisionStepResult
            {
                Step = $"IsOutside = {circleCenterIsOutside}, checking for region A"
            };

            var v1 = () => circle.Position - minCurrVertex();
            var v2 = () => minNextVertex() - minCurrVertex();

            var drawV1V2 = () =>
            {
                Graphics.DrawVectorRel(minCurrVertex(), v1(), _line);
                Graphics.DrawVectorRel(minCurrVertex(), v2(), _line);
            };

            yield return new CollisionStepResult
            {
                Step = $"Vertex to circle v1, Vertex to next Vertex v2",
                Draw = () =>
                {
                    drawV1V2();
                }
            };
        }
        else
        {
            //=== DECISION # 2D ================================================
            yield return new CollisionStepResult
            {
                Step = $"No positive projection found, meaning circle center is inside of polygon"
            };

            yield break;
        }

    }
}
