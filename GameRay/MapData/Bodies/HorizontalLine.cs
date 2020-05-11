using GameRay.Utils;
using SFML.Graphics;
using SFML.System;
using static MathFloat.MathF;

namespace GameRay.MapData.Bodies
{
    public class HorizontalLine : Body
    {
        public float Length { get; set; }

        public HorizontalLine(float x, float y, float len)
        {
            Position = new Vector2f(x, y);
            Length = len;
            Color = Color.White;
        }

        public override float Distance(Vector2f p)
        {
            if (p.X < Position.X)
                return MathUtils.Distance(p, Position);
            else
            if (p.X > Position.X + Length)
                return MathUtils.Distance(p, Position + new Vector2f(Length, 0));
            else
                return Abs(p.Y-Position.Y);
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
                    Position = Position + new Vector2f(Length,0),
                    Color = Color
                }
            }, 0, 2, PrimitiveType.Lines, RenderStates.Default);
        }

        public override float TextureCoord(Vector2f surfacePoint)
        {
            return (surfacePoint.X - Position.X) / Length;
        }
    }
}
