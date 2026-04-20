using System;
using Microsoft.Data.SqlClient;
using ExamCenterSystem.Data;

namespace ExamCenterSystem.Services
{
    public class ExamService
    {
        public void AddExam()
        {
            try
            {
                Console.Write("Subject: ");
                string subject = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(subject))
                {
                    Console.WriteLine("❌ Subject cannot be empty!");
                    return;
                }

                Console.Write("Date (DD-MM-YYYY): ");
                string date = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(date))
                {
                    Console.WriteLine("❌ Date cannot be empty!");
                    return;
                }

                Console.Write("Time (HH:MM AM/PM): ");
                string time = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(time))
                {
                    Console.WriteLine("❌ Time cannot be empty!");
                    return;
                }

                Console.Write("Duration: ");
                string duration = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(duration))
                {
                    Console.WriteLine("❌ Duration cannot be empty!");
                    return;
                }

                using var con = DbConnection.GetConnection();
                con.Open();

                var cmd = new SqlCommand(@"
                    INSERT INTO Exams (SubjectName, Date, Time, Duration)
                    VALUES (@s,@d,@t,@du)", con);

                cmd.Parameters.AddWithValue("@s", subject);
                cmd.Parameters.AddWithValue("@d", date);
                cmd.Parameters.AddWithValue("@t", time);
                cmd.Parameters.AddWithValue("@du", duration);

                int rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                {
                    Console.WriteLine("✔ Exam Added Successfully!");
                }
                else
                {
                    Console.WriteLine("❌ Failed to add exam!");
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

        public void ViewExams()
        {
            try
            {
                using var con = DbConnection.GetConnection();
                con.Open();

                // First, get all exams
                var cmd = new SqlCommand("SELECT Id, SubjectName, Date, Time, Duration FROM Exams ORDER BY Id", con);
                using var r = cmd.ExecuteReader();

                if (!r.HasRows)
                {
                    Console.WriteLine("\n⚠ No exams found!");
                    return;
                }

                // ========== HEADINGS WITH PROPER FORMAT ==========
                Console.WriteLine("\n======================== EXAMS LIST ========================");
                Console.WriteLine("----------------------------------------------------------------");
                Console.WriteLine($"{"ID",-5} {"Subject",-20} {"Date",-15} {"Time",-15} {"Duration",-10}");
                Console.WriteLine("----------------------------------------------------------------");

                while (r.Read())
                {
                    int id = r.GetInt32(r.GetOrdinal("Id"));
                    string subject = r.GetString(r.GetOrdinal("SubjectName"));
                    string date = r.GetString(r.GetOrdinal("Date"));
                    string time = r.GetString(r.GetOrdinal("Time"));
                    string duration = r.GetString(r.GetOrdinal("Duration"));

                    Console.WriteLine($"{id,-5} {subject,-20} {date,-15} {time,-15} {duration,-10}");
                }

                Console.WriteLine("----------------------------------------------------------------");

                // IMPORTANT: Close the reader before running another query
                r.Close();

                // Now get the total count with a separate command
                using var countCmd = new SqlCommand("SELECT COUNT(*) FROM Exams", con);
                int total = (int)countCmd.ExecuteScalar();
                Console.WriteLine($"\n📊 Total Exams: {total}");
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

        public void UpdateExam()
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

                // Check if exam exists
                using var check = new SqlCommand("SELECT COUNT(*) FROM Exams WHERE Id=@id", con);
                check.Parameters.AddWithValue("@id", id);

                if ((int)check.ExecuteScalar() == 0)
                {
                    Console.WriteLine("❌ Exam not found with this ID!");
                    return;
                }

                Console.Write("Subject: ");
                string subject = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(subject))
                {
                    Console.WriteLine("❌ Subject cannot be empty!");
                    return;
                }

                Console.Write("Date (DD-MM-YYYY): ");
                string date = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(date))
                {
                    Console.WriteLine("❌ Date cannot be empty!");
                    return;
                }

                Console.Write("Time (HH:MM AM/PM): ");
                string time = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(time))
                {
                    Console.WriteLine("❌ Time cannot be empty!");
                    return;
                }

                Console.Write("Duration: ");
                string duration = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(duration))
                {
                    Console.WriteLine("❌ Duration cannot be empty!");
                    return;
                }

                using var cmd = new SqlCommand(@"
                    UPDATE Exams SET 
                    SubjectName=@s, 
                    Date=@d, 
                    Time=@t, 
                    Duration=@du
                    WHERE Id=@id", con);

                cmd.Parameters.AddWithValue("@s", subject);
                cmd.Parameters.AddWithValue("@d", date);
                cmd.Parameters.AddWithValue("@t", time);
                cmd.Parameters.AddWithValue("@du", duration);
                cmd.Parameters.AddWithValue("@id", id);

                int rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                {
                    Console.WriteLine("✔ Exam Updated Successfully!");
                }
                else
                {
                    Console.WriteLine("❌ Failed to update exam!");
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

        public void DeleteExam()
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

                // Get exam details before deletion
                using var getCmd = new SqlCommand("SELECT SubjectName, Date FROM Exams WHERE Id=@id", con);
                getCmd.Parameters.AddWithValue("@id", id);
                using var reader = getCmd.ExecuteReader();

                if (!reader.Read())
                {
                    Console.WriteLine("❌ Exam not found with this ID!");
                    return;
                }

                string subject = reader.GetString(0);
                string date = reader.GetString(1);
                reader.Close(); // Close reader before delete

                // Confirm deletion
                Console.Write($"Are you sure you want to delete exam '{subject}' ({date})? (y/n): ");
                string confirm = Console.ReadLine()?.ToLower();

                if (confirm != "y" && confirm != "yes")
                {
                    Console.WriteLine("❌ Deletion cancelled!");
                    return;
                }

                using var cmd = new SqlCommand("DELETE FROM Exams WHERE Id=@id", con);
                cmd.Parameters.AddWithValue("@id", id);

                int rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                {
                    Console.WriteLine($"✔ Exam Deleted: {subject}");
                }
                else
                {
                    Console.WriteLine("❌ Failed to delete exam!");
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

        public void ResetExams()
        {
            try
            {
                Console.Write("⚠ WARNING: This will delete ALL exams! Are you sure? (y/n): ");
                string confirm = Console.ReadLine()?.ToLower();

                if (confirm != "y" && confirm != "yes")
                {
                    Console.WriteLine("❌ Reset cancelled!");
                    return;
                }

                using var con = DbConnection.GetConnection();
                con.Open();

                // Get count before deletion
                using var countCmd = new SqlCommand("SELECT COUNT(*) FROM Exams", con);
                int count = (int)countCmd.ExecuteScalar();

                if (count == 0)
                {
                    Console.WriteLine("⚠ No exams to reset!");
                    return;
                }

                using var deleteCmd = new SqlCommand("DELETE FROM Exams", con);
                deleteCmd.ExecuteNonQuery();

                using var reseedCmd = new SqlCommand("DBCC CHECKIDENT ('Exams', RESEED, 0)", con);
                reseedCmd.ExecuteNonQuery();

                Console.WriteLine($"⚠ Exams Reset Done! {count} exam(s) deleted.");
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

        // Search exams by subject
        public void SearchExamBySubject()
        {
            try
            {
                Console.Write("Enter Subject Name: ");
                string subject = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(subject))
                {
                    Console.WriteLine("❌ Subject cannot be empty!");
                    return;
                }

                using var con = DbConnection.GetConnection();
                con.Open();

                using var cmd = new SqlCommand("SELECT Id, SubjectName, Date, Time, Duration FROM Exams WHERE SubjectName LIKE @s ORDER BY Id", con);
                cmd.Parameters.AddWithValue("@s", "%" + subject + "%");
                using var r = cmd.ExecuteReader();

                if (!r.HasRows)
                {
                    Console.WriteLine($"⚠ No exams found for subject: {subject}");
                    return;
                }

                Console.WriteLine($"\n=========== EXAMS FOR '{subject}' ===========");
                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine($"{"ID",-5} {"Subject",-20} {"Date",-15} {"Time",-15} {"Duration",-10}");
                Console.WriteLine("----------------------------------------------------------");

                while (r.Read())
                {
                    int id = r.GetInt32(0);
                    string subj = r.GetString(1);
                    string date = r.GetString(2);
                    string time = r.GetString(3);
                    string duration = r.GetString(4);

                    Console.WriteLine($"{id,-5} {subj,-20} {date,-15} {time,-15} {duration,-10}");
                }

                Console.WriteLine("----------------------------------------------------------");
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
    }
}