using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShowPhysics.Library.Managers;
using static ShowPhysics.Library.Managers.GameExt;

namespace Lib01_Tests;

public class Game1 : Game
{
    public Game1()
    {
        GameExt.Initialize(this);

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        Configure(1600, 900);

        Graphics.Initialize(this, "Lib_Shared_Arial_24");

        // Do this last
        Camera.SetViewport(1600, 900); // TODO: make dynamic on resize

        base.Initialize();
    }

    protected override void LoadContent()
    {

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        Input.Update();

        if (Input.IsKeyClicked(Keys.Z))
        {
            Camera.Zoom += 0.1f;
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        Graphics.Mid.Rect().Center(500, 500).Width(300).Height(200).Color(Color.White).Thickness(4).Stroke();

        Graphics.Draw();

        base.Draw(gameTime);
    }
}
