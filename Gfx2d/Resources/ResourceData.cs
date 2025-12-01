using Newtonsoft.Json;

namespace Gfx2d.Resources
{
    internal class ResourceData
    {
        [JsonProperty(Required = Required.Always)]
        public string Name { get; set; } = string.Empty;

        [JsonProperty(Required = Required.Always)]
        public int[] North { get; set; } = Array.Empty<int>();

        [JsonProperty(Required = Required.Always)]
        public int[] East { get; set; } = Array.Empty<int>();

        [JsonProperty(Required = Required.Always)]
        public int[] South { get; set; } = Array.Empty<int>();

        [JsonProperty(Required = Required.Always)]
        public int[] West { get; set; } = Array.Empty<int>();
    }
}
