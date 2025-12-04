using SDL2;
using Gfx2d.Resources;

namespace Gfx2d.Engine
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

    /// <summary>
    /// Represents the game state.
    /// This includes pointers to the SDL objects used to render the graphics, details about the screen dimensions, player and camera details, what keys on the keyboard are currently pressed, and so on.
    /// </summary>
    internal class GameState
    {
        /// <summary>
        /// A reference to the MathHelpers library.
        /// </summary>
        public MathHelpers MathHelpers { get; private set; }

        /// <summary>
        /// Indicates whether the game is running.
        /// If true, the game loop continues unabated!
        /// If false, the SDL library cleans up its resources and the application terminates.
        /// </summary>
        public bool Running { get; set; }

        /// <summary>
        /// A pointer reference to the SDL window.
        /// </summary>
        public IntPtr Window { get; private set; }
        /// <summary>
        /// A pointer reference to the SDL renderer.
        /// </summary>
        public IntPtr Renderer { get; private set; }
        /// <summary>
        /// A pointer reference to the SDL textture that is used to blit the pixel array to the window.
        /// </summary>
        public IntPtr Texture { get; private set; }

        /// <summary>
        /// The pixel array that is blitted to the SDL window.
        /// </summary>
        private int[] pixels = Array.Empty<int>();
        public int[] Pixels => pixels;

        /// <summary>
        /// The game's screen width at normal scale.
        /// </summary>
        public int ScreenWidth { get; private set; }
        /// <summary>
        /// The game's screen height at normal scale.
        /// </summary>
        public int ScreenHeight { get; private set; }
        /// <summary>
        /// A cached value of half the screen's height. This value is used in many calculations.
        /// </summary>
        public int ScreenHeightHalved { get; private set; }
        /// <summary>
        /// The number of bytes per row of data in the pixel array. This number is based on the width of the screen and is needed when blitting the pixel array to the SDL window.
        /// </summary>
        public int BytesPerRowOfPixelData { get; private set; }


        /// <summary>
        /// The player's position on the map.
        /// </summary>
        public Point2d PlayerPos { get; set; } = new(0, 0);
        /// <summary>
        /// Defines the player's 2d bounds on the map. The player on the map fills a square of these dimensions and these bounds are used to
        /// determine when a player encounters elements on the map (such as walls, items, etc.).
        /// </summary>
        public double PlayerWidth => 0.5;


        /// <summary>
        /// The distance on the map between the player (<see cref="PlayerPos"/>) and the camera where the scene is rendered.
        /// </summary>
        public double CameraDistanceFromPlayer { get; set; } = 1;

        private double cameraWidth;
        /// <summary>
        /// The width of the camera on the map. This value determines the width of the field of view of the player - the greater the value the wider the view.
        /// </summary>
        public double CameraWidth
        {
            get => cameraWidth;
            set
            {
                cameraWidth = value;

                // Compute camera sweep angle. This angle is based on the width of the camera and the distance from the player.
                // A triangle is formed from the player's POV as the apex and the camera width as the base.
                // We extend a line from the apex to the base to form a right triangle and to calcuate the sweep angle.
                double sweepAngle = Math.Atan(this.cameraWidth / 2 / this.CameraDistanceFromPlayer) * 2;

                // An pre-comptued array of angles is used throughout. Therefore, we need to determine how many iterations we will
                // need to make through this array in order to cover sweepAngle number of radians. This number is determined by
                // taking the sweepAngle and dividing it by the space "between" each radian entry in the array.
                this.cameraSweepAngleIterations = (int)(sweepAngle / MathHelpers.RotationRadiansDelta);
            }
        }

        /// <summary>
        /// The height of the player's POV when they are standing.
        /// </summary>
        public const double StandingCameraZ = 0.6;

        /// <summary>
        /// The height of the player's POV when they are fully crouched.
        /// </summary>
        public const double FullyCrouchingCameraZ = 0.25;

        private double cameraZ = StandingCameraZ;
        /// <summary>
        /// The height of the vertical center of the camera. This is the "height" of the player's POV.
        /// This can be used to allow the player to crouch, for example.
        /// This value gets set when loading a map.
        /// </summary>
        public double CameraZ
        {
            get => cameraZ;
            set
            {
                cameraZ = Math.Clamp(value, FullyCrouchingCameraZ, StandingCameraZ);
            }
        }

        /// <summary>
        /// A player that is not at their current standing height is considered crouching, even if they are not at a full crouch.
        /// </summary>
        public bool IsCrouching => cameraZ < StandingCameraZ;

        private int cameraAngleIndex;
        /// <summary>
        /// The index in the <see cref="MathHelpers.PossibleRotationRadians"/> array that represents the angle of the camera.
        /// </summary>
        public int CameraAngleIndex
        {
            get => cameraAngleIndex;
            set
            {
                // The PossibleRotationRadians array is circular, so if a value is assigned that is outside of the array bounds we can use a
                // little modulo arithmatic to get back into the array at the corresponding location.
                int userValue = value;
                if (userValue < 0)
                    cameraAngleIndex = Math.Clamp(MathHelpers.PossibleRotationRadiansLength + userValue, 0, MathHelpers.PossibleRotationRadiansLength - 1);
                else if (userValue >= MathHelpers.PossibleRotationRadiansLength)
                    cameraAngleIndex = userValue % MathHelpers.PossibleRotationRadiansLength;
                else
                    cameraAngleIndex = userValue;
            }
        }

        private int cameraSweepAngleIterations;
        /// <summary>
        /// The number of camera sweep angle iterations to render when drawing the screen.
        /// This value is automatically calculated whenever <see cref="CameraWidth"/> is assigned.
        /// </summary>
        public int CameraSweepAngleIterations => this.cameraSweepAngleIterations;

        /// <summary>
        /// What camera mode to render to the screen.
        /// </summary>
        public CameraMode CameraMode { get; set; } = CameraMode.FirstPerson;

        /// <summary>
        /// Whether to render the window as full screen or windowed.
        /// </summary>
        public bool Fullscreen { get; private set; } = false;


        public KeyboardState Key_Right = KeyboardState.Unpressed;
        public KeyboardState Key_Left = KeyboardState.Unpressed;
        public KeyboardState Key_Up = KeyboardState.Unpressed;
        public KeyboardState Key_Down = KeyboardState.Unpressed;
        public KeyboardState Key_A = KeyboardState.Unpressed;
        public KeyboardState Key_D = KeyboardState.Unpressed;
        public KeyboardState Key_Z = KeyboardState.Unpressed;


        public GameState(MathHelpers math, int screenWidth, int screenHeight)
        {
            this.MathHelpers = math;

            this.ScreenWidth = screenWidth;
            this.ScreenHeight = screenHeight;
            this.ScreenHeightHalved = screenHeight / 2;
            this.BytesPerRowOfPixelData = screenWidth * 4;

            this.pixels = new int[screenWidth * screenHeight];
        }


        /// <summary>
        /// Initializes the SDL Window, Renderer, and Texture, as well as other game state.
        /// </summary>
        public void Initialize()
        {
            this.Running = true;
            this.CameraWidth = 1.5;

            // Init the SDL video subsystem
            int initSuccess = SDL.SDL_Init(SDL.SDL_INIT_VIDEO);
            if (initSuccess != 0)
                throw new Exception($"There was an issue initializing SDL. {SDL.SDL_GetError()}");

            // Create a window with the specified width & height.
            // Note that we initially HIDE the window. This is to prevent the flash of a small window being drawn only to then, a few seconds later, be replaced by a full screen window.
            this.Window = SDL.SDL_CreateWindow(
                title: "Gfx",
                x: SDL.SDL_WINDOWPOS_CENTERED,
                y: SDL.SDL_WINDOWPOS_CENTERED,
                w: this.ScreenWidth,
                h: this.ScreenHeight,
                flags: SDL.SDL_WindowFlags.SDL_WINDOW_HIDDEN
            );

            if (this.Window == IntPtr.Zero)
                throw new Exception($"There was an issue creating the window. {SDL.SDL_GetError()}");

            // Set full screen mode, if needed
            if (this.Fullscreen)
                SetFullscreen();

            // Now that we have the appropriate screen mode (windowed or full screen) we're ready to show the window.
            SDL.SDL_ShowWindow(this.Window);

            // Creates a new SDL hardware renderer using the default graphics device with VSYNC enabled.
            // The SDL_RENDERER_PRESENTVSYNC flag tells SDL to time its rendering with the monitor's refresh rate.
            // The upside to this is that we don't have to handle the timing of blitting the pixel array to the window.
            // The downside is that the FPS is capped at the monitor's refresh rate.
            this.Renderer = SDL.SDL_CreateRenderer(
                window: this.Window,
                index: -1,
                flags: SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC
            );

            if (this.Renderer == IntPtr.Zero)
                throw new Exception($"There was an issue creating the renderer. {SDL.SDL_GetError()}");

            // Creates a new SDL hardware texture using the ARGB8888 pixel format.
            this.Texture = SDL.SDL_CreateTexture(
                this.Renderer,
                SDL.SDL_PIXELFORMAT_ARGB8888,
                (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING,
                this.ScreenWidth,
                this.ScreenHeight
            );
        }

        /// <summary>
        /// Sets the SDL Window to display as full screen.
        /// </summary>
        private void SetFullscreen()
        {
            int rslt = SDL.SDL_SetWindowFullscreen(this.Window, (uint)SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP);

            if (rslt < 0)
                throw new Exception($"There was an issue going to full screen. {SDL.SDL_GetError()}");
        }

        /// <summary>
        /// Sets the SDL Window to display as a window.
        /// </summary>
        private void SetWindowedScreen()
        {
            int rslt = SDL.SDL_SetWindowFullscreen(this.Window, 0);

            if (rslt < 0)
                throw new Exception($"There was an issue going to a windowed screen. {SDL.SDL_GetError()}");
        }

        /// <summary>
        /// Toggles the SDL Window's mode, either switching from windowed to full screen, or vice-versa.
        /// </summary>
        public void ToggleFullscreen()
        {
            if (this.Fullscreen)
                SetWindowedScreen();
            else
                SetFullscreen();

            this.Fullscreen = !this.Fullscreen;
        }

        /// <summary>
        /// Toggles the camera mode, either switching from first-person to overhead, or vice-versa.
        /// </summary>
        public void ToggleCameraView() => this.CameraMode = this.CameraMode == CameraMode.FirstPerson ? CameraMode.Overhead : CameraMode.FirstPerson;

        /// <summary>
        /// Draws a single pixel at coordinate (x, y) with the specified <see cref="ColorArgb"/> color.
        /// The coordinate system places (0, 0) in the upper left corner and (ScreenWidth, ScreenHeight) in the bottom right corner.
        /// An exception is thrown if the specified (x, y) coordinate lands out of bounds.
        /// </summary>
        public void SetPixel(int x, int y, ColorArgb c)
        {
            this.pixels[y * this.ScreenWidth + x] =
                (c.A << 24) |
                (c.R << 16) |
                (c.G << 8) |
                c.B;
        }

        /// <summary>
        /// Fills a column on the screen from point y1 to y2, inclusive, with a given <see cref="ColorArgb"/> color.
        /// y2 does NOT have to be greater than or equal to y1.
        /// </summary>
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

        /// <summary>
        /// Fills a rectangle on the screen bounded by points (x1, y1) and (x2, y2) with a given <see cref="ColorArgb"/> color.
        /// </summary>
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