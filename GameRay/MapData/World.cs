using System.Collections.Generic;

namespace GameRay.MapData
{
    public class World
    {
        public List<Body> Objects { get; set; }

        public World()
        {
            Objects = new List<Body>();
        }

        
    }
}
