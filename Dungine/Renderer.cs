using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungine
{
    public struct Closest
    {
        public float Distance;
        public Shape Shape;
        
        public Closest(float distance, Shape shape)
        {
            Distance = distance;
            Shape = shape;
        }
    }
    public struct Ray
    {
        public Shape Hit;
        public Vector2 HitPosition;
        public Vector2 HitNormal;
        public Vector2 UV;
        public float Distance;
        public Ray(Shape hit, Vector2 hitPosition, float length, Vector2 normal, Vector2 uv)
        {
            Hit = hit;
            HitPosition = hitPosition;
            Distance = length;
            HitNormal = normal;
            UV = uv;
        }
    }

    public static class Renderer
    {

        public static float RenderDistance = 500f;
        public static int FOV = 90;
        public static float ProjectionPlaneDist = 158f;
        static Point ScreenSize = new Point(512,512);

        public static float CameraRotation = 0;
        public static Vector2 CameraPosition = new Vector2(10,10);

        // setup
        public static void Setup(float renderDistance, int fov, Point screenSize)
        {
            RenderDistance = renderDistance;
            FOV = fov;
            ProjectionPlaneDist = (screenSize.X / 2) / MathF.Tan(FOV / 2);
        }

        public static void SetCamera(Vector2 position, float rotation)
        {
            CameraPosition = position;
            CameraRotation = rotation;
        }

        /// <summary>
        /// Render the world
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="texture"></param>
        /// <param name="shapes"></param>
        public static void Render(SpriteBatch sb, List<Shape> shapes) // texture is temporary
        {
            for (int i = 0; i < 512; i++)
            {
                // raymarch
                //float angle = MathHelper.ToRadians(i / 512f * 90f - 45);
                float angle = MathF.Atan((i - 256) / 270f);

                Vector2 direction = new Vector2(
                    MathF.Cos(angle + CameraRotation),
                    MathF.Sin(angle + CameraRotation));


                Ray hit = Raymarch(shapes, CameraPosition, direction, 0.001f, RenderDistance);
                float distance = Math.Min(hit.Distance, RenderDistance)
                                 * MathF.Cos(angle); // fix distortion

                DrawSegment(sb, hit, angle, i);
            }

        }


        public static Ray Raymarch(List<Shape> shapes, Vector2 startPos, Vector2 direction, float minDist, float maxDist)
        {
            if (direction.Length() > 1.001f || direction.Length() > 0.999f) direction.Normalize(); // normalize direction

            Shape closestShape = null;
            
            Vector2 currentPos = startPos;
            Vector2 hitNormal = new Vector2();
            Vector2 UV = new Vector2();
            
            float totalDistance = 0;
            float distance = float.MaxValue;
            

            while (distance >= minDist && totalDistance < maxDist)
            {
                // get distance
                Closest closest = GetClosest(currentPos, shapes);
                distance = MathF.Abs(closest.Distance);
                closestShape = closest.Shape;

                // "march" ray position
                currentPos += direction * distance;
                totalDistance += distance;
            
                // get normal data
                float normalX = closestShape.SDF(currentPos + new Vector2(0.01f, 0)) - closestShape.SDF(currentPos - new Vector2(0.01f, 0));
                float normalY = closestShape.SDF(currentPos + new Vector2(0, 0.01f)) - closestShape.SDF(currentPos - new Vector2(0, 0.01f));

                hitNormal = new Vector2(normalX, normalY) / 0.01f;
                
                if (closestShape is SDFCircle)
                {
                    Vector2 p = closestShape.Position - currentPos;
                    float a = MathF.Atan(p.X / p.Y) + MathF.PI;
                    UV = new Vector2(a * 32, 0);
                }
                else if (closestShape is SDFRectangle)
                {
                    UV = new Vector2(currentPos.X, 0);
                }
            }

            return new Ray(closestShape, currentPos, totalDistance, Vector2.Normalize(hitNormal), UV);
        }


        public static Closest GetClosest(Vector2 samplePosition, List<Shape> shapes)
        {
            float dist = float.MaxValue;
            Shape lastHit = null;
            for (int i = 0; i < shapes.Count; i++)
            {
                float newdist = shapes[i].SDF(samplePosition);
                if (newdist < dist)
                {
                    dist = newdist;
                    lastHit = shapes[i];
                }
            }
            
            return new Closest(dist, lastHit);
        }


        public static void DrawSegment(SpriteBatch sb, Ray ray, float angle, int x)
        {
            float distance = Math.Min(ray.Distance, RenderDistance)
                                 * MathF.Cos(angle); // fix distortion
            
            if (distance < RenderDistance * MathF.Cos(angle))
            {
                int drawHeight = (int)(ProjectionPlaneDist * (64f / distance));
                
                sb.Draw(
                    ray.Hit.Texture,
                    new Rectangle(
                        x,
                        256 - drawHeight / 2,
                        1,
                        drawHeight),
                    new Rectangle((int)(ray.UV.X % 128f), 0, 1, 128),
                    Color.White);// new Color(ray.HitNormal.X, ray.HitNormal.Y, 0));
            }
        }

        // Debugging
        public static void DrawShapes2D(SpriteBatch sb, List<Shape> shapes)
        {
            // draw shapes
            for (int i = 0; i < shapes.Count; i++)
            {
                if (shapes[i] is SDFCircle)
                {
                    SDFCircle s = (SDFCircle)shapes[i];
                    sb.DrawCircle(s.Position, s.Radius, 24, Color.Green);
                }
                else if (shapes[i] is SDFRectangle)
                {
                    SDFRectangle r = (SDFRectangle)shapes[i];
                    sb.DrawLine( // top
                        new Vector2( // left
                            r.Position.X - r.Size.X / 2,
                            r.Position.Y - r.Size.Y / 2),
                        new Vector2( // right
                            r.Position.X + r.Size.X / 2,
                            r.Position.Y - r.Size.Y / 2),
                        Color.Green);

                    sb.DrawLine( // bottom
                        new Vector2( // left
                            r.Position.X - r.Size.X / 2,
                            r.Position.Y + r.Size.Y / 2),
                        new Vector2( // right
                            r.Position.X + r.Size.X / 2,
                            r.Position.Y + r.Size.Y / 2),
                        Color.Green);

                    sb.DrawLine( // left
                        new Vector2( // top
                            r.Position.X - r.Size.X / 2,
                            r.Position.Y - r.Size.Y / 2),
                        new Vector2( // bottom
                            r.Position.X - r.Size.X / 2,
                            r.Position.Y + r.Size.Y / 2),
                        Color.Green);

                    sb.DrawLine( // right
                        new Vector2( // top
                            r.Position.X + r.Size.X / 2,
                            r.Position.Y - r.Size.Y / 2),
                        new Vector2( // bottom
                            r.Position.X + r.Size.X / 2,
                            r.Position.Y + r.Size.Y / 2),
                        Color.Green);
                }
            }

        }

    }
}
