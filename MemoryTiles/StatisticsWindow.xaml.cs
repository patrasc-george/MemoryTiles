using System.Windows;
using System.Windows.Controls;

namespace MemoryTiles
{
    public partial class StatisticsWindow : Window
    {
        public StatisticsWindow()
        {
            InitializeComponent();
            statisticsListView.ItemsSource = MainWindow.Profiles;

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void statisticsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}