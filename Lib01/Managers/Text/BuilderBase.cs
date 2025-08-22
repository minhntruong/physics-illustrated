using System;
using Microsoft.Xna.Framework;

namespace ShowPhysics.Library.Managers.Text;

public partial class TextImpl
{
    public abstract class BuilderBase<TBuilder> where TBuilder : BuilderBase<TBuilder>
    {
        public BuilderBase(TextImpl textImpl)
        {
            _textImpl = textImpl;
            _states = textImpl.DefaultStates;
        }

        protected TextImpl _textImpl;
        protected TextStates _states;

        public TBuilder Color(Color value)
        {
            _states.Color = value;
            return (TBuilder)this;
        }

        public TBuilder Anchor(TextAnchor value)
        {
            _states.Anchor = value;
            return (TBuilder)this;
        }

        public TBuilder Scale(float value)
        {
            _states.Scale = value;
            return (TBuilder)this;
        }

        public TBuilder Rotation(float value)
        {
            _states.Rotation = value;
            return (TBuilder)this;
        }

        public void Default()
        {
            _textImpl.DefaultStates = _states;
        }
    }
}