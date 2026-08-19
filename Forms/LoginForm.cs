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
    public partial class LoginForm : Form
    {
        private readonly AuthService _authService;
        public LoginForm()
        {
            InitializeComponent();
            _authService = new AuthService(new UserRepository());
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }
        private void OpenDashboardFor(User user)
        {
            Form dashboard;
            switch (user.RoleName)
            {
                case "Admin":
                    dashboard = new AdminDashboardForm(user.FullName);
                    break;
                case "Librarian":
                    dashboard = new LibrarianDashboardForm(user.FullName);
                    break;
                case "Student":
                    dashboard = new StudentDashboardForm(user.FullName);
                    break;
                default:
                    MessageBox.Show("Unknown role. Please contact the administrator.");
                    return;
            }

            dashboard.FormClosed += (s, args) => this.Close(); 
            dashboard.Show();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                User user = _authService.Login(textBox1.Text, textBox2.Text);
                OpenDashboardFor(user);
                this.Hide();
            }
            catch (ServiceException ex)
            {
                MessageBox.Show(ex.Message, "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception)
            {
                MessageBox.Show("Something went wrong while trying to log in. Please try again.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox1.Focus();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
