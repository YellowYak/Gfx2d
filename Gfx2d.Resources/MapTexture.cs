namespace Gfx2d.Resources
{
    public class MapTexture
    {
        /// <summary>
        /// A human-friendly name for the resource.
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        public Bitmap? NorthBitmap = null;

        public Bitmap GetBitmapForSide(ResourceSide side)
        {
            return side switch
            {
                _ => NorthBitmap!
            };
        }

        public static MapTexture Create(TextureData textureData)
        {
            MapTexture texture = new();

            texture.Name = textureData.Name;

            texture.NorthBitmap = new Bitmap(textureData);

            return texture;
        }
    }
}
