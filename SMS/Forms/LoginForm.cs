using Microsoft.EntityFrameworkCore;
using SMS.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMS
{
    public partial class LoginForm : Form
    {
        AppDbContext _context = new AppDbContext();
        public LoginForm()
        {
            InitializeComponent();
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            // تحقق إن المستخدم مدخل بيانات
            if (username == "" || password == "")
            {
                MessageBox.Show("Please enter username and password");
                return;
            }

            // البحث في الداتا بيز
            var user = _context.Users
                .FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                MessageBox.Show("Login Success");

                SMS mainForm = new SMS(); // الفورم اللي بعد اللوجين
                mainForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Invalid Username or Password");
            }
        }

       
    }
}
