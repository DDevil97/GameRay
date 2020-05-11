using SFML.System;
using System.Linq;
using System.Collections.Generic;
using static GameRay.Utils.MathUtils;
using static MathFloat.MathF;
namespace RayMarchingTest
{
    class Collision
    {
        public Body Object { get; set; }
        public Vector2f Position { get; set; }
        public float Distance { get; set; } 
        public float TextureCoord { get; set; }
    }

    class World
    {
        public List<Body> Objects { get; set; }

        public World()
        {
            Objects = new List<Body>();
        }

        public Collision CastRay(Vector2f position, float angle)
        {
            angle *= ToRadians;
            Vector2f actualPosition = position;
            Body picked = null;
            float distance;

            for (; ;)
            {
                distance = 3000;

                float calculatedDistance;
                for (int i = 0; i < Objects.Count; i++)
                {
                    calculatedDistance = Objects[i].Distance(actualPosition);
                    if (calculatedDistance < distance)
                    {
                        distance = calculatedDistance;
                        picked = Objects[i];
                    }
                }  

                if (distance < 0.1) 
                    break;
                else if (distance > 1200)
                {
                    picked = null;
                    break;
                }

                actualPosition.X += Cos(angle)*distance;
                actualPosition.Y += Sin(angle)*distance;
            }

            Collision collision = new Collision
            {
                Distance = distance,
                Object = picked,
                Position = actualPosition,
                TextureCoord = picked != null ? picked.TextureCoord(actualPosition) : 0           
            };

            return collision;
        }
    }
}
