using System.Collections.Generic;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Repositories
{
    public class MemberFilter
    {
        public string Keyword { get; set; }   // matches StudentId / Name / Email
        public string Department { get; set; }
        public string Semester { get; set; }
        public string Status { get; set; }
    }

    public interface IMemberRepository
    {
        List<Member> GetAll();
        Member GetById(int memberId);
        Member GetByUserId(int userId);
        List<Member> Find(MemberFilter filter);
        bool StudentIdExists(string studentId, int excludeMemberId = 0);
        bool HasActiveBorrowings(int memberId);
        int Add(Member member);
        void Update(Member member);
        void Delete(int memberId);
    }
}
