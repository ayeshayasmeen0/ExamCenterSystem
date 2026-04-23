using System;
using Microsoft.Data.Sqlite;

namespace ExamCenterSystem.Data
{
    public static class DbInitializer
    {
        public static void Initialize()
        {
            using var con = DbConnection.GetConnection();
            con.Open();

            // ============================================================
            // 1. STUDENTS TABLE
            // ============================================================
            string createStudentsTable = @"
                CREATE TABLE IF NOT EXISTS Students (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    RollNumber TEXT NOT NULL UNIQUE,
                    Department TEXT NOT NULL,
                    Semester TEXT NOT NULL,
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                )";

            using var cmd1 = new SqliteCommand(createStudentsTable, con);
            cmd1.ExecuteNonQuery();

            // ============================================================
            // 2. EXAMS TABLE
            // ============================================================
            string createExamsTable = @"
                CREATE TABLE IF NOT EXISTS Exams (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    SubjectName TEXT NOT NULL,
                    Date TEXT NOT NULL,
                    Time TEXT NOT NULL,
                    Duration TEXT NOT NULL,
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                )";

            using var cmd2 = new SqliteCommand(createExamsTable, con);
            cmd2.ExecuteNonQuery();

            // ============================================================
            // 3. SEATS TABLE (UPDATED with SeatNumber, RowNum, ColumnNum)
            // ============================================================
            string createSeatsTable = @"
                CREATE TABLE IF NOT EXISTS Seats (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    StudentName TEXT NOT NULL,
                    RollNumber TEXT NOT NULL,
                    Department TEXT NOT NULL,
                    RoomName TEXT NOT NULL,
                    SeatNumber INTEGER NOT NULL,
                    RowNum INTEGER NOT NULL,
                    ColumnNum INTEGER NOT NULL,
                    AllocatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                )";

            using var cmd3 = new SqliteCommand(createSeatsTable, con);
            cmd3.ExecuteNonQuery();

            // ============================================================
            // 4. RESULTS TABLE
            // ============================================================
            string createResultsTable = @"
                CREATE TABLE IF NOT EXISTS Results (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    StudentName TEXT NOT NULL,
                    RollNumber TEXT NOT NULL,
                    SubjectName TEXT NOT NULL,
                    Marks INTEGER NOT NULL CHECK (Marks >= 0 AND Marks <= 100),
                    ExamDate DATETIME DEFAULT CURRENT_TIMESTAMP
                )";

            using var cmd4 = new SqliteCommand(createResultsTable, con);
            cmd4.ExecuteNonQuery();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n╔══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║           DATABASE INITIALIZED SUCCESSFULLY             ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════╣");
            Console.WriteLine("║  ✓ Students Table                                      ║");
            Console.WriteLine("║  ✓ Exams Table                                         ║");
            Console.WriteLine("║  ✓ Seats Table (with Row & Column)                     ║");
            Console.WriteLine("║  ✓ Results Table                                       ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine("\n📁 Database File: examcenter.db\n");
        }

        // 🔥 FIX METHOD: Check and recreate Seats table if columns missing
        public static void FixSeatsTable()
        {
            using var con = DbConnection.GetConnection();
            con.Open();

            try
            {
                // Check if SeatNumber column exists
                var checkCmd = new SqliteCommand("PRAGMA table_info(Seats)", con);
                using var reader = checkCmd.ExecuteReader();
                bool hasSeatNumber = false;
                bool hasRowNum = false;
                bool hasColumnNum = false;

                while (reader.Read())
                {
                    string colName = reader.GetString(1);
                    if (colName == "SeatNumber") hasSeatNumber = true;
                    if (colName == "RowNum") hasRowNum = true;
                    if (colName == "ColumnNum") hasColumnNum = true;
                }

                if (!hasSeatNumber || !hasRowNum || !hasColumnNum)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\n⚠ Old Seats table detected. Recreating with correct schema...");
                    Console.ResetColor();

                    // Drop and recreate
                    new SqliteCommand("DROP TABLE IF EXISTS Seats", con).ExecuteNonQuery();

                    string createSeatsTable = @"
                        CREATE TABLE Seats (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            StudentName TEXT NOT NULL,
                            RollNumber TEXT NOT NULL,
                            Department TEXT NOT NULL,
                            RoomName TEXT NOT NULL,
                            SeatNumber INTEGER NOT NULL,
                            RowNum INTEGER NOT NULL,
                            ColumnNum INTEGER NOT NULL,
                            AllocatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                        )";

                    new SqliteCommand(createSeatsTable, con).ExecuteNonQuery();

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("✓ Seats table fixed successfully!");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // ============================================================
        // RESET METHODS
        // ============================================================

        public static void ResetStudents()
        {
            using var con = DbConnection.GetConnection();
            con.Open();
            new SqliteCommand("DELETE FROM Students", con).ExecuteNonQuery();
            new SqliteCommand("DELETE FROM sqlite_sequence WHERE name='Students'", con).ExecuteNonQuery();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("⚠ All Students Reset! Next ID will start from 1");
            Console.ResetColor();
        }

        public static void ResetExams()
        {
            using var con = DbConnection.GetConnection();
            con.Open();
            new SqliteCommand("DELETE FROM Exams", con).ExecuteNonQuery();
            new SqliteCommand("DELETE FROM sqlite_sequence WHERE name='Exams'", con).ExecuteNonQuery();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("⚠ All Exams Reset! Next ID will start from 1");
            Console.ResetColor();
        }

        public static void ResetSeats()
        {
            using var con = DbConnection.GetConnection();
            con.Open();
            new SqliteCommand("DELETE FROM Seats", con).ExecuteNonQuery();
            new SqliteCommand("DELETE FROM sqlite_sequence WHERE name='Seats'", con).ExecuteNonQuery();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("⚠ All Seats Reset! Next ID will start from 1");
            Console.ResetColor();
        }

        public static void ResetResults()
        {
            using var con = DbConnection.GetConnection();
            con.Open();
            new SqliteCommand("DELETE FROM Results", con).ExecuteNonQuery();
            new SqliteCommand("DELETE FROM sqlite_sequence WHERE name='Results'", con).ExecuteNonQuery();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("⚠ All Results Reset! Next ID will start from 1");
            Console.ResetColor();
        }
    }
}