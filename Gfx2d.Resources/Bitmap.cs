namespace Gfx2d.Resources
{
    public class Bitmap
    {
        public int Width;
        public int Height;
        private ColorArgb[][] North;
        private ColorArgb[][] East;
        private ColorArgb[][] South;
        private ColorArgb[][] West;

        public Bitmap(TextureData texture)
        {
            Width = texture.Width;
            Height = texture.Height;

            North = new ColorArgb[Width][];
            East = new ColorArgb[Width][];
            South = new ColorArgb[Width][];
            West = new ColorArgb[Width][];

            for (int x = 0; x < Width; x++)
            {
                North[x] = new ColorArgb[Height];
                East[x] = new ColorArgb[Height];
                South[x] = new ColorArgb[Height];
                West[x] = new ColorArgb[Height];

                for (int y = 0; y < Height; y++)
                {
                    North[x][y] = new ColorArgb(
                        (byte)texture.North[y][x][0],
                        (byte)texture.North[y][x][1],
                        (byte)texture.North[y][x][2],
                        (byte)texture.North[y][x][3]
                    );

                    East[x][y] = new ColorArgb(
                        (byte)texture.East[y][x][0],
                        (byte)texture.East[y][x][1],
                        (byte)texture.East[y][x][2],
                        (byte)texture.East[y][x][3]
                    );

                    South[x][y] = new ColorArgb(
                        (byte)texture.South[y][x][0],
                        (byte)texture.South[y][x][1],
                        (byte)texture.South[y][x][2],
                        (byte)texture.South[y][x][3]
                    );

                    West[x][y] = new ColorArgb(
                        (byte)texture.West[y][x][0],
                        (byte)texture.West[y][x][1],
                        (byte)texture.West[y][x][2],
                        (byte)texture.West[y][x][3]
                    );
                }
            }
        }

        /// <summary>
        /// Returns a representative color from the bitmap's North surface (the top-left pixel).
        /// </summary>
        public ColorArgb GetRepresentativeColor() => North[0][0];

        /// <summary>
        /// Returns the color in the Bitmap at the specified side and (x, y) coordinates.
        /// </summary>
        public ColorArgb GetColorAtCoordinates(ResourceSide side, int x, int y)
        {
            return side switch
            {
                ResourceSide.E => East[x][y],
                ResourceSide.S => South[x][y],
                ResourceSide.W => West[x][y],
                _ => North[x][y],
            };
        }

        /// <summary>
        /// Returns the color in the Bitmap at the specified side and (x, y) coordinates.
        /// </summary>
        public ColorArgb[] GetColorColumn(ResourceSide side, int x)
        {
            return side switch
            {
                ResourceSide.E => East[x],
                ResourceSide.S => South[x],
                ResourceSide.W => West[x],
                _ => North[x],
            };
        }
    }
}
