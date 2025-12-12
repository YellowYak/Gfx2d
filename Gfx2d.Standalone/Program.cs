using Gfx2d.Engine;
using SDL2;

public class StandaloneGameHost : IGameHost
{
    private IntPtr window;
    private bool running = true;
    private GameEngine engine;

    public int ScreenWidth { get; }
    public int ScreenHeight { get; }

    // SDL window was already created by this host
    public bool IsNativeWindowHandle => false;

    public StandaloneGameHost(int width, int height)
    {
        ScreenWidth = width;
        ScreenHeight = height;

        engine = new GameEngine(ScreenWidth, ScreenHeight);
    }

    public void Initialize()
    {
        // Create the SDL window and initialize the engine
        window = SDL.SDL_CreateWindow(
            title: "Gfx",
            x: SDL.SDL_WINDOWPOS_CENTERED,
            y: SDL.SDL_WINDOWPOS_CENTERED,
            w: ScreenWidth,
            h: ScreenHeight,
            flags: SDL.SDL_WindowFlags.SDL_WINDOW_HIDDEN
        );

        if (window == IntPtr.Zero)
            throw new Exception($"Window creation failed: {SDL.SDL_GetError()}");
        
        engine.Initialize(this, @"C:\Users\scott\OneDrive\My Projects\Programming Projects\Gfx2d\Resources\Level1.json");
    }

    public IntPtr GetWindowHandle() => window;

    public bool ProcessEvents()
    {
        while (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1)
        {
            switch (e.type)
            {
                case SDL.SDL_EventType.SDL_QUIT: return false;

                case SDL.SDL_EventType.SDL_KEYDOWN:
                    bool continueRunning = engine.HandleKeyDown(e.key.keysym.sym);

                    if (!continueRunning)
                        return false;
                    
                    break;

                case SDL.SDL_EventType.SDL_KEYUP:
                    engine.HandleKeyUp(e.key.keysym.sym);
                    break;
            }
        }

        return true;
    }

    public void Run()
    {
        // Main game loop - keep on chugging until ProcessEvents returns false
        while (ProcessEvents())
        {
            engine.Update();
            engine.Render();
        }
    }

    public void Cleanup()
    {
        engine.Cleanup();

        // We need to destroy the window since we created it (rather than GameEngine)
        SDL.SDL_DestroyWindow(window);
        SDL.SDL_Quit();
    }

    static void Main()
    {
        const int WindowScreenWidth = 1287;
        const int WindowScreenHeight = 720;

        var host = new StandaloneGameHost(WindowScreenWidth, WindowScreenHeight);

        host.Initialize();
        host.Run();
        host.Cleanup();
    }
}