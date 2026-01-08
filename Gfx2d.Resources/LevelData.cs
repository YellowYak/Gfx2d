using Gfx2d.Resources.Serialization;
using Newtonsoft.Json;

namespace Gfx2d.Resources
{
    public class LevelData
    {
        /// <summary>
        /// Determines if a specific file is a valid level data file.
        /// </summary>
        /// <returns>True if the specified file exists and can be deserialized without error; false otherwise.</returns>
        public static bool ValidFile(string levelFilePath)
        {
            try
            {
                string json = File.ReadAllText(levelFilePath);
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
        public static LevelData LoadFromFile(string levelFilePath)
        {
            if (string.IsNullOrEmpty(levelFilePath)) throw new ArgumentNullException(nameof(levelFilePath));
            if (!File.Exists(levelFilePath)) throw new FileNotFoundException("Level file not found.", levelFilePath);

            string folder = Path.GetDirectoryName(levelFilePath)!;

            // Read in the level JSON file
            string json = File.ReadAllText(levelFilePath);
            var level = JsonConvert.DeserializeObject<LevelData>(json)!;

            // Loop through the level's map textures and load those in
            level.textureMap.Clear();
            foreach (TextureReference rr in level.TextureReferences)
            {
                string fullPath = Path.Combine(folder, rr.FileName);
                if (!File.Exists(fullPath)) throw new FileNotFoundException("Texture reference file not found.", fullPath);

                string resourceJson = File.ReadAllText(fullPath);
                TextureData data = JsonConvert.DeserializeObject<TextureData>(resourceJson)!;

                level.AddTexture(
                    rr.Id,
                    System.IO.Path.GetDirectoryName(fullPath)!,
                    data
                );
            }

            level.floorColorArgb = new ColorArgb(level.FloorColor);
            level.ceilingColorArgb = new ColorArgb(level.CeilingColor);

            return level;
        }

        /// <summary>
        /// Serializes the JSON and saves the level data to the specified path.
        /// </summary>
        public void Save(string levelFilePath)
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(levelFilePath, json);
        }

        /// <summary>
        /// Returns how many times a specific texture reference ID is used in the map.
        /// </summary>
        public int GetTextureReferenceUsageCount(int resourceRefId)
        {
            int count = 0;

            foreach (var row in this.MapTiles)
                foreach (int cell in row)
                    if (cell == resourceRefId)
                        count++;

            return count;
        }

        /// <summary>
        /// Remove all references to a particular texture from the map tiles.
        /// </summary>
        public void RemoveTextureReferenceFromMap(int resourceRefId)
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

        private Dictionary<int, Texture> textureMap = new();
        public Dictionary<int, Texture> GetTextureMap() => this.textureMap;
        public void AddTexture(int resourceRefId, Texture texture)
        {
            textureMap.Add(
                resourceRefId,
                texture
            );
        }
        public void AddTexture(int resourceRefId, string folder, TextureData resource)
        {
            AddTexture(
                resourceRefId,
                Texture.Create(folder, resource)
            );
        }


        private ColorArgb? floorColorArgb;
        public ColorArgb GetFloorColor() => floorColorArgb ?? ColorArgb.White();

        private ColorArgb? ceilingColorArgb;
        public ColorArgb GetCeilingColor() => ceilingColorArgb ?? ColorArgb.White();


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

        [JsonProperty("textureReferences", Required = Required.Always)]
        public List<TextureReference> TextureReferences { get; set; } = new();

        [JsonProperty("ceilingColor", Required = Required.Always)]
        [JsonConverter(typeof(ByteArrayAsArrayConverter))]
        public byte[] CeilingColor { get; set; } = { 255, 40, 40, 40 };

        [JsonProperty("floorColor", Required = Required.Always)]
        [JsonConverter(typeof(ByteArrayAsArrayConverter))]
        public byte[] FloorColor { get; set; } = { 255, 90, 90, 90 };
    }
}
