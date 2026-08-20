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
    public partial class ProfileForm : Form
    {
        private readonly AuthService _authService;
        public ProfileForm()
        {
            InitializeComponent();
            _authService = new AuthService(new UserRepository());
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void ProfileForm_Load(object sender, EventArgs e)
        {
            var user = AuthService.CurrentUser;
            label13.Text = user.FullName;
            label12.Text = user.Username;
            label11.Text = user.Email;
            label10.Text = user.RoleName;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                _authService.ChangePassword(
                    AuthService.CurrentUser.UserId,
                    textBox1.Text,
                    textBox2.Text,
                    textBox3.Text);

                MessageBox.Show("Password changed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox1.Clear(); textBox2.Clear(); textBox3.Clear();
            }
            catch (ServiceException ex)
            {
                MessageBox.Show(ex.Message, "Cannot Change Password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
