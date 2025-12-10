namespace Gfx2d.Resources
{
    public enum ResourceSide
    {
        North,
        East,
        South,
        West
    }

    /// <summary>
    /// Provides details about a specific type of map resource.
    /// </summary>
    public class MapResource
    {
        /// <summary>
        /// A human-friendly name for the resource.
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        public ColorArgb NorthColor { get; private set; } = ColorArgb.Black();
        public ColorArgb EastColor { get; private set; } = ColorArgb.Black();
        public ColorArgb SouthColor { get; private set; } = ColorArgb.Black();
        public ColorArgb WestColor { get; private set; } = ColorArgb.Black();

        public ColorArgb GetColorForSide(ResourceSide side)
        {
            switch (side) {
                case ResourceSide.East: return this.EastColor;
                case ResourceSide.South: return this.SouthColor;
                case ResourceSide.West: return this.WestColor;
                default: return this.NorthColor;
            }
        }

        public static MapResource Create(
            string name,
            byte[] argb
        )
        {
            MapResource resource = new();

            resource.Name = name;

            resource.NorthColor = new ColorArgb(argb[0], argb[1], argb[2], argb[3]);
            resource.EastColor = new ColorArgb(argb[0], argb[1], argb[2], argb[3]);
            resource.SouthColor = new ColorArgb(argb[0], argb[1], argb[2], argb[3]);
            resource.WestColor = new ColorArgb(argb[0], argb[1], argb[2], argb[3]);

            return resource;
        }

        public static MapResource Create(
            string name,
            byte[] northArgb,
            byte[] eastArgb,
            byte[] southArgb,
            byte[] westArgb
        )
        {
            MapResource resource = new();

            resource.Name = name;

            resource.NorthColor = new ColorArgb(northArgb[0], northArgb[1], northArgb[2], northArgb[3]);
            resource.EastColor = new ColorArgb(eastArgb[0], eastArgb[1], eastArgb[2], eastArgb[3]);
            resource.SouthColor = new ColorArgb(southArgb[0], southArgb[1], southArgb[2], southArgb[3]);
            resource.WestColor = new ColorArgb(westArgb[0], westArgb[1], westArgb[2], westArgb[3]);

            return resource;
        }

        public static MapResource Create(ResourceData resourceData)
        {
            MapResource resource = new();

            resource.Name = resourceData.Name;

            resource.NorthColor = new ColorArgb(resourceData.North[0], resourceData.North[1], resourceData.North[2], resourceData.North[3]);
            resource.EastColor = new ColorArgb(resourceData.East[0], resourceData.East[1], resourceData.East[2], resourceData.East[3]);
            resource.SouthColor = new ColorArgb(resourceData.South[0], resourceData.South[1], resourceData.South[2], resourceData.South[3]);
            resource.WestColor = new ColorArgb(resourceData.North[0], resourceData.North[1], resourceData.North[2], resourceData.North[3]);

            return resource;
        }
    }
}