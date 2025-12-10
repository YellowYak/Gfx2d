using System.Windows.Media;

namespace Gfx2d.ResourceEditor.Extensions
{
    internal static class ColorExtensions
    {
        public static byte[] ToByteArray(this Color c) => new byte[] { c.A, c.R, c.G, c.B };
    }
}
