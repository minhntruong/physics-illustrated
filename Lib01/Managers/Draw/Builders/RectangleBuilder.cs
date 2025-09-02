using System;
using Microsoft.Xna.Framework;

namespace ShowPhysics.Library.Managers.Draw.Builders;

public class RectangleBuilder : ShapeBuilder<RectangleBuilder>
{
    public RectangleBuilder(DrawImpl drawImpl) : base(drawImpl)
    {
    }

    private float _width;
    private float _height;

    public RectangleBuilder Width(float width)
    {
        _width = width;
        return this;
    }

    public RectangleBuilder Height(float height)
    {
        _height = height;
        return this;
    }

    public RectangleBuilder Stroke()
    {
        var coords = GetCoordinates();

        _drawImpl.CreateStrokedPolygon(coords, _states.Color, _states.Thickness);
        return this;
    }

    public RectangleBuilder Fill()
    {
        var width = _width * (_sizeAbs ? 1 / Camera.Zoom : 1);
        var height = _height * (_sizeAbs ? 1 / Camera.Zoom : 1);

        _drawImpl.CreateFilledRectangle(_center, width, height, _states.Color);
        return this;
    }

    //==========================================================================
    private Span<Vector2> GetCoordinates()
    {
        var scale = _sizeAbs ? 1/Camera.Zoom : 1;
        var data = _drawImpl.GetCoordinatesStorage(4);
        Coordinates.Rectangle(_center, _width, _height, data, scale);
        return data;
    }
}
