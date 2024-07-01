using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Models;
using WpfApp1.ViewModels;

namespace WpfApp1.Views
{
    public partial class QuizWindow : Window
    {
        private QuizViewModel _viewModel;

        public QuizWindow(string username)
        {
            InitializeComponent();
            _viewModel = new QuizViewModel(new User { Username = username });
            DataContext = _viewModel;
            InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            Trace.WriteLine($"[DEBUG:InitializeAsync] Initializing QuizWindow for {_viewModel.User.Username} @{DateTime.Now}");
            await _viewModel.ConnectAsync();
            if (_viewModel.Questions != null && _viewModel.Questions.Count > 0)
            {
                _viewModel.OnPropertyChanged(nameof(_viewModel.CurrentQuestion));
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine($"[DEBUG:CloseButton_Click] Closing QuizWindow @{DateTime.Now}");
            _viewModel.CalculateScore();
            _viewModel.SaveUserChoices();
            MessageBox.Show($"Your score: {_viewModel.User.Score}");
            this.Close();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine($"[DEBUG:NextButton_Click] Moving to next question @{DateTime.Now}");
            if (_viewModel.CurrentQuestionIndex < _viewModel.Questions.Count - 1)
            {
                _viewModel.CurrentQuestionIndex++;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine($"[DEBUG:BackButton_Click] Moving to previous question @{DateTime.Now}");
            if (_viewModel.CurrentQuestionIndex > 0)
            {
                _viewModel.CurrentQuestionIndex--;
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine($"[DEBUG:RadioButton_Checked] RadioButton checked @{DateTime.Now}");
            if (sender is RadioButton radioButton && radioButton.IsChecked == true)
            {
                var answerIndex = _viewModel.CurrentQuestion.Answers.ToList().IndexOf(radioButton.Content.ToString());
                _viewModel.RecordUserChoice(answerIndex);
            }
        }
    }
}
