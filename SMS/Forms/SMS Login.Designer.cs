namespace SMS
{
    partial class LoginForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            loginCard = new MaterialSkin.Controls.MaterialCard();
            btnLogin = new MaterialSkin.Controls.MaterialButton();
            txtPassword = new MaterialSkin.Controls.MaterialTextBox2();
            txtUsername = new MaterialSkin.Controls.MaterialTextBox2();
            lblSubtitle = new Label();
            lblTitle = new Label();
            picLogo = new PictureBox();
            loginCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picLogo).BeginInit();
            SuspendLayout();
            // 
            // loginCard
            // 
            loginCard.Anchor = AnchorStyles.None;
            loginCard.BackColor = Color.FromArgb(255, 255, 255);
            loginCard.Controls.Add(btnLogin);
            loginCard.Controls.Add(txtPassword);
            loginCard.Controls.Add(txtUsername);
            loginCard.Controls.Add(lblSubtitle);
            loginCard.Controls.Add(lblTitle);
            loginCard.Controls.Add(picLogo);
            loginCard.Depth = 0;
            loginCard.ForeColor = Color.FromArgb(222, 0, 0, 0);
            loginCard.Location = new Point(24, 161);
            loginCard.Margin = new Padding(20, 23, 20, 23);
            loginCard.MouseState = MaterialSkin.MouseState.HOVER;
            loginCard.Name = "loginCard";
            loginCard.Padding = new Padding(20, 23, 20, 23);
            loginCard.Size = new Size(571, 885);
            loginCard.TabIndex = 0;
            // 
            // btnLogin
            // 
            btnLogin.AutoSize = false;
            btnLogin.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnLogin.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            btnLogin.Depth = 0;
            btnLogin.HighEmphasis = true;
            btnLogin.Icon = null;
            btnLogin.Location = new Point(29, 660);
            btnLogin.Margin = new Padding(6, 10, 6, 10);
            btnLogin.MouseState = MaterialSkin.MouseState.HOVER;
            btnLogin.Name = "btnLogin";
            btnLogin.NoAccentTextColor = Color.Empty;
            btnLogin.Size = new Size(514, 80);
            btnLogin.TabIndex = 4;
            btnLogin.Text = "SIGN IN";
            btnLogin.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            btnLogin.UseAccentColor = false;
            btnLogin.UseVisualStyleBackColor = true;
            btnLogin.Click += btnLogin_Click;
            // 
            // txtPassword
            // 
            txtPassword.AnimateReadOnly = false;
            txtPassword.BackgroundImageLayout = ImageLayout.None;
            txtPassword.CharacterCasing = CharacterCasing.Normal;
            txtPassword.Depth = 0;
            txtPassword.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            txtPassword.HideSelection = true;
            txtPassword.Hint = "Password";
            txtPassword.LeadingIcon = null;
            txtPassword.Location = new Point(29, 523);
            txtPassword.Margin = new Padding(4, 5, 4, 5);
            txtPassword.MaxLength = 50;
            txtPassword.MouseState = MaterialSkin.MouseState.OUT;
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '●';
            txtPassword.PrefixSuffixText = null;
            txtPassword.ReadOnly = false;
            txtPassword.RightToLeft = RightToLeft.No;
            txtPassword.SelectedText = "";
            txtPassword.SelectionLength = 0;
            txtPassword.SelectionStart = 0;
            txtPassword.ShortcutsEnabled = true;
            txtPassword.Size = new Size(514, 48);
            txtPassword.TabIndex = 3;
            txtPassword.TabStop = false;
            txtPassword.TextAlign = HorizontalAlignment.Left;
            txtPassword.TrailingIcon = null;
            txtPassword.UseSystemPasswordChar = true;
            // 
            // txtUsername
            // 
            txtUsername.AnimateReadOnly = false;
            txtUsername.BackgroundImageLayout = ImageLayout.None;
            txtUsername.CharacterCasing = CharacterCasing.Normal;
            txtUsername.Depth = 0;
            txtUsername.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            txtUsername.HideSelection = true;
            txtUsername.Hint = "Username";
            txtUsername.LeadingIcon = null;
            txtUsername.Location = new Point(29, 397);
            txtUsername.Margin = new Padding(4, 5, 4, 5);
            txtUsername.MaxLength = 50;
            txtUsername.MouseState = MaterialSkin.MouseState.OUT;
            txtUsername.Name = "txtUsername";
            txtUsername.PasswordChar = '\0';
            txtUsername.PrefixSuffixText = null;
            txtUsername.ReadOnly = false;
            txtUsername.RightToLeft = RightToLeft.No;
            txtUsername.SelectedText = "";
            txtUsername.SelectionLength = 0;
            txtUsername.SelectionStart = 0;
            txtUsername.ShortcutsEnabled = true;
            txtUsername.Size = new Size(514, 48);
            txtUsername.TabIndex = 2;
            txtUsername.TabStop = false;
            txtUsername.TextAlign = HorizontalAlignment.Left;
            txtUsername.TrailingIcon = null;
            txtUsername.UseSystemPasswordChar = false;
            // 
            // lblSubtitle
            // 
            lblSubtitle.BackColor = Color.Transparent;
            lblSubtitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblSubtitle.ForeColor = Color.FromArgb(180, 180, 200);
            lblSubtitle.Location = new Point(29, 330);
            lblSubtitle.Margin = new Padding(4, 0, 4, 0);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(514, 51);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Sign in to your account";
            lblSubtitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblTitle
            // 
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("Segoe UI", 17F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(29, 280);
            lblTitle.Margin = new Padding(4, 0, 4, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(514, 50);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Student Management System";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // picLogo
            // 
            picLogo.Location = new Point(186, 88);
            picLogo.Margin = new Padding(4, 5, 4, 5);
            picLogo.Name = "picLogo";
            picLogo.Size = new Size(200, 175);
            picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            picLogo.TabIndex = 5;
            picLogo.TabStop = false;
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(743, 1106);
            Controls.Add(loginCard);
            Margin = new Padding(4, 5, 4, 5);
            Name = "LoginForm";
            Padding = new Padding(4, 107, 4, 5);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "SMS  —  Login";
            loginCard.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picLogo).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private MaterialSkin.Controls.MaterialCard      loginCard;
        private System.Windows.Forms.Label              lblTitle;
        private System.Windows.Forms.Label              lblSubtitle;
        private System.Windows.Forms.PictureBox         picLogo;
        private MaterialSkin.Controls.MaterialTextBox2  txtUsername;
        private MaterialSkin.Controls.MaterialTextBox2  txtPassword;
        private MaterialSkin.Controls.MaterialButton    btnLogin;
    }
}