using System.Windows;

namespace MemoryTiles
{
    public partial class NewProfileWindow : Window
    {
        public string Name { get; set; }

        public NewProfileWindow()
        {
            InitializeComponent();
        }

        private void NewProfileButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(profileNameTextBox.Text))
            {
                Name = profileNameTextBox.Text;
                this.DialogResult = true;
                this.Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}