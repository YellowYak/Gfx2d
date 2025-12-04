using Gfx2d;
using Gfx2d.Resources;
using SDL2;
using System.Diagnostics;

const int SCREEN_WIDTH = 1287;
const int SCREEN_HEIGHT = 720;

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
        case SDL.SDL_Keycode.SDLK_z: state.Key_Z = KeyboardState.Pressed; break;

        case SDL.SDL_Keycode.SDLK_F12: state.ToggleFullscreen(); break;

        case SDL.SDL_Keycode.SDLK_ESCAPE:
            state.Running = false;
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
        case SDL.SDL_Keycode.SDLK_z: state.Key_Z = KeyboardState.Unpressed; break;
    }
}

void UpdateGameState()
{
    if (state.Key_Right == KeyboardState.Pressed) state.CameraAngleIndex += MathHelpers.RotationIndexDistanceWhenPlayerRotating;
    if (state.Key_Left == KeyboardState.Pressed) state.CameraAngleIndex -= MathHelpers.RotationIndexDistanceWhenPlayerRotating;

    if (state.Key_Z == KeyboardState.Pressed && state.CameraZ > GameState.FullyCrouchingCameraZ) state.CameraZ -= 0.02;
    if (state.Key_Z == KeyboardState.Unpressed && state.IsCrouching) state.CameraZ += 0.04;

    double playerMovementAcceleration = 0.05;
    if (state.IsCrouching)
        playerMovementAcceleration -= 0.02;

    Point2d newPlayerPos = new(state.PlayerPos);
    bool playerPosUpdated = false;

    if (state.Key_Up == KeyboardState.Pressed)
    {
        newPlayerPos.X += MathHelpers.Cos(state.CameraAngleIndex) * playerMovementAcceleration;
        newPlayerPos.Y += MathHelpers.Sin(state.CameraAngleIndex) * playerMovementAcceleration;
        playerPosUpdated = true;
    }

    if (state.Key_Down == KeyboardState.Pressed)
    {
        newPlayerPos.X -= MathHelpers.Cos(state.CameraAngleIndex) * playerMovementAcceleration;
        newPlayerPos.Y -= MathHelpers.Sin(state.CameraAngleIndex) * playerMovementAcceleration;
        playerPosUpdated = true;
    }

    if (state.Key_A == KeyboardState.Pressed)
    {
        int prIndex = state.CameraAngleIndex + MathHelpers.PiOverTwoIndex;
        newPlayerPos.X -= MathHelpers.Cos(prIndex) * playerMovementAcceleration;
        newPlayerPos.Y -= MathHelpers.Sin(prIndex) * playerMovementAcceleration;
        playerPosUpdated = true;
        Debug.Print($"Player pos: {newPlayerPos}, CAI={state.CameraAngleIndex}, prIndex={prIndex}, CAdeg={MathHelpers.GetPossibleRotationRadians(state.CameraAngleIndex) * 180 / Math.PI}, prDeg={MathHelpers.GetPossibleRotationRadians(prIndex) * 180 / Math.PI}");
    }

    if (state.Key_D == KeyboardState.Pressed)
    {
        int prIndex = state.CameraAngleIndex + MathHelpers.PiOverTwoIndex;
        newPlayerPos.X += MathHelpers.Cos(prIndex) * playerMovementAcceleration;
        newPlayerPos.Y += MathHelpers.Sin(prIndex) * playerMovementAcceleration;
        playerPosUpdated = true;
    }

    // TODO: Play a sound if they run into the wall
    // TODO: Play a sound if they run into the wall
    // TODO: Play a sound if they run into the wall
    // TODO: Play a sound if they run into the wall

    // TODO: Make this smarter so that if they are moving in multiple directions (e.g., UP + strafe) they can still move in the direction that doesn't slam them into the wall
    // TODO: Make this smarter so that if they are moving in multiple directions (e.g., UP + strafe) they can still move in the direction that doesn't slam them into the wall
    // TODO: Make this smarter so that if they are moving in multiple directions (e.g., UP + strafe) they can still move in the direction that doesn't slam them into the wall
    // TODO: Make this smarter so that if they are moving in multiple directions (e.g., UP + strafe) they can still move in the direction that doesn't slam them into the wall

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
    // TODO: Make this smarter so that the pixelsPerTileY is based on the screen's height/width ratio
    // TODO: Make this smarter so that the pixelsPerTileY is based on the screen's height/width ratio
    // TODO: Make this smarter so that the pixelsPerTileY is based on the screen's height/width ratio
    // TODO: Make this smarter so that the pixelsPerTileY is based on the screen's height/width ratio

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




// TODO: Refactor logic here to loop across the screen (0..ScreenWidth) rather than looping through the angles
// TODO: Refactor logic here to loop across the screen (0..ScreenWidth) rather than looping through the angles
// TODO: Refactor logic here to loop across the screen (0..ScreenWidth) rather than looping through the angles
// TODO: Refactor logic here to loop across the screen (0..ScreenWidth) rather than looping through the angles
// TODO: Refactor logic here to loop across the screen (0..ScreenWidth) rather than looping through the angles
// TODO: Refactor logic here to loop across the screen (0..ScreenWidth) rather than looping through the angles
// TODO: Refactor logic here to loop across the screen (0..ScreenWidth) rather than looping through the angles
// TODO: Refactor logic here to loop across the screen (0..ScreenWidth) rather than looping through the angles
// TODO: Refactor logic here to loop across the screen (0..ScreenWidth) rather than looping through the angles
// TODO: Refactor logic here to loop across the screen (0..ScreenWidth) rather than looping through the angles
// TODO: Refactor logic here to loop across the screen (0..ScreenWidth) rather than looping through the angles

void RenderFirstPersonViewToPixelBuffer()
{
    List<RaycastResult> raycastLengths = new(state.CameraSweepAngleIterations);

    int sweepAngleIterations = state.CameraSweepAngleIterations;

    // We may need to adjust how many angles we iterate through when performing the ray tracing across the camera.
    // Specifically, if there are more sweep iterations than columns on the screen then we need to drop some of the rendered columns
    List<int>? sweepAnglesToIgnore = null;
    if (sweepAngleIterations > state.ScreenWidth)
    {
        // Determine exactly how many iterations we need to drop and put them in an ordered array
        int startingAngleIndex = state.CameraAngleIndex - MathHelpers.Floor(sweepAngleIterations / 2);
        int numberOfItersToRemove = sweepAngleIterations - state.ScreenWidth;
        int spaceBetweenEachRemoval = sweepAngleIterations / numberOfItersToRemove;
        sweepAnglesToIgnore = new(numberOfItersToRemove);

        // The indices in sweepAnglesToIgnore will be skipped over when rendering
        for (int i = 0; i < numberOfItersToRemove; i++)
            sweepAnglesToIgnore.Add(spaceBetweenEachRemoval * i + startingAngleIndex);
    }

    // Determine the starting and stoppping camera angle indices
    int startCameraAngleIndex = state.CameraAngleIndex - MathHelpers.Floor(state.CameraSweepAngleIterations / 2);
    int endCameraAngleIndex = state.CameraAngleIndex + MathHelpers.Ceiling(state.CameraSweepAngleIterations / 2);

    // If the screen is significantly wider than the number of sweep angles then we may want to have each iteration render a column that is wider than just one pixel
    int colsPerIteration = Math.Max(1, state.ScreenWidth / state.CameraSweepAngleIterations);
    
    // Has the X coordinate of the current column being displayed on screen
    int currentColumnX = 0;

    // If the width taken up by the sweep is less than the width of the screen then determine the difference and center the output
    int usedScreenWidth = colsPerIteration * state.CameraSweepAngleIterations;
    int leftBuffer = 0;
    if (usedScreenWidth < state.ScreenWidth)
    {
        int delta = state.ScreenWidth - usedScreenWidth;

        // Start with a rectangle of black from the far left in order to center the display
        leftBuffer = delta / 2;
        state.FillRectangle(0, 0, leftBuffer, state.ScreenHeight - 1, ColorArgb.Black());

        // Update the currentColumnX
        currentColumnX = leftBuffer;
    }

    
    // Determine the X and Y map indices for the player, as well as the offset within the tile.
    int player_ind_x = (int)MathHelpers.Floor(state.PlayerPos.X / map.TileWidth);
    double player_offset_x = state.PlayerPos.X - map.TileWidth * player_ind_x;

    int player_ind_y = (int)MathHelpers.Floor(state.PlayerPos.Y / map.TileWidth);
    double player_offset_y = state.PlayerPos.Y - map.TileWidth * player_ind_y;


    // Here we loop through each angle in the sweep!
    for (int rawCameraAngleIndex = startCameraAngleIndex; rawCameraAngleIndex < endCameraAngleIndex; rawCameraAngleIndex++)
    {
        // Big picture, at each iteration we are shooting a "ray" from the player until it strikes a wall. We then compute the perpendicular distance
        // from the player and the wall and then use that to determine how much of the current column being drawn is composed of ceiling versus
        // wall versus floor.

        // Do we need to skip this angle index? If so, remove the entry from the front of the ordered array and then continue onto the next loop iteration
        if (sweepAnglesToIgnore != null && sweepAnglesToIgnore.Count > 0 && sweepAnglesToIgnore[0] == rawCameraAngleIndex)
        {
            sweepAnglesToIgnore.RemoveAt(0);
            continue;
        }

        // Next, make sure that the camera angle index we are working with on this iteration is kosher. It must be greater than or equal to 0 and strictly less than PossibleRotationRadiansLength
        // If this isn't the case, adjust accordingly.
        int cameraAngleIndex = rawCameraAngleIndex;
        if (cameraAngleIndex < 0)
            cameraAngleIndex = MathHelpers.PossibleRotationRadiansLength + cameraAngleIndex;
        else if (cameraAngleIndex > MathHelpers.PossibleRotationRadiansLength)
            cameraAngleIndex = cameraAngleIndex % MathHelpers.PossibleRotationRadiansLength;

        // Next, determine the ray's current X and Y map indices, as well as its offset within the tile.
        int raycast_ind_x = player_ind_x;
        double raycast_offset_x = player_offset_x;
        int raycast_ind_y = player_ind_y;
        double raycast_offset_y = player_offset_y;

        // We also want to know the X and Y direction of the ray, as this will be used to determine if we're moving up or down and right or left as we move from one tile to the next.
        int rayTraceDirectionX = MathHelpers.GetDirectionXFromRotationIndex(cameraAngleIndex);
        int rayTraceDirectionY = MathHelpers.GetDirectionYFromRotationIndex(cameraAngleIndex);

        // Next, for mathy reasons we want to have the angle as a value between 0..PI/2 where the X axis is the adjacent leg to the angle.
        // If the angle is already between 0..PI/2 then we are done! But otherwise we need to change the angle accordingly.
        if (cameraAngleIndex >= MathHelpers.PiOverTwoIndex && cameraAngleIndex < MathHelpers.PiIndex)        // Lower-left quadrant
            cameraAngleIndex = MathHelpers.PiOverTwoIndex - (cameraAngleIndex - MathHelpers.PiOverTwoIndex);
        else if (cameraAngleIndex >= MathHelpers.PiIndex && cameraAngleIndex < MathHelpers.ThreePiOverTwoIndex)  // Upper-left quadrant
            cameraAngleIndex = cameraAngleIndex % MathHelpers.PiIndex;
        else if (cameraAngleIndex >= MathHelpers.ThreePiOverTwoIndex)   // Upper-right quadrant
            cameraAngleIndex = MathHelpers.PossibleRotationRadiansLength - cameraAngleIndex;

        // Get the resource at the current tile. If this is null then the current tile is a floor, otherwise it's a wall.
        MapResource? currentTileResource = map.GetTileResource(raycast_ind_x, raycast_ind_y);

        // We also want to determine which direction we encountered the wall, as walls can have different displays for each wall face.
        ResourceSide wallSideStruckByRay = ResourceSide.North;

        // This loop sends the ray along its path from one tile to the next until it hits a wall.
        while (currentTileResource == null)
        {
            double triangle_x_length = 0;
            double triangle_y_length = 0;
            double triangle_hyp_length = 0;

            // Determine how much space on the Y axis exists within this tile given the Y direction the ray is heading.
            double remaining_tile_y_length = raycast_offset_y;
            if (rayTraceDirectionY < 0 && remaining_tile_y_length == 0)
                remaining_tile_y_length = map.TileWidth;
            if (rayTraceDirectionY > 0)
                remaining_tile_y_length = map.TileWidth - raycast_offset_y;

            if (rayTraceDirectionX != 0)
            {
                if (rayTraceDirectionY == 0)
                {
                    if (rayTraceDirectionX < 0)
                    {
                        // Going due west
                        
                        // Move the ray one tile to the left
                        raycast_ind_x--;

                        // Indicate that we hit the east side of the tile
                        wallSideStruckByRay = ResourceSide.East;

                        // In the new tile, set our X offset on the far right side
                        raycast_offset_x = map.TileWidth;
                    }
                    else
                    {
                        // Going due east

                        // Move the ray one tile to the right
                        raycast_ind_x++;

                        // Indicate that we hit the west side of the tile
                        wallSideStruckByRay = ResourceSide.West;

                        // In the new tile, set our X offset on the far left side
                        raycast_offset_x = 0;
                    }
                }
                else
                {
                    // We are heading at some angle (not due north, south, east, or west)

                    // Determine the X length of our right triangle based on the X direction we are heading
                    if (rayTraceDirectionX > 0)
                        triangle_x_length = map.TileWidth - raycast_offset_x;
                    else if (rayTraceDirectionX < 0)
                        triangle_x_length = raycast_offset_x == 0 ? map.TileWidth : raycast_offset_x;

                    // Use a little trig to compute the hypotenuse and opposite side length
                    triangle_hyp_length = triangle_x_length / MathHelpers.Cos(cameraAngleIndex);
                    triangle_y_length = MathHelpers.Sin(cameraAngleIndex) * triangle_hyp_length;

                    // Now we need to determine if we crossed the tile horizontally, vertically, or both (diagonally)
                    if (triangle_y_length <= remaining_tile_y_length)
                    {
                        // In this case, the length of the Y portion of the triangle is less than or equal to the amount of space we had on the Y axis in this tile.
                        // Therefore, the ray has struck the left or right of the tile before the top or bottom.

                        // Move the ray one tile to the right or left
                        raycast_ind_x += rayTraceDirectionX;

                        // Indicate what  side of the tile we hit
                        wallSideStruckByRay = rayTraceDirectionX < 0 ? ResourceSide.East : ResourceSide.West;

                        // Update the X and Y offsets in the new tile
                        raycast_offset_x = rayTraceDirectionX > 0 ? 0 : map.TileWidth;
                        raycast_offset_y += triangle_y_length * rayTraceDirectionY;
                    }
                    if (triangle_y_length >= remaining_tile_y_length)
                    {
                        // In this case, the length of the Y portion of the triangle is greater than or equal to the amount of space we had on the Y axis in this tile.
                        // Therefore, the ray has struck the top or bottom of the tile before the left or right.

                        // Move the ray one tile up or down
                        raycast_ind_y += rayTraceDirectionY;

                        // Indicate what  side of the tile we hit
                        wallSideStruckByRay = rayTraceDirectionY < 0 ? ResourceSide.South : ResourceSide.North;

                        // Update the Y offset in the new tile
                        raycast_offset_y = rayTraceDirectionY > 0 ? 0 : map.TileWidth;

                        // A little more math is needed to compute the X offset in the new tile.
                        // Need to draw a right triangle with the opposite leg being remaining_tile_y_length and the adjacent leg the amount to adjust raycast_offset_x
                        triangle_y_length = remaining_tile_y_length;
                        triangle_hyp_length = triangle_y_length / MathHelpers.Sin(cameraAngleIndex);
                        triangle_x_length = MathHelpers.Cos(cameraAngleIndex) * triangle_hyp_length;

                        raycast_offset_x += triangle_x_length * rayTraceDirectionX;
                    }
                }
            }
            else
            {
                if (rayTraceDirectionY < 0)
                {
                    // Going due north

                    // Move the ray one tile up
                    raycast_ind_y--;

                    // Indicate that we hit the south side of the tile
                    wallSideStruckByRay = ResourceSide.South;

                    // In the new tile, set our Y offset on the far bottom
                    raycast_offset_y = map.TileWidth;
                }
                else
                {
                    // Going due south

                    // Move the ray one tile down
                    raycast_ind_y++;

                    // Indicate that we hit the north side of the tile
                    wallSideStruckByRay = ResourceSide.North;

                    // In the new tile, set our Y offset on the far top
                    raycast_offset_y = 0;
                }
            }

            // Did the ray just now land on a wall tile?
            currentTileResource = map.GetTileResource(raycast_ind_x, raycast_ind_y);
        }

        // We've hit a wall! Huzzah! Determine the distance between the player and the point on the wall struck by the ray
        Point2d raycastHit = new(raycast_ind_x + raycast_offset_x, raycast_ind_y + raycast_offset_y);
        double raycastLength = Point2d.GetLength(state.PlayerPos, raycastHit);

        // For determining the wall height we want the -perpendicular- distance from the player to the hit wall.
        // This is the line looking straight out from the player's POV. To calculate its length, we just need
        // a sprinkle of trig - we know the hypotonuse length (raycastLength) - so the perpendicular distance is
        // the length of the adjacent leg. The angle formed is the delta between the ray's angle and the camera angle.
        int angleIndexDelta = Math.Abs(rawCameraAngleIndex - state.CameraAngleIndex);

        // Ensure we don't get a zero, as we'll be dividing by this number
        double perpDistance = Math.Max(0.0001, raycastLength * MathHelpers.Cos(angleIndexDelta));


        // Next determine how many pixels the floor and ceiling will occupy.
        int floorUpperBound = Math.Clamp(
            value: state.ScreenHeightHalved - (int)(state.CameraDistanceFromPlayer * state.CameraZ / perpDistance * state.ScreenHeight),
            min: 0,
            max: state.ScreenHeight
        );

        int wallUpperBound = Math.Clamp(
            value: (int)(state.CameraDistanceFromPlayer * map.WallHeight / perpDistance * state.ScreenHeight) + floorUpperBound,
            min: 0,
            max: state.ScreenHeight
        );

        // Map this to our coordinate system and start drawing!
        int bottomOfCeiling = state.ScreenHeight - wallUpperBound;
        if (bottomOfCeiling > 0)
        {
            if (bottomOfCeiling == state.ScreenHeight)
                bottomOfCeiling--;
            state.FillRectangle(currentColumnX, 0, (currentColumnX + colsPerIteration) - 1, bottomOfCeiling, map.GetCeilingResource().NorthColor);
        }

        int bottomOfWall = state.ScreenHeight - floorUpperBound;
        if (bottomOfWall == state.ScreenHeight)
            bottomOfWall--;
        state.FillRectangle(currentColumnX, bottomOfCeiling, (currentColumnX + colsPerIteration) - 1, bottomOfWall, currentTileResource.GetColorForSide(wallSideStruckByRay));

        if (floorUpperBound > 0)
            state.FillRectangle(currentColumnX, state.ScreenHeight - floorUpperBound, (currentColumnX + colsPerIteration) - 1, state.ScreenHeight - 1, map.GetFloorResource().NorthColor);


        // Record details about the raycast just performed.
        raycastLengths.Add(
            new RaycastResult
            {
                ColumnX = currentColumnX,
                RaycastLength = raycastLength,
                PerpendicularLength = perpDistance,
                FloorHeight = floorUpperBound,
                WallHeight = bottomOfWall - bottomOfCeiling,
                CeilingHeight = bottomOfCeiling,
                PlayerPos = state.PlayerPos,
                RaycastPoint = raycastHit
            }
        );

        // Move over to start drawing the next column(s) on screen
        currentColumnX += colsPerIteration;
    }

    // DebugHelpers.LogIfFileDoesNotExist(raycastLengths, @"C:\Users\scott\OneDrive\My Projects\Programming Projects\Gfx2d\DebugLog\raycast.log");

    // Draw the right buffer, if needed
    if (leftBuffer > 0)
        state.FillRectangle(currentColumnX, 0, state.ScreenWidth - 1, state.ScreenHeight - 1, ColorArgb.Black());
}

void PresentPixelBuffer()
{
    SDL.SDL_RenderClear(state.Renderer);

    // The following unsafe code blits the pixel array to the SDL texture object
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
map.Load(LevelData.LoadFromFile(@"C:\Users\scott\OneDrive\My Projects\Programming Projects\Gfx2d\Resources\Level1.json"), state);

// Main game loop - keep on chugging until the game is no longer running!
while (state.Running)
{
    ProcessEvents();
    UpdateGameState();
    RenderToPixelBuffer();
    PresentPixelBuffer();
}

CleanUp();