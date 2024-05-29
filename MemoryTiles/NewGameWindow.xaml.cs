using System.Windows;

namespace MemoryTiles
{
    public partial class NewGameWindow : Window
    {
        public NewGameWindow()
        {
            InitializeComponent();
        }

        private bool VerifyDimension(int rows, int columns)
        {
            if (rows < 2 || columns < 2)
            {
                MessageBox.Show("The dimensions given are too small. Please enter other values.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (rows > 10 || columns > 10)
            {
                MessageBox.Show("The dimensions given are too large. Please enter other values.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (rows * columns % 2 == 1)
            {
                MessageBox.Show("No pairs can be made with the dimensions entered. Please enter other values.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            int rows, columns;
            if (int.TryParse(rowsTextBox.Text, out rows) && int.TryParse(columnsTextBox.Text, out columns) && VerifyDimension(rows, columns))
            {
                this.Close();
                GameWindow gameWindow = new GameWindow(false, false, rows, columns);
                gameWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                gameWindow.ShowDialog();
            }
            else
            {
                return;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}