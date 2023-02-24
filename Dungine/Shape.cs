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

        protected Shape(Vector2 position, float rotation, Texture2D texture, Vector2 textureOffset)
        {
            Position = position;
            Rotation = rotation;
            Texture = texture;
            TextureOffset = textureOffset;
        }



        /// <summary>
        /// returns the closest distance between a samplePoint and the shape
        /// </summary>
        /// <param name="samplePosition"></param>
        /// <returns></returns>
        public abstract float SDF(Vector2 samplePosition);

        // rotate and transform a vector by shape rotation and position
        protected Vector2 Transform(Vector2 samplePosition) 
        {
            Vector2 rotated = Rotate(samplePosition, Rotation);
            Vector2 translated = Translate(rotated, Position);
            return translated; 
        }

        protected Vector2 Untransform(Vector2 samplePosition)
        {
            Vector2 rotated = Rotate(samplePosition, -Rotation);
            Vector2 translated = Translate(rotated, -Position);
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

        public SDFCircle(Vector2 position, float rotation, Texture2D texture, Vector2 textureOffset, float radius) : base(position, rotation, texture, textureOffset)
        {
            Radius = radius;
        }

        public override float SDF(Vector2 samplePosition)
        {
            samplePosition = Transform(samplePosition);
            return samplePosition.Length() - Radius;
        }
    }

    public class SDFRectangle : Shape
    {
        public Vector2 Size;

        public SDFRectangle(Vector2 position, float rotation, Texture2D texture, Vector2 textureOffset, Vector2 size) : base(position, rotation, texture, textureOffset)
        {
            Size = size;
        }

        public override float SDF(Vector2 samplePosition)
        {
            samplePosition = Transform(samplePosition);

            Vector2 componentWiseEdgeDistance = new Vector2(Math.Abs(samplePosition.X), Math.Abs(samplePosition.Y)) - (Size/2);

            float outsideDistance = new Vector2(Math.Max(componentWiseEdgeDistance.X, 0), Math.Max(componentWiseEdgeDistance.Y, 0)).Length();
            float insideDistance = Math.Min(Math.Max(componentWiseEdgeDistance.X, componentWiseEdgeDistance.Y), 0);
            
            return outsideDistance + insideDistance;
        }
    }
}
