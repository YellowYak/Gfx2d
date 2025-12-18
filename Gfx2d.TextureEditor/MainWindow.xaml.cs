using Gfx2d.Resources;
using Gfx2d.TextureEditor.ViewModels;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Gfx2d.TextureEditor
{
    public partial class MainWindow : Window
    {
        TextureEditorViewModel ViewModel => (DataContext as TextureEditorViewModel)!;
        private Rectangle[,] pixels = new Rectangle[TextureData.TextureWidth, TextureData.TextureHeight];
        private bool isDrawing = false;

        public MainWindow()
        {
            InitializeComponent();

            LoadTextureEditorViewModel();            

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
        }
        void HandleCommandNew(object sender, ExecutedRoutedEventArgs e)
        {
            if (CanProceedWithUnsavedChanges())
                LoadTextureEditorViewModel();
        }

        void HandleCommandOpen(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Open Texture Data File",
                Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                DefaultExt = ".json",
                Multiselect = false
            };

            bool? result = dlg.ShowDialog(this);

            if (result == true && CanProceedWithUnsavedChanges())
            {
                string selectedFilePath = dlg.FileName;
                if (!TextureData.ValidFile(selectedFilePath))
                    MessageBox.Show(
                        messageBoxText: $"The texture data file you attempted to load - {selectedFilePath} - either does not exist, cannot be opened, or is an invalid texture data file.",
                        caption: "Missing or invalid texture data file",
                        button: MessageBoxButton.OK
                    );
                else
                {
                    LoadTextureEditorViewModel(selectedFilePath);
                }
            }
        }

        void HandleCommandSave(object sender, ExecutedRoutedEventArgs e) => ExecuteSaveCommand();
        bool ExecuteSaveCommand()
        {
            if (!ViewModel.HasBeenSaved)
                return ExecuteSaveAsCommand();
            else
                ViewModel.Save();

            return true;
        }

        void HandleCommandSaveAs(object sender, ExecutedRoutedEventArgs e) => ExecuteSaveAsCommand();
        bool ExecuteSaveAsCommand()
        {
            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Save Texture Data",
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
        }

        bool CanProceedWithUnsavedChanges()
        {
            return !ViewModel.IsDirty || MessageBox.Show(
                messageBoxText: "There are unsaved changes. Are you sure you want to continue? Any unsaved changes will be lost!",
                caption: "Unsaved Changes!",
                button: MessageBoxButton.YesNo
            ) == MessageBoxResult.Yes;
        }
        #endregion

        private void InitializePixelGrid()
        {
            pixelGrid.Children.Clear();

            for (int row = 0; row < TextureData.TextureHeight; row++)
            {
                for (int col = 0; col < TextureData.TextureWidth; col++)
                {
                    Rectangle pixel = new()
                    {
                        Fill = new SolidColorBrush(ViewModel.GetColor(row, col)),
                        Stroke = Brushes.LightGray,
                        StrokeThickness = 0.1,
                        Width = 8,
                        Height = 8
                    };

                    // Store reference in our 2D array
                    pixels[row, col] = pixel;

                    // Add to the UniformGrid
                    pixelGrid.Children.Add(pixel);
                }
            }
        }

        #region Pixel Drawing
        private void PixelGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDrawing = true;
            PaintPixel(e.GetPosition(pixelGrid));
        }

        private void PixelGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDrawing = true;
            PaintPixel(e.GetPosition(pixelGrid), true);
        }

        private void PixelGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
                PaintPixel(e.GetPosition(pixelGrid), e.RightButton == MouseButtonState.Pressed);
        }

        private void PixelGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDrawing = false;
        }
        private void PixelGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            UpdateCursorForCurrentTool();
        }

        private void PixelGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            pixelGrid.Cursor = null;
        }

        private void UpdateCursorForCurrentTool()
        {
            if (rbPencil.IsChecked == true)
                pixelGrid.Cursor = Cursors.Pen;
            else 
                pixelGrid.Cursor = null;
        }

        private void PaintPixel(Point position, bool? erase = null)
        {
            // Calculate which pixel was clicked based on position
            double pixelWidth = pixelGrid.ActualWidth / TextureData.TextureWidth;
            double pixelHeight = pixelGrid.ActualHeight / TextureData.TextureHeight;

            int col = (int)(position.X / pixelWidth);
            int row = (int)(position.Y / pixelHeight);

            // Bounds check
            if (row >= 0 && row < TextureData.TextureHeight && col >= 0 && col < TextureData.TextureWidth)
            {
                if (rbPencil.IsChecked == true && erase != true)
                {
                    pixels[row, col].Fill = ViewModel.SelectedColor;
                    ViewModel.UpdateTextureData(row, col, ViewModel.SelectedColor.Color);
                }
                else if (rbEraser.IsChecked == true || erase == true)
                {
                    pixels[row, col].Fill = Brushes.White;
                    ViewModel.UpdateTextureData(row, col, Brushes.White.Color);
                }
                else if (rbFill.IsChecked == true)
                {
                    FloodFill(row, col, ((SolidColorBrush)pixels[row, col].Fill).Color, ViewModel.SelectedColor.Color);
                }
            }
        }

        int iters = 0;
        private void FloodFill(int row, int col, Color targetColor, Color replacementColor)
        {
            Queue<(int, int)> pixelsToCheck = new();
            pixelsToCheck.Enqueue((row + 1, col));
            pixelsToCheck.Enqueue((row - 1, col));
            pixelsToCheck.Enqueue((row, col + 1));
            pixelsToCheck.Enqueue((row, col - 1));

            while (pixelsToCheck.Count > 0)
            {
                var (r, c) = pixelsToCheck.Dequeue();
                
                // Bounds check
                if (r < 0 || r >= TextureData.TextureHeight || c < 0 || c >= TextureData.TextureWidth)
                    continue;
                
                Color currentColor = ((SolidColorBrush)pixels[r, c].Fill).Color;
                if (currentColor == targetColor && currentColor != replacementColor)
                {
                    pixels[r, c].Fill = ViewModel.SelectedColor;
                    ViewModel.UpdateTextureData(r, c, ViewModel.SelectedColor.Color);

                    pixelsToCheck.Enqueue((r + 1, c));
                    pixelsToCheck.Enqueue((r - 1, c));
                    pixelsToCheck.Enqueue((r, c + 1));
                    pixelsToCheck.Enqueue((r, c - 1));
                }
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            for (int row = 0; row < TextureData.TextureHeight; row++)
            {
                for (int col = 0; col < TextureData.TextureWidth; col++)
                {
                    pixels[row, col].Fill = Brushes.White;
                    ViewModel.UpdateTextureData(row, col, Brushes.White.Color);
                }
            }
        }
        #endregion

        void LoadTextureEditorViewModel(string? path = null)
        {
            TextureData texture;

            if (string.IsNullOrEmpty(path))
                texture = new TextureData();
            else
                texture = TextureData.LoadFromFile(path);

            DataContext = new TextureEditorViewModel(texture, path);

            InitializePixelGrid();
        }
    }
}