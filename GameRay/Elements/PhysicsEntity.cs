using SFML.System;

namespace GameRay.Elements
{
    public class PhysicsEntity
    {
        //Standar properties
        public Vector2f Position { get; set; }
        public Vector2f Velocity { get; set; }
        public float Mass { get; set; }
        public float Bounce { get; set; }
        public bool MapCollision { get; set; }
        public bool EntityCollision { get; set; }
    }
}
