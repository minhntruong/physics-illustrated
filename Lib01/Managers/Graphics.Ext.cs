using System;
using Microsoft.Xna.Framework;
using ShowPhysics.Library.Managers.Text;
using ShowPhysics.Library.Physics;
using ShowPhysics.Library.Physics.Math;
using ShowPhysics.Library.Physics.Shapes;

namespace ShowPhysics.Library.Managers;

public static partial class Graphics
{
    public static void DrawLabeledDistance(Body a, Body b, bool label = true)
    {
        DrawLabeledDistance(a.Position, b.Position, 0, label);
    }

    public static void DrawLabeledDistance(Body a, Body b, float threshold, bool label = true)
    {
        DrawLabeledDistance(a.Position, b.Position, threshold, label);
    }

    public static void DrawLabeledDistance(PolygonShape poly, int vertex, Body b, float threshold = 0, bool label = true)
    {
        var v1 = poly.WorldVertices[vertex];
        var v2 = b.Position;

        DrawLabeledDistance(v1, v2, threshold, label);
    }

    public static void DrawLabeledDistance(Vector2 v1, Vector2 v2, float threshold = 0, bool label = true)
    {
        var color = Theme.ContactDistance;

        if (threshold > 0 && (v2 - v1).Length() < threshold)
        {
            color = Theme.ContactDistanceThreshold;
        }

        Mid.Line()
            .Start(v1)
            .End(v2)
            .Color(color)
            .ThicknessAbs(Theme.ShapeLineThicknessAbs)
            .Stroke();

        if (label)
            Text.Color(Theme.Label).Scale(Theme.LabelScale).TextLengthOf(v1, v2);
    }

    public static void DrawRadius(Body a)
    {
        if (a.Shape is CircleShape circle)
        {
            var end = a.Position + new Vector2(-circle.Radius, 0);

            Bot.Line()
                .Start(a.Position)
                .End(end)
                .Color(Theme.Shape)
                .ThicknessAbs(2)
                .Stroke();

            Text.TextLengthOf(a.Position, end).Color(Theme.Label).Scale(Theme.LabelScale);
        }
    }

    public static void DrawVertex(Vector2 position)
    {
        DrawVertex(position, Color.White);
    }

    public static void DrawVertex(Vector2 position, Color color, bool highlight = false)
    {
        if (highlight)
        {
            Top.Rect()
                .Center(position)
                .Width(16)
                .Height(16)
                .SizeAbs()
                .Color(color)
                .Fill();

            Top.Rect()
                .Center(position)
                .Width(10)
                .Height(10)
                .SizeAbs()
                .Color(Color.White)
                .Fill();
        }
        else
        {
            Top.Rect()
                .Center(position)
                .Width(6)
                .Height(6)
                .SizeAbs()
                .Color(color)
                .Fill();
        }
    }

    public static void DrawSelectedEdge(PolygonShape shape, int vertexIndex)
    {
        var v1 = shape.WorldVertices[vertexIndex];
        var v2 = shape.WorldVertexAfter(vertexIndex);

        Mid.Line()
            .Start(v1)
            .End(v2)
            .Color(Theme.EdgeSelected)
            .ThicknessAbs(Theme.ShapeLineThicknessAbs)
            .Stroke();

        DrawVertex(v1);
        DrawVertex(v2);
    }

    public static void DrawNormal(PolygonShape shape, int vertexIndex)
    {
        var v1 = shape.WorldVertices[vertexIndex];
        var edge = shape.WorldEdgeAt(vertexIndex);
        var normal = edge.RightUnitNormal();

        Mid.States().ThicknessAbs(Theme.ShapeLineThicknessAbs).Default();

        Mid.Vector().Start(v1).End(v1 + normal * 50).Color(Theme.Normals).Stroke();

        Mid.Line().Start(v1).End(v1 + normal * 1500).Color(Theme.Normals).Stroke();
    }

    public static void DrawLineToBody(PolygonShape shape, int vertexIndex, Body target, Color color)
    {
        var v1 = shape.WorldVertices[vertexIndex];
        var v2 = target.Position;

        Mid.States().ThicknessAbs(Theme.ShapeLineThicknessAbs).Default();
        Mid.Line().Start(v1).End(v2).Color(color).Stroke();
    }

    public static void DrawVectorToBody(PolygonShape shape, int vertexIndex, Body target)
    {
        var v1 = shape.WorldVertices[vertexIndex];
        var v2 = target.Position;

        Mid.States().ThicknessAbs(Theme.ShapeLineThicknessAbs).Default();
        Mid.Vector().Start(v1).End(v2).Color(Theme.Normals).Stroke();
    }

    public static void DrawVectorFromBody(Body source, PolygonShape destShape, int destVertexIndex, float distance, Color color)
    {
        var v1 = source.Position;
        var v2 = destShape.WorldVertices[destVertexIndex];
        var direction = Vector2.Normalize(v2 - v1);

        Mid.Vector()
            .Start(v1)
            .End(v1 + direction * distance)
            .ThicknessAbs(Theme.ShapeLineThicknessAbs)
            .Color(color)
            .Stroke();
    }

    public static void DrawProjectionOnNormalFromBody(PolygonShape shape, int vertexIndex, Body target)
    {
        var v1 = shape.WorldVertices[vertexIndex];
        var edge = shape.WorldEdgeAt(vertexIndex);
        var normal = edge.RightUnitNormal();
        var toCircle = target.Position - v1;
        var projection = Vector2.Dot(toCircle, normal);
        var v2 = v1 + normal * projection;

        Mid.States().ThicknessAbs(Theme.ShapeLineThicknessAbs).Default();

        Mid.Vector().Start(v1).End(v2).Color(Theme.Projection).Stroke();

        Text
            .Color(Theme.Label)
            .Scale(Theme.LabelScale)
            .TextLengthOf(v1, v2, false)
            .Text(projection.ToString("0.0"));
    }

    public static void DrawProjectionOnEdgeFromBody(PolygonShape shape, int vertexIndex, Body target, bool reverse = false)
    {
        var from = vertexIndex;
        var to = shape.NextVertexIndex(vertexIndex);

        if (reverse)
        {
            from = shape.NextVertexIndex(vertexIndex);
            to = vertexIndex;
        }

        var v1 = shape.WorldVertices[from];
        var vEdge = shape.WorldVertices[to];

        var edge = vEdge - v1;
        edge.Normalize();

        var toCircle = target.Position - v1;
        var projection = Vector2.Dot(toCircle, edge);
        var vProj = v1 + edge * projection;

        Mid.States().ThicknessAbs(Theme.ShapeOverlayLineThicknessAbs).Default();

        Mid.Vector().Start(v1).End(vProj).Color(Theme.Projection).Stroke();

        var formatted = projection >= 0 ? projection.ToString("0.0") : $"({projection:0.0})";

        Text
            .Anchor(TextAnchor.TopBaselineCenter)
            .Scale(Theme.LabelScale)
            .Color(Theme.Label)
            .TextLengthOf(v1, vProj, false)
            .Text(formatted);
    }

    public static void DrawVectorAlongEdge(PolygonShape shape, int vertexIndex, bool reverse = false)
    {
        var from = vertexIndex;
        var to = shape.NextVertexIndex(vertexIndex);
        
        if (reverse)
        {
            from = shape.NextVertexIndex(vertexIndex);
            to = vertexIndex;
        }

        var v1 = shape.WorldVertices[from];
        var v2 = shape.WorldVertices[to];

        var dir = v2 - v1;
        var unit = Vector2.Normalize(dir);
        
        Mid.States().ThicknessAbs(Theme.ShapeLineThicknessAbs).Default();
        Mid.Vector().Start(v1).End(v1 + unit * 50).Color(Theme.EdgeSelected).Stroke();
    }

    public static void DrawContactFromCircle(PolygonShape shape, int vertexIndex, Body circleBody, Color color)
    {
        var v1 = circleBody.Position;
        var normal = Vector2.Normalize(shape.WorldVertices[vertexIndex] - v1);
        var v2 = v1 + normal * (circleBody.Shape as CircleShape).Radius;

        DrawVertex(v2, color, true);
    }

    public static void DrawContact(PolygonShape shape, int vertexIndex, Color color)
    {
        var v1 = shape.WorldVertices[vertexIndex];

        DrawVertex(v1, color, true);
    }


}
