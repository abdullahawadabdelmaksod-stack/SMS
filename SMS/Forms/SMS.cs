using SMS.Models;
using SMS.Services;
using System;
using System.Windows.Forms;
using System.Xml.Linq;

namespace SMS
{
    public partial class SMS : Form
    {
        private StudentService _service = new StudentService();

        public SMS()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            dataGridView1.DataSource = _service.GetStudents();
        }

        private void ClearFields()
        {
            txtId.Clear();
            txtName.Clear();
            txtAge.Clear();
            txtDepartment.Clear();
        }

        // ADD
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Enter Name");
                return;
            }

            if (!int.TryParse(txtAge.Text, out int age))
            {
                MessageBox.Show("Age must be number");
                return;
            }
            try
            {
                var student = new Student
                {
                    Name = txtName.Text,
                    Age = int.Parse(txtAge.Text),
                    Department = txtDepartment.Text
                };

                _service.AddStudent(student);
                MessageBox.Show("Added Successfully");
                LoadData();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // UPDATE
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                var student = new Student
                {
                    Id = int.Parse(txtId.Text),
                    Name = txtName.Text,
                    Age = int.Parse(txtAge.Text),
                    Department = txtDepartment.Text
                };

                _service.UpdateStudent(student);
                LoadData();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // DELETE
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // delete code åäÇ
            }
            try
            {
                _service.DeleteStudent(int.Parse(txtId.Text));
                LoadData();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // SEARCH
        private void btnSearch_Click(object sender, EventArgs e)
        {
            var data = _service.GetStudents()
            .Where(s => s.Name.Contains(txtSearch.Text)
            || s.Department.Contains(txtSearch.Text))
               .ToList();

            dataGridView1.DataSource = data;
            try
            {
                dataGridView1.DataSource = _service.Search(txtSearch.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // CLICK ROW (ÇÎÊíÇÑí ÈÓ ãåã)
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dataGridView1.Rows[e.RowIndex];

                txtId.Text = row.Cells["Id"].Value.ToString();
                txtName.Text = row.Cells["Name"].Value.ToString();
                txtAge.Text = row.Cells["Age"].Value.ToString();
                txtDepartment.Text = row.Cells["Department"].Value.ToString();
            }
        }

        
    }
}
       


