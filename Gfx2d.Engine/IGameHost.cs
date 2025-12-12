
namespace Gfx2d.Engine
{
    public interface IGameHost
    {
        /// <summary>
        /// Returns a pointer to the SDL window handle.
        /// This could be either an SDL window pointer or a native HWND.
        /// </summary>
        IntPtr GetWindowHandle();

        /// <summary>
        /// Indicates whether GetWindowHandle returns a native window handle (true)
        /// or an already-created SDL window (false).
        /// </summary>
        bool IsNativeWindowHandle { get; }

        /// <summary>
        /// Processes all pending events in the event queue.
        /// </summary>
        /// <returns>True if processing is to continue, false if processing should terminate.</returns>
        bool ProcessEvents();

        /// <summary>
        /// The width of the screen in pixels.
        /// </summary>
        int ScreenWidth { get; }

        /// <summary>
        /// The height of the screen in pixels.
        /// </summary>
        int ScreenHeight { get; }
    }
}
