using System;
using System.Windows;
using QuizApp.Models;

namespace Evaluator.Views
{
    public partial class AddQuestionWindow : Window
    {
        public Question Question { get; private set; }

        public AddQuestionWindow()
        {
            InitializeComponent();
        }

        private void AddQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            string questionText = QuestionTextBox.Text;
            string[] answers = AnswersTextBox.Text.Split(',');
            int correctAnswerIndex;
            int maxScore;

            if (string.IsNullOrEmpty(questionText) || answers.Length == 0 || !int.TryParse(CorrectAnswerTextBox.Text, out correctAnswerIndex) || !int.TryParse(MaxScoreTextBox.Text, out maxScore))
            {
                MessageBox.Show("Please fill in all fields correctly.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Question = new Question
            {
                QuestionText = questionText,
                Answers = answers,
                CorrectAnswerIndex = correctAnswerIndex,
                MaxScore = maxScore
            };

            DialogResult = true;
            this.Close();
        }
    }
}
