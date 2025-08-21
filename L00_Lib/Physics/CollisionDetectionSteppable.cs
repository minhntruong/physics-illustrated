using Microsoft.Xna.Framework;
using PhysicsIllustrated.Library.Illustrators;
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

    public bool MinEdgeFound { get; set; }

    public float? DistanceCircleEdge { get; set; }
    public Contact Contact { get; set; }
    public bool? CollisionDetected { get; set; }

    public Action Draw;
}

public static class CollisionDetectionSteppable
{
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
            Step = "Finding closest facing edge",
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

                Graphics.Top
                    .P0(v0())
                    .P1(p2)
                    .Color(Theme.ShapeHilite)
                    .Thickness(2)
                    .DrawLine();

                Graphics.DrawVertex(v0());
                Graphics.DrawVertex(p2);
            };

            yield return new CollisionStepResult
            {
                Step = $"Evaluating edge {i}",
                Draw = drawEdge
            };

            //=== STEP NORMAL ==================================================

            var normal = () => edge().RightUnitNormal();
            var drawNormal = () =>
            {
                Graphics.Mid.Color(Theme.ShapeLolite).Thickness(2).Default();
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
                Graphics.Mid.P0(v0()).P1(circle.Position).Color(Theme.ShapeLolite).Thickness(2).DrawVector();
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

                Graphics.Mid.P0(v0()).P1(p2).Color(Theme.ShapeStandout).Thickness(2).DrawVector();
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
            // A positive projection means the circle center is outside of the polygon

            if (projection() > 0)
            {
                distanceCircleEdge = projection();

                var index = i;
                minCurrVertex = () => polygonShape.WorldVertices[index];
                minNextVertex = () => polygonShape.WorldVertexAfter(index);

                circleCenterIsOutside = true;

                yield return new CollisionStepResult
                {
                    Step = $"Positive projection found: {projection():F1}, circle center is outside of polygon",
                    MinCurrVertex = minCurrVertex,
                    MinNextVertex = minNextVertex,
                    MinEdgeFound = true,
                    //Draw = () =>
                    //{
                    //    drawEdge();
                    //    drawVertexToCircleCenter();
                    //    drawNormal();
                    //    drawProj();
                    //}
                };

                break;
            }
            //=== DECISION # 1B ================================================
            // No positive projection found, but we keep the largest negative value
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
                        Step = $"Positive projection not found, but value {distanceCircleEdge:F1} is larger than prev {temp}",
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
            // Circle center is outside of polygon, so we check region A

            yield return new CollisionStepResult
            {
                Step = $"Circle center is outside of polygon, checking for region A",
                IsOutside = circleCenterIsOutside,
            };

            var cirPos = () => circle.Position;

            var drawV1V2Proj = CreateDrawV1V2Proj(
                cirPos,
                minCurrVertex,
                minNextVertex,
                out var v1,
                out var v1Proj);

            yield return new CollisionStepResult
            {
                Step = $"Vertex to circle v1, Vertex to next Vertex v2",
                Draw = () =>
                {
                    drawV1V2Proj();
                }
            };

            if (v1Proj() < 0)
            {
                //===  REGION A CONFIRMED ======================================

                yield return new CollisionStepResult
                {
                    Step = $"Circle center is in region A"
                };

                var drawDistance = () =>
                {
                    Graphics.Mid.P0(minCurrVertex()).P1(cirPos()).Color(Theme.ShapeStandout).Thickness(2).DrawLine();
                };

                yield return new CollisionStepResult
                {
                    Step = $"Comparing distance from vertex to circle center",
                    Draw = () =>
                    {
                        drawDistance();
                    }
                };

                if (v1().Length() > circleShape.Radius)
                {
                    //=== DISTANCE TOO FAR, NO COLLISION =======================

                    yield return new CollisionStepResult
                    {
                        Step = $"Circle center is in region A, but distance to edge is {v1().Length():F2} > circle radius {circleShape.Radius:F2}",
                        CollisionDetected = false,
                        Draw = () =>
                        {
                            drawV1V2Proj();
                        }
                    };

                    yield break;
                }

                //=== DISTANCE WITHIN RADIUS, COLLISION ========================

                // TODO: Calculate Contact
            } // Region A check

            //=== DECISION # 2B ================================================
            // Check for region B

            drawV1V2Proj = CreateDrawV1V2Proj(
                cirPos,
                minNextVertex,
                minCurrVertex,
                out v1,
                out v1Proj);

            //v1 = () => cirPos() - minNextVertex();
            //v2 = () => minCurrVertex() - minNextVertex();

            //v2Norm = () => Vector2.Normalize(v2());
            //v1Proj = () => Vector2.Dot(v1(), v2Norm());

            yield return new CollisionStepResult
            {
                Step = $"Circle center not in region A, checking for region B",
                Draw = () =>
                {
                    drawV1V2Proj();
                }
            };

            if (v1Proj() < 0)
            {
                //===  REGION B CONFIRMED ======================================

                yield return new CollisionStepResult
                {
                    Step = $"Circle center is in region B"
                };
            }

            //=== REGION C CONFIGRMED ==========================================

            yield return new CollisionStepResult
            {
                Step = $"Circle center is not in region B, so it must be in region C"
            };

            //
        }
        else
        {
            //=== DECISION # 2D ================================================
            yield return new CollisionStepResult
            {
                Step = $"No positive projection found, circle center is inside of polygon"
            };

            yield return new CollisionStepResult
            {
                Step = $"Closest edge will be used to calculate contact info"
            };

            yield break;
        }
    }

    //==========================================================================

    private static Action CreateDrawV1V2Proj(
        Func<Vector2> circlePos,
        Func<Vector2> vertexFrom,
        Func<Vector2> vertexTo,
        out Func<Vector2> v1,
        out Func<float> v1Proj)
    {
        v1 = () => circlePos() - vertexFrom();
        var v2 = () => vertexTo() - vertexFrom();

        var v1Copy = v1; // We can't use v1 directly in the lambda below, so we copy it

        var v2Norm = () => Vector2.Normalize(v2());
        v1Proj = () => Vector2.Dot(v1Copy(), v2Norm());

        var v1ProjCopy = v1Proj;

        return () =>
        {
            var v1Val = v1Copy();
            var v2Val = v2();

            Graphics.DrawVectorRel(vertexFrom(), v1Val, Theme.ShapeLolite);
            Graphics.DrawVectorRel(vertexFrom(), v2Val, Theme.ShapeLolite);
            Graphics.DrawVectorRel(vertexFrom(), v2Norm() * v1ProjCopy(), Theme.ShapeStandout);
        };
    }
}
