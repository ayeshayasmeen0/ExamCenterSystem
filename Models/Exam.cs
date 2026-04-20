using System;

namespace ExamCenterSystem.Models
{
    public class Exam
    {
        public int Id { get; set; }
        public string SubjectName { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Duration { get; set; }

        // 🎯 POLYMORPHISM METHOD
        public virtual void DisplayExam()
        {
            System.Console.WriteLine(
                $"Subject: {SubjectName}, Date: {Date}, Time: {Time}, Duration: {Duration}"
            );
        }
    }
}