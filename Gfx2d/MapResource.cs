namespace Gfx2d
{
    public enum ResourceType
    {
        Ceiling,
        Floor,
        Wall
    }

    internal class MapResource
    {
        public string Name { get; private set; } = string.Empty;
        public ResourceType ResourceType { get; private set; }
        public ColorArgb Color { get; private set; }

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

            resource.Color = new ColorArgb(a, r, g, b);

            return resource;
        }
    }
}
