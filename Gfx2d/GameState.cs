using SDL2;

namespace Gfx2d
{
    internal enum CameraMode
    {
        FirstPerson,
        Overhead
    }

    internal enum KeyboardState
    {
        Unpressed,
        Pressed,
    }

    internal class GameState
    {
        public bool Running { get; set; }

        public IntPtr Window { get; private set; }
        public IntPtr Renderer { get; private set; }
        public IntPtr Texture { get; private set; }

        private int[] pixels = new int[0];
        public int[] Pixels => pixels;

        public int ScreenWidth { get; private set; }
        public int ScreenHeight { get; private set; }
        public int ScreenHeightHalved { get; private set; }
        public int BytesPerRowOfPixelData { get; private set; }


        public Point2d PlayerPos { get; set; } = new(0, 0);
        public double PlayerWidth => 0.5;


        public double CameraDistanceFromPlayer { get; set; } = 1;

        private double cameraWidth;
        public double CameraWidth
        {
            get { return cameraWidth; }
            set
            {
                cameraWidth = value;

                // Compute cameraHeight
                this.cameraHeight = this.cameraWidth * Convert.ToDouble(this.ScreenHeight) / Convert.ToDouble(this.ScreenWidth);

                // Compute camera sweep angle
                double sweepAngle = Math.Round(Math.Atan(this.cameraWidth / 2 / this.CameraDistanceFromPlayer), 5);

                // Determine how many iterations through the array that would be
                this.cameraSweepAngleIterations = (int)(sweepAngle / MathHelpers.RadiansPerEntry);
            }
        }

        private double cameraHeight;
        public double CameraHeight => this.cameraHeight;

        public double CameraZ { get; set; } = 0.5f;

        private int cameraAngleIndex;
        public int CameraAngleIndex
        {
            get { return cameraAngleIndex; }
            set
            {
                int userValue = value;
                if (userValue < 0)
                    userValue = MathHelpers.PossibleRotationRadians.Length - 1;

                if (userValue >= MathHelpers.PossibleRotationRadians.Length)
                    cameraAngleIndex = userValue % MathHelpers.PossibleRotationRadians.Length;
                else
                    cameraAngleIndex = userValue;

                // Determine CameraDirectionX & CameraDirectionY                
                this.cameraDirectionX = MathHelpers.GetDirectionXFromRotationIndex(cameraAngleIndex);
                this.cameraDirectionY = MathHelpers.GetDirectionYFromRotationIndex(cameraAngleIndex);
            }
        }

        int cameraDirectionX = 0;
        public int CameraDirectionX => this.cameraDirectionX;

        int cameraDirectionY = 0;
        public int CameraDirectionY => this.cameraDirectionY;

        public double CameraAngle => MathHelpers.PossibleRotationRadians[cameraAngleIndex];

        private int cameraSweepAngleIterations;
        public int CameraSweepAngleIterations => this.cameraSweepAngleIterations;

        public CameraMode CameraMode { get; set; } = CameraMode.FirstPerson;

        public KeyboardState Key_Right = KeyboardState.Unpressed;
        public KeyboardState Key_Left = KeyboardState.Unpressed;
        public KeyboardState Key_Up = KeyboardState.Unpressed;
        public KeyboardState Key_Down = KeyboardState.Unpressed;
        public KeyboardState Key_A = KeyboardState.Unpressed;
        public KeyboardState Key_D = KeyboardState.Unpressed;


        public GameState(int screenWidth, int screenHeight)
        {
            this.ScreenWidth = screenWidth;
            this.ScreenHeight = screenHeight;
            this.ScreenHeightHalved = screenHeight / 2;
            this.BytesPerRowOfPixelData = screenWidth * 4;

            this.pixels = new int[screenWidth * screenHeight];
        }

        public void Initialize()
        {
            this.Running = true;
            this.CameraWidth = 1.5;

            // Init the SDL video subsystem
            int initSuccess = SDL.SDL_Init(SDL.SDL_INIT_VIDEO);
            if (initSuccess != 0)
                throw new Exception($"There was an issue initializing SDL. {SDL.SDL_GetError()}");

            // Create a window
            this.Window = SDL.SDL_CreateWindow(
                title: "Gfx",
                x: SDL.SDL_WINDOWPOS_CENTERED,
                y: SDL.SDL_WINDOWPOS_CENTERED,
                w: this.ScreenWidth,
                h: this.ScreenHeight,
                flags: SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN
            );

            if (this.Window == IntPtr.Zero)
                throw new Exception($"There was an issue creating the window. {SDL.SDL_GetError()}");


            // Creates a new SDL hardware renderer using the default graphics device with VSYNC enabled.
            this.Renderer = SDL.SDL_CreateRenderer(
                window: this.Window,
                index: -1,
                flags: SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC
            );

            if (this.Renderer == IntPtr.Zero)
                throw new Exception($"There was an issue creating the renderer. {SDL.SDL_GetError()}");

            // Creates a new SDL hardware texture
            this.Texture = SDL.SDL_CreateTexture(
                this.Renderer,
                SDL.SDL_PIXELFORMAT_ARGB8888,
                (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING,
                this.ScreenWidth,
                this.ScreenHeight
            );
        }

        public void ToggleCameraView() => this.CameraMode = this.CameraMode == CameraMode.FirstPerson ? CameraMode.Overhead : CameraMode.FirstPerson;

        public void SetPixel(int x, int y, ColorArgb c)
        {
            this.pixels[y * this.ScreenWidth + x] =
                (c.A << 24) |
                (c.R << 16) |
                (c.G << 8) |
                c.B;
        }

        public void FillColumn(int x, int y1, int y2, ColorArgb c)
        {
            int sy = y1;
            int ey = y2;

            if (y1 > y2)
            {
                sy = y2;
                ey = y1;
            }

            for (int y = sy; y <= ey; y++)
                this.SetPixel(x, y, c);
        }

        public void FillRectangle(int x1, int y1, int x2, int y2, ColorArgb c)
        {
            int sx = x1;
            int ex = x2;

            if (x1 > x2)
            {
                sx = x2;
                ex = x1;
            }

            for (int x = sx; x <= ex; x++)
                this.FillColumn(x, y1, y2, c);
        }
    }
}
