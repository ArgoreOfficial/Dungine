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
        public static Texture2D MissingTexture;
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


                Ray hit = Raymarch(shapes, CameraPosition, direction, 0.01f, RenderDistance);
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
            }

            UV = closestShape.GetTextureCoordinates(currentPos);
            hitNormal = closestShape.GetNormal(currentPos);
            return new Ray(closestShape, currentPos, totalDistance, Vector2.Normalize(hitNormal), UV);
        }


        public static Closest GetClosest(Vector2 samplePosition, List<Shape> shapes)
        {
            float dist = float.MaxValue;
            Shape hit = null;

            for (int i = 0; i < shapes.Count; i++)
            {
                float objectDist = shapes[i].GetSDF(samplePosition);
                for (int diff = 0; diff < shapes[i].Differences.Count; diff++)
                {
                    objectDist = MathF.Max(objectDist, Shape.Difference(shapes[i], shapes[i].Differences[diff], samplePosition));
                }
                for (int inter = 0; inter < shapes[i].Intersections.Count; inter++)
                {
                    objectDist = MathF.Max(objectDist, Shape.Intersect(shapes[i], shapes[i].Intersections[inter], samplePosition));
                }
                

                if (objectDist < dist)
                {
                    dist = objectDist;
                    hit = shapes[i];
                }
            }
            
            return new Closest(dist, hit);
        }


        public static void DrawSegment(SpriteBatch sb, Ray ray, float angle, int x)
        {
            float distance = Math.Min(ray.Distance, RenderDistance)
                                 * MathF.Cos(angle); // fix distortion
            
            if (distance < RenderDistance * MathF.Cos(angle))
            {
                int drawHeight = (int)(ProjectionPlaneDist * (56f / distance));
                
                sb.Draw(
                    ray.Hit.Texture,
                    new Rectangle(
                        x,
                        256 - drawHeight / 2,
                        1,
                        drawHeight),
                    new Rectangle(
                        (int)(ray.UV.X / ray.Hit.TextureScale.X % 128f),
                        (int)(ray.UV.Y / ray.Hit.TextureScale.Y % 128f), 
                        1, 
                        (int)(128f * ray.Hit.TextureScale.Y)),
                    Color.White * (50f / ray.Distance) * (ray.HitNormal.X / 3f + 0.7f));// new Color(ray.HitNormal.X, ray.HitNormal.Y, 0));
            }
        }

        // Debugging
        public static void DrawShapes2D(SpriteBatch sb, List<Shape> shapes)
        {
            // draw shapes
            for (int i = 0; i < shapes.Count; i++)
            {
                Color c = Color.LimeGreen;
                if (shapes[i] is SDFCircle)
                {
                    SDFCircle s = (SDFCircle)shapes[i];
                    sb.DrawCircle(s.Position, s.Radius, 24, c);
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
                        c);

                    sb.DrawLine( // bottom
                        new Vector2( // left
                            r.Position.X - r.Size.X / 2,
                            r.Position.Y + r.Size.Y / 2),
                        new Vector2( // right
                            r.Position.X + r.Size.X / 2,
                            r.Position.Y + r.Size.Y / 2),
                        c);

                    sb.DrawLine( // left
                        new Vector2( // top
                            r.Position.X - r.Size.X / 2,
                            r.Position.Y - r.Size.Y / 2),
                        new Vector2( // bottom
                            r.Position.X - r.Size.X / 2,
                            r.Position.Y + r.Size.Y / 2),
                        c);

                    sb.DrawLine( // right
                        new Vector2( // top
                            r.Position.X + r.Size.X / 2,
                            r.Position.Y - r.Size.Y / 2),
                        new Vector2( // bottom
                            r.Position.X + r.Size.X / 2,
                            r.Position.Y + r.Size.Y / 2),
                        c);
                }
            }

        }

    }
}
