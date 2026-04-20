using System;
using Microsoft.Data.SqlClient;
using ExamCenterSystem.Data;

namespace ExamCenterSystem.Services
{
    public class SeatService
    {
        // ➕ ALLOCATE SEAT (ROLL BASED LOGIC)
        public void AllocateSeat()
        {
            try
            {
                Console.Write("Student Name: ");
                string name = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("❌ Student name cannot be empty!");
                    return;
                }

                Console.Write("Roll Number: ");
                if (!int.TryParse(Console.ReadLine(), out int roll) || roll <= 0)
                {
                    Console.WriteLine("❌ Invalid roll number! Please enter a positive number.");
                    return;
                }

                Console.Write("Department: ");
                string dept = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(dept))
                {
                    Console.WriteLine("❌ Department cannot be empty!");
                    return;
                }

                using var con = DbConnection.GetConnection();
                con.Open();

                // ✅ DUPLICATE CHECK
                using var check = new SqlCommand("SELECT COUNT(*) FROM Seats WHERE RollNumber = @r", con);
                check.Parameters.AddWithValue("@r", roll.ToString());

                int existingCount = (int)check.ExecuteScalar();
                if (existingCount > 0)
                {
                    Console.WriteLine("❌ Roll number already allocated!");
                    return;
                }

                // ✅ ROOM LOGIC (10 students per room)
                int roomNumber = ((roll - 1) / 10) + 1;
                string roomName = "Room " + roomNumber;

                using var cmd = new SqlCommand(@"
                    INSERT INTO Seats (StudentName, RollNumber, Department, RoomName)
                    VALUES (@n, @r, @d, @room)", con);

                cmd.Parameters.AddWithValue("@n", name);
                cmd.Parameters.AddWithValue("@r", roll.ToString());
                cmd.Parameters.AddWithValue("@d", dept);
                cmd.Parameters.AddWithValue("@room", roomName);

                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine($"✔ Seat Allocated Successfully!");
                    Console.WriteLine($"➡ Room: {roomName}");
                    Console.WriteLine($"➡ Roll Number: {roll}");
                }
                else
                {
                    Console.WriteLine("❌ Failed to allocate seat!");
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"❌ Database Error: {ex.Message}");
                if (ex.Number == 207)
                {
                    Console.WriteLine("⚠ Make sure the 'Department' column exists in the Seats table!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Unexpected Error: {ex.Message}");
            }
        }

        // 👁 VIEW SEATS
        public void ViewSeats()
        {
            try
            {
                using var con = DbConnection.GetConnection();
                con.Open();

                using var cmd = new SqlCommand("SELECT Id, StudentName, RollNumber, Department, RoomName FROM Seats ORDER BY CAST(RollNumber AS INT)", con);
                using var reader = cmd.ExecuteReader();

                if (!reader.HasRows)
                {
                    Console.WriteLine("\n⚠ No seats allocated yet!");
                    return;
                }

                Console.WriteLine("\n=========== SEAT LIST ===========");
                Console.WriteLine("------------------------------------------------------");
                Console.WriteLine($"{"ID",-5} {"Name",-20} {"Roll",-10} {"Dept",-12} {"Room",-10}");
                Console.WriteLine("------------------------------------------------------");

                while (reader.Read())
                {
                    int id = reader.GetInt32(reader.GetOrdinal("Id"));
                    string studentName = reader.GetString(reader.GetOrdinal("StudentName"));
                    string rollNumber = reader.GetString(reader.GetOrdinal("RollNumber"));
                    string department = reader.GetString(reader.GetOrdinal("Department"));
                    string roomName = reader.GetString(reader.GetOrdinal("RoomName"));

                    Console.WriteLine($"{id,-5} {studentName,-20} {rollNumber,-10} {department,-12} {roomName,-10}");
                }

                Console.WriteLine("------------------------------------------------------");
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"❌ Database Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Unexpected Error: {ex.Message}");
            }
        }

        // ✏ UPDATE SEAT
        public void UpdateSeat()
        {
            try
            {
                Console.Write("Enter ID to update: ");
                if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
                {
                    Console.WriteLine("❌ Invalid ID!");
                    return;
                }

                using var con = DbConnection.GetConnection();
                con.Open();

                // ✅ CHECK IF SEAT EXISTS
                using var check = new SqlCommand("SELECT COUNT(*) FROM Seats WHERE Id = @id", con);
                check.Parameters.AddWithValue("@id", id);

                if ((int)check.ExecuteScalar() == 0)
                {
                    Console.WriteLine("❌ No seat found with this ID!");
                    return;
                }

                Console.Write("Student Name: ");
                string name = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("❌ Student name cannot be empty!");
                    return;
                }

                Console.Write("Roll Number: ");
                if (!int.TryParse(Console.ReadLine(), out int roll) || roll <= 0)
                {
                    Console.WriteLine("❌ Invalid roll number!");
                    return;
                }

                Console.Write("Department: ");
                string dept = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(dept))
                {
                    Console.WriteLine("❌ Department cannot be empty!");
                    return;
                }

                // ✅ CHECK ROLL NUMBER DUPLICATE (excluding current seat)
                using var dupCheck = new SqlCommand("SELECT COUNT(*) FROM Seats WHERE RollNumber = @r AND Id != @id", con);
                dupCheck.Parameters.AddWithValue("@r", roll.ToString());
                dupCheck.Parameters.AddWithValue("@id", id);

                if ((int)dupCheck.ExecuteScalar() > 0)
                {
                    Console.WriteLine("❌ Roll number already exists for another student!");
                    return;
                }

                // ✅ AUTO RECALCULATE ROOM
                int roomNumber = ((roll - 1) / 10) + 1;
                string roomName = "Room " + roomNumber;

                using var cmd = new SqlCommand(@"
                    UPDATE Seats SET 
                    StudentName = @n,
                    RollNumber = @r,
                    Department = @d,
                    RoomName = @room
                    WHERE Id = @id", con);

                cmd.Parameters.AddWithValue("@n", name);
                cmd.Parameters.AddWithValue("@r", roll.ToString());
                cmd.Parameters.AddWithValue("@d", dept);
                cmd.Parameters.AddWithValue("@room", roomName);
                cmd.Parameters.AddWithValue("@id", id);

                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("✔ Seat Updated Successfully!");
                    Console.WriteLine($"➡ New Room: {roomName}");
                }
                else
                {
                    Console.WriteLine("❌ Failed to update seat!");
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"❌ Database Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Unexpected Error: {ex.Message}");
            }
        }

        // ❌ DELETE SEAT
        public void DeleteSeat()
        {
            try
            {
                Console.Write("Enter ID to delete: ");
                if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
                {
                    Console.WriteLine("❌ Invalid ID!");
                    return;
                }

                using var con = DbConnection.GetConnection();
                con.Open();

                // ✅ GET SEAT DETAILS BEFORE DELETING
                using var getCmd = new SqlCommand("SELECT StudentName, RollNumber FROM Seats WHERE Id = @id", con);
                getCmd.Parameters.AddWithValue("@id", id);

                using var reader = getCmd.ExecuteReader();
                if (!reader.Read())
                {
                    Console.WriteLine("❌ No seat found with this ID!");
                    return;
                }

                string studentName = reader.GetString(0);
                string rollNumber = reader.GetString(1);
                reader.Close();

                // ✅ CONFIRM DELETION
                Console.Write($"Are you sure you want to delete seat for {studentName} (Roll: {rollNumber})? (y/n): ");
                string confirmation = Console.ReadLine()?.ToLower();

                if (confirmation != "y" && confirmation != "yes")
                {
                    Console.WriteLine("❌ Deletion cancelled!");
                    return;
                }

                using var cmd = new SqlCommand("DELETE FROM Seats WHERE Id = @id", con);
                cmd.Parameters.AddWithValue("@id", id);

                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine($"✔ Deleted Successfully! Removed {studentName} (Roll: {rollNumber})");
                }
                else
                {
                    Console.WriteLine("❌ Failed to delete seat!");
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"❌ Database Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Unexpected Error: {ex.Message}");
            }
        }

        // 🔄 RESET ALL SEATS
        public void ResetSeats()
        {
            try
            {
                Console.Write("⚠ WARNING: This will delete ALL seats! Are you sure? (y/n): ");
                string confirmation = Console.ReadLine()?.ToLower();

                if (confirmation != "y" && confirmation != "yes")
                {
                    Console.WriteLine("❌ Reset cancelled!");
                    return;
                }

                using var con = DbConnection.GetConnection();
                con.Open();

                // Get count before deletion
                using var countCmd = new SqlCommand("SELECT COUNT(*) FROM Seats", con);
                int seatCount = (int)countCmd.ExecuteScalar();

                if (seatCount == 0)
                {
                    Console.WriteLine("⚠ No seats to reset!");
                    return;
                }

                using var deleteCmd = new SqlCommand("DELETE FROM Seats", con);
                int deletedRows = deleteCmd.ExecuteNonQuery();

                using var reseedCmd = new SqlCommand("DBCC CHECKIDENT ('Seats', RESEED, 0)", con);
                reseedCmd.ExecuteNonQuery();

                Console.WriteLine($"⚠ Seats Reset Complete! {deletedRows} seat(s) deleted.");
                Console.WriteLine("✅ Identity counter reset to 0.");
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"❌ Database Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Unexpected Error: {ex.Message}");
            }
        }

        // 🔍 SEARCH SEATS BY DEPARTMENT
        public void SearchByDepartment()
        {
            try
            {
                Console.Write("Enter Department Name: ");
                string dept = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(dept))
                {
                    Console.WriteLine("❌ Department cannot be empty!");
                    return;
                }

                using var con = DbConnection.GetConnection();
                con.Open();

                using var cmd = new SqlCommand("SELECT Id, StudentName, RollNumber, RoomName FROM Seats WHERE Department = @d ORDER BY CAST(RollNumber AS INT)", con);
                cmd.Parameters.AddWithValue("@d", dept);

                using var reader = cmd.ExecuteReader();

                if (!reader.HasRows)
                {
                    Console.WriteLine($"⚠ No students found in {dept} department!");
                    return;
                }

                Console.WriteLine($"\n=========== {dept} DEPARTMENT SEATS ===========");
                Console.WriteLine("------------------------------------------------------");
                Console.WriteLine($"{"ID",-5} {"Name",-20} {"Roll",-10} {"Room",-10}");
                Console.WriteLine("------------------------------------------------------");

                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string studentName = reader.GetString(1);
                    string rollNumber = reader.GetString(2);
                    string roomName = reader.GetString(3);

                    Console.WriteLine($"{id,-5} {studentName,-20} {rollNumber,-10} {roomName,-10}");
                }

                Console.WriteLine("------------------------------------------------------");
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"❌ Database Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Unexpected Error: {ex.Message}");
            }
        }
    }
}