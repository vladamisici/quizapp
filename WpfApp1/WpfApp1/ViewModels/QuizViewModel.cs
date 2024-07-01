using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using WpfApp1.Models;
using WpfApp1.Services;
using QuizApp.Services;

namespace WpfApp1.ViewModels
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
            Trace.WriteLine($"[DEBUG:QuizViewModel] Initializing QuizViewModel for user {user.Username} @{DateTime.Now}");
            User = user;
            _quizService = new QuizService();
            _communicationService = new CommunicationService();
            _userChoices = new Dictionary<int, int>();
            _currentQuestionIndex = 0;
        }

        public User User { get; set; }
        public List<Question> Questions { get; set; }

        public Question CurrentQuestion => Questions != null && Questions.Count > 0 ? Questions[_currentQuestionIndex] : null;

        public int CurrentQuestionIndex
        {
            get => _currentQuestionIndex;
            set
            {
                Trace.WriteLine($"[DEBUG:CurrentQuestionIndex] Setting CurrentQuestionIndex to {value} @{DateTime.Now}");
                _currentQuestionIndex = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentQuestion));
            }
        }

        public async Task ConnectAsync()
        {
            try
            {
                Trace.WriteLine($"[DEBUG:ConnectAsync] Connecting to server @{DateTime.Now}");
                await _communicationService.ConnectAsync(new Uri("ws://localhost:5000"));
                await SendMessageAsync($"register:{User.Username}:testtaker");  // + role
                await _communicationService.ListenAsync(OnMessageReceived);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"[ERROR:ConnectAsync] {ex.Message} @{DateTime.Now}");
            }
        }

        private async Task SendMessageAsync(string message)
        {
            try
            {
                Trace.WriteLine($"[DEBUG:SendMessageAsync] Sending message: {message} @{DateTime.Now}");
                await _communicationService.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"[ERROR:SendMessageAsync] {ex.Message} @{DateTime.Now}");
            }
        }

        private void OnMessageReceived(string message)
        {
            Trace.WriteLine($"[DEBUG:OnMessageReceived] Received message: {message} @{DateTime.Now}");
            if (message.StartsWith("quiz:"))
            {
                try
                {
                    var quizData = message.Substring(5);
                    Questions = JsonSerializer.Deserialize<List<Question>>(quizData);
                    _currentQuestionIndex = 0;
                    OnPropertyChanged(nameof(Questions));
                    OnPropertyChanged(nameof(CurrentQuestion));
                    MessageBox.Show("Quiz received!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"[ERROR:OnMessageReceived] Error processing quiz data: {ex.Message} @{DateTime.Now}");
                }
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
            try
            {
                Trace.WriteLine($"[DEBUG:SaveUserChoices] Saving user choices @{DateTime.Now}");
                _quizService.SaveUserChoices(User.Username, _userChoices);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"[ERROR:SaveUserChoices] {ex.Message} @{DateTime.Now}");
            }
        }

        public void RecordUserChoice(int answerIndex)
        {
            Trace.WriteLine($"[DEBUG:RecordUserChoice] Recording user choice: {answerIndex} for question index {_currentQuestionIndex} @{DateTime.Now}");
            _userChoices[_currentQuestionIndex] = answerIndex;
        }

        public async void CalculateScore()
        {
            Trace.WriteLine($"[DEBUG:CalculateScore] Calculating score @{DateTime.Now}");
            int score = 0;
            for (int i = 0; i < Questions.Count; i++)
            {
                if (_userChoices.ContainsKey(i) && _userChoices[i] == Questions[i].CorrectAnswerIndex)
                {
                    score += Questions[i].Score;
                }
            }
            User.Score = score;
            User.Status = "Completed";
            OnPropertyChanged(nameof(User));
            await SendMessageAsync($"status:{User.Username}:{User.Status}:{User.Score}");
        }

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
