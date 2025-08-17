using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PhysicsIllustrated.Library.Illustrators;
using PhysicsIllustrated.Library.Managers;
using PhysicsIllustrated.Library.Physics;
using PhysicsIllustrated.Library.Physics.Shapes;
using static PhysicsIllustrated.Library.Managers.GameExt;

namespace L01_Circles;

public class Game1 : Game
{
    public Game1()
    {
        // Creation of GraphicsDeviceManager must be done in the constructor
        GameExt.Initialize(this);

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    private Body _movable;
    private CirclesIllustrator _illustrator;

    protected override void Initialize()
    {
        Configure(1600, 900);

        Graphics.Initialize(this, "Lib_Shared_Arial_24");

        _illustrator = new CirclesIllustrator(GraphicsDevice);

        var circle = new Body(new CircleShape(150), Width() / 2, Height() / 2, 1.0f);
        _movable = new Body(new CircleShape(100), Width() - 100 - 10, 100 + 10, 1.0f);

        _illustrator.Bodies.Add(circle);
        _illustrator.Bodies.Add(_movable);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        Graphics.OnLoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        //=== UI logic =========================================================

        Input.Update();

        StatesOnInputs();

        if (States.IsMouseEngaged)
        {
            _movable.Position = Input.MousePosition();
        }

        IsMouseVisible = !States.IsMouseEngaged;

        //=== Illusatrator logic ===============================================

        _illustrator.Update(gameTime);

        //=== Base logic =======================================================

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        Graphics.Text.Position(20, 20).Scale(0.75f).Text(MenuText());

        _illustrator.ShowRadii(States.ShowRadii);

        _illustrator.PreDraw();
        _illustrator.Draw();

        Graphics.Draw();

        base.Draw(gameTime);
    }

    private string MenuText()
    {
        return @"
Mouse press to move
'R' to toggle showing radii
";
    }

    private void StatesOnInputs()
    {
        SetByMouseLeftButtonPressed(ref States.IsMouseEngaged);
        ToggleOnKeyClicked(Keys.R, ref States.ShowRadii);
    }
}
