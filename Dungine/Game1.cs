using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Dungine
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        int[,] map = {
            { 1,1,1,1,1,1,1,1 },
            { 1,1,1,1,1,1,1,1 },
            { 1,0,0,0,1,1,1,1 },
            { 1,1,1,0,1,1,1,1 },
            { 1,1,1,0,1,1,1,1 },
            { 1,1,1,0,0,0,1,1 },
            { 1,1,1,0,1,1,1,1 },
            { 1,1,1,1,1,1,1,1 }
        };
        Point playerPos = new Point(3, 6);



        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            for (int i = playerPos.Y; i >= 0; i--)
            {

            }


            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}