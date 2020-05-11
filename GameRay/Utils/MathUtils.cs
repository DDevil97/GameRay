using SFML.System;
using System;
using static MathFloat.MathF;

namespace GameRay.Utils
{
    public static class MathUtils
    {
        //Constants
        public static float ToDegrees = (float)(180.0f / Math.PI);
        public static float ToRadians = (float)(Math.PI / 180.0f);

        //Static private variables
        private const int trigTableMultiplier = 10;
        private static float[] cosTable = new float[360 * trigTableMultiplier];
        private static float[] sinTable = new float[360 * trigTableMultiplier];

        static MathUtils() 
        {
            for (int i = 0; i < 360 * trigTableMultiplier; i++)
            {
                float angle = (float)i / trigTableMultiplier * ToRadians;
                cosTable[i] = Cos(angle);
                sinTable[i] = Sin(angle);
            }
        }

        //Transformation functions
        public static Vector2f RotateAroundPoint(Vector2f vector, Vector2f origin, float angle)
        {
            angle *= ToRadians;
            vector -= origin;
            return new Vector2f(
                Cos(angle) * vector.X - Sin(angle) * vector.Y,
                Sin(angle) * vector.X + Cos(angle) * vector.Y)
                + origin;
        }
        public static Vector2f Rotate(Vector2f vector, float angle)
        {
            angle *= ToRadians;
            return new Vector2f(
                Cos(angle) * vector.X - Sin(angle) * vector.Y,
                Sin(angle) * vector.X + Cos(angle) * vector.Y);
        }
        public static float Distance(Vector2f a, Vector2f b) => Sqrt(Pow(a.X - b.X, 2) + Pow(a.Y - b.Y, 2));

        //Standar math functions
        public static int FloorInt(float number) => (int)Math.Floor(number);
        public static float Atan2D(float x, float y) => Atan2(y, x) * ToDegrees;
        public static float Atan2D(Vector2f a, Vector2f b) => Atan2(a.Y - b.Y, a.X - b.X) * ToDegrees;
        //public static float CosD(float angle) => Cos(angle * ToRadians);
        //public static float SinD(float angle) => Sin(angle * ToRadians);

        public static float CosD(float angle)
        {
            angle %= 360;
            if (angle < 0)
                angle += 360;

            return cosTable[(int)(angle * trigTableMultiplier)];
        }

        public static float SinD(float angle)
        {
            angle %= 360;
            if (angle < 0)
                angle += 360;

            return sinTable[(int)(angle * trigTableMultiplier)];
        }

        public static float TanD(float angle) => Tan(angle * ToRadians);
        public static float AtanD(float value) => Atan(value)*ToDegrees;
    }
}
