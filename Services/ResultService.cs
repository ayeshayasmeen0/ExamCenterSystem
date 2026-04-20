using System;
using Microsoft.Data.SqlClient;
using ExamCenterSystem.Data;

namespace ExamCenterSystem.Services
{
    public class ResultService
    {
        public void AddResult()
        {
            try
            {
                Console.Write("Student Name: ");
                string name = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("❌ Student name cannot be empty!");
                    return;
                }

                Console.Write("Roll Number: ");
                string roll = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(roll))
                {
                    Console.WriteLine("❌ Roll number cannot be empty!");
                    return;
                }

                Console.Write("Subject: ");
                string subject = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(subject))
                {
                    Console.WriteLine("❌ Subject cannot be empty!");
                    return;
                }

                Console.Write("Marks (0-100): ");
                if (!int.TryParse(Console.ReadLine(), out int marks) || marks < 0 || marks > 100)
                {
                    Console.WriteLine("❌ Invalid marks! Please enter marks between 0-100.");
                    return;
                }

                using var con = DbConnection.GetConnection();
                con.Open();

                // Check for duplicate
                var check = new SqlCommand("SELECT COUNT(*) FROM Results WHERE RollNumber=@r AND SubjectName=@s", con);
                check.Parameters.AddWithValue("@r", roll);
                check.Parameters.AddWithValue("@s", subject);

                if ((int)check.ExecuteScalar() > 0)
                {
                    Console.WriteLine("❌ Result already exists for this student and subject!");
                    return;
                }

                var cmd = new SqlCommand(@"
                    INSERT INTO Results (StudentName, RollNumber, SubjectName, Marks)
                    VALUES (@n,@r,@s,@m)", con);

                cmd.Parameters.AddWithValue("@n", name);
                cmd.Parameters.AddWithValue("@r", roll);
                cmd.Parameters.AddWithValue("@s", subject);
                cmd.Parameters.AddWithValue("@m", marks);

                int rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                {
                    Console.WriteLine("✔ Result Added");
                    string grade = GetGrade(marks);
                    Console.WriteLine($"➡ Grade: {grade}");
                }
                else
                {
                    Console.WriteLine("❌ Failed to add result!");
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"❌ Database Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
        }

        public void ViewResults()
        {
            try
            {
                using var con = DbConnection.GetConnection();
                con.Open();

                var cmd = new SqlCommand("SELECT Id, StudentName, RollNumber, SubjectName, Marks FROM Results ORDER BY Id", con);
                var r = cmd.ExecuteReader();

                if (!r.HasRows)
                {
                    Console.WriteLine("\n⚠ No results found!");
                    return;
                }

                // ========== HEADINGS WITH PROPER FORMAT ==========
                Console.WriteLine("\n===================== RESULTS LIST =====================");
                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine($"{"ID",-5} {"Name",-20} {"Roll No",-10} {"Subject",-15} {"Marks",-5}");
                Console.WriteLine("----------------------------------------------------------");

                while (r.Read())
                {
                    int id = r.GetInt32(r.GetOrdinal("Id"));
                    string studentName = r.GetString(r.GetOrdinal("StudentName"));
                    string rollNumber = r.GetString(r.GetOrdinal("RollNumber"));
                    string subjectName = r.GetString(r.GetOrdinal("SubjectName"));
                    int marks = r.GetInt32(r.GetOrdinal("Marks"));

                    // Color coding based on marks
                    if (marks >= 80)
                        Console.ForegroundColor = ConsoleColor.Green;
                    else if (marks >= 60)
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    else if (marks >= 40)
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    else
                        Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine($"{id,-5} {studentName,-20} {rollNumber,-10} {subjectName,-15} {marks,-5}");
                    Console.ResetColor();
                }

                Console.WriteLine("----------------------------------------------------------");
                r.Close();

                // Show statistics
                var statsCmd = new SqlCommand("SELECT COUNT(*), AVG(CAST(Marks AS FLOAT)), MAX(Marks), MIN(Marks) FROM Results", con);
                var statsReader = statsCmd.ExecuteReader();

                if (statsReader.Read())
                {
                    int total = statsReader.GetInt32(0);
                    double avg = statsReader.IsDBNull(1) ? 0 : statsReader.GetDouble(1);
                    int max = statsReader.IsDBNull(2) ? 0 : statsReader.GetInt32(2);
                    int min = statsReader.IsDBNull(3) ? 0 : statsReader.GetInt32(3);

                    Console.WriteLine($"\n📊 Statistics:");
                    Console.WriteLine($"   Total Results: {total}");
                    Console.WriteLine($"   Average Marks: {avg:F2}");
                    Console.WriteLine($"   Highest Marks: {max}");
                    Console.WriteLine($"   Lowest Marks: {min}");
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"❌ Database Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Unexpected Error: {ex.Message}");
            }
        }

        public void UpdateResult()
        {
            try
            {
                Console.Write("Enter ID to update: ");
                if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
                {
                    Console.WriteLine("❌ Invalid ID!");
                    return;
                }

                using var con = DbConnection.GetConnection();
                con.Open();

                // Check if exists
                var check = new SqlCommand("SELECT COUNT(*) FROM Results WHERE Id=@id", con);
                check.Parameters.AddWithValue("@id", id);

                if ((int)check.ExecuteScalar() == 0)
                {
                    Console.WriteLine("❌ Result not found with this ID!");
                    return;
                }

                Console.Write("Student Name: ");
                string name = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("❌ Student name cannot be empty!");
                    return;
                }

                Console.Write("Roll Number: ");
                string roll = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(roll))
                {
                    Console.WriteLine("❌ Roll number cannot be empty!");
                    return;
                }

                Console.Write("Subject: ");
                string subject = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(subject))
                {
                    Console.WriteLine("❌ Subject cannot be empty!");
                    return;
                }

                Console.Write("Marks (0-100): ");
                if (!int.TryParse(Console.ReadLine(), out int marks) || marks < 0 || marks > 100)
                {
                    Console.WriteLine("❌ Invalid marks! Please enter marks between 0-100.");
                    return;
                }

                var cmd = new SqlCommand(@"
                    UPDATE Results SET 
                    StudentName=@n,
                    RollNumber=@r,
                    SubjectName=@s,
                    Marks=@m
                    WHERE Id=@id", con);

                cmd.Parameters.AddWithValue("@n", name);
                cmd.Parameters.AddWithValue("@r", roll);
                cmd.Parameters.AddWithValue("@s", subject);
                cmd.Parameters.AddWithValue("@m", marks);
                cmd.Parameters.AddWithValue("@id", id);

                int rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                {
                    Console.WriteLine("✔ Result Updated");
                    string grade = GetGrade(marks);
                    Console.WriteLine($"➡ New Grade: {grade}");
                }
                else
                {
                    Console.WriteLine("❌ Failed to update result!");
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"❌ Database Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
        }

        public void DeleteResult()
        {
            try
            {
                Console.Write("Enter ID to delete: ");
                if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
                {
                    Console.WriteLine("❌ Invalid ID!");
                    return;
                }

                using var con = DbConnection.GetConnection();
                con.Open();

                // Get details before deletion
                var getCmd = new SqlCommand("SELECT StudentName, SubjectName FROM Results WHERE Id=@id", con);
                getCmd.Parameters.AddWithValue("@id", id);
                var reader = getCmd.ExecuteReader();

                if (!reader.Read())
                {
                    Console.WriteLine("❌ Result not found with this ID!");
                    return;
                }

                string studentName = reader.GetString(0);
                string subjectName = reader.GetString(1);
                reader.Close();

                // Confirm deletion
                Console.Write($"Are you sure you want to delete result for {studentName} ({subjectName})? (y/n): ");
                string confirm = Console.ReadLine()?.ToLower();

                if (confirm != "y" && confirm != "yes")
                {
                    Console.WriteLine("❌ Deletion cancelled!");
                    return;
                }

                var cmd = new SqlCommand("DELETE FROM Results WHERE Id=@id", con);
                cmd.Parameters.AddWithValue("@id", id);

                int rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                {
                    Console.WriteLine($"✔ Result Deleted for {studentName} - {subjectName}");
                }
                else
                {
                    Console.WriteLine("❌ Failed to delete result!");
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"❌ Database Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
        }

        public void ResetResults()
        {
            try
            {
                Console.Write("⚠ WARNING: This will delete ALL results! Are you sure? (y/n): ");
                string confirm = Console.ReadLine()?.ToLower();

                if (confirm != "y" && confirm != "yes")
                {
                    Console.WriteLine("❌ Reset cancelled!");
                    return;
                }

                using var con = DbConnection.GetConnection();
                con.Open();

                // Get count before deletion
                var countCmd = new SqlCommand("SELECT COUNT(*) FROM Results", con);
                int count = (int)countCmd.ExecuteScalar();

                if (count == 0)
                {
                    Console.WriteLine("⚠ No results to reset!");
                    return;
                }

                new SqlCommand("DELETE FROM Results", con).ExecuteNonQuery();
                new SqlCommand("DBCC CHECKIDENT ('Results', RESEED, 0)", con).ExecuteNonQuery();

                Console.WriteLine($"⚠ Results Reset Done! {count} result(s) deleted.");
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"❌ Database Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
        }

        // Helper method to get grade
        private string GetGrade(int marks)
        {
            if (marks >= 90) return "A+";
            if (marks >= 80) return "A";
            if (marks >= 70) return "B+";
            if (marks >= 60) return "B";
            if (marks >= 50) return "C+";
            if (marks >= 40) return "C";
            if (marks >= 33) return "D";
            return "F";
        }
    }
}