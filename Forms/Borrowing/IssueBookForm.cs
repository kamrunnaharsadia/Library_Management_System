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
    public partial class IssueBookForm : Form
    {
        private readonly BorrowingService _borrowingService;
        private readonly MemberService _memberService;
        private readonly BookService _bookService;
        public IssueBookForm()
        {
            InitializeComponent();
            var borrowingRepo = new BorrowingRepository();
            var bookRepo = new BookRepository();
            var memberRepo = new MemberRepository();
            var fineRepo = new FineRepository();

            _borrowingService = new BorrowingService(borrowingRepo, bookRepo, memberRepo, fineRepo);
            _memberService = new MemberService(memberRepo, new UserRepository());
            _bookService = new BookService(bookRepo);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void IssueBookForm_Load(object sender, EventArgs e)
        {
            comboBox1.DataSource = _memberService.GetAllMembers();
            comboBox1.DisplayMember = "StudentId"; 
            comboBox1.ValueMember = "MemberId";

            comboBox2.DataSource = _bookService.FindBooks(new BookFilter { AvailableOnly = true });
            comboBox2.DisplayMember = "Title";
            comboBox2.ValueMember = "BookId";

            dateTimePicker1.Value = DateTime.Today;
            dateTimePicker2.Value = DateTime.Today.AddDays(AppConstants.DefaultLoanPeriodDays);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.SelectedValue == null || comboBox2.SelectedValue == null)
                {
                    MessageBox.Show("Please select both a member and a book.", "Missing Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int memberId = (int)comboBox1.SelectedValue;
                int bookId = (int)comboBox2.SelectedValue;
                int issuedByUserId = AuthService.CurrentUser.UserId;

                _borrowingService.IssueBook(bookId, memberId, issuedByUserId, dateTimePicker1.Value.Date, dateTimePicker2.Value.Date);

                MessageBox.Show("Book issued successfully.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (ServiceException ex)
            {
                MessageBox.Show(ex.Message, "Cannot Issue Book", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception)
            {
                MessageBox.Show("An unexpected error occurred while issuing the book.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
