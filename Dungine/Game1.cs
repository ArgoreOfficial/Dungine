using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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
            { 1,1,1,0,0,0,1,1 },
            { 1,1,1,0,0,0,1,1 },
            { 1,1,1,0,1,1,1,1 },
            { 1,1,1,1,1,1,1,1 }
        };
        Point playerPos = new Point(3, 6);

        //████████
        //████████
        //█   ████
        //███ ████
        //███   ██
        //███   ██
        //███ ████
        //████████

        //████████
        //████████
        //█▒▒▒████
        //███▒████
        //███▒▒▒██
        //███▒▒▒██
        //███▒████
        //████████

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            List<Point> check = new List<Point>();
            check.Add(playerPos);

            for (int i = 0; i < check.Count; i++)
            {
                map[check[i].Y, check[i].X] = 2;

                if (check[i].Y - 1 <= 0) continue;

                if (map[check[i].Y - 1, check[i].X] == 0)
                {
                    check.Add(new Point(check[i].X, check[i].Y - 1)); // top

                    // only happens if top one is visible
                    if (map[check[i].Y - 1, check[i].X + 1] == 0) // right
                        check.Add(new Point(check[i].X + 1, check[i].Y - 1));
                    if (map[check[i].Y - 1, check[i].X - 1] == 0) // left
                        check.Add(new Point(check[i].X - 1, check[i].Y - 1));
                }
            }

            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    char c = ' ';
                    if (map[y, x] == 1) c = '█';
                    if (map[y, x] == 2) c = '▒';
                    Debug.Write(c);
                }
                Debug.Write(Environment.NewLine);
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