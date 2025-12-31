# Gfx2d: A Simple Wolfenstein 3D Style Raycasting Engine using SDL2 in .NET / C#

This project is my implementation of a basic raycasting engine inspired by the classic Wolfenstein 3D game.
The engine is built using SDL2 to blit frames to a window. It is written in .NET / C# and demonstrates fundamental
concepts of 2D raycasting to create a pseudo-3D environment. It borrows heavily from the informative blog post by Tim Wheeler:
[Wolfenstein 3D Raycasting in C](https://timallanwheeler.com/blog/2023/04/01/wolfenstein-3d-raycasting-in-c/).

## Running the Project

Pre-requisites:

* Windows
* .NET 10
* Visual Studio

Open the Solution in Visual Studio. Set the appropriate project as the Start Up Project: **Gfx2d.Standalone** to "play" the game; or **Gfx2d.LevelEditor** to create or edit a level. Then Build and Run the project. (Note: presently the **Gfx2d.Standalone** project has a hard-coded path to the level JSON file - you'll need to update this manually.)

## Gfx2d.Engine

This Class Library project contains the core logic for the raycasting engine. It handles the keyboard input,
raycasting calculations, and rendering the scene. It is designed to be reusable and can be integrated into other projects.
In short, a project that hosts the Gfx2d.Engine library is responsible for creating the window and forwarding keyboard inputs to
the **Gfx2d.Engine**. The engine draws the scene to the providded window and updates the game state based on the keyboard inputs.

## Gfx2d.Resources

A Class Library project that contains classes used for loading and managing level data. This is in a separate project because it is
used both the engine (**Gfx2d.Engine**) and the level editor (**Gfx2d.LevelEditor**).

## Gfx2d.Standalone

This Windows Application project serves as a standalone executable that hosts the **Gfx2d.Engine** library. It simply creates an SDL2 window
and passes that instance to the **Gfx2d.Engine** for rendering. It also captures keyboard inputs and forwards them to the engine.

## Gfx2d.LevelEditor

A WPF application for creating and editing level maps.

## Major TODOs

* Look into improving raycasting logic for determining the position of the raycast in the subsequent map tile. Presently, the logic uses
  trigonometeric functions to find each tile boundary, which may be inefficient. Could use them once to determine the slope, then just use algebra to find the remaining tile boundaries?
* Update the engine to support textures for floors and ceilings.
* Implement sprite rendering for objects within the scene (enemies, items, etc.).
* Update **Gfx2d.LevelEditor** to allow placement of sprite starting locations on the map.
* Look into adding sound support in SDL2. E.g., play a sound when the player bumps into a wall.
* Update the engine to support drawing text on screen.
* Update **Gfx2d.Standalone** to allow the user to interactively select what level to load.
* Decide what in the heck I'm going to use this toy engine for! A FPS-type game? A puzzle-like game? Something else?

## Credits & Thanks

* Ideas & inspiration from Tim Wheeler's blog post, [Wolfenstein 3D Raycasting in C](https://timallanwheeler.com/blog/2023/04/01/wolfenstein-3d-raycasting-in-c/)
* PNG image processing from [SixLabors.ImageSharp](https://sixlabors.com/products/imagesharp/)
* Icons from [IconPacks](https://www.iconpacks.net/)
* Map textures from:

  * [CodeSpree from Seamless 64px RPG Tiles Pack Pack](https://codespree.itch.io/seamless-rpg-tiles) (CC BY-SA 4.0)
