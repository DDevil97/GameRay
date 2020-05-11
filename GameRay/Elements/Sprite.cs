using GameRay.MapData;
using SFML.Graphics;
using SFML.System;

namespace GameRay.Elements
{
    public class Sprite : PhysicsEntity
    {
        //Standar properties
        public Vector2i AtlasTexture { get; set; }      
        public Color Light { get; set; }
        public string Identifier { get; set; }
        public float Angle { get; set; }

        //Used for rendering, ignored for everthing else
        public Vector2f TransformedPosition { get; set; }
        public float DistanceToPlayer { get; set; }

        public Sprite(Vector2f pos, Vector2i atlas)
        {
            Position = pos;
            AtlasTexture = atlas;
        }

        //Public interface
        public virtual void DoAction(Map map)
        {

        }
    }
}
