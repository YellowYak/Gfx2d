using System.Windows.Input;

namespace Gfx2d.ResourceEditor.Commands
{
    public static class ResourceEditorCommands
    {
        public static readonly RoutedUICommand AddResource =
            new RoutedUICommand(
                "Add Resource",
                "AddResource",
                typeof(ResourceEditorCommands)
            );
    }
}