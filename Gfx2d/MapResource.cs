namespace Gfx2d
{
    public enum ResourceType
    {
        Ceiling,
        Floor,
        Wall
    }

    public enum ResourceSide
    {
        North,
        East,
        South,
        West
    }

    internal class MapResource
    {
        public string Name { get; private set; } = string.Empty;
        public ResourceType ResourceType { get; private set; }
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
            ResourceType type,
            int a,
            int r,
            int g,
            int b
        )
        {
            MapResource resource = new();

            resource.Name = name;

            resource.ResourceType = type;

            resource.NorthColor = new ColorArgb(a, r, g, b);
            resource.EastColor = new ColorArgb(a, r, g, b);
            resource.SouthColor = new ColorArgb(a, r, g, b);
            resource.WestColor = new ColorArgb(a, r, g, b);

            return resource;
        }

        public static MapResource Create(
            string name,
            ResourceType type,
            int northA,
            int northR,
            int northG,
            int northB,
            int eastA,
            int eastR,
            int eastG,
            int eastB,
            int southA,
            int southR,
            int southG,
            int southB,
            int westA,
            int westR,
            int westG,
            int westB
        )
        {
            MapResource resource = new();

            resource.Name = name;

            resource.ResourceType = type;

            resource.NorthColor = new ColorArgb(northA, northR, northG, northB);
            resource.EastColor = new ColorArgb(eastA, eastR, eastG, eastB);
            resource.SouthColor = new ColorArgb(southA, southR, southG, southB);
            resource.WestColor = new ColorArgb(westA, westR, westG, westB);

            return resource;
        }
    }
}
