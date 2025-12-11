using System.Windows.Input;

namespace Gfx2d.ResourceEditor.Commands
{
    public static class ResourceEditorCommands
    {
        public static readonly RoutedUICommand AddResource =
            new RoutedUICommand(
                text: "Add Resource",
                name: "AddResource",
                ownerType: typeof(ResourceEditorCommands)
            );

        public static readonly RoutedUICommand ResizeMap =
            new RoutedUICommand(
                text: "Resize Map",
                name: "ResizeMap",
                ownerType: typeof(ResourceEditorCommands)
            );
    }
}