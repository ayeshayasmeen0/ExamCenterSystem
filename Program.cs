using System;
using ExamCenterSystem.Services;
using ExamCenterSystem.Data;

class Program
{
    static bool Login()
    {
        Console.WriteLine("=================================");
        Console.WriteLine("        ADMIN LOGIN SYSTEM       ");
        Console.WriteLine("=================================");

        Console.Write("Username: ");
        string user = Console.ReadLine();

        Console.Write("Password: ");
        string pass = Console.ReadLine();

        if (user == "admin" && pass == "1234")
        {
            Console.WriteLine("\nLogin Successful ✔");
            Console.ReadKey();
            return true;
        }

        Console.WriteLine("\nInvalid Login ❌");
        Console.ReadKey();
        return false;
    }

    static void Main()
    {
        DbInitializer.Initialize();

        if (!Login())
        {
            Console.WriteLine("Access Denied!");
            return;
        }

        StudentService studentService = new StudentService();
        ExamService examService = new ExamService();
        SeatService seatService = new SeatService();
        ResultService resultService = new ResultService();

        while (true)
        {
            Console.Clear();

            Console.WriteLine("=================================");
            Console.WriteLine("   EXAM CENTER MANAGEMENT SYSTEM ");
            Console.WriteLine("=================================");
            Console.WriteLine("1. Add Student");
            Console.WriteLine("2. View Students");
            Console.WriteLine("3. Add Exam");
            Console.WriteLine("4. View Exams");
            Console.WriteLine("5. Allocate Seat");
            Console.WriteLine("6. View Seats");
            Console.WriteLine("7. Add Result");
            Console.WriteLine("8. View Results");
            Console.WriteLine("9. Exit");
            Console.WriteLine("=================================");
            Console.Write("Choose option: ");

            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Invalid input!");
                Console.ReadKey();
                continue;
            }

            Console.Clear();

            switch (choice)
            {
                case 1:
                    studentService.AddStudent();
                    break;

                case 2:
                    studentService.ViewStudents();
                    break;

                case 3:
                    examService.AddExam(); // ✔ ALL INPUT INSIDE SERVICE
                    break;

                case 4:
                    examService.ViewExams();
                    break;

                case 5:
                    seatService.AllocateSeat();
                    break;

                case 6:
                    seatService.ViewSeats();
                    break;

                case 7:
                    resultService.AddResult();
                    break;

                case 8:
                    resultService.ViewResults();
                    break;

                case 9:
                    Console.WriteLine("Exiting system...");
                    return;

                default:
                    Console.WriteLine("Invalid option!");
                    break;
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}