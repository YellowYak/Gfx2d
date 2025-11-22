namespace Gfx2d
{
    /// <summary>
    /// Represents an color in the SDL_PIXELFORMAT_ARGB8888 format.
    /// </summary>
    internal class ColorArgb
    {
        public int A { get; private set; }
        public int R { get; private set; }
        public int G { get; private set; }
        public int B { get; private set; }

        public ColorArgb(int a, int r, int g, int b)
        {
            A = a;
            R = r;
            G = g;
            B = b;
        }

        public static ColorArgb Black() => new ColorArgb(255, 0, 0, 0);
    }
}
