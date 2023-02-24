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



            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            WallTexture = Content.Load<Texture2D>("Textures/Testing/wall");
            WindowTexture = Content.Load<Texture2D>("Textures/Testing/window");
            
            // needs to be after
            shapes.Add(new SDFCircle(new Vector2(30, 30), 0f, WallTexture, new Vector2(), 10f));
            shapes.Add(new SDFCircle(new Vector2(50, 30), 0f, WallTexture, new Vector2(), 15f));
            shapes.Add(new SDFCircle(new Vector2(240, 140), 0f, WallTexture, new Vector2(), 15f));
            shapes.Add(new SDFRectangle(new Vector2(90, 200), 0f, WindowTexture, new Vector2(), new Vector2(32, 64)));

        }

        protected void Input(GameTime gameTime)
        {
            KeyboardState key = Keyboard.GetState();
            if (key.IsKeyDown(Keys.Escape))
                Exit();

            Vector2 playerDir = new Vector2(
                MathF.Cos(PlayerRotation),
                MathF.Sin(PlayerRotation));

            if (key.IsKeyDown(Keys.W))
            {
                PlayerPosition += (float)gameTime.ElapsedGameTime.TotalSeconds * MovementSpeed * playerDir;
            }
            if (key.IsKeyDown(Keys.S))
            {
                PlayerPosition -= (float)gameTime.ElapsedGameTime.TotalSeconds * MovementSpeed * playerDir;
            }
            if (key.IsKeyDown(Keys.A))
            {
                PlayerRotation -= (float)gameTime.ElapsedGameTime.TotalSeconds * 2f;
            }
            if (key.IsKeyDown(Keys.D))
            {
                PlayerRotation += (float)gameTime.ElapsedGameTime.TotalSeconds * 2f;
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

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, null);
            // Render world
            Renderer.Render(_spriteBatch, shapes);



            //  -- DEBUG STUFF -- //

            // DEBUG: Draw world
            Renderer.DrawShapes2D(_spriteBatch, shapes);

            // DEBUG: Mouse distance
            Closest closest = Renderer.GetClosest(Mouse.GetState().Position.ToVector2(), shapes);
            _spriteBatch.DrawCircle(Mouse.GetState().Position.ToVector2(), closest.Distance, (int)Math.Abs(closest.Distance), Color.White);

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