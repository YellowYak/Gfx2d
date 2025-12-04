using Newtonsoft.Json;

namespace Gfx2d.Resources
{
    public class LevelData
    {
        /// <summary>
        /// Loads a level and its associated resources from a JSON file.
        /// </summary>
        public static LevelData LoadFromFile(string levelDataFilePath)
        {
            if (string.IsNullOrEmpty(levelDataFilePath)) throw new ArgumentNullException(nameof(levelDataFilePath));
            if (!File.Exists(levelDataFilePath)) throw new FileNotFoundException("Level file not found.", levelDataFilePath);

            string folder = Path.GetDirectoryName(levelDataFilePath)!;

            // Read in the level JSON file
            string json = File.ReadAllText(levelDataFilePath);
            var level = JsonConvert.DeserializeObject<LevelData>(json)!;

            // Loop through the level's tile resources and load those in
            level.tileResources.Clear();
            foreach (ResourceReference rr in level.TileResources)
            {
                string fullPath = Path.Combine(folder, rr.FileName);
                if (!File.Exists(fullPath)) throw new FileNotFoundException("Resource file not found.", fullPath);

                string resourceJson = File.ReadAllText(fullPath);
                ResourceData data = JsonConvert.DeserializeObject<ResourceData>(resourceJson)!;

                level.tileResources.Add(
                    rr.Id,
                    MapResource.Create(
                        data.Name,
                        data.North,
                        data.East,
                        data.South,
                        data.West
                    )
                );
            }

            // Read in the level's ceiling & floor resource color data
            level.ceilingResource = MapResource.Create("Ceiling", level.CeilingColor);
            level.floorResource = MapResource.Create("Ceiling", level.FloorColor);

            return level;
        }


        private Dictionary<int, MapResource> tileResources = new();
        public Dictionary<int, MapResource> GetTileResources() => this.tileResources;

        private MapResource floorResource = new();
        public MapResource FloorResource => floorResource;

        private MapResource ceilingResource = new();
        public MapResource CeilingResource => ceilingResource;


        [JsonProperty(Required = Required.Always)]
        public LevelDimensions Dimensions { get; set; } = new();

        [JsonProperty(Required = Required.Always)]
        public LevelStartingPosition StartingPosition { get; set; } = new();

        [JsonProperty(Required = Required.Always)]
        public int[][] MapTiles { get; set; } = Array.Empty<int[]>();

        [JsonProperty(Required = Required.Always)]
        public ResourceReference[] TileResources { get; set; } = Array.Empty<ResourceReference>();

        [JsonProperty(Required = Required.Always)]
        public int[] CeilingColor { get; set; } = Array.Empty<int>();

        [JsonProperty(Required = Required.Always)]
        public int[] FloorColor { get; set; } = Array.Empty<int>();
    }
}
