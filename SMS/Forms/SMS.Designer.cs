namespace SMS
{
    partial class SMS
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            splitContainerMain = new SplitContainer();
            dataGridView1 = new DataGridView();
            materialCard1 = new MaterialSkin.Controls.MaterialCard();
            btnDelete = new MaterialSkin.Controls.MaterialButton();
            btnUpdate = new MaterialSkin.Controls.MaterialButton();
            btnSearch = new MaterialSkin.Controls.MaterialButton();
            txtSearch = new MaterialSkin.Controls.MaterialTextBox2();
            btnAdd = new MaterialSkin.Controls.MaterialButton();
            txtDepartment = new MaterialSkin.Controls.MaterialTextBox2();
            txtAge = new MaterialSkin.Controls.MaterialTextBox2();
            txtName = new MaterialSkin.Controls.MaterialTextBox2();
            txtId = new MaterialSkin.Controls.MaterialTextBox2();
            materialLabelSubtitle = new Label();
            materialLabel1 = new Label();
            ((System.ComponentModel.ISupportInitialize)splitContainerMain).BeginInit();
            splitContainerMain.Panel1.SuspendLayout();
            splitContainerMain.Panel2.SuspendLayout();
            splitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            materialCard1.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainerMain
            // 
            splitContainerMain.Dock = DockStyle.Fill;
            splitContainerMain.Location = new Point(4, 107);
            splitContainerMain.Margin = new Padding(4, 5, 4, 5);
            splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            splitContainerMain.Panel1.Controls.Add(dataGridView1);
            splitContainerMain.Panel1MinSize = 400;
            // 
            // splitContainerMain.Panel2
            // 
            splitContainerMain.Panel2.Controls.Add(materialCard1);
            splitContainerMain.Panel2MinSize = 320;
            splitContainerMain.Size = new Size(1706, 994);
            splitContainerMain.SplitterDistance = 1037;
            splitContainerMain.SplitterWidth = 6;
            splitContainerMain.TabIndex = 0;
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.Location = new Point(0, 0);
            dataGridView1.Margin = new Padding(4, 5, 4, 5);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.RowHeadersWidth = 62;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Size = new Size(1037, 994);
            dataGridView1.TabIndex = 0;
            dataGridView1.CellClick += dataGridView1_CellClick;
            // 
            // materialCard1
            // 
            materialCard1.AutoScroll = true;
            materialCard1.BackColor = Color.FromArgb(255, 255, 255);
            materialCard1.Controls.Add(btnDelete);
            materialCard1.Controls.Add(btnUpdate);
            materialCard1.Controls.Add(btnSearch);
            materialCard1.Controls.Add(txtSearch);
            materialCard1.Controls.Add(btnAdd);
            materialCard1.Controls.Add(txtDepartment);
            materialCard1.Controls.Add(txtAge);
            materialCard1.Controls.Add(txtName);
            materialCard1.Controls.Add(txtId);
            materialCard1.Controls.Add(materialLabelSubtitle);
            materialCard1.Controls.Add(materialLabel1);
            materialCard1.Depth = 0;
            materialCard1.Dock = DockStyle.Fill;
            materialCard1.ForeColor = Color.FromArgb(222, 0, 0, 0);
            materialCard1.Location = new Point(0, 0);
            materialCard1.Margin = new Padding(20, 23, 20, 23);
            materialCard1.MouseState = MaterialSkin.MouseState.HOVER;
            materialCard1.Name = "materialCard1";
            materialCard1.Padding = new Padding(14, 17, 14, 17);
            materialCard1.Size = new Size(663, 994);
            materialCard1.TabIndex = 0;
            // 
            // btnDelete
            // 
            btnDelete.AutoSize = false;
            btnDelete.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnDelete.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            btnDelete.Depth = 0;
            btnDelete.HighEmphasis = false;
            btnDelete.Icon = null;
            btnDelete.Location = new Point(337, 873);
            btnDelete.Margin = new Padding(6, 10, 6, 10);
            btnDelete.MouseState = MaterialSkin.MouseState.HOVER;
            btnDelete.Name = "btnDelete";
            btnDelete.NoAccentTextColor = Color.Empty;
            btnDelete.Size = new Size(129, 67);
            btnDelete.TabIndex = 10;
            btnDelete.Text = "DELETE";
            btnDelete.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined;
            btnDelete.UseAccentColor = false;
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            // 
            // btnUpdate
            // 
            btnUpdate.AutoSize = false;
            btnUpdate.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnUpdate.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            btnUpdate.Depth = 0;
            btnUpdate.HighEmphasis = false;
            btnUpdate.Icon = null;
            btnUpdate.Location = new Point(189, 873);
            btnUpdate.Margin = new Padding(6, 10, 6, 10);
            btnUpdate.MouseState = MaterialSkin.MouseState.HOVER;
            btnUpdate.Name = "btnUpdate";
            btnUpdate.NoAccentTextColor = Color.Empty;
            btnUpdate.Size = new Size(129, 67);
            btnUpdate.TabIndex = 9;
            btnUpdate.Text = "UPDATE";
            btnUpdate.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined;
            btnUpdate.UseAccentColor = false;
            btnUpdate.UseVisualStyleBackColor = true;
            btnUpdate.Click += btnUpdate_Click;
            // 
            // btnSearch
            // 
            btnSearch.AutoSize = false;
            btnSearch.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnSearch.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            btnSearch.Depth = 0;
            btnSearch.HighEmphasis = false;
            btnSearch.Icon = null;
            btnSearch.Location = new Point(40, 873);
            btnSearch.Margin = new Padding(6, 10, 6, 10);
            btnSearch.MouseState = MaterialSkin.MouseState.HOVER;
            btnSearch.Name = "btnSearch";
            btnSearch.NoAccentTextColor = Color.Empty;
            btnSearch.Size = new Size(129, 67);
            btnSearch.TabIndex = 8;
            btnSearch.Text = "SEARCH";
            btnSearch.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined;
            btnSearch.UseAccentColor = false;
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += btnSearch_Click;
            // 
            // txtSearch
            // 
            txtSearch.AnimateReadOnly = false;
            txtSearch.BackgroundImageLayout = ImageLayout.None;
            txtSearch.CharacterCasing = CharacterCasing.Normal;
            txtSearch.Depth = 0;
            txtSearch.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            txtSearch.HideSelection = true;
            txtSearch.Hint = "Search by name or department";
            txtSearch.LeadingIcon = null;
            txtSearch.Location = new Point(40, 747);
            txtSearch.Margin = new Padding(4, 5, 4, 5);
            txtSearch.MaxLength = 100;
            txtSearch.MouseState = MaterialSkin.MouseState.OUT;
            txtSearch.Name = "txtSearch";
            txtSearch.PasswordChar = '\0';
            txtSearch.PrefixSuffixText = null;
            txtSearch.ReadOnly = false;
            txtSearch.RightToLeft = RightToLeft.No;
            txtSearch.SelectedText = "";
            txtSearch.SelectionLength = 0;
            txtSearch.SelectionStart = 0;
            txtSearch.ShortcutsEnabled = true;
            txtSearch.Size = new Size(429, 48);
            txtSearch.TabIndex = 7;
            txtSearch.TabStop = false;
            txtSearch.TextAlign = HorizontalAlignment.Left;
            txtSearch.TrailingIcon = null;
            txtSearch.UseSystemPasswordChar = false;
            // 
            // btnAdd
            // 
            btnAdd.AutoSize = false;
            btnAdd.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnAdd.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            btnAdd.Depth = 0;
            btnAdd.HighEmphasis = true;
            btnAdd.Icon = null;
            btnAdd.Location = new Point(40, 630);
            btnAdd.Margin = new Padding(6, 10, 6, 10);
            btnAdd.MouseState = MaterialSkin.MouseState.HOVER;
            btnAdd.Name = "btnAdd";
            btnAdd.NoAccentTextColor = Color.Empty;
            btnAdd.Size = new Size(429, 73);
            btnAdd.TabIndex = 6;
            btnAdd.Text = "ADD STUDENT";
            btnAdd.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            btnAdd.UseAccentColor = false;
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // txtDepartment
            // 
            txtDepartment.AnimateReadOnly = false;
            txtDepartment.BackgroundImageLayout = ImageLayout.None;
            txtDepartment.CharacterCasing = CharacterCasing.Normal;
            txtDepartment.Depth = 0;
            txtDepartment.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            txtDepartment.HideSelection = true;
            txtDepartment.Hint = "Department";
            txtDepartment.LeadingIcon = null;
            txtDepartment.Location = new Point(40, 500);
            txtDepartment.Margin = new Padding(4, 5, 4, 5);
            txtDepartment.MaxLength = 100;
            txtDepartment.MouseState = MaterialSkin.MouseState.OUT;
            txtDepartment.Name = "txtDepartment";
            txtDepartment.PasswordChar = '\0';
            txtDepartment.PrefixSuffixText = null;
            txtDepartment.ReadOnly = false;
            txtDepartment.RightToLeft = RightToLeft.No;
            txtDepartment.SelectedText = "";
            txtDepartment.SelectionLength = 0;
            txtDepartment.SelectionStart = 0;
            txtDepartment.ShortcutsEnabled = true;
            txtDepartment.Size = new Size(429, 48);
            txtDepartment.TabIndex = 5;
            txtDepartment.TabStop = false;
            txtDepartment.TextAlign = HorizontalAlignment.Left;
            txtDepartment.TrailingIcon = null;
            txtDepartment.UseSystemPasswordChar = false;
            // 
            // txtAge
            // 
            txtAge.AnimateReadOnly = false;
            txtAge.BackgroundImageLayout = ImageLayout.None;
            txtAge.CharacterCasing = CharacterCasing.Normal;
            txtAge.Depth = 0;
            txtAge.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            txtAge.HideSelection = true;
            txtAge.Hint = "Age";
            txtAge.LeadingIcon = null;
            txtAge.Location = new Point(40, 383);
            txtAge.Margin = new Padding(4, 5, 4, 5);
            txtAge.MaxLength = 3;
            txtAge.MouseState = MaterialSkin.MouseState.OUT;
            txtAge.Name = "txtAge";
            txtAge.PasswordChar = '\0';
            txtAge.PrefixSuffixText = null;
            txtAge.ReadOnly = false;
            txtAge.RightToLeft = RightToLeft.No;
            txtAge.SelectedText = "";
            txtAge.SelectionLength = 0;
            txtAge.SelectionStart = 0;
            txtAge.ShortcutsEnabled = true;
            txtAge.Size = new Size(429, 48);
            txtAge.TabIndex = 4;
            txtAge.TabStop = false;
            txtAge.TextAlign = HorizontalAlignment.Left;
            txtAge.TrailingIcon = null;
            txtAge.UseSystemPasswordChar = false;
            // 
            // txtName
            // 
            txtName.AnimateReadOnly = false;
            txtName.BackgroundImageLayout = ImageLayout.None;
            txtName.CharacterCasing = CharacterCasing.Normal;
            txtName.Depth = 0;
            txtName.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            txtName.HideSelection = true;
            txtName.Hint = "Full Name";
            txtName.LeadingIcon = null;
            txtName.Location = new Point(40, 267);
            txtName.Margin = new Padding(4, 5, 4, 5);
            txtName.MaxLength = 100;
            txtName.MouseState = MaterialSkin.MouseState.OUT;
            txtName.Name = "txtName";
            txtName.PasswordChar = '\0';
            txtName.PrefixSuffixText = null;
            txtName.ReadOnly = false;
            txtName.RightToLeft = RightToLeft.No;
            txtName.SelectedText = "";
            txtName.SelectionLength = 0;
            txtName.SelectionStart = 0;
            txtName.ShortcutsEnabled = true;
            txtName.Size = new Size(429, 48);
            txtName.TabIndex = 3;
            txtName.TabStop = false;
            txtName.TextAlign = HorizontalAlignment.Left;
            txtName.TrailingIcon = null;
            txtName.UseSystemPasswordChar = false;
            // 
            // txtId
            // 
            txtId.AnimateReadOnly = false;
            txtId.BackgroundImageLayout = ImageLayout.None;
            txtId.CharacterCasing = CharacterCasing.Normal;
            txtId.Depth = 0;
            txtId.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            txtId.HideSelection = true;
            txtId.Hint = "ID  —  auto assigned";
            txtId.LeadingIcon = null;
            txtId.Location = new Point(40, 150);
            txtId.Margin = new Padding(4, 5, 4, 5);
            txtId.MaxLength = 10;
            txtId.MouseState = MaterialSkin.MouseState.OUT;
            txtId.Name = "txtId";
            txtId.PasswordChar = '\0';
            txtId.PrefixSuffixText = null;
            txtId.ReadOnly = true;
            txtId.RightToLeft = RightToLeft.No;
            txtId.SelectedText = "";
            txtId.SelectionLength = 0;
            txtId.SelectionStart = 0;
            txtId.ShortcutsEnabled = true;
            txtId.Size = new Size(429, 48);
            txtId.TabIndex = 2;
            txtId.TabStop = false;
            txtId.TextAlign = HorizontalAlignment.Left;
            txtId.TrailingIcon = null;
            txtId.UseSystemPasswordChar = false;
            // 
            // materialLabelSubtitle
            // 
            materialLabelSubtitle.BackColor = Color.Transparent;
            materialLabelSubtitle.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point, 0);
            materialLabelSubtitle.ForeColor = Color.FromArgb(180, 180, 200);
            materialLabelSubtitle.Location = new Point(40, 80);
            materialLabelSubtitle.Margin = new Padding(4, 0, 4, 0);
            materialLabelSubtitle.Name = "materialLabelSubtitle";
            materialLabelSubtitle.Size = new Size(514, 50);
            materialLabelSubtitle.TabIndex = 1;
            materialLabelSubtitle.Text = "Click a row to load, then add, update or delete";
            // 
            // materialLabel1
            // 
            materialLabel1.BackColor = Color.Transparent;
            materialLabel1.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            materialLabel1.ForeColor = Color.White;
            materialLabel1.Location = new Point(40, 23);
            materialLabel1.Margin = new Padding(4, 0, 4, 0);
            materialLabel1.Name = "materialLabel1";
            materialLabel1.Size = new Size(429, 57);
            materialLabel1.TabIndex = 0;
            materialLabel1.Text = "Student Details";
            // 
            // SMS
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1714, 1106);
            Controls.Add(splitContainerMain);
            Margin = new Padding(4, 5, 4, 5);
            Name = "SMS";
            Padding = new Padding(4, 107, 4, 5);
            Text = "Student Management System";
            splitContainerMain.Panel1.ResumeLayout(false);
            splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerMain).EndInit();
            splitContainerMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            materialCard1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.SplitContainer         splitContainerMain;
        private System.Windows.Forms.DataGridView           dataGridView1;
        private MaterialSkin.Controls.MaterialCard          materialCard1;
        private System.Windows.Forms.Label                  materialLabel1;
        private System.Windows.Forms.Label                  materialLabelSubtitle;
        private MaterialSkin.Controls.MaterialTextBox2      txtId;
        private MaterialSkin.Controls.MaterialTextBox2      txtName;
        private MaterialSkin.Controls.MaterialTextBox2      txtAge;
        private MaterialSkin.Controls.MaterialTextBox2      txtDepartment;
        private MaterialSkin.Controls.MaterialButton        btnAdd;
        private MaterialSkin.Controls.MaterialTextBox2      txtSearch;
        private MaterialSkin.Controls.MaterialButton        btnSearch;
        private MaterialSkin.Controls.MaterialButton        btnUpdate;
        private MaterialSkin.Controls.MaterialButton        btnDelete;
    }
}
