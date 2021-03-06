﻿using GameRay.MapData;
using GameRay.MapData.Collision;
using SFML.System;
using System;
using static GameRay.Utils.MathUtils;
using static MathFloat.MathF;

namespace TestBed
{
    class CircleTile : Tile
    {

        public static int LineCircleIntersections(float cx, float cy, float radius,
                                                  Vector2f point1, Vector2f point2,
                                                  out Vector2f intersection1, out Vector2f intersection2)
        {
            float dx, dy, A, B, C, det, t;

            dx = point2.X - point1.X;
            dy = point2.Y - point1.Y;

            A = dx * dx + dy * dy;
            B = 2 * (dx * (point1.X - cx) + dy * (point1.Y - cy));
            C = (point1.X - cx) * (point1.X - cx) +
                (point1.Y - cy) * (point1.Y - cy) -
                radius * radius;

            det = B * B - 4 * A * C;
            if ((A <= 0.0000000001) || (det < 0))
            {
                // No real solutions.
                intersection1 = new Vector2f(float.NaN, float.NaN);
                intersection2 = new Vector2f(float.NaN, float.NaN);
                return 0;
            }
            else if (det == 0)
            {
                // One solution.
                t = -B / (2 * A);
                intersection1 =
                    new Vector2f(point1.X + t * dx, point1.Y + t * dy);
                intersection2 = new Vector2f(float.NaN, float.NaN);
                return 1;
            }
            else
            {
                // Two solutions.
                t = (float)((-B + Math.Sqrt(det)) / (2 * A));
                intersection1 =
                    new Vector2f(point1.X + t * dx, point1.Y + t * dy);
                t = (float)((-B - Math.Sqrt(det)) / (2 * A));
                intersection2 =
                    new Vector2f(point1.X + t * dx, point1.Y + t * dy);
                return 2;
            }
        }

        public override RayResult OnIntersection(RayResult original, Vector2f O, float angle, RayCaster caster)
        {
            int points = LineCircleIntersections(caster.CellSize * original.Tile.X + (caster.CellSize / 2),
                                                 caster.CellSize * original.Tile.Y + (caster.CellSize / 2),
                                                 caster.CellSize / 2,
                                                 original.Position,
                                                 original.Position + new Vector2f(300 * CosD(angle), 300 * SinD(angle)),
                                                 out Vector2f p1, out Vector2f p2);

            switch (points)
            {
                case 0:
                    original.Valid = false;
                    return original;
                case 1:
                    return new RayResult
                    {
                        Valid = true,
                        Magnitude = Distance(O, p1),
                        Side = Side.None,
                        Position = p1,
                        Tile = original.Tile
                    };
                case 2:
                    Vector2f final = Distance(O, p1) <= Distance(O, p2) ? p1 : p2;
                    return new RayResult
                    {
                        Valid = true,
                        Magnitude = Distance(O, final),
                        Side = Side.None,
                        Position = final,
                        Tile = original.Tile
                    };
            }

            original.Valid = true;
            return original;
        }

        public override (Vector2f top, Vector2f bottom) CalculateTextureCoords(RayResult original, Vector2f O, float angle, RayCaster caster)
        {
            float tx = Atan2(caster.CellSize * original.Tile.Y + (caster.CellSize / 2) - original.Position.Y,
                             caster.CellSize * original.Tile.X + (caster.CellSize / 2) - original.Position.X) + (float)Math.PI;

            tx = (float)((tx * caster.CellSize) / (Math.PI * 2));
            tx *= 4;
            tx %= (float)(Math.PI * 2);
            Vector2f top = new Vector2f(
                        DownAtlas.X * caster.CellSize + tx,
                        DownAtlas.Y * caster.CellSize);
            Vector2f bottom = new Vector2f(
                top.X,
                top.Y + caster.CellSize);

            return (top, bottom);
        }
    }
}
