using System.Windows.Input;

namespace Gfx2d.LevelEditor.Commands
{
    public static class LevelEditorCommands
    {
        public static readonly RoutedUICommand AddTextureResource =
            new RoutedUICommand(
                text: "Add Texture Resource",
                name: "AddTextureResource",
                ownerType: typeof(LevelEditorCommands)
            );

        public static readonly RoutedUICommand ResizeMap =
            new RoutedUICommand(
                text: "Resize Map",
                name: "ResizeMap",
                ownerType: typeof(LevelEditorCommands)
            );

        public static readonly RoutedUICommand LaunchLevel =
            new RoutedUICommand(
                text: "Launch Level",
                name: "LaunchLevel",
                ownerType: typeof(LevelEditorCommands)
            );
    }
}