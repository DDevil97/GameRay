using SFML.System;
using System;

namespace GameRay.MapData
{
    public class Map
    {
        //Read only properties
        public Tile[,] Tiles { get; protected set; }

        public Map(int xSize, int ySize)
        {
            Tiles = new Tile[xSize, ySize];
        }

        //Public interface
        public Tile this[int px, int py, Vector2i? toIgnore = null]
        {
            get
            {
                if (toIgnore.HasValue && px == toIgnore.Value.X & py == toIgnore.Value.Y)
                    return new Tile { Solid = false };

                if (px < 0 || px > Tiles.GetLength(0) - 1 || py < 0 || py > Tiles.GetLength(1) - 1)
                    return new Tile { Solid = true };
                else
                    return Tiles[px, py];
            }
            set
            {
                if (px < 0 || px > Tiles.GetLength(0) - 1 || py < 0 || py > Tiles.GetLength(1) - 1)
                    throw new IndexOutOfRangeException($"The map coordinates provided ({px},{py}) are out of the map size ({Tiles.GetLength(0)},{Tiles.GetLength(1)}).");
                else
                    Tiles[px, py] = value;
            }
        }
    }
}
