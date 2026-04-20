using System;
using Microsoft.Data.SqlClient;
using ExamCenterSystem.Data;

namespace ExamCenterSystem.Services
{
    public class StudentService
    {
        public void AddStudent()
        {
            Console.Write("Name: ");
            string name = Console.ReadLine() ?? "";

            Console.Write("Roll Number: ");
            string roll = Console.ReadLine() ?? "";

            Console.Write("Department: ");
            string dept = Console.ReadLine() ?? "";

            Console.Write("Semester: ");
            string sem = Console.ReadLine() ?? "";

            using var con = DbConnection.GetConnection();
            con.Open();

            var cmd = new SqlCommand(@"
                INSERT INTO Students (Name, RollNumber, Department, Semester)
                VALUES (@n, @r, @d, @s)", con);

            cmd.Parameters.AddWithValue("@n", name);
            cmd.Parameters.AddWithValue("@r", roll);
            cmd.Parameters.AddWithValue("@d", dept);
            cmd.Parameters.AddWithValue("@s", sem);

            cmd.ExecuteNonQuery();

            Console.WriteLine("✔ Student Added Successfully");
        }

        public void ViewStudents()
        {
            using var con = DbConnection.GetConnection();
            con.Open();

            var cmd = new SqlCommand("SELECT * FROM Students", con);
            var r = cmd.ExecuteReader();

            Console.WriteLine("\n--- STUDENTS LIST ---");

            while (r.Read())
            {
                Console.WriteLine(
                    $"ID: {r["Id"]} | Name: {r["Name"]} | Roll: {r["RollNumber"]} | Dept: {r["Department"]} | Sem: {r["Semester"]}"
                );
            }
        }

        public void UpdateStudent()
        {
            Console.Write("Enter ID: ");
            int id = int.Parse(Console.ReadLine() ?? "0");

            Console.Write("New Name: ");
            string name = Console.ReadLine() ?? "";

            Console.Write("New Roll Number: ");
            string roll = Console.ReadLine() ?? "";

            Console.Write("New Department: ");
            string dept = Console.ReadLine() ?? "";

            Console.Write("New Semester: ");
            string sem = Console.ReadLine() ?? "";

            using var con = DbConnection.GetConnection();
            con.Open();

            // ✔ Check if student exists
            var check = new SqlCommand("SELECT COUNT(*) FROM Students WHERE Id=@id", con);
            check.Parameters.AddWithValue("@id", id);

            if ((int)check.ExecuteScalar() == 0)
            {
                Console.WriteLine("❌ Student Not Found!");
                return;
            }

            var cmd = new SqlCommand(@"
                UPDATE Students SET 
                Name=@n, 
                RollNumber=@r, 
                Department=@d, 
                Semester=@s
                WHERE Id=@id", con);

            cmd.Parameters.AddWithValue("@n", name);
            cmd.Parameters.AddWithValue("@r", roll);
            cmd.Parameters.AddWithValue("@d", dept);
            cmd.Parameters.AddWithValue("@s", sem);
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();

            Console.WriteLine("✔ Student Updated");
        }

        public void DeleteStudent()
        {
            Console.Write("Enter ID: ");
            int id = int.Parse(Console.ReadLine() ?? "0");

            using var con = DbConnection.GetConnection();
            con.Open();

            var cmd = new SqlCommand("DELETE FROM Students WHERE Id=@id", con);
            cmd.Parameters.AddWithValue("@id", id);

            int rows = cmd.ExecuteNonQuery();

            if (rows > 0)
                Console.WriteLine("✔ Student Deleted");
            else
                Console.WriteLine("❌ Student Not Found");
        }

        // 🔥 RESET (FIXED PROPER LOGIC)
        public void ResetStudents()
        {
            using var con = DbConnection.GetConnection();
            con.Open();

            // 1. Delete all data
            new SqlCommand("DELETE FROM Students", con).ExecuteNonQuery();

            // 2. Reset identity so next ID = 1
            new SqlCommand("DBCC CHECKIDENT ('Students', RESEED, 0)", con).ExecuteNonQuery();

            Console.WriteLine("⚠ All Students Reset Successfully (ID will start from 1)");
        }
    }
}