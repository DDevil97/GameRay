using GameRay.Elements;
using SFML.System;
using System;
using System.Collections.Generic;
using static GameRay.Utils.MathUtils;
using static MathFloat.MathF;

namespace GameRay.MapData.Collision
{
    //Represents the result of a raycast to the map
    public class RayResult
    {
        public bool Valid { get; set; }
        public Vector2i Tile { get; set; }
        public Vector2f Position { get; set; }
        public Side Side { get; set; }
        public float Magnitude { get; set; }
    }

    public class RayCaster
    {
        //Constants
        public const float MaxDistance = 60000;
        public const int MaxRayRecast = 3;

        //Read only properties
        public Map Map { get; protected set; }
        public List<Sprite> Entities { get; protected set; }
        public int CellSize { get; protected set; }

        public RayCaster(Map map, int cellSize)
        {
            Map = map;
            CellSize = cellSize;
        }

        //Private functions
        private RayResult RayCastInternal(Vector2f O, Vector2f startO, float A, Vector2i? toIgnore)
        {
            A *= ToRadians;
            Vector2f D = startO + new Vector2f(Cos(A) * 1000, Sin(A) * 1000);
            Vector2f Slope = D - startO;
            Vector2f Delta;
            Vector2f Ph, Pv;
            float Dh, Dv;
            RayResult res;

            Delta.X = 0;
            Delta.Y = 0;
            //If Dx is zero, then there's no horizontal intersections
            if (Slope.X == 0)
            {
                Ph = new Vector2f(startO.X + MaxDistance, startO.Y + MaxDistance);
                Dh = MaxDistance;
            }
            else
            {
                Delta.X = CellSize * Math.Sign(Slope.X);

                Ph.X = (Floor(startO.X / CellSize) + (Math.Sign(Delta.X) == 1 ? 1 : 0)) * CellSize;

                if (Slope.Y == 0)
                {
                    Delta.Y = 0;
                    Ph.Y = startO.Y;
                }
                else
                {
                    Delta.Y = (Delta.X * Slope.Y) / Slope.X;
                    Ph.Y = startO.Y + (Slope.Y * (Ph.X - startO.X)) / Slope.X;
                }

                while (!Map[FloorInt(Ph.X / CellSize) + (Delta.X < 0 ? -1 : 0), FloorInt(Ph.Y / CellSize), toIgnore].Solid)
                    Ph += Delta;

                Dh = Pow((Ph.X - O.X), 2) + Pow((Ph.Y - O.Y), 2);
            }

            //If Dy is zero, then there's no vertical intersections
            if (Slope.Y == 0)
            {
                Pv = new Vector2f(startO.X + MaxDistance, startO.Y + MaxDistance);
                Dv = MaxDistance;
            }
            else
            {
                Delta.Y = CellSize * Math.Sign(Slope.Y);

                Pv.Y = (Floor(startO.Y / CellSize) + (Math.Sign(Delta.Y) == 1 ? 1 : 0)) * CellSize;

                if (Slope.X == 0)
                {
                    Delta.X = 0;
                    Pv.X = startO.X;
                }
                else
                {
                    Delta.X = (Delta.Y * Slope.X) / Slope.Y;
                    Pv.X = startO.X + (Slope.X * (Pv.Y - startO.Y)) / Slope.Y;
                }


                while (!Map[FloorInt(Pv.X / CellSize), FloorInt(Pv.Y / CellSize) + (Delta.Y < 0 ? -1 : 0), toIgnore].Solid)
                    Pv += Delta;

                Dv = Pow(Pv.X - O.X, 2) + Pow(Pv.Y - O.Y, 2);
            }


            if (Dh < Dv)
            {
                res = new RayResult
                {
                    Tile = new Vector2i(FloorInt(Ph.X / CellSize) + (Delta.X < 0 ? -1 : 0), FloorInt(Ph.Y / CellSize)),
                    Position = Ph,
                    Magnitude = Sqrt(Dh),
                    Side = Slope.X < 0 ? Side.Left : Side.Right
                };

                return res;
            }
            else
            {
                res = new RayResult
                {
                    Tile = new Vector2i(FloorInt(Pv.X / CellSize), FloorInt(Pv.Y / CellSize) + (Delta.Y < 0 ? -1 : 0)),
                    Position = Pv,
                    Magnitude = Sqrt(Dv),
                    Side = Slope.Y < 0 ? Side.Up : Side.Down
                };

                return res;
            }
        }

        //Public interface
        public RayResult RayCast(Vector2f O, float A)
        {
            RayResult result;
            Vector2f start = O;
            Vector2i? toIgnore = null;
            int fs = 0;

            do
            {
                result = RayCastInternal(O, start, A, toIgnore);
                result = Map[result.Tile.X, result.Tile.Y, toIgnore].OnIntersection(result, O, A, this);
                start = result.Position;
                toIgnore = result.Tile;
            }
            while (!result.Valid & fs++ < MaxRayRecast);

            return result;
        }
    }
}
