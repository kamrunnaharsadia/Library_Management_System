using System;

namespace LibraryManagementSystem.Models
{
    public class Borrowing
    {
        public int BorrowingId { get; set; }
        public int BookId { get; set; }
        public int MemberId { get; set; }
        public int IssuedBy { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Status { get; set; }   // Active / Returned / Overdue

        // Convenience fields populated by JOIN, useful for grids/history screens
        public string BookTitle { get; set; }
        public string MemberName { get; set; }
        public string StudentId { get; set; }
        public string IssuedByName { get; set; }

        public bool IsOverdue => Status == "Active" && DateTime.Today > DueDate;
    }
}
