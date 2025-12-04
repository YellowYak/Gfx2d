using Newtonsoft.Json;

namespace Gfx2d.Resources
{
    public class ResourceReference
    {
        [JsonProperty(Required = Required.Always)]
        public int Id { get; set; }
        
        [JsonProperty(Required = Required.Always)]
        public string FileName { get; set; } = string.Empty;
    }
}
