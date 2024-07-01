using System.Linq;
using System.Text.Json;
using System.Windows;
using QuizApp.Models;
using Evaluator.ViewModels;
using Evaluator.Views;
using System.IO;

namespace Evaluator
{
    public partial class MainWindow : Window
    {
        private EvaluatorViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new EvaluatorViewModel();
            DataContext = _viewModel;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void UsersListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (UsersListBox.SelectedItem != null)
            {
                QuizPanel.Visibility = Visibility.Visible;
            }
        }

        private void CreateQuizButton_Click(object sender, RoutedEventArgs e)
        {
            QuizCreationWindow quizCreationWindow = new QuizCreationWindow();
            quizCreationWindow.ShowDialog();
        }

        private async void SendQuizButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersListBox.SelectedItem != null)
            {
                var user = (User)UsersListBox.SelectedItem;
                var username = user.Username;
                var quizName = QuizNameTextBox.Text;

                if (string.IsNullOrEmpty(quizName))
                {
                    MessageBox.Show("Please enter a quiz name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string quizPath = $"{quizName}.json";
                if (!File.Exists(quizPath))
                {
                    MessageBox.Show("Quiz file not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string quizData = File.ReadAllText(quizPath);
                await _viewModel.SendQuizAsync(username, quizData);
                QuizPanel.Visibility = Visibility.Collapsed;
            }
        }
    }
}
