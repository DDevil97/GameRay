using SFML.Graphics;
using SFML.System;

namespace GameRay.Elements
{
    public class Light
    {
        //Standar properties
        public Vector2f Position { get; set; }
        public Color Color { get; set; }

        public Light(Vector2f pos, Color col)
        {
            Position = pos;
            Color = col;
        }
    }
}
