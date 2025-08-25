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

    public bool ProcessEnded { get; set; }

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

        // Set outside for region C use
        Func<Vector2> edge = null;
        Func<Vector2> normal = null;
        Action drawNormal = null;
        Action drawVertexToCircleCenter = null;
        Action drawProj = null;

        // Loop through all the edges of the polygon/box
        for (var i = 0; i < polygonShape.WorldVertices.Length; i++)
        {
            var iSaved = i;
            edge = () => polygonShape.WorldEdgeAt(iSaved);

            var v0 = () => polygonShape.WorldVertices[iSaved];

            //=== STEP HIGHLIGHT EDGE ==========================================

            var drawEdge = () =>
            {
                var p2 = v0() + edge();

                Graphics.Top
                    .P0(v0())
                    .P1(p2)
                    .Color(Theme.EdgeSelected)
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

            normal = () => edge().RightUnitNormal();
            drawNormal = () =>
            {
                Graphics.Mid.Color(Theme.Normals).Thickness(2).Default();
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

            drawVertexToCircleCenter = () =>
            {
                Graphics.Mid.P0(v0()).P1(circle.Position).Color(Theme.Normals).Thickness(2).DrawVector();
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

            drawProj = () =>
            {
                var p2 = v0() + normal() * projection();

                Graphics.Mid.P0(v0()).P1(p2).Color(Theme.Projection).Thickness(2).DrawVector();
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
                    MinEdgeFound = true
                };

                break;
            }
            //=== DECISION # 1B ================================================
            // No positive projection found, so the center of the circle is inside
            // the polygon, but we are tracking the "closest" edge by tracking
            // the largest distance value (that is negativex)
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

            var edgeVertex0 = minCurrVertex;
            var edgeVertex1 = minNextVertex;

            var drawV1V2Proj = CreateDrawV1V2Proj(
                cirPos,
                edgeVertex0,
                edgeVertex1,
                out var vertexToCirc,
                out var vertexToCircProj);

            yield return new CollisionStepResult
            {
                Step = $"Vertex to circle v1, Vertex to next Vertex v2",
                Draw = () =>
                {
                    drawV1V2Proj();
                }
            };

            var drawDistance = () =>
            {
                Graphics.Mid.P0(edgeVertex0()).P1(cirPos()).Color(Theme.Projection).Thickness(2).DrawLine();
            };

            #region Region A check =============================================

            if (vertexToCircProj() < 0)
            {
                //===  REGION A CONFIRMED ======================================

                yield return new CollisionStepResult
                {
                    Step = $"Circle center is in region A"
                };

                yield return new CollisionStepResult
                {
                    Step = $"Comparing distance from vertex to circle center",
                    Draw = () =>
                    {
                        drawDistance();
                    }
                };

                if (vertexToCirc().Length() > circleShape.Radius)
                {
                    //=== DISTANCE TOO FAR, NO COLLISION =======================

                    yield return new CollisionStepResult
                    {
                        Step = $"Circle center is in region A, but distance to vertex is {vertexToCirc().Length():F2} > circle radius {circleShape.Radius:F2}",
                        CollisionDetected = false,
                        ProcessEnded = true,
                        Draw = () =>
                        {
                            drawDistance();
                        }
                    };

                    yield break;
                }

                //=== DISTANCE WITHIN RADIUS, COLLISION ========================

                yield return new CollisionStepResult
                {
                    Step = $"Distance to vertex is less than circle radius",
                    CollisionDetected = true,
                    Draw = () =>
                    {
                        drawDistance();
                    }
                };

                var drawDepth = CreateDrawDepth(
                    vertexToCirc,
                    circle,
                    circleShape,
                    edgeVertex0,
                    out var contactStart,
                    out var contactEnd);

                yield return new CollisionStepResult
                {
                    Step = $"Depth is the remainder of the circle radius and distance",
                    CollisionDetected = true,
                    Draw = () =>
                    {
                        drawDistance();
                        drawDepth();
                    }
                };

                contact = new Contact();
                contacts.Add(contact);

                contact.A = polygon;
                contact.B = circle;
                contact.Depth = circleShape.Radius - vertexToCirc().Length();
                contact.Normal = Vector2.Normalize(vertexToCirc());
                contact.Start = circle.Position + (contact.Normal * -circleShape.Radius);
                contact.End = contact.Start + contact.Normal * contact.Depth;

                yield return new CollisionStepResult
                {
                    Step = $"Start of contact",
                    Draw = () =>
                    {
                        drawDistance();
                        drawDepth();
                        Graphics.DrawVertex(contactStart(), Color.GreenYellow);
                    }
                };

                yield return new CollisionStepResult
                {
                    Step = $"End of contact",
                    Draw = () =>
                    {
                        drawDistance();
                        drawDepth();
                        Graphics.DrawVertex(contactStart(), Color.GreenYellow);
                        Graphics.DrawVertex(contactEnd(), Color.MonoGameOrange);
                    }
                };

                yield return new CollisionStepResult
                {
                    Step = $"Check results",
                    Draw = () =>
                    {
                        Graphics.DrawVertex(contactStart(), Color.GreenYellow);
                        Graphics.DrawVertex(contactEnd(), Color.MonoGameOrange);
                    }
                };

                yield return new CollisionStepResult
                {
                    Step = $"Check ended",
                    Contact = contact,
                    CollisionDetected = true,
                    ProcessEnded = true,
                };

                yield break;

            } // Region A check
            #endregion

            //=== DECISION # 2B ================================================
            // Check for region B

            edgeVertex0 = minNextVertex;
            edgeVertex1 = minCurrVertex;

            drawV1V2Proj = CreateDrawV1V2Proj(
                cirPos,
                edgeVertex0,
                edgeVertex1,
                out vertexToCirc,
                out vertexToCircProj);

            yield return new CollisionStepResult
            {
                Step = $"Circle center not in region A, checking for region B",
                Draw = () =>
                {
                    drawV1V2Proj();
                }
            };

            #region Region B Check

            if (vertexToCircProj() < 0)
            {
                //===  REGION B CONFIRMED ======================================

                yield return new CollisionStepResult
                {
                    Step = $"Circle center is in region B"
                };

                yield return new CollisionStepResult
                {
                    Step = $"Comparing distance from vertex to circle center",
                    Draw = () =>
                    {
                        drawDistance();
                    }
                };

                if (vertexToCirc().Length() > circleShape.Radius)
                {
                    //=== DISTANCE TOO FAR, NO COLLISION =======================

                    yield return new CollisionStepResult
                    {
                        Step = $"Circle center is in region B, but distance to vertex is {vertexToCirc().Length():F2} > circle radius {circleShape.Radius:F2}",
                        CollisionDetected = false,
                        ProcessEnded = true,
                        Draw = () =>
                        {
                            drawDistance();
                        }
                    };

                    yield break;
                }

                //=== DISTANCE WITHIN RADIUS, COLLISION ========================

                yield return new CollisionStepResult
                {
                    Step = $"Distance to vertex is less than circle radius",
                    CollisionDetected = true,
                    Draw = () =>
                    {
                        drawDistance();
                    }
                };

                var drawDepth = CreateDrawDepth(
                    vertexToCirc,
                    circle,
                    circleShape,
                    edgeVertex0,
                    out var contactStart,
                    out var contactEnd);

                yield return new CollisionStepResult
                {
                    Step = $"Depth is the remainder of the circle radius and distance",
                    CollisionDetected = true,
                    Draw = () =>
                    {
                        drawDistance();
                        drawDepth();
                    }
                };

                contact = new Contact();
                contacts.Add(contact);

                contact.A = polygon;
                contact.B = circle;
                contact.Depth = circleShape.Radius - vertexToCirc().Length();
                contact.Normal = Vector2.Normalize(vertexToCirc());
                contact.Start = circle.Position + (contact.Normal * -circleShape.Radius);
                contact.End = contact.Start + contact.Normal * contact.Depth;

                yield return new CollisionStepResult
                {
                    Step = $"Start of contact",
                    Draw = () =>
                    {
                        drawDistance();
                        drawDepth();
                        Graphics.DrawVertex(contactStart(), Color.GreenYellow);
                    }
                };

                yield return new CollisionStepResult
                {
                    Step = $"End of contact",
                    Draw = () =>
                    {
                        drawDistance();
                        drawDepth();
                        Graphics.DrawVertex(contactStart(), Color.GreenYellow);
                        Graphics.DrawVertex(contactEnd(), Color.MonoGameOrange);
                    }
                };

                yield return new CollisionStepResult
                {
                    Step = $"Check results",
                    Draw = () =>
                    {
                        Graphics.DrawVertex(contactStart(), Color.GreenYellow);
                        Graphics.DrawVertex(contactEnd(), Color.MonoGameOrange);
                    }
                };

                yield return new CollisionStepResult
                {
                    Step = $"Check ended",
                    Contact = contact,
                    CollisionDetected = true,
                    ProcessEnded = true,
                };

                yield break;
            }

            #endregion

            //=== REGION C CONFIGRMED ==========================================

            yield return new CollisionStepResult
            {
                Step = $"Circle center is not in region B, so it must be in region C"
            };

            yield return new CollisionStepResult
            {
                Step = $"Region C uses the method for finding the right edge",
                Draw = () =>
                {
                    drawNormal();
                    drawVertexToCircleCenter();
                    drawProj();
                }
            };

            if (distanceCircleEdge > circleShape.Radius)
            {
                //=== DISTANCE TOO FAR, NO COLLISION ===========================
                yield return new CollisionStepResult
                {
                    Step = $"Projection is greater than circle radius / Projection = {distanceCircleEdge:F1} > circle radius {circleShape.Radius:F0}",
                    CollisionDetected = false,
                    ProcessEnded = true
                };
                yield break;
            }

            //=== DISTANCE WITHIN RADIUS, COLLISION ============================
            
            var vertexToCircle = () => circle.Position - minCurrVertex();
            var normalC = () => (minNextVertex() - minCurrVertex()).RightUnitNormal();
            var projection = () => Vector2.Dot(vertexToCircle(), normalC());


            var depthC = () => circleShape.Radius - projection();
            var contactStartC = () => cirPos() - normalC() * circleShape.Radius;
            var contactEndC = () => contactStartC() + normalC() * depthC();

            yield return new CollisionStepResult
            {
                Step = $"Contact start",
                Draw = () =>
                {
                    Graphics.Mid.P0(cirPos()).P1(contactEndC()).Color(Color.Aqua).DrawLine();
                    Graphics.DrawVertex(contactStartC(), Color.GreenYellow);
                    Graphics.DrawVertex(contactEndC(), Color.MonoGameOrange);
                }
            };
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
        out Func<Vector2> vertexToCirc,
        out Func<float> vertexToCircProj)
    {
        vertexToCirc = () => circlePos() - vertexFrom();
        var edgeVertex0To1 = () => vertexTo() - vertexFrom();

        var vertexToCircCopy = vertexToCirc; // We can't use v1 directly in the lambda below, so we copy it

        var edgeVertex0To1Norm = () => Vector2.Normalize(edgeVertex0To1());
        vertexToCircProj = () => Vector2.Dot(vertexToCircCopy(), edgeVertex0To1Norm());

        var vertexToCircProjCopy = vertexToCircProj;

        return () =>
        {
            var vertexToCircCopyVal = vertexToCircCopy();
            var edgeVertex0To1Val = edgeVertex0To1();

            Graphics.DrawVectorRel(vertexFrom(), vertexToCircCopyVal, Theme.Normals);

            Graphics.Mid.P0(vertexFrom()).P1(vertexTo()).Color(Theme.EdgeSelected).DrawLine();

            
            Graphics.DrawVectorRel(vertexFrom(), edgeVertex0To1Norm() * 50, Theme.EdgeSelected);

            Graphics.DrawVectorRel(vertexFrom(), edgeVertex0To1Norm() * vertexToCircProjCopy(), Theme.Projection);
        };
    }

    private static Action CreateDrawDepth(
        Func<Vector2> vertexToCirc,
        Body circle,
        CircleShape circleShape,
        Func<Vector2> edgeVertex0,
        out Func<Vector2> contactStartCopy,
        out Func<Vector2> contactEndCopy)
    {
        var contactNormal = () => Vector2.Normalize(vertexToCirc());
        
        var contactStart = () => circle.Position + (contactNormal() * -circleShape.Radius);
        var contactEnd = () => contactStart() + contactNormal() * (circleShape.Radius - vertexToCirc().Length());

        contactStartCopy = contactStart;
        contactEndCopy = contactEnd;

        var drawDepth = () =>
        {
            var depth = circleShape.Radius - vertexToCirc().Length();
            Graphics.Mid.P0(edgeVertex0()).P1(contactStart()).Color(Theme.Normals).DrawLine();

            var centerDepth = edgeVertex0() - contactNormal() * depth * 0.5f;

            Graphics.Text
                .Position(centerDepth)
                .Rotation(0)
                .Scale(0.75f)
                .Color(Color.White)
                .Anchor(TextAnchor.Center)
                .Text($"depth = {depth:F1}");

            var centerDist = edgeVertex0() + vertexToCirc() * 0.5f;
            var centerDistLen = centerDist.Length();

            Graphics.Text
                .Position(centerDist)
                .Rotation(0)
                .Scale(0.75f)
                .Color(Color.White)
                .Anchor(TextAnchor.Center)
                .Text($"distance = {vertexToCirc().Length():F1}");
        };

        return drawDepth;
    }
}
