using Gfx2d.Resources;
using System.Windows.Media;

namespace Gfx2d.ResourceEditor.Extensions
{
    internal static class ColorExtensions
    {
        /// <summary>
        /// Generates an ARGB byte array from a System.Windows.Media.Color instance.
        /// </summary>
        public static byte[] ToByteArray(this Color c) => new byte[] { c.A, c.R, c.G, c.B };

        /// <summary>
        /// Generates a System.Windows.Media.Brush from a ColorArgb instance.
        /// </summary>
        public static Brush ToBrush(this ColorArgb argb) => new SolidColorBrush(Color.FromArgb(argb.A, argb.R, argb.G, argb.B));
    }
}
