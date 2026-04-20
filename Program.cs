<<<<<<< Updated upstream
﻿Console.WriteLine("Hello, World!");
=======
﻿using System;
using ExamCenterSystem.Services;
using ExamCenterSystem.Data;

class Program
{
    static void Main()
    {
        // Initialize database first (ensures all tables and columns exist)
        DbInitializer.EnsureDepartmentColumn();

        Console.Clear();

        Console.WriteLine("====================================");
        Console.WriteLine("   WELCOME TO EXAM CENTER SYSTEM    ");
        Console.WriteLine("====================================\n");

        Console.WriteLine("===== LOGIN =====");

        Console.Write("Username: ");
        string? username = Console.ReadLine();

        Console.Write("Password: ");
        string? password = Console.ReadLine();

        if (username != "admin" || password != "1234")
        {
            Console.WriteLine("\n❌ Invalid Login!");
            return;
        }

        Console.WriteLine("\n✔ Login Successful!");
        Console.WriteLine("Press any key...");
        Console.ReadKey();

        ExamService exam = new ExamService();
        StudentService student = new StudentService();
        SeatService seat = new SeatService();
        ResultService result = new ResultService();

        while (true)
        {
            Console.Clear();

            Console.WriteLine("====================================");
            Console.WriteLine("        EXAM CENTER SYSTEM          ");
            Console.WriteLine("====================================");

            Console.WriteLine("\n---- STUDENTS ----");
            Console.WriteLine("1. Add Student");
            Console.WriteLine("2. View Students");
            Console.WriteLine("3. Update Student");
            Console.WriteLine("4. Delete Student");

            Console.WriteLine("\n---- EXAMS ----");
            Console.WriteLine("5. Add Exam");
            Console.WriteLine("6. View Exams");
            Console.WriteLine("7. Update Exam");
            Console.WriteLine("8. Delete Exam");

            Console.WriteLine("\n---- SEATS ----");
            Console.WriteLine("9. Allocate Seat");
            Console.WriteLine("10. View Seats");
            Console.WriteLine("11. Update Seat");
            Console.WriteLine("12. Delete Seat");

            Console.WriteLine("\n---- RESULTS ----");
            Console.WriteLine("13. Add Result");
            Console.WriteLine("14. View Results");
            Console.WriteLine("15. Update Result");
            Console.WriteLine("16. Delete Result");

            Console.WriteLine("\n---- RESET ----");
            Console.WriteLine("17. Reset Students");
            Console.WriteLine("18. Reset Exams");
            Console.WriteLine("19. Reset Seats");
            Console.WriteLine("20. Reset Results");

            Console.WriteLine("\n0. Exit");

            Console.Write("\nChoose option: ");
            string? choice = Console.ReadLine();

            // ⭐ IMPORTANT: CLEAR BEFORE EXECUTION OUTPUT
            Console.Clear();

            switch (choice)
            {
                case "1": student.AddStudent(); break;
                case "2": student.ViewStudents(); break;
                case "3": student.UpdateStudent(); break;
                case "4": student.DeleteStudent(); break;

                case "5": exam.AddExam(); break;
                case "6": exam.ViewExams(); break;
                case "7": exam.UpdateExam(); break;
                case "8": exam.DeleteExam(); break;

                case "9": seat.AllocateSeat(); break;
                case "10": seat.ViewSeats(); break;
                case "11": seat.UpdateSeat(); break;
                case "12": seat.DeleteSeat(); break;

                case "13": result.AddResult(); break;
                case "14": result.ViewResults(); break;
                case "15": result.UpdateResult(); break;
                case "16": result.DeleteResult(); break;

                case "17": student.ResetStudents(); break;
                case "18": exam.ResetExams(); break;
                case "19": seat.ResetSeats(); break;
                case "20": result.ResetResults(); break;

                case "0":
                    Console.WriteLine("Goodbye 👋");
                    return;

                default:
                    Console.WriteLine("❌ Invalid option");
                    break;
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}
>>>>>>> Stashed changes
