namespace LibraryManagementSystem.Services
{
    /// <summary>
    /// Central place for "business rule numbers" so nobody has to hunt
    /// through forms to find a hardcoded value.
    /// </summary>
    public static class AppConstants
    {
        /// <summary>Fine charged per day a book is returned late (BDT).</summary>
        public const decimal FinePerOverdueDay = 10.00m;

        /// <summary>Default loan period (in days) used when issuing a book,
        /// if the librarian doesn't pick a custom due date.</summary>
        public const int DefaultLoanPeriodDays = 14;
    }
}
