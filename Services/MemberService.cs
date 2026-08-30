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

            int userId;

            // If username exists, reuse that user’s ID
            if (_userRepository.UsernameExists(user.Username))
            {
                var existingUser = _userRepository.GetByUsername(user.Username);
                userId = existingUser.UserId;

                if (_memberRepository.GetByUserId(userId) != null)
                    throw new ServiceException($"User '{user.Username}' is already registered as a member.");
            }
            else
            {
                // Otherwise, create a new user
                user.RoleId = studentRoleId;
                user.Password = plainPassword;
                user.Status = "Active";
                userId = _userRepository.Add(user);
            }

            // Ensure StudentId is unique in Members
            if (_memberRepository.StudentIdExists(member.StudentId))
                throw new ServiceException($"Student ID '{member.StudentId}' is already registered.");

            member.UserId = userId;
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
            if (_memberRepository.HasActiveBorrowings(memberId))
                throw new ServiceException("This member cannot be deleted while they have active borrowings.");
            _memberRepository.Delete(memberId);
        }
    }
}
