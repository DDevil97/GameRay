using GameRay.Utils;
using SFML.Graphics;
using SFML.System;
using static MathFloat.MathF;
using static GameRay.Utils.MathUtils;

namespace GameRay.MapData.Bodies
{
    public class Line : Body
    {
        public Vector2f Destination { get; set; }

        public Line(float x1, float y1, float x2, float y2)
        {
            Position = new Vector2f(x1, y1);
            Destination = new Vector2f(x2, y2);
            Color = Color.White;
        }

        public override float Distance(Vector2f p)
        {
            float angle = Atan2D(Destination, Position);
            p = RotateAroundPoint(p, Position, -angle);
            Vector2f rotatedDestination = RotateAroundPoint(Destination, Position, -angle);
            if (p.X < Position.X)
                return MathUtils.Distance(p, Position);
            else
            if (p.X > rotatedDestination.X)
                return MathUtils.Distance(p, rotatedDestination);
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
                    Position = Destination,
                    Color = Color
                }
            }, 0, 2, PrimitiveType.Lines, RenderStates.Default);
        }

        public override float TextureCoord(Vector2f surfacePoint)
        {
            float angle = Atan2D(Destination, Position);
            Vector2f rotated = RotateAroundPoint(surfacePoint, Position, -angle);
            Vector2f rotatedDestination = RotateAroundPoint(Destination, Position, -angle);
            float lineSize = Abs(rotatedDestination.X - Position.X);
            return (rotated.X - Position.X) / lineSize;
        }
    }
}
