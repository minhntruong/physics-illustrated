using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShowPhysics.Library;
using ShowPhysics.Library.Managers;
using ShowPhysics.Library.Shows;
using static ShowPhysics.Library.Managers.GameExt;


namespace Lib01_Circles;

public class Game1 : Game, IGameExt
{
    public Game1()
    {
        Construct(this);

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    //==========================================================================

    private ShowCircles _show;

    //==========================================================================

    public event EventHandler<EventArgs> WindowClientSizeChanged;

    public void RaiseWindowClientSizeChanged()
    {
        WindowClientSizeChanged?.Invoke(this, EventArgs.Empty);
    }

    //==========================================================================

    protected override void Initialize()
    {
        GameExt.Initialize(1600, 900, () =>
        {
            Graphics.Initialize(this, "Lib01_Shared_Arial_24");
            Camera.Initialize(this);
            Input.Initialize(this);
        });

        _show = new ShowCircles(GraphicsDevice);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        Graphics.OnLoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        _show.PreUpdate();

        _show.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _show.PreDraw();

        _show.Draw();

        Graphics.Draw();

        base.Draw(gameTime);
    }
}
