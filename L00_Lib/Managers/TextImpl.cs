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

    //==========================================================================

    private TextStates _defaultStates = new TextStates();
    private TextStates _currentStates = new TextStates();

    //private TextAnchor _anchor = TextAnchor.TopLeft;
    private Vector2 _position = Vector2.Zero;
    //private Color _color = F.Color.White;
    //private float _scale = 1.0f;
    //private float _rotation = 0.0f;


    private bool _begun = false;
    private SpriteBatch _spriteBatch;
    private SpriteFont _font;

    //==========================================================================
    public float Zoom { get; set; } = 1.0f;

    public Vector2 Origin { get; set; } = Vector2.Zero;

    public TextImpl Anchor(TextAnchor value)
    {
        _currentStates.Anchor = value;
        return this;
    }

    public TextImpl Position(Vector2 value)
    {
        // Graphics.DrawVertex(value); // Draw here to not use TextImpl Scale & Origin
        _position = value * Zoom - Origin;
        return this;
    }

    public TextImpl Position(float x, float y)
    {
        return Position(new Vector2(x, y));
    }

    public TextImpl Color(Color value)
    {
        _currentStates.Color = value;
        return this;
    }

    public TextImpl Scale(float value)
    {
        _currentStates.Scale = value;
        return this;
    }

    public TextImpl Rotation(float value)
    {
        _currentStates.Rotation = value;
        return this;
    }

    public TextImpl RotationOf(Vector2 fromPt, Vector2 toPt)
    {
        // Calculate the angle of the 2 points

        var vector = toPt - fromPt;

        float angle = MathF.Atan2(vector.Y, vector.X);
        _currentStates.Rotation = angle;

        return this;
    }

    public void Default()
    {
        _defaultStates.CopyFrom(_currentStates);
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

        var textSize = Vector2.Zero;

        if (_currentStates.Anchor == TextAnchor.Center)
        {
            textSize = _font.MeasureString(textStr);
            //position -= textSize * 0.5f * _currentStates.Scale;
        }

        if (_currentStates.Anchor == TextAnchor.Center)
        {
            // Always use rotational draw for center anchor because monogame provides
            // us with the centering functionality
            _spriteBatch.DrawString(
                _font,
                textStr,
                position,
                _currentStates.Color,
                _currentStates.Rotation,
                textSize * 0.5f,
                _currentStates.Scale,
                SpriteEffects.None,
                0f);
        }
        else if (_currentStates.Scale == 1.0f && _currentStates.Rotation == 0.0f)
        {
            // Presumably LeftTop anchor
            _spriteBatch.DrawString(_font, textStr, position, _currentStates.Color);
        }
        else
        {
            _spriteBatch.DrawString(
                _font,
                textStr,
                position,
                _currentStates.Color,
                _currentStates.Rotation,
                Vector2.Zero,               // Rotation around top-left corner -- weird but ok
                _currentStates.Scale,
                SpriteEffects.None,
                0f);
        }
    }

    public void Reset()
    {
        ResetStates();
        //_anchor = TextAnchor.TopLeft;
        //_color = F.Color.White;
        //_scale = 1.0f;
        //_rotation = 0.0f;
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

    //==========================================================================

    private void ResetStates()
    {
        _currentStates.Color = _defaultStates.Color;
        _currentStates.Anchor = _defaultStates.Anchor;
        _currentStates.Scale = _defaultStates.Scale;
        _currentStates.Rotation = _defaultStates.Rotation;
    }

    //=== SUB CLASSES ==========================================================
    class TextStates
    {
        public TextAnchor Anchor { get; set; } = TextAnchor.TopLeft;
        public Color Color { get; set; } = Color.White;
        public float Scale { get; set; } = 1;
        public float Rotation { get; set; } = 0;

        public void CopyFrom(TextStates other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            var type = typeof(TextStates);
            foreach (var prop in type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            {
                if (prop.CanWrite)
                {
                    prop.SetValue(this, prop.GetValue(other));
                }
            }
        }
    }
}
