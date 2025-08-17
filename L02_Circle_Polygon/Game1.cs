using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhysicsIllustrated.Library.Illustrators;
using PhysicsIllustrated.Library.Managers;
using PhysicsIllustrated.Library.Physics;
using PhysicsIllustrated.Library.Physics.Shapes;
using static PhysicsIllustrated.Library.Managers.GameExt;

namespace L02_Circle_Polygon
{
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
        private CirclePolyIllustrator _illustrator;

        protected override void Initialize()
        {
            Configure(1600, 900);

            Graphics.Initialize(this, "Lib_Shared_Arial_24");

            _illustrator = new CirclePolyIllustrator(GraphicsDevice);
            
            var circle = new Body(new CircleShape(150), Width() / 2, Height() / 2, 1.0f);
            _movable = new Body(new BoxShape(150, 150), Width() * 0.8f, 100, 1.0f);

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

            var isMouseEngaged = Input.MouseLeftButtonPressed();
            if (isMouseEngaged)
            {
                _movable.Position = Input.MousePosition();
            }

            IsMouseVisible = !isMouseEngaged;

            //=== Illusatrator logic ===============================================

            _illustrator.Update(gameTime);

            //=== Base logic =======================================================

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Graphics.Text.Position(20, 20).Scale(0.75f).Text(MenuText());

            _illustrator.PreDraw();
            _illustrator.Draw();

            Graphics.Draw();

            base.Draw(gameTime);
        }

        //======================================================================

        private string MenuText()
        {
            return @"
Mouse press to move
";
        }
    }
}
