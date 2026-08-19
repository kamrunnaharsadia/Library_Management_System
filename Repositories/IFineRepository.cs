using System.Collections.Generic;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Repositories
{
    public class FineFilter
    {
        public string Keyword { get; set; }     // matches student name / student id
        public string PaidStatus { get; set; }  // Unpaid / Paid
    }

    public interface IFineRepository
    {
        List<Fine> GetAll();
        List<Fine> GetByMember(int memberId);
        List<Fine> Find(FineFilter filter);
        Fine GetByBorrowingId(int borrowingId);
        int Add(Fine fine);
        void MarkPaid(int fineId);
        decimal GetTotalUnpaid();
        decimal GetTotalUnpaidByMember(int memberId);
    }
}
