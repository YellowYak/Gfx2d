using Gfx2d.Engine;
using SDL2;

namespace Gfx2d.LevelEditor
{
    public class WpfGameHost : IGameHost
    {
        private IntPtr hwnd;
        private GameEngine engine;

        public int ScreenWidth { get; }
        public int ScreenHeight { get; }

        // WPF provides a native Win32 window handle
        public bool IsNativeWindowHandle => true;

        public WpfGameHost(IntPtr windowHandle, int width, int height)
        {
            hwnd = windowHandle;
            
            ScreenWidth = width;
            ScreenHeight = height;

            engine = new GameEngine(ScreenWidth, ScreenHeight);
        }

        public IntPtr GetWindowHandle() => hwnd;

        public bool ProcessEvents()
        {
            // WPF handles events through its own mechanism
            return true;
        }

        public void Initialize(string levelPath) => engine.Initialize(this, levelPath);

        public void Update() => engine.Update();
        public void Render() => engine.Render();

        public void HandleKeyDown(SDL.SDL_Keycode key) => engine.HandleKeyDown(key);
        public void HandleKeyUp(SDL.SDL_Keycode key) => engine.HandleKeyUp(key);

        public void Cleanup()
        {
            engine.Cleanup();
        }
    }
}