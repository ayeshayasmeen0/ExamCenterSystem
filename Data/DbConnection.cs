using Microsoft.Data.SqlClient;

namespace ExamCenterSystem.Data
{
    public class DbConnection
    {
        public static SqlConnection GetConnection()
        {
            string conn = @"Server=(localdb)\MSSQLLocalDB;Database=ExamCenterDB;Trusted_Connection=True;";
            return new SqlConnection(conn);
        }
    }
}