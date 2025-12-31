using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Gfx2d.Resources
{
    public class MapTexture
    {
        public const int Width = 64;
        public const int Height = 64;

        /// <summary>
        /// A human-friendly name for the resource.
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        public ColorArgb[][] NorthBitmap;
        public ColorArgb[][] EastBitmap;
        public ColorArgb[][] SouthBitmap;
        public ColorArgb[][] WestBitmap;

        public MapTexture()
        {
            NorthBitmap = new ColorArgb[Width][];
            EastBitmap = new ColorArgb[Width][];
            SouthBitmap = new ColorArgb[Width][];
            WestBitmap = new ColorArgb[Width][];

            for (int x = 0; x < Width; x++)
            {
                NorthBitmap[x] = new ColorArgb[Height];
                EastBitmap[x] = new ColorArgb[Height];
                SouthBitmap[x] = new ColorArgb[Height];
                WestBitmap[x] = new ColorArgb[Height];

                for (int y = 0; y < Height; y++)
                {
                    NorthBitmap[x][y] = ColorArgb.White();
                    EastBitmap[x][y] = ColorArgb.White();
                    SouthBitmap[x][y] = ColorArgb.White();
                    WestBitmap[x][y] = ColorArgb.White();
                }
            }
        }

        public ColorArgb? GetRepresentativeColor(ResourceSide side = ResourceSide.N)
        {
            return side switch
            {
                ResourceSide.E => GetRepresentativeColor(EastBitmap),
                ResourceSide.S => GetRepresentativeColor(SouthBitmap),
                ResourceSide.W => GetRepresentativeColor(WestBitmap),
                _ => GetRepresentativeColor(NorthBitmap),
            };
        }

        public ColorArgb? GetRepresentativeColor(ColorArgb[][] bitmap)
        {
            if (bitmap == null) return null;

            long totalA = 0;
            long totalR = 0;
            long totalG = 0;
            long totalB = 0;

            int pixelCount = Width * Height;

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    ColorArgb pixel = bitmap[x][y];
                    totalA += pixel.A;
                    totalR += pixel.R;
                    totalG += pixel.G;
                    totalB += pixel.B;
                }
            }

            return new ColorArgb(
                (byte)(totalA / pixelCount),
                (byte)(totalR / pixelCount),
                (byte)(totalG / pixelCount),
                (byte)(totalB / pixelCount)
            );
        }

        /// <summary>
        /// Returns the colors in the texture at the specified side and column.
        /// </summary>
        public ColorArgb[] GetColorColumn(ResourceSide side, int x)
        {
            return side switch
            {
                ResourceSide.E => EastBitmap[x],
                ResourceSide.S => SouthBitmap[x],
                ResourceSide.W => WestBitmap[x],
                _ => NorthBitmap[x],
            };
        }

        internal static void Initialize(ColorArgb[][] bitmap, Image<Rgba32> image)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));
            if (image.Width != Width || image.Height != Height) throw new ArgumentException($"Texture image must be {Width}x{Height} pixels.");

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Rgba32 pixel = image[x, y];

                    bitmap[x][y] = new ColorArgb(
                        pixel.A,
                        pixel.R,
                        pixel.G,
                        pixel.B
                    );
                }
            }
        }

        public static MapTexture Create(string folder, TextureData textureData)
        {
            MapTexture texture = new();

            texture.Name = textureData.Name;

            if (!string.IsNullOrEmpty(textureData.NorthFileName))
            {
                string fullPath = Path.Combine(folder, textureData.NorthFileName);
                if (!File.Exists(fullPath)) throw new FileNotFoundException("North texture file not found.", fullPath);

                using (Image<Rgba32> image = Image.Load<Rgba32>(fullPath)) Initialize(texture.NorthBitmap, image);
            }

            if (!string.IsNullOrEmpty(textureData.EastFileName))
            {
                string fullPath = Path.Combine(folder, textureData.EastFileName);
                if (!File.Exists(fullPath)) throw new FileNotFoundException("East texture file not found.", fullPath);

                using (Image<Rgba32> image = Image.Load<Rgba32>(fullPath)) Initialize(texture.EastBitmap, image);
            }

            if (!string.IsNullOrEmpty(textureData.SouthFileName))
            {
                string fullPath = Path.Combine(folder, textureData.SouthFileName);
                if (!File.Exists(fullPath)) throw new FileNotFoundException("South texture file not found.", fullPath);

                using (Image<Rgba32> image = Image.Load<Rgba32>(fullPath)) Initialize(texture.SouthBitmap, image);
            }

            if (!string.IsNullOrEmpty(textureData.WestFileName))
            {
                string fullPath = Path.Combine(folder, textureData.WestFileName);
                if (!File.Exists(fullPath)) throw new FileNotFoundException("West texture file not found.", fullPath);

                using (Image<Rgba32> image = Image.Load<Rgba32>(fullPath)) Initialize(texture.WestBitmap, image);
            }

            return texture;
        }
    }
}
