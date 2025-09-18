using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ShowPhysics.Library;
using ShowPhysics.Library.Managers;
using ShowPhysics.Library.Shows;
using static ShowPhysics.Library.Managers.GameExt;

namespace Lib01_Shows_Host;

public class Game1 : Game, IGameExt
{
    public Game1()
    {
        Construct(this);

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    //==========================================================================

    private List<ShowBase> _shows = new();
    private ShowBase _show;

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

        Register(new ShowCircles(GraphicsDevice));
        //Register(new ShowCirclePolygon(GraphicsDevice));

        base.Initialize();
    }

    protected override void LoadContent()
    {
        Graphics.OnLoadContent();

        foreach (var show in _shows)
        {
            show.LoadContent();
        }
    }

    protected override void Update(GameTime gameTime)
    {
        if (Input.IsKeyClicked(Keys.OemPlus) && (Input.IsKeyDown(Keys.LeftShift) || Input.IsKeyDown(Keys.RightShift)))
        {
            var i = _shows.FindIndex(s => s == _show);
            i = (i + 1) % _shows.Count;
            _show = _shows[i];
        }

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

    //==========================================================================

    private void Register(ShowBase show)
    {
        _shows.Add(show);
        _show = show;
    }
}
