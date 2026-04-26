using MaterialSkin;
using MaterialSkin.Controls;
using SMS.Data;
using SMS.Models;

namespace SMS
{
    public partial class LoginForm : MaterialForm
    {
        private readonly AppDbContext _context = new();

        public LoginForm()
        {
            InitializeComponent();

            // ── MaterialSkin setup ────────────────────────────────────────────────
            var skin = MaterialSkinManager.Instance;
            skin.AddFormToManage(this);
            skin.Theme = MaterialSkinManager.Themes.DARK;
            skin.ColorScheme = new ColorScheme(
                Primary.Indigo800,
                Primary.Indigo900,
                Primary.Indigo500,
                Accent.Teal200,
                TextShade.WHITE);

            // ── Font overrides — set AFTER MaterialSkin so its theme-change ───────
            // callbacks can't overwrite them.
            ApplyFonts();

            // ── Load logo then apply circular rendering ───────────────────────────
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

            // ── Keep card centred ────────────────────────────────────────────────
            CentreCard();
            Resize += (_, _) => CentreCard();

            // Re-apply fonts after the form is fully shown (last line of defence)
            Shown += (_, _) => ApplyFonts();
        }

        /// <summary>Applies heading/subtitle fonts — called after MaterialSkin to prevent overrides.</summary>
        private void ApplyFonts()
        {
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Bold);
            lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 14F);
            lblSubtitle.Height = 44;
        }

        private void CentreCard()
        {
            loginCard.Location = new System.Drawing.Point(
                (ClientSize.Width - loginCard.Width) / 2,
                (ClientSize.Height - loginCard.Height) / 2);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (username == "" || password == "")
            {
                MessageBox.Show("Please enter username and password.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var db = new AppDbContext();
            var user = db.Users
                .FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                var dashboard = new Dashboard();
                dashboard.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Login Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    }
}
