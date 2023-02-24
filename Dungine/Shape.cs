using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Dungine
{
    public abstract class Shape
    {
        public Vector2 Position;
        public float Rotation;

        public Texture2D Texture;
        public Vector2 TextureOffset;
        public Vector2 TextureScale;
        protected Shape(Vector2 position, float rotation)
        {
            Position = position;
            Rotation = rotation;

        }

        public void SetTexture(Texture2D texture, Vector2 textureOffset, Vector2 textureScale)
        {
            Texture = texture;
            TextureOffset = textureOffset;
            TextureScale = textureScale;
        }

        /// <summary>
        /// returns the closest distance between a samplePoint and the shape
        /// </summary>
        /// <param name="samplePosition"></param>
        /// <returns></returns>
        public abstract float GetSDF(Vector2 samplePosition);

        /// <summary>
        /// returns the texture coordinate for a said pixel collumn
        /// </summary>
        /// <param name="samplePosition"></param>
        /// <returns></returns>
        public abstract Vector2 GetTextureCoordinates(Vector2 samplePosition);

        public Vector2 GetNormal(Vector2 samplePosition)
        {
            float small = 0.01f; // a small number

            // get normal data
            float normalX = GetSDF(samplePosition + new Vector2(small, 0)) - GetSDF(samplePosition - new Vector2(small, 0));
            float normalY = GetSDF(samplePosition + new Vector2(0, small)) - GetSDF(samplePosition - new Vector2(0, small));

            return new Vector2(normalX, normalY) / small;
        }

        // rotate and transform a vector by shape rotation and position
        protected Vector2 Transform(Vector2 samplePosition) 
        {
            Vector2 rotated = Rotate(samplePosition, Rotation);
            Vector2 translated = Translate(rotated, Position);
            return translated; 
        }


        // translate a vector
        protected Vector2 Translate(Vector2 samplePosition, Vector2 offset)
        {
            return samplePosition - offset;
        }

        // rotate a vector
        Vector2 Rotate(Vector2 samplePosition, float rotation)
        {
            float angle = rotation * MathF.PI * 2 * -1;
            float sine = MathF.Sin(angle);
            float cosine = MathF.Cos(angle);
            return new Vector2(cosine * samplePosition.X + sine * samplePosition.Y, cosine * samplePosition.Y - sine * samplePosition.X);
        }
    }

    // circle object
    public class SDFCircle : Shape
    {
        public float Radius;

        public SDFCircle(Vector2 position, float rotation, float radius) : base(position, rotation)
        {
            Radius = radius;
        }

        public override float GetSDF(Vector2 samplePosition)
        {
            samplePosition = Transform(samplePosition);
            return samplePosition.Length() - Radius;
        }

        public override Vector2 GetTextureCoordinates(Vector2 samplePosition)
        {
            Vector2 p = Position - samplePosition; // relative position
            float a = MathF.Atan(p.X / p.Y) + MathF.PI; // angle to position
            return new Vector2(a / (MathF.PI) * 128, 0); // mapped to a vector
        }
    }

    public class SDFRectangle : Shape
    {
        public Vector2 Size;

        public SDFRectangle(Vector2 position, float rotation, Vector2 size) : base(position, rotation)
        {
            Size = size;
        }

        public override float GetSDF(Vector2 samplePosition)
        {
            samplePosition = Transform(samplePosition);

            Vector2 componentWiseEdgeDistance = new Vector2(Math.Abs(samplePosition.X), Math.Abs(samplePosition.Y)) - (Size/2);

            float outsideDistance = new Vector2(Math.Max(componentWiseEdgeDistance.X, 0), Math.Max(componentWiseEdgeDistance.Y, 0)).Length();
            float insideDistance = Math.Min(Math.Max(componentWiseEdgeDistance.X, componentWiseEdgeDistance.Y), 0);
            
            return outsideDistance + insideDistance;
        }

        public override Vector2 GetTextureCoordinates(Vector2 samplePosition)
        {
            Vector2 normal = GetNormal(samplePosition);

            if(normal.X > 0.7853982f || normal.X < -0.7853982f)
            {
                return new Vector2(samplePosition.Y, 0);
            }
            
            return new Vector2(samplePosition.X, 0);
        }
    }
}
