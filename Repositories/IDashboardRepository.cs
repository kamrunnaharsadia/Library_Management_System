namespace LibraryManagementSystem.Repositories
{
    public class DashboardStats
    {
        public int TotalBooks { get; set; }
        public int AvailableBooks { get; set; }
        public int TotalMembers { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveBorrowings { get; set; }
        public int OverdueBooks { get; set; }
        public decimal TotalUnpaidFines { get; set; }
    }

    public interface IDashboardRepository
    {
        DashboardStats GetStats();
    }
}
