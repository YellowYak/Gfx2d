using Gfx2d.Resources;

namespace Gfx2d
{
    /// <summary>
    /// Defines a map in the game. A map is modeled as a 2d array of "tiles," each of which is a square with identical lengths.
    /// A tile can be a "wall" or a "floor." Every wall tile has the same defined height.
    /// </summary>
    internal class Map
    {
        private int[][] tiles = new int[0][];

        private Dictionary<int, MapResource> resources = new();
        private MapResource? ceilingResource;
        private MapResource? floorResource;

        /// <summary>
        /// The dimensions of each tile on the map. All tiles on the map are a square.
        /// </summary>
        public double TileWidth => this.LevelData.Dimensions!.TileWidth;
        /// <summary>
        /// The height of all walls on the map.
        /// </summary>
        public double WallHeight => this.LevelData.Dimensions!.WallHeight;

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

            resources.Clear();

            // TODO: Have map resources read from JSON file. But where to define the resources? Separate files? Separate single file? In JSON directly?
            // TODO: Have map resources read from JSON file. But where to define the resources? Separate files? Separate single file? In JSON directly?
            // TODO: Have map resources read from JSON file. But where to define the resources? Separate files? Separate single file? In JSON directly?
            // TODO: Have map resources read from JSON file. But where to define the resources? Separate files? Separate single file? In JSON directly?
            // TODO: Have map resources read from JSON file. But where to define the resources? Separate files? Separate single file? In JSON directly?
            // TODO: Have map resources read from JSON file. But where to define the resources? Separate files? Separate single file? In JSON directly?
            // TODO: Have map resources read from JSON file. But where to define the resources? Separate files? Separate single file? In JSON directly?
            // TODO: Have map resources read from JSON file. But where to define the resources? Separate files? Separate single file? In JSON directly?
            // TODO: Have map resources read from JSON file. But where to define the resources? Separate files? Separate single file? In JSON directly?

            resources.Add(1, MapResource.Create("Red Wall", ResourceType.Wall, 255, 125, 10, 10, 255, 155, 25, 25, 255, 125, 10, 10, 255, 155, 25, 25));
            resources.Add(2, MapResource.Create("Blue Wall", ResourceType.Wall, 255, 22, 10, 222, 255, 50, 40, 255, 255, 22, 10, 222, 255, 50, 40, 255));
            resources.Add(3, MapResource.Create("Green Wall", ResourceType.Wall, 255, 39, 226, 77, 255, 60, 255, 100, 255, 39, 226, 77, 255, 60, 255, 100));
            resources.Add(4, MapResource.Create("Yellow Wall", ResourceType.Wall, 255, 242, 233, 15, 255, 255, 250, 45, 255, 242, 233, 15, 255, 255, 250, 45));
            resources.Add(5, MapResource.Create("Purple Wall", ResourceType.Wall, 255, 155, 10, 242, 255, 175, 30, 255, 255, 155, 10, 242, 255, 175, 30, 255));

            ceilingResource = MapResource.Create("Ceiling", ResourceType.Ceiling, 255, 40, 40, 40);
            floorResource = MapResource.Create("Floor", ResourceType.Floor, 255, 88, 88, 88);

            this.tiles = this.LevelData.MapTiles;

            this.Width = this.tiles.Max(tr => tr.Length);
            this.Height = this.tiles.Length;
        }

        /// <summary>
        /// Returns details about a tile resource at a specific (x, y) coordinate on the map.
        /// If the tile resource is the floor, null is returned.
        /// Note that the coordinates here are relative to the 2d tile array.
        /// </summary>
        /// <param name="x_index">The x tile index. Must be greater than or equal to 0 and strictly less than <see cref="Width"/>.</param>
        /// <param name="y_index">The y tile index. Must be greater than or equal to 0 and strictly less than <see cref="Height"/>.</param>
        /// <returns>null if the tile resource is the floor, otherwise the wall's resource.</returns>
        public MapResource? GetTileResource(int x_index, int y_index)
        {
            if (x_index < 0 || x_index >= this.Width) throw new ArgumentOutOfRangeException(nameof(x_index));
            if (y_index < 0 || y_index >= this.Height) throw new ArgumentOutOfRangeException(nameof(x_index));

            int resourceId = this.tiles[y_index][x_index];

            return resourceId == 0 ? null : this.resources[resourceId];
        }

        /// <summary>
        /// The resource associated with the ceiling.
        /// </summary>
        public MapResource GetCeilingResource() => ceilingResource!;

        /// <summary>
        /// The resource associated with the floor.
        /// </summary>
        /// <returns></returns>
        public MapResource GetFloorResource() => floorResource!;
    }
}