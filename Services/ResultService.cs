using System;
using Microsoft.Data.Sqlite;
using ExamCenterSystem.Data;

namespace ExamCenterSystem.Services
{
    public class ResultService
    {
        private string GetGrade(int marks)
        {
            if (marks >= 90) return "A+";
            if (marks >= 80) return "A";
            if (marks >= 75) return "B+";
            if (marks >= 70) return "B";
            if (marks >= 60) return "C+";
            if (marks >= 50) return "C";
            if (marks >= 40) return "D";
            return "F";
        }

        public void AddResult()
        {
            Console.Write("Enter Student Name: ");
            string name = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("❌ Student name cannot be empty!");
                return;
            }

            Console.Write("Enter Roll Number: ");
            string roll = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(roll))
            {
                Console.WriteLine("❌ Roll number cannot be empty!");
                return;
            }

            Console.Write("Enter Subject Name: ");
            string subject = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(subject))
            {
                Console.WriteLine("❌ Subject cannot be empty!");
                return;
            }

            Console.Write("Enter Marks (0-100): ");
            if (!int.TryParse(Console.ReadLine(), out int marks) || marks < 0 || marks > 100)
            {
                Console.WriteLine("❌ Invalid marks! Please enter marks between 0-100.");
                return;
            }

            using var con = DbConnection.GetConnection();
            con.Open();

            // Check for duplicate
            var checkCmd = new SqliteCommand("SELECT COUNT(*) FROM Results WHERE RollNumber = @roll AND SubjectName = @subject", con);
            checkCmd.Parameters.AddWithValue("@roll", roll);
            checkCmd.Parameters.AddWithValue("@subject", subject);

            if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
            {
                Console.WriteLine("❌ Result already exists for this student and subject!");
                return;
            }

            var cmd = new SqliteCommand(@"
                INSERT INTO Results (StudentName, RollNumber, SubjectName, Marks)
                VALUES (@name, @roll, @subject, @marks)", con);

            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@roll", roll);
            cmd.Parameters.AddWithValue("@subject", subject);
            cmd.Parameters.AddWithValue("@marks", marks);

            cmd.ExecuteNonQuery();
            string grade = GetGrade(marks);
            Console.WriteLine($"✔ Result Added Successfully!");
            Console.WriteLine($"   ➡ Grade: {grade}");
        }

        public void ViewResults()
        {
            using var con = DbConnection.GetConnection();
            con.Open();

            var cmd = new SqliteCommand("SELECT Id, StudentName, RollNumber, SubjectName, Marks FROM Results ORDER BY Id", con);
            using var reader = cmd.ExecuteReader();

            if (!reader.HasRows)
            {
                Console.WriteLine("\n⚠ No results found!");
                return;
            }

            Console.WriteLine("\n===================== RESULTS LIST =====================");
            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine($"{"ID",-5} {"Student Name",-20} {"Roll No",-10} {"Subject",-15} {"Marks",-5} {"Grade",-5}");
            Console.WriteLine("---------------------------------------------------------");

            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                string name = reader.GetString(1);
                string roll = reader.GetString(2);
                string subject = reader.GetString(3);
                int marks = reader.GetInt32(4);
                string grade = GetGrade(marks);

                // Color coding
                if (marks >= 80)
                    Console.ForegroundColor = ConsoleColor.Green;
                else if (marks >= 60)
                    Console.ForegroundColor = ConsoleColor.Cyan;
                else if (marks >= 40)
                    Console.ForegroundColor = ConsoleColor.Yellow;
                else
                    Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine($"{id,-5} {name,-20} {roll,-10} {subject,-15} {marks,-5} {grade,-5}");
                Console.ResetColor();
            }
            Console.WriteLine("---------------------------------------------------------");

            // Statistics
            reader.Close();
            var statsCmd = new SqliteCommand("SELECT COUNT(*), AVG(Marks), MAX(Marks), MIN(Marks) FROM Results", con);
            using var statsReader = statsCmd.ExecuteReader();
            if (statsReader.Read())
            {
                int total = statsReader.GetInt32(0);
                double avg = statsReader.GetDouble(1);
                int max = statsReader.GetInt32(2);
                int min = statsReader.GetInt32(3);

                Console.WriteLine($"\n📊 Statistics:");
                Console.WriteLine($"   Total Results: {total}");
                Console.WriteLine($"   Average Marks: {avg:F2}");
                Console.WriteLine($"   Highest Marks: {max}");
                Console.WriteLine($"   Lowest Marks: {min}");
            }
        }

        // ✏️ UPDATE RESULT - NEW METHOD
        public void UpdateResult()
        {
            Console.Write("Enter Result ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("❌ Invalid ID!");
                return;
            }

            using var con = DbConnection.GetConnection();
            con.Open();

            // Check if result exists and get current details
            var checkCmd = new SqliteCommand("SELECT StudentName, RollNumber, SubjectName, Marks FROM Results WHERE Id = @id", con);
            checkCmd.Parameters.AddWithValue("@id", id);
            using var reader = checkCmd.ExecuteReader();

            if (!reader.Read())
            {
                Console.WriteLine("❌ Result not found!");
                return;
            }

            string currentName = reader.GetString(0);
            string currentRoll = reader.GetString(1);
            string currentSubject = reader.GetString(2);
            int currentMarks = reader.GetInt32(3);
            reader.Close();

            Console.WriteLine($"\nCurrent Result Details:");
            Console.WriteLine($"   Student Name: {currentName}");
            Console.WriteLine($"   Roll Number: {currentRoll}");
            Console.WriteLine($"   Subject: {currentSubject}");
            Console.WriteLine($"   Marks: {currentMarks} (Grade: {GetGrade(currentMarks)})");

            Console.WriteLine("\nEnter New Details (press Enter to keep current value):");

            Console.Write($"New Student Name [{currentName}]: ");
            string name = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(name))
                name = currentName;

            Console.Write($"New Roll Number [{currentRoll}]: ");
            string roll = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(roll))
                roll = currentRoll;

            Console.Write($"New Subject [{currentSubject}]: ");
            string subject = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(subject))
                subject = currentSubject;

            Console.Write($"New Marks (0-100) [{currentMarks}]: ");
            string marksInput = Console.ReadLine()?.Trim();
            int marks = currentMarks;
            if (!string.IsNullOrWhiteSpace(marksInput))
            {
                if (!int.TryParse(marksInput, out marks) || marks < 0 || marks > 100)
                {
                    Console.WriteLine("❌ Invalid marks! Keeping current marks.");
                    marks = currentMarks;
                }
            }

            var cmd = new SqliteCommand(@"
                UPDATE Results SET 
                StudentName = @name, 
                RollNumber = @roll, 
                SubjectName = @subject, 
                Marks = @marks 
                WHERE Id = @id", con);

            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@roll", roll);
            cmd.Parameters.AddWithValue("@subject", subject);
            cmd.Parameters.AddWithValue("@marks", marks);
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();
            Console.WriteLine($"✔ Result Updated Successfully!");
            Console.WriteLine($"   ➡ New Grade: {GetGrade(marks)}");
        }

        // 🗑️ DELETE RESULT - NEW METHOD
        public void DeleteResult()
        {
            Console.Write("Enter Result ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("❌ Invalid ID!");
                return;
            }

            using var con = DbConnection.GetConnection();
            con.Open();

            // Get details before deletion
            var getCmd = new SqliteCommand("SELECT StudentName, SubjectName, Marks FROM Results WHERE Id = @id", con);
            getCmd.Parameters.AddWithValue("@id", id);
            using var reader = getCmd.ExecuteReader();

            if (!reader.Read())
            {
                Console.WriteLine("❌ Result not found!");
                return;
            }

            string studentName = reader.GetString(0);
            string subjectName = reader.GetString(1);
            int marks = reader.GetInt32(2);
            reader.Close();

            Console.WriteLine($"\nResult to delete:");
            Console.WriteLine($"   Student: {studentName}");
            Console.WriteLine($"   Subject: {subjectName}");
            Console.WriteLine($"   Marks: {marks} (Grade: {GetGrade(marks)})");

            Console.Write($"\nAre you sure you want to delete this result? (y/n): ");
            string confirm = Console.ReadLine()?.ToLower();

            if (confirm != "y" && confirm != "yes")
            {
                Console.WriteLine("❌ Deletion cancelled!");
                return;
            }

            var cmd = new SqliteCommand("DELETE FROM Results WHERE Id = @id", con);
            cmd.Parameters.AddWithValue("@id", id);

            int rows = cmd.ExecuteNonQuery();
            if (rows > 0)
                Console.WriteLine($"✔ Result Deleted Successfully! Removed {studentName} - {subjectName}");
            else
                Console.WriteLine("❌ Result not found!");
        }

        public void ResetResults()
        {
            DbInitializer.ResetResults();
        }
    }
}