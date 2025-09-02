using Microsoft.Xna.Framework;

namespace ShowPhysics.Library.Managers.Draw.Builders;

public abstract class ShapeBuilder<TBuilder> : BuilderBase<TBuilder> where TBuilder : ShapeBuilder<TBuilder>
{
    protected ShapeBuilder(DrawImpl drawImpl) : base(drawImpl)
    {
    }

    protected bool _sizeAbs = false;

    public TBuilder Center(Vector2 value)
    {
        _center = value;
        return (TBuilder)this;
    }

    public TBuilder Center(float x, float y)
    {
        _center = new Vector2(x, y);
        return (TBuilder)this;
    }

    public TBuilder SizeAbs(bool value = true)
    {
        _sizeAbs = value;
        return (TBuilder)this;
    }
}