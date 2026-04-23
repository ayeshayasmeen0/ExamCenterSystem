using System;
using ExamCenterSystem.Services;
using ExamCenterSystem.Data;

class Program
{
    static void Main()
    {
        Console.Title = "Exam Center Management System";
        Console.WindowWidth = 100;
        Console.WindowHeight = 40;

        // Initialize Database
        DbInitializer.Initialize();

        // 🔥 FIX: Check and recreate Seats table if columns missing
        DbInitializer.FixSeatsTable();  // 👈 YEH LINE ADD KI HAI

        // Login Screen
        if (!ShowLogin())
        {
            return;
        }

        // Services
        StudentService studentService = new StudentService();
        ExamService examService = new ExamService();
        SeatService seatService = new SeatService();
        ResultService resultService = new ResultService();

        while (true)
        {
            ShowMainMenu();

            Console.Write("\n   👉 Choose option: ");
            string choice = Console.ReadLine();

            Console.Clear();

            switch (choice)
            {
                // Students CRUD
                case "1": studentService.AddStudent(); break;
                case "2": studentService.ViewStudents(); break;
                case "3": studentService.UpdateStudent(); break;
                case "4": studentService.DeleteStudent(); break;

                // Exams CRUD
                case "5": examService.AddExam(); break;
                case "6": examService.ViewExams(); break;
                case "7": examService.UpdateExam(); break;
                case "8": examService.DeleteExam(); break;

                // Seats Management
                case "9": seatService.AllocateSeat(); break;
                case "10": seatService.ViewSeats(); break;
                case "11": seatService.ViewSeatsByDepartment(); break;
                case "12": seatService.ViewRoomLayout(); break;
                case "13": seatService.UpdateSeat(); break;
                case "14": seatService.DeleteSeat(); break;

                // Results Management
                case "15": resultService.AddResult(); break;
                case "16": resultService.ViewResults(); break;
                case "17": resultService.UpdateResult(); break;
                case "18": resultService.DeleteResult(); break;

                // Reset Operations
                case "19": studentService.ResetStudents(); break;
                case "20": examService.ResetExams(); break;
                case "21": seatService.ResetSeats(); break;
                case "22": resultService.ResetResults(); break;

                case "0":
                    ShowGoodbye();
                    return;

                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n   ❌ Invalid option! Please try again.");
                    Console.ResetColor();
                    break;
            }

            Console.WriteLine("\n   Press any key to continue...");
            Console.ReadKey();
        }
    }

    static bool ShowLogin()
    {
        Console.Clear();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n");
        Console.WriteLine("   ╔══════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("   ║                                                                  ║");
        Console.WriteLine("   ║     ███████╗██╗  ██╗ █████╗ ███╗   ███╗                          ║");
        Console.WriteLine("   ║     ██╔════╝╚██╗██╔╝██╔══██╗████╗ ████║                          ║");
        Console.WriteLine("   ║     █████╗   ╚███╔╝ ╚█████╔╝██╔████╔██║                          ║");
        Console.WriteLine("   ║     ██╔══╝   ██╔██╗ ██╔══██╗██║╚██╔╝██║                          ║");
        Console.WriteLine("   ║     ███████╗██╔╝ ██╗╚█████╔╝██║ ╚═╝ ██║                          ║");
        Console.WriteLine("   ║     ╚══════╝╚═╝  ╚═╝ ╚════╝ ╚═╝     ╚═╝                          ║");
        Console.WriteLine("   ║                                                                  ║");
        Console.WriteLine("   ║                   EXAM CENTER MANAGEMENT SYSTEM                  ║");
        Console.WriteLine("   ║                          Version 2.0                             ║");
        Console.WriteLine("   ╚══════════════════════════════════════════════════════════════════╝");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n\n   ┌─────────────────────────────────────────────────────────────────┐");
        Console.WriteLine("   │                        ADMIN LOGIN                              │");
        Console.WriteLine("   └─────────────────────────────────────────────────────────────────┘");
        Console.ResetColor();

        Console.Write("\n   👤 Username: ");
        string username = Console.ReadLine();

        Console.Write("   🔒 Password: ");
        string password = ReadPassword();
        Console.WriteLine();

        if (username != "admin" || password != "1234")
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n   ❌ Invalid Credentials! Access Denied.");
            Console.ResetColor();
            Console.WriteLine("\n   Press any key to exit...");
            Console.ReadKey();
            return false;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n   ✅ Login Successful!");
        Console.ResetColor();
        Console.WriteLine("\n   Press any key to continue...");
        Console.ReadKey();
        return true;
    }

    static void ShowMainMenu()
    {
        Console.Clear();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n   ╔══════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("   ║                                                                  ║");
        Console.WriteLine("   ║                     EXAM CENTER SYSTEM                           ║");
        Console.WriteLine("   ║                        Main Menu                                 ║");
        Console.WriteLine("   ║                                                                  ║");
        Console.WriteLine("   ╚══════════════════════════════════════════════════════════════════╝");
        Console.ResetColor();

        // Students Section
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n   ┌───────────────────── STUDENTS ─────────────────────┐");
        Console.ResetColor();
        Console.WriteLine("   │  1.  ➕  Add Student                                │");
        Console.WriteLine("   │  2.  👁   View Students                             │");
        Console.WriteLine("   │  3.  ✏️  Update Student                             │");
        Console.WriteLine("   │  4.  🗑️  Delete Student                             │");

        // Exams Section
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("   ├───────────────────── EXAMS ────────────────────────┤");
        Console.ResetColor();
        Console.WriteLine("   │  5.  ➕  Add Exam                                   │");
        Console.WriteLine("   │  6.  👁   View Exams                                │");
        Console.WriteLine("   │  7.  ✏️  Update Exam                                │");
        Console.WriteLine("   │  8.  🗑️  Delete Exam                                │");

        // Seats Section
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("   ├───────────────────── SEATS ────────────────────────┤");
        Console.ResetColor();
        Console.WriteLine("   │  9.  💺  Allocate Seat                              │");
        Console.WriteLine("   │ 10.  👁   View All Seats                            │");
        Console.WriteLine("   │ 11.  🔍  View Seats by Department                   │");
        Console.WriteLine("   │ 12.  🗺️  View Room Layout                           │");
        Console.WriteLine("   │ 13.  ✏️  Update Seat                                │");
        Console.WriteLine("   │ 14.  🗑️  Delete Seat                                │");

        // Results Section
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("   ├───────────────────── RESULTS ──────────────────────┤");
        Console.ResetColor();
        Console.WriteLine("   │ 15.  📊  Add Result                                 │");
        Console.WriteLine("   │ 16.  📈  View Results                               │");
        Console.WriteLine("   │ 17.  ✏️  Update Result                              │");
        Console.WriteLine("   │ 18.  🗑️  Delete Result                              │");

        // Reset Section
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("   ├───────────────────── RESET ────────────────────────┤");
        Console.ResetColor();
        Console.WriteLine("   │ 19.  🔄  Reset Students                             │");
        Console.WriteLine("   │ 20.  🔄  Reset Exams                                │");
        Console.WriteLine("   │ 21.  🔄  Reset Seats                                │");
        Console.WriteLine("   │ 22.  🔄  Reset Results                              │");

        // Exit
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("   ├─────────────────────────────────────────────────────┤");
        Console.WriteLine("   │  0.  🚪  Exit                                        │");
        Console.WriteLine("   └─────────────────────────────────────────────────────┘");
        Console.ResetColor();
    }

    static void ShowGoodbye()
    {
        Console.Clear();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n\n");
        Console.WriteLine("   ╔══════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("   ║                                                                  ║");
        Console.WriteLine("   ║                     THANK YOU FOR USING                          ║");
        Console.WriteLine("   ║                   EXAM CENTER SYSTEM                             ║");
        Console.WriteLine("   ║                                                                  ║");
        Console.WriteLine("   ║                      Developed by:                               ║");
        Console.WriteLine("   ║                        Ayesha Yasmim                              ║");
        Console.WriteLine("   ║                      Roll No: 232201023                          ║");
        Console.WriteLine("   ║                         Semester: 6A                             ║");
        Console.WriteLine("   ║                      Department: BSCS                            ║");
        Console.WriteLine("   ║                                                                  ║");
        Console.WriteLine("   ║                         Goodbye! 👋                              ║");
        Console.WriteLine("   ╚══════════════════════════════════════════════════════════════════╝");
        Console.ResetColor();

        Console.WriteLine("\n\n   Press any key to exit...");
        Console.ReadKey();
    }

    // 🔐 Helper method for password masking (static method inside Program class)
    static string ReadPassword()
    {
        string password = "";
        ConsoleKeyInfo key;

        do
        {
            key = Console.ReadKey(true);

            if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
            {
                password += key.KeyChar;
                Console.Write("*");
            }
            else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password.Substring(0, password.Length - 1);
                Console.Write("\b \b");
            }
        }
        while (key.Key != ConsoleKey.Enter);

        return password;
    }
}