using Microsoft.Xna.Framework;
using ShowPhysics.Library.Physics.Math;
using System;
using System.Collections.Generic;

namespace ShowPhysics.Library.Physics.Shapes;

public class PolygonShape : Shape
{
    public PolygonShape(params (float X, float Y)[] vertices)
    {
        if (vertices.Length < 3)
        {
            throw new ArgumentException("A polygon must have at least 3 vertices.");
        }

        LocalVertices = new Vector2[vertices.Length];
        WorldVertices = new Vector2[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            LocalVertices[i] = new Vector2(vertices[i].X, vertices[i].Y);
            WorldVertices[i] = new Vector2(vertices[i].X, vertices[i].Y); // Initially, world vertices are the same as local
        }
    }

    public override string ToString()
    {
        return $"{Type}";
    }

    public override ShapeType Type => ShapeType.Polygon;

    public override float MomentOfInertiaFactor => 5000; // TODO

    public Vector2[] LocalVertices { get; protected set; } = Array.Empty<Vector2>();

    public Vector2[] WorldVertices { get; protected set; } = Array.Empty<Vector2>();

    public Vector2 WorldVertexAfter(int index)
    {
        var nextIndex = NextVertexIndex(index);

        return WorldVertices[nextIndex];
    }

    public Vector2 WorldEdgeAt(int index)
    {
        if (index < 0 || index >= WorldVertices.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range for world vertices.");
        }

        var nextIndex = NextVertexIndex(index);

        return WorldVertices[nextIndex] - WorldVertices[index];
    }

    public Vector2 WorldEdgeAfter(int index)
    {
        var nextIndex = NextVertexIndex(index);

        return WorldEdgeAt(nextIndex);
    }

    public float FindMinSeparation(PolygonShape other, ref int indexReferenceEdge, ref Vector2 supportPoint)
    {
        float separation = float.MinValue;

        // Loop all the vertices of "this" polygon
        for (int i = 0; i < WorldVertices.Length; i++) 
        {
            Vector2 va = WorldVertices[i];
            Vector2 normal = WorldEdgeAt(i).RightUnitNormal();
        
            // Loop all the vertices of the "other" polygon
            float minSep = float.MaxValue;
            Vector2 minVertex = Vector2.Zero;
            
            for (int j = 0; j < other.WorldVertices.Length; j++)
            {
                Vector2 vb = other.WorldVertices[j];
                float proj = Vector2.Dot(vb - va, normal);
                if (proj < minSep)
                {
                    minSep = proj;
                    minVertex = vb;
                }
            }

            if (minSep > separation)
            {
                separation = minSep;
                indexReferenceEdge = i;
                supportPoint = minVertex;
            }
        }
        return separation;
    }

    public int FindIncidentEdge(Vector2 normal)
    {
        int indexIncidentEdge = -1;
        float minProj = float.MaxValue;
        
        for (int i = 0; i < WorldVertices.Length; ++i) 
        {
            var edgeNormal = WorldEdgeAt(i).RightUnitNormal();
            var proj = Vector2.Dot(edgeNormal, normal);
            
            if (proj < minProj)
            {
                minProj = proj;
                indexIncidentEdge = i;
            }
        }
        return indexIncidentEdge;
    }

    public int ClipSegmentToLine(List<Vector2> contactsIn, List<Vector2> contactsOut, Vector2 c0, Vector2 c1)
    {
        // Start with no output points
        int numOut = 0;

        // Calculate the distance of end points to the line
        Vector2 normal = Vector2.Normalize(c1 - c0);
        float dist0 = (contactsIn[0] - c0).Cross(normal);
        float dist1 = (contactsIn[1] - c0).Cross(normal);

        // If the points are behind the plane
        if (dist0 <= 0)
            contactsOut[numOut++] = contactsIn[0];
        if (dist1 <= 0)
            contactsOut[numOut++] = contactsIn[1];

        // If the points are on different sides of the plane (one distance is negative and the other is positive)
        if (dist0 * dist1 < 0) {
            float totalDist = dist0 - dist1;

            // Fint the intersection using linear interpolation: lerp(start,end) => start + t*(end-start)
            float t = dist0 / (totalDist);
            Vector2 contact = contactsIn[0] + (contactsIn[1] - contactsIn[0]) * t;
            contactsOut[numOut] = contact;
            numOut++;
        }
        return numOut;
    }

    public override void UpdateVertices(float rotation, Vector2 position)
    {
        for (int i = 0; i < LocalVertices.Length; i++)
        {
            // Rotate and translate each vertex
            var vertex = LocalVertices[i];
            var rotatedX = vertex.X * MathF.Cos(rotation) - vertex.Y * MathF.Sin(rotation);
            var rotatedY = vertex.X * MathF.Sin(rotation) + vertex.Y * MathF.Cos(rotation);
            WorldVertices[i] = new Vector2(rotatedX, rotatedY) + position;
        }
    }

    public int NextVertexIndex(int index)
    {
        var nextIndex = index + 1;
        if (nextIndex >= WorldVertices.Length)
        {
            nextIndex = 0; // Wrap around to the first vertex
        }

        return nextIndex;
    }
}