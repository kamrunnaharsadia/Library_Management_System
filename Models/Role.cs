namespace LibraryManagementSystem.Models
{
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }

        // Well-known role name constants avoid "magic strings" scattered in code
        public const string Admin = "Admin";
        public const string Librarian = "Librarian";
        public const string Student = "Student";
    }
}
