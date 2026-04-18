using System;
using Microsoft.Data.SqlClient;
using ExamCenterSystem.Data;

namespace ExamCenterSystem.Services
{
    public class ExamService
    {
        public void AddExam()
        {
            Console.WriteLine("=== ADD EXAM ===");

            Console.Write("Enter Subject Name: ");
            string subject = Console.ReadLine();

            Console.Write("Enter Date (dd-MM-yyyy): ");
            string date = Console.ReadLine();

            Console.Write("Enter Time (hh:mm tt e.g. 02:30 PM): ");
            string time = Console.ReadLine();

            Console.Write("Enter Duration (e.g. 1 hour / 30 minutes / 45 seconds): ");
            string duration = Console.ReadLine();

            using (SqlConnection con = DbConnection.GetConnection())
            {
                con.Open();

                string query = @"INSERT INTO Exams 
                                (SubjectName, Date, Time, Duration)
                                VALUES (@s, @d, @t, @du)";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@s", subject);
                cmd.Parameters.AddWithValue("@d", date);
                cmd.Parameters.AddWithValue("@t", time);
                cmd.Parameters.AddWithValue("@du", duration);

                cmd.ExecuteNonQuery();
            }

            Console.WriteLine("\nExam Added ✔");
        }

        public void ViewExams()
        {
            using (SqlConnection con = DbConnection.GetConnection())
            {
                con.Open();

                string query = "SELECT * FROM Exams";
                SqlCommand cmd = new SqlCommand(query, con);

                SqlDataReader reader = cmd.ExecuteReader();

                Console.WriteLine("\n--- EXAMS LIST ---\n");

                while (reader.Read())
                {
                    Console.WriteLine(
                        $"ID: {reader["Id"]} | " +
                        $"Subject: {reader["SubjectName"]} | " +
                        $"Date: {reader["Date"]} | " +
                        $"Time: {reader["Time"]} | " +
                        $"Duration: {reader["Duration"]}"
                    );
                }

                reader.Close();
            }
        }
    }
}