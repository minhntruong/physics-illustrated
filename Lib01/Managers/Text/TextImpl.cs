using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShowPhysics.Library.Managers.Text;

public partial class TextImpl
{
    public TextImpl(SpriteBatch spriteBatch, SpriteFont font)
    {
        _spriteBatch = spriteBatch;
        _font = font;
    }

    private bool _begun = false;
    private SpriteBatch _spriteBatch;
    private SpriteFont _font;

    //==========================================================================

    public TextStates DefaultStates { get; set; }

    //==========================================================================
    public void Draw()
    {
        if (_begun)
        {
            _spriteBatch.End();
            _begun = false;
        }
    }

    //==========================================================================
    private void CreateText(object text, Vector2 position, Color color, TextAnchor anchor = TextAnchor.TopLeft, float scale = 1, float rotation = 0)
    {
        if (!_begun)
        {
            _spriteBatch.Begin();
            _begun = true;
        }

        var textStr = text.ToString();

        var textBounds = Vector2.Zero;

        if (anchor == TextAnchor.Center)
        {
            textBounds = _font.MeasureString(textStr);
        }

        if (scale == 1.0f && rotation == 0.0f)
        {
            _spriteBatch.DrawString(_font, textStr, position, color);
            return;
        }
        else
        {
            _spriteBatch.DrawString(
                _font,
                textStr,
                position,
                color,
                rotation,
                textBounds * 0.5f,
                scale,
                SpriteEffects.None,
                0f);
        }
    }


}
