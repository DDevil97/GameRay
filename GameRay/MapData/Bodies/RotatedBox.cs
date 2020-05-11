using SFML.Graphics;
using SFML.System;
using static GameRay.Utils.MathUtils;
using static MathFloat.MathF;

namespace GameRay.MapData.Bodies
{
    public class RotatedBox : Body
    {
        public Vector2f Size { get; set; }
        public float Angle { get; set; }

        public RotatedBox(float x, float y, float w, float h, float angle)
        {
            Position = new Vector2f(x, y);
            Size = new Vector2f(w, h);
            Color = Color.White;
            Angle = angle;
        }

        public override void Render(RenderTarget buffer, Texture texture)
        {
            RectangleShape shape = new RectangleShape(Size);
            shape.FillColor = Color;
            shape.Position = Position;
            shape.Rotation = Angle;

            buffer.Draw(shape);
            shape.Dispose();
        }

        public override float Distance(Vector2f p)
        {
            float dx, dy;
            Vector2f rotated = RotateAroundPoint(p, Position, -Angle);
            float x = rotated.X, y = rotated.Y, x1 = Position.X, y1 = Position.Y, x2 = Position.X + Size.X, y2 = Position.Y + Size.Y;
            if (x < x1)
            {
                dx = x1 - x;
                if (y < y1)
                {
                    dy = y1 - y;
                    return Sqrt(dx * dx + dy * dy);
                }
                else if (y > y2)
                {
                    dy = y - y2;
                    return Sqrt(dx * dx + dy * dy);
                }
                else
                    return dx;
            }
            else if (x > x2)
            {
                dx = x - x2;
                if (y < y1)
                {
                    dy = y1 - y;
                    return Sqrt(dx * dx + dy * dy);
                }
                else if (y > y2)
                {
                    dy = y - y2;
                    return Sqrt(dx * dx + dy * dy);
                }
                else
                    return dx;
            }
            else
            {
                if (y < y1)
                    return y1 - y;
                else if (y > y2)
                    return y - y2;
                else
                    return 0f; // inside the rectangle or on the edge
            }
        }

        public override float TextureCoord(Vector2f surfacePoint)
        {
            Vector2f rotated = RotateAroundPoint(surfacePoint, Position, -Angle);
            if (rotated.X > Position.X && rotated.X < Position.X + Size.X)
                return (rotated.X - Position.X) / Size.X;
            else
                return (rotated.Y - Position.Y) / Size.Y;
        }
    }
}

