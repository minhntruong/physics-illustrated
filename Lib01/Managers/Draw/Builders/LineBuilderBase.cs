using Microsoft.Xna.Framework;
using System;

namespace ShowPhysics.Library.Managers.Draw.Builders;

public abstract class LineBuilderBase<TBuilder> : BuilderBase<TBuilder> where TBuilder : LineBuilderBase<TBuilder>
{
    protected LineBuilderBase(DrawImpl drawImpl) : base(drawImpl)
    {
    }

    protected Vector2 _start;
    protected Vector2 _end;

    public TBuilder Start(Vector2 value)
    {
        _start = value;
        return (TBuilder)this;
    }

    public TBuilder Start(float x, float y)
    {
        _start = new Vector2(x, y);
        return (TBuilder)this;
    }

    public TBuilder End(Vector2 value)
    {
        _end = value;
        return (TBuilder)this;
    }

    public TBuilder End(float x, float y)
    {
        _end = new Vector2(x, y);
        return (TBuilder)this;
    }
}
