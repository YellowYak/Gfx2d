using System.Windows;

namespace Gfx2d.LevelEditor
{
    public partial class ResizeMapInputDialog : Window
    {
        public int MapWidth { get; private set; }
        public int MapHeight { get; private set; }

        public ResizeMapInputDialog(int mapWidth, int mapHeight)
        {
            InitializeComponent();

            this.MapWidth = mapWidth;
            this.MapHeight = mapHeight;

            txtMapWidth.Text = mapWidth.ToString();
            txtMapHeight.Text = mapHeight.ToString();

            txtMapWidth.Focus();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            bool widthValidInteger = int.TryParse(txtMapWidth.Text, out int width);
            bool heightValueInteger = int.TryParse(txtMapHeight.Text, out int height);

            if (widthValidInteger && heightValueInteger && width > 0 && height > 0)
            {
                MapWidth = width;
                MapHeight = height;

                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Please enter valid numbers greater than zero.", "Invalid Inputs");
            }
        }

        private void txtMapWidth_GotFocus(object sender, RoutedEventArgs e)
        {
            txtMapWidth.SelectAll();
        }

        private void txtMapHeight_GotFocus(object sender, RoutedEventArgs e)
        {
            txtMapHeight.SelectAll();
        }
    }
}
