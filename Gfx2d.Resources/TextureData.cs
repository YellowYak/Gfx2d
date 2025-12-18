using Gfx2d.Resources.Serialization;
using Newtonsoft.Json;

namespace Gfx2d.Resources
{
    public enum ResourceSide
    {
        N,
        NE,
        E,
        SE,
        S,
        SW,
        W,
        NW
    }

    public class TextureData
    {
        public const int TextureWidth = 64;
        public const int TextureHeight = 64;

        public TextureData()
        {
            North = new int[TextureHeight][][];
            East = new int[TextureHeight][][];
            South = new int[TextureHeight][][];
            West = new int[TextureHeight][][];

            for (int y = 0; y < TextureHeight; y++)
            {
                North[y] = new int[TextureWidth][];
                East[y] = new int[TextureWidth][];
                South[y] = new int[TextureWidth][];
                West[y] = new int[TextureWidth][];

                for (int x = 0; x < TextureWidth; x++)
                {
                    North[y][x] = new int[4] { 255, 255, 255, 255 };
                    East[y][x] = new int[4] { 255, 255, 255, 255 };
                    South[y][x] = new int[4] { 255, 255, 255, 255 };
                    West[y][x] = new int[4] { 255, 255, 255, 255 };
                }
            }
        }

        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; set; } = "New texture";


        [JsonProperty("width", Required = Required.Always)]
        public int Width { get; set; } = TextureWidth;

        [JsonProperty("height", Required = Required.Always)]
        public int Height { get; set; } = TextureHeight;


        [JsonProperty("north", Required = Required.Always)]
        public int[][][] North { get; set; }
        [JsonProperty("east")]
        public int[][][] East { get; set; }
        [JsonProperty("south")]
        public int[][][] South { get; set; }
        [JsonProperty("west")]
        public int[][][] West { get; set; }


        /// <summary>
        /// Determines if a specific file is a valid level texture file.
        /// </summary>
        /// <returns>True if the specified file exists and can be deserialized without error; false otherwise.</returns>
        public static bool ValidFile(string path)
        {
            try
            {
                string json = File.ReadAllText(path);
                JsonConvert.DeserializeObject<TextureData>(json);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Loads a resource from a JSON file.
        /// </summary>
        public static TextureData LoadFromFile(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            if (!File.Exists(path)) throw new FileNotFoundException("Texture file not found.", path);

            // Read in the level JSON file
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<TextureData>(json)!;
        }

        /// <summary>
        /// Serializes the JSON and saves the texture data to the specified path.
        /// </summary>
        public void Save(string path)
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(path, json);
        }
    }
}
