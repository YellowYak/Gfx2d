using Newtonsoft.Json;

namespace Gfx2d.Resources
{
    internal class ResourceData
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
    }
}
