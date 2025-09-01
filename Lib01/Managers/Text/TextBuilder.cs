using System;
using Microsoft.Xna.Framework;
using ShowPhysics.Library.Managers.Draw;

namespace ShowPhysics.Library.Managers.Text;

public partial class TextImpl
{
    public class TextBuilder : BuilderBase<TextBuilder>
    {
        public TextBuilder(TextImpl textImpl) : base(textImpl)
        {
        }

        private Vector2 _position = Vector2.Zero;

        public TextBuilder Position(Vector2 value)
        {
            _position = value;
            return this;
        }

        public TextBuilder Position(float x, float y)
        {
            _position = new Vector2(x, y);
            return this;
        }

        public TextBuilder Text(object value)
        {
            if (value == null) { return this; }

            _textImpl.CreateText(
                value, 
                _position, 
                _states.Color, 
                _states.Anchor, 
                _states.Scale, 
                _states.Rotation);

            return this;
        }
    }
}