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
    public partial class BorrowingHistoryForm : Form
    {
        private readonly BorrowingService _borrowingService;
        private readonly int? _memberId;
        public BorrowingHistoryForm()
        {
            InitializeComponent();
            _borrowingService = new BorrowingService(
                new BorrowingRepository(), new BookRepository(), new MemberRepository(), new FineRepository());
            _memberId = null;
        }

        public BorrowingHistoryForm(int memberId)
        {
            InitializeComponent();
            _borrowingService = new BorrowingService(
                new BorrowingRepository(), new BookRepository(), new MemberRepository(), new FineRepository());
            _memberId = memberId;
        }
        private void BorrowingHistoryForm_Load(object sender, EventArgs e)
        {
            bool isStudentView = _memberId.HasValue;
            textBox3.Visible = !isStudentView;
            label3.Visible = !isStudentView;
            Button1.Visible = !isStudentView;
            LoadGrid();
        }

        private void LoadGrid()
        {
            var filter = new BorrowingFilter
            {
                Keyword = _memberId.HasValue ? null : textBox3.Text
            };

            var results = _borrowingService.FindBorrowingHistory(filter)
               .Select(b => new
               {
                   MemberId = b.MemberId,
                   Member = b.MemberName,
                   Book = b.BookTitle,
                   IssueDate = b.IssueDate,
                   DueDate = b.DueDate,
                   Return = b.ReturnDate,
                   Status = b.Status
               })
             .ToList();

            if (_memberId.HasValue)
                results = results.FindAll(b => b.MemberId == _memberId.Value);


            dataGridView1.DataSource = results;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            LoadGrid();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
