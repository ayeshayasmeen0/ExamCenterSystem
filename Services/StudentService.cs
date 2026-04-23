using System;
using Microsoft.Data.Sqlite;
using ExamCenterSystem.Data;
using ExamCenterSystem.Models;

namespace ExamCenterSystem.Services
{
    public class StudentService
    {
        public void AddStudent()
        {
            Console.Write("Enter Student Name: ");
            string name = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("❌ Name cannot be empty!");
                return;
            }

            Console.Write("Enter Roll Number: ");
            string roll = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(roll))
            {
                Console.WriteLine("❌ Roll Number cannot be empty!");
                return;
            }

            Console.Write("Enter Department: ");
            string dept = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(dept))
            {
                Console.WriteLine("❌ Department cannot be empty!");
                return;
            }

            Console.Write("Enter Semester: ");
            string sem = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(sem))
            {
                Console.WriteLine("❌ Semester cannot be empty!");
                return;
            }

            using var con = DbConnection.GetConnection();
            con.Open();

            // Check if roll number already exists
            var checkCmd = new SqliteCommand("SELECT COUNT(*) FROM Students WHERE RollNumber = @roll", con);
            checkCmd.Parameters.AddWithValue("@roll", roll);
            int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

            if (exists > 0)
            {
                Console.WriteLine("❌ Student with this Roll Number already exists!");
                return;
            }

            var cmd = new SqliteCommand(@"
                INSERT INTO Students (Name, RollNumber, Department, Semester)
                VALUES (@name, @roll, @dept, @sem)", con);

            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@roll", roll);
            cmd.Parameters.AddWithValue("@dept", dept);
            cmd.Parameters.AddWithValue("@sem", sem);

            cmd.ExecuteNonQuery();
            Console.WriteLine("✔ Student Added Successfully!");
        }

        public void ViewStudents()
        {
            using var con = DbConnection.GetConnection();
            con.Open();

            var cmd = new SqliteCommand("SELECT Id, Name, RollNumber, Department, Semester FROM Students ORDER BY Id", con);
            using var reader = cmd.ExecuteReader();

            if (!reader.HasRows)
            {
                Console.WriteLine("\n⚠ No students found!");
                return;
            }

            Console.WriteLine("\n===================== STUDENTS LIST =====================");
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine($"{"ID",-5} {"Name",-20} {"Roll No",-10} {"Department",-15} {"Semester",-8}");
            Console.WriteLine("----------------------------------------------------------");

            while (reader.Read())
            {
                Console.WriteLine($"{reader.GetInt32(0),-5} {reader.GetString(1),-20} {reader.GetString(2),-10} {reader.GetString(3),-15} {reader.GetString(4),-8}");
            }
            Console.WriteLine("----------------------------------------------------------");
        }

        public void UpdateStudent()
        {
            Console.Write("Enter Student ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("❌ Invalid ID!");
                return;
            }

            using var con = DbConnection.GetConnection();
            con.Open();

            // Check if student exists
            var checkCmd = new SqliteCommand("SELECT COUNT(*) FROM Students WHERE Id = @id", con);
            checkCmd.Parameters.AddWithValue("@id", id);
            if (Convert.ToInt32(checkCmd.ExecuteScalar()) == 0)
            {
                Console.WriteLine("❌ Student not found!");
                return;
            }

            Console.Write("Enter New Name: ");
            string name = Console.ReadLine()?.Trim();

            Console.Write("Enter New Roll Number: ");
            string roll = Console.ReadLine()?.Trim();

            Console.Write("Enter New Department: ");
            string dept = Console.ReadLine()?.Trim();

            Console.Write("Enter New Semester: ");
            string sem = Console.ReadLine()?.Trim();

            var cmd = new SqliteCommand(@"
                UPDATE Students SET 
                Name = @name, 
                RollNumber = @roll, 
                Department = @dept, 
                Semester = @sem 
                WHERE Id = @id", con);

            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@roll", roll);
            cmd.Parameters.AddWithValue("@dept", dept);
            cmd.Parameters.AddWithValue("@sem", sem);
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();
            Console.WriteLine("✔ Student Updated Successfully!");
        }

        public void DeleteStudent()
        {
            Console.Write("Enter Student ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("❌ Invalid ID!");
                return;
            }

            using var con = DbConnection.GetConnection();
            con.Open();

            var cmd = new SqliteCommand("DELETE FROM Students WHERE Id = @id", con);
            cmd.Parameters.AddWithValue("@id", id);

            int rows = cmd.ExecuteNonQuery();
            if (rows > 0)
                Console.WriteLine("✔ Student Deleted Successfully!");
            else
                Console.WriteLine("❌ Student not found!");
        }

        public void ResetStudents()
        {
            DbInitializer.ResetStudents();
        }
    }
}