using System.Windows;

namespace MemoryTiles
{
    public partial class PlayWindow : Window
    {
        public PlayWindow()
        {
            InitializeComponent();
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            NewGameWindow newGameWindow = new NewGameWindow();
            newGameWindow.Owner = this;
            newGameWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            newGameWindow.ShowDialog();
            this.Show();
        }

        private void OpenGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.SelectedProfile.SaveGame.Count > 1 && MainWindow.SelectedProfile.SaveGame[0].Count > 1)
            {
                GameWindow gameWindow = new GameWindow(true);
                gameWindow.Owner = this;
                gameWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                this.Hide();
                gameWindow.ShowDialog();
                this.ShowDialog();
            }
        }

        private void StatisticsButton_Click(object sender, RoutedEventArgs e)
        {
            StatisticsWindow statisticsWindow = new StatisticsWindow();
            statisticsWindow.Owner = this;
            statisticsWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            statisticsWindow.ShowDialog();
            this.Show();
        }

        private void CreditsButton_Click(object sender, RoutedEventArgs e)
        {
            CreditsWindow creditsWindow = new CreditsWindow();
            creditsWindow.Owner = this;
            creditsWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            creditsWindow.ShowDialog();
            this.Show();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}