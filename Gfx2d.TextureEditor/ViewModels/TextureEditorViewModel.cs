using Gfx2d.Resources;
using Gfx2d.TextureEditor.Commands;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;

namespace Gfx2d.TextureEditor.ViewModels
{
    public class TextureEditorViewModel : INotifyPropertyChanged
    {
        private readonly TextureData _model;

        public TextureEditorViewModel(TextureData model, string? path = null)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _filePath = path;

            _selectedColor = new SolidColorBrush(Colors.Black);
        }


        #region IsDirty
        private bool _isDirty;
        public bool IsDirty
        {
            get => _isDirty;
            private set
            {
                if (_isDirty != value)
                {
                    _isDirty = value;
                    OnPropertyChanged(nameof(IsDirty));
                }
            }
        }
        #endregion

        #region SelectedColor
        public ICommand UpdateSelectedColorCommand => new RelayCommand<SolidColorBrush>(UpdateSelectedColor);
        private void UpdateSelectedColor(SolidColorBrush? color)
        {
            if (color != null)
            {
                SelectedColor = new SolidColorBrush(color.Color);
            }
        }

        private SolidColorBrush _selectedColor;
        public SolidColorBrush SelectedColor
        {
            get => _selectedColor;
            set
            {
                if (_selectedColor != value)
                {
                    _selectedColor = value;
                    IsDirty = true;
                    OnPropertyChanged(nameof(SelectedColor));
                }
            }
        }
        #endregion

        #region FilePath
        public bool HasBeenSaved => !string.IsNullOrEmpty(this.FilePath);

        private string? _filePath = null;
        public string? FilePath
        {
            get => _filePath;
            set
            {
                if (_filePath != value)
                {
                    _filePath = value;
                    IsDirty = true;
                    OnPropertyChanged(nameof(FilePath));
                    OnPropertyChanged(nameof(WindowTitle));
                }
            }
        }
        #endregion

        #region WindowTitle
        public string WindowTitle
        {
            get
            {
                string fileNameDisplay;

                if (string.IsNullOrEmpty(this.FilePath))
                    fileNameDisplay = "Unsaved file...";
                else
                    fileNameDisplay = System.IO.Path.GetFileNameWithoutExtension(this.FilePath);

                return $"Texture Editor - {fileNameDisplay}";
            }
        }
        #endregion

        #region PropertyChanged Event
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region Methods
        public void Save()
        {
            if (string.IsNullOrEmpty(this.FilePath)) throw new ArgumentNullException(nameof(this.FilePath));

            this.IsDirty = false;

            _model.Save(this.FilePath);
        }

        public void UpdateTextureData(int x, int y, Color color)
        {
            byte r = color.R;
            byte g = color.G;
            byte b = color.B;
            byte a = color.A;

            _model.North[x][y] = new int[4] { a, r, g, b };

            IsDirty = true;
        }

        public Color GetColor(int x, int y)
        {
            int[] data = _model.North[x][y];

            return Color.FromArgb((byte)data[0], (byte)data[1], (byte)data[2], (byte)data[3]);
        }
        #endregion
    }
}
