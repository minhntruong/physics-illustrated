using Microsoft.Xna.Framework;

namespace ShowPhysics.Library.Managers.Draw.Builders;

public abstract class BuilderBase<TBuilder> where TBuilder : BuilderBase<TBuilder>
{
    public BuilderBase(DrawImpl drawImpl)
    {
        _drawImpl = drawImpl;
        _states = drawImpl.DefaultStates;
    }

    protected DrawImpl _drawImpl;
    protected Vector2 _center = Vector2.Zero;
    protected DrawStates _states;

    public TBuilder Color(Color value)
    {
        _states.Color = value;
        return (TBuilder)this;
    }

    public TBuilder Thickness(float value)
    {
        _states.Thickness = value;
        _states.ThicknessAbs = false;
        return (TBuilder)this;
    }

    public TBuilder ThicknessAbs(bool value = true)
    {
        _states.ThicknessAbs = value;
        return (TBuilder)this;
    }

    public TBuilder ThicknessAbs(float value)
    {
        _states.Thickness = value;
        _states.ThicknessAbs = true;
        return (TBuilder)this;
    }

    public void Default()
    {
        _drawImpl.DefaultStates = _states;
    }
}