using Library_Management_System.Data;
using System.Data.SqlClient;

namespace LibraryManagementSystem.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        public DashboardStats GetStats()
        {
            var stats = new DashboardStats();
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                stats.TotalBooks = Scalar(conn, "SELECT COUNT(*) FROM Books");
                stats.AvailableBooks = Scalar(conn, "SELECT ISNULL(SUM(AvailableQuantity),0) FROM Books");
                stats.TotalMembers = Scalar(conn, "SELECT COUNT(*) FROM Members");
                stats.TotalUsers = Scalar(conn, "SELECT COUNT(*) FROM Users");
                stats.ActiveBorrowings = Scalar(conn, "SELECT COUNT(*) FROM Borrowings WHERE Status IN ('Active','Overdue')");
                stats.OverdueBooks = Scalar(conn, "SELECT COUNT(*) FROM Borrowings WHERE Status IN ('Active','Overdue') AND DueDate < CAST(GETDATE() AS DATE)");
                stats.TotalUnpaidFines = ScalarDecimal(conn, "SELECT ISNULL(SUM(Amount),0) FROM Fines WHERE PaidStatus='Unpaid'");
            }
            return stats;
        }

        private static int Scalar(SqlConnection conn, string sql)
        {
            using (var cmd = new SqlCommand(sql, conn))
                return (int)cmd.ExecuteScalar();
        }

        private static decimal ScalarDecimal(SqlConnection conn, string sql)
        {
            using (var cmd = new SqlCommand(sql, conn))
                return (decimal)cmd.ExecuteScalar();
        }
    }
}
