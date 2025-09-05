using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShowPhysics.Library.Managers.Draw.Builders;
using System;

namespace ShowPhysics.Library.Managers.Draw;

public class DrawImpl
{
    public DrawImpl(GraphicsDevice graphicsDevice, int capacity = 10_000)
    {
        _graphics = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));

        // Initialize BasicEffect
        _effect = new BasicEffect(graphicsDevice);
        _effect.VertexColorEnabled = true;
        _effect.LightingEnabled = false;

        // Initialize graphics device-side resources
        _vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor), capacity, BufferUsage.WriteOnly);
        _indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, capacity * 3, BufferUsage.WriteOnly);

        // Initialize main memory-side resources
        _vertices = new VertexPositionColor[capacity];
        _indices = new int[capacity * 3];

        Camera.OnChanged += OnCameraChanged;
    }


    private GraphicsDevice _graphics;
    private BasicEffect _effect;
    private bool _isDisposed = false;

    // Graphics device-side buffers
    private VertexBuffer _vertexBuffer;
    private IndexBuffer _indexBuffer;

    // Main memory-side buffers
    private VertexPositionColor[] _vertices;
    private int[] _indices;

    private int _verticesCount = 0;
    private int _indicesCount = 0;

    private Vector2[] _coordinatesStorage = new Vector2[256];

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

    public DrawStates DefaultStates { get; set; }

    //==========================================================================

    private void OnCameraChanged(object sender, CameraChangedEventArgs e)
    {
        _view = e.View;
        _projection = e.Projection;
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

    public void Reset()
    {
        _verticesCount = 0;
        _indicesCount = 0;
    }

    //==========================================================================

    public void CreateStrokedPolygon(Span<Vector2> coords, Color color, float thickness = 4)
    {
        for (int i = 0; i < coords.Length - 1; i++)
        {
            var start = coords[i];
            var end = coords[i + 1];
            CreateLine(start, end, color, thickness);
        }

        // Connect the last vertex to the first
        CreateLine(coords[^1], coords[0], color, thickness);
    }

    public void CreateFilledRectangle(Vector2 center, float width, float height, Color color)
    {
        // Vertices, indices for a triangle list

        var trianglesRequired = 2;
        var verticesRequired = 4;
        var indicesRequired = trianglesRequired * 3;

        RequestSpace(verticesRequired, indicesRequired, out var vertices, out var indices, out var startIndex);

        var p0x = center.X - width / 2;
        var p0y = center.Y - height / 2;

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

    public void CreateLine(Vector2 start, Vector2 end, Color color, float thickness = 4)
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

    private void RequestSpace(int requestedVertices, int requestedIndices, out Span<VertexPositionColor> vertices, out Span<int> indices, out int verticesStart)
    {
        verticesStart = _verticesCount;

        vertices = _vertices.AsSpan(_verticesCount, requestedVertices);
        _verticesCount += requestedVertices;

        indices = _indices.AsSpan(_indicesCount, requestedIndices);
        _indicesCount += requestedIndices;
    }

    //==========================================================================

    public PolygonBuilder Poly()
    {
        return new PolygonBuilder(this);
    }

    public RectangleBuilder Rect()
    {
        return new RectangleBuilder(this);
    }

    public CircleBuilder Circle()
    {
        return new CircleBuilder(this);
    }

    public LineBuilder Line()
    {
        return new LineBuilder(this);
    }

    public VectorBuilder Vector()
    {
        return new VectorBuilder(this);
    }

    public StatesBuilder States()
    {
        return new StatesBuilder(this);
    }

    public Span<Vector2> GetCoordinatesStorage(int count)
    {
        if (count > _coordinatesStorage.Length)
            throw new ArgumentOutOfRangeException(nameof(count), "Requested count exceeds storage capacity.");
        return _coordinatesStorage.AsSpan(0, count);
    }

    //==========================================================================

}
