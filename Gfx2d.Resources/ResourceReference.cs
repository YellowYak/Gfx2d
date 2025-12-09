using Newtonsoft.Json;

namespace Gfx2d.Resources
{
    public class ResourceReference
    {
        [JsonProperty("id", Required = Required.Always)]
        public int Id { get; set; }
        
        [JsonProperty("fileName", Required = Required.Always)]
        public string FileName { get; set; } = string.Empty;
    }
}
