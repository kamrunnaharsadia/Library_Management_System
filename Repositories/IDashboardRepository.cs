namespace LibraryManagementSystem.Repositories
{
    /// <summary>Simple aggregate counts shown as the "cards" on every dashboard.</summary>
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
