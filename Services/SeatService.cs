using ExamCenterSystem.Data;
using ExamCenterSystem.Models;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace ExamCenterSystem.Services
{
    public class SeatService
    {
        private List<Room> rooms = new List<Room>()
        {
            new Room { RoomId = 1, RoomName = "Room 101", Capacity = 2 },
            new Room { RoomId = 2, RoomName = "Room 102", Capacity = 2 }
        };

        private int roomIndex = 0;
        private int seatCount = 0;

        // 🪑 Allocate Seat + SAVE in DB
        public void AllocateSeat()
        {
            Console.Write("Enter Student Name: ");
            string name = Console.ReadLine();

            Console.Write("Enter Roll Number: ");
            string roll = Console.ReadLine();

            if (roomIndex >= rooms.Count)
            {
                Console.WriteLine("No rooms available!");
                return;
            }

            Room currentRoom = rooms[roomIndex];

            if (seatCount >= currentRoom.Capacity)
            {
                roomIndex++;
                seatCount = 0;

                if (roomIndex >= rooms.Count)
                {
                    Console.WriteLine("All rooms full!");
                    return;
                }

                currentRoom = rooms[roomIndex];
            }

            // Save in DB
            using (SqlConnection con = DbConnection.GetConnection())
            {
                con.Open();

                string query = "INSERT INTO Seats (StudentName, RollNumber, RoomName) VALUES (@n,@r,@room)";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@n", name);
                cmd.Parameters.AddWithValue("@r", roll);
                cmd.Parameters.AddWithValue("@room", currentRoom.RoomName);

                cmd.ExecuteNonQuery();
            }

            seatCount++;

            Console.WriteLine($"Seat Assigned → {name} in {currentRoom.RoomName}");
        }

        // 🪑 VIEW SEATS
        public void ViewSeats()
        {
            using (SqlConnection con = DbConnection.GetConnection())
            {
                con.Open();

                string query = "SELECT * FROM Seats";

                SqlCommand cmd = new SqlCommand(query, con);

                SqlDataReader reader = cmd.ExecuteReader();

                Console.WriteLine("\n--- SEAT ALLOCATION ---");

                while (reader.Read())
                {
                    Console.WriteLine(
                        $"ID: {reader["Id"]} | " +
                        $"Name: {reader["StudentName"]} | " +
                        $"Roll: {reader["RollNumber"]} | " +
                        $"Room: {reader["RoomName"]}"
                    );
                }
            }
        }
    }
}