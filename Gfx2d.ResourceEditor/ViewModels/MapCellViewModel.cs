using Gfx2d.Resources;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

public class MapCellViewModel : INotifyPropertyChanged
{
    public int Row { get; private set; }
    public int Column { get; private set; }

    public int Value { get; private set; }

    public Brush CellBrush { get; private set; }
    public Brush TextColor { get; private set; }

    public MapCellViewModel(int row, int col, int value, ColorArgb resourceColor)
    {
        this.Row = row;
        this.Column = col;
        this.Value = value;

        this.CellBrush = new SolidColorBrush(Color.FromArgb(resourceColor.A, resourceColor.R, resourceColor.G, resourceColor.B));

        // Determine the TextColor based on the brightness of the CellBrush.
        // See https://stackoverflow.com/q/596216/160830 for more info
        double backgroundBrightness = (
             0.299 * resourceColor.R +
             0.587 * resourceColor.G +
             0.114 * resourceColor.B
        );

        this.TextColor = backgroundBrightness < 128 ? Brushes.White : Brushes.Black;
    }


    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
