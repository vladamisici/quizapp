using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using QuizApp.Models;

namespace Evaluator.Views
{
    public partial class QuizCreationWindow : Window
    {
        private List<Question> _questions;

        public QuizCreationWindow()
        {
            InitializeComponent();
            _questions = new List<Question>();
            QuestionsListBox.ItemsSource = _questions;
        }

        private void AddQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            var addQuestionWindow = new AddQuestionWindow();
            if (addQuestionWindow.ShowDialog() == true)
            {
                _questions.Add(addQuestionWindow.Question);
                QuestionsListBox.Items.Refresh();
            }
        }

        private void SaveQuizButton_Click(object sender, RoutedEventArgs e)
        {
            string quizName = QuizNameTextBox.Text;

            if (string.IsNullOrEmpty(quizName) || _questions.Count == 0)
            {
                MessageBox.Show("Please provide a quiz name and at least one question.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string json = JsonSerializer.Serialize(_questions, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText($"{quizName}.json", json);

            MessageBox.Show("Quiz saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }
    }
}
