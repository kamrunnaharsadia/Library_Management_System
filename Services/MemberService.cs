using System.Collections.Generic;
using System.Text.RegularExpressions;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories;

namespace LibraryManagementSystem.Services
{
    public class MemberService
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IUserRepository _userRepository;
        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        public MemberService(IMemberRepository memberRepository, IUserRepository userRepository)
        {
            _memberRepository = memberRepository;
            _userRepository = userRepository;
        }

        public List<Member> GetAllMembers() => _memberRepository.GetAll();

        public List<Member> FindMembers(MemberFilter filter) => _memberRepository.Find(filter ?? new MemberFilter());

        public Member GetMemberByUserId(int userId) => _memberRepository.GetByUserId(userId);

        /// <summary>
        /// Registers a brand-new student: creates the Users row (RoleId = Student)
        /// AND the linked Members row in one operation, so a Member can never
        /// exist without its User account.
        /// </summary>
        public int RegisterMember(User user, Member member, string plainPassword, int studentRoleId)
        {
            if (string.IsNullOrWhiteSpace(user.FullName))
                throw new ServiceException("Full name is required.");
            if (string.IsNullOrWhiteSpace(user.Email) || !EmailRegex.IsMatch(user.Email))
                throw new ServiceException("Please enter a valid email address.");
            if (string.IsNullOrWhiteSpace(user.Username) || user.Username.Length < 3)
                throw new ServiceException("Username must be at least 3 characters.");
            if (string.IsNullOrWhiteSpace(plainPassword) || plainPassword.Length < 6)
                throw new ServiceException("Password must be at least 6 characters.");
            if (string.IsNullOrWhiteSpace(member.StudentId))
                throw new ServiceException("Student ID is required.");

            if (_userRepository.UsernameExists(user.Username))
                throw new ServiceException($"Username '{user.Username}' is already taken.");
            if (_memberRepository.StudentIdExists(member.StudentId))
                throw new ServiceException($"Student ID '{member.StudentId}' is already registered.");

            user.RoleId = studentRoleId;
            user.Password = AuthService.HashPassword(plainPassword);
            user.Status = "Active";
            int newUserId = _userRepository.Add(user);

            member.UserId = newUserId;
            return _memberRepository.Add(member);
        }

        public void UpdateMember(Member member)
        {
            if (string.IsNullOrWhiteSpace(member.StudentId))
                throw new ServiceException("Student ID is required.");
            if (_memberRepository.StudentIdExists(member.StudentId, member.MemberId))
                throw new ServiceException($"Student ID '{member.StudentId}' is already registered to another member.");
            _memberRepository.Update(member);
        }

        public void DeleteMember(int memberId)
        {
            // Business rule: a member with an active loan must return it before removal.
            if (_memberRepository.HasActiveBorrowings(memberId))
                throw new ServiceException("This member cannot be deleted while they have active borrowings.");
            _memberRepository.Delete(memberId);
        }
    }
}
