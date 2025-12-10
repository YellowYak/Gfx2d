using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Gfx2d.Resources
{
    public class LevelData
    {
        /// <summary>
        /// Determines if a specific file is a valid level data file.
        /// </summary>
        /// <returns>True if the specified file exists and can be deserialized without error; false otherwise.</returns>
        public static bool ValidFile(string path)
        {
            try
            {
                string json = File.ReadAllText(path);
                JsonConvert.DeserializeObject<LevelData>(json);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Loads a level and its associated resources from a JSON file.
        /// </summary>
        public static LevelData LoadFromFile(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            if (!File.Exists(path)) throw new FileNotFoundException("Level file not found.", path);

            string folder = Path.GetDirectoryName(path)!;

            // Read in the level JSON file
            string json = File.ReadAllText(path);
            var level = JsonConvert.DeserializeObject<LevelData>(json)!;

            // Loop through the level's tile resources and load those in
            level.mapTileResources.Clear();
            foreach (ResourceReference rr in level.TileResources)
            {
                string fullPath = Path.Combine(folder, rr.FileName);
                if (!File.Exists(fullPath)) throw new FileNotFoundException("Resource file not found.", fullPath);

                string resourceJson = File.ReadAllText(fullPath);
                ResourceData data = JsonConvert.DeserializeObject<ResourceData>(resourceJson)!;

                level.AddMapTileResource(
                    rr.Id,
                    data
                );
            }

            // Read in the level's ceiling & floor resource color data
            level.ceilingResource = MapResource.Create("Ceiling", level.CeilingColor);
            level.floorResource = MapResource.Create("Ceiling", level.FloorColor);

            return level;
        }

        /// <summary>
        /// Serializes the JSON and saves the level data to the specified path.
        /// </summary>
        public void Save(string path)
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        /// <summary>
        /// Returns how many times a specific resource reference ID is used in the map.
        /// </summary>
        public int GetResourceReferenceUsageCount(int resourceRefId)
        {
            int count = 0;

            foreach (var row in this.MapTiles)
                foreach (int cell in row)
                    if (cell == resourceRefId)
                        count++;

            return count;
        }

        /// <summary>
        /// Remove all references to a particular resource from the map tiles.
        /// </summary>
        public void RemoveResourceReferenceFromMap(int resourceRefId)
        {
            for (int i = 0; i < MapTiles.Length; i++)
            {
                int[] row = MapTiles[i];

                for (int j = 0; j < row.Length; j++)
                {
                    if (MapTiles[i][j] == resourceRefId)
                        MapTiles[i][j] = 0;
                }
            }
        }

        private Dictionary<int, MapResource> mapTileResources = new();
        public Dictionary<int, MapResource> GetMapTileResources() => this.mapTileResources;
        public void AddMapTileResource(int resourceRefId, MapResource tileResource)
        {
            mapTileResources.Add(
                resourceRefId,
                tileResource
            );
        }
        public void AddMapTileResource(int resourceRefId, ResourceData resource)
        {
            AddMapTileResource(
                resourceRefId,
                MapResource.Create(resource)
            );
        }

        private MapResource floorResource = new();
        [JsonIgnore]
        public MapResource FloorResource => floorResource;

        private MapResource ceilingResource = new();
        [JsonIgnore]
        public MapResource CeilingResource => ceilingResource;


        [JsonProperty("name")]
        public string Name { get; set; } = "New level";
        
        [JsonProperty("description")]
        public string Description { get; set; } = string.Empty;

        [JsonProperty("dimensions", Required = Required.Always)]
        public LevelDimensions Dimensions { get; set; } = new();

        [JsonProperty("startingPosition", Required = Required.Always)]
        public LevelStartingPosition StartingPosition { get; set; } = new();

        [JsonProperty("mapTiles", Required = Required.Always)]
        public int[][] MapTiles { get; set; } = {
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        };

        [JsonProperty("tileResources", Required = Required.Always)]
        public List<ResourceReference> TileResources { get; set; } = new();

        [JsonProperty("ceilingColor", Required = Required.Always)]
        public byte[] CeilingColor { get; set; } = Array.Empty<byte>();

        [JsonProperty("floorColor", Required = Required.Always)]
        public byte[] FloorColor { get; set; } = Array.Empty<byte>();
    }
}
