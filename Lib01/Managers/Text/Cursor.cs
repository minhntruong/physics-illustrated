using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ShowPhysics.Library.Managers.Text;

public class Cursor
{
    public Cursor(SpriteFont font)
    {
        _lineSpacing = font.LineSpacing;
    }

    private int _lineSpacing = 0;
    private Vector2 _position = Vector2.Zero;
    private Vector2 _range = Vector2.Zero;


}
