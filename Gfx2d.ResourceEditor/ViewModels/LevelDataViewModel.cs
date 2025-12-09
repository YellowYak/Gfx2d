using Gfx2d.Resources;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Gfx2d.ResourceEditor.ViewModels
{
    public class LevelDataViewModel : INotifyPropertyChanged
    {
        private readonly LevelData _model;

        public LevelDataViewModel(LevelData model, string? path = null)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _filePath = path;

            _resourceReferences = new ObservableCollection<ResourceReference>(_model.TileResources);
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

        private ObservableCollection<ResourceReference> _resourceReferences;
        public ObservableCollection<ResourceReference> ResourceReferences
        {
            get => _resourceReferences;
            private set
            {
                _resourceReferences = value;
                OnPropertyChanged(nameof(ResourceReferences));
            }
        }


        private ResourceReference? _selectedResourceReference;
        public ResourceReference? SelectedResourceReference
        {
            get => _selectedResourceReference;
            set
            {
                _selectedResourceReference = value;
                OnPropertyChanged(nameof(SelectedResourceReference));
            }
        }

        public void RemoveResourceReference(ResourceReference resourceRef)
        {
            if (resourceRef == null) return;

            // See how many times the resource is used on the map
            int rrCount = _model.GetResourceReferenceUsageCount(resourceRef.Id);

            if (rrCount > 0)
            {
                MessageBoxResult result = MessageBox.Show(
                    $"Are you sure you want to remove resource '{resourceRef.FileName}' from the level data? This will remove -all- references to the resource from the map.",
                    "Confirm Removal",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (result != MessageBoxResult.Yes)
                    return;
            }

            // Remove the ResourceReference from both the view model and underlying model
            ResourceReferences.Remove(resourceRef);
            _model.TileResources = ResourceReferences.ToArray();

            // Remove all references to the resouce reference from the map, if any
            if (rrCount > 0)
                _model.RemoveResourceReferenceFromMap(resourceRef.Id);

            IsDirty = true;
        }

        public ICommand RemoveResourceCommand => new RelayCommand<ResourceReference>(RemoveResourceReference);


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
