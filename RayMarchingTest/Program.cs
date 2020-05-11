using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Threading;
using static GameRay.Utils.MathUtils;

namespace RayMarchingTest
{
    class Program
    {
        static void Main(string[] args)
        {
            RenderWindow window = new RenderWindow(new VideoMode(1000, 800), "RayMatchingTest");
            World world = new World();
            Texture texture = new Texture("Blank.bmp");
            Font font = new Font("Font.ttf");
            Text fpsLabel = new Text("", font);
            Circle o = new Circle
            {
                Color = Color.Magenta,
                Position = new Vector2f(300, 300),
                Radius = 10
            };
            Box rect = new Box(0, 0, 200, 50);
            Random random = new Random();

            int actualTick = Environment.TickCount;
            int actualFps = 0, fpsCounter = 0;

            window.SetVisible(true);

            for (int i = 0; i < 10; i++)
            {
                switch (random.Next(0, 6))
                {
                    case 0:
                        world.Objects.Add(new HorizontalLine((float)random.NextDouble() * window.Size.X,
                                                             (float)random.NextDouble() * window.Size.Y, random.Next(10, 200)));
                        break;
                    case 1:
                        world.Objects.Add(new VerticalLine((float)random.NextDouble() * window.Size.X,
                                                             (float)random.NextDouble() * window.Size.Y, random.Next(10, 200)));
                        break;
                    case 2:
                        world.Objects.Add(new Circle((float)random.NextDouble() * window.Size.X,
                                                    (float)random.NextDouble() * window.Size.Y, random.Next(10, 100)));
                        break;
                    case 3:
                        world.Objects.Add(new Box((float)random.NextDouble() * window.Size.X,
                                                    (float)random.NextDouble() * window.Size.Y,
                                                    random.Next(10, 100),
                                                    random.Next(10, 100)));
                        break;
                    case 4:
                        world.Objects.Add(new RotatedBox((float)random.NextDouble() * window.Size.X,
                                                    (float)random.NextDouble() * window.Size.Y,
                                                    random.Next(10, 100),
                                                    random.Next(10, 100), 
                                                    random.Next(0, 360)));
                        break;
                    case 5:
                        world.Objects.Add(new Line((float)random.NextDouble() * window.Size.X,
                                                    (float)random.NextDouble() * window.Size.Y,
                                                    (float)random.NextDouble() * window.Size.X,
                                                    (float)random.NextDouble() * window.Size.Y));
                        break;
                }
            }

            rect.Color = Color.Black;
            fpsLabel.Position = new Vector2f(10, 10);
            actualTick = Environment.TickCount;
            while (window.IsOpen)
            {
                window.DispatchEvents();

                if (Environment.TickCount - actualTick >= 1000)
                {
                    actualTick = Environment.TickCount;
                    actualFps = fpsCounter;
                    fpsCounter = 0;
                }
                fpsCounter++;

                if (Keyboard.IsKeyPressed(Keyboard.Key.Escape))
                    window.Close();

                Vector2i mouse = Mouse.GetPosition(window);
                if (Mouse.IsButtonPressed(Mouse.Button.Left))
                {
                    o.Position = new Vector2f(mouse.X, mouse.Y);
                    window.Clear();
                }


                world.Objects.ForEach(obj => obj.Color = Color.White);

                window.Clear();
                foreach (Body body in world.Objects)
                    body.Render(window, texture);

                Collision result = world.CastRay(o.Position, Atan2D(new Vector2f(mouse.X, mouse.Y), o.Position));

                if (result.Object != null)
                    result.Object.Color = Color.Green;

                o.Render(window, texture);
                window.Draw(new Vertex[]
                {
                    new Vertex
                    {
                        Position = result.Position,
                        Color = Color.Red
                    },
                    new Vertex
                    {
                        Position = o.Position,
                        Color = o.Color
                    }
                }, 0, 2, PrimitiveType.Lines, RenderStates.Default);

                rect.Render(window, texture);
                fpsLabel.DisplayedString = $"Fps: {actualFps} - {result.TextureCoord}";
                window.Draw(fpsLabel);
                window.Display();
                Thread.Sleep(0);
            }
        }
    }
}
