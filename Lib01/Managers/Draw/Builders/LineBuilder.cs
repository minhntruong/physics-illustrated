using System;
using Microsoft.Xna.Framework;

namespace ShowPhysics.Library.Managers.Draw.Builders;

public class LineBuilder : BuilderBase<LineBuilder>
{
    public LineBuilder(DrawImpl drawImpl) : base(drawImpl)
    {
    }

    private Vector2 _start;
    private Vector2 _end;

    public LineBuilder Start(Vector2 value)
    {
        _start = value;
        return this;
    }

    public LineBuilder End(Vector2 value)
    {
        _end = value;
        return this;
    }

    public LineBuilder Stroke()
    {
        _drawImpl.CreateLine(_start, _end, _states.Color, _states.Thickness);
        return this;
    }
}
