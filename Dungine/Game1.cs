using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;

namespace Dungine
{

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Point PrevMousePosition;

        List<Shape> shapes = new List<Shape>();

        Vector2 PlayerPosition = new Vector2(83, 53);
        float PlayerRotation = 0;

        Texture2D WallTexture;
        Texture2D WindowTexture;

        float MovementSpeed = 50f;
        
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 512;
            _graphics.PreferredBackBufferHeight = 512;
            _graphics.ApplyChanges();

            Point mousePos = new Point(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);

            Mouse.SetPosition(mousePos.X, mousePos.Y);
            PrevMousePosition = mousePos;
            IsMouseVisible = false;

            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Renderer.MissingTexture = Content.Load<Texture2D>("Textures/MissingTexture");


            WallTexture = Content.Load<Texture2D>("Textures/StoneWall1");
            WindowTexture = Content.Load<Texture2D>("Textures/Testing/window");



            // map has to be created after loading textures
            shapes.Add(new SDFCircle(new Vector2(150, 128), 0f, 15f));
            shapes.Last().SetTexture(WallTexture, new Vector2(), new Vector2(0.5f, 2));
            shapes.Last().CSGType = ShapeCSG.Difference;
            
            shapes.Add(new SDFRectangle(new Vector2(128, 128), 0f, new Vector2(32, 32)));
            shapes.Last().SetTexture(WallTexture, new Vector2(), new Vector2(0.5f, 2));

            shapes.Add(new SDFRectangle(new Vector2(128, 160), 0f, new Vector2(32, 32)));
            shapes.Last().SetTexture(WindowTexture, new Vector2(), new Vector2(0.5f, 2));


            Debug.WriteLine(Renderer.GetClosest(new Vector2(165, 128), shapes).Distance);
        }

        protected void Input(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();
            KeyboardState key = Keyboard.GetState();
            if (key.IsKeyDown(Keys.Escape))
                Exit();

            Vector2 playerDir = new Vector2(
                MathF.Cos(PlayerRotation),
                MathF.Sin(PlayerRotation));

            if (key.IsKeyDown(Keys.W))
            {
                PlayerPosition += playerDir * MovementSpeed  * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (key.IsKeyDown(Keys.S))
            {
                PlayerPosition -= playerDir * MovementSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (key.IsKeyDown(Keys.A))
            {
                PlayerPosition -= new Vector2(-playerDir.Y, playerDir.X) * MovementSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (key.IsKeyDown(Keys.D))
            {
                PlayerPosition += new Vector2(-playerDir.Y, playerDir.X) * MovementSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (key.IsKeyUp(Keys.LeftShift))
            {
                float mouseDeltaX = (mouse.Position.X - PrevMousePosition.X);
                PlayerRotation += mouseDeltaX * (float)gameTime.ElapsedGameTime.TotalSeconds;

                Mouse.SetPosition(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            Input(gameTime);
            Renderer.SetCamera(PlayerPosition, PlayerRotation);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap, null, null, null, null);
            // Render world
            Renderer.Render(_spriteBatch, shapes);



            //  -- DEBUG STUFF -- //

            // DEBUG: Draw world
            Renderer.DrawShapes2D(_spriteBatch, shapes);

            // DEBUG: Mouse distance
            //Closest closest = Renderer.GetClosest(Mouse.GetState().Position.ToVector2(), shapes);
            //float dist = shapes[0].DifferenceWith(shapes[1], Mouse.GetState().Position.ToVector2());
            //_spriteBatch.DrawCircle(Mouse.GetState().Position.ToVector2(), dist, (int)Math.Abs(dist), Color.White);

            // DEBUG: Draw camera direction
            Vector2 playerDir = new Vector2(
                MathF.Cos(PlayerRotation),
                MathF.Sin(PlayerRotation));
            _spriteBatch.DrawLine(PlayerPosition, PlayerPosition + playerDir * 24, Color.Red);

            //  -- DEBUG STUFF -- //



            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}