using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using QuizApp.Models;
using QuizApp.Services;

namespace QuizApp.ViewModels
{
    public class QuizViewModel : INotifyPropertyChanged
    {
        private readonly QuizService _quizService;
        private readonly CommunicationService _communicationService;
        private int _currentQuestionIndex;
        private Dictionary<int, int> _userChoices;

        public event PropertyChangedEventHandler PropertyChanged;

        public QuizViewModel(User user)
        {
            User = user;
            _quizService = new QuizService();
            _communicationService = new CommunicationService();
            _userChoices = new Dictionary<int, int>();
            _currentQuestionIndex = 0;
            _communicationService.ListenAsync(OnMessageReceived);
            OnPropertyChanged(nameof(CurrentQuestion));
        }

        public User User { get; set; }
        public List<Question> Questions { get; set; }

        public Question CurrentQuestion => Questions.Count > 0 ? Questions[_currentQuestionIndex] : null;

        public int CurrentQuestionIndex
        {
            get => _currentQuestionIndex;
            set
            {
                _currentQuestionIndex = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentQuestion));
            }
        }

        public async Task ConnectAsync()
        {
            await _communicationService.ConnectAsync(new Uri("ws://localhost:5000"));
            await SendMessageAsync($"register:{User.Username}");
        }

        private async Task SendMessageAsync(string message)
        {
            await _communicationService.SendMessageAsync(message);
        }

        private void OnMessageReceived(string message)
        {
            if (message.StartsWith("quiz:"))
            {
                var quizData = message.Substring(5);
                // Deserialize quiz data and update the UI accordingly
                Questions = JsonSerializer.Deserialize<List<Question>>(quizData);
                _currentQuestionIndex = 0;
                OnPropertyChanged(nameof(Questions));
                OnPropertyChanged(nameof(CurrentQuestion));
                MessageBox.Show("Quiz received!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (message.StartsWith("status:"))
            {
                var status = message.Substring(7);
                User.Status = status;
                OnPropertyChanged(nameof(User));
            }
        }

        public void SaveUserChoices()
        {
            _quizService.SaveUserChoices(User.Username, _userChoices);
        }

        public void RecordUserChoice(int answerIndex)
        {
            _userChoices[_currentQuestionIndex] = answerIndex;
        }

        public void CalculateScore()
        {
            int score = 0;
            for (int i = 0; i < Questions.Count; i++)
            {
                if (_userChoices.ContainsKey(i) && _userChoices[i] == Questions[i].CorrectAnswerIndex)
                {
                    score++;
                }
            }
            User.Score = score;
            User.Status = "Completed";
            OnPropertyChanged(nameof(User));
            SendMessageAsync($"status:{User.Username}:{User.Status}:{User.Score}");
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
