using Newtonsoft.Json;

namespace Gfx2d.Resources
{
    internal class ResourceData
    {
        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("north", Required = Required.Always)]
        public int[] North { get; set; } = Array.Empty<int>();

        [JsonProperty("east", Required = Required.Always)]
        public int[] East { get; set; } = Array.Empty<int>();

        [JsonProperty("south", Required = Required.Always)]
        public int[] South { get; set; } = Array.Empty<int>();

        [JsonProperty("west", Required = Required.Always)]
        public int[] West { get; set; } = Array.Empty<int>();
    }
}
