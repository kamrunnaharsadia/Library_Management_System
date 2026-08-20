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
    public partial class ReturnBookForm : Form
    {
        private readonly BorrowingService _borrowingService;
        public ReturnBookForm()
        {
            InitializeComponent();
            var bookRepo = new BookRepository();
            var memberRepo = new MemberRepository();
            var fineRepo = new FineRepository();
            _borrowingService = new BorrowingService(new BorrowingRepository(), bookRepo, memberRepo, fineRepo);
        }

        private void ReturnBookForm_Load(object sender, EventArgs e)
        {
            LoadGrid();
        }
        private void LoadGrid()
        {
            var activeBorrowings = _borrowingService.GetActiveBorrowings()
               .Select(b => new
              {
                Book = b.BookTitle,
                Student = b.StudentId,
                IssueDate = b.IssueDate,
                DueDate = b.DueDate,
                Status = b.Status
              })
             .ToList();

            dataGridView1.DataSource = activeBorrowings;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Please select a borrowing record to return.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int borrowingId = (int)dataGridView1.CurrentRow.Cells["BorrowingId"].Value;

            var confirm = MessageBox.Show("Confirm this book has been returned?", "Confirm Return",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                decimal fine = _borrowingService.ReturnBook(borrowingId, DateTime.Today);

                string message = fine > 0
                    ? $"Book returned. A late fine of {fine:C} (BDT) was applied."
                    : "Book returned on time. No fine applied.";
                MessageBox.Show(message, "Return Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadGrid();
            }
            catch (ServiceException ex)
            {
                MessageBox.Show(ex.Message, "Cannot Return Book", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception)
            {
                MessageBox.Show("An unexpected error occurred while returning the book.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoadGrid();
        }
    }
}
