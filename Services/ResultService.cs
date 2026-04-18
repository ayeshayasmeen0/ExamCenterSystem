using System;
using Microsoft.Data.SqlClient;
using ExamCenterSystem.Data;

namespace ExamCenterSystem.Services
{
    public class ResultService
    {
        // ➕ ADD RESULT
        public void AddResult()
        {
            Console.Write("Enter Student Name: ");
            string name = Console.ReadLine();

            Console.Write("Enter Roll Number: ");
            string roll = Console.ReadLine();

            Console.Write("Enter Subject: ");
            string subject = Console.ReadLine();

            Console.Write("Enter Marks: ");
            int marks = int.Parse(Console.ReadLine());

            using (SqlConnection con = DbConnection.GetConnection())
            {
                con.Open();

                string query = "INSERT INTO Results VALUES (@n,@r,@s,@m)";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@n", name);
                cmd.Parameters.AddWithValue("@r", roll);
                cmd.Parameters.AddWithValue("@s", subject);
                cmd.Parameters.AddWithValue("@m", marks);

                cmd.ExecuteNonQuery();

                Console.WriteLine("Result Saved ✔");
            }
        }

        // 👁 VIEW RESULTS
        public void ViewResults()
        {
            using (SqlConnection con = DbConnection.GetConnection())
            {
                con.Open();

                string query = "SELECT * FROM Results";

                SqlCommand cmd = new SqlCommand(query, con);

                SqlDataReader reader = cmd.ExecuteReader();

                Console.WriteLine("\n--- STUDENT RESULTS ---");

                while (reader.Read())
                {
                    int marks = Convert.ToInt32(reader["Marks"]);

                    string grade;

                    if (marks >= 80) grade = "A+";
                    else if (marks >= 70) grade = "A";
                    else if (marks >= 60) grade = "B";
                    else if (marks >= 50) grade = "C";
                    else grade = "Fail";

                    Console.WriteLine(
                        $"Name: {reader["StudentName"]} | " +
                        $"Roll: {reader["RollNumber"]} | " +
                        $"Subject: {reader["SubjectName"]} | " +
                        $"Marks: {marks} | Grade: {grade}"
                    );
                }
            }
        }
    }
}