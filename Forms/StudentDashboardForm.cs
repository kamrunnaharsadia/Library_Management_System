using LibraryManagementSystem.Repositories;
using LibraryManagementSystem.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Library_Management_System.Forms
{
    public partial class StudentDashboardForm : Form
    {
        private readonly MemberService _memberService;
        private readonly BorrowingService _borrowingService;
        private readonly FineService _fineService;
        private int _memberId;
        public StudentDashboardForm(string name)
        {
            InitializeComponent();
            label6.Text = name;
            var memberRepo = new MemberRepository();
            _memberService = new MemberService(memberRepo, new UserRepository());
            _borrowingService = new BorrowingService(new BorrowingRepository(), new BookRepository(), memberRepo, new FineRepository());
            _fineService = new FineService(new FineRepository());
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ProfileForm p = new ProfileForm();
            p.ShowDialog();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            var member = _memberService.GetMemberByUserId(AuthService.CurrentUser.UserId);
            if (member == null)
            {
                MessageBox.Show("No member profile is linked to this account. Please contact the librarian.",
                    "Profile Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            _memberId = member.MemberId;

            var activeBorrowings = _borrowingService.GetActiveBorrowingsForMember(_memberId);
            label10.Text = activeBorrowings.Count.ToString();
            label9.Text = _fineService.GetTotalUnpaidForMember(_memberId).ToString("C");

            dataGridView1.DataSource = activeBorrowings;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BookSearchForm b = new BookSearchForm();
            b.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            //BorrowingHistoryForm b = new BorrowingHistoryForm(_memberId);
            //    b.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //FineManagementForm f = new FineManagementForm(_memberId);    
            // f.ShowDialog();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            new AuthService(new UserRepository()).Logout();
            LoginForm loginForm = new LoginForm();
            this.Dispose();
            loginForm.ShowDialog();
        }

        private void StudentDashboardForm_Load(object sender, EventArgs e)
        {

        }
    }
}
