using Microsoft.Data.Sqlite;

namespace ExamCenterSystem.Data
{
    public static class DbConnection
    {
        public static SqliteConnection GetConnection()
        {
            // SQLite database file - container ke andar /app folder mein
            string connectionString = "Data Source=examcenter.db";
            return new SqliteConnection(connectionString);
        }
    }
}