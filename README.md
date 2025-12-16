# Gfx2d: A Simple Wolfenstein 3D Style Raycasting Engine using SDL2 in .NET / C#

This project is my implementation of a basic raycasting engine inspired by the classic Wolfenstein 3D game.
The engine is built using SDL2 to blit frames to a window. It is written in .NET / C# and demonstrates fundamental
concepts of 2D raycasting to create a pseudo-3D environment. It borrows heavily from the informative blog post by Tim Wheeler:
[Wolfenstein 3D Raycasting in C](https://timallanwheeler.com/blog/2023/04/01/wolfenstein-3d-raycasting-in-c/).

## Gfx2d.Engine

This Class Library project contains the core logic for the raycasting engine. It handles the keyboard input,
raycasting calculations, and rendering the scene. It is designed to be reusable and can be integrated into other projects.
In short, a project that hosts the Gfx2d.Engine library is responsible for creating the window and forwarding keyboard inputs to
the **Gfx2d.Engine**. The engine draws the scene to the providded window and updates the game state based on the keyboard inputs.

## Gfx2d.Resources

A Class Library project that contains classes used for loading and managing level data. This is in a separate project because it is
used both the engin (**Gfx2d.Engine**) and the resource editor (**Gfx2d.ResourceEditor**).

## Gfx2d.Standalone

This Windows Application project serves as a standalone executable that hosts the **Gfx2d.Engine** library. It simply creates an SDL2 window
and passes that instance to the **Gfx2d.Engine** for rendering. It also captures keyboard inputs and forwards them to the engine.

## Gfx2d.ResourceEditor

A WPF application for creating and editing level maps.