using System;
using Microsoft.Data.SqlClient;

namespace ExamCenterSystem.Data
{
    public static class DbConnection
    {
        // Change this connection string according to your SQL Server
        private static readonly string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=ExamCenterDB;Trusted_Connection=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}