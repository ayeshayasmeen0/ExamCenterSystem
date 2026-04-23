using System;
using Microsoft.Data.Sqlite;
using ExamCenterSystem.Data;

namespace ExamCenterSystem.Services
{
    public class ExamService
    {
        public void AddExam()
        {
            Console.Write("Enter Subject Name: ");
            string subject = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(subject))
            {
                Console.WriteLine("❌ Subject cannot be empty!");
                return;
            }

            Console.Write("Enter Date (DD-MM-YYYY): ");
            string date = Console.ReadLine()?.Trim();

            Console.Write("Enter Time (HH:MM AM/PM): ");
            string time = Console.ReadLine()?.Trim();

            Console.Write("Enter Duration: ");
            string duration = Console.ReadLine()?.Trim();

            using var con = DbConnection.GetConnection();
            con.Open();

            var cmd = new SqliteCommand(@"
                INSERT INTO Exams (SubjectName, Date, Time, Duration)
                VALUES (@subject, @date, @time, @duration)", con);

            cmd.Parameters.AddWithValue("@subject", subject);
            cmd.Parameters.AddWithValue("@date", date);
            cmd.Parameters.AddWithValue("@time", time);
            cmd.Parameters.AddWithValue("@duration", duration);

            cmd.ExecuteNonQuery();
            Console.WriteLine("✔ Exam Added Successfully!");
        }

        public void ViewExams()
        {
            using var con = DbConnection.GetConnection();
            con.Open();

            var cmd = new SqliteCommand("SELECT Id, SubjectName, Date, Time, Duration FROM Exams ORDER BY Id", con);
            using var reader = cmd.ExecuteReader();

            if (!reader.HasRows)
            {
                Console.WriteLine("\n⚠ No exams found!");
                return;
            }

            Console.WriteLine("\n===================== EXAMS LIST =====================");
            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine($"{"ID",-5} {"Subject",-20} {"Date",-15} {"Time",-15} {"Duration",-10}");
            Console.WriteLine("-------------------------------------------------------");

            while (reader.Read())
            {
                Console.WriteLine($"{reader.GetInt32(0),-5} {reader.GetString(1),-20} {reader.GetString(2),-15} {reader.GetString(3),-15} {reader.GetString(4),-10}");
            }
            Console.WriteLine("-------------------------------------------------------");
        }

        public void UpdateExam()
        {
            Console.Write("Enter Exam ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("❌ Invalid ID!");
                return;
            }

            using var con = DbConnection.GetConnection();
            con.Open();

            var checkCmd = new SqliteCommand("SELECT COUNT(*) FROM Exams WHERE Id = @id", con);
            checkCmd.Parameters.AddWithValue("@id", id);
            if (Convert.ToInt32(checkCmd.ExecuteScalar()) == 0)
            {
                Console.WriteLine("❌ Exam not found!");
                return;
            }

            Console.Write("Enter Subject Name: ");
            string subject = Console.ReadLine()?.Trim();

            Console.Write("Enter Date (DD-MM-YYYY): ");
            string date = Console.ReadLine()?.Trim();

            Console.Write("Enter Time (HH:MM AM/PM): ");
            string time = Console.ReadLine()?.Trim();

            Console.Write("Enter Duration: ");
            string duration = Console.ReadLine()?.Trim();

            var cmd = new SqliteCommand(@"
                UPDATE Exams SET 
                SubjectName = @subject, 
                Date = @date, 
                Time = @time, 
                Duration = @duration 
                WHERE Id = @id", con);

            cmd.Parameters.AddWithValue("@subject", subject);
            cmd.Parameters.AddWithValue("@date", date);
            cmd.Parameters.AddWithValue("@time", time);
            cmd.Parameters.AddWithValue("@duration", duration);
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();
            Console.WriteLine("✔ Exam Updated Successfully!");
        }

        public void DeleteExam()
        {
            Console.Write("Enter Exam ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("❌ Invalid ID!");
                return;
            }

            using var con = DbConnection.GetConnection();
            con.Open();

            var cmd = new SqliteCommand("DELETE FROM Exams WHERE Id = @id", con);
            cmd.Parameters.AddWithValue("@id", id);

            int rows = cmd.ExecuteNonQuery();
            if (rows > 0)
                Console.WriteLine("✔ Exam Deleted Successfully!");
            else
                Console.WriteLine("❌ Exam not found!");
        }

        public void ResetExams()
        {
            DbInitializer.ResetExams();
        }
    }
}