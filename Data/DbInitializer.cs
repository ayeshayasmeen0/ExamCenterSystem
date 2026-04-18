using System;
using Microsoft.Data.SqlClient;

namespace ExamCenterSystem.Data
{
    public class DbInitializer
    {
        public static void Initialize()
        {
            CreateDatabase();
            CreateTables();
        }

        private static void CreateDatabase()
        {
            string conn = @"Server=(localdb)\MSSQLLocalDB;Integrated Security=True;";

            using (SqlConnection con = new SqlConnection(conn))
            {
                con.Open();

                string query = "IF DB_ID('ExamCenterDB') IS NULL CREATE DATABASE ExamCenterDB";

                new SqlCommand(query, con).ExecuteNonQuery();
            }
        }

        private static void CreateTables()
        {
            string conn = @"Server=(localdb)\MSSQLLocalDB;Database=ExamCenterDB;Trusted_Connection=True;";

            using (SqlConnection con = new SqlConnection(conn))
            {
                con.Open();

                // 🧑 Students
                string students = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Students')
                CREATE TABLE Students (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    Name NVARCHAR(100),
                    RollNumber NVARCHAR(50),
                    Department NVARCHAR(50),
                    Semester INT
                )";

                // 📘 Exams (FIXED)
                string exams = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Exams')
                BEGIN
                    CREATE TABLE Exams (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        SubjectName NVARCHAR(100),
                        Date NVARCHAR(50),
                        Time NVARCHAR(50),
                        Duration NVARCHAR(50)
                    )
                END
                ELSE
                BEGIN
                    ALTER TABLE Exams ALTER COLUMN Duration NVARCHAR(50)
                END";

                // 🪑 Seats
                string seats = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Seats')
                CREATE TABLE Seats (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    StudentName NVARCHAR(100),
                    RollNumber NVARCHAR(50),
                    RoomName NVARCHAR(50)
                )";

                // 📊 Results
                string results = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Results')
                CREATE TABLE Results (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    StudentName NVARCHAR(100),
                    RollNumber NVARCHAR(50),
                    SubjectName NVARCHAR(100),
                    Marks INT
                )";

                new SqlCommand(students, con).ExecuteNonQuery();
                new SqlCommand(exams, con).ExecuteNonQuery();
                new SqlCommand(seats, con).ExecuteNonQuery();
                new SqlCommand(results, con).ExecuteNonQuery();
            }
        }
    }
}