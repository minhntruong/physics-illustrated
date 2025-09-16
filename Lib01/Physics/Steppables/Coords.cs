using System;
using Microsoft.Xna.Framework;
using ShowPhysics.Library.Managers;
using ShowPhysics.Library.Managers.Text;
using ShowPhysics.Library.Physics.Math;
using ShowPhysics.Library.Physics.Shapes;

namespace ShowPhysics.Library.Physics.Steppables;

public static class Coords
{
    public static Vector2 Vertex(PolygonShape poly, int vertexInd)
    {
        return poly.WorldVertices[vertexInd];
    }

    public static (Vector2 Start, Vector2 End) Edge(PolygonShape poly, int vertexInd, bool reverse = false)
    {
        var v1 = poly.WorldVertices[vertexInd];
        var v2 = poly.WorldVertexAfter(vertexInd);

        if (reverse)
        {
            var vTemp= v1;
            v1 = v2;
            v2 = vTemp;
        }

        return (v1, v2);
    }

    public static (Vector2 Start, Vector2 Normal) EdgeNormal(PolygonShape poly, int vertexInd)
    {
        var v = poly.WorldVertices[vertexInd];
        var edge = poly.WorldEdgeAt(vertexInd);
        var normal = edge.RightUnitNormal();

        return (v, normal);
    }

    public static (Vector2 Start, Vector2 End) VertexToBody(PolygonShape poly, int vertexInd, Body body)
    {
        var v1 = poly.WorldVertices[vertexInd];
        var v2 = body.Position;

        return (v1, v2);
    }

    public static (Vector2 Start, Vector2 End) BodyToVertex(Body body, PolygonShape poly, int vertexInd)
    {
        var v1 = body.Position;
        var v2 = poly.WorldVertices[vertexInd];

        return (v1, v2);
    }

    public static (Vector2 Start, Vector2 ToBody, Vector2 EdgeNormal) EdgeNormalToBody(PolygonShape poly, int vertexInd, Body body)
    {
        var (start, edgeNormal) = EdgeNormal(poly, vertexInd);
        var (v1, v2) = VertexToBody(poly, vertexInd, body);

        // start & v1 should be the same

        return (start, v2 - v1, edgeNormal);
    }

    public static (Vector2 Start, Vector2 ToBody, Vector2 EdgeUnit) EdgeUnitToBody(PolygonShape poly, int vertexInd, Body body, bool reverse = false)
    {
        var start = poly.WorldVertices[vertexInd];
        var edgeEnd = poly.WorldVertexAfter(vertexInd);

        if (reverse)
        {
            var temp = start;
            start = edgeEnd; 
            edgeEnd = temp;
        }

        var unit = Vector2.Normalize(edgeEnd - start);
        var toBody = body.Position - start;

        // start & v1 should be the same

        return (start, toBody, unit);
    }

    public static (Vector2 Start, Vector2 End) BodyExtentToVertex(Body body, float extent, PolygonShape poly, int vertexInd)
    {
        var v1 = body.Position;
        var vertex = poly.WorldVertices[vertexInd];
        var dir = Vector2.Normalize(vertex - v1);
        var v2 = v1 + dir * extent;

        return (v1, v2);
    }
}

public static class CoordinatesExtensions
{
    public static void DrawVertex(this Vector2 v)
    {
        Graphics.DrawVertex(v);
    }

    public static void DrawLabeledDistance(this (Vector2 v1, Vector2 v2) data, float threshold = 0, bool showLabel = true, float transitionFactor = 1)
    {
        var color = Theme.ContactDistance;

        if (threshold > 0 && Vector2.Distance(data.v1, data.v2) <= threshold)
        {
            color = Theme.ContactDistanceThreshold;
        }

        var v1 = data.v1;
        var v2 = data.v1 + (data.v2 - data.v1) * transitionFactor;

        Graphics.Mid.Line()
            .Start(v1)
            .End(v2)
            .Color(color)
            .ThicknessAbs(Theme.ShapeOverlayLineThicknessAbs)
            .Stroke();

        if (!showLabel) { return; }

        if (transitionFactor < 1) { return; }

        Graphics.Text
            .Color(Theme.Label)
            .Scale(Theme.LabelScale)
            .TextLengthOf(v1, v2);
    }

    public static void DrawEdgeNormalRef(this (Vector2 Start, Vector2 Normal) data, float distanceFactor = 1)
    {
        Graphics.Mid.States().ThicknessAbs(Theme.ShapeLineThicknessAbs).Default();

        Graphics.Mid.Vector()
            .Start(data.Start)
            .End(data.Start + data.Normal * distanceFactor * 50)
            .Color(Theme.Normals)
            .Stroke();

        Graphics.Mid.Line()
            .Start(data.Start)
            .End(data.Start + data.Normal * 1500)
            .Color(Theme.Normals)
            .Stroke();
    }

    public static void DrawVector(this (Vector2 Start, Vector2 End) data, Color color, float distanceFactor = 1, float length = 0)
    {
        var dir = data.End - data.Start;

        if (length > 0)
        {
            dir = Vector2.Normalize(dir) * length;
        }

        Graphics.Mid.Vector()
            .Start(data.Start)
            .End(data.Start + dir * distanceFactor)
            .ThicknessAbs(Theme.ShapeLineThicknessAbs)
            .Color(color)
            .Stroke();
    }

    public static void DrawEdge(this (PolygonShape Poly, int VertexInd) data)
    {
        var v1 = data.Poly.WorldVertices[data.VertexInd];
        var v2 = data.Poly.WorldVertexAfter(data.VertexInd);

        Graphics.Mid.Line()
            .Start(v1)
            .End(v2)
            .Color(Theme.EdgeSelected)
            .ThicknessAbs(Theme.ShapeOverlayLineThicknessAbs)
            .Stroke();

        Graphics.DrawVertex(v1);
        Graphics.DrawVertex(v2);
    }

    public static void DrawLine(this (Vector2 Start, Vector2 End) data, Color color, float transitionFactor = 1)
    {
        Graphics.Mid.Line()
            .Start(data.Start)
            .End(data.Start + (data.End - data.Start) * transitionFactor)
            .Color(color)
            .ThicknessAbs(Theme.ShapeLineThicknessAbs)
            .Stroke();
    }

    public static void DrawProjection(this (Vector2 Start, Vector2 V1, Vector2 V2) data, float transitionFactor = 1)
    {
        var projection = Vector2.Dot(data.V1, data.V2);
        
        var nV1 = data.Start;
        var nV2TransStart = data.Start + data.V1;
        var nV2TransEnd = data.Start + data.V2 * projection;

        var nV2 = Vector2.Lerp(nV2TransStart, nV2TransEnd, transitionFactor);

        var color = Theme.Projection;

        Graphics.Mid.Vector()
            .Start(nV1)
            .End(nV2)
            .Color(color)
            .ThicknessAbs(Theme.ShapeOverlayLineThicknessAbs)
            .Stroke();

        // Don't show length if transition isn't complete
        if (transitionFactor < 1) { return; }

        var formatted = projection >= 0 ? projection.ToString("0.0") : $"({projection:0.0})";

        Graphics.Text
            .Color(Theme.Label)
            .Scale(Theme.LabelScale)
            .TextLengthOf(nV1, nV2, false)
            .Text(formatted);
    }
}