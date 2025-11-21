using Gfx2d;
using SDL2;
using System.Diagnostics;

const int SCREEN_WIDTH = 644;
const int SCREEN_HEIGHT = 480;

var MathHelpers = new MathHelpers(0.001);
var state = new GameState(MathHelpers, SCREEN_WIDTH, SCREEN_HEIGHT);
var map = new Map();

void ProcessEvents()
{
    while (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1)
    {
        switch (e.type)
        {
            case SDL.SDL_EventType.SDL_QUIT:
                state.Running = false;
                break;

            case SDL.SDL_EventType.SDL_KEYDOWN:
                Debug.Print($"Keydown: {e.key.keysym.sym}");
                HandleKeyDown(e.key.keysym);

                break;

            case SDL.SDL_EventType.SDL_KEYUP:
                Debug.Print($"Keyup: {e.key.keysym.sym}");
                HandleKeyUp(e.key.keysym);
                break;
        }
    }
}

void HandleKeyDown(SDL.SDL_Keysym key)
{
    switch (key.sym)
    {
        case SDL.SDL_Keycode.SDLK_RIGHT: state.Key_Right = KeyboardState.Pressed; break;
        case SDL.SDL_Keycode.SDLK_LEFT: state.Key_Left = KeyboardState.Pressed; break;
        case SDL.SDL_Keycode.SDLK_UP: state.Key_Up = KeyboardState.Pressed; break;
        case SDL.SDL_Keycode.SDLK_DOWN: state.Key_Down = KeyboardState.Pressed; break;
        case SDL.SDL_Keycode.SDLK_a: state.Key_A = KeyboardState.Pressed; break;
        case SDL.SDL_Keycode.SDLK_d: state.Key_D = KeyboardState.Pressed; break;

        case SDL.SDL_Keycode.SDLK_F12: state.ToggleFullscreen(); break;

        case SDL.SDL_Keycode.SDLK_ESCAPE:
            CleanUp();
            Environment.Exit(0);
            break;

        case SDL.SDL_Keycode.SDLK_TAB:
            state.ToggleCameraView();
            break;
    }
}

void HandleKeyUp(SDL.SDL_Keysym key)
{
    switch (key.sym)
    {
        case SDL.SDL_Keycode.SDLK_RIGHT: state.Key_Right = KeyboardState.Unpressed; break;
        case SDL.SDL_Keycode.SDLK_LEFT: state.Key_Left = KeyboardState.Unpressed; break;
        case SDL.SDL_Keycode.SDLK_UP: state.Key_Up = KeyboardState.Unpressed; break;
        case SDL.SDL_Keycode.SDLK_DOWN: state.Key_Down = KeyboardState.Unpressed; break;
        case SDL.SDL_Keycode.SDLK_a: state.Key_A = KeyboardState.Unpressed; break;
        case SDL.SDL_Keycode.SDLK_d: state.Key_D = KeyboardState.Unpressed; break;
    }
}

void UpdateGameState()
{
    const double PlayerMovementAcceleration = 0.035;

    if (state.Key_Right == KeyboardState.Pressed) state.CameraAngleIndex += MathHelpers.RotationIndexDistanceWhenPlayerRotating;
    if (state.Key_Left == KeyboardState.Pressed) state.CameraAngleIndex -= MathHelpers.RotationIndexDistanceWhenPlayerRotating;

    Point2d newPlayerPos = new(state.PlayerPos);
    bool playerPosUpdated = false;

    if (state.Key_Up == KeyboardState.Pressed)
    {
        newPlayerPos.X += MathHelpers.Cos(state.CameraAngleIndex) * PlayerMovementAcceleration;
        newPlayerPos.Y += MathHelpers.Sin(state.CameraAngleIndex) * PlayerMovementAcceleration;
        playerPosUpdated = true;
    }

    if (state.Key_Down == KeyboardState.Pressed)
    {
        newPlayerPos.X -= MathHelpers.Cos(state.CameraAngleIndex) * PlayerMovementAcceleration;
        newPlayerPos.Y -= MathHelpers.Sin(state.CameraAngleIndex) * PlayerMovementAcceleration;
        playerPosUpdated = true;
    }

    if (state.Key_A == KeyboardState.Pressed)
    {
        int prIndex = state.CameraAngleIndex + MathHelpers.PiOverTwoIndex;
        newPlayerPos.X -= MathHelpers.Cos(prIndex) * PlayerMovementAcceleration;
        newPlayerPos.Y -= MathHelpers.Sin(prIndex) * PlayerMovementAcceleration;
        playerPosUpdated = true;
        Debug.Print($"Player pos: {newPlayerPos}, CAI={state.CameraAngleIndex}, prIndex={prIndex}, CAdeg={MathHelpers.GetPossibleRotationRadians(state.CameraAngleIndex) * 180 / Math.PI}, prDeg={MathHelpers.GetPossibleRotationRadians(prIndex) * 180 / Math.PI}");
    }

    if (state.Key_D == KeyboardState.Pressed)
    {
        int prIndex = state.CameraAngleIndex + MathHelpers.PiOverTwoIndex;
        newPlayerPos.X += MathHelpers.Cos(prIndex) * PlayerMovementAcceleration;
        newPlayerPos.Y += MathHelpers.Sin(prIndex) * PlayerMovementAcceleration;
        playerPosUpdated = true;
    }

    // If the player moved, make sure they didn't move into a wall. If so, stop them!
    if (playerPosUpdated)
    {
        // Need to check to see if any of the bounds of the player are on a tile that contains a wall
        bool newPlayerPosAllowed = true;
        var bounds = newPlayerPos.GetRectBounds(state.PlayerWidth);
        foreach (var bound in bounds)
        {
            int indX = (int)MathHelpers.Floor(bound.X / map.TileWidth);
            int indY = (int)MathHelpers.Floor(bound.Y / map.TileWidth);

            if (map.GetTileResource(indX, indY) != null)
            {
                newPlayerPosAllowed = false;
                break;
            }
        }

        // If the new position is allowed then update state.PlayerPos accordingly
        if (newPlayerPosAllowed)
        {
            state.PlayerPos.X = newPlayerPos.X;
            state.PlayerPos.Y = newPlayerPos.Y;
        }
    }
}

void RenderToPixelBuffer()
{
    if (state.CameraMode == CameraMode.FirstPerson)
        RenderFirstPersonViewToPixelBuffer();
    else
        RenderOverheadViewToPixelBuffer();
}

void RenderOverheadViewToPixelBuffer()
{
    // Determine number of pixels per tile
    int pixelsPerTileX = Convert.ToInt32(state.ScreenWidth / map.Width);
    int pixelsPerTileY = Convert.ToInt32(state.ScreenHeight / map.Height);

    double playerWidthToTileWidthRatio = state.PlayerWidth / map.TileWidth;
    int pixelsPerPlayerXHalved = Convert.ToInt32(pixelsPerTileX * playerWidthToTileWidthRatio) / 2;
    int pixelsPerPlayerYHalved = Convert.ToInt32(pixelsPerTileY * playerWidthToTileWidthRatio) / 2;

    // Draw map
    for (int x = 0; x < map.Width; x++)
    {
        for (int y = 0; y < map.Height; y++)
        {
            MapResource? mapResource = map.GetTileResource(x, y);
            ColorArgb c = mapResource == null ? map.GetFloorResource().NorthColor : mapResource.NorthColor;

            state.FillRectangle(
                x * pixelsPerTileX,
                y * pixelsPerTileY,
                (x + 1) * pixelsPerTileX - 1,
                (y + 1) * pixelsPerTileY - 1,
                c
            );
        }
    }

    // Draw character
    int playerCenterX = Convert.ToInt32(pixelsPerTileX * state.PlayerPos.X);
    int playerCenterY = Convert.ToInt32(pixelsPerTileY * state.PlayerPos.Y);
    state.FillRectangle(
        playerCenterX - pixelsPerPlayerXHalved,
        playerCenterY - pixelsPerPlayerYHalved,
        playerCenterX + pixelsPerPlayerXHalved,
        playerCenterY + pixelsPerPlayerYHalved,
        new ColorArgb(255, 255, 255, 255)
    );

    // Draw camera
    int cameraCenterX = Convert.ToInt32(MathHelpers.Cos(state.CameraAngleIndex) * state.CameraDistanceFromPlayer * pixelsPerTileX) + playerCenterX;
    int cameraCenterY = Convert.ToInt32(MathHelpers.Sin(state.CameraAngleIndex) * state.CameraDistanceFromPlayer * pixelsPerTileY) + playerCenterY;
    state.FillRectangle(
        cameraCenterX - 3,
        cameraCenterY - 3,
        cameraCenterX + 3,
        cameraCenterY + 3,
        new ColorArgb(255, 66, 66, 66)
    );
}

void RenderFirstPersonViewToPixelBuffer()
{
    int startCameraAngleIndex = state.CameraAngleIndex - MathHelpers.Floor(state.CameraSweepAngleIterations / 2);
    int endCameraAngleIndex = state.CameraAngleIndex + MathHelpers.Ceiling(state.CameraSweepAngleIterations / 2);
    int colsPerIteration = state.ScreenWidth / state.CameraSweepAngleIterations;
    int currentColumnX = 0;
    int leftBuffer = 0;

    // If colsPerIteration is 0 we need to adjust
    int usedScreenWidth = colsPerIteration * state.CameraSweepAngleIterations;
    if (usedScreenWidth < state.ScreenWidth)
    {
        int delta = state.ScreenWidth - usedScreenWidth;
        leftBuffer = delta / 2;

        state.FillRectangle(0, 0, leftBuffer, state.ScreenHeight - 1, ColorArgb.Black());

        currentColumnX = leftBuffer;
    }

    for (int rawCameraAngleIndex = startCameraAngleIndex; rawCameraAngleIndex < endCameraAngleIndex; rawCameraAngleIndex++)
    {
        int cameraAngleIndex = rawCameraAngleIndex;
        if (cameraAngleIndex < 0)
            cameraAngleIndex = MathHelpers.PossibleRotationRadiansLength + cameraAngleIndex;
        else if (cameraAngleIndex > MathHelpers.PossibleRotationRadiansLength)
            cameraAngleIndex = cameraAngleIndex % MathHelpers.PossibleRotationRadiansLength;

        int player_ind_x = (int)MathHelpers.Floor(state.PlayerPos.X / map.TileWidth);
        double player_offset_x = state.PlayerPos.X - map.TileWidth * player_ind_x;

        int player_ind_y = (int)MathHelpers.Floor(state.PlayerPos.Y / map.TileWidth);
        double player_offset_y = state.PlayerPos.Y - map.TileWidth * player_ind_y;

        int raycast_ind_x = player_ind_x;
        double raycast_offset_x = player_offset_x;
        int raycast_ind_y = player_ind_y;
        double raycast_offset_y = player_offset_y;

        int rayTraceDirectionX = MathHelpers.GetDirectionXFromRotationIndex(cameraAngleIndex);
        int rayTraceDirectionY = MathHelpers.GetDirectionYFromRotationIndex(cameraAngleIndex);

        if (cameraAngleIndex >= MathHelpers.PiOverTwoIndex && cameraAngleIndex < MathHelpers.PiIndex)        // Lower-left quadrant
            cameraAngleIndex = MathHelpers.PiOverTwoIndex - (cameraAngleIndex - MathHelpers.PiOverTwoIndex);
        else if (cameraAngleIndex >= MathHelpers.PiIndex && cameraAngleIndex < MathHelpers.ThreePiOverTwoIndex)  // Upper-left quadrant
            cameraAngleIndex = cameraAngleIndex % MathHelpers.PiIndex;
        else if (cameraAngleIndex >= MathHelpers.ThreePiOverTwoIndex)   // Upper-right quadrant
            cameraAngleIndex = MathHelpers.PossibleRotationRadiansLength - cameraAngleIndex;

        MapResource? currentTileResource = map.GetTileResource(raycast_ind_x, raycast_ind_y);

        ResourceSide wallSideStruckByRay = ResourceSide.North;

        while (currentTileResource == null)
        {
            double remaining_tile_y_length = raycast_offset_y;
            if (rayTraceDirectionY < 0 && remaining_tile_y_length == 0)
                remaining_tile_y_length = map.TileWidth;
            if (rayTraceDirectionY > 0)
                remaining_tile_y_length = map.TileWidth - raycast_offset_y;

            double triangle_x_length = 0;
            double triangle_y_length = 0;
            double triangle_hyp_length = 0;

            if (rayTraceDirectionX != 0)
            {
                if (rayTraceDirectionY == 0)
                {
                    // Going straight right or left
                    if (rayTraceDirectionX < 0)
                    {
                        raycast_ind_x--;
                        wallSideStruckByRay = ResourceSide.East;

                        raycast_offset_x = map.TileWidth;
                    }
                    else
                    {
                        raycast_ind_x++;
                        wallSideStruckByRay = ResourceSide.West;

                        raycast_offset_x = 0;
                    }
                }
                else
                {
                    if (rayTraceDirectionX > 0)
                        triangle_x_length = map.TileWidth - raycast_offset_x;
                    else if (rayTraceDirectionX < 0)
                        triangle_x_length = raycast_offset_x == 0 ? map.TileWidth : raycast_offset_x;

                    triangle_hyp_length = triangle_x_length / MathHelpers.Cos(cameraAngleIndex);
                    triangle_y_length = MathHelpers.Sin(cameraAngleIndex) * triangle_hyp_length;

                    // Now determine if we need to move up and/or down on the map
                    if (triangle_y_length <= remaining_tile_y_length)
                    {
                        raycast_ind_x += rayTraceDirectionX;
                        wallSideStruckByRay = rayTraceDirectionX < 0 ? ResourceSide.East : ResourceSide.West;

                        raycast_offset_x = rayTraceDirectionX > 0 ? 0 : map.TileWidth;
                        raycast_offset_y += triangle_y_length * rayTraceDirectionY;
                    }
                    if (triangle_y_length >= remaining_tile_y_length)
                    {
                        raycast_ind_y += rayTraceDirectionY;
                        wallSideStruckByRay = rayTraceDirectionY < 0 ? ResourceSide.South : ResourceSide.North;

                        raycast_offset_y = rayTraceDirectionY > 0 ? 0 : map.TileWidth;

                        // Need to compute a triangle with the opposite leg being remaining_tile_y_length and the adjacent leg the amount to adjust raycast_offset_x
                        triangle_y_length = remaining_tile_y_length;
                        triangle_hyp_length = triangle_y_length / MathHelpers.Sin(cameraAngleIndex);
                        triangle_x_length = MathHelpers.Cos(cameraAngleIndex) * triangle_hyp_length;

                        raycast_offset_x += triangle_x_length * rayTraceDirectionX;
                    }
                }
            }
            else
            {
                // Going straight up or down
                if (rayTraceDirectionY < 0)
                {
                    raycast_ind_y--;
                    raycast_offset_y = map.TileWidth;
                }
                else
                {
                    raycast_ind_y++;
                    raycast_offset_y = 0;
                }
            }

            // Did we hit a wall?
            currentTileResource = map.GetTileResource(raycast_ind_x, raycast_ind_y);
        }

        Point2d raycastHit = new(raycast_ind_x + raycast_offset_x, raycast_ind_y + raycast_offset_y);
        double raycastLength = Point2d.GetLength(state.PlayerPos, raycastHit);

        int floorUpperBound = Math.Clamp(state.ScreenHeightHalved - (int)(state.CameraDistanceFromPlayer * state.CameraZ / raycastLength * state.ScreenHeight), 0, state.ScreenHeight);
        int wallUpperBound = Math.Clamp((int)(state.CameraDistanceFromPlayer * map.WallHeight / raycastLength * state.ScreenHeight) + floorUpperBound, 0, state.ScreenHeight);

        int bottomOfCeiling = state.ScreenHeight - wallUpperBound;
        if (bottomOfCeiling > 0)
        {
            if (bottomOfCeiling == state.ScreenHeight)
                bottomOfCeiling--;
            state.FillRectangle(currentColumnX, 0, (currentColumnX + colsPerIteration) - 1, bottomOfCeiling, map.GetCeilingResource().NorthColor);
        }

        int topOfWall = state.ScreenHeight - wallUpperBound;
        int bottomOfWall = state.ScreenHeight - floorUpperBound;
        if (bottomOfWall == state.ScreenHeight)
            bottomOfWall--;
        state.FillRectangle(currentColumnX, topOfWall, (currentColumnX + colsPerIteration) - 1, bottomOfWall, currentTileResource.GetColorForSide(wallSideStruckByRay));

        if (floorUpperBound > 0)
            state.FillRectangle(currentColumnX, state.ScreenHeight - floorUpperBound, (currentColumnX + colsPerIteration) - 1, state.ScreenHeight - 1, map.GetFloorResource().NorthColor);

        currentColumnX += colsPerIteration;
    }

    // Draw the right buffer, if needed
    if (leftBuffer > 0)
        state.FillRectangle(currentColumnX, 0, state.ScreenWidth - 1, state.ScreenHeight - 1, ColorArgb.Black());
}

void PresentPixelBuffer()
{
    SDL.SDL_RenderClear(state.Renderer);

    unsafe
    {
        fixed (int* pixels = state.Pixels)
        {
            SDL.SDL_UpdateTexture(
                state.Texture,
                IntPtr.Zero,
                (IntPtr)pixels,
                state.BytesPerRowOfPixelData
            );
        }
    }

    SDL.SDL_RenderCopy(state.Renderer, state.Texture, IntPtr.Zero, IntPtr.Zero);
    SDL.SDL_RenderPresent(state.Renderer);
}

void CleanUp()
{
    SDL.SDL_DestroyRenderer(state.Renderer);
    SDL.SDL_DestroyWindow(state.Window);
    SDL.SDL_Quit();
}



state.Initialize();
map.Load(state);

while (state.Running)
{
    ProcessEvents();
    UpdateGameState();
    RenderToPixelBuffer();
    PresentPixelBuffer();
}

CleanUp();