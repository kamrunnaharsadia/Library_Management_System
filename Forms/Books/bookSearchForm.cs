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
    public partial class BookSearchForm : Form
    {
        private readonly BookService _bookService;
        private readonly CategoryService _categoryService;
        public BookSearchForm()
        {
            InitializeComponent();
            _bookService = new BookService(new BookRepository());
            _categoryService = new CategoryService(new CategoryRepository());
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void BookSearchForm_Load(object sender, EventArgs e)
        {
            comboBox1.DataSource = _categoryService.GetAllCategories();
            comboBox1.DisplayMember = "CategoryName";
            comboBox1.ValueMember = "CategoryId";

            LoadGrid(new BookFilter());
        }
        private void LoadGrid(BookFilter filter) => dataGridView1.DataSource = _bookService.FindBooks(filter);

        private void button1_Click(object sender, EventArgs e)
        {
            LoadGrid(new BookFilter
            {
                Keyword = textBox1.Text,
                CategoryId = comboBox1.SelectedValue as int?,
                AvailableOnly = checkBox1.Checked
            });
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
