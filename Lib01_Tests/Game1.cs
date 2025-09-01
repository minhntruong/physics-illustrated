using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShowPhysics.Library;
using ShowPhysics.Library.Managers;
using static ShowPhysics.Library.Managers.GameExt;

namespace Lib01_Tests;

public class Game1 : Game, IGameExt
{
    public Game1()
    {
        Construct(this);

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    public event EventHandler<EventArgs> WindowClientSizeChanged;

    public void RaiseWindowClientSizeChanged()
    {
        WindowClientSizeChanged?.Invoke(this, EventArgs.Empty);
    }

    private Vector2 _initOrigin = Vector2.Zero;

    protected override void Initialize()
    {
        GameExt.Initialize(1600, 900, () =>
        {
            Graphics.Initialize(this, "Lib01_Shared_Arial_24");
            Camera.Initialize(this);
            Input.Initialize(this);

            Camera.Origin = _initOrigin;
        });

        base.Initialize();
    }

    protected override void LoadContent()
    {

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        Input.Update();

        Input.CheckMousePanCamera();
        Input.CheckMouseZoomCamera();

        if (Input.IsKeyClicked(Keys.R))
        {
            Camera.Zoom = 1f;
            Camera.Origin = _initOrigin;
        }

        if (Input.IsKeyClicked(Keys.Z))
        {
            Camera.SetZoomFocus(0f, new Vector2(500, 500));
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        Graphics.Mid.States().Color(Color.White).ThicknessAbs(4).Default();
        Graphics.Mid.Rect().Center(500, 500).Width(300).Height(200).Stroke();

        Graphics.Mid.States().Color(Color.Yellow).Thickness(1).Default();
        Graphics.Mid.Line().Start(490, 500).End(510, 500).Stroke();
        Graphics.Mid.Line().Start(500, 490).End(500, 510).Stroke();

        Graphics.Mid.States().Color(Color.White).ThicknessAbs(4).Default();
        Graphics.Mid.Circle().Center(600, 200).Radius(100).Stroke();

        Graphics.Draw();

        base.Draw(gameTime);
    }

}
