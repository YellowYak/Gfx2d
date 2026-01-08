using Newtonsoft.Json;

namespace Gfx2d.Resources
{
    public class TextureReference
    {
        [JsonProperty("id", Required = Required.Always)]
        public int Id { get; set; }
        
        [JsonProperty("fileName", Required = Required.Always)]
        public string FileName { get; set; } = string.Empty;

        [JsonIgnore]
        public string DisplayName => $"{Id}: {FileName}";
    }
}
