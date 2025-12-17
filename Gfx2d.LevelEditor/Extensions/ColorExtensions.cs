using Gfx2d.Resources;
using System.Windows.Media;

namespace Gfx2d.LevelEditor.Extensions
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

        /// <summary>
        /// Generates a System.Windows.Media.Brush from a color byte array.
        /// Specifically, the color byte array needs exactly four entries in ARGB order.
        /// </summary>
        public static Brush ToBrush(this byte[] argbByteArray)
        {
            if (argbByteArray == null) throw new ArgumentNullException(nameof(argbByteArray));
            if (argbByteArray.Length != 4) throw new ArgumentOutOfRangeException(nameof(argbByteArray));

            return new SolidColorBrush(Color.FromArgb(argbByteArray[0], argbByteArray[1], argbByteArray[2], argbByteArray[3]));
        }
    }
}
