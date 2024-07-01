﻿namespace QuizApp.Models
{
    public class Question
    {
        public string QuestionText { get; set; }
        public string[] Answers { get; set; }
        public int CorrectAnswerIndex { get; set; }
        public int MaxScore { get; set; }
    }
}
