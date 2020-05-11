using GameRay.Utils;
using SFML.Graphics;
using SFML.System;
using static MathFloat.MathF;

namespace GameRay.MapData.Bodies
{
    public class VerticalLine : Body
    {
        public float Length { get; set; }

        public VerticalLine(float x, float y, float len)
        {
            Position = new Vector2f(x, y);
            Length = len;
            Color = Color.White;
        }

        public override float Distance(Vector2f p)
        {
            if (p.Y < Position.Y)
                return MathUtils.Distance(p, Position);
            else
            if (p.Y > Position.Y + Length)
                return MathUtils.Distance(p, Position + new Vector2f(0, Length));
            else
                return Abs(p.X-Position.X);
        }

        public override void Render(RenderTarget buffer, Texture texture)
        {
            buffer.Draw(new Vertex[]
            {
                new Vertex
                {
                    Position = Position,
                    Color = Color
                },
                new Vertex
                {
                    Position = Position + new Vector2f(0,Length),
                    Color = Color
                }
            }, 0, 2, PrimitiveType.Lines, RenderStates.Default);
        }

        public override float TextureCoord(Vector2f surfacePoint)
        {
            return (surfacePoint.Y - Position.Y) / Length;
        }
    }
}
