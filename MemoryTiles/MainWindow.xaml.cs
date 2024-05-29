using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;

namespace MemoryTiles
{
    public partial class MainWindow : Window
    {
        static public string[] ImagePaths { get; set; }
        static public List<Profile> Profiles { get; set; }
        static public List<List<List<int>>> SaveGames { get; set; }
        static public Profile SelectedProfile { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ImagePaths = Directory.GetFiles(@"C:\Users\George Patrasc\source\repos\MemoryTiles\MemoryTiles\Avatars", "*.jpg");
            InitialiseSaveGames();
            InitialiseProfiles();
            profilesListBox.ItemsSource = Profiles;
            profilesListBox.DisplayMemberPath = "Name";
            profilesListBox.SelectedIndex = 0;
        }

        private List<string> ReadFromFile(string filename)
        {
            List<string> date = new List<string>();

            try
            {
                using (StreamReader reader = new StreamReader(filename))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        date.Add(line);
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Error: " + e.Message);
            }

            return date;
        }

        private void InitialiseSaveGames()
        {
            SaveGames = new List<List<List<int>>>();
            List<List<int>> currentSaveGame = new List<List<int>>();

            foreach (string line in File.ReadAllLines("C:\\Users\\George Patrasc\\source\\repos\\MemoryTiles\\MemoryTiles\\SaveGames.txt"))
            {
                if (line.Trim() == "")
                {
                    SaveGames.Add(currentSaveGame);
                    currentSaveGame = new List<List<int>>();
                }
                else
                {
                    List<int> row = new List<int>();
                    string[] elements = line.Split(' ');
                    foreach (string element in elements)
                    {
                        if (element != "")
                            row.Add(int.Parse(element));
                    }
                    currentSaveGame.Add(row);
                }
            }
            if (currentSaveGame.Count > 0)
            {
                SaveGames.Add(currentSaveGame);
            }
        }

        private void InitialiseProfiles()
        {
            int j = 0;
            List<string> initialProfiles = ReadFromFile("C:\\Users\\George Patrasc\\source\\repos\\MemoryTiles\\MemoryTiles\\Profiles.txt");
            Profiles = new List<Profile>();
            for (int i = 0; i < initialProfiles.Count(); i = i + 6)
            {
                Profiles.Add(new Profile(j, initialProfiles[i], initialProfiles[i + 1], int.Parse(initialProfiles[i + 2]), int.Parse(initialProfiles[i + 3]), int.Parse(initialProfiles[i + 4]), int.Parse(initialProfiles[i + 5]), SaveGames[j]));
                j++;
            }
        }

        public static void UpdateProfilesFile()
        {
            string filePath = @"C:\Users\George Patrasc\source\repos\MemoryTiles\MemoryTiles\Profiles.txt";
            File.WriteAllText(filePath, "");
            foreach (Profile profile in Profiles)
            {
                string profileData = profile.Name + "\n" + profile.Avatar + "\n" + profile.GamesPlayed.Item1 + "\n" + profile.GamesPlayed.Item2 + "\n" + profile.SaveGameLevel + "\n" + profile.Timer + "\n";
                File.AppendAllText(filePath, profileData);
            }
        }

        public static void UpdateSaveGamesFile()
        {
            string filePath = @"C:\Users\George Patrasc\source\repos\MemoryTiles\MemoryTiles\SaveGames.txt";
            File.WriteAllText(filePath, "");
            foreach (var board in SaveGames)
            {
                foreach (var line in board)
                {
                    foreach (var tile in line)
                    {
                        string profileData = tile.ToString() + " ";
                        File.AppendAllText(filePath, profileData);
                    }
                    File.AppendAllText(filePath, "\n");
                }
                File.AppendAllText(filePath, "\n");
            }
        }

        private void SelectedProfileButton_Click(object sender, SelectionChangedEventArgs e)
        {
            SelectedProfile = profilesListBox.SelectedItem as Profile;
            if (SelectedProfile != null)
            {
                BitmapImage bitmap = new BitmapImage(new Uri(SelectedProfile.Avatar));
                imageControl.Source = bitmap;
            }
        }

        private void NewProfileButton_Click(object sender, RoutedEventArgs e)
        {
            NewProfileWindow newProfileWindow = new NewProfileWindow();
            newProfileWindow.Owner = this;
            newProfileWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            newProfileWindow.ShowDialog();
            if (newProfileWindow.DialogResult == true)
            {
                if (Profiles.Exists(profile => profile.Name == newProfileWindow.Name))
                {
                    MessageBox.Show("This name already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                List<List<int>> matrix = new List<List<int>>();
                matrix.Add(new List<int>() { 0 });
                SaveGames.Add(matrix);
                Profile newProfile = new Profile(Profiles.Count, newProfileWindow.Name, ImagePaths[0], 0, 0, 1, 0, matrix);
                Profiles.Add(newProfile);
                profilesListBox.ItemsSource = null;
                profilesListBox.ItemsSource = Profiles;
                UpdateProfilesFile();
                UpdateSaveGamesFile();
                profilesListBox.SelectedIndex = Profiles.Count - 1;
            }
        }

        private void DeleteProfileButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this profile?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                if (SelectedProfile != null && Profiles.Count > 1)
                {
                    int id = SelectedProfile.Id;
                    Profiles.RemoveAt(id);
                    SaveGames.RemoveAt(id);

                    profilesListBox.ItemsSource = null;
                    profilesListBox.ItemsSource = Profiles;
                    UpdateProfilesFile();
                    UpdateSaveGamesFile();
                    profilesListBox.SelectedIndex = 0;
                }
            }
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProfile != null)
            {
                int currentImageIndex = Array.IndexOf(ImagePaths, SelectedProfile.Avatar);
                if (currentImageIndex > 0)
                {
                    currentImageIndex--;
                    BitmapImage bitmap = new BitmapImage(new Uri(ImagePaths[currentImageIndex]));
                    SelectedProfile.Avatar = ImagePaths[currentImageIndex];
                    UpdateProfilesFile();
                    imageControl.Source = bitmap;
                }
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProfile != null)
            {
                int currentImageIndex = Array.IndexOf(ImagePaths, SelectedProfile.Avatar);
                if (currentImageIndex < ImagePaths.Length - 1)
                {
                    currentImageIndex++;
                    BitmapImage bitmap = new BitmapImage(new Uri(ImagePaths[currentImageIndex]));
                    SelectedProfile.Avatar = ImagePaths[currentImageIndex];
                    UpdateProfilesFile();
                    imageControl.Source = bitmap;
                }
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProfile != null)
            {
                PlayWindow playWindow = new PlayWindow();
                playWindow.Owner = this;
                playWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                this.Hide();
                playWindow.ShowDialog();
                this.ShowDialog();
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}