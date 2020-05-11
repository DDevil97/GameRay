using SFML.Graphics;

namespace GameRay.Utils
{
    public static class ColorUtil
    {
        public static Color ClampColorByte(float r, float g, float b)
        {
            return new Color
                (
                    (byte)(r > 255 ? 255 : r < 0 ? 0 : r),
                    (byte)(g > 255 ? 255 : g < 0 ? 0 : g),
                    (byte)(b > 255 ? 255 : b < 0 ? 0 : b)
                );
        }
    }
}
