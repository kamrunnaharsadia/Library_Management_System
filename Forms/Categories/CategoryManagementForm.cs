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
    public partial class CategoryManagementForm : Form
    {
        private readonly CategoryService _categoryService;
        private int _selectedCategoryId = 0;
        public CategoryManagementForm()
        {
            InitializeComponent();
            _categoryService = new CategoryService(new CategoryRepository());
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void CategoryManagementForm_Load(object sender, EventArgs e)
        {
            LoadGrid(null);
        }
        private void LoadGrid(string keyword)
        {
            var categories = _categoryService.SearchCategories(keyword).ToList();
            dataGridView1.DataSource = categories;
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                if (col.Name != "CategoryName" &&
                    col.Name != "Description" 
                   )
                {
                    col.Visible = false;
                }

            }
        }


        private void button5_Click(object sender, EventArgs e)
        {
            LoadGrid(textBox3.Text);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentRow?.DataBoundItem is Category c)
            {
                _selectedCategoryId = c.CategoryId;
                textBox1.Text = c.CategoryName;
                textBox2.Text = c.Description;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                _categoryService.AddCategory(new Category { CategoryName = textBox1.Text.Trim(), Description = textBox2.Text.Trim() });
                MessageBox.Show("Category added.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
                LoadGrid(null);
            }
            catch (ServiceException ex)
            {
                MessageBox.Show(ex.Message, "Cannot Add Category", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (_selectedCategoryId == 0) { MessageBox.Show("Select a category first."); return; }
            try
            {
                _categoryService.UpdateCategory(new Category
                {
                    CategoryId = _selectedCategoryId,
                    CategoryName = textBox1.Text.Trim(),
                    Description = textBox2.Text.Trim()
                });
                MessageBox.Show("Category updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
                LoadGrid(null);
            }
            catch (ServiceException ex)
            {
                MessageBox.Show(ex.Message, "Cannot Update Category", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (_selectedCategoryId == 0) { MessageBox.Show("Select a category first."); return; }
            if (MessageBox.Show("Delete this category?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                _categoryService.DeleteCategory(_selectedCategoryId);
                ClearForm();
                LoadGrid(null);
            }
            catch (ServiceException ex)
            {
                MessageBox.Show(ex.Message, "Cannot Delete Category", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void ClearForm()
        {
            _selectedCategoryId = 0;
            textBox1.Clear();
            textBox2.Clear();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
