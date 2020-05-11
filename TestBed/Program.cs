using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameRay.Audio;
using GameRay.Elements;
using GameRay.Graphics;
using GameRay.MapData;
using GameRay.MapData.Collision;
using GameRay.Utils;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using static GameRay.Utils.MathUtils;
using static MathFloat.MathF;
using Sprite = GameRay.Elements.Sprite;

namespace TestBed
{
    class Program
    {
        static void Main(string[] args)
        {
            MySFMLProgram app = new MySFMLProgram();
            app.StartSFMLProgram();
        }
    }

    class MySFMLProgram
    {
        RenderTexture rs;
        RenderWindow window;
        Font font;
        RayCaster caster;
        float angle = 0;
        

        float fov = 90;

        int[,] _m = {
        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,2,0,0,0,0,2,0,0,0,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,1,1,1,1,0,1,1,1,1,1},
        {1,0,0,0,0,0,1,6,6,6,6,6,6,6,6,1},
        {1,0,0,0,0,1,1,6,6,6,6,6,6,6,6,1},
        {1,0,0,0,0,0,1,6,6,6,6,6,6,6,6,1},
        {1,0,0,0,0,0,1,6,6,6,6,6,6,6,6,1},
        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
        };

        public void StartSFMLProgram()
        {
            #region Initialization      
            Map Map = new Map(_m.GetLength(0), _m.GetLength(1)); 

            for (int y = 0; y < _m.GetLength(1); y++)
                for (int x = 0; x < _m.GetLength(0); x++)
                    switch (_m[y, x])
                    {
                        case 1:
                            Map[x, y] = new Tile
                            {
                                Solid = true,
                                DownAtlas = new Vector2i(1, 0),
                                UpAtlas = new Vector2i(1, 0),
                                LeftAtlas = new Vector2i(1, 0),
                                RightAtlas = new Vector2i(1, 0)
                            };
                            break;
                        case 2:
                            Map[x, y] = new CircleTile
                            {
                                Solid = true,
                                DownAtlas = new Vector2i(1, 0),
                                UpAtlas = new Vector2i(1, 0),
                                LeftAtlas = new Vector2i(1, 0),
                                RightAtlas = new Vector2i(1, 0),
                                CeilAtlas = new Vector2i(0, 0),
                                FloorAtlas = new Vector2i(2, 0),
                                //IsCeilMap = true

                            };
                            break;
                        case 5:
                            Map[x, y] = new Tile
                            {
                                Solid = false,
                                DownAtlas = new Vector2i(0, 0),
                                UpAtlas = new Vector2i(0, 0),
                                LeftAtlas = new Vector2i(1, 0),
                                RightAtlas = new Vector2i(1, 0),
                                CeilAtlas = new Vector2i(0, 0),
                                FloorAtlas = new Vector2i(5, 0)
                            };
                            break;
                        case 0:
                            Map[x, y] = new Tile
                            {
                                Solid = false,
                                DownAtlas = new Vector2i(0, 0),
                                UpAtlas = new Vector2i(0, 0),
                                LeftAtlas = new Vector2i(1, 0),
                                RightAtlas = new Vector2i(1, 0),
                                CeilAtlas = new Vector2i(0, 0),
                                FloorAtlas = new Vector2i(5, 0)
                            };
                            break;
                        case 6:
                                Map[x, y] = new Tile
                                {
                                    Solid = false,
                                    DownAtlas = new Vector2i(0, 0),
                                    UpAtlas = new Vector2i(0, 0),
                                    LeftAtlas = new Vector2i(1, 0),
                                    RightAtlas = new Vector2i(1, 0),
                                    CeilAtlas = new Vector2i(0, 0),
                                    FloorAtlas = new Vector2i(5, 0),
                                    IsCeilMap = true
                                };
                            break;
                    }


            window = new RenderWindow(new VideoMode(800, 600), "SFML window", Styles.Default);
            window.SetVisible(true);
            window.Closed += new EventHandler(OnClosed);
            window.KeyPressed += new EventHandler<KeyEventArgs>(OnKeyPressed);
            window.MouseMoved += Window_MouseMoved;

            //Vector2f screen = new Vector2f(200, 100);

            Vector2f screen = new Vector2f(window.Size.X, window.Size.Y);
            screen *= 0.3f;
            rs = new RenderTexture((uint)screen.X, (uint)screen.Y);

            caster = new RayCaster(Map, 32);

            Renderer ren = new Renderer(caster,window,rs,Map,fov,new Color(16,16,16));
            ren.Textures.Add(new Texture("Texture.png"));
            ren.MapAtlasInUse = 0;
            #endregion

            Vector2f player = new Vector2f(caster.CellSize * 6 + 8, caster.CellSize * 5 + 8);
            Vector2f sp1 = player + new Vector2f(70, 15);
            Vector2f sp2 = player + new Vector2f(50, 70);

            Vector2f sp3 = new Vector2f(caster.CellSize * 6 + 8, caster.CellSize * 5 + 8) + new Vector2f(30, -30);
            Vector2f scen = sp1 + new Vector2f(60, 60);

            Vector2f M;

            font = new Font("Perfect DOS VGA 437 Win.ttf");
            Text t = new Text("Fps: ", font, 16);
            int fps = 0;
            int fpsCounter = 0;
            int ticks = Environment.TickCount;
            int ticksFps = Environment.TickCount;


            int timeDelta = 0;

            var lamps = new List<Light>();
                //{
                //    new Light (
                //        new Vector2f(sp1.X,sp1.Y),
                //        new Color(255,255,255)
                //    ),
                //    new Light (
                //        new Vector2f(sp2.X,sp2.Y),
                //        new Color(128,128,0)
                //    )
                //};

            

            List<Sprite> sprites = new List<Sprite>();
            Random r = new Random();

            for (int i = 0; i < 5; i++)
                sprites.Add(new Sprite
                (
                    new Vector2f((float)r.NextDouble() * caster.CellSize * Map.Tiles.GetLength(0), (float)r.NextDouble() * caster.CellSize * Map.Tiles.GetLength(1)),
                    new Vector2i(3, 0)
                ));
            Random rand = new Random();
            lamps = sprites.Select(s => new Light(s.Position, new Color((byte)(rand.NextDouble()*80), 0, (byte)(rand.NextDouble() * 80)))).ToList();


            ren.GenerateLightMap(lamps, 0.5f);
            //sprites.Add(new Sprite
            //(
            //    sp2,
            //    new Vector2i(3, 0)        
            //));

            //Sprite p = new Sprite
            //(
            //    sp1,
            //    new Vector2i(3, 0)          
            //);
            //sprites.Add(p);

            AudioSystem audio = new AudioSystem(32, Map, caster);
            Sprite playerSprite = new Sprite(player,new Vector2i(0,0));
            audio.Listener = playerSprite;
            int a = audio.LoadSound("ts.wav");
            audio.PlaySound(a, true, sp1);

            float look = rs.Size.Y / 2;
            float height = caster.CellSize / 2;
            ren.SkyPosition = new Vector2f(5,5);
            ren.SkyAtlas = new Vector2i(6,0);

            window.MouseWheelScrolled += (o, e) =>
            {
                height += e.Delta / 1;
                if (height > caster.CellSize)
                    height = caster.CellSize;

                if (height < 0)
                    height = 0;
            };

            while (window.IsOpen)
            {
                if (Environment.TickCount - ticksFps >= timeDelta)
                {
                    ticksFps = Environment.TickCount;
                    if (Environment.TickCount - ticks >= 1000)
                    {
                        fps = fpsCounter;
                        fpsCounter = 0;
                        ticks = Environment.TickCount;
                    }

                    angle -= (window.Size.X / 2 - Mouse.GetPosition(window).X) / 4f;
                    look += (window.Size.Y / 2 - Mouse.GetPosition(window).Y) / 3f;

                    if (look < 0)
                        look = 0;
                    if (look > rs.Size.Y)
                        look = rs.Size.Y;
                    
                    ren.SkyPosition += new Vector2f(0.01f,0.05f);


                    if (Keyboard.IsKeyPressed(Keyboard.Key.P))
                        lamps.Add(
                            new Light(player, new Color(255,255,255)));

                    angle -= Keyboard.IsKeyPressed(Keyboard.Key.Left) ? 2 : 0;
                    angle += Keyboard.IsKeyPressed(Keyboard.Key.Right) ? 2 : 0;

                    M = new Vector2f(0, 0);

                    if (Keyboard.IsKeyPressed(Keyboard.Key.W))
                        M += new Vector2f(CosD(angle) * 2, SinD(angle) * 2);

                    if (Keyboard.IsKeyPressed(Keyboard.Key.S))
                        M -= new Vector2f(CosD(angle) * 2, SinD(angle) * 2);

                    if (Keyboard.IsKeyPressed(Keyboard.Key.D))
                        M += new Vector2f(CosD(angle + 90) * 2, SinD(angle + 90) * 2);

                    if (Keyboard.IsKeyPressed(Keyboard.Key.A))
                        M += new Vector2f(CosD(angle - 90) * 2, SinD(angle - 90) * 2);


                    RayResult R = caster.RayCast(player, 0);
                    if (R.Magnitude < Math.Abs(M.X) + 10 && Math.Sign(M.X) == 1)
                        M.X = 0;

                    R = caster.RayCast(player, 180);
                    if (R.Magnitude < Math.Abs(M.X) + 10 && Math.Sign(M.X) == -1)
                        M.X = 0;

                    R = caster.RayCast(player, 90);
                    if (R.Magnitude < Math.Abs(M.Y) + 10 && Math.Sign(M.Y) == 1)
                        M.Y = 0;

                    R = caster.RayCast(player, 270);
                    if (R.Magnitude < Math.Abs(M.Y) + 10 && Math.Sign(M.Y) == -1)
                        M.Y = 0;

                    player += M;



                    Mouse.SetPosition(new Vector2i((int)window.Size.X / 2, (int)window.Size.Y / 2),window);

                    window.DispatchEvents();
                    rs.Clear(Color.Black);


                                        
                    
                    playerSprite.Position = player;
                    playerSprite.Angle = angle;
                    audio.UpdateAudio();
                    ren.Render(player, angle, height, look, sprites, lamps);
                    t.DisplayedString = $"Fps: {fps} - Lights: {lamps.Count}";
                    rs.Draw(t);
                    ren.ShowBuffer();
                    Thread.Sleep(0);

                    fpsCounter++;
                }
            }
        }

        //private void RenderMiniMap()

        private void Window_MouseMoved(object sender, MouseMoveEventArgs e)
        {
        }

        #region Event listeners


        void OnClosed(object sender, EventArgs e)
        {
            window.Close();
        }

        void OnKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Escape)
                window.Close();
        }
        #endregion
    }
}

