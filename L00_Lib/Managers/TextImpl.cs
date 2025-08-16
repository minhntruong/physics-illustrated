using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using F = Microsoft.Xna.Framework;

namespace PhysicsIllustrated.Library.Managers;

public enum TextAnchor { TopLeft, Center };

public class TextImpl
{

    public TextImpl(SpriteBatch spriteBatch, SpriteFont font)
    {
        _spriteBatch = spriteBatch;
        _font = font;
    }

    private TextAnchor _anchor = TextAnchor.TopLeft;
    private Vector2 _position = Vector2.Zero;
    private Color _color = F.Color.White;
    private float _scale = 1.0f;
    private float _rotation = 0.0f;

    private bool _begun = false;
    private SpriteBatch _spriteBatch;
    private SpriteFont _font;

    public TextImpl Anchor(TextAnchor value)
    {
        _anchor = value;
        return this;
    }

    public TextImpl Position(Vector2 value)
    {
        _position = value;
        return this;
    }

    public TextImpl Position(float x, float y)
    {
        _position = new Vector2(x, y);
        return this;
    }

    public TextImpl Color(Color value)
    {
        _color = value;
        return this;
    }

    public TextImpl Scale(float value)
    {
        _scale = value;
        return this;
    }

    public TextImpl Rotation(float value)
    {
        _rotation = value;
        return this;
    }

    public void Text(object text)
    {
        if (!_begun)
        {
            _spriteBatch.Begin();
            _begun = true;
        }

        var position = _position;

        var textStr = text.ToString();

        if (_anchor == TextAnchor.Center)
        {
            var size = _font.MeasureString(textStr);
            position -= size * 0.5f * _scale;
        }

        if (_scale == 1.0f && _rotation == 0.0f)
        {
            _spriteBatch.DrawString(_font, textStr, position, _color);
            return;
        }
        else
        {
            _spriteBatch.DrawString(_font, textStr, position, _color, _rotation, Vector2.Zero, _scale, SpriteEffects.None, 0f);
        }
    }

    public void Reset()
    {
        _anchor = TextAnchor.TopLeft;
        _color = F.Color.White;
        _scale = 1.0f;
        _rotation = 0.0f;
    }

    public void Draw()
    {
        if (_begun)
        {
            _spriteBatch.End();
            Reset();
            _begun = false;
        }
    }
}
