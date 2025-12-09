using Gfx2d.Resources;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Gfx2d.ResourceEditor.ViewModels
{
    public class LevelDataViewModel : INotifyPropertyChanged
    {
        private readonly LevelData _model;

        public LevelDataViewModel(LevelData model, string? path = null)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _filePath = path;
        }

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
                    OnPropertyChanged(nameof(FilePath));
                    OnPropertyChanged(nameof(WindowTitle));
                }
            }
        }

        public string WindowTitle
        {
            get
            {
                string fileNameDisplay;

                if (string.IsNullOrEmpty(this.FilePath))
                    fileNameDisplay = "Unsaved file...";
                else
                    fileNameDisplay = System.IO.Path.GetFileNameWithoutExtension(this.FilePath);

                return $"Resource Editor - {fileNameDisplay}";
            }
        }

        public string Name
        {
            get => _model.Name;
            set
            {
                if (_model.Name != value)
                {
                    _model.Name = value;
                    IsDirty = true;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string Description
        {
            get => _model.Description;
            set
            {
                if (_model.Description != value)
                {
                    _model.Description = value;
                    IsDirty = true;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        public string MapSize
        {
            get
            {
                int width = _model.MapTiles.Max(tr => tr.Length);
                int height = _model.MapTiles.Length;

                return $"{width}x{height}";
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void Save()
        {
            if (string.IsNullOrEmpty(this.FilePath)) throw new ArgumentNullException(nameof(this.FilePath));

            this.IsDirty = false;

            _model.Save(this.FilePath);
        }
    }
}
