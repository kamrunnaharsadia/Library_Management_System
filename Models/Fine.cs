using System;

namespace LibraryManagementSystem.Models
{
    public class Fine
    {
        public int FineId { get; set; }
        public int BorrowingId { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public string PaidStatus { get; set; }  // Unpaid / Paid
        public DateTime CreatedAt { get; set; }

        // Convenience fields populated by JOIN
        public string StudentName { get; set; }
        public string StudentId { get; set; }
        public string BookTitle { get; set; }
    }
}
