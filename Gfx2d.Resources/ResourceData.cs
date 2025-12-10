using Newtonsoft.Json;

namespace Gfx2d.Resources
{
    public class ResourceData
    {
        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("north", Required = Required.Always)]
        public byte[] North { get; set; } = Array.Empty<byte>();

        [JsonProperty("east", Required = Required.Always)]
        public byte[] East { get; set; } = Array.Empty<byte>();

        [JsonProperty("south", Required = Required.Always)]
        public byte[] South { get; set; } = Array.Empty<byte>();

        [JsonProperty("west", Required = Required.Always)]
        public byte[] West { get; set; } = Array.Empty<byte>();

        /// <summary>
        /// Determines if a specific file is a valid level resource file.
        /// </summary>
        /// <returns>True if the specified file exists and can be deserialized without error; false otherwise.</returns>
        public static bool ValidFile(string path)
        {
            try
            {
                string json = File.ReadAllText(path);
                JsonConvert.DeserializeObject<ResourceData>(json);
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
        public static ResourceData LoadFromFile(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            if (!File.Exists(path)) throw new FileNotFoundException("Resource file not found.", path);

            // Read in the level JSON file
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<ResourceData>(json)!;
        }
    }
}
