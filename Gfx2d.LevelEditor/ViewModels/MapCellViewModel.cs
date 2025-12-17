using Gfx2d.LevelEditor.Extensions;
using Gfx2d.Resources;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

public class MapCellViewModel : INotifyPropertyChanged
{
    public int Row { get; private set; }
    public int Column { get; private set; }

    private int _value;
    public int Value
    {
        get => _value;
        set {
            if (_value != value)
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }
    }

    private Brush _cellBrush = Brushes.LightGray;
    public Brush CellBrush
    {
        get => _cellBrush;
        set {
            if (_cellBrush != value)
            {
                _cellBrush = value;
                OnPropertyChanged(nameof(CellBrush));

                // Determine the TextColor to use based on the brightness of the CellBrush.
                // Namely, use white text for dark backgroun colors and black text for light ones.
                Color? color = (_cellBrush as SolidColorBrush)?.Color;
                if (color != null)
                {
                    // See https://stackoverflow.com/q/596216/160830 for more info on this zany formula
                    double backgroundBrightness = (
                         0.299 * color.Value.R +
                         0.587 * color.Value.G +
                         0.114 * color.Value.B
                    );

                    this.TextColor = backgroundBrightness < 128 ? Brushes.White : Brushes.Black;
                    OnPropertyChanged(nameof(TextColor));
                }
            }
        }
    }

    public Brush TextColor { get; private set; } = Brushes.White;

    public MapCellViewModel(int row, int col, int value, ColorArgb resourceColor)
    {
        this.Row = row;
        this.Column = col;
        this.Value = value;

        this.CellBrush = resourceColor.ToBrush();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}