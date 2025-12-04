namespace Gfx2d.Resources
{
    /// <summary>
    /// Represents an color in the SDL_PIXELFORMAT_ARGB8888 format.
    /// </summary>
    public class ColorArgb
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

        /// <summary>
        /// Returns a new ColorArgb object that has had a shading level applied to the color.
        /// </summary>
        /// <param name="shading">
        /// A value between 0 and 1, inclusive. A value of 0 indicates no shading whereas a value of 1 indicates complete shading.
        /// In short, this value is the percentage of shading to apply to the color.
        /// </param>
        /// <returns>A new ColorArgb that has had the specified level of shading applied.</returns>
        public ColorArgb ApplyShading(double shading)
        {
            if (shading >= 1) return Black();

            double shadingPercent = 1 - Math.Max(shading, 0);

            return new ColorArgb(
                this.A,
                (int)((double)this.R * shadingPercent),
                (int)((double)this.G * shadingPercent),
                (int)((double)this.B * shadingPercent)
            );
        }

        public static ColorArgb Black() => new ColorArgb(255, 0, 0, 0);
    }
}
