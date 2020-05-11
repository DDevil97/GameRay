using GameRay.Elements;
using GameRay.MapData;
using GameRay.MapData.Collision;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static GameRay.Utils.MathUtils;
using static MathFloat.MathF;
using static GameRay.Utils.ColorUtil;
using Sprite = GameRay.Elements.Sprite;
using System.Threading;
using System.IO;

namespace GameRay.Graphics
{
    public class Renderer
    {
        //Private variables
        private float HalfFov;
        private float DistanceToProjectionPlane;
        private float[] Angles;
        private float[] DepthPerStrip;
        private float LightMultiplier;
        private Color[,] LightMap;
        private Vertex[] DisplayVertices;
        private Vertex[] FloorVertices;
        private Vertex[] WallVertices;

        //Toggles
        private bool DynamicLight = false;
        private bool DrawFloors = true;

        //Read-only properties
        public RenderWindow Screen { get; internal set; }
        public float Fov { get; internal set; }
        public float LightMapScaler { get; internal set; }
        public RayCaster Caster { get; internal set; }
        public Vector2f AtlasTileSize { get; internal set; }
        public RenderTexture Buffer { get; internal set; }
        public Map Map { get; internal set; }

        //Standar properties
        public int MapAtlasInUse { get; set; }
        public Color AmbientLight { get; set; }
        public List<Texture> Textures { get; set; }
        public Vector2f SkyPosition { get; set; }
        public Vector2i SkyAtlas { get; set; }

        //Private methods
        private float CalculateLampIntensityAtPoint(Vector2f mPos, Vector2f lPos)
        {
            RayResult r = Caster.RayCast(lPos, Atan2D(mPos, lPos));
            if (r.Magnitude + 1 >= Distance(mPos, lPos))
                return 1f / ((float)Math.Pow(Distance(mPos, lPos), 2) / LightMultiplier);
            else
                return 0;
        }

        private void CalculateCeil(Vector2f player, float angle, int y, float height, float look, ref Vertex[] points, ref List<Light> lamps)
        {
            if (DrawFloors)
            {
                float dist = (Caster.CellSize - height) / (look - y) * DistanceToProjectionPlane / CosD(HalfFov);
                Vector2f left = player + new Vector2f(dist * CosD(angle - HalfFov), dist * SinD(angle - HalfFov));
                Vector2f right = player + new Vector2f(dist * CosD(angle + HalfFov), dist * SinD(angle + HalfFov));
                Vector2f delta = (right - left) / Buffer.Size.X;

                Vector2f skyLeft = SkyPosition + new Vector2f(dist * CosD(angle - HalfFov), dist * SinD(angle - HalfFov));

                for (int x = 0; x < Buffer.Size.X; x++)
                {

                    Color col;

                    if (!DynamicLight)
                    {
                        int pX = (int)(left.X * LightMapScaler);
                        if (pX >= LightMap.GetLength(0)) pX = LightMap.GetLength(0) - 1;
                        if (pX < 0) pX = 0;

                        int pY = (int)(left.Y * LightMapScaler);
                        if (pY >= LightMap.GetLength(0)) pY = LightMap.GetLength(1) - 1;
                        if (pY < 0) pY = 0;
                        col = LightMap[pX, pY];
                    }
                    else
                        col = getLightAtPoint(left, lamps);

                    Tile t = Map[FloorInt(left.X / Caster.CellSize), FloorInt(left.Y / Caster.CellSize)];

                    if (!t.IsCeilMap)
                    {
                        points[y * Buffer.Size.X + x] = new Vertex
                        {
                            Position = new Vector2f(x, y),
                            TexCoords = new Vector2f(t.CeilAtlas.X * Caster.CellSize + (left.X % Caster.CellSize), t.CeilAtlas.Y * Caster.CellSize + left.Y % Caster.CellSize),
                            Color = col
                        };
                    }
                    else
                    {
                        points[y * Buffer.Size.X + x] = new Vertex
                        {
                            Position = new Vector2f(x, y),
                            TexCoords = new Vector2f(SkyAtlas.X * Caster.CellSize + (skyLeft.X % Caster.CellSize), SkyAtlas.Y * Caster.CellSize + skyLeft.Y % Caster.CellSize),
                            Color = new Color(128, 128, 128)
                        };
                    }

                    left += delta;
                    skyLeft += delta;
                    skyLeft.X %= Caster.CellSize;
                    if (skyLeft.X < 0)
                        skyLeft.X += Caster.CellSize;
                    skyLeft.Y %= Caster.CellSize;
                    if (skyLeft.Y < 0)
                        skyLeft.Y += Caster.CellSize;
                }
            }
        }

        private void CalculateFloor(Vector2f player, float angle, int y, float height, float look, ref Vertex[] points, ref List<Light> lamps)
        {
            if (DrawFloors)
            {
                float dist = (height / (y - (look)) * DistanceToProjectionPlane) / CosD(HalfFov);
                Vector2f left = player + new Vector2f(dist * CosD(angle - HalfFov), dist * SinD(angle - HalfFov));
                Vector2f right = player + new Vector2f(dist * CosD(angle + HalfFov), dist * SinD(angle + HalfFov));
                Vector2f delta = (right - left) / Buffer.Size.X;

                for (int x = 0; x < Buffer.Size.X; x++)
                {

                    Color col;

                    if (!DynamicLight)
                    {
                        int pX = (int)(left.X * LightMapScaler);
                        if (pX >= LightMap.GetLength(0)) pX = LightMap.GetLength(0) - 1;
                        if (pX < 0) pX = 0;

                        int pY = (int)(left.Y * LightMapScaler);
                        if (pY >= LightMap.GetLength(0)) pY = LightMap.GetLength(1) - 1;
                        if (pY < 0) pY = 0;
                        col = LightMap[pX, pY];
                    }
                    else
                        col = getLightAtPoint(left, lamps);

                    Tile t = Map[FloorInt(left.X / Caster.CellSize), FloorInt(left.Y / Caster.CellSize)];
                    points[y * Buffer.Size.X + x] = new Vertex
                    {
                        Position = new Vector2f(x, y),
                        TexCoords = new Vector2f(t.FloorAtlas.X * Caster.CellSize + (left.X % Caster.CellSize), t.FloorAtlas.Y * Caster.CellSize + left.Y % Caster.CellSize),
                        Color = col
                    };

                    left += delta;
                }


            }
        }

        private Color getLightAtPoint(Vector2f point, List<Light> lamps)
        {
            float r = 0, g = 0, b = 0;
            Color ret = AmbientLight;

            foreach (Light l in lamps)
            {
                float i = CalculateLampIntensityAtPoint(point, l.Position);
                r += i * l.Color.R;
                g += i * l.Color.G;
                b += i * l.Color.B;
                ret = ClampColorByte(ret.R + r, ret.G + g, ret.B + b);
            }

            return ret;
        }

        public Renderer(RayCaster rayCaster, RenderWindow renderWindow, RenderTexture drawBuffer, Map map, float fov, Color ambientLight)
        {
            //Assign local variables
            Caster = rayCaster;
            Screen = renderWindow;
            Buffer = drawBuffer;
            Map = map;
            Fov = fov;
            AmbientLight = ambientLight;

            //Calculate inital values for some variables
            HalfFov = fov / 2;
            DistanceToProjectionPlane = (Buffer.Size.X / 2f) / TanD(HalfFov);
            Textures = new List<Texture>();
            DepthPerStrip = new float[Buffer.Size.X];
            AtlasTileSize = new Vector2f(Caster.CellSize, Caster.CellSize);
            Angles = new float[Buffer.Size.X];
            LightMultiplier = 4000;

            for (int x = 0; x < Buffer.Size.X; x++)
                Angles[x] = AtanD((x - Buffer.Size.X / 2.0f) / DistanceToProjectionPlane);

            FloorVertices = new Vertex[Buffer.Size.X * Buffer.Size.Y];
            WallVertices = new Vertex[Buffer.Size.X * 2];

            float aspectRatio = Buffer.Size.X / (float)Buffer.Size.Y;
            DisplayVertices = new Vertex[] {
                    new Vertex
                    {
                        Position = new Vector2f(Screen.Size.X / 2 - (Screen.Size.Y*aspectRatio)/2,0),
                        TexCoords = new Vector2f(0,Buffer.Size.Y-1),
                        Color = Color.White
                    },
                    new Vertex
                    {
                        Position = new Vector2f(Screen.Size.X / 2 - (Screen.Size.Y*aspectRatio)/2,Screen.Size.Y-1),
                        TexCoords = new Vector2f(0,0),
                        Color = Color.White
                    },
                    new Vertex
                    {
                        Position = new Vector2f(Screen.Size.X / 2 + (Screen.Size.Y*aspectRatio)/2,Screen.Size.Y-1),
                        TexCoords = new Vector2f(Buffer.Size.X-1,0),
                        Color = Color.White
                    },
                    new Vertex
                    {
                        Position = new Vector2f(Screen.Size.X / 2 + (Screen.Size.Y*aspectRatio)/2,0),
                        TexCoords = new Vector2f(Buffer.Size.X-1,Buffer.Size.Y-1),
                        Color = Color.White
                    },
                };
        }

        //Public interface
        public void GenerateLightMap(List<Light> lamps, float lightMapScaler)
        {
            if (!DynamicLight)
            {
                LightMapScaler = lightMapScaler;
                LightMap = new Color[FloorInt(Caster.CellSize * Map.Tiles.GetLength(0) * LightMapScaler), FloorInt(Caster.CellSize * Map.Tiles.GetLength(1) * LightMapScaler)];
                for (int y = 0; y < LightMap.GetLength(1); y++)
                    for (int x = 0; x < LightMap.GetLength(0); x++)
                        LightMap[x, y] = AmbientLight;

                for (int y = 0; y < LightMap.GetLength(1); y++)
                    for (int x = 0; x < LightMap.GetLength(0); x++)
                    {
                        float r = 0, g = 0, b = 0;
                        foreach (Light l in lamps)
                        {
                            float i = CalculateLampIntensityAtPoint(new Vector2f(x / LightMapScaler, y / LightMapScaler), l.Position);
                            r += i * l.Color.R;
                            g += i * l.Color.G;
                            b += i * l.Color.B;
                            LightMap[x, y] = ClampColorByte(LightMap[x, y].R + r, LightMap[x, y].G + g, LightMap[x, y].B + b);
                        }
                    }

                var test = File.Open("D:/Map.bin", FileMode.OpenOrCreate);
                test.Write(BitConverter.GetBytes(LightMap.GetLength(0)), 0, 4);
                test.Write(BitConverter.GetBytes(LightMap.GetLength(1)), 0, 4);

                for (int y = 0; y < LightMap.GetLength(1); y++)
                    for (int x = 0; x < LightMap.GetLength(0); x++)
                        test.Write(BitConverter.GetBytes(LightMap[x, y].ToInteger()), 0, 4);

                test.Flush();
                test.Dispose();
            }
        }

        public void Render(Vector2f player, float angle, float height, float look, List<Sprite> sprites, List<Light> lamps)
        {
            List<Vertex> spritesLines = new List<Vertex>();

            ParallelLoopResult floorResult = Parallel.For((int)look, (int)Buffer.Size.Y, i => CalculateFloor(player, angle, i, height, look, ref FloorVertices, ref lamps));
            ParallelLoopResult ceilResult = Parallel.For(0, (int)look - 1, i => CalculateCeil(player, angle, i, height, look, ref FloorVertices, ref lamps));


            for (int x = 0; x < Buffer.Size.X; x++)
            {
                float rayAngle = Angles[x];
                RayResult ray = Caster.RayCast(player, angle + rayAngle);
                Tile t = Map[ray.Tile.X, ray.Tile.Y];

                (Vector2f textureCordUp, Vector2f textureCordDown) = t.CalculateTextureCoords(ray, player, angle, Caster);

                Color col;

                if (!DynamicLight)
                {
                    int pX = (int)(ray.Position.X * LightMapScaler);
                    if (pX >= LightMap.GetLength(0)) pX = LightMap.GetLength(0) - 1;
                    if (pX < 0) pX = 0;

                    int pY = (int)(ray.Position.Y * LightMapScaler);
                    if (pY >= LightMap.GetLength(1)) pY = LightMap.GetLength(1) - 1;
                    if (pY < 0) pY = 0;
                    col = LightMap[pX, pY];
                }
                else
                    col = getLightAtPoint(ray.Position, lamps);


                DepthPerStrip[x] = ray.Magnitude * CosD(rayAngle);

                var ratio = DistanceToProjectionPlane / DepthPerStrip[x];
                var bottomOfWall = (ratio * height + look);
                var scale = (ratio * Caster.CellSize);
                var topOfWall = bottomOfWall - scale;

                WallVertices[x << 1] = new Vertex
                {
                    Position = new Vector2f(x, bottomOfWall),
                    Color = col,
                    TexCoords = textureCordUp
                };
                WallVertices[(x << 1) + 1] = new Vertex
                {
                    Position = new Vector2f(x, topOfWall),
                    Color = col,
                    TexCoords = textureCordDown
                };

            }


            List<Sprite> finalList = new List<Sprite>();
            foreach (Sprite spr in sprites)
            {
                if (!DynamicLight)
                {
                    int pX = (int)(spr.Position.X * LightMapScaler);
                    if (pX >= LightMap.GetLength(0)) pX = LightMap.GetLength(0) - 1;
                    if (pX < 0) pX = 0;

                    int pY = (int)(spr.Position.Y * LightMapScaler);
                    if (pY >= LightMap.GetLength(0)) pY = LightMap.GetLength(1) - 1;
                    if (pY < 0) pY = 0;

                    spr.Light = LightMap[pX, pY];
                }
                else
                    spr.Light = getLightAtPoint(spr.Position, lamps);

                spr.TransformedPosition = Rotate(spr.Position - player, -angle);
                spr.DistanceToPlayer = spr.TransformedPosition.X;
                if (spr.DistanceToPlayer > 0)
                    finalList.Add(spr);
            }

            finalList = finalList.OrderByDescending(s => s.TransformedPosition.X).ToList();

            foreach (Sprite spr in finalList)
            {

                int lineHeight = FloorInt((Caster.CellSize / spr.DistanceToPlayer) * DistanceToProjectionPlane);
                int px = (int)(Buffer.Size.X / 2 + (spr.TransformedPosition.Y / spr.DistanceToPlayer) * DistanceToProjectionPlane);
                for (int x = 0; x < lineHeight; x++)
                {
                    int posX = (px + lineHeight / 2) - x;
                    if (posX >= 0 && posX < Buffer.Size.X && DepthPerStrip[posX] > spr.DistanceToPlayer)
                    {
                        float tex = (float)x * (Caster.CellSize) / lineHeight;

                        Vector2f textureCordUp = new Vector2f(
                            (spr.AtlasTexture.X + 1) * Caster.CellSize - 0.00001f - tex,
                            spr.AtlasTexture.Y * Caster.CellSize);

                        Vector2f textureCordDown = new Vector2f(
                            textureCordUp.X,
                            textureCordUp.Y + Caster.CellSize);

                        var ratio = DistanceToProjectionPlane / spr.DistanceToPlayer;
                        var bottomOfWall = (ratio * height + look);
                        var scale = (ratio * Caster.CellSize);
                        var topOfWall = bottomOfWall - scale;

                        spritesLines.Add(new Vertex
                        {
                            Position = new Vector2f(posX, topOfWall),
                            Color = spr.Light,
                            TexCoords = textureCordUp
                        });
                        spritesLines.Add(new Vertex
                        {
                            Position = new Vector2f(posX, bottomOfWall),
                            Color = spr.Light,
                            TexCoords = textureCordDown
                        });
                    }
                }
            }

            while (!floorResult.IsCompleted)
                Thread.Sleep(0);

            while (!ceilResult.IsCompleted)
                Thread.Sleep(0);

            Buffer.Draw(FloorVertices, 0, (uint)FloorVertices.Length, PrimitiveType.Points, new RenderStates(Textures[MapAtlasInUse]));
            Buffer.Draw(WallVertices, 0, (uint)WallVertices.Length, PrimitiveType.Lines, new RenderStates(Textures[MapAtlasInUse]));
            Buffer.Draw(spritesLines.ToArray(), 0, (uint)spritesLines.Count, PrimitiveType.Lines, new RenderStates(Textures[MapAtlasInUse]));
        }

        public void ShowBuffer()
        {
            Screen.Draw(DisplayVertices, 0, 4, PrimitiveType.Quads, new RenderStates(Buffer.Texture));
            Screen.Display();
        }
    }
}
