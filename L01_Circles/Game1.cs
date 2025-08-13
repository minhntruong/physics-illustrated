using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PhysicsIllustrated.Library.Managers;
using PhysicsIllustrated.Library.Physics;
using System.Xml;
using static PhysicsIllustrated.Library.Managers.GameExt;
using static PhysicsIllustrated.Library.Physics.Constants;

namespace L01_Circles
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

        private World _world;

        protected override void Initialize()
        {
            Configure(1600, 900);

            Graphics.Initialize(this, "Proj_Arial_12");

            _world = new World(9.8f);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Graphics.OnLoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (Input.IsDefaultExitInput()) { Exit(); }

            Input.Update();

            StatesOnInputs();
            var dt = ProcessGameTime(gameTime);

            _world.Update(dt);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.CornflowerBlue);
            States.Clear(GraphicsDevice);

            Graphics.DrawString("Hello, MonoGame", new Vector2(20, 20));
            Graphics.Draw();

            base.Draw(gameTime);
        }

        private void StatesOnInputs()
        {
            ToggleOnKeyClicked(Keys.P, ref States.IsPaused);
            SetByKeyClicked(Keys.S, ref States.IsStepRequested);
        }

        private float ProcessGameTime(GameTime gameTime)
        {
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (dt > SECS_PER_FRAME)
            {
                dt = SECS_PER_FRAME;
            }

            if (States.IsPaused && States.IsStepRequested == false)
            {
                dt = 0.0f;
            }

            if (States.IsPaused && States.IsStepRequested)
            {
                dt = SECS_PER_FRAME;
            }

            return dt;
        }
    }
}
