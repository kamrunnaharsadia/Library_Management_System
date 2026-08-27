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
    public partial class MemberManagementForm : Form
    {
        private readonly MemberService _memberService;
        private readonly IRoleRepository _roleRepository; 
        private int _selectedMemberId = 0;
        public MemberManagementForm()
        {
            InitializeComponent();
            _memberService = new MemberService(new MemberRepository(), new UserRepository());
            _roleRepository = new RoleRepository();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void MemberManagementForm_Load(object sender, EventArgs e)
        {
            LoadGrid(new MemberFilter());
        }
        private void LoadGrid(MemberFilter filter) {
            var members = _memberService.FindMembers(filter).ToList();
            dataGridView1.DataSource = members;
            foreach(DataGridViewColumn col in dataGridView1.Columns)
    {
                if (col.Name != "StudentId" &&
                    col.Name != "FullName" &&
                    col.Name != "Department" &&
                    col.Name != "Semester")
                {
                    col.Visible = false;
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            LoadGrid(new MemberFilter
            {
                Keyword = textBox1.Text
            });
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentRow?.DataBoundItem is Member m)
            {
                _selectedMemberId = m.MemberId;
                textBox2.Text = m.FullName;
                textBox3.Text = m.Email;
                textBox6.Text = m.StudentId;
                textBox7.Text = m.Department;
                textBox8.Text = m.Semester;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int studentRoleId = _roleRepository.GetRoleIdByName(Role.Student);

                var user = new User { FullName = textBox2.Text.Trim(), Username = textBox4.Text.Trim(), Email = textBox3.Text.Trim() };
                var member = new Member { StudentId = textBox6.Text.Trim(), Department = textBox7.Text.Trim(), Semester = textBox8.Text.Trim() };

                _memberService.RegisterMember(user, member, textBox5.Text, studentRoleId);

                MessageBox.Show("Member registered successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
                LoadGrid(new MemberFilter());
            }
            catch (ServiceException ex)
            {
                MessageBox.Show(ex.Message, "Cannot Register Member", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (_selectedMemberId == 0) { MessageBox.Show("Select a member first."); return; }
            try
            {
                _memberService.UpdateMember(new Member
                {
                    MemberId = _selectedMemberId,
                    StudentId = textBox6.Text.Trim(),
                    Department = textBox7.Text.Trim(),
                    Semester = textBox8.Text.Trim()
                });
                MessageBox.Show("Member updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
                LoadGrid(new MemberFilter());
            }
            catch (ServiceException ex)
            {
                MessageBox.Show(ex.Message, "Cannot Update Member", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (_selectedMemberId == 0) { MessageBox.Show("Select a member first."); return; }
            if (MessageBox.Show("Delete this member?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                _memberService.DeleteMember(_selectedMemberId);
                ClearForm();
                LoadGrid(new MemberFilter());
            }
            catch (ServiceException ex)
            {
                MessageBox.Show(ex.Message, "Cannot Delete Member", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void ClearForm()
        {
            _selectedMemberId = 0;
            textBox2.Clear(); textBox3.Clear(); textBox4.Clear(); textBox5.Clear();
            textBox6.Clear(); textBox7.Clear(); textBox8.Clear();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ClearForm();
        }
    }
}
