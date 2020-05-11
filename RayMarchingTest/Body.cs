using SFML.Graphics;
using SFML.System;

namespace RayMarchingTest
{
    abstract class Body
    {
        public Vector2f Position { get; set; }

        public Color Color { get; set; }

        public Body()
        {
            Position = new Vector2f();
        }

        public Body(Vector2f pos)
        {
            Position = pos;
        }

        public abstract float TextureCoord(Vector2f surfacePoint);

        public abstract void Render(RenderTarget buffer, Texture texture);

        public abstract float Distance(Vector2f p);
    }
}
