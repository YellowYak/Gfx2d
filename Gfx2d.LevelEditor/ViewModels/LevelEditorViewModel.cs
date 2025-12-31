using Gfx2d.LevelEditor.Commands;
using Gfx2d.LevelEditor.Extensions;
using Gfx2d.Resources;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Gfx2d.LevelEditor.ViewModels
{
    public class LevelEditorViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private readonly LevelData _model;

        public LevelEditorViewModel(LevelData model, string? path = null)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _filePath = path;

            _ceilingColor = Color.FromArgb(
                _model.CeilingColor[0],
                _model.CeilingColor[1],
                _model.CeilingColor[2],
                _model.CeilingColor[3]
            );

            _floorColor = Color.FromArgb(
                _model.FloorColor[0],
                _model.FloorColor[1],
                _model.FloorColor[2],
                _model.FloorColor[3]
            );

            _resourceReferences = new ObservableCollection<ResourceReference>(_model.MapTextures);

            ConstructMapCellsCollection();            
        }

        #region Data Validation
        public string Error => string.Empty;

        public string this[string columnName]
        {
            get
            {
                string result = string.Empty;

                if (columnName == nameof(Name))
                {
                    if (string.IsNullOrWhiteSpace(Name))
                        result = "Name is required.";
                }
                else if (columnName == nameof(TileWidthText))
                {
                    if (string.IsNullOrWhiteSpace(TileWidthText))
                        result = "Tile Size is required.";
                    else if (!double.TryParse(TileWidthText, out double tw))
                        result = "Tile Size must be a valid number.";
                    else if (tw <= 0)
                        result = "Tile Size must be greater than 0.";
                }
                else if (columnName == nameof(WallHeightText))
                {
                    if (string.IsNullOrWhiteSpace(WallHeightText))
                        result = "Wall Height is required.";
                    else if (!double.TryParse(WallHeightText, out double wh))
                        result = "Wall Height must be a valid number.";
                    else if (wh <= 0)
                        result = "Wall Height must be greater than 0.";
                }
                else if (columnName == nameof(PlayerPosXText))
                {
                    if (string.IsNullOrWhiteSpace(PlayerPosXText))
                        result = "Player Position X is required.";
                    else if (!double.TryParse(PlayerPosXText, out double pp))
                        result = "Player Position X must be a valid number.";
                    else if (pp < 0)
                        result = "Player Position X must be greater than 0.";
                    else if (pp > MapWidth)
                        result = $"Player Position X must be less than or equal to {MapWidth}.";
                }
                else if (columnName == nameof(PlayerPosYText))
                {
                    if (string.IsNullOrWhiteSpace(PlayerPosYText))
                        result = "Player Position Y is required.";
                    else if (!double.TryParse(PlayerPosYText, out double pp))
                        result = "Player Position Y must be a valid number.";
                    else if (pp < 0)
                        result = "Player Position Y must be greater than 0.";
                    else if (pp > MapHeight)
                        result = $"Player Position Y must be less than or equal to {MapHeight}.";
                }
                else if (columnName == nameof(CameraDirectionAngleText))
                {
                    if (string.IsNullOrWhiteSpace(CameraDirectionAngleText))
                        result = "Player Position Angle is required.";
                    else if (!double.TryParse(CameraDirectionAngleText, out double pp))
                        result = "Player Position Angle must be a valid number.";
                    else if (pp < 0)
                        result = "Player Position Angle must be greater than 0.";
                    else if (pp > 6.28)
                        result = $"Player Position Y must be less than or equal to 6.28.";
                }

                return result;
            }
        }

        public bool IsValid =>
                string.IsNullOrEmpty(this[nameof(Name)]) &&
                string.IsNullOrEmpty(this[nameof(TileWidthText)]) &&
                string.IsNullOrEmpty(this[nameof(WallHeightText)]) &&
                string.IsNullOrEmpty(this[nameof(PlayerPosXText)]) &&
                string.IsNullOrEmpty(this[nameof(PlayerPosYText)]) &&
                string.IsNullOrEmpty(this[nameof(CameraDirectionAngleText)]);
        #endregion

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

                return $"Level Editor - {fileNameDisplay} ({MapSize})";
            }
        }
        #endregion

        #region Name
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
        #endregion

        #region Tile Width
        private string? _tileWidthText;
        public string TileWidthText
        {
            get => _tileWidthText ?? TileWidth.ToString();
            set
            {
                _tileWidthText = value;
                OnPropertyChanged(nameof(TileWidthText));

                // Only update the actual double if valid
                if (double.TryParse(value, out double result))
                {
                    TileWidth = result;
                }
            }
        }

        public double TileWidth
        {
            get => _model.Dimensions.TileWidth;
            set
            {
                if (_model.Dimensions.TileWidth != value)
                {
                    _model.Dimensions.TileWidth = value;
                    IsDirty = true;
                    OnPropertyChanged(nameof(TileWidth));
                }
            }
        }
        #endregion

        #region Wall Height

        private string? _wallHeightText = null;
        public string WallHeightText
        {
            get => _wallHeightText ?? WallHeight.ToString();
            set
            {
                _wallHeightText = value;
                OnPropertyChanged(nameof(WallHeightText));

                // Only update the actual double if valid
                if (double.TryParse(value, out double result))
                {
                    WallHeight = result;
                }
            }
        }

        public double WallHeight
        {
            get => _model.Dimensions.WallHeight;
            set
            {
                if (_model.Dimensions.WallHeight != value)
                {
                    _model.Dimensions.WallHeight = value;
                    IsDirty = true;
                    OnPropertyChanged(nameof(WallHeight));
                }
            }
        }
        #endregion

        #region Description
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
        #endregion

        #region Ceiling Color
        private Color _ceilingColor;
        public Color CeilingColor
        {
            get => _ceilingColor;
            set
            {
                if (_ceilingColor != value)
                {
                    _ceilingColor = value;
                    _model.CeilingColor = value.ToByteArray();
                    IsDirty = true;
                    OnPropertyChanged(nameof(CeilingColor));
                }
            }
        }
        #endregion

        #region Floor Color
        private Color _floorColor;
        public Color FloorColor
        {
            get => _floorColor;
            set
            {
                if (_floorColor != value)
                {
                    _floorColor = value;
                    _model.FloorColor = value.ToByteArray();
                    IsDirty = true;
                    OnPropertyChanged(nameof(FloorColor));
                    
                    ConstructMapCellsCollection();            
                    OnPropertyChanged(nameof(MapCells));
                }
            }
        }
        #endregion

        #region Player Position
        private string? _playerPosXText;
        public string PlayerPosXText
        {
            get => _playerPosXText ?? PlayerPosX.ToString();
            set
            {
                _playerPosXText = value;
                OnPropertyChanged(nameof(PlayerPosXText));

                // Only update the actual double if valid
                if (double.TryParse(value, out double result))
                {
                    PlayerPosX = result;
                }
            }
        }

        public double PlayerPosX
        {
            get => _model.StartingPosition.PlayerPos.X;
            set
            {
                if (_model.StartingPosition.PlayerPos.X != value)
                {
                    _model.StartingPosition.PlayerPos.X = value;
                    IsDirty = true;
                    OnPropertyChanged(nameof(PlayerPosX));
                    OnPropertyChanged(nameof(PlayerPosXText));
                    OnPropertyChanged(nameof(PlayerCircleLeft));
                }
            }
        }

        private string? _playerPosYText;
        public string PlayerPosYText
        {
            get => _playerPosYText ?? PlayerPosY.ToString();
            set
            {
                _playerPosYText = value;
                OnPropertyChanged(nameof(PlayerPosYText));

                // Only update the actual double if valid
                if (double.TryParse(value, out double result))
                {
                    PlayerPosY = result;
                }
            }
        }

        public double PlayerPosY
        {
            get => _model.StartingPosition.PlayerPos.Y;
            set
            {
                if (_model.StartingPosition.PlayerPos.Y != value)
                {
                    _model.StartingPosition.PlayerPos.Y = value;
                    IsDirty = true;
                    OnPropertyChanged(nameof(PlayerPosY));
                    OnPropertyChanged(nameof(PlayerPosYText));
                    OnPropertyChanged(nameof(PlayerCircleTop));
                }
            }
        }

        private string? _cameraDirectionAngleText;
        public string CameraDirectionAngleText
        {
            get => _cameraDirectionAngleText ?? CameraDirectionAngle.ToString();
            set
            {
                _cameraDirectionAngleText = value;
                OnPropertyChanged(nameof(CameraDirectionAngleText));

                // Only update the actual double if valid
                if (double.TryParse(value, out double result))
                {
                    CameraDirectionAngle = result;
                }
            }
        }

        public double CameraDirectionAngle
        {
            get => _model.StartingPosition.CameraAngleRads;
            set
            {
                if (_model.StartingPosition.CameraAngleRads != value)
                {
                    _model.StartingPosition.CameraAngleRads = value;
                    IsDirty = true;
                    OnPropertyChanged(nameof(CameraDirectionAngle));
                    OnPropertyChanged(nameof(CameraDirectionAngleDegrees));
                    OnPropertyChanged(nameof(CameraDirectionAngleText));
                }
            }
        }

        /// <summary>
        /// Returns the Camera Direction Angle in degrees.
        /// </summary>
        public double CameraDirectionAngleDegrees => CameraDirectionAngle * (180 / Math.PI);

        public ICommand UpdatePlayerPositionCommand => new RelayCommand<Point>(UpdatePlayerPosition);
        private void UpdatePlayerPosition(Point gridPosition)
        {
            // Update the player position properties
            // This will automatically update the UI through data binding
            PlayerPosX = Math.Round(gridPosition.X, 2);
            PlayerPosY = Math.Round(gridPosition.Y, 2);

            // Also update the text fields
            _playerPosXText = null;
            _playerPosYText = null;
            OnPropertyChanged(nameof(PlayerPosXText));
            OnPropertyChanged(nameof(PlayerPosYText));

            IsDirty = true;
        }
        #endregion

        #region Map Size
        public int MapWidth => _model.MapTiles.Max(tr => tr.Length);
        public int MapHeight => _model.MapTiles.Length;

        public string MapSize => $"{MapWidth}x{MapHeight}";
        #endregion

        #region Map Cells
        const double DefaultGridCellWidth = 50;
        public double GridCellWidth => DefaultGridCellWidth * (MapEditorZoomLevel / 100);

        const double DefaultGridCellHeight = 50;
        public double GridCellHeight => DefaultGridCellHeight * (MapEditorZoomLevel / 100);

        public double MapPixelWidth => MapWidth * GridCellWidth;
        
        public double MapPixelHeight => MapHeight * GridCellHeight;

        public double PlayerCircleLeft => PlayerPosX * GridCellWidth;
        public double PlayerCircleTop => PlayerPosY * GridCellHeight;


        private double _mapEditorZoomLevel = 100;
        public double MapEditorZoomLevel
        {
            get => _mapEditorZoomLevel;
            set
            {
                _mapEditorZoomLevel = value;

                OnPropertyChanged(nameof(MapEditorZoomLevel));
                OnPropertyChanged(nameof(MapEditorZoomLevelDisplay));

                OnPropertyChanged(nameof(GridCellWidth));
                OnPropertyChanged(nameof(GridCellHeight));
                OnPropertyChanged(nameof(MapPixelWidth));
                OnPropertyChanged(nameof(MapPixelHeight));
                OnPropertyChanged(nameof(PlayerCircleLeft));
                OnPropertyChanged(nameof(PlayerCircleTop));
            }
        }

        public string MapEditorZoomLevelDisplay => $"Zoom ({MapEditorZoomLevel:N0}%): ";

        private ObservableCollection<MapCellViewModel> _mapCells = new();
        public ObservableCollection<MapCellViewModel> MapCells
        {
            get => _mapCells;
            private set
            {
                _mapCells = value;
                OnPropertyChanged(nameof(MapCells));
            }
        }

        /// <summary>
        /// The status message to display in the map editor status bar.
        /// Returns the currently selected resource reference, or a default message if none is selected.
        /// </summary>
        public string MapEditorStatusMessage
        {
            get
            {
                if (this.SelectedResourceReference == null)
                    return "No resource selected. Clicking a cell will set it to 'Floor' (0).";
                else
                    return $"Selected resource: '{this.SelectedResourceReference.FileName}' (ID: {this.SelectedResourceReference.Id})";
            }
        }

        /// <summary>
        /// This command fires when the user clicks the left mouse button over a cell in the map editor,
        /// or has the left button pressed when mousing over a cell in the map editor.
        /// </summary>
        public ICommand PaintCellCommand => new RelayCommand<MapCellViewModel>(PaintCell);
        private void PaintCell(MapCellViewModel cell)
        {
            if (cell == null) return;

            // If a resource reference is currently selected then use its Id, otherwise use 0 (Floor)
            int newValue = SelectedResourceReference?.Id ?? 0;

            if (cell.Value == newValue)
                return;

            // If we reach here the user has just changed a cell in the map
            // Update the underlying model
            _model.MapTiles[cell.Row][cell.Column] = newValue;

            // Get the resource data for the new value
            Dictionary<int, MapTexture> textureData = _model.GetMapTextures();
            ColorArgb newColor = textureData.ContainsKey(newValue)
                ? textureData[newValue].GetRepresentativeColor() ?? ColorArgb.Black()
                : ColorArgb.LightGray();

            // Update the cell view model
            cell.Value = newValue;
            cell.CellBrush = newColor.ToBrush();

            IsDirty = true;
        }

        /// <summary>
        /// This command fires when the user clicks the right mouse button over a cell in the map editor,
        /// or has the right button pressed when mousing over a cell in the map editor.
        /// </summary>
        public ICommand EraseCellCommand => new RelayCommand<MapCellViewModel>(EraseCell);
        private void EraseCell(MapCellViewModel cell)
        {
            if (cell == null || cell.Value == 0)
                return;

            // Update the underlying model
            _model.MapTiles[cell.Row][cell.Column] = 0;

            // Update the cell view model with the default "empty" color
            cell.Value = 0;
            cell.CellBrush = _model.FloorColor.ToBrush();

            IsDirty = true;
        }
        #endregion

        #region Resource References
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
                OnPropertyChanged(nameof(MapEditorStatusMessage));
            }
        }

        public ICommand RemoveResourceCommand => new RelayCommand<ResourceReference>(RemoveResourceReference);
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
            _model.MapTextures = ResourceReferences.ToList();

            // Remove all references to the resouce reference from the map, if any
            if (rrCount > 0)
                _model.RemoveResourceReferenceFromMap(resourceRef.Id);

            IsDirty = true;
        }

        public ICommand UnselectResourceReferenceCommand => new RelayCommand<ResourceReference>(
            UnselectResourceReference,
            rr => rr != null
        );
        private void UnselectResourceReference(ResourceReference currentlySelectedResourceRef)
        {
            SelectedResourceReference = null;
        }

        public ICommand FillEdgesWithSelectedResourceReferenceCommand => new RelayCommand<ResourceReference>(
            FillEdgesWithSelectedResourceReference,
            rr => rr != null
        );
        private void FillEdgesWithSelectedResourceReference(ResourceReference currentlySelectedResourceRef)
        {
            foreach (var cell in MapCells)
                if (cell.Row == 0 || cell.Row == MapHeight - 1 || cell.Column == 0 || cell.Column == MapWidth - 1)
                {
                    // Get the resource data for the new value
                    Dictionary<int, MapTexture> textureData = _model.GetMapTextures();
                    ColorArgb newColor = textureData.ContainsKey(currentlySelectedResourceRef.Id)
                        ? textureData[currentlySelectedResourceRef.Id].GetRepresentativeColor() ?? ColorArgb.Black()
                        : ColorArgb.LightGray();

                    cell.Value = currentlySelectedResourceRef.Id;
                    cell.CellBrush = newColor.ToBrush();

                    _model.MapTiles[cell.Row][cell.Column] = currentlySelectedResourceRef.Id;
                }

            IsDirty = true;
            
            OnPropertyChanged(nameof(MapCells));
        }

        public ICommand AddTextureResourceReferenceCommand => new RelayCommand<IEnumerable<string>>(AddTextureResourceReference);
        private void AddTextureResourceReference(IEnumerable<string> paths)
        {
            // First make sure all files are kosher
            foreach (string path in paths)
                if (!TextureData.ValidFile(path))
                    throw new Exception($"The texture file {path} either does not exist, cannot be opened, or is an invalid texture data file.");

            // Determine max ResourceRefId being used in this level
            int currentRrId = 1;
            if (_model.MapTextures.Any())
                currentRrId = _model.MapTextures.Max(rr => rr.Id) + 1;

            // Now load them up!
            foreach (string path in paths)
            {
                TextureData texture = TextureData.LoadFromFile(path);

                // Create new ResourceReference
                ResourceReference rr = new()
                {
                    Id = currentRrId,
                    FileName = System.IO.Path.GetFileName(path)
                };

                _model.MapTextures.Add(rr);

                _model.AddMapTexture(
                    rr.Id,
                    System.IO.Path.GetFileName(rr.FileName),
                    texture
                );

                currentRrId++;
            }

            ResourceReferences = new ObservableCollection<ResourceReference>(_model.MapTextures);
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

        private void ConstructMapCellsCollection()
        {
            // Construct the MapCellViewModel collection from the underlying model's MapTiles
            Dictionary<int, MapTexture> textureData = _model.GetMapTextures();

            _mapCells = new ObservableCollection<MapCellViewModel>();
            for (int row = 0; row < _model.MapTiles.Length; row++)
            {
                int[] rowData = _model.MapTiles[row];
                for (int col = 0; col < rowData.Length; col++)
                {
                    ColorArgb rr = textureData.ContainsKey(rowData[col]) ?
                        textureData[rowData[col]].GetRepresentativeColor() ?? ColorArgb.Black() : 
                        new ColorArgb(_model.FloorColor);

                    _mapCells.Add(new MapCellViewModel(row, col, rowData[col], rr));
                }
            }
        }

        /// <summary>
        /// Resizes the map to the specified dimensions. Any new cells created will be initialized to 0 (Floor).
        /// Any existing cells that fall outside the new dimensions will be discarded.
        /// </summary>
        public void ResizeMap(int newWidth, int newHeight)
        {
            int[][] newMapTiles = new int[newHeight][];

            for (int row = 0; row < newHeight; row++)
            {
                newMapTiles[row] = new int[newWidth];

                for (int col = 0; col < newWidth; col++)
                {
                    // If there is existing data for this cell, copy it over, otherwise set to 0 (Floor)
                    if (row < _model.MapTiles.Length && col < _model.MapTiles[row].Length)
                        newMapTiles[row][col] = _model.MapTiles[row][col];
                    else
                        newMapTiles[row][col] = 0;
                }
            }

            _model.MapTiles = newMapTiles;

            ConstructMapCellsCollection();
            OnPropertyChanged(nameof(MapWidth));
            OnPropertyChanged(nameof(MapHeight));
            OnPropertyChanged(nameof(MapSize));
            OnPropertyChanged(nameof(MapCells));

            IsDirty = true;
        }
        #endregion
    }
}