using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Dungine
{
    public struct RenderWall
    {
        public Point RelativePosition;
        public Point WorldPosition;
        public int Side; // 0: right, 1: front, 2: left

        public RenderWall(Point relativePosition, Point worldPosition, int side)
        {
            RelativePosition = relativePosition;
            WorldPosition = worldPosition;
            Side = side;
        }
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        int[,] map = {
            { 1,1,1,1,1,1,1,1 },
            { 1,1,1,1,1,0,0,1 },
            { 1,0,0,0,1,0,0,1 },
            { 1,1,1,0,1,0,0,1 },
            { 1,1,1,0,0,0,0,1 },
            { 1,1,1,0,0,0,0,1 },
            { 1,1,1,0,1,1,1,1 },
            { 1,1,1,1,1,1,1,1 }
        };
        Point PlayerPos = new Point(3, 6);
        List<RenderWall> RenderWalls = new List<RenderWall>();
        int MaxShadeDist = 6;
        float MoveCooldown = 0;

        // viewtest map:
        //████████
        //█████  █
        //█   █  █
        //███ █  █
        //███    █
        //███    █
        //███ ████
        //████████

        // viewtest result:
        //████████
        //█████▒▒█
        //█ ▒▒█▒▒█
        //███▒█▒▒█
        //███▒▒▒ █
        //███▒▒  █
        //███▒████
        //████████

        Texture2D LeftWall;
        Texture2D RightWall;
        Texture2D FrontWall;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 256;
            _graphics.PreferredBackBufferHeight = 256;
            _graphics.ApplyChanges();

            base.Initialize();
        }
        
        public void VisibilityAlgorithm()
        {
            List<Point> check = new List<Point>();
            RenderWalls.Clear();
            check.Add(PlayerPos);

            // get visible walls
            for (int i = 0; i < check.Count; i++)
            {
                if (check[i].Y <= 0) continue;

                // check if there's a wall in front
                if (map[check[i].Y - 1, check[i].X] != 0)
                {
                    RenderWall rw = new RenderWall( // create renderwall
                        check[i] - PlayerPos + new Point(0, 1),
                        check[i],
                        1);
                    RenderWalls.Add(rw); // add to renderque
                }
                else
                {
                    check.Add(new Point(check[i].X, check[i].Y - 1));

                    // check if there's a wall to the right
                    if (map[check[i].Y - 1, check[i].X + 1] != 0) // if a wall is detected
                    {
                        RenderWall rw = new RenderWall( // create renderwall
                            check[i] - PlayerPos,
                            check[i],
                            0);
                        RenderWalls.Add(rw); // add to renderque
                    }
                    else { check.Add(new Point(check[i].X + 1, check[i].Y - 1)); }

                    // check if there's a wall to the left
                    if (map[check[i].Y - 1, check[i].X - 1] != 0) // if a wall is detected
                    {
                        RenderWall rw = new RenderWall( // create renderwall
                            check[i] - PlayerPos,
                            check[i],
                            2);
                        RenderWalls.Add(rw); // add to renderque
                    }
                    else { check.Add(new Point(check[i].X - 1, check[i].Y - 1)); } // add to checklist
                }
            }

            RenderWalls = RenderWalls.OrderBy(w => w.RelativePosition.Y).ToList();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            LeftWall = Content.Load<Texture2D>("Textures/Testing/left");
            RightWall = Content.Load<Texture2D>("Textures/Testing/right");
            FrontWall = Content.Load<Texture2D>("Textures/Testing/front");
        }

        protected override void Update(GameTime gameTime)
        {
            if (MoveCooldown > 0) MoveCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            KeyboardState key = Keyboard.GetState();
            if (key.IsKeyDown(Keys.Escape))
                Exit();
            
            if(MoveCooldown <= 0 && key.IsKeyDown(Keys.W))
            {
                PlayerPos.Y--;
                MoveCooldown = 0.3f;
            }
            if (MoveCooldown <= 0 && key.IsKeyDown(Keys.S))
            {
                PlayerPos.Y++;
                MoveCooldown = 0.3f;
            }
            if (MoveCooldown <= 0 && key.IsKeyDown(Keys.A))
            {
                PlayerPos.X--;
                MoveCooldown = 0.3f;
            }
            if (MoveCooldown <= 0 && key.IsKeyDown(Keys.D))
            {
                PlayerPos.X++;
                MoveCooldown = 0.3f;
            }

            VisibilityAlgorithm();


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            
            for (int i = 0; i < RenderWalls.Count; i++)
            {
                if (RenderWalls[i].Side == 0)
                {
                    DrawRightWall(RenderWalls[i].RelativePosition * new Point(1, -1));
                }
                else if (RenderWalls[i].Side == 1)
                {
                    DrawFrontWall(RenderWalls[i].RelativePosition * new Point(1, -1));
                }
                else if (RenderWalls[i].Side == 2)
                {
                    DrawLeftWall(RenderWalls[i].RelativePosition * new Point(1, -1));
                }
            }
            /*
            
            DrawLeftWall(new Point(0, 0));
            DrawLeftWall(new Point(0, 1));
            DrawLeftWall(new Point(0, 2));
            
            DrawRightWall(new Point(1, 0));
            DrawRightWall(new Point(1, 1));
            DrawRightWall(new Point(1, 2));

            DrawFrontWall(new Point(0, 2));
            DrawFrontWall(new Point(1, 2));
            */
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public void DrawLeftWall(Point localPos)
        {
            float pow = MathF.Pow(2, localPos.Y);
            float posMultiplier = -2f / pow + 2f;
            float c = (1f - (float)localPos.Y / (float)MaxShadeDist);

            _spriteBatch.Draw(
                    LeftWall,
                    new Rectangle(
                        (int)(LeftWall.Width * posMultiplier) + 32 * localPos.X,
                        (int)(LeftWall.Height * posMultiplier) / 4,
                        (int)(LeftWall.Width / pow),
                        (int)(LeftWall.Height / pow)),
                    new Color(c,c,c));
        }

        public void DrawRightWall(Point localPos)
        {
            float pow = MathF.Pow(2, localPos.Y);
            float posMultiplier = -2f / pow + 2f;
            int width = (int)(RightWall.Width / pow);
            float c = (1f - (float)localPos.Y / (float)MaxShadeDist);

            

            _spriteBatch.Draw(
                    RightWall,
                    new Rectangle(
                        256 - (int)(RightWall.Width * posMultiplier) + 32 * localPos.X - width,
                        (int)(RightWall.Height * posMultiplier) / 4,
                        width,
                        (int)(RightWall.Height / pow)),
                    new Color(c, c, c));
        }

        public void DrawFrontWall(Point localPos)
        {
            float pow = MathF.Pow(2, localPos.Y);
            float posMultiplier = -2f / pow + 2f;
            int width = (int)(FrontWall.Width / pow);
            float c = (1f - (float)localPos.Y / (float)MaxShadeDist);

            _spriteBatch.Draw(
                    FrontWall,
                    new Rectangle(
                        (int)(LeftWall.Width * posMultiplier) + localPos.X * width + width/2,
                        (int)(LeftWall.Height * posMultiplier) / 4 + width/2,
                        width,
                        (int)(FrontWall.Height / pow)),
                    new Color(c, c, c));
        }
    }
}