using System;

namespace ExamCenterSystem.Models
{
    public class Exam
    {
        public int Id { get; set; }
        public string SubjectName { get; set; }

        public string Date { get; set; }   // dd-MM-yyyy
        public string Time { get; set; }   // hh:mm tt

        // ⛳ CHANGED: now flexible duration (no crash, any format allowed)
        public string Duration { get; set; }
        // Example: "1 hour", "30 minutes", "45 seconds"
    }
}