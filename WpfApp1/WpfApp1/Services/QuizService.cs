using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using WpfApp1.Models;

namespace WpfApp1.Services
{
    public class QuizService
    {
        private const string QuestionsFilePath = "questions.json";

        public List<Question> LoadQuestions()
        {
            string json = File.ReadAllText(QuestionsFilePath);
            return JsonSerializer.Deserialize<List<Question>>(json);
        }

        public void SaveUserChoices(string username, Dictionary<int, int> userChoices)
        {
            string filePath = $"user_choices_{username}.json";
            string json = JsonSerializer.Serialize(userChoices);
            File.WriteAllText(filePath, json);
        }
    }
}
