using LibraryManagementSystem.Models;
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
    public partial class BookManagementForm : Form
    {
        private readonly BookService _bookService;
        private readonly CategoryService _categoryService;
        private int _selectedBookId = 0;
        public BookManagementForm()
        {
            InitializeComponent();
            _bookService = new BookService(new BookRepository());
            _categoryService = new CategoryService(new CategoryRepository());
            bool canEdit = AuthService.CurrentUser.RoleName == Role.Admin || AuthService.CurrentUser.RoleName == Role.Librarian;
            button1.Enabled = button2.Enabled = button3.Enabled = canEdit;
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void LoadGrid(BookFilter filter)
        {
            dataGridView1.DataSource = _bookService.FindBooks(filter);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;
            var book = (Book)dataGridView1.CurrentRow.DataBoundItem;
            if (book == null) return;

            _selectedBookId = book.BookId;
            textBox1.Text = book.ISBN;
            textBox2.Text = book.Title;
            textBox3.Text = book.Author;
            textBox4.Text = book.Publisher;
            comboBox1.SelectedValue = book.CategoryId;
            textBox6.Text = book.PublicationYear.ToString();
            numericUpDown2.Value = book.Quantity;
            textBox8.Text = book.ShelfLocation;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var filter = new BookFilter
            {
                Keyword = textBox9.Text,
                CategoryId = comboBox1.SelectedValue as int?,
                AvailableOnly = checkBox1.Checked
            };
            LoadGrid(filter);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private Book ReadFormIntoBook() => new Book
        {
            ISBN = textBox1.Text.Trim(),
            Title = textBox2.Text.Trim(),
            Author = textBox3.Text.Trim(),
            Publisher = textBox4.Text.Trim(),
            CategoryId = (int)(comboBox1.SelectedValue ?? 0),
            PublicationYear = Convert.ToInt32(textBox6.Text),
            Quantity = (int)numericUpDown2.Value,
            ShelfLocation = textBox8.Text.Trim(),
            Status = "Active"
        };


        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var book = ReadFormIntoBook();
                _bookService.AddBook(book);
                MessageBox.Show("Book added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
                LoadGrid(new BookFilter());
            }
            catch (ServiceException ex)
            {
                MessageBox.Show(ex.Message, "Cannot Add Book", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception)
            {
                MessageBox.Show("An unexpected error occurred while adding the book.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (_selectedBookId == 0)
            {
                MessageBox.Show("Please select a book from the list first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var book = ReadFormIntoBook();
                book.BookId = _selectedBookId;
                _bookService.UpdateBook(book);
                MessageBox.Show("Book updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
                LoadGrid(new BookFilter());
            }
            catch (ServiceException ex)
            {
                MessageBox.Show(ex.Message, "Cannot Update Book", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception)
            {
                MessageBox.Show("An unexpected error occurred while updating the book.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (_selectedBookId == 0)
            {
                MessageBox.Show("Please select a book from the list first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var confirm = MessageBox.Show("Are you sure you want to delete this book?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                _bookService.DeleteBook(_selectedBookId);
                MessageBox.Show("Book deleted.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
                LoadGrid(new BookFilter());
            }
            catch (ServiceException ex)
            {
                MessageBox.Show(ex.Message, "Cannot Delete Book", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception)
            {
                MessageBox.Show("An unexpected error occurred while deleting the book.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            _selectedBookId = 0;
            textBox1.Clear(); textBox2.Clear(); textBox3.Clear(); textBox4.Clear(); textBox8.Clear();
            textBox6.Text = DateTime.Now.Year.ToString();
            numericUpDown2.Value = 1;
        }

        private void BookManagementForm_Load(object sender, EventArgs e)
        {
            comboBox1.DataSource = _categoryService.GetAllCategories();
            comboBox1.DisplayMember = "CategoryName";
            comboBox1.ValueMember = "CategoryId";

            LoadGrid(new BookFilter());
        }

        private void label14_Click(object sender, EventArgs e)
        {

        }
    }
}
