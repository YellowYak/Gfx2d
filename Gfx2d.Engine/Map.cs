using Gfx2d.Resources;

namespace Gfx2d.Engine
{
    /// <summary>
    /// Defines a map in the game. A map is modeled as a 2d array of "tiles," each of which is a square with identical lengths.
    /// A tile can be a "wall" or a "floor." Every wall tile has the same defined height.
    /// </summary>
    internal class Map
    {
        /// <summary>
        /// The dimensions of each tile on the map. All tiles on the map are a square.
        /// </summary>
        public double TileWidth => this.LevelData.Dimensions.TileWidth;
        /// <summary>
        /// The height of all walls on the map.
        /// </summary>
        public double WallHeight => this.LevelData.Dimensions.WallHeight;

        private LevelData LevelData { get; set; } = new();

        /// <summary>
        /// The width of the map in number of tiles.
        /// </summary>
        public int Width { get; private set; }
        /// <summary>
        /// The height of the map in number of tiles.
        /// </summary>
        public int Height { get; private set; }

        public void Load(LevelData level, GameState state)
        {
            this.LevelData = level;

            state.PlayerPos = new Point2d(this.LevelData.StartingPosition.PlayerPos);
            state.CameraAngleIndex = state.MathHelpers.GetClosestRotationIndex(this.LevelData.StartingPosition.CameraAngleRads);

            this.Width = this.LevelData.MapTiles.Max(tr => tr.Length);
            this.Height = this.LevelData.MapTiles.Length;
        }

        /// <summary>
        /// Returns details about a tile resource at a specific (x, y) coordinate on the map.
        /// If the tile resource is the floor, null is returned.
        /// Note that the coordinates here are relative to the 2d tile array.
        /// </summary>
        /// <returns>null if the tile resource is the floor, otherwise the wall's resource.</returns>
        public MapTexture? GetMapTileTexture(int x_index, int y_index)
        {
            int resourceId = this.LevelData.MapTiles[y_index][x_index];

            return resourceId == 0 ? null : this.LevelData.GetMapTextures()[resourceId];
        }

        public ColorArgb CeilingColor => this.LevelData.GetCeilingColor();

        public ColorArgb FloorColor => this.LevelData.GetFloorColor();
    }
}