using Newtonsoft.Json;

namespace Gfx2d.Resources
{
    internal class LevelData
    {
        public static LevelData LoadFromFile(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            if (!File.Exists(path)) throw new FileNotFoundException($"Level file {path} not found.", path);

            string json = File.ReadAllText(path);

            return JsonConvert.DeserializeObject<LevelData>(json)!;
        }

        [JsonProperty(Required = Required.Always, PropertyName = "dimensions")]
        public LevelDimensions Dimensions { get; set; } = new();

        [JsonProperty(Required = Required.Always, PropertyName = "startingPosition")]
        public LevelStartingPosition StartingPosition { get; set; } = new();

        [JsonProperty(Required = Required.Always, PropertyName = "mapTiles")]
        public int[][] MapTiles { get; set; } = Array.Empty<int[]>();
    }
}
