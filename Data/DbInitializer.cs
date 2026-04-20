using System;
using Microsoft.Data.SqlClient;

namespace ExamCenterSystem.Data
{
    public static class DbInitializer
    {
        public static void Initialize()
        {
            try
            {
                using var con = DbConnection.GetConnection();
                con.Open();

                // =========================
                // STUDENTS TABLE
                // =========================
                string createStudentsTable = @"
                IF OBJECT_ID('Students', 'U') IS NULL
                BEGIN
                    CREATE TABLE Students (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        Name NVARCHAR(100) NOT NULL,
                        RollNumber INT NOT NULL UNIQUE,
                        Department NVARCHAR(100) NOT NULL,
                        CreatedAt DATETIME DEFAULT GETDATE()
                    )
                END";

                using var cmd1 = new SqlCommand(createStudentsTable, con);
                cmd1.ExecuteNonQuery();

                // =========================
                // EXAMS TABLE
                // =========================
                string createExamsTable = @"
                IF OBJECT_ID('Exams', 'U') IS NULL
                BEGIN
                    CREATE TABLE Exams (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        SubjectName NVARCHAR(100) NOT NULL,
                        ExamDate DATE NOT NULL,
                        ExamTime TIME NOT NULL,
                        Duration INT NOT NULL,
                        RoomName NVARCHAR(50),
                        CreatedAt DATETIME DEFAULT GETDATE()
                    )
                END";

                using var cmd2 = new SqlCommand(createExamsTable, con);
                cmd2.ExecuteNonQuery();

                // =========================
                // SEATS TABLE - DROP AND RECREATE TO ENSURE CORRECT SCHEMA
                // =========================

                // First, check if we need to fix the table
                bool needsFix = false;
                string checkColumn = @"
                    SELECT COUNT(*) 
                    FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = 'Seats' AND COLUMN_NAME = 'Department'";

                using var checkCmd = new SqlCommand(checkColumn, con);
                int columnExists = (int)checkCmd.ExecuteScalar();

                if (columnExists == 0)
                {
                    needsFix = true;
                    Console.WriteLine("⚠ Fixing Seats table schema...");

                    // Drop foreign key constraint if exists
                    try
                    {
                        new SqlCommand(@"
                            IF OBJECT_ID('Results', 'U') IS NOT NULL 
                            BEGIN
                                ALTER TABLE Results DROP CONSTRAINT IF EXISTS FK_Results_Seats
                            END", con).ExecuteNonQuery();
                    }
                    catch { }

                    // Drop existing Seats table
                    new SqlCommand("DROP TABLE IF EXISTS Seats", con).ExecuteNonQuery();
                }

                // Create fresh Seats table
                string createSeatsTable = @"
                IF OBJECT_ID('Seats', 'U') IS NULL
                BEGIN
                    CREATE TABLE Seats (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        StudentName NVARCHAR(100) NOT NULL,
                        RollNumber INT NOT NULL UNIQUE,
                        Department NVARCHAR(100) NOT NULL,
                        RoomName NVARCHAR(50) NOT NULL,
                        AllocatedAt DATETIME DEFAULT GETDATE()
                    )
                END";

                using var cmd3 = new SqlCommand(createSeatsTable, con);
                cmd3.ExecuteNonQuery();

                if (needsFix)
                {
                    Console.WriteLine("✓ Seats table fixed with Department column!");
                }

                // =========================
                // RESULTS TABLE
                // =========================
                string createResultsTable = @"
                IF OBJECT_ID('Results', 'U') IS NULL
                BEGIN
                    CREATE TABLE Results (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        StudentName NVARCHAR(100) NOT NULL,
                        RollNumber INT NOT NULL,
                        SubjectName NVARCHAR(100) NOT NULL,
                        Marks INT NOT NULL CHECK (Marks >= 0 AND Marks <= 100),
                        ExamDate DATETIME DEFAULT GETDATE()
                    )
                END";

                using var cmd4 = new SqlCommand(createResultsTable, con);
                cmd4.ExecuteNonQuery();

                // =========================
                // CREATE INDEXES FOR BETTER PERFORMANCE
                // =========================
                string createIndexes = @"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Seats_RollNumber' AND object_id = OBJECT_ID('Seats'))
                BEGIN
                    CREATE INDEX IX_Seats_RollNumber ON Seats(RollNumber)
                END
                
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Seats_Department' AND object_id = OBJECT_ID('Seats'))
                BEGIN
                    CREATE INDEX IX_Seats_Department ON Seats(Department)
                END
                
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Results_RollNumber' AND object_id = OBJECT_ID('Results'))
                BEGIN
                    CREATE INDEX IX_Results_RollNumber ON Results(RollNumber)
                END";

                using var cmd5 = new SqlCommand(createIndexes, con);
                cmd5.ExecuteNonQuery();

                Console.WriteLine("✓ Database initialized successfully!");
                Console.WriteLine("✓ Tables: Students, Exams, Seats, Results");
                Console.WriteLine("✓ Indexes created for performance");
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"❌ Database Error: {ex.Message}");
                Console.WriteLine($"Error Number: {ex.Number}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Unexpected Error: {ex.Message}");
                throw;
            }
        }

        // Method to reset all tables
        public static void ResetDatabase()
        {
            try
            {
                using var con = DbConnection.GetConnection();
                con.Open();

                Console.Write("⚠ WARNING: This will delete ALL data! Continue? (y/n): ");
                string confirmation = Console.ReadLine()?.ToLower();

                if (confirmation != "y" && confirmation != "yes")
                {
                    Console.WriteLine("❌ Reset cancelled!");
                    return;
                }

                // Drop tables in correct order
                string dropTables = @"
                IF OBJECT_ID('Results', 'U') IS NOT NULL DROP TABLE Results
                IF OBJECT_ID('Seats', 'U') IS NOT NULL DROP TABLE Seats
                IF OBJECT_ID('Exams', 'U') IS NOT NULL DROP TABLE Exams
                IF OBJECT_ID('Students', 'U') IS NOT NULL DROP TABLE Students";

                using var cmd = new SqlCommand(dropTables, con);
                cmd.ExecuteNonQuery();

                Console.WriteLine("✓ All tables dropped successfully!");

                // Reinitialize
                Initialize();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error resetting database: {ex.Message}");
            }
        }

        // Quick fix method - just add the column if missing
        public static void EnsureDepartmentColumn()
        {
            try
            {
                using var con = DbConnection.GetConnection();
                con.Open();

                // Check if Department column exists
                string checkQuery = @"
                    SELECT COUNT(*) 
                    FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = 'Seats' AND COLUMN_NAME = 'Department'";

                using var checkCmd = new SqlCommand(checkQuery, con);
                int exists = (int)checkCmd.ExecuteScalar();

                if (exists == 0)
                {
                    Console.WriteLine("Adding Department column to Seats table...");

                    // Add the column
                    string alterQuery = "ALTER TABLE Seats ADD Department NVARCHAR(100) NOT NULL DEFAULT ''";
                    using var alterCmd = new SqlCommand(alterQuery, con);
                    alterCmd.ExecuteNonQuery();

                    Console.WriteLine("✓ Department column added successfully!");
                }
                else
                {
                    Console.WriteLine("✓ Department column already exists.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
        }
    }
}