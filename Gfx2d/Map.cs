namespace Gfx2d
{
    internal class Map
    {
        private int[][] tiles = new int[0][];

        private Dictionary<int, MapResource> resources = new();
        private MapResource? ceilingResource;
        private MapResource? floorResource;

        public double TileWidth { get; private set; }
        public double WallHeight { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public Point2d InitialCameraPos { get; private set; } = new Point2d(0, 0);
        public double InitialCameraZ { get; private set; }
        public int CameraAngleIndex { get; set; }

        public void Load(GameState state)
        {
            // For now, hard-code tile width & height
            this.TileWidth = 1.0;
            this.WallHeight = 1.4;

            this.InitialCameraPos = new Point2d(8.35, 2.68);
            this.InitialCameraZ = 0.6f;
            this.CameraAngleIndex = state.MathHelpers.ThreePiOverTwoIndex;

            state.PlayerPos = new Point2d(this.InitialCameraPos);
            state.CameraZ = this.InitialCameraZ;
            state.CameraAngleIndex = this.CameraAngleIndex;

            resources.Clear();

            resources.Add(1, MapResource.Create("Red Wall", ResourceType.Wall, 255, 125, 10, 10, 255, 155, 25, 25, 255, 125, 10, 10, 255, 155, 25, 25));
            resources.Add(2, MapResource.Create("Blue Wall", ResourceType.Wall, 255, 22, 10, 222, 255, 50, 40, 255, 255, 22, 10, 222, 255, 50, 40, 255));
            resources.Add(3, MapResource.Create("Green Wall", ResourceType.Wall, 255, 39, 226, 77, 255, 60, 255, 100, 255, 39, 226, 77, 255, 60, 255, 100));
            resources.Add(4, MapResource.Create("Yellow Wall", ResourceType.Wall, 255, 242, 233, 15, 255, 255, 250, 45, 255, 242, 233, 15, 255, 255, 250, 45));
            resources.Add(5, MapResource.Create("Purple Wall", ResourceType.Wall, 255, 155, 10, 242, 255, 175, 30, 255, 255, 155, 10, 242, 255, 175, 30, 255));

            ceilingResource = MapResource.Create("Ceiling", ResourceType.Ceiling, 255, 40, 40, 40);
            floorResource = MapResource.Create("Floor", ResourceType.Floor, 255, 88, 88, 88);

            this.tiles = new int[10][];
            this.tiles[0] = new int[] { 5, 2, 3, 1, 1, 1, 1, 1, 4, 5, 1 };
            this.tiles[1] = new int[] { 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2 };
            this.tiles[2] = new int[] { 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3 };
            this.tiles[3] = new int[] { 1, 0, 0, 1, 1, 0, 0, 1, 1, 0, 4 };
            this.tiles[4] = new int[] { 2, 0, 2, 0, 0, 0, 0, 0, 1, 0, 5 };
            this.tiles[5] = new int[] { 2, 0, 2, 0, 0, 0, 0, 0, 1, 0, 4 };
            this.tiles[6] = new int[] { 2, 0, 2, 0, 0, 0, 0, 0, 1, 0, 3 };
            this.tiles[7] = new int[] { 2, 0, 0, 1, 5, 5, 5, 1, 1, 0, 2 };
            this.tiles[8] = new int[] { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 };
            this.tiles[9] = new int[] { 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

            this.Width = this.tiles.Max(tr => tr.Length);
            this.Height = this.tiles.Length;
        }

        public MapResource? GetTileResource(int x_offset, int y_offset)
        {
            int resourceId = this.tiles[y_offset][x_offset];

            return resourceId == 0 ? null : this.resources[resourceId];
        }

        public MapResource GetCeilingResource() => ceilingResource!;
        public MapResource GetFloorResource() => floorResource!;
    }
}
