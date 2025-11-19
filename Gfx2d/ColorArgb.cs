namespace Gfx2d
{
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
    }
}
