using GameRay.MapData.Collision;
using SFML.System;

namespace GameRay.MapData
{
    //Represents the sides of a four sided tile
    public enum Side
    {
        Up = 0,
        Right,
        Down,
        Left,
        None
    }

    public class Tile
    {
        //Standar properties
        public bool Solid { get; set; }
        public bool IsCeilMap { get; set; }
        public bool IsFloorMap { get; set; }
        public Vector2i UpAtlas { get; set; }
        public Vector2i DownAtlas { get; set; }
        public Vector2i LeftAtlas { get; set; }
        public Vector2i RightAtlas { get; set; }
        public Vector2i FloorAtlas { get; set; }
        public Vector2i CeilAtlas { get; set; }

        //Public interface
        public virtual RayResult OnIntersection(RayResult original, Vector2f O, float angle, RayCaster caster)
        {
            original.Valid = true;
            return original;
        }
        public virtual (Vector2f top, Vector2f bottom) CalculateTextureCoords(RayResult original, Vector2f O, float angle, RayCaster caster)
        {
            Vector2f textureCordUp;
            Vector2f textureCordDown;
            switch (original.Side)
            {
                case Side.Down:
                    textureCordUp = new Vector2f(
                        DownAtlas.X * caster.CellSize + (original.Position.X % caster.CellSize),
                        DownAtlas.Y * caster.CellSize);
                    textureCordDown = new Vector2f(
                        textureCordUp.X,
                        textureCordUp.Y + caster.CellSize);
                    break;
                case Side.Up:
                    textureCordUp = new Vector2f(
                        UpAtlas.X * caster.CellSize + (original.Position.X % caster.CellSize),
                        UpAtlas.Y * caster.CellSize);
                    textureCordDown = new Vector2f(
                        textureCordUp.X,
                        textureCordUp.Y + caster.CellSize);
                    break;
                case Side.Left:
                    textureCordUp = new Vector2f(
                        LeftAtlas.X * caster.CellSize + (original.Position.Y % caster.CellSize),
                        LeftAtlas.Y * caster.CellSize);
                    textureCordDown = new Vector2f(
                        textureCordUp.X,
                        textureCordUp.Y + caster.CellSize);
                    break;
                case Side.Right:
                    textureCordUp = new Vector2f(
                        RightAtlas.X * caster.CellSize + (original.Position.Y % caster.CellSize),
                        RightAtlas.Y * caster.CellSize);
                    textureCordDown = new Vector2f(
                        textureCordUp.X,
                        textureCordUp.Y + caster.CellSize);
                    break;
                default:
                    textureCordUp = new Vector2f(
                        (original.Position.Y % caster.CellSize),
                        RightAtlas.Y * caster.CellSize);
                    textureCordDown = new Vector2f(
                        textureCordUp.X,
                        textureCordUp.Y + caster.CellSize - 1);
                    break;
            }

            return (textureCordUp, textureCordDown);
        }
    }
}
