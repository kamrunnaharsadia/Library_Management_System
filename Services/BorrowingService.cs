using System;
using System.Collections.Generic;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories;

namespace LibraryManagementSystem.Services
{
    /// <summary>
    /// The heart of the system: issuing books, returning books, and turning
    /// late returns into fines. Keeping this logic in one Service (instead of
    /// inside the Form's button click) is what makes the rules easy to test
    /// and explain.
    /// </summary>
    public class BorrowingService
    {
        private readonly IBorrowingRepository _borrowingRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IFineRepository _fineRepository;

        public BorrowingService(
            IBorrowingRepository borrowingRepository,
            IBookRepository bookRepository,
            IMemberRepository memberRepository,
            IFineRepository fineRepository)
        {
            _borrowingRepository = borrowingRepository;
            _bookRepository = bookRepository;
            _memberRepository = memberRepository;
            _fineRepository = fineRepository;
        }

        public List<Borrowing> GetActiveBorrowings() => _borrowingRepository.GetActive();

        public List<Borrowing> GetActiveBorrowingsForMember(int memberId) => _borrowingRepository.GetActiveByMember(memberId);

        public List<Borrowing> FindBorrowingHistory(BorrowingFilter filter) => _borrowingRepository.Find(filter ?? new BorrowingFilter());

        /// <summary>
        /// Issues a book to a member.
        /// Rules enforced here:
        ///   - book must exist and have at least 1 available copy
        ///   - member must exist
        ///   - due date cannot be before issue date
        /// </summary>
        public int IssueBook(int bookId, int memberId, int issuedByUserId, DateTime issueDate, DateTime dueDate)
        {
            var book = _bookRepository.GetById(bookId);
            if (book == null)
                throw new ServiceException("Selected book was not found.");
            if (book.AvailableQuantity <= 0)
                throw new ServiceException($"'{book.Title}' has no available copies right now.");

            var member = _memberRepository.GetById(memberId);
            if (member == null)
                throw new ServiceException("Selected member was not found.");

            if (dueDate < issueDate)
                throw new ServiceException("Due date cannot be earlier than the issue date.");

            var borrowing = new Borrowing
            {
                BookId = bookId,
                MemberId = memberId,
                IssuedBy = issuedByUserId,
                IssueDate = issueDate,
                DueDate = dueDate
            };

            int borrowingId = _borrowingRepository.Add(borrowing);

            // Atomic, guarded decrement - AdjustAvailableQuantity throws if it
            // would ever go negative (defends against a race condition where
            // two librarians issue the last copy at the same moment).
            _bookRepository.AdjustAvailableQuantity(bookId, -1);

            return borrowingId;
        }

        /// <summary>
        /// Returns a book. If it's late, creates a Fine record using the
        /// configured per-day rate (see AppConstants.FinePerOverdueDay).
        /// Returns the fine amount (0 if returned on time).
        /// </summary>
        public decimal ReturnBook(int borrowingId, DateTime returnDate)
        {
            var borrowing = _borrowingRepository.GetById(borrowingId);
            if (borrowing == null)
                throw new ServiceException("Borrowing record not found.");
            if (borrowing.Status == "Returned")
                throw new ServiceException("This book has already been returned.");

            _borrowingRepository.MarkReturned(borrowingId, returnDate);
            _bookRepository.AdjustAvailableQuantity(borrowing.BookId, +1);

            decimal fineAmount = 0m;
            int overdueDays = (returnDate.Date - borrowing.DueDate.Date).Days;
            if (overdueDays > 0)
            {
                fineAmount = overdueDays * AppConstants.FinePerOverdueDay;
                _fineRepository.Add(new Fine
                {
                    BorrowingId = borrowingId,
                    Amount = fineAmount,
                    Reason = $"Returned {overdueDays} day(s) late"
                });
            }

            return fineAmount;
        }

        /// <summary>
        /// Housekeeping job: flips any Active borrowing whose due date has
        /// passed to "Overdue" status. Call this once when the Admin/Librarian
        /// dashboard loads so the dashboard counts and grids stay accurate.
        /// </summary>
        public int RefreshOverdueStatuses()
        {
            var overdue = _borrowingRepository.GetOverdueActive();
            foreach (var b in overdue)
            {
                if (b.Status != "Overdue")
                    _borrowingRepository.UpdateStatus(b.BorrowingId, "Overdue");
            }
            return overdue.Count;
        }
    }
}
