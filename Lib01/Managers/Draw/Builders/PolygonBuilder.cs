using Microsoft.Xna.Framework;
using System;

namespace ShowPhysics.Library.Managers.Draw.Builders;

public class PolygonBuilder : ShapeBuilder<PolygonBuilder>
{
    public PolygonBuilder(DrawImpl drawImpl) : base(drawImpl)
    {
    }

    private Vector2[] _coords;

    public PolygonBuilder Coordinates(params Vector2[] coords)
    {
        _coords = coords ?? throw new ArgumentNullException(nameof(coords));
        return this;
    }

    public PolygonBuilder Stroke()
    {
        var coords = _coords.AsSpan();

        _drawImpl.CreateStrokedPolygon(coords, _states.Color, _states.Thickness);
        return this;
    }

    public PolygonBuilder Fill()
    {
        throw new NotImplementedException();
    }
}