using System.Windows;
using WpfApp1.Views;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartQuizButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            if (!string.IsNullOrEmpty(username))
            {
                QuizWindow quizWindow = new QuizWindow(username);
                quizWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid username (null).", "Error");
            }
        }
    }
}
