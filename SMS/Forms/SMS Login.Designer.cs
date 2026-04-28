namespace SMS
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            loginCard   = new MaterialSkin.Controls.MaterialCard();
            btnLogin    = new MaterialSkin.Controls.MaterialButton();
            btnRegister = new MaterialSkin.Controls.MaterialButton();
            txtPassword = new MaterialSkin.Controls.MaterialTextBox2();
            txtUsername = new MaterialSkin.Controls.MaterialTextBox2();
            lblSubtitle = new Label();
            lblTitle    = new Label();
            picLogo     = new PictureBox();
            loginCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picLogo).BeginInit();
            SuspendLayout();

            // ── loginCard ────────────────────────────────────────────────────
            loginCard.Anchor    = AnchorStyles.None;
            loginCard.BackColor = System.Drawing.Color.FromArgb(28, 30, 50);
            loginCard.Controls.Add(btnLogin);
            loginCard.Controls.Add(btnRegister);
            loginCard.Controls.Add(txtPassword);
            loginCard.Controls.Add(txtUsername);
            loginCard.Controls.Add(lblSubtitle);
            loginCard.Controls.Add(lblTitle);
            loginCard.Controls.Add(picLogo);
            loginCard.Depth      = 0;
            loginCard.ForeColor  = System.Drawing.Color.White;
            loginCard.Location   = new System.Drawing.Point(40, 90);
            loginCard.Margin     = new Padding(14);
            loginCard.MouseState = MaterialSkin.MouseState.HOVER;
            loginCard.Name       = "loginCard";
            loginCard.Padding    = new Padding(20);
            loginCard.Size       = new System.Drawing.Size(600, 520);
            loginCard.TabIndex   = 0;

            // ── picLogo ──────────────────────────────────────────────────────
            picLogo.Location = new System.Drawing.Point(235, 30);
            picLogo.Name     = "picLogo";
            picLogo.Size     = new System.Drawing.Size(130, 110);
            picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            picLogo.TabIndex = 5;
            picLogo.TabStop  = false;

            // ── lblTitle ─────────────────────────────────────────────────────
            lblTitle.BackColor  = System.Drawing.Color.Transparent;
            lblTitle.Font       = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor  = System.Drawing.Color.White;
            lblTitle.Location   = new System.Drawing.Point(20, 155);
            lblTitle.Name       = "lblTitle";
            lblTitle.Size       = new System.Drawing.Size(560, 38);
            lblTitle.TabIndex   = 0;
            lblTitle.Text       = "Student Management System  ·  نظام إدارة الطلاب";
            lblTitle.TextAlign  = System.Drawing.ContentAlignment.MiddleCenter;

            // ── lblSubtitle ──────────────────────────────────────────────────
            lblSubtitle.BackColor  = System.Drawing.Color.Transparent;
            lblSubtitle.Font       = new System.Drawing.Font("Segoe UI", 10F);
            lblSubtitle.ForeColor  = System.Drawing.Color.FromArgb(160, 170, 210);
            lblSubtitle.Location   = new System.Drawing.Point(20, 196);
            lblSubtitle.Name       = "lblSubtitle";
            lblSubtitle.Size       = new System.Drawing.Size(560, 30);
            lblSubtitle.TabIndex   = 1;
            lblSubtitle.Text       = "Sign in to your account  |  تسجيل الدخول إلى حسابك";
            lblSubtitle.TextAlign  = System.Drawing.ContentAlignment.MiddleCenter;

            // ── txtUsername ──────────────────────────────────────────────────
            txtUsername.AnimateReadOnly         = false;
            txtUsername.BackgroundImageLayout   = ImageLayout.None;
            txtUsername.CharacterCasing         = CharacterCasing.Normal;
            txtUsername.Depth                   = 0;
            txtUsername.Font                    = new System.Drawing.Font("Segoe UI", 11F);
            txtUsername.HideSelection           = true;
            txtUsername.Hint                    = "Username (e.g. ahmed1)  |  اسم المستخدم";
            txtUsername.Location                = new System.Drawing.Point(20, 246);
            txtUsername.MaxLength               = 50;
            txtUsername.MouseState              = MaterialSkin.MouseState.OUT;
            txtUsername.Name                    = "txtUsername";
            txtUsername.PasswordChar            = '\0';
            txtUsername.Size                    = new System.Drawing.Size(560, 48);
            txtUsername.TabIndex                = 2;
            txtUsername.UseSystemPasswordChar   = false;

            // ── txtPassword ──────────────────────────────────────────────────
            txtPassword.AnimateReadOnly         = false;
            txtPassword.BackgroundImageLayout   = ImageLayout.None;
            txtPassword.CharacterCasing         = CharacterCasing.Normal;
            txtPassword.Depth                   = 0;
            txtPassword.Font                    = new System.Drawing.Font("Segoe UI", 11F);
            txtPassword.HideSelection           = true;
            txtPassword.Hint                    = "Password (letters + numbers)  |  كلمة المرور";
            txtPassword.Location                = new System.Drawing.Point(20, 316);
            txtPassword.MaxLength               = 50;
            txtPassword.MouseState              = MaterialSkin.MouseState.OUT;
            txtPassword.Name                    = "txtPassword";
            txtPassword.PasswordChar            = '●';
            txtPassword.Size                    = new System.Drawing.Size(560, 48);
            txtPassword.TabIndex                = 3;
            txtPassword.UseSystemPasswordChar   = true;

            // ── btnLogin ─────────────────────────────────────────────────────
            btnLogin.AutoSize              = false;
            btnLogin.Density              = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            btnLogin.Depth                = 0;
            btnLogin.HighEmphasis         = true;
            btnLogin.Location             = new System.Drawing.Point(20, 392);
            btnLogin.Margin               = new Padding(0, 10, 0, 6);
            btnLogin.MouseState           = MaterialSkin.MouseState.HOVER;
            btnLogin.Name                 = "btnLogin";
            btnLogin.Size                 = new System.Drawing.Size(560, 48);
            btnLogin.TabIndex             = 4;
            btnLogin.Text                 = "SIGN IN  ·  دخول";
            btnLogin.Type                 = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            btnLogin.UseVisualStyleBackColor = true;
            btnLogin.Click               += btnLogin_Click;

            // ── btnRegister ──────────────────────────────────────────────────
            btnRegister.AutoSize              = false;
            btnRegister.Density              = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            btnRegister.Depth                = 0;
            btnRegister.HighEmphasis         = false;
            btnRegister.Location             = new System.Drawing.Point(20, 452);
            btnRegister.Margin               = new Padding(0, 6, 0, 0);
            btnRegister.MouseState           = MaterialSkin.MouseState.HOVER;
            btnRegister.Name                 = "btnRegister";
            btnRegister.Size                 = new System.Drawing.Size(560, 44);
            btnRegister.TabIndex             = 5;
            btnRegister.Text                 = "CREATE NEW ACCOUNT  ·  إنشاء حساب جديد";
            btnRegister.Type                 = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined;
            btnRegister.UseVisualStyleBackColor = true;
            btnRegister.Click               += btnRegister_Click;

            // ── LoginForm ────────────────────────────────────────────────────
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode       = AutoScaleMode.Font;
            ClientSize          = new System.Drawing.Size(680, 700);
            Controls.Add(loginCard);
            MinimumSize         = new System.Drawing.Size(680, 700);
            MaximumSize         = new System.Drawing.Size(680, 700);
            Name                = "LoginForm";
            Padding             = new Padding(4, 100, 4, 4);
            StartPosition       = FormStartPosition.CenterScreen;
            Text                = "SMS  —  Login";

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
        private MaterialSkin.Controls.MaterialButton    btnRegister;
    }
}