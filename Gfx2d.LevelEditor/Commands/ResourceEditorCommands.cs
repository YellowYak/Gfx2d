using System.Windows.Input;

namespace Gfx2d.LevelEditor.Commands
{
    public static class LevelEditorCommands
    {
        public static readonly RoutedUICommand AddResource =
            new RoutedUICommand(
                text: "Add Resource",
                name: "AddResource",
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