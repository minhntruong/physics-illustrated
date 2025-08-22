using System;
using Microsoft.Xna.Framework;
using ShowPhysics.Library.Managers.Draw;

namespace ShowPhysics.Library.Managers.Text;

public partial class TextImpl
{
    public class TextBuilder : BuilderBase<TextBuilder>
    {
        public TextBuilder(TextImpl textImpl, bool useTransforms = true) : base(textImpl)
        {
            _useTransforms = useTransforms;
        }

        private bool _useTransforms = true;
        private Vector2 _position = Vector2.Zero;

        public TextBuilder Position(Vector2 value)
        {
            _position = value;
            return this;
        }

        public TextBuilder Text(object value)
        {
            if (value == null) { return this; }

            var pos = _useTransforms ? Coordinates.Transform(_position) : _position; 

            _textImpl.CreateText(value, pos, _states.Color, _states.Anchor, _states.Scale, _states.Rotation);

            return this;
        }
    }
}