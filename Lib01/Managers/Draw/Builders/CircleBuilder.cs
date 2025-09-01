using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace ShowPhysics.Library.Managers.Draw.Builders;

public class CircleBuilder : ShapeBuilder<CircleBuilder>
{
    public CircleBuilder(DrawImpl drawImpl) : base(drawImpl)
    {
    }

    private float _radius;
    private int _segments = 32; // Default number of segments for the circle
    private bool _autoSegments = true;
    private float? _rotation = null; 

    public CircleBuilder Radius(float value)
    {
        _radius = value;
        return this;
    }

    public CircleBuilder Segments(int value)
    {
        _segments = value;
        _autoSegments = false;
        return this;
    }

    public CircleBuilder AutoSegments(bool value = true)
    {
        _autoSegments = value;
        return this;
    }

    public CircleBuilder Rotation(float value)
    {
        _rotation = value;
        return this;
    }

    private Span<Vector2> GetCoordinates()
    {
        var segments = _segments;

        if (_autoSegments)
        {
            // Adjust segments based on radius for smoother appearance
            const int minSegments = 20;
            const int maxSegments = 192;
            const float tuning = 0.4f; // Adjust this value to change sensitivity

            float zoom = Camera.Zoom > 0 ? Camera.Zoom : 1.0f;
            int dynamicSegments = (int)(tuning * _radius * zoom);
            dynamicSegments = Math.Clamp(dynamicSegments, minSegments, maxSegments);

            segments = dynamicSegments;
        }

        //System.Diagnostics.Debug.WriteLine($"Circle radius: {_radius}, segments: {segments}, zoom: {Camera.Zoom}");

        var data = _drawImpl.GetCoordinatesStorage(segments);
        Coordinates.Circle(_center, _radius, segments, data);

        return data;
    }

    public CircleBuilder Stroke()
    {
        var coords = GetCoordinates();

        // Draw line segments between the generated points
        for (var i = 0; i < coords.Length - 1; i++)
        {
            var start = coords[i];
            var end = coords[i + 1];

            _drawImpl.CreateLine(start, end, _states.Color, _states.Thickness);
        }

        // Connect the last point to the first to close the circle
        _drawImpl.CreateLine(coords[coords.Length - 1], coords[0], _states.Color, _states.Thickness);

        // Draw the angle line
        if (_rotation.HasValue)
        {
            _drawImpl.CreateLine(
                _center,
                new Vector2(_center.X + MathF.Cos(_rotation.Value) * _radius,
                            _center.Y + MathF.Sin(_rotation.Value) * _radius),
                _states.Color,
                _states.Thickness);
        }

        return this;
    }

    public CircleBuilder Fill()
    {
        var coords = GetCoordinates();

        throw new NotImplementedException();
    }
}
