using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShowPhysics.Library.Managers.Text;

public partial class TextImpl
{
    public TextImpl(SpriteBatch spriteBatch, SpriteFont font, bool useCamera = true)
    {
        _spriteBatch = spriteBatch;
        _font = font;
        _useCamera = useCamera;
        
        if (_useCamera)
        {
            Camera.OnChanged += OnCameraChanged;
        }

        _cursor = new Cursor(_font);
    }

    private bool _useCamera;
    private Matrix _view = Matrix.Identity;

    private void OnCameraChanged(object sender, CameraChangedEventArgs e)
    {
        _view = e.View;
    }

    private bool _begun = false;
    private SpriteBatch _spriteBatch;
    private SpriteFont _font;
    private Cursor _cursor;

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
            _spriteBatch.Begin(transformMatrix: _view);
            _begun = true;
        }

        var textStr = text.ToString();

        var origin = Vector2.Zero;

        var textBounds = _font.MeasureString(textStr);
        // TODO: use textBounds to advance cursor somehow

        if (anchor == TextAnchor.Center)
        {
            origin = textBounds * 0.5f;
        }

        if (_useCamera)
        {
            // This is so that the text size remains constant regardless of camera zoom level
            scale /= Camera.Zoom;
        }

        _spriteBatch.DrawString(
            _font,
            textStr,
            position,
            color,
            rotation,
            origin,
            scale,
            SpriteEffects.None,
            0f);
    }
}
