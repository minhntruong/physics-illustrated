using Microsoft.Xna.Framework;

namespace ShowPhysics.Library.Managers.Draw.Builders;

public abstract class ShapeBuilder<TBuilder> : BuilderBase<TBuilder> where TBuilder : ShapeBuilder<TBuilder>
{
    protected ShapeBuilder(DrawImpl drawImpl) : base(drawImpl)
    {
    }

    public new TBuilder Center(Vector2 value)
    {
        _center = value;
        return (TBuilder)this;
    }
}