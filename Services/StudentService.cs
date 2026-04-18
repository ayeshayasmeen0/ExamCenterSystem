using System;
using Microsoft.Data.SqlClient;
using ExamCenterSystem.Data;

namespace ExamCenterSystem.Services
{
    public class StudentService
    {
        public void AddStudent()
        {
            Console.Write("Enter Name: ");
            string name = Console.ReadLine();

            Console.Write("Enter Roll No: ");
            string roll = Console.ReadLine();

            Console.Write("Enter Department: ");
            string dept = Console.ReadLine();

            Console.Write("Enter Semester: ");
            int sem = int.Parse(Console.ReadLine());

            using (SqlConnection con = DbConnection.GetConnection())
            {
                con.Open();

                string query = "INSERT INTO Students (Name, RollNumber, Department, Semester) VALUES (@n,@r,@d,@s)";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@n", name);
                cmd.Parameters.AddWithValue("@r", roll);
                cmd.Parameters.AddWithValue("@d", dept);
                cmd.Parameters.AddWithValue("@s", sem);

                cmd.ExecuteNonQuery();

                Console.WriteLine("Student Added ✔");
            }
        }

        // 🔥 VIEW STUDENTS (NEW)
        public void ViewStudents()
        {
            using (SqlConnection con = DbConnection.GetConnection())
            {
                con.Open();

                string query = "SELECT * FROM Students";

                SqlCommand cmd = new SqlCommand(query, con);

                SqlDataReader reader = cmd.ExecuteReader();

                Console.WriteLine("\n--- STUDENTS LIST ---");

                while (reader.Read())
                {
                    Console.WriteLine(
                        $"ID: {reader["Id"]} | " +
                        $"Name: {reader["Name"]} | " +
                        $"Roll: {reader["RollNumber"]} | " +
                        $"Dept: {reader["Department"]} | " +
                        $"Sem: {reader["Semester"]}"
                    );
                }
            }
        }
    }
}