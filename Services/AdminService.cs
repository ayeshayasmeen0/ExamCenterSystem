using System;
using Microsoft.Data.SqlClient;
using ExamCenterSystem.Data;

namespace ExamCenterSystem.Services
{
    public class AdminService
    {
        public void ResetAll()
        {
            using var con = DbConnection.GetConnection();
            con.Open();

            new SqlCommand("DELETE FROM Students", con).ExecuteNonQuery();
            new SqlCommand("DELETE FROM Seats", con).ExecuteNonQuery();
            new SqlCommand("DELETE FROM Exams", con).ExecuteNonQuery();
            new SqlCommand("DELETE FROM Results", con).ExecuteNonQuery();

            Console.WriteLine("✔ System Reset Successfully!");
        }
    }
}