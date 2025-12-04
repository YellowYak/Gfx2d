namespace Gfx2d.Engine
{
    internal static class DebugHelpers
    {
        /// <summary>
        /// Writes the contents of the supplied enumeration to a specified file path ONLY if there does not already exist a file there with that name.
        /// If a file with the same name exists, no action is taken.
        /// </summary>
        public static void LogIfFileDoesNotExist(IEnumerable<object> data, string path)
        {
            if (File.Exists(path)) return;

            File.AppendAllLines(path, data.Select(entry =>  entry == null ? string.Empty : entry!.ToString()!));
        }
    }
}
