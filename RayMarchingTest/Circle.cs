using SFML.Graphics;
using SFML.System;
using static GameRay.Utils.MathUtils;
using static MathFloat.MathF;

namespace RayMarchingTest
{
    class Circle : Body
    {
        public float Radius { get; set; }

        public Circle()
        {

        }

        public Circle(Vector2f pos, float rad)
        {
            Position = pos;
            Radius = rad;
            Color = Color.White;
        }

        public Circle(float x, float y, float rad)
        {
            Position = new Vector2f(x, y);
            Radius = rad;
            Color = Color.White;
        }

        public override float Distance(Vector2f p)
        {
            return Sqrt(Pow(p.X - Position.X, 2) + Pow(p.Y - Position.Y, 2)) - Radius;
        }

        public override void Render(RenderTarget buffer, Texture texture)
        {
            CircleShape shape = new CircleShape(Radius)
            {
                FillColor = Color
            };
            shape.SetPointCount((uint)FloorInt(Radius / 1.2f));
            shape.Position = Position - new Vector2f(Radius, Radius);

            buffer.Draw(shape);
            shape.Dispose();
        }

        public override float TextureCoord(Vector2f surfacePoint)
        {
            float angle = Atan2D(Position, surfacePoint) % 360;
            if (angle < 0)
                angle += 360;
            return angle / 360f;
        }
    }
}
