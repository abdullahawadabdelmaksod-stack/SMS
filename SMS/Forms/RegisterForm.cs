using MaterialSkin;
using MaterialSkin.Controls;
using SMS.Data;
using SMS.Models;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SMS
{
    public class RegisterForm : MaterialForm
    {
        // ── Palette ──────────────────────────────────────────────────────────
        private static readonly Color BgMain  = Color.FromArgb(18, 18, 28);
        private static readonly Color BgCard  = Color.FromArgb(28, 30, 50);
        private static readonly Color Indigo  = Color.FromArgb(48, 63, 159);
        private static readonly Color Teal    = Color.FromArgb(0, 150, 136);
        private static readonly Color TextSub = Color.FromArgb(160, 170, 210);

        private readonly AppDbContext _context = new();

        private MaterialTextBox2 _txtUsername = null!;
        private MaterialTextBox2 _txtPassword = null!;
        private MaterialTextBox2 _txtConfirm  = null!;
        private MaterialComboBox  _cmbRole    = null!;
        private MaterialButton    _btnCreate  = null!;
        private Label             _lblStatus  = null!;

        public RegisterForm()
        {
            // MaterialSkin
            var skin = MaterialSkinManager.Instance;
            skin.AddFormToManage(this);
            skin.Theme       = MaterialSkinManager.Themes.DARK;
            skin.ColorScheme = new ColorScheme(
                Primary.Indigo800, Primary.Indigo900, Primary.Indigo500,
                Accent.Teal200, TextShade.WHITE);

            Text            = "Register New Account";
            Size            = new Size(880, 640);
            MinimumSize     = new Size(880, 640);
            MaximumSize     = new Size(880, 640);
            StartPosition   = FormStartPosition.CenterParent;
            BackColor       = BgMain;

            BuildUI();
        }

        private void BuildUI()
        {
            // ── Gradient header ───────────────────────────────────────────────
            var header = new Panel { Dock = DockStyle.Top, Height = 100, BackColor = Indigo };
            header.Paint += (_, e) =>
            {
                var g  = e.Graphics;
                var rc = header.ClientRectangle;
                if (rc.Width <= 0) return;
                using var grad = new LinearGradientBrush(rc,
                    Color.FromArgb(26, 35, 100), Color.FromArgb(72, 88, 200),
                    LinearGradientMode.Horizontal);
                g.FillRectangle(grad, rc);
                using var accent = new Pen(Color.FromArgb(80, 180, 255), 2);
                g.DrawLine(accent, 0, rc.Height - 1, rc.Width, rc.Height - 1);
                using var f1 = new Font("Segoe UI", 16F, FontStyle.Bold);
                using var f2 = new Font("Segoe UI", 9F);
                using var w  = new SolidBrush(Color.White);
                using var lw = new SolidBrush(Color.FromArgb(190, 210, 255));
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawString("Create New Account  ·  إنشاء حساب جديد", f1, w, 22, 18);
                g.DrawString("Fill in all fields · username and password must contain letters + numbers  |  املأ جميع الحقول", f2, lw, 24, 60);
            };

            // ── Card body ─────────────────────────────────────────────────────
            var card = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = BgCard,
                Padding   = new Padding(30, 20, 30, 20)
            };

            // Fields
            _txtUsername = Field("👤  Username (letters + numbers)  |  اسم المستخدم");
            _txtPassword = Field("🔒  Password (letters + numbers)  |  كلمة المرور", password: true);
            _txtConfirm  = Field("🔒  Confirm Password  |  تأكيد كلمة المرور", password: true);

            _cmbRole = new MaterialComboBox
            {
                Hint     = "Select Role  |  اختر الصلاحية",
                Dock     = DockStyle.Top,
                Height   = 50,
                Margin   = new Padding(0, 0, 0, 12)
            };
            _cmbRole.Items.AddRange(new object[] { "Admin", "Professor", "Parent" });

            _btnCreate = new MaterialButton
            {
                Text         = "CREATE ACCOUNT  ·  إنشاء حساب",
                HighEmphasis = true,
                AutoSize     = false,
                Dock         = DockStyle.Top,
                Height       = 48,
                Margin       = new Padding(0, 16, 0, 0)
            };
            _btnCreate.Click += BtnCreate_Click;

            _lblStatus = new Label
            {
                Text      = "",
                Font      = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(229, 57, 53),
                Dock      = DockStyle.Top,
                Height    = 28,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Add in reverse (last = top with Dock=Top)
            card.Controls.Add(_btnCreate);
            card.Controls.Add(_lblStatus);
            card.Controls.Add(_cmbRole);
            card.Controls.Add(_txtConfirm);
            card.Controls.Add(_txtPassword);
            card.Controls.Add(_txtUsername);

            Controls.Add(card);
            Controls.Add(header);
        }

        private static MaterialTextBox2 Field(string hint, bool password = false) =>
            new MaterialTextBox2
            {
                Hint                  = hint,
                UseSystemPasswordChar = password,
                Dock                  = DockStyle.Top,
                Height                = 50,
                Margin                = new Padding(0, 0, 0, 6)
            };

        // ── Validation & submit ───────────────────────────────────────────────
        private void BtnCreate_Click(object? sender, EventArgs e)
        {
            _lblStatus.Text = "";

            var username = _txtUsername.Text.Trim();
            var password = _txtPassword.Text;
            var confirm  = _txtConfirm.Text;
            var role     = _cmbRole.SelectedItem?.ToString();

            // Empty check
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirm)  ||
                string.IsNullOrWhiteSpace(role))
            { Warn("All fields are required."); return; }

            // Username rules
            if (username.Length < 4)
            { Warn("Username must be at least 4 characters."); return; }
            if (!username.Any(char.IsLetter))
            { Warn("Username must contain at least one letter (a–z)."); return; }
            if (!username.Any(char.IsDigit))
            { Warn("Username must contain at least one number (0–9)."); return; }
            if (!Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$"))
            { Warn("Username may only use letters, digits, and underscores."); return; }

            // Password rules
            if (password.Length < 6)
            { Warn("Password must be at least 6 characters."); return; }
            if (!password.Any(char.IsLetter))
            { Warn("Password must contain at least one letter (a–z)."); return; }
            if (!password.Any(char.IsDigit))
            { Warn("Password must contain at least one number (0–9)."); return; }

            // Confirm match
            if (password != confirm)
            { Warn("Passwords do not match."); return; }

            try
            {
                _btnCreate.Text    = "CREATING…  ·  جاري الإنشاء…";
                _btnCreate.Enabled = false;
                Application.DoEvents();

                if (_context.Users.Any(u => u.Username == username))
                { Warn("Username already exists. Choose a different one."); return; }

                _context.Users.Add(new User { Username = username, Password = password, Role = role });
                _context.SaveChanges();

                MessageBox.Show("Account created successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _btnCreate.Text    = "CREATE ACCOUNT  ·  إنشاء حساب";
                _btnCreate.Enabled = true;
            }
        }

        private void Warn(string msg)
        {
            _lblStatus.Text = $"  ⚠  {msg}";
            MessageBox.Show(msg, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        [DllImport("Gdi32.dll")]
        private static extern IntPtr CreateRoundRectRgn(int x1, int y1, int x2, int y2, int w, int h);
    }
}
