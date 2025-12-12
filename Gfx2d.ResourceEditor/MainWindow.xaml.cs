using Gfx2d.ResourceEditor.Commands;
using Gfx2d.ResourceEditor.ViewModels;
using Gfx2d.Resources;
using SDL2;
using System.Reflection.Metadata.Ecma335;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Gfx2d.ResourceEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LevelDataViewModel ViewModel => (DataContext as LevelDataViewModel)!;

        private WpfGameHost? gameHost;
        private DispatcherTimer? gameTimer;
        private SdlHost? sdlHost;

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

            CommandBindings.Add(new CommandBinding(ResourceEditorCommands.AddResource, HandleAddResource));
            CommandBindings.Add(new CommandBinding(ResourceEditorCommands.ResizeMap, HandleResizeMap));
            CommandBindings.Add(new CommandBinding(ResourceEditorCommands.LaunchLevel, HandleLaunchLevel));
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


        void HandleCommandSave(object sender, ExecutedRoutedEventArgs e) => ExecuteSaveCommand();
        bool ExecuteSaveCommand()
        {
            if (!IsValidCheck()) return false;

            if (!ViewModel.HasBeenSaved)
                return ExecuteSaveAsCommand();
            else
                ViewModel.Save();

            return true;
        }

        void HandleCommandSaveAs(object sender, ExecutedRoutedEventArgs e) => ExecuteSaveAsCommand();
        bool ExecuteSaveAsCommand()
        {
            if (!IsValidCheck()) return false;

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
                return true;
            }

            return false;
        }


        // For the Application.Close command simply close the window straightaway.
        // The dirty check is done in the Window's Closing event handler.
        void HandleCommandClose(object sender, ExecutedRoutedEventArgs e) => Close();

        private void winMainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!CanProceedWithUnsavedChanges())
            {
                // Can't proceed, cancel closing
                e.Cancel = true;
            }
            else
            {
                // Window is closing, clean up game host if it exists
                CleanUpGameHost();
            }
        }
        #endregion

        void LoadLevelDataViewModel(string? path = null)
        {
            LevelData level;

            if (string.IsNullOrEmpty(path))
                level = new LevelData();
            else
                level = LevelData.LoadFromFile(path);

            DataContext = new LevelDataViewModel(level, path);
        }

        /// <summary>
        /// Checks to see if the view model is valid. If so, returns true. If not, shows a message box and returns false.
        /// </summary>
        bool IsValidCheck()
        {
            if (ViewModel.IsValid)
                return true;
            else
            {
                MessageBox.Show(
                    messageBoxText: "There are invalid inputs. These must be fixed before you can save.",
                    caption: "Invalid Data",
                    button: MessageBoxButton.OK,
                    icon: MessageBoxImage.Warning
                );
                return false;
            }
        }

        bool CanProceedWithUnsavedChanges()
        {
            return !ViewModel.IsDirty || MessageBox.Show(
                messageBoxText: "There are unsaved changes. Are you sure you want to continue? Any unsaved changes will be lost!",
                caption: "Unsaved Changes!",
                button: MessageBoxButton.YesNo
            ) == MessageBoxResult.Yes;
        }

        private void HandleAddResource(object sender, ExecutedRoutedEventArgs e)
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


        private void HandleResizeMap(object sender, ExecutedRoutedEventArgs e)
        {
            // Have the user select the resource JSON file
            var dialog = new ResizeMapInputDialog(mapWidth: ViewModel.MapWidth, mapHeight: ViewModel.MapHeight);
            dialog.Owner = this;

            if (dialog.ShowDialog() == true)
            {
                int newMapWidth = dialog.MapWidth;
                int newMapHeight = dialog.MapHeight;

                if (newMapWidth != ViewModel.MapWidth || newMapHeight != ViewModel.MapHeight)
                {
                    ViewModel.ResizeMap(newMapWidth, newMapHeight);
                }
            }
        }


        private void HandleLaunchLevel(object sender, ExecutedRoutedEventArgs e)
        {
            // Save the current level
            if (ExecuteSaveCommand())
            {
                // Switch to the Game Preview tab
                tabControl.SelectedItem = tabGamePreview;

                btnLaunchLevelTabButton.Visibility = Visibility.Hidden;
                gameHostBorder.Visibility = Visibility.Visible;

                CleanUpGameHost();

                // Use Dispatcher to ensure the tab content is fully loaded
                Dispatcher.BeginInvoke(new Action(InitializeGameHost), DispatcherPriority.Loaded);
            }
        }

        #region Game Rendering Logic
        private void InitializeGameHost()
        {
            // Use fixed dimensions
            const int gameWindowWidth = 1287;
            const int gameWindowHeight = 720;

            // Create the SDL host
            sdlHost = new SdlHost(gameWindowWidth, gameWindowHeight);

            // Add it to the border - this triggers BuildWindowCore
            gameHostBorder.Child = sdlHost;

            // Wait a bit longer for the window handle to be created
            Dispatcher.BeginInvoke(new Action(() =>
            {
                IntPtr handle = sdlHost.GetHandle();

                if (handle != IntPtr.Zero)
                {
                    // Initialize the game
                    gameHost = new WpfGameHost(handle, gameWindowWidth, gameWindowHeight);

                    gameHost.Initialize(ViewModel.FilePath!);

                    // Set up game loop timer
                    gameTimer = new DispatcherTimer();
                    gameTimer.Interval = TimeSpan.FromMilliseconds(10);
                    gameTimer.Tick += GameLoop_Tick;
                    gameTimer.Start();

                    // Make sure the SDL host has focus for keyboard input
                    sdlHost.Focus();
                }
                else
                {
                    MessageBox.Show("Failed to create SDL host window handle.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }), DispatcherPriority.ApplicationIdle);
        }

        private void GameLoop_Tick(object? sender, EventArgs e)
        {
            if (gameHost != null)
            {
                gameHost.Update();
                gameHost.Render();
            }
        }

        private void CleanUpGameHost()
        {
            // Clean up the game
            if (gameTimer != null)
            {
                gameTimer.Stop();
                gameTimer = null;
            }

            if (gameHost != null)
            {
                gameHost.Cleanup();
                gameHost = null;
            }

            if (sdlHost != null)
            {
                sdlHost = null;
            }
        }

        #region Keyboard Input Forwarding
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Only forward keys if the game preview tab is active and game is initialized and no input control is focused
            if (tabControl.SelectedItem == tabGamePreview && gameHost != null && !IsInputControlFocused())
            {
                SDL.SDL_Keycode sdlKey = WpfKeyToSDLKey(e.Key);
                if (sdlKey != SDL.SDL_Keycode.SDLK_UNKNOWN)
                {
                    gameHost.HandleKeyDown(sdlKey);
                    e.Handled = true; // Prevent WPF from processing this key
                }
            }
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            // Only forward keys if the game preview tab is active and game is initialized and no input control is focused
            if (tabControl.SelectedItem == tabGamePreview && gameHost != null && !IsInputControlFocused())
            {
                SDL.SDL_Keycode sdlKey = WpfKeyToSDLKey(e.Key);
                if (sdlKey != SDL.SDL_Keycode.SDLK_UNKNOWN)
                {
                    gameHost.HandleKeyUp(sdlKey);
                    e.Handled = true; // Prevent WPF from processing this key
                }
            }
        }

        /// <summary>
        /// Checks if the currently focused element is an input control (TextBox, etc.)
        /// </summary>
        private bool IsInputControlFocused()
        {
            IInputElement focusedElement = Keyboard.FocusedElement;

            // Check if focused element is a text input control
            return focusedElement is System.Windows.Controls.TextBox ||
                   focusedElement is System.Windows.Controls.RichTextBox ||
                   focusedElement is System.Windows.Controls.PasswordBox ||
                   focusedElement is System.Windows.Controls.ComboBox;
        }

        /// <summary>
        /// Converts a WPF Key to an SDL Keycode
        /// </summary>
        private SDL.SDL_Keycode WpfKeyToSDLKey(Key key)
        {
            return key switch
            {
                // Arrow keys
                Key.Left => SDL.SDL_Keycode.SDLK_LEFT,
                Key.Up => SDL.SDL_Keycode.SDLK_UP,
                Key.Right => SDL.SDL_Keycode.SDLK_RIGHT,
                Key.Down => SDL.SDL_Keycode.SDLK_DOWN,

                // Special keys
                Key.Escape => SDL.SDL_Keycode.SDLK_ESCAPE,
                Key.Tab => SDL.SDL_Keycode.SDLK_TAB,
                Key.F12 => SDL.SDL_Keycode.SDLK_F12,

                // Letter keys (A-Z)
                Key.A => SDL.SDL_Keycode.SDLK_a,
                Key.B => SDL.SDL_Keycode.SDLK_b,
                Key.C => SDL.SDL_Keycode.SDLK_c,
                Key.D => SDL.SDL_Keycode.SDLK_d,
                Key.E => SDL.SDL_Keycode.SDLK_e,
                Key.F => SDL.SDL_Keycode.SDLK_f,
                Key.G => SDL.SDL_Keycode.SDLK_g,
                Key.H => SDL.SDL_Keycode.SDLK_h,
                Key.I => SDL.SDL_Keycode.SDLK_i,
                Key.J => SDL.SDL_Keycode.SDLK_j,
                Key.K => SDL.SDL_Keycode.SDLK_k,
                Key.L => SDL.SDL_Keycode.SDLK_l,
                Key.M => SDL.SDL_Keycode.SDLK_m,
                Key.N => SDL.SDL_Keycode.SDLK_n,
                Key.O => SDL.SDL_Keycode.SDLK_o,
                Key.P => SDL.SDL_Keycode.SDLK_p,
                Key.Q => SDL.SDL_Keycode.SDLK_q,
                Key.R => SDL.SDL_Keycode.SDLK_r,
                Key.S => SDL.SDL_Keycode.SDLK_s,
                Key.T => SDL.SDL_Keycode.SDLK_t,
                Key.U => SDL.SDL_Keycode.SDLK_u,
                Key.V => SDL.SDL_Keycode.SDLK_v,
                Key.W => SDL.SDL_Keycode.SDLK_w,
                Key.X => SDL.SDL_Keycode.SDLK_x,
                Key.Y => SDL.SDL_Keycode.SDLK_y,
                Key.Z => SDL.SDL_Keycode.SDLK_z,

                // Number keys (0-9)
                Key.D0 => SDL.SDL_Keycode.SDLK_0,
                Key.D1 => SDL.SDL_Keycode.SDLK_1,
                Key.D2 => SDL.SDL_Keycode.SDLK_2,
                Key.D3 => SDL.SDL_Keycode.SDLK_3,
                Key.D4 => SDL.SDL_Keycode.SDLK_4,
                Key.D5 => SDL.SDL_Keycode.SDLK_5,
                Key.D6 => SDL.SDL_Keycode.SDLK_6,
                Key.D7 => SDL.SDL_Keycode.SDLK_7,
                Key.D8 => SDL.SDL_Keycode.SDLK_8,
                Key.D9 => SDL.SDL_Keycode.SDLK_9,

                _ => SDL.SDL_Keycode.SDLK_UNKNOWN
            };
        }
        #endregion
        #endregion
    }
}