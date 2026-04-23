namespace ExamCenterSystem.Models
{
    public class Exam
    {
        public int Id { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;

        public void DisplayExam()
        {
            Console.WriteLine($"Subject: {SubjectName}, Date: {Date}, Time: {Time}, Duration: {Duration}");
        }
    }
}