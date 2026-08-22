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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Library_Management_System.Forms
{
    public partial class UserManagementForm : Form
    {
        private readonly UserService _userService;
        private readonly IRoleRepository _roleRepository;
        private int _selectedUserId = 0;
        public UserManagementForm()
        {
            InitializeComponent();
            _userService = new UserService(new UserRepository());
            _roleRepository = new RoleRepository();
        }

        private void UserManagementForm_Load(object sender, EventArgs e)
        {
            comboBox1.DataSource = _roleRepository.GetAll();
            comboBox1.DisplayMember = "RoleName";
            comboBox1.ValueMember = "RoleId";

            LoadGrid(null);
        }

        private void LoadGrid(string keyword)
        {
            var users = _userService.SearchUsers(keyword)
                .Select( b =>  new
                {
                    Name = b.FullName,
                    Username = b.Username,
                    Role = b.RoleName,
                    Status = b.Status
                }
                ).ToList();

            dataGridView1.DataSource = users;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            LoadGrid(textBox6.Text);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentRow?.DataBoundItem is User u)
            {
                _selectedUserId = u.UserId;
                textBox1.Text = u.FullName;
                textBox2.Text = u.Username;
                textBox3.Text = u.Email;
                textBox4.Text = u.Phone;
                comboBox1.SelectedValue = u.RoleId;
                checkBox1.Checked = u.IsActive;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var user = ReadFormIntoUser();
                _userService.CreateUser(user, textBox5.Text);
                MessageBox.Show("User created.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
                LoadGrid(null);
            }
            catch (ServiceException ex)
            {
                MessageBox.Show(ex.Message, "Cannot Add User", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (_selectedUserId == 0) { MessageBox.Show("Select a user first."); return; }
            try
            {
                var user = ReadFormIntoUser();
                user.UserId = _selectedUserId;
                _userService.UpdateUser(user);
                MessageBox.Show("User updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
                LoadGrid(null);
            }
            catch (ServiceException ex)
            {
                MessageBox.Show(ex.Message, "Cannot Update User", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (_selectedUserId == 0) { MessageBox.Show("Select a user first."); return; }
            if (MessageBox.Show("Delete this user?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                _userService.DeleteUser(_selectedUserId);
                ClearForm();
                LoadGrid(null);
            }
            catch (ServiceException ex)
            {
                MessageBox.Show(ex.Message, "Cannot Delete User", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private User ReadFormIntoUser() => new User
        {
            FullName = textBox1.Text.Trim(),
            Username = textBox2.Text.Trim(),
            Email = textBox3.Text.Trim(),
            Phone = textBox4.Text.Trim(),
            RoleId = (int)(comboBox1.SelectedValue ?? 0),
            Status = checkBox1.Checked ? "Active" : "Inactive"
        };

        private void ClearForm()
        {
            _selectedUserId = 0;
            textBox1.Clear(); textBox2.Clear(); textBox3.Clear(); textBox4.Clear(); textBox5.Clear();
            checkBox1.Checked = true;
        }
    }
}
