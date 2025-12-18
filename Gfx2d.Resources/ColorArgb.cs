namespace Gfx2d.Resources
{
    /// <summary>
    /// Represents an color in the SDL_PIXELFORMAT_ARGB8888 format.
    /// </summary>
    public class ColorArgb
    {
        public byte A;
        public byte R;
        public byte G;
        public byte B;

        public ColorArgb(byte a, byte r, byte g, byte b)
        {
            A = a;
            R = r;
            G = g;
            B = b;
        }

        public ColorArgb(byte[] argbByteArray)
        {
            if (argbByteArray == null) throw new ArgumentNullException(nameof(argbByteArray));
            if (argbByteArray.Length != 4) throw new ArgumentOutOfRangeException(nameof(argbByteArray));

            A = argbByteArray[0];
            R = argbByteArray[1];
            G = argbByteArray[2];
            B = argbByteArray[3];
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
            // TODO: Improve performance by creating a lookup table for shading values rather than calculating on the fly.
            // TODO: Improve performance by creating a lookup table for shading values rather than calculating on the fly.
            // TODO: Improve performance by creating a lookup table for shading values rather than calculating on the fly.
            // TODO: Improve performance by creating a lookup table for shading values rather than calculating on the fly.
            // TODO: Improve performance by creating a lookup table for shading values rather than calculating on the fly.
            // TODO: Improve performance by creating a lookup table for shading values rather than calculating on the fly.
            // TODO: Improve performance by creating a lookup table for shading values rather than calculating on the fly.
            // TODO: Improve performance by creating a lookup table for shading values rather than calculating on the fly.
            // TODO: Improve performance by creating a lookup table for shading values rather than calculating on the fly.

            double shadingPercent = 1 - shading;

            return new ColorArgb(
                this.A,
                (byte)((double)this.R * shadingPercent),
                (byte)((double)this.G * shadingPercent),
                (byte)((double)this.B * shadingPercent)
            );
        }

        public static ColorArgb Black() => new ColorArgb(255, 0, 0, 0);
        public static ColorArgb White() => new ColorArgb(255, 255, 255, 255);
        public static ColorArgb LightGray() => new ColorArgb(255, 222, 222, 222);
    }
}
