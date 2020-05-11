using SFML.System;
using System.Linq;
using static GameRay.Utils.MathUtils;
using static MathFloat.MathF;

namespace GameRay.MapData.Collision
{
    public class MarchingCollision
    {
        public Body Object { get; set; }
        public Vector2f Position { get; set; }
        public float Distance { get; set; }
        public float TextureCoord { get; set; }
    }

    public class RayMarching
    {
        public World World { get; set; }

        public RayMarching(World world)
        {
            World = world;
        }

        public MarchingCollision CastRay(Vector2f position, float angle)
        {
            angle *= ToRadians;
            Vector2f actualPosition = position;
            Body picked = null;
            float distance;

            for (; ; )
            {
                distance = 3000;

                float calculatedDistance;
                for (int i = 0; i < World.Objects.Count; i++)
                {
                    calculatedDistance = World.Objects[i].Distance(actualPosition);
                    if (calculatedDistance < distance)
                    {
                        distance = calculatedDistance;
                        picked = World.Objects[i];
                    }
                }

                if (distance < 0.1)
                    break;
                else if (distance > 1200)
                {
                    picked = null;
                    break;
                }

                actualPosition.X += Cos(angle) * distance;
                actualPosition.Y += Sin(angle) * distance;
            }

            MarchingCollision collision = new MarchingCollision
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
