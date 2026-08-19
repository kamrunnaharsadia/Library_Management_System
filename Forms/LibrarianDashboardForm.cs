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
    public partial class LibrarianDashboardForm : Form
    {
        private readonly DashboardService _dashboardService;
        private readonly BorrowingService _borrowingService;
        public LibrarianDashboardForm(string name)
        {
            InitializeComponent();
            label5.Text = name;
            _dashboardService = new DashboardService(new DashboardRepository());
            _borrowingService = new BorrowingService(
                new BorrowingRepository(), new BookRepository(), new MemberRepository(), new FineRepository());
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            _borrowingService.RefreshOverdueStatuses();
            var stats = _dashboardService.GetStats();
            label10.Text = stats.TotalBooks.ToString();
            label6.Text = stats.AvailableBooks.ToString();
            label8.Text = stats.ActiveBorrowings.ToString();
            label12.Text = stats.OverdueBooks.ToString();
            label14.Text = stats.TotalMembers.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BookManagementForm b = new BookManagementForm();
            b.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CategoryManagementForm c = new CategoryManagementForm(); 
            c.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MemberManagementForm m = new MemberManagementForm();
            m.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            IssueBookForm i = new IssueBookForm();
            i.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ReturnBookForm r = new ReturnBookForm();
            r.ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            BorrowingHistoryForm b = new BorrowingHistoryForm();
            b.ShowDialog();
        }

        private void button7_Click(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            ProfileForm p = new ProfileForm();
            p.ShowDialog();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            new AuthService(new UserRepository()).Logout();
            this.Close();
        }
    }
}
