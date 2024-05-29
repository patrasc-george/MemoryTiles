using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MemoryTiles
{
    public partial class GameWindow : Window
    {
        private bool OpenGame;
        private bool NewRound;
        private List<List<int>> Board;
        private int Rows;
        private int Columns;
        private TimeSpan timeElapsed;
        private DispatcherTimer gameTimer;
        public GameWindow(bool OpenGame, bool NewRound = false, int rows = 0, int columns = 0)
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.None;
            this.OpenGame = OpenGame;
            this.NewRound = NewRound;
            if (OpenGame)
            {
                if (!NewRound)
                {
                    MainWindow.SelectedProfile.CurentLevel = MainWindow.SelectedProfile.SaveGameLevel;
                }
                this.Rows = MainWindow.SelectedProfile.SaveGame.Count;
                this.Columns = MainWindow.SelectedProfile.SaveGame[0].Count;
            }
            else
            {
                this.Rows = rows;
                this.Columns = columns;
            }
            if (MainWindow.SelectedProfile != null)
            {
                BitmapImage bitmap = new BitmapImage(new Uri(MainWindow.SelectedProfile.Avatar));
                imageControl.Source = bitmap;
                profileName.Text = MainWindow.SelectedProfile.Name;
                profileLevel.Text = "Level " + MainWindow.SelectedProfile.CurentLevel.ToString();
            }
            InitializeBoard();
            InitializeGrid();
        }

        private void InitializeBoard()
        {
            List<int> tempTiles = new List<int>();
            if (OpenGame && !NewRound)
            {
                for (int i = 0; i < Rows; i++)
                {
                    for (int j = 0; j < Columns; j++)
                    {
                        tempTiles.Add(MainWindow.SelectedProfile.SaveGame[i][j]);
                    }
                }
            }
            else
            {
                Random random = new Random();
                List<int> generatedIndices = new List<int>();
                for (int i = 0; i < Rows * Columns / 2; i++)
                {
                    int randomIndex = random.Next(0, MainWindow.ImagePaths.Length);
                    if (!generatedIndices.Contains(randomIndex))
                    {
                        tempTiles.Add(randomIndex);
                        tempTiles.Add(randomIndex);
                        generatedIndices.Add(randomIndex);
                    }
                    else
                    {
                        i--;
                    }
                    if (generatedIndices.Count == MainWindow.ImagePaths.Length)
                    {
                        generatedIndices.Clear();
                    }
                }
                tempTiles = tempTiles.OrderBy(x => Guid.NewGuid()).ToList();
            }
            int tilesAdded = 0;
            Board = new List<List<int>>();
            for (int i = 0; i < Rows; i++)
            {
                Board.Add(new List<int>());
                for (int j = 0; j < Columns; j++)
                {
                    Board[i].Add(tempTiles[tilesAdded]);
                    tilesAdded++;
                }
            }
        }

        private int AllottedTime()
        {
            if (OpenGame && !NewRound)
            {
                return MainWindow.SelectedProfile.Timer;
            }
            else
            {
                return Rows * Columns * 5;
            }
        }

        private void StartTimer()
        {
            timeElapsed = TimeSpan.FromSeconds(AllottedTime());
            gameTimer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                timerLabel.Content = timeElapsed.ToString("mm\\:ss");
                if (timeElapsed == TimeSpan.Zero)
                {
                    gameTimer.Stop();
                }
                timeElapsed = timeElapsed.Subtract(TimeSpan.FromSeconds(1));
            }, Application.Current.Dispatcher);
            gameTimer.Start();
        }

        private void ButtonPressed(ref Button button, ref Image image, ref bool buttonPressed, object sender, ref int i, ref int j)
        {
            button = (Button)sender;
            i = Grid.GetRow(button);
            j = Grid.GetColumn(button);

            // Sterge butonul din grid
            var parentGrid = (Grid)button.Parent;
            parentGrid.Children.Remove(button);

            buttonPressed = true;
            image = new Image();
            image.Source = new BitmapImage(new Uri(MainWindow.ImagePaths[Board[i][j]]));
            image.Margin = new Thickness(5);
            parentGrid.Children.Add(image);
            Grid.SetRow(image, i);
            Grid.SetColumn(image, j);
        }

        private void HideTiles(ref Button button, ref Image image)
        {
            var buttonParentGrid = (Grid)image.Parent;
            buttonParentGrid.Children.Remove(image);
            buttonParentGrid.Children.Add(button);
            Grid.SetRow(button, Grid.GetRow(button));
            Grid.SetColumn(button, Grid.GetColumn(button));
        }

        private void InitializeVariables(ref bool buttonPressed, ref Button button)
        {
            buttonPressed = false;
            button = null;
        }

        private void GameContinues()
        {
            MessageBoxResult result = MessageBox.Show("You won! You are ready for the next level?", "Win!", MessageBoxButton.OK);
            if (result == MessageBoxResult.OK)
            {
                MainWindow.SelectedProfile.CurentLevel++;
                this.Close();
                GameWindow gameWindow = new GameWindow(OpenGame, true, Rows, Columns);
                gameWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                gameWindow.ShowDialog();
            }
        }

        private void EndOfTheGame(bool win)
        {
            MessageBoxResult result;
            if (win)
            {
                result = MessageBox.Show("Congratulations! You have passed all levels.", "Win!", MessageBoxButton.OK);
            }
            else
            {
                result = MessageBox.Show("Time's up. You lose!", "Game Over", MessageBoxButton.OK);
            }
            if (result == MessageBoxResult.OK)
            {
                if (OpenGame)
                {
                    Board = new List<List<int>>();
                    Board.Add(new List<int> { 0 });
                    MainWindow.SelectedProfile.SaveGame = Board;
                    MainWindow.SaveGames[MainWindow.SelectedProfile.Id] = Board;
                    MainWindow.UpdateSaveGamesFile();
                    MainWindow.SelectedProfile.SaveGameLevel = 1;
                    MainWindow.SelectedProfile.Timer = 0;
                }
                MainWindow.SelectedProfile.CurentLevel = 1;
                MainWindow.SelectedProfile.GamesPlayed = new Tuple<int, int>(MainWindow.SelectedProfile.GamesPlayed.Item1 + 1, MainWindow.SelectedProfile.GamesPlayed.Item2 + Convert.ToInt32(win));
                MainWindow.UpdateProfilesFile();
                this.Close();
            }
        }
        private void VerifyFinal(int buttonsRemoved)
        {
            if (buttonsRemoved == Rows * Columns)
            {
                gameTimer.Stop();
                if (MainWindow.SelectedProfile.CurentLevel < 3)
                {
                    GameContinues();
                }
                else
                {
                    EndOfTheGame(true);
                }
            }
        }

        private void InitializeGrid()
        {
            StartTimer();
            List<List<Button>> buttonGrid = new List<List<Button>>();
            Button firstButton = null, secondButton = null;
            Image firstImage = null, secondImage = null;
            bool firstButtonPressed = false, secondButtonPressed = false;
            int buttonsRemoved = 0;
            for (int i = 0; i < Rows; i++)
            {
                buttonGrid.Add(new List<Button>());
                for (int j = 0; j < Columns; j++)
                {
                    Button button = new Button();
                    button.Margin = new Thickness(5);

                    button.Click += (sender, e) =>
                    {
                        if (timeElapsed.TotalSeconds < 0)
                        {
                            EndOfTheGame(false);
                            return;
                        }
                        if (!firstButtonPressed)
                        {
                            ButtonPressed(ref firstButton, ref firstImage, ref firstButtonPressed, sender, ref i, ref j);
                        }
                        else if (!secondButtonPressed)
                        {
                            ButtonPressed(ref secondButton, ref secondImage, ref secondButtonPressed, sender, ref i, ref j);

                            if (Board[i][j] == Board[Grid.GetRow(firstButton)][Grid.GetColumn(firstButton)])
                            {
                                Board[i][j] = Board[i][j] + MainWindow.ImagePaths.Length;
                                Board[Grid.GetRow(firstButton)][Grid.GetColumn(firstButton)] = Board[i][j];
                                InitializeVariables(ref firstButtonPressed, ref firstButton);
                                InitializeVariables(ref secondButtonPressed, ref secondButton);
                                buttonsRemoved = buttonsRemoved + 2;
                                VerifyFinal(buttonsRemoved);
                            }
                            else
                            {
                                DispatcherTimer timer = new DispatcherTimer();
                                timer.Interval = TimeSpan.FromMilliseconds(500);
                                timer.Tick += (s, args) =>
                                {
                                    timer.Stop();
                                    HideTiles(ref firstButton, ref firstImage);
                                    HideTiles(ref secondButton, ref secondImage);
                                    InitializeVariables(ref firstButtonPressed, ref firstButton);
                                    InitializeVariables(ref secondButtonPressed, ref secondButton);
                                };
                                timer.Start();
                            }
                        }
                    };
                    buttonGrid[i].Add(button);
                }

            }
            Grid grid = new Grid();
            for (int i = 0; i < Rows; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
            }
            for (int j = 0; j < Columns; j++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (Board[i][j] > MainWindow.ImagePaths.Length - 1)
                    {
                        buttonsRemoved++;
                        int index = Board[i][j] - MainWindow.ImagePaths.Length;
                        Image image = new Image();
                        image.Source = new BitmapImage(new Uri(MainWindow.ImagePaths[index]));
                        image.Margin = new Thickness(5);
                        grid.Children.Add(image);
                        Grid.SetRow(image, i);
                        Grid.SetColumn(image, j);
                    }
                    else
                    {
                        grid.Children.Add(buttonGrid[i][j]);
                        Grid.SetRow(buttonGrid[i][j], i);
                        Grid.SetColumn(buttonGrid[i][j], j);
                    }
                }
            }
            MyGrid.Children.Add(grid);
        }
        private void SaveGame()
        {
            MainWindow.SelectedProfile.SaveGame = Board;
            MainWindow.SelectedProfile.SaveGameLevel = MainWindow.SelectedProfile.CurentLevel;
            MainWindow.SelectedProfile.Timer = Convert.ToInt32(timeElapsed.TotalSeconds);
            MainWindow.SaveGames[MainWindow.SelectedProfile.Id] = Board;
            MainWindow.UpdateProfilesFile();
            MainWindow.UpdateSaveGamesFile();
            this.Close();
        }
        private void SaveGameButton_Click(object sender, RoutedEventArgs e)
        {
            gameTimer.Stop();
            MessageBoxResult result = MessageBox.Show("Are you sure you want to save the current game and exit?", "Corfirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                SaveGame();
            }
            else
            {
                gameTimer.Start();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            gameTimer.Stop();
            MessageBoxResult result = MessageBox.Show("Are you sure you want to exit the game?", "Corfirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                MainWindow.SelectedProfile.CurentLevel = 1;
                this.Close();
            }
            else
            {
                gameTimer.Start();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}