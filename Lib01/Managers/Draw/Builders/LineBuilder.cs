using System;

namespace ShowPhysics.Library.Managers.Draw.Builders;

public class LineBuilder : LineBuilderBase<LineBuilder>
{
    public LineBuilder(DrawImpl drawImpl) : base(drawImpl)
    {
    }

    public LineBuilder Stroke()
    {
        _drawImpl.CreateLine(_start, _end, _states.Color, _states.Thickness);
        return this;
    }
}
