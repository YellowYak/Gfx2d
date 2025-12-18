using Gfx2d.Resources;
using SDL2;

namespace Gfx2d.Engine
{
    public class GameEngine
    {
        private GameState state;
        private Map map;
        private MathHelpers MathHelpers { get; set; }

        public GameEngine(int screenWidth, int screenHeight)
        {
            MathHelpers = new MathHelpers(0.001);
            state = new GameState(MathHelpers, screenWidth, screenHeight);
            map = new Map();
        }

        public void Initialize(IGameHost host, string levelPath)
        {
            state.Initialize(host);
            map.Load(LevelData.LoadFromFile(levelPath), state);
        }

        public void Update()
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

                    if (map.GetMapTileTexture(indX, indY) != null)
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

        public void Render()
        {
            RenderToPixelBuffer();
            PresentPixelBuffer();
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

            // Clear out the background
            state.FillScreen(ColorArgb.Black());

            // Draw map
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    MapTexture? mapTexture = map.GetMapTileTexture(x, y);
                    ColorArgb c = mapTexture == null ?
                        map.FloorColor : 
                        mapTexture.NorthBitmap?.GetRepresentativeColor() ?? ColorArgb.Black();

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
#if DEBUG
            List<RaycastResult> raycastLengths = new(state.CameraSweepAngleIterations);
#endif

            int sweepAngleIterations = state.CameraSweepAngleIterations;
            
            // Creating local variables to improve performance in the tight loop below
            double tileWidth = map.TileWidth;
            double wallHeight = map.WallHeight;
            int screenWidth = state.ScreenWidth;
            int screenHeight = state.ScreenHeight;
            int screenHeightHalved = screenHeight / 2;

            // We may need to adjust how many angles we iterate through when performing the ray tracing across the camera.
            // Specifically, if there are more sweep iterations than columns on the screen then we need to drop some of the rendered columns
            List<int>? sweepAnglesToIgnore = null;
            if (sweepAngleIterations > screenWidth)
            {
                // Determine exactly how many iterations we need to drop and put them in an ordered array
                int startingAngleIndex = state.CameraAngleIndex - MathHelpers.Floor(sweepAngleIterations / 2);
                int numberOfItersToRemove = sweepAngleIterations - screenWidth;
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
            int colsPerIteration = Math.Max(1, screenWidth / state.CameraSweepAngleIterations);

            // Has the X coordinate of the current column being displayed on screen
            int currentColumnX = 0;

            // If the width taken up by the sweep is less than the width of the screen then determine the difference and center the output
            int usedScreenWidth = colsPerIteration * state.CameraSweepAngleIterations;
            int leftBuffer = 0;
            if (usedScreenWidth < screenWidth)
            {
                int delta = screenWidth - usedScreenWidth;

                // Start with a rectangle of black from the far left in order to center the display
                leftBuffer = delta / 2;
                state.FillRectangle(0, 0, leftBuffer, screenHeight - 1, ColorArgb.Black());

                // Update the currentColumnX
                currentColumnX = leftBuffer;
            }


            // Determine the X and Y map indices for the player, as well as the offset within the tile.
            int player_ind_x = (int)MathHelpers.Floor(state.PlayerPos.X / tileWidth);
            double player_offset_x = state.PlayerPos.X - tileWidth * player_ind_x;

            int player_ind_y = (int)MathHelpers.Floor(state.PlayerPos.Y / tileWidth);
            double player_offset_y = state.PlayerPos.Y - tileWidth * player_ind_y;


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
                MapTexture? currentTileTexture = map.GetMapTileTexture(raycast_ind_x, raycast_ind_y);

                // We also want to determine which direction we encountered the wall, as walls can have different displays for each wall face.
                ResourceSide wallSideStruckByRay = ResourceSide.N;

                // This loop sends the ray along its path from one tile to the next until it hits a wall.
                while (currentTileTexture == null)
                {
                    double triangle_x_length = 0;
                    double triangle_y_length = 0;
                    double triangle_hyp_length = 0;

                    // Determine how much space on the Y axis exists within this tile given the Y direction the ray is heading.
                    double remaining_tile_y_length = raycast_offset_y;
                    if (rayTraceDirectionY < 0 && remaining_tile_y_length == 0)
                        remaining_tile_y_length = tileWidth;
                    if (rayTraceDirectionY > 0)
                        remaining_tile_y_length = tileWidth - raycast_offset_y;

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
                                wallSideStruckByRay = ResourceSide.E;

                                // In the new tile, set our X offset on the far right side
                                raycast_offset_x = tileWidth;
                            }
                            else
                            {
                                // Going due east

                                // Move the ray one tile to the right
                                raycast_ind_x++;

                                // Indicate that we hit the west side of the tile
                                wallSideStruckByRay = ResourceSide.W;

                                // In the new tile, set our X offset on the far left side
                                raycast_offset_x = 0;
                            }
                        }
                        else
                        {
                            // We are heading at some angle (not due north, south, east, or west)

                            // Determine the X length of our right triangle based on the X direction we are heading
                            if (rayTraceDirectionX > 0)
                                triangle_x_length = tileWidth - raycast_offset_x;
                            else if (rayTraceDirectionX < 0)
                                triangle_x_length = raycast_offset_x == 0 ? tileWidth : raycast_offset_x;

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
                                wallSideStruckByRay = rayTraceDirectionX < 0 ? ResourceSide.E : ResourceSide.W;

                                // Update the X and Y offsets in the new tile
                                raycast_offset_x = rayTraceDirectionX > 0 ? 0 : tileWidth;
                                raycast_offset_y += triangle_y_length * rayTraceDirectionY;
                            }
                            if (triangle_y_length >= remaining_tile_y_length)
                            {
                                // In this case, the length of the Y portion of the triangle is greater than or equal to the amount of space we had on the Y axis in this tile.
                                // Therefore, the ray has struck the top or bottom of the tile before the left or right.

                                // Move the ray one tile up or down
                                raycast_ind_y += rayTraceDirectionY;

                                // Indicate what  side of the tile we hit
                                wallSideStruckByRay = rayTraceDirectionY < 0 ? ResourceSide.S : ResourceSide.N;

                                // Update the Y offset in the new tile
                                raycast_offset_y = rayTraceDirectionY > 0 ? 0 : tileWidth;

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
                            wallSideStruckByRay = ResourceSide.S;

                            // In the new tile, set our Y offset on the far bottom
                            raycast_offset_y = tileWidth;
                        }
                        else
                        {
                            // Going due south

                            // Move the ray one tile down
                            raycast_ind_y++;

                            // Indicate that we hit the north side of the tile
                            wallSideStruckByRay = ResourceSide.N;

                            // In the new tile, set our Y offset on the far top
                            raycast_offset_y = 0;
                        }
                    }

                    // Did the ray just now land on a wall tile?
                    currentTileTexture = map.GetMapTileTexture(raycast_ind_x, raycast_ind_y);
                }

                // We've hit a wall! Huzzah! Determine the distance between the player and the point on the wall struck by the ray
                Point2d raycastHit = new(raycast_ind_x + raycast_offset_x, raycast_ind_y + raycast_offset_y);
                double raycastLength = Point2d.GetLength(state.PlayerPos, raycastHit);

                // Determine shading level based on the raycast length
                double shading = raycastLength / 20;
                shading = Math.Clamp(shading, 0, 0.66);

                // For determining the wall height we want the -perpendicular- distance from the player to the hit wall.
                // This is the line looking straight out from the player's POV. (Note: a fisheye distortion occurs if the
                // actual distance from the player to the wall is used in place of the perpendicular distance.)
                // To calculate its length, we just need a sprinkle of trig - we know the hypotonuse length (raycastLength) -
                // so the perpendicular distance is the length of the adjacent leg. The angle formed is the delta between the
                // ray's angle and the camera angle.
                int angleIndexDelta = Math.Abs(rawCameraAngleIndex - state.CameraAngleIndex);

                // Ensure we don't get a zero, as we'll be dividing by this number
                double perpDistance = Math.Max(0.0001, raycastLength * MathHelpers.Cos(angleIndexDelta));


                // Next determine how many pixels the floor and ceiling will occupy.
                int floorUpperBoundUnclamped = screenHeightHalved - (int)(state.CameraDistanceFromPlayer * state.CameraZ / perpDistance * screenHeight);
                int floorUpperBound = Math.Clamp(
                    value: floorUpperBoundUnclamped,
                    min: 0,
                    max: screenHeight
                );

                int wallUpperBoundUnclamped = (int)(state.CameraDistanceFromPlayer * wallHeight / perpDistance * screenHeight) + floorUpperBoundUnclamped;
                int wallUpperBound = Math.Clamp(
                    value: wallUpperBoundUnclamped,
                    min: 0,
                    max: screenHeight
                );

                // Map this to our coordinate system and start drawing!
                int bottomOfCeiling = screenHeight - wallUpperBound;
                if (bottomOfCeiling > 0)
                {
                    if (bottomOfCeiling == screenHeight)
                        bottomOfCeiling--;

                    state.FillRectangle(
                        currentColumnX,
                        0,
                        (currentColumnX + colsPerIteration) - 1,
                        bottomOfCeiling,
                        map.CeilingColor
                    );
                }

                int bottomOfWall = screenHeight - floorUpperBound;
                if (bottomOfWall == screenHeight)
                    bottomOfWall--;

                // Calculate which portion of the texture is actually visible
                int fullWallHeight = wallUpperBoundUnclamped - floorUpperBoundUnclamped;

                // The wall is a texture rather than a solid color, so we need to determine which column of the texture to use based on where the ray struck the wall
                double offsetOnWall = wallSideStruckByRay == ResourceSide.N || wallSideStruckByRay == ResourceSide.S ? raycast_offset_x : raycast_offset_y;
                Bitmap texture = currentTileTexture.GetBitmapForSide(wallSideStruckByRay);

                // Determine the corresponding X coordinate on the texture
                int textureX = (int)(offsetOnWall / tileWidth * texture.Width);
                textureX = Math.Clamp(textureX, 0, texture.Width - 1);

                // Get the column of colors on the texture for the textureX we're currently rendering
                ColorArgb[] textureColumn = texture.GetColorColumn(wallSideStruckByRay, textureX);

                for (int y = bottomOfCeiling; y < bottomOfWall; y++)
                {
                    // How many pixels from the top of the THEORETICAL wall is screen position y?
                    // Top of theoretical wall in screen coords: screenHeight - wallUpperBoundUnclamped
                    int pixelsFromTopOfWall = y - (screenHeight - wallUpperBoundUnclamped);

                    double textureYRatio = (double)pixelsFromTopOfWall / (double)fullWallHeight;
                    int textureY = (int)(textureYRatio * texture.Height);
                    textureY = Math.Clamp(textureY, 0, texture.Height - 1);

                    // Get the apporpriate color from the texture color column and apply shading
                    ColorArgb texelColor = textureColumn[textureY].ApplyShading(shading);

                    // Draw the pixel column
                    for (int colX = currentColumnX; colX < currentColumnX + colsPerIteration; colX++)
                    {
                        state.SetPixel(colX, y, texelColor);
                    }
                }

                if (floorUpperBound > 0)
                    state.FillRectangle(
                        currentColumnX,
                        screenHeight - floorUpperBound,
                        (currentColumnX + colsPerIteration) - 1,
                        screenHeight - 1,
                        map.FloorColor
                    );


#if DEBUG
                // Record details about the raycast just performed.
                raycastLengths.Add(
                    new RaycastResult
                    {
                        ColumnX = currentColumnX,
                        RaycastLength = raycastLength,
                        PerpendicularLength = perpDistance,
                        Shading = shading,
                        FloorHeight = floorUpperBound,
                        WallHeight = bottomOfWall - bottomOfCeiling,
                        CeilingHeight = bottomOfCeiling,
                        PlayerPos = state.PlayerPos,
                        RaycastPoint = raycastHit
                    }
                );
#endif


                // Move over to start drawing the next column(s) on screen
                currentColumnX += colsPerIteration;
            }

#if DEBUG
            // DebugHelpers.LogIfFileDoesNotExist(raycastLengths, @"C:\Users\scott\OneDrive\My Projects\Programming Projects\Gfx2d\DebugLog\raycast.log");
#endif

            // Draw the right buffer, if needed
            if (leftBuffer > 0)
                state.FillRectangle(currentColumnX, 0, screenWidth - 1, screenHeight - 1, ColorArgb.Black());
        }

        public void PresentPixelBuffer()
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

        public bool HandleKeyDown(SDL.SDL_Keycode key)
        {
            switch (key)
            {
                case SDL.SDL_Keycode.SDLK_RIGHT: state.Key_Right = KeyboardState.Pressed; break;
                case SDL.SDL_Keycode.SDLK_LEFT: state.Key_Left = KeyboardState.Pressed; break;
                case SDL.SDL_Keycode.SDLK_UP: state.Key_Up = KeyboardState.Pressed; break;
                case SDL.SDL_Keycode.SDLK_DOWN: state.Key_Down = KeyboardState.Pressed; break;
                case SDL.SDL_Keycode.SDLK_a: state.Key_A = KeyboardState.Pressed; break;
                case SDL.SDL_Keycode.SDLK_d: state.Key_D = KeyboardState.Pressed; break;
                case SDL.SDL_Keycode.SDLK_z: state.Key_Z = KeyboardState.Pressed; break;

                case SDL.SDL_Keycode.SDLK_F12: state.ToggleFullscreen(); break;

                // Quit on ESCAPE
                case SDL.SDL_Keycode.SDLK_ESCAPE: return false;

                case SDL.SDL_Keycode.SDLK_TAB:
                    state.ToggleCameraView();
                    break;
            }

            return true;
        }

        public void HandleKeyUp(SDL.SDL_Keycode key)
        {
            switch (key)
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

        public void Cleanup()
        {
            SDL.SDL_DestroyRenderer(state.Renderer);
            SDL.SDL_DestroyTexture(state.Texture);

            state.Cleanup();
        }
    }
}
