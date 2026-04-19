using MaterialSkin;
using MaterialSkin.Controls;
using SMS.Models;
using SMS.Services;
using System.Drawing;

namespace SMS
{
    public partial class SMS : MaterialForm
    {
        private readonly StudentService _service = new();
        private System.Windows.Forms.PictureBox _picLogo = new();

        public SMS()
        {
            InitializeComponent();

            var skin = MaterialSkinManager.Instance;
            skin.AddFormToManage(this);
            skin.Theme = MaterialSkinManager.Themes.DARK;
            skin.ColorScheme = new ColorScheme(
                Primary.Indigo800,
                Primary.Indigo900,
                Primary.Indigo500,
                Accent.Teal200,
                TextShade.WHITE);

            // Set heading fonts AFTER MaterialSkin (it resets fonts on theme apply)
            ApplyFonts();

            StyleDataGrid();

            // ── App icon ────────────────────────────────────────────────────────
            try
            {
                var icoPath = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "Resources", "app.ico");
                if (System.IO.File.Exists(icoPath))
                    Icon = new System.Drawing.Icon(icoPath);
            }
            catch { }

            // ── Logo in editor card ──────────────────────────────────────────────
            _picLogo = new System.Windows.Forms.PictureBox
            {
                Size     = new Size(72, 72),
                SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom,
                TabStop  = false
            };
            try
            {
                var logoPath = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "Resources", "sms_icon.png");
                if (System.IO.File.Exists(logoPath))
                    _picLogo.Image = Image.FromFile(logoPath);
            }
            catch { }
            materialCard1.Controls.Add(_picLogo);
            _picLogo.BringToFront();
            UIHelper.MakeCircular(_picLogo);   // rounded edges

            // Label positions are computed inside LayoutEditorColumn — no hard-coded Tops here

            materialCard1.SizeChanged += (_, _) => LayoutEditorColumn();
            Shown += (_, _) =>
            {
                ApplyFonts();   // re-apply after all rendering — last defence
                LayoutEditorColumn();
                if (splitContainerMain.Width > 0)
                    splitContainerMain.SplitterDistance = Math.Clamp(
                        (int)(splitContainerMain.Width * 0.60),
                        splitContainerMain.Panel1MinSize,
                        splitContainerMain.Width - splitContainerMain.SplitterWidth - splitContainerMain.Panel2MinSize);
            };

            LoadData();
        }

        // ── Heading font overrides (runs after MaterialSkin to avoid being reset) ─
        private void ApplyFonts()
        {
            materialLabel1.Font         = new Font("Segoe UI", 20F, FontStyle.Bold);
            materialLabelSubtitle.Font  = new Font("Segoe UI", 13F);
            // Heights: let the label auto-size vertically so nothing gets clipped
            materialLabel1.AutoSize       = false;
            materialLabel1.Height         = materialLabel1.Font.Height + 10;   // ~40 px
            materialLabelSubtitle.AutoSize= false;
            materialLabelSubtitle.Height  = materialLabelSubtitle.Font.Height + 8; // ~28 px
        }

        // ── DataGrid dark-theme styling ──────────────────────────────────────────
        private void StyleDataGrid()
        {
            var indigoHeader = Color.FromArgb(48, 63, 159);   // Indigo 800
            var darkBg       = Color.FromArgb(33, 33, 33);
            var darkAltBg    = Color.FromArgb(42, 42, 55);
            var fgWhite      = Color.White;
            var selBlue      = Color.FromArgb(63, 81, 181);   // Indigo 500
            var borderColor  = Color.FromArgb(60, 60, 80);

            dataGridView1.BackgroundColor = darkBg;
            dataGridView1.GridColor       = borderColor;

            // Column headers
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor  = indigoHeader;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor  = fgWhite;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font       = new Font("Segoe UI", 11F, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.Padding    = new Padding(12, 0, 0, 0);
            dataGridView1.ColumnHeadersDefaultCellStyle.SelectionBackColor = indigoHeader;
            dataGridView1.ColumnHeadersDefaultCellStyle.SelectionForeColor = fgWhite;

            // Regular rows
            dataGridView1.DefaultCellStyle.BackColor         = darkBg;
            dataGridView1.DefaultCellStyle.ForeColor         = fgWhite;
            dataGridView1.DefaultCellStyle.SelectionBackColor= selBlue;
            dataGridView1.DefaultCellStyle.SelectionForeColor= fgWhite;
            dataGridView1.DefaultCellStyle.Font              = new Font("Segoe UI", 11F);
            dataGridView1.DefaultCellStyle.Padding           = new Padding(12, 4, 0, 4);

            // Alternating rows
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor          = darkAltBg;
            dataGridView1.AlternatingRowsDefaultCellStyle.ForeColor          = fgWhite;
            dataGridView1.AlternatingRowsDefaultCellStyle.SelectionBackColor = selBlue;
            dataGridView1.AlternatingRowsDefaultCellStyle.SelectionForeColor = fgWhite;

            // Row height
            dataGridView1.RowTemplate.Height = 44;
        }

        private void LayoutEditorColumn()
        {
            int pad    = 28;
            int usable = Math.Max(260, materialCard1.ClientSize.Width - pad * 2);
            int x      = (materialCard1.ClientSize.Width - usable) / 2;

            // ── Logo: centred horizontally at top ─────────────────────────────────
            _picLogo.Left = (materialCard1.ClientSize.Width - _picLogo.Width) / 2;
            _picLogo.Top  = 16;
            int y = _picLogo.Bottom + 14;   // gap below logo

            // ── Heading & subtitle ────────────────────────────────────────────────
            materialLabel1.Left   = x;
            materialLabel1.Width  = usable;
            materialLabel1.Top    = y;
            y = materialLabel1.Bottom + 6;  // tight but distinct gap

            materialLabelSubtitle.Left  = x;
            materialLabelSubtitle.Width = usable;
            materialLabelSubtitle.Top   = y;
            y = materialLabelSubtitle.Bottom + 22; // generous gap before inputs

            // ── Text boxes (width + left set here too) ────────────────────────────
            foreach (Control c in materialCard1.Controls)
                if (c is MaterialSkin.Controls.MaterialTextBox2 tb)
                    tb.Width = usable;

            txtId.Left  = x; txtId.Top  = y; y = txtId.Bottom + 12;
            txtName.Left = x; txtName.Top = y; y = txtName.Bottom + 12;
            txtAge.Left  = x; txtAge.Top  = y; y = txtAge.Bottom + 12;
            txtDepartment.Left = x; txtDepartment.Top = y; y = txtDepartment.Bottom + 24;

            // ── Buttons ───────────────────────────────────────────────────────────
            btnAdd.Left  = x; btnAdd.Width = usable; btnAdd.Top = y;
            y = btnAdd.Bottom + 20;

            txtSearch.Left = x; txtSearch.Width = usable; txtSearch.Top = y;
            y = txtSearch.Bottom + 10;

            int gap = 10;
            int bW  = (usable - gap * 2) / 3;
            btnSearch.Left  = x;                     btnSearch.Width = bW;                         btnSearch.Top = y;
            btnUpdate.Left  = x + bW + gap;          btnUpdate.Width = bW;                         btnUpdate.Top = y;
            btnDelete.Left  = x + 2 * (bW + gap);    btnDelete.Width = x + usable - btnDelete.Left; btnDelete.Top = y;
        }


        // ── Data helpers ─────────────────────────────────────────────────────────
        private void LoadData()
        {
            dataGridView1.DataSource = _service.GetStudents();
        }

        private void ClearFields()
        {
            txtId.Text         = "";
            txtName.Text       = "";
            txtAge.Text        = "";
            txtDepartment.Text = "";
        }

        // ── CRUD handlers ─────────────────────────────────────────────────────────
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter a name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtAge.Text, out _))
            {
                MessageBox.Show("Age must be a valid number.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var student = new Student
                {
                    Name       = txtName.Text.Trim(),
                    Age        = int.Parse(txtAge.Text),
                    Department = txtDepartment.Text.Trim()
                };

                _service.AddStudent(student);
                MessageBox.Show("Student added successfully.", "Student Management", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Student Management", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out var id) || id <= 0)
            {
                MessageBox.Show("Select a student from the table before updating.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtAge.Text, out _))
            {
                MessageBox.Show("Age must be a valid number.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var student = new Student
                {
                    Id         = id,
                    Name       = txtName.Text.Trim(),
                    Age        = int.Parse(txtAge.Text),
                    Department = txtDepartment.Text.Trim()
                };

                _service.UpdateStudent(student);
                MessageBox.Show("Student updated successfully.", "Student Management", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Student Management", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out var id) || id <= 0)
            {
                MessageBox.Show("Select a student from the table before deleting.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Delete this student? This cannot be undone.", "Confirm delete",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                _service.DeleteStudent(id);
                MessageBox.Show("Student removed.", "Student Management", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Student Management", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            var q = txtSearch.Text.Trim();
            try
            {
                dataGridView1.DataSource = string.IsNullOrEmpty(q)
                    ? _service.GetStudents()
                    : _service.Search(q);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Student Management", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            var row = dataGridView1.Rows[e.RowIndex];
            txtId.Text         = row.Cells["Id"]?.Value?.ToString()         ?? "";
            txtName.Text       = row.Cells["Name"]?.Value?.ToString()       ?? "";
            txtAge.Text        = row.Cells["Age"]?.Value?.ToString()        ?? "";
            txtDepartment.Text = row.Cells["Department"]?.Value?.ToString() ?? "";
        }
    }
}
