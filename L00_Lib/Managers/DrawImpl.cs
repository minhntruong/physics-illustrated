using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhysicsIllustrated.Library.Managers;

public class DrawImpl
{
    public DrawImpl(GraphicsDevice graphicsDevice, int capacity = 10_000)
    {
        _graphics = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));

        // Initialize BasicEffect
        _effect = new BasicEffect(graphicsDevice);
        _effect.VertexColorEnabled = true;
        _effect.LightingEnabled = false;

        // Create vertex and index buffers
        _vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor), capacity, BufferUsage.WriteOnly);
        _indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, capacity * 3, BufferUsage.WriteOnly);

        // Initialize dynamic lists
        _vertices = new VertexPositionColor[capacity];
        _indices = new int[capacity * 3];

        _vectors = new Vector2[256];

        OnViewportChange();
    }

    private GraphicsDevice _graphics;
    private BasicEffect _effect;
    private bool _isDisposed = false;

    // Vertex and index buffers for different primitive types
    private VertexBuffer _vertexBuffer;
    private IndexBuffer _indexBuffer;

    // Dynamic lists for building geometry
    private VertexPositionColor[] _vertices;
    private int[] _indices;

    private int _verticesCount = 0;
    private int _indicesCount = 0;

    private Vector2[] _vectors;

    private Matrix _view = Matrix.Identity;
    private Matrix _projection;

    #region === IDisposable implementation =====================================

    public void Dispose()
    {
        if (_isDisposed)
            return;

        _effect?.Dispose();
        _vertexBuffer?.Dispose();
        _indexBuffer?.Dispose();

        _isDisposed = true;
    }
    #endregion

    //==========================================================================

    public float Zoom { get; set; } = 1.0f;

    public Vector2 Origin { get; set; } = Vector2.Zero;

    public void OnViewportChange()
    {
        _projection = Matrix.CreateOrthographicOffCenter(
            0f,
            _graphics.Viewport.Width,
            _graphics.Viewport.Height,
            0f,
            0f,
            1f);
    }

    public void Reset()
    {
        _verticesCount = 0;
        _indicesCount = 0;
        ResetStates();
    }

    public void Draw()
    {
        if (_verticesCount == 0)
            return;

        // Turn off face culling so all faces show up
        var prevRasterizerState = _graphics.RasterizerState;
        _graphics.RasterizerState = RasterizerState.CullNone;

        // Upload vertex data
        _vertexBuffer.SetData(_vertices, 0, _verticesCount);
        _indexBuffer.SetData(_indices, 0, _indicesCount);

        // Set up graphics device
        _graphics.SetVertexBuffer(_vertexBuffer);
        _graphics.Indices = _indexBuffer;

        _effect.View = _view;
        _effect.Projection = _projection;

        // Draw all triangles in one call
        foreach (var pass in _effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            _graphics.DrawIndexedPrimitives(
                PrimitiveType.TriangleList,
                0,
                0,
                _indicesCount / 3);
        }

        // Restore previous rasterizer state
        _graphics.RasterizerState = prevRasterizerState;

        Reset();
    }


    //==========================================================================

    private DrawStates _defaultStates = new DrawStates();
    private DrawStates _currentStates = new DrawStates();
    private Vector2 _p0 = Vector2.Zero;
    private Vector2 _p1 = Vector2.Zero;
    private float _width;
    private float _height;
    private float _radius;

    public DrawImpl Color(Color value)
    {
        _currentStates.Color = value;
        return this;
    }

    public DrawImpl Thickness(float value)
    {
        _currentStates.Thickness = value;
        return this;
    }

    public DrawImpl Filled(bool value = true)
    {
        _currentStates.Filled = value;
        return this;
    }

    public void Default()
    {
        _defaultStates.Color = _currentStates.Color;
        _defaultStates.Thickness = _currentStates.Thickness;
    }

    public DrawImpl P0(Vector2 value)
    {
        _p0 = value * Zoom - Origin;
        return this;
    }

    public DrawImpl P1(Vector2 value)
    {
        _p1 = value * Zoom - Origin;
        return this;
    }

    public DrawImpl Width(float value)
    {
        _width = value * Zoom;
        return this;
    }

    public DrawImpl Height(float value)
    {
        _height = value * Zoom;
        return this;
    }

    public DrawImpl Radius(float value)
    {
        _radius = value * Zoom;
        return this;
    }

    public DrawImpl Angle(float value)
    {
        _currentStates.Angle = value;
        return this;
    }

    public void DrawLine()
    {
        DrawLine(_p0, _p1, _currentStates.Color, _currentStates.Thickness);
    }

    public void DrawVector()
    {
        // Draw the main line
        DrawLine(_p0, _p1, _currentStates.Color, _currentStates.Thickness);

        // Arrowhead parameters
        float arrowLength = 16f; // Length of the arrowhead lines
        float arrowAngle = MathF.PI / 7f; // Angle between the arrowhead lines and the main line

        // Direction from P0 to P1
        Vector2 direction = _p1 - _p0;
        if (direction.LengthSquared() < 1e-6f)
            return; // Avoid zero-length vectors

        direction = Vector2.Normalize(direction);

        // Calculate the two arrowhead points
        float theta = MathF.Atan2(direction.Y, direction.X);

        float angle1 = theta + MathF.PI - arrowAngle;
        float angle2 = theta + MathF.PI + arrowAngle;

        Vector2 arrowPoint1 = _p1 + new Vector2(MathF.Cos(angle1), MathF.Sin(angle1)) * arrowLength;
        Vector2 arrowPoint2 = _p1 + new Vector2(MathF.Cos(angle2), MathF.Sin(angle2)) * arrowLength;

        // Draw the two arrowhead lines
        DrawLine(_p1, arrowPoint1, _currentStates.Color, _currentStates.Thickness);
        DrawLine(_p1, arrowPoint2, _currentStates.Color, _currentStates.Thickness);
    }

    public void DrawSquare()
    {
        var coords = GenerateRectCoordinates(_p0, _width, _width);

        if (_currentStates.Filled)
        {
            DrawFillRect(_p0.X, _p0.Y, _width, _width, _currentStates.Color);
        }
        else
        {
            DrawPolygon(_p0, coords, _currentStates.Color, _currentStates.Thickness);
        }
    }

    public void DrawRect()
    {
        var coords = GenerateRectCoordinates(_p0, _width, _height);

        if (_currentStates.Filled)
        {
            DrawFillRect(_p0.X, _p0.Y, _width, _height, _currentStates.Color);
        }
        else
        {
            DrawPolygon(_p0, coords, _currentStates.Color, _currentStates.Thickness);
        }
    }

    public void DrawCircle()
    {
        var vectors = _vectors.AsSpan(0, _currentStates.Segments);

        GenerateCircleCoordinates(_p0, _radius, vectors);

        if (_currentStates.Filled)
        {
            throw new Exception("Filled circles are not implemented yet.");
        }
        else
        {
            // Draw line segments between the generated points
            for (var i = 0; i < vectors.Length - 1; i++)
            {
                var start = vectors[i];
                var end = vectors[i + 1];

                DrawLine(start, end, _currentStates.Color, _currentStates.Thickness);
            }

            // Connect the last point to the first to close the circle
            DrawLine(vectors[vectors.Length - 1], vectors[0], _currentStates.Color, _currentStates.Thickness);

            // Draw the angle line
            if (_currentStates.Angle >= 0)
            {
                DrawLine(
                    _p0,
                    new Vector2(_p0.X + MathF.Cos(_currentStates.Angle) * _radius,
                                _p0.Y + MathF.Sin(_currentStates.Angle) * _radius),
                    _currentStates.Color,
                    _currentStates.Thickness);
            }
        }
    }

    public void DrawPolygon(Vector2[] vertices)
    {
        var xformedVertices = new Vector2[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            // Scale + offset vertices
            xformedVertices[i] = vertices[i] * Zoom - Origin;
        }

        // Stroke only for now
        DrawPolygon(_p0, xformedVertices, _currentStates.Color, _currentStates.Thickness);
    }

    //==========================================================================

    private void ResetStates()
    {
        _currentStates.Color = _defaultStates.Color;
        _currentStates.Thickness = _defaultStates.Thickness;
    }

    private void DrawLine(int x0, int y0, int x1, int y1, Color color, float thickness = 4)
    {
        var start = new Vector2(x0, y0);
        var end = new Vector2(x1, y1);

        DrawLine(start, end, color, thickness);
    }

    private void DrawLine(Vector2 start, Vector2 end, Color color, float thickness = 4)
    {
        var direction = end - start;
        var perpendicular = Vector2.Normalize(new Vector2(-direction.Y, direction.X)) * thickness * 0.5f;

        var trianglesRequired = 2;
        var verticesRequired = 4;
        var indicesRequired = trianglesRequired * 3;

        RequestSpace(verticesRequired, indicesRequired, out var vertices, out var indices, out var startIndex);

        // Create quad for the line
        vertices[0] = new VertexPositionColor(new Vector3(start + perpendicular, 0), color);
        vertices[1] = new VertexPositionColor(new Vector3(start - perpendicular, 0), color);
        vertices[2] = new VertexPositionColor(new Vector3(end - perpendicular, 0), color);
        vertices[3] = new VertexPositionColor(new Vector3(end + perpendicular, 0), color);

        // Add indices for two triangles
        indices[0] = startIndex;
        indices[1] = startIndex + 1;
        indices[2] = startIndex + 2;

        indices[3] = startIndex;
        indices[4] = startIndex + 2;
        indices[5] = startIndex + 3;
    }

    private void DrawPolygon(Vector2 center, Vector2[] vertices, Color color, float thickness = 2)
    {
        for (int i = 0; i < vertices.Length - 1; i++)
        {
            var start = vertices[i];
            var end = vertices[i + 1];
            DrawLine(start, end, color, thickness);
        }

        // Connect the last vertex to the first
        DrawLine(vertices[vertices.Length - 1], vertices[0], color, thickness);

        // Draw center
        //DrawFillCircle(center, 2, color, 4);
    }

    private void DrawCircle(int centerX, int centerY, float radius, float angle, Color color, float thickness = 2, int segments = 32)
    {
        DrawCircle(new Vector2(centerX, centerY), radius, angle, color, thickness, segments);
    }

    private void DrawCircle(Vector2 center, float radius, float angle, Color color, float thickness = 2, int segments = 32)
    {
        var vectors = _vectors.AsSpan(0, segments);

        GenerateCircleCoordinates(center, radius, vectors);

        // Draw line segments between the generated points
        for (var i = 0; i < vectors.Length - 1; i++)
        {
            var start = vectors[i];
            var end = vectors[i + 1];

            DrawLine(start, end, color, thickness);
        }

        // Connect the last point to the first to close the circle
        DrawLine(vectors[vectors.Length - 1], vectors[0], color, thickness);

        // Draw the angle line
        if (angle >= 0)
        {
            DrawLine(
                center,
                new Vector2(center.X + MathF.Cos(angle) * radius,
                            center.Y + MathF.Sin(angle) * radius),
                color,
                thickness);
        }
    }

    private void DrawFillCircle(Vector2 center, float radius, Color color, int segments = 16)
    {
        DrawFillCircle(center.X, center.Y, radius, color, segments);
    }

    private void DrawFillCircle(float centerX, float centerY, float radius, Color color, int segments = 16)
    {
        // Vertices, indices for a triangle list

        var trianglesRequired = segments - 2;         // A 3-segments circle (a triangle) requires 1 triangle, 4 segments requires 2 triangles, etc.
        var verticesRequired = segments;              // Need vertices to define the segments
        var indicesRequired = trianglesRequired * 3;  // 3 indices per triangle

        RequestSpace(verticesRequired, indicesRequired, out var vertices, out var indices, out var startIndex);

        for (var v = 0; v < verticesRequired; v++)
        {
            var angle = (float)v / verticesRequired * MathHelper.TwoPi;
            var x = centerX + MathF.Cos(angle) * radius;
            var y = centerY + MathF.Sin(angle) * radius;

            vertices[v] = new VertexPositionColor(new Vector3(x, y, 0), color);
        }

        // Create indices for the triangle list
        // Indices for the triangle list
        // For segments = 3, then triangleCount = 1, we want the sequence: shape 1: [ 0, 1, 2 ], shape 2: [3, 4, 5]
        // For segments = 4, then triangleCount = 2, we want the sequence: shape 1: [ 0, 1, 2 / 0, 2, 3 ], shape 2: [ 4, 5, 6 / 4, 6, 7 ]
        for (int tri = 0, vIndex = 0, i = 0; tri < trianglesRequired; tri++, vIndex++)
        {
            indices[i++] = startIndex + 0;          // triangle origin
            indices[i++] = startIndex + vIndex + 1; // next vertex
            indices[i++] = startIndex + vIndex + 2; // next-next vertex
        }
    }

    private void DrawFillRect(float centerX, float centerY, float width, float height, Color color)
    {
        // Vertices, indices for a triangle list

        var trianglesRequired = 2;
        var verticesRequired = 4;
        var indicesRequired = trianglesRequired * 3;

        RequestSpace(verticesRequired, indicesRequired, out var vertices, out var indices, out var startIndex);

        var p0x = centerX - width / 2;
        var p0y = centerY - height / 2;

        var p1x = p0x + width;
        var p1y = p0y + height;

        vertices[0] = new VertexPositionColor(new Vector3(p0x, p0y, 0), color);
        vertices[1] = new VertexPositionColor(new Vector3(p1x, p0y, 0), color);
        vertices[2] = new VertexPositionColor(new Vector3(p1x, p1y, 0), color);
        vertices[3] = new VertexPositionColor(new Vector3(p0x, p1y, 0), color);

        // Create indices for the 2 triangles
        indices[0] = startIndex + 0;
        indices[1] = startIndex + 1;
        indices[2] = startIndex + 2;

        indices[3] = startIndex + 0;
        indices[4] = startIndex + 2;
        indices[5] = startIndex + 3;

    }

    private Vector2[] GenerateRectCoordinates(Vector2 center, float width, float height)
    {
        var hw = width / 2;
        var hh = height / 2;

        var coords = new Vector2[4];

        coords[0] = center + new Vector2(-hw, -hh);
        coords[1] = center + new Vector2(+hw, -hh);
        coords[2] = center + new Vector2(+hw, +hh);
        coords[3] = center + new Vector2(-hw, +hh);

        return coords;
    }

    //==========================================================================

    private void RequestSpace(int requestedVertices, int requestedIndices, out Span<VertexPositionColor> vertices, out Span<int> indices, out int verticesStart)
    {
        verticesStart = _verticesCount;

        vertices = _vertices.AsSpan(_verticesCount, requestedVertices);
        _verticesCount += requestedVertices;

        indices = _indices.AsSpan(_indicesCount, requestedIndices);
        _indicesCount += requestedIndices;
    }

    private void GenerateCircleCoordinates(float centerX, float centerY, float radius, Span<Vector2> vectors)
    {
        GenerateCircleCoordinates(new Vector2(centerX, centerY), radius, vectors);
    }

    private void GenerateCircleCoordinates(Vector2 center, float radius, Span<Vector2> vectors)
    {
        var vectorsCount = vectors.Length;

        for (var v = 0; v < vectorsCount; v++)
        {
            var angle = (float)v / vectorsCount * MathHelper.TwoPi;
            var x = center.X + MathF.Cos(angle) * radius;
            var y = center.Y + MathF.Sin(angle) * radius;

            vectors[v] = new Vector2(x, y);
        }
    }

    //=== SUB CLASSES ==========================================================
    class DrawStates
    {
        public Color Color { get; set; } = Color.White;
        public float Thickness { get; set; } = 4f;
        public bool Filled { get; set; } = false;
        public int Segments { get; set; } = 32; // For circles, number of segments to draw
        public float Angle { get; set; } = -1f; // For circles, angle line to draw, -1 = none

        public void CopyFrom(DrawStates other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            var type = typeof(DrawStates);
            foreach (var prop in type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            {
                if (prop.CanWrite)
                {
                    prop.SetValue(this, prop.GetValue(other));
                }
            }
        }
    }
}