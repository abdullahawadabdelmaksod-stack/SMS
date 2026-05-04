using MaterialSkin;
using MaterialSkin.Controls;
using SMS.Data;
using SMS.Models;
using System.Linq;

namespace SMS
{
    public partial class LoginForm : MaterialForm
    {
        public LoginForm()
        {
            InitializeComponent();

            // ── MaterialSkin setup ────────────────────────────────────────────
            var skin = MaterialSkinManager.Instance;
            skin.AddFormToManage(this);
            skin.Theme = MaterialSkinManager.Themes.DARK;
            skin.ColorScheme = new ColorScheme(
                Primary.Indigo800, Primary.Indigo900, Primary.Indigo500,
                Accent.Teal200, TextShade.WHITE);

            ApplyFonts();

            // ── Logo ─────────────────────────────────────────────────────────
            try
            {
                var icoPath = System.IO.Path.Combine(
                    System.AppDomain.CurrentDomain.BaseDirectory, "Resources", "app.ico");
                if (System.IO.File.Exists(icoPath))
                    Icon = new System.Drawing.Icon(icoPath);
            }
            catch { }

            try
            {
                var logoPath = System.IO.Path.Combine(
                    System.AppDomain.CurrentDomain.BaseDirectory, "Resources", "sms_icon.png");
                if (System.IO.File.Exists(logoPath))
                    picLogo.Image = System.Drawing.Image.FromFile(logoPath);
            }
            catch { }
            UIHelper.MakeCircular(picLogo);

            // ── Centre card ───────────────────────────────────────────────────
            CentreCard();
            Resize += (_, _) => CentreCard();
            Shown  += (_, _) => ApplyFonts();
        }

        private void ApplyFonts()
        {
            lblTitle.Font    = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 14F);
            lblSubtitle.Height = 44;
        }

        private void CentreCard()
        {
            loginCard.Location = new System.Drawing.Point(
                (ClientSize.Width  - loginCard.Width)  / 2,
                (ClientSize.Height - loginCard.Height) / 2);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            // ── Empty check ───────────────────────────────────────────────────
            if (username == "" || password == "")
            {
                MessageBox.Show("Please enter username and password.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ── Format: must contain both letters AND digits ──────────────────
            if (!username.Any(char.IsLetter) || !username.Any(char.IsDigit))
            {
                MessageBox.Show(
                    "Username must contain both letters and numbers.\n" +
                    "Example:  ahmed1",
                    "Invalid Format", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!password.Any(char.IsLetter) || !password.Any(char.IsDigit))
            {
                MessageBox.Show(
                    "Password must contain both letters and numbers.\n" +
                    "Example:  pass123",
                    "Invalid Format", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ── DB lookup ─────────────────────────────────────────────────────
            using var db = new AppDbContext();
            var norm = username.ToLowerInvariant();
            var user = db.Users.FirstOrDefault(
                u => u.Username.ToLower() == norm && u.Password == password);

            if (user != null)
            {
                CurrentUser.UserId   = user.UserId;
                CurrentUser.Username = user.Username;
                CurrentUser.Role     = user.Role;

                if (string.Equals(user.Role, "Parent", StringComparison.OrdinalIgnoreCase))
                    new ParentPortal().Show();
                else
                    new Dashboard().Show();

                Hide();
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Login Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            new RegisterForm().ShowDialog();
        }
    }
}
