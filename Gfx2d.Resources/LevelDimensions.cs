using Newtonsoft.Json;

namespace Gfx2d.Resources
{
    public class LevelDimensions
    {
        /// <summary>
        /// The dimensions of each tile on the map. All tiles on the map are a square.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public double TileWidth { get; set; }

        /// <summary>
        /// The height of all walls on the map.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public double WallHeight { get; set; }
    }
}
