using System.Collections.Generic;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Repositories
{
    public class BorrowingFilter
    {
        public string Keyword { get; set; }   // matches member name / student id / book title
        public string Status { get; set; }    // Active / Returned / Overdue
    }

    public interface IBorrowingRepository
    {
        List<Borrowing> GetActive();
        List<Borrowing> GetActiveByMember(int memberId);
        List<Borrowing> Find(BorrowingFilter filter);
        Borrowing GetById(int borrowingId);
        int Add(Borrowing borrowing);
        void MarkReturned(int borrowingId, System.DateTime returnDate);
        void UpdateStatus(int borrowingId, string status);
        List<Borrowing> GetOverdueActive(); // Active borrowings whose DueDate has passed
    }
}
