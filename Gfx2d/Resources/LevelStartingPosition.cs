using Newtonsoft.Json;

namespace Gfx2d.Resources
{
    internal class LevelStartingPosition
    {
        /// <summary>
        /// The player's starting point on the map.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public Point2d PlayerPos { get; set; } = new(0, 0);
        
        /// <summary>
        /// The direction the camera is facing in radians.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public double CameraAngleRads { get; set; }
    }
}
