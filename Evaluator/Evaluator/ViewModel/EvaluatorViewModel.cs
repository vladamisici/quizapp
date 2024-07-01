using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using QuizApp.Models;
using QuizApp.Services;

namespace Evaluator.ViewModels
{
    public class EvaluatorViewModel : INotifyPropertyChanged
    {
        private readonly CommunicationService _communicationService;
        private ObservableCollection<User> _users;

        public event PropertyChangedEventHandler PropertyChanged;

        public EvaluatorViewModel()
        {
            Trace.WriteLine($"[DEBUG:EvaluatorViewModel] Initializing EvaluatorViewModel @{DateTime.Now}");
            _communicationService = new CommunicationService();
            _users = new ObservableCollection<User>();
            _ = ListenAsync();
        }

        public ObservableCollection<User> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged();
            }
        }

        private async Task ListenAsync()
        {
            try
            {
                Trace.WriteLine($"[DEBUG:ListenAsync] Starting to listen for messages @{DateTime.Now}");
                await _communicationService.ListenAsync(OnMessageReceived);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"[ERROR:ListenAsync] {ex.Message} @{DateTime.Now}");
            }
        }

        private void OnMessageReceived(string message)
        {
            Trace.WriteLine($"[DEBUG:OnMessageReceived] Received message: {message} @{DateTime.Now}");
            var parts = message.Split(':');
            Trace.WriteLine("partea0:", parts[0]);
            Trace.WriteLine("partea1:", parts[1]);
            Trace.WriteLine("partea2:", parts[2]);
            if (parts.Length == 2)
            {
                var username = parts[0];
                var role = parts[1];
                if (role == "testtaker")
                {
                    var newUser = new User { Username = username, Status = "Waiting for quiz" };
                    Trace.WriteLine($"[DEBUG:OnMessageReceived] Adding user {username} to user list @{DateTime.Now}");
                    Users.Add(newUser);
                    OnPropertyChanged(nameof(Users));
                }
            }
        }
            /*else if (message.StartsWith("status:"))
            {
                var parts = message.Split(':');
                if (parts.Length == 4)
                {
                    var username = parts[1];
                    var status = parts[2];
                    var score = parts[3];

                    var user = Users.FirstOrDefault(u => u.Username == username);
                    if (user != null)
                    {
                        user.Status = status;
                        user.Score = int.Parse(score);
                        Trace.WriteLine($"[DEBUG:OnMessageReceived] Updated status for user: {username} @{DateTime.Now}");
                        OnPropertyChanged(nameof(Users));
                    }
                }
            }
        }*/

        public async Task SendQuizAsync(string username, string quizData)
        {
            try
            {
                Trace.WriteLine($"[DEBUG:SendQuizAsync] Sending quiz to {username} @{DateTime.Now}");
                await _communicationService.SendMessageAsync($"quiz:{username}:{quizData}");
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"[ERROR:SendQuizAsync] {ex.Message} @{DateTime.Now}");
            }
        }

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
