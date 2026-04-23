using System;
using Microsoft.Data.Sqlite;
using ExamCenterSystem.Data;

namespace ExamCenterSystem.Services
{
    public class SeatService
    {
        // ➕ ALLOCATE SEAT (Department Wise + 20 Students Per Room + Row/Column Logic)
        public void AllocateSeat()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n   ╔════════════════════════════════════════╗");
            Console.WriteLine("   ║           ALLOCATE NEW SEAT             ║");
            Console.WriteLine("   ╚════════════════════════════════════════╝");
            Console.ResetColor();

            Console.Write("\n   👤 Enter Student Name: ");
            string name = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   ❌ Name cannot be empty!");
                Console.ResetColor();
                return;
            }

            Console.Write("   🔢 Enter Roll Number: ");
            if (!int.TryParse(Console.ReadLine(), out int roll) || roll <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   ❌ Invalid Roll Number! Please enter a positive number.");
                Console.ResetColor();
                return;
            }

            Console.Write("   🏛️ Enter Department (CS/SE/IT/DS): ");
            string dept = Console.ReadLine()?.Trim().ToUpper();

            if (string.IsNullOrWhiteSpace(dept))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   ❌ Department cannot be empty!");
                Console.ResetColor();
                return;
            }

            // Validate department
            string[] validDepts = { "CS", "SE", "IT", "DS" };
            if (!Array.Exists(validDepts, d => d == dept))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   ❌ Invalid Department! Please enter CS, SE, IT, or DS");
                Console.ResetColor();
                return;
            }

            using var con = DbConnection.GetConnection();
            con.Open();

            // Check if already allocated
            var checkCmd = new SqliteCommand("SELECT COUNT(*) FROM Seats WHERE RollNumber = @roll AND Department = @dept", con);
            checkCmd.Parameters.AddWithValue("@roll", roll.ToString());
            checkCmd.Parameters.AddWithValue("@dept", dept);
            if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"   ❌ Seat already allocated for Roll Number {roll} in {dept} department!");
                Console.ResetColor();
                return;
            }

            // ============================================================
            // LOGIC: Department Wise Rooms + 20 Students Per Room
            // ============================================================

            // Get count of students in same department
            var countCmd = new SqliteCommand("SELECT COUNT(*) FROM Seats WHERE Department = @dept", con);
            countCmd.Parameters.AddWithValue("@dept", dept);
            int studentsInDept = Convert.ToInt32(countCmd.ExecuteScalar());

            // Calculate room number (1 room = 20 students)
            int roomNumber = (studentsInDept / 20) + 1;

            // Calculate seat number (1-20)
            int seatNumber = (studentsInDept % 20) + 1;

            // Calculate row and column for visual display (5 columns per row)
            int row = ((seatNumber - 1) / 5) + 1;      // Rows 1-4
            int column = ((seatNumber - 1) % 5) + 1;    // Columns 1-5

            string roomName = $"{dept} - Room {roomNumber}";
            string seatPosition = $"Row {row}, Col {column}";

            var cmd = new SqliteCommand(@"
                INSERT INTO Seats (StudentName, RollNumber, Department, RoomName, SeatNumber, RowNum, ColumnNum)
                VALUES (@name, @roll, @dept, @room, @seatNum, @row, @col)", con);

            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@roll", roll.ToString());
            cmd.Parameters.AddWithValue("@dept", dept);
            cmd.Parameters.AddWithValue("@room", roomName);
            cmd.Parameters.AddWithValue("@seatNum", seatNumber);
            cmd.Parameters.AddWithValue("@row", row);
            cmd.Parameters.AddWithValue("@col", column);

            cmd.ExecuteNonQuery();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n   ╔════════════════════════════════════════╗");
            Console.WriteLine("   ║        SEAT ALLOCATED SUCCESSFULLY      ║");
            Console.WriteLine("   ╚════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine($"\n   📍 Department: {dept}");
            Console.WriteLine($"   🏠 Room: {roomName}");
            Console.WriteLine($"   💺 Seat Number: {seatNumber}");
            Console.WriteLine($"   📐 Position: {seatPosition}");
            Console.WriteLine($"   📊 Total Students in {dept}: {studentsInDept + 1}");
        }

        // 👁️ VIEW ALL SEATS
        public void ViewSeats()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n   ╔════════════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("   ║                              COMPLETE SEAT LIST                                ║");
            Console.WriteLine("   ╚════════════════════════════════════════════════════════════════════════════════╝");
            Console.ResetColor();

            using var con = DbConnection.GetConnection();
            con.Open();

            var cmd = new SqliteCommand(@"
                SELECT Id, StudentName, RollNumber, Department, RoomName, SeatNumber, RowNum, ColumnNum 
                FROM Seats 
                ORDER BY Department, RoomName, SeatNumber", con);
            using var reader = cmd.ExecuteReader();

            if (!reader.HasRows)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n   ⚠ No seats allocated yet!");
                Console.ResetColor();
                return;
            }

            Console.WriteLine("\n   ┌─────┬──────────────────────┬──────────┬────────────┬──────────────────┬───────┬────────┐");
            Console.WriteLine("   │ ID  │ Student Name         │ Roll No  │ Department │ Room             │ Seat  │ Position│");
            Console.WriteLine("   ├─────┼──────────────────────┼──────────┼────────────┼──────────────────┼───────┼────────┤");

            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                string name = reader.GetString(1);
                string roll = reader.GetString(2);
                string dept = reader.GetString(3);
                string room = reader.GetString(4);
                int seat = reader.GetInt32(5);
                int row = reader.GetInt32(6);
                int col = reader.GetInt32(7);

                Console.WriteLine($"   │ {id,-3} │ {name,-20} │ {roll,-8} │ {dept,-10} │ {room,-16} │ {seat,-5} │ R{row}C{col,-3} │");
            }

            Console.WriteLine("   └─────┴──────────────────────┴──────────┴────────────┴──────────────────┴───────┴────────┘");
        }

        // 🔍 VIEW SEATS BY DEPARTMENT
        public void ViewSeatsByDepartment()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n   ╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("   ║                    VIEW SEATS BY DEPARTMENT                     ║");
            Console.WriteLine("   ╚════════════════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Console.Write("\n   🏛️ Enter Department Name (CS/SE/IT/DS): ");
            string dept = Console.ReadLine()?.Trim().ToUpper();

            if (string.IsNullOrWhiteSpace(dept))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   ❌ Department cannot be empty!");
                Console.ResetColor();
                return;
            }

            using var con = DbConnection.GetConnection();
            con.Open();

            var cmd = new SqliteCommand(@"
                SELECT StudentName, RollNumber, RoomName, SeatNumber, RowNum, ColumnNum 
                FROM Seats 
                WHERE Department = @dept
                ORDER BY RoomName, SeatNumber", con);
            cmd.Parameters.AddWithValue("@dept", dept);
            using var reader = cmd.ExecuteReader();

            if (!reader.HasRows)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n   ⚠ No seats allocated for {dept} department!");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n   ╔══════════════════ {dept} DEPARTMENT SEAT ALLOCATION ══════════════════╗");
            Console.ResetColor();
            Console.WriteLine("   ┌─────────────────────────────────────────────────────────────────────┐");
            Console.WriteLine($"   │ {"Student Name",-20} │ {"Roll No",-8} │ {"Room",-18} │ {"Seat",-5} │ {"Pos",-6} │");
            Console.WriteLine("   ├─────────────────────────────────────────────────────────────────────┤");

            while (reader.Read())
            {
                string name = reader.GetString(0);
                string roll = reader.GetString(1);
                string room = reader.GetString(2);
                int seat = reader.GetInt32(3);
                int row = reader.GetInt32(4);
                int col = reader.GetInt32(5);

                Console.WriteLine($"   │ {name,-20} │ {roll,-8} │ {room,-18} │ {seat,-5} │ R{row}C{col,-4} │");
            }

            Console.WriteLine("   └─────────────────────────────────────────────────────────────────────┘");

            // Show summary
            var countCmd = new SqliteCommand("SELECT COUNT(*) FROM Seats WHERE Department = @dept", con);
            countCmd.Parameters.AddWithValue("@dept", dept);
            int total = Convert.ToInt32(countCmd.ExecuteScalar());

            int rooms = (total / 20) + (total % 20 > 0 ? 1 : 0);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n   📊 Summary for {dept} Department:");
            Console.WriteLine($"      Total Students: {total}");
            Console.WriteLine($"      Rooms Used: {rooms}");
            Console.WriteLine($"      Seats Available in Last Room: {(rooms * 20) - total}");
            Console.ResetColor();
        }

        // 🗺️ VIEW ROOM LAYOUT (Visual Grid)
        public void ViewRoomLayout()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n   ╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("   ║                        ROOM LAYOUT VIEW                         ║");
            Console.WriteLine("   ╚════════════════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Console.Write("\n   🏛️ Enter Department Name (CS/SE/IT/DS): ");
            string dept = Console.ReadLine()?.Trim().ToUpper();

            if (string.IsNullOrWhiteSpace(dept))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   ❌ Department cannot be empty!");
                Console.ResetColor();
                return;
            }

            Console.Write("   🔢 Enter Room Number (1, 2, 3...): ");
            if (!int.TryParse(Console.ReadLine(), out int roomNum) || roomNum <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   ❌ Invalid Room Number!");
                Console.ResetColor();
                return;
            }

            using var con = DbConnection.GetConnection();
            con.Open();

            string roomName = $"{dept} - Room {roomNum}";

            var cmd = new SqliteCommand(@"
                SELECT SeatNumber, StudentName, RollNumber, RowNum, ColumnNum 
                FROM Seats 
                WHERE RoomName = @room
                ORDER BY SeatNumber", con);
            cmd.Parameters.AddWithValue("@room", roomName);
            using var reader = cmd.ExecuteReader();

            // Create a 4x5 grid (4 rows, 5 columns) = 20 seats per room
            string[,] grid = new string[5, 6]; // Rows 1-4, Columns 1-5

            // Initialize grid with empty
            for (int r = 1; r <= 4; r++)
            {
                for (int c = 1; c <= 5; c++)
                {
                    grid[r, c] = "[     ]";
                }
            }

            // Fill grid with allocated seats
            while (reader.Read())
            {
                int seatNum = reader.GetInt32(0);
                string studentName = reader.GetString(1);
                int row = reader.GetInt32(3);
                int col = reader.GetInt32(4);

                // Shorten name for display (max 5 chars)
                string shortName = studentName.Length > 5 ? studentName.Substring(0, 5) : studentName;
                grid[row, col] = $"[{shortName,5}]";
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n   ╔══════════════════════ {roomName} LAYOUT ══════════════════════╗");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n                         🧑‍🏫 TEACHER'S DESK");
            Console.WriteLine("                         ═══════════════");
            Console.ResetColor();

            // Display Whiteboard/Projector Screen
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n   ┌─────────────────────────────────────────────────────────────┐");
            Console.WriteLine("   │                      📺 WHITEBOARD                           │");
            Console.WriteLine("   └─────────────────────────────────────────────────────────────┘");
            Console.ResetColor();

            // Display seating arrangement (4 rows, 5 columns)
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n                     S E A T I N G   A R R A N G E M E N T");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n            Col 1       Col 2       Col 3       Col 4       Col 5");
            Console.WriteLine("            ─────────────────────────────────────────────────────");
            Console.ResetColor();

            for (int r = 1; r <= 4; r++)
            {
                Console.Write($"   Row {r}     ");
                for (int c = 1; c <= 5; c++)
                {
                    if (grid[r, c] != "[     ]")
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    }
                    Console.Write($"{grid[r, c]}  ");
                    Console.ResetColor();
                }
                Console.WriteLine();
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n   📌 Legend:");
            Console.WriteLine("      🟢 [Name] = Occupied");
            Console.WriteLine("      ⚪ [     ] = Empty");
            Console.ResetColor();

            // Count occupied seats
            var countCmd = new SqliteCommand("SELECT COUNT(*) FROM Seats WHERE RoomName = @room", con);
            countCmd.Parameters.AddWithValue("@room", roomName);
            int occupied = Convert.ToInt32(countCmd.ExecuteScalar());

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n   📊 Room Statistics:");
            Console.WriteLine($"      💺 Occupied Seats: {occupied}");
            Console.WriteLine($"      🔲 Available Seats: {20 - occupied}");
            Console.WriteLine($"      📈 Utilization: {(occupied * 100 / 20)}%");
            Console.ResetColor();
        }

        // ✏️ UPDATE SEAT
        public void UpdateSeat()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n   ╔════════════════════════════════════════╗");
            Console.WriteLine("   ║             UPDATE SEAT INFO            ║");
            Console.WriteLine("   ╚════════════════════════════════════════╝");
            Console.ResetColor();

            Console.Write("\n   🔢 Enter Seat ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   ❌ Invalid ID!");
                Console.ResetColor();
                return;
            }

            using var con = DbConnection.GetConnection();
            con.Open();

            // Check if seat exists
            var checkCmd = new SqliteCommand("SELECT StudentName, RollNumber, Department FROM Seats WHERE Id = @id", con);
            checkCmd.Parameters.AddWithValue("@id", id);
            using var reader = checkCmd.ExecuteReader();

            if (!reader.Read())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   ❌ Seat not found!");
                Console.ResetColor();
                return;
            }

            string currentName = reader.GetString(0);
            string currentRoll = reader.GetString(1);
            string currentDept = reader.GetString(2);
            reader.Close();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n   📋 Current Seat Details:");
            Console.ResetColor();
            Console.WriteLine($"      👤 Student Name: {currentName}");
            Console.WriteLine($"      🔢 Roll Number: {currentRoll}");
            Console.WriteLine($"      🏛️ Department: {currentDept}");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n   ✏️ Enter New Details (press Enter to keep current value):");
            Console.ResetColor();

            Console.Write($"      New Student Name [{currentName}]: ");
            string name = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(name))
                name = currentName;

            Console.Write($"      New Roll Number [{currentRoll}]: ");
            string rollInput = Console.ReadLine()?.Trim();
            int roll;
            if (string.IsNullOrWhiteSpace(rollInput))
            {
                roll = int.Parse(currentRoll);
            }
            else
            {
                if (!int.TryParse(rollInput, out roll) || roll <= 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("   ❌ Invalid Roll Number! Keeping current.");
                    Console.ResetColor();
                    roll = int.Parse(currentRoll);
                }
            }

            Console.Write($"      New Department [{currentDept}]: ");
            string dept = Console.ReadLine()?.Trim().ToUpper();
            if (string.IsNullOrWhiteSpace(dept))
                dept = currentDept;

            // Get count of students in same department (excluding this seat)
            var countCmd = new SqliteCommand("SELECT COUNT(*) FROM Seats WHERE Department = @dept AND Id != @id", con);
            countCmd.Parameters.AddWithValue("@dept", dept);
            countCmd.Parameters.AddWithValue("@id", id);
            int studentsInDept = Convert.ToInt32(countCmd.ExecuteScalar());

            // Calculate new room and seat
            int roomNumber = (studentsInDept / 20) + 1;
            int seatNumber = (studentsInDept % 20) + 1;
            int row = ((seatNumber - 1) / 5) + 1;
            int column = ((seatNumber - 1) % 5) + 1;
            string roomName = $"{dept} - Room {roomNumber}";

            var cmd = new SqliteCommand(@"
                UPDATE Seats SET 
                StudentName = @name, 
                RollNumber = @roll, 
                Department = @dept, 
                RoomName = @room,
                SeatNumber = @seatNum,
                RowNum = @row,
                ColumnNum = @col
                WHERE Id = @id", con);

            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@roll", roll.ToString());
            cmd.Parameters.AddWithValue("@dept", dept);
            cmd.Parameters.AddWithValue("@room", roomName);
            cmd.Parameters.AddWithValue("@seatNum", seatNumber);
            cmd.Parameters.AddWithValue("@row", row);
            cmd.Parameters.AddWithValue("@col", column);
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n   ╔════════════════════════════════════════╗");
            Console.WriteLine("   ║           SEAT UPDATED SUCCESSFULLY     ║");
            Console.WriteLine("   ╚════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine($"\n   📍 New Department: {dept}");
            Console.WriteLine($"   🏠 New Room: {roomName}");
            Console.WriteLine($"   💺 New Seat Number: {seatNumber}");
            Console.WriteLine($"   📐 New Position: Row {row}, Column {column}");
        }

        // 🗑️ DELETE SEAT
        public void DeleteSeat()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n   ╔════════════════════════════════════════╗");
            Console.WriteLine("   ║              DELETE SEAT                ║");
            Console.WriteLine("   ╚════════════════════════════════════════╝");
            Console.ResetColor();

            Console.Write("\n   🔢 Enter Seat ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   ❌ Invalid ID!");
                Console.ResetColor();
                return;
            }

            using var con = DbConnection.GetConnection();
            con.Open();

            // Get details before deletion
            var getCmd = new SqliteCommand("SELECT StudentName, RollNumber, Department, RoomName, SeatNumber FROM Seats WHERE Id = @id", con);
            getCmd.Parameters.AddWithValue("@id", id);
            using var reader = getCmd.ExecuteReader();

            if (!reader.Read())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   ❌ Seat not found!");
                Console.ResetColor();
                return;
            }

            string studentName = reader.GetString(0);
            string rollNumber = reader.GetString(1);
            string department = reader.GetString(2);
            string roomName = reader.GetString(3);
            int seatNumber = reader.GetInt32(4);
            reader.Close();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n   📋 Seat to delete:");
            Console.ResetColor();
            Console.WriteLine($"      👤 Student: {studentName}");
            Console.WriteLine($"      🔢 Roll Number: {rollNumber}");
            Console.WriteLine($"      🏛️ Department: {department}");
            Console.WriteLine($"      🏠 Room: {roomName}");
            Console.WriteLine($"      💺 Seat Number: {seatNumber}");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"\n   ⚠️ Are you sure you want to delete this seat allocation? (y/n): ");
            string confirm = Console.ReadLine()?.ToLower();
            Console.ResetColor();

            if (confirm != "y" && confirm != "yes")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("   ❌ Deletion cancelled!");
                Console.ResetColor();
                return;
            }

            var cmd = new SqliteCommand("DELETE FROM Seats WHERE Id = @id", con);
            cmd.Parameters.AddWithValue("@id", id);

            int rows = cmd.ExecuteNonQuery();
            if (rows > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n   ✅ Seat Deleted Successfully! Removed {studentName} (Roll: {rollNumber})");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   ❌ Seat not found!");
                Console.ResetColor();
            }
        }

        // 🔄 RESET ALL SEATS
        public void ResetSeats()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n   ╔════════════════════════════════════════╗");
            Console.WriteLine("   ║              RESET SEATS                ║");
            Console.WriteLine("   ╚════════════════════════════════════════╝");
            Console.ResetColor();

            Console.Write("\n   ⚠️ WARNING: This will delete ALL seat allocations! Are you sure? (y/n): ");
            string confirm = Console.ReadLine()?.ToLower();

            if (confirm != "y" && confirm != "yes")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("   ❌ Reset cancelled!");
                Console.ResetColor();
                return;
            }

            DbInitializer.ResetSeats();
        }

        // 📊 VIEW SEAT SUMMARY
        public void ViewSeatSummary()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n   ╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("   ║                         SEAT SUMMARY                            ║");
            Console.WriteLine("   ╚════════════════════════════════════════════════════════════════╝");
            Console.ResetColor();

            using var con = DbConnection.GetConnection();
            con.Open();

            // Department wise summary
            var cmd = new SqliteCommand(@"
                SELECT Department, COUNT(*) as Total, 
                       COUNT(DISTINCT RoomName) as Rooms,
                       MAX(SeatNumber) as MaxSeat
                FROM Seats 
                GROUP BY Department
                ORDER BY Department", con);
            using var reader = cmd.ExecuteReader();

            if (!reader.HasRows)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n   ⚠ No seats allocated yet!");
                Console.ResetColor();
                return;
            }

            Console.WriteLine("\n   ┌────────────┬────────────┬────────────┬────────────┐");
            Console.WriteLine("   │ Department │ Total Seats │ Rooms Used │ Max Seat   │");
            Console.WriteLine("   ├────────────┼────────────┼────────────┼────────────┤");

            while (reader.Read())
            {
                string dept = reader.GetString(0);
                int total = reader.GetInt32(1);
                int rooms = reader.GetInt32(2);
                int maxSeat = reader.GetInt32(3);

                Console.WriteLine($"   │ {dept,-10} │ {total,-10} │ {rooms,-10} │ {maxSeat,-10} │");
            }

            Console.WriteLine("   └────────────┴────────────┴────────────┴────────────┘");

            // Total summary
            var totalCmd = new SqliteCommand("SELECT COUNT(*) FROM Seats", con);
            int grandTotal = Convert.ToInt32(totalCmd.ExecuteScalar());

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n   📊 Grand Total: {grandTotal} seats allocated");
            Console.ResetColor();
        }
    }
}