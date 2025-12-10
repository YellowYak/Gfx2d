using Gfx2d.ResourceEditor.Commands;
using Gfx2d.ResourceEditor.ViewModels;
using Gfx2d.Resources;
using System.Windows;
using System.Windows.Input;

namespace Gfx2d.ResourceEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LevelDataViewModel ViewModel => (DataContext as LevelDataViewModel)!;

        public MainWindow()
        {
            InitializeComponent();

            LoadLevelDataViewModel();

            InitializeCommands();
        }

        #region Commands
        void InitializeCommands()
        {
            CommandBindings.Add(new CommandBinding(ApplicationCommands.New, HandleCommandNew));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, HandleCommandOpen));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, HandleCommandSave));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.SaveAs, HandleCommandSaveAs));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, HandleCommandClose));

            CommandBindings.Add(new CommandBinding(ResourceEditorCommands.AddResource, ExecuteAddResource));
        }

        void HandleCommandNew(object sender, ExecutedRoutedEventArgs e)
        {
            if (CanProceedWithUnsavedChanges())
                LoadLevelDataViewModel();
        }

        void HandleCommandOpen(object sender, ExecutedRoutedEventArgs e)
        {

            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Open Level Data File",
                Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                DefaultExt = ".json",
                Multiselect = false
            };

            bool? result = dlg.ShowDialog(this);

            if (result == true && CanProceedWithUnsavedChanges())
            {
                string selectedFilePath = dlg.FileName;
                if (!LevelData.ValidFile(selectedFilePath))
                    MessageBox.Show(
                        messageBoxText: $"The level data file you attempted to load - {selectedFilePath} - either does not exist, cannot be opened, or is an invalid level data file.",
                        caption: "Missing or invalid level data file",
                        button: MessageBoxButton.OK
                    );
                else
                {
                    LoadLevelDataViewModel(selectedFilePath);
                }
            }
        }

        void HandleCommandSave(object sender, ExecutedRoutedEventArgs e)
        {
            if (!ViewModel.HasBeenSaved)
                HandleCommandSaveAs(sender, e);
            else
                ViewModel.Save();
        }

        void HandleCommandSaveAs(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Save Level Data",
                Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                DefaultExt = ".json"
            };

            bool? result = dlg.ShowDialog(this);

            if (result == true)
            {
                ViewModel.FilePath = dlg.FileName;
                ViewModel.Save();
            }
        }

        void HandleCommandClose(object sender, ExecutedRoutedEventArgs e)
        {
            if (CanProceedWithUnsavedChanges())
                Close();
        }
        #endregion

        void LoadLevelDataViewModel(string? path = null)
        {
            LevelData? level = null;

            if (string.IsNullOrEmpty(path))
                level = new LevelData();
            else
                level = LevelData.LoadFromFile(path);

            DataContext = new LevelDataViewModel(level!, path);
        }

        bool CanProceedWithUnsavedChanges()
        {
            return !ViewModel.IsDirty || MessageBox.Show(
                messageBoxText: "There are unsaved changes. Are you sure you want to continue? Any unsaved changes will be lost!",
                caption: "Unsaved Changes!",
                button: MessageBoxButton.YesNo
            ) == MessageBoxResult.Yes;
        }

        private void ExecuteAddResource(object sender, ExecutedRoutedEventArgs e)
        {
            // Have the user select the resource JSON file
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Open Resource File",
                Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                DefaultExt = ".json",
                Multiselect = true
            };

            bool? result = dlg.ShowDialog(this);

            if (result == true)
            {
                try
                {
                    ViewModel.AddResourceReferenceCommand.Execute(dlg.FileNames);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        messageBoxText: ex.Message,
                        caption: "Error Loading Resource File",
                        button: MessageBoxButton.OK,
                        icon: MessageBoxImage.Error
                    );
                }
            }
        }
    }
}