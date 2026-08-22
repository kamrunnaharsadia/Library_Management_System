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
    public partial class FineManagementForm : Form
    {
        private readonly FineService _fineService;
        private readonly int? _memberId;
        public FineManagementForm()
        {
            InitializeComponent();
            _fineService = new FineService(new FineRepository());
            _memberId = null;
        }

        public FineManagementForm(int memberId)
        {
            InitializeComponent();
            _fineService = new FineService(new FineRepository());
            _memberId = memberId;
        }
        private void FineManagementForm_Load(object sender, EventArgs e)
        {
            bool isStudentView = _memberId.HasValue;
            button2.Visible = !isStudentView;
            textBox1.Visible = !isStudentView;

            LoadGrid();
        }

        private void LoadGrid()
        {
            if (_memberId.HasValue)
            {
                 var fines  = _fineService.GetFinesForMember(_memberId.Value)
                    .Select(b => new
                    {
                        b.FineId,
                        Student = b.StudentName, 
                        Book = b.BookTitle,
                        Amount = b.Amount,
                        Reason = b.Reason,
                        Date = b.CreatedAt,
                        PaidStatus = b.PaidStatus,
                    }).ToList();
                dataGridView1.DataSource = fines;
            }
            else
            {
                var fines = _fineService.FindFines(new FineFilter
                {
                    Keyword = textBox1.Text
                }).Select(b => new
                    {
                        b.FineId,
                        Student = b.StudentName,
                        Book = b.BookTitle,
                        Amount = b.Amount,
                        Reason = b.Reason,
                        Date = b.CreatedAt,
                        PaidStatus = b.PaidStatus,
                    }).ToList();
                dataGridView1.DataSource = fines;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoadGrid();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var item = dataGridView1.CurrentRow?.DataBoundItem;
            if (item != null)
            {
                var paidStatus = item.GetType().GetProperty("PaidStatus")?.GetValue(item)?.ToString();
                if (paidStatus == "Paid")
                {
                    MessageBox.Show("This fine is already marked as paid.");
                    return;
                }

                var fineId = (int)item.GetType().GetProperty("FineId")?.GetValue(item);
                _fineService.MarkFinePaid(fineId);
                LoadGrid();
            }
            else
            {
                MessageBox.Show("Please select a fine record first.");
            }

        }
    }
}
