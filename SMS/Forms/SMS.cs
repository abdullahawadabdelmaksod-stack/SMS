using MaterialSkin;
using MaterialSkin.Controls;
using Microsoft.EntityFrameworkCore;
using SMS.Data;
using SMS.Models;
using SMS.Services;
using System.Drawing;

namespace SMS
{
    public partial class SMS : MaterialForm
    {
        // ── Services ─────────────────────────────────────────────────────────
        private readonly StudentService _studentService = new();
        private System.Windows.Forms.PictureBox _logoPictureBox = new();

        // ── Left-panel TabControl & extra DataGridViews ───────────────────────
        private TabControl _tabMain = new();
        private DataGridView _dgvCourses = new();
        private DataGridView _dgvGrades = new();
        private DataGridView _dgvAttendances = new();

        // ── Dynamic Student Controls ─────────────────────────────────────────
        private MaterialTextBox2 txtPhone = new(), txtBirthDate = new();
        private MaterialComboBox txtLevel = new();

        // ── Course editor controls ────────────────────────────────────────────
        private MaterialTextBox2 _txtCourseId = new(), _txtCourseName = new(),
            _txtCourseCode = new(), _txtCourseDesc = new(), _txtCourseCredits = new(),
            _txtCourseSearch = new();
        private MaterialButton _btnCourseAdd = new(), _btnCourseUpdate = new(),
            _btnCourseDelete = new(), _btnCourseSearch = new();
        private Panel _pnlCourse = new();

        // ── Grade editor controls ─────────────────────────────────────────────
        private MaterialTextBox2 _txtGradeId = new(), _txtGradeStudentId = new(),
            _txtGradeCourseId = new(), _txtGradeScore = new(), _txtGradeLetter = new(),
            _txtGradeSemester = new(), _txtGradeSearch = new();
        private MaterialButton _btnGradeAdd = new(), _btnGradeUpdate = new(),
            _btnGradeDelete = new(), _btnGradeSearch = new(), _btnGradeReturn = new();
        private Panel _pnlGrade = new();

        // ── Attendance editor controls ────────────────────────────────────────
        private MaterialTextBox2 _txtAttId = new(), _txtAttStudentId = new(),
            _txtAttCourseId = new(), _txtAttDate = new(), _txtAttNotes = new(),
            _txtAttSearch = new(), _txtAttCourseSearch = new();
        private CheckBox _chkAttPresent = new();
        private MaterialButton _btnAttAdd = new(), _btnAttUpdate = new(),
            _btnAttDelete = new(), _btnAttSearch = new();
        private Panel _pnlAttendance = new();

        // ── Student quick-nav buttons (link to Grades / Attendance tabs) ──────
        private MaterialButton _btnViewGrades = new();
        private MaterialButton _btnViewAttendance = new();

        // ─────────────────────────────────────────────────────────────────────
        //  Constructor
        // ─────────────────────────────────────────────────────────────────────
        private int _defaultTabIndex;

        public SMS(int defaultTabIndex = 0)
        {
            _defaultTabIndex = defaultTabIndex;
            InitializeComponent();
            _txtGradeId.ReadOnly = true; // Grade ID is auto-generated
            _txtGradeId.Text = "(Auto Assigned)";
            // MaterialSkin
            var skin = MaterialSkinManager.Instance;
            skin.AddFormToManage(this);
            skin.Theme = MaterialSkinManager.Themes.DARK;
            skin.ColorScheme = new ColorScheme(
                Primary.Indigo800, Primary.Indigo900, Primary.Indigo500,
                Accent.Teal200, TextShade.WHITE);

            ApplyFonts();
            StyleDgv(dataGridView1);

            // App icon
            try
            {
                var p = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "app.ico");
                if (System.IO.File.Exists(p)) Icon = new System.Drawing.Icon(p);
            }
            catch { }

            // Logo
            _logoPictureBox = new System.Windows.Forms.PictureBox
            { Size = new Size(72, 72), SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom, TabStop = false };
            try
            {
                var p = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "sms_icon.png");
                if (System.IO.File.Exists(p)) _logoPictureBox.Image = Image.FromFile(p);
            }
            catch { }
            materialCard1.Controls.Add(_logoPictureBox);
            _logoPictureBox.BringToFront();
            UIHelper.MakeCircular(_logoPictureBox);

            // Initialize dynamic student controls
            txtPhone.Hint = "Phone";
            txtBirthDate.Hint = "Birth Date (YYYY-MM-DD)";
            txtAge.Hint = "Age (Auto-calculated)";
            txtAge.ReadOnly = true;
            txtLevel.Hint = "Level";
            txtLevel.Items.AddRange(new object[] { "Freshman", "Sophomore", "Junior", "Senior" });
            materialCard1.Controls.AddRange(new Control[] { txtPhone, txtBirthDate, txtLevel });

            // Auto-calculate Age as user types Birth Date
            txtBirthDate.TextChanged += (_, _) =>
            {
                if (DateOnly.TryParse(txtBirthDate.Text, out var dob))
                {
                    int age = DateTime.Today.Year - dob.Year;
                    if (dob > DateOnly.FromDateTime(DateTime.Today.AddYears(-age))) age--;
                    txtAge.Text = age.ToString();
                }
                else
                {
                    txtAge.Text = "";
                }
            };

            // Build left-panel tabs
            InitTabControl();

            // Build right-panel entity editors
            InitCourseEditor();
            InitGradeEditor();
            InitAttendanceEditor();

            // Enforce color rules
            ConfigBtn(btnSearch, "SEARCH", btnSearch_Click);
            ConfigBtn(btnAdd, "ADD STUDENT", btnAdd_Click);
            ConfigBtn(btnUpdate, "UPDATE", btnUpdate_Click);
            ConfigBtn(btnDelete, "DELETE", btnDelete_Click);
            btnAdd.HighEmphasis = true;

            // Update search hint
            txtSearch.Hint = "Search by ID, name, or phone...";

            // ── Quick-nav buttons in student editor ──────────────────────────
            _btnViewGrades.Text = "VIEW GRADES →";
            _btnViewGrades.AutoSize = false;
            _btnViewGrades.HighEmphasis = false;
            _btnViewGrades.Visible = true;
            _btnViewGrades.Click += (_, _) => { _tabMain.SelectedIndex = 2; };

            _btnViewAttendance.Text = "VIEW ATTENDANCE →";
            _btnViewAttendance.AutoSize = false;
            _btnViewAttendance.HighEmphasis = false;
            _btnViewAttendance.Visible = true;
            _btnViewAttendance.Click += (_, _) => { _tabMain.SelectedIndex = 3; };

            materialCard1.Controls.Add(_btnViewGrades);
            materialCard1.Controls.Add(_btnViewAttendance);

            // Events
            materialCard1.SizeChanged += (_, _) => LayoutEditorColumn();
            _tabMain.SelectedIndexChanged += (_, _) => SetActiveTab(_tabMain.SelectedIndex);

            Shown += (_, _) =>
            {
                ApplyFonts();
                LayoutEditorColumn();
                _tabMain.SelectedIndex = _defaultTabIndex;
                SetActiveTab(_defaultTabIndex);
                if (splitContainerMain.Width > 0)
                    splitContainerMain.SplitterDistance = Math.Clamp(
                        (int)(splitContainerMain.Width * 0.60),
                        splitContainerMain.Panel1MinSize,
                        splitContainerMain.Width - splitContainerMain.SplitterWidth - splitContainerMain.Panel2MinSize);
            };

            LoadStudents();
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Left-panel tab control
        // ─────────────────────────────────────────────────────────────────────
        private void InitTabControl()
        {
            var dark = Color.FromArgb(28, 28, 35);

            _tabMain.Dock = DockStyle.Fill;
            _tabMain.DrawMode = TabDrawMode.OwnerDrawFixed;
            _tabMain.SizeMode = TabSizeMode.Fixed;
            _tabMain.ItemSize = new Size(0, 38);
            _tabMain.Padding = new Point(18, 0);
            _tabMain.BackColor = dark;
            _tabMain.DrawItem += TabMain_DrawItem;

            // Students tab — move existing DGV into it
            var tStudents = new TabPage("  Students  ") { BackColor = dark };
            splitContainerMain.Panel1.Controls.Remove(dataGridView1);
            dataGridView1.Dock = DockStyle.Fill;
            tStudents.Controls.Add(dataGridView1);

            // Courses tab
            var tCourses = new TabPage("  Courses  ") { BackColor = dark };
            _dgvCourses = MakeDgv();
            _dgvCourses.CellClick += DgvCourses_CellClick;
            tCourses.Controls.Add(_dgvCourses);

            // Grades tab
            var tGrades = new TabPage("  Grades  ") { BackColor = dark };
            _dgvGrades = MakeDgv();
            _dgvGrades.CellClick += DgvGrades_CellClick;
            tGrades.Controls.Add(_dgvGrades);

            // Attendance tab
            var tAtt = new TabPage("  Attendance  ") { BackColor = dark };
            _dgvAttendances = MakeDgv();
            _dgvAttendances.CellClick += DgvAttendances_CellClick;
            tAtt.Controls.Add(_dgvAttendances);

            _tabMain.TabPages.AddRange(new[] { tStudents, tCourses, tGrades, tAtt });
            splitContainerMain.Panel1.Controls.Add(_tabMain);
        }

        private DataGridView MakeDgv()
        {
            var dgv = new DataGridView { Dock = DockStyle.Fill };
            StyleDgv(dgv);
            return dgv;
        }

        private void TabMain_DrawItem(object? sender, DrawItemEventArgs e)
        {
            var tab = (TabControl)sender!;
            bool sel = e.Index == tab.SelectedIndex;
            using var bg = new SolidBrush(sel ? Color.FromArgb(48, 63, 159) : Color.FromArgb(40, 40, 52));
            e.Graphics.FillRectangle(bg, e.Bounds);
            using var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            using var br = new SolidBrush(Color.White);
            using var fnt = new Font("Segoe UI", 10F, sel ? FontStyle.Bold : FontStyle.Regular);
            e.Graphics.DrawString(tab.TabPages[e.Index].Text, fnt, br, e.Bounds, fmt);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Right-panel entity editor init
        // ─────────────────────────────────────────────────────────────────────
        private void InitCourseEditor()
        {
            _txtCourseId.Hint = "Course ID";
            _txtCourseName.Hint = "Course Name";
            _txtCourseCode.Hint = "Course Code (e.g. CS101)";
            _txtCourseDesc.Hint = "Description";
            _txtCourseCredits.Hint = "Credits";
            _txtCourseSearch.Hint = "Search by name or code…";

            ConfigBtn(_btnCourseAdd, "ADD COURSE", BtnCourseAdd_Click);
            ConfigBtn(_btnCourseSearch, "SEARCH", (_, _) => LoadCourses(_txtCourseSearch.Text));
            ConfigBtn(_btnCourseUpdate, "UPDATE", BtnCourseUpdate_Click);
            ConfigBtn(_btnCourseDelete, "DELETE", BtnCourseDelete_Click);
            _btnCourseAdd.HighEmphasis = true;

            _pnlCourse.Visible = false;
            _pnlCourse.Controls.AddRange(new Control[]
            {
                _txtCourseId, _txtCourseName, _txtCourseCode, _txtCourseDesc, _txtCourseCredits,
                _btnCourseAdd, _txtCourseSearch, _btnCourseSearch, _btnCourseUpdate, _btnCourseDelete
            });
            materialCard1.Controls.Add(_pnlCourse);
        }

        private void InitGradeEditor()
        {
            _txtGradeId.Hint = "Grade ID";
            _txtGradeStudentId.Hint = "Student ID";
            _txtGradeCourseId.Hint = "Course ID";
            _txtGradeScore.Hint = "Score (0 – 100)";
            _txtGradeLetter.Hint = "Letter Grade (Auto-calculated)";
            _txtGradeLetter.ReadOnly = true; // Auto-calculated!
            _txtGradeSemester.Hint = "Semester (e.g. 2024-S1)";
            _txtGradeSearch.Hint = "Search by Student ID…";

            // Dynamically calculate Letter Grade as user types
            _txtGradeScore.TextChanged += (_, _) =>
            {
                if (double.TryParse(_txtGradeScore.Text, out var score))
                    _txtGradeLetter.Text = CalculateLetterGrade(score);
                else
                    _txtGradeLetter.Text = "";
            };

            ConfigBtn(_btnGradeAdd, "ADD GRADE", BtnGradeAdd_Click);
            ConfigBtn(_btnGradeSearch, "SEARCH", (_, _) => LoadGrades(_txtGradeSearch.Text));
            ConfigBtn(_btnGradeUpdate, "UPDATE", BtnGradeUpdate_Click);
            ConfigBtn(_btnGradeDelete, "DELETE", BtnGradeDelete_Click);
            ConfigBtn(_btnGradeReturn, "← BACK TO STUDENTS", (_, _) => _tabMain.SelectedTab = _tabMain.TabPages[0]);
            _btnGradeAdd.HighEmphasis = true;

            _pnlGrade.Visible = false;
            _pnlGrade.Controls.AddRange(new Control[]
            {
                _txtGradeId, _txtGradeStudentId, _txtGradeCourseId, _txtGradeScore,
                _txtGradeLetter, _txtGradeSemester,
                _btnGradeAdd, _txtGradeSearch, _btnGradeSearch, _btnGradeUpdate, _btnGradeDelete,
                _btnGradeReturn
            });
            materialCard1.Controls.Add(_pnlGrade);
        }

        private void InitAttendanceEditor()
        {
            _txtAttId.Hint = "Attendance ID (auto)"; _txtAttId.ReadOnly = true;
            _txtAttStudentId.Hint = "Student ID";
            _txtAttCourseId.Hint = "Course ID";
            _txtAttDate.Hint = "Date (yyyy-MM-dd)";
            _txtAttNotes.Hint = "Notes";
            _txtAttSearch.Hint = "Search by Student ID…";

            _chkAttPresent.Text = "Present";
            _chkAttPresent.ForeColor = Color.White;
            _chkAttPresent.Font = new Font("Segoe UI", 11F);
            _chkAttPresent.BackColor = Color.Transparent;

            _txtAttCourseSearch.Hint = "Search by Course ID";

            ConfigBtn(_btnAttAdd, "ADD ATTENDANCE", BtnAttAdd_Click);
            ConfigBtn(_btnAttSearch, "SEARCH",
                (_, _) => LoadAttendances(_txtAttSearch.Text, _txtAttCourseSearch.Text));
            ConfigBtn(_btnAttUpdate, "UPDATE", BtnAttUpdate_Click);
            ConfigBtn(_btnAttDelete, "DELETE", BtnAttDelete_Click);
            _btnAttAdd.HighEmphasis = true;

            _pnlAttendance.Visible = false;
            _pnlAttendance.Controls.AddRange(new Control[]
            {
                _txtAttId, _txtAttStudentId, _txtAttCourseId, _txtAttDate, _txtAttNotes,
                _chkAttPresent, _txtAttSearch, _txtAttCourseSearch, _btnAttAdd, _btnAttSearch,
                _btnAttUpdate, _btnAttDelete
            });
            materialCard1.Controls.Add(_pnlAttendance);
        }

        private static void ConfigBtn(MaterialButton btn, string text, EventHandler handler)
        {
            btn.Text = text;
            btn.AutoSize = false;
            btn.HighEmphasis = true; // Globally enforce uniform coloring
            btn.Click += handler;
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Tab switch
        // ─────────────────────────────────────────────────────────────────────
        private void SetActiveTab(int idx)
        {
            bool isStudent = idx == 0;
            txtId.Visible = txtName.Visible = txtAge.Visible = txtDepartment.Visible = isStudent;
            txtPhone.Visible = txtBirthDate.Visible = txtLevel.Visible = isStudent;
            btnAdd.Visible = txtSearch.Visible = btnSearch.Visible =
                btnUpdate.Visible = btnDelete.Visible = isStudent;
            _btnViewGrades.Visible = isStudent;
            _btnViewAttendance.Visible = isStudent;

            _pnlCourse.Visible = idx == 1;
            _pnlGrade.Visible = idx == 2;
            _pnlAttendance.Visible = idx == 3;

            switch (idx)
            {
                case 0:
                    materialLabel1.Text = "Student Details";
                    materialLabelSubtitle.Text = "Add, edit or remove students";
                    LoadStudents();
                    break;
                case 1:
                    materialLabel1.Text = "Course Details";
                    materialLabelSubtitle.Text = "Add, edit or remove courses";
                    LoadCourses();
                    break;
                case 2:
                    materialLabel1.Text = "Grade Details";
                    materialLabelSubtitle.Text = "Record and manage grades";
                    LoadGrades();
                    break;
                case 3:
                    materialLabel1.Text = "Attendance Details";
                    materialLabelSubtitle.Text = "Track daily attendance";
                    LoadAttendances();
                    break;
            }

            LayoutEditorColumn();
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Fonts
        // ─────────────────────────────────────────────────────────────────────
        private void ApplyFonts()
        {
            materialLabel1.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            materialLabelSubtitle.Font = new Font("Segoe UI", 13F);
            materialLabel1.AutoSize = false;
            materialLabel1.Height = materialLabel1.Font.Height + 10;
            materialLabelSubtitle.AutoSize = false;
            materialLabelSubtitle.Height = materialLabelSubtitle.Font.Height + 8;
        }

        // ─────────────────────────────────────────────────────────────────────
        //  DataGrid styling (generic)
        // ─────────────────────────────────────────────────────────────────────
        private static void StyleDgv(DataGridView dgv)
        {
            var header = Color.FromArgb(48, 63, 159);
            var darkBg = Color.FromArgb(33, 33, 33);
            var altBg = Color.FromArgb(42, 42, 55);
            var white = Color.White;
            var sel = Color.FromArgb(63, 81, 181);
            var border = Color.FromArgb(60, 60, 80);

            dgv.BackgroundColor = darkBg;
            dgv.GridColor = border;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoGenerateColumns = true;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.RowHeadersVisible = false;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.ColumnHeadersHeight = 46;
            dgv.RowTemplate.Height = 44;

            dgv.ColumnHeadersDefaultCellStyle.BackColor = header;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = white;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(12, 0, 0, 0);
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = header;
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = white;

            dgv.DefaultCellStyle.BackColor = darkBg;
            dgv.DefaultCellStyle.ForeColor = white;
            dgv.DefaultCellStyle.SelectionBackColor = sel;
            dgv.DefaultCellStyle.SelectionForeColor = white;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 11F);
            dgv.DefaultCellStyle.Padding = new Padding(12, 4, 0, 4);

            dgv.AlternatingRowsDefaultCellStyle.BackColor = altBg;
            dgv.AlternatingRowsDefaultCellStyle.ForeColor = white;
            dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = sel;
            dgv.AlternatingRowsDefaultCellStyle.SelectionForeColor = white;
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Layout engine
        // ─────────────────────────────────────────────────────────────────────
        private void LayoutEditorColumn()
        {
            int pad = 28;
            int usable = Math.Max(260, materialCard1.ClientSize.Width - pad * 2);
            int x = (materialCard1.ClientSize.Width - usable) / 2;

            // Logo
            _logoPictureBox.Left = (materialCard1.ClientSize.Width - _logoPictureBox.Width) / 2;
            _logoPictureBox.Top = 16;
            int y = _logoPictureBox.Bottom + 14;

            // Headings
            materialLabel1.Left = x; materialLabel1.Width = usable; materialLabel1.Top = y;
            y = materialLabel1.Bottom + 6;
            materialLabelSubtitle.Left = x; materialLabelSubtitle.Width = usable; materialLabelSubtitle.Top = y;
            y = materialLabelSubtitle.Bottom + 22;

            int panelH = materialCard1.ClientSize.Height - y - 8;

            if (txtId.Visible)
                LayoutStudentControls(x, usable, ref y);

            if (_pnlCourse.Visible)
                LayoutPanel(_pnlCourse, x, y, usable, panelH, LayoutCourseInPanel);

            if (_pnlGrade.Visible)
                LayoutPanel(_pnlGrade, x, y, usable, panelH, LayoutGradeInPanel);

            if (_pnlAttendance.Visible)
                LayoutPanel(_pnlAttendance, x, y, usable, panelH, LayoutAttendanceInPanel);
        }

        private void LayoutStudentControls(int x, int usable, ref int y)
        {
            foreach (Control c in materialCard1.Controls)
                if (c is MaterialTextBox2 tb && tb.Visible) { tb.Left = x; tb.Width = usable; }

            Place(txtId, x, usable, ref y, 12);
            Place(txtName, x, usable, ref y, 12);
            Place(txtAge, x, usable, ref y, 12);
            Place(txtDepartment, x, usable, ref y, 12);
            Place(txtPhone, x, usable, ref y, 12);
            Place(txtBirthDate, x, usable, ref y, 12);
            Place(txtLevel, x, usable, ref y, 24);

            btnAdd.Left = x; btnAdd.Width = usable; btnAdd.Top = y;
            y = btnAdd.Bottom + 20;

            txtSearch.Left = x; txtSearch.Width = usable; txtSearch.Top = y;
            y = txtSearch.Bottom + 10;

            int g = 10, bW = (usable - g * 2) / 3;
            btnSearch.Left = x; btnSearch.Width = bW; btnSearch.Top = y;
            btnUpdate.Left = x + bW + g; btnUpdate.Width = bW; btnUpdate.Top = y;
            btnDelete.Left = x + 2 * (bW + g); btnDelete.Width = usable - 2 * (bW + g); btnDelete.Top = y;
            y = btnSearch.Bottom + 20;

            // ── Quick-nav row ────────────────────────────────────────────────
            int half = (usable - g) / 2;
            _btnViewGrades.Left = x; _btnViewGrades.Width = half; _btnViewGrades.Top = y;
            _btnViewAttendance.Left = x + half + g; _btnViewAttendance.Width = usable - half - g; _btnViewAttendance.Top = y;
        }

        private static void LayoutPanel(Panel pnl, int x, int y, int usable, int height,
            Action<Panel, int> arrange)
        {
            pnl.Location = new Point(x, y);
            pnl.Width = usable;
            pnl.Height = Math.Max(200, height);
            arrange(pnl, usable);
        }

        private void LayoutCourseInPanel(Panel pnl, int w)
        {
            StackFields(w, new Control[]
                { _txtCourseId, _txtCourseName, _txtCourseCode, _txtCourseDesc, _txtCourseCredits },
                _btnCourseAdd, _txtCourseSearch,
                new[] { _btnCourseSearch, _btnCourseUpdate, _btnCourseDelete });
        }

        private void LayoutGradeInPanel(Panel pnl, int w)
        {
            StackFields(w, new Control[]
                { _txtGradeId, _txtGradeStudentId, _txtGradeCourseId, _txtGradeScore, _txtGradeLetter, _txtGradeSemester },
                _btnGradeAdd, _txtGradeSearch,
                new[] { _btnGradeSearch, _btnGradeUpdate, _btnGradeDelete });
        }

        private void LayoutAttendanceInPanel(Panel pnl, int w)
        {
            int y = 0;
            foreach (var c in new Control[] { _txtAttId, _txtAttStudentId, _txtAttCourseId, _txtAttDate, _txtAttNotes })
            { c.Left = 0; c.Width = w; c.Top = y; y = c.Bottom + 12; }

            _chkAttPresent.Left = 4; _chkAttPresent.Top = y; _chkAttPresent.Width = w;
            y = _chkAttPresent.Bottom + 16;

            _btnAttAdd.Left = 0; _btnAttAdd.Width = w; _btnAttAdd.Top = y;
            y = _btnAttAdd.Bottom + 16;

            int gap2 = 10, half2 = (w - gap2) / 2;
            _txtAttSearch.Left = 0; _txtAttSearch.Width = half2; _txtAttSearch.Top = y;
            _txtAttCourseSearch.Left = half2 + gap2; _txtAttCourseSearch.Width = w - half2 - gap2; _txtAttCourseSearch.Top = y;
            y = _txtAttSearch.Bottom + 10;

            int g = 10, bW = (w - g * 2) / 3;
            _btnAttSearch.Left = 0; _btnAttSearch.Width = bW; _btnAttSearch.Top = y;
            _btnAttUpdate.Left = bW + g; _btnAttUpdate.Width = bW; _btnAttUpdate.Top = y;
            _btnAttDelete.Left = 2 * (bW + g); _btnAttDelete.Width = w - 2 * (bW + g); _btnAttDelete.Top = y;
        }

        // Stacks fields top-to-bottom within parent's coordinate space (0-based y inside panel)
        private static void StackFields(int w, Control[] fields,
            MaterialButton addBtn, MaterialTextBox2 searchBox, MaterialButton[] actionBtns)
        {
            int y = 0;
            foreach (var c in fields) { c.Left = 0; c.Width = w; c.Top = y; y = c.Bottom + 12; }

            addBtn.Left = 0; addBtn.Width = w; addBtn.Top = y; y = addBtn.Bottom + 16;
            searchBox.Left = 0; searchBox.Width = w; searchBox.Top = y; y = searchBox.Bottom + 10;

            int g = 10, bW = (w - g * 2) / 3;
            for (int i = 0; i < actionBtns.Length; i++)
            {
                actionBtns[i].Left = i * (bW + g);
                actionBtns[i].Width = i == actionBtns.Length - 1 ? w - i * (bW + g) : bW;
                actionBtns[i].Top = y;
            }
        }

        private static void Place(Control c, int x, int w, ref int y, int gap)
        {
            c.Left = x; c.Width = w; c.Top = y;
            y = c.Bottom + gap;
        }

        // ─────────────────────────────────────────────────────────────────────
        //  STUDENT CRUD
        // ─────────────────────────────────────────────────────────────────────
        private void LoadStudents()
        {
            using var db = new AppDbContext();
            var today = DateOnly.FromDateTime(DateTime.Today);
            dataGridView1.DataSource = db.Students
                .Select(s => new
                {
                    s.StudentId,
                    s.Name,
                    Age = today.Year - s.BirthDate.Year, // Approx for grid
                    s.Department,
                    s.Level,
                    s.Phone,
                    BirthDate = s.BirthDate.ToString()
                })
                .ToList();
        }

        private void ClearStudent()
        {
            txtId.Text = txtName.Text = txtAge.Text = txtPhone.Text = txtBirthDate.Text = "";
            txtLevel.SelectedIndex = -1;
            txtDepartment.SelectedIndex = -1;
        }

        private void btnAdd_Click(object? sender, EventArgs e)
        {
            if (txtName.Text.Any(char.IsDigit)) { Warn("Name cannot contain numbers."); return; }
            if (txtDepartment.SelectedIndex == -1) { Warn("Department is required."); return; }
            if (!DateOnly.TryParse(txtBirthDate.Text, out var dob)) { Warn("Invalid birth date format."); return; }
            if (txtLevel.SelectedIndex == -1) { Warn("Level is required."); return; }
            if (txtPhone.Text.Trim().Length > 20) { Warn("Phone number cannot exceed 20 characters."); return; }
            try
            {
                _studentService.AddStudent(new Student
                {
                    Name = txtName.Text.Trim(),
                    Department = txtDepartment.Text.Trim(),
                    Phone = txtPhone.Text.Trim(),
                    BirthDate = dob,
                    Level = txtLevel.Text.Trim()
                });
                Info("Student added."); LoadStudents(); ClearStudent();
            }
            catch (Exception ex) { Err(ex.Message); }
        }

        private void btnUpdate_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out var id) || id <= 0) { return; }
            if (string.IsNullOrWhiteSpace(txtName.Text)) { Warn("Please enter a student name in the Full Name field."); return; }
            if (txtName.Text.Any(char.IsDigit)) { Warn("Name cannot contain numbers."); return; }
            if (txtDepartment.SelectedIndex == -1) { Warn("Department is required."); return; }
            if (!DateOnly.TryParse(txtBirthDate.Text, out var dob)) { Warn("Invalid birth date format."); return; }
            if (txtLevel.SelectedIndex == -1) { Warn("Level is required."); return; }
            if (txtPhone.Text.Trim().Length > 20) { Warn("Phone number cannot exceed 20 characters."); return; }
            try
            {
                _studentService.UpdateStudent(new Student
                {
                    StudentId = id,
                    Name = txtName.Text.Trim(),
                    Department = txtDepartment.Text.Trim(),
                    Phone = txtPhone.Text.Trim(),
                    BirthDate = dob,
                    Level = txtLevel.Text.Trim()
                });
                Info("Student updated."); LoadStudents(); ClearStudent();
            }
            catch (Exception ex) { Err(ex.Message); }
        }

        private void btnDelete_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out var id) || id <= 0) {; return; }
            if (Ask("Delete this student? Their grades and attendance will also be deleted.") != DialogResult.Yes) return;
            try { _studentService.DeleteStudent(id); Info("Student deleted."); LoadStudents(); ClearStudent(); }
            catch (Exception ex) { Err(ex.Message); }
        }

        private void btnSearch_Click(object? sender, EventArgs e)
        {
            var q = txtSearch.Text.Trim();
            try
            {
                using var db = new AppDbContext();
                var query = db.Students.AsQueryable();
                if (!string.IsNullOrEmpty(q))
                {
                    if (int.TryParse(q, out int id) && q.Length <= 8)
                        query = query.Where(s => s.StudentId == id);
                    else
                        query = query.Where(s => s.Name.Contains(q) || s.Department.Contains(q) || s.Level.Contains(q) || s.Phone.Contains(q));
                }
                dataGridView1.DataSource = query
                    .Select(s => new
                    {
                        s.StudentId,
                        s.Name,
                        Age = DateTime.Today.Year - s.BirthDate.Year,
                        s.Department,
                        s.Level,
                        s.Phone,
                        BirthDate = s.BirthDate.ToString()
                    })
                    .ToList();
            }
            catch (Exception ex) { Err(ex.Message); }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dataGridView1.Rows[e.RowIndex];
            txtId.Text = row.Cells["StudentId"]?.Value?.ToString() ?? "";
            txtName.Text = row.Cells["Name"]?.Value?.ToString() ?? "";
            txtAge.Text = row.Cells["Age"]?.Value?.ToString() ?? "";
            txtDepartment.Text = row.Cells["Department"]?.Value?.ToString() ?? "";
            txtPhone.Text = row.Cells["Phone"]?.Value?.ToString() ?? "";
            txtBirthDate.Text = row.Cells["BirthDate"]?.Value?.ToString() ?? "";
            txtLevel.Text = row.Cells["Level"]?.Value?.ToString() ?? "";

            // Cross-link: pre-load this student's grades & attendance in the other tabs
            if (int.TryParse(txtId.Text, out var sid))
                LinkStudentData(sid);
        }

        /// <summary>
        /// Pre-filters the Grades and Attendance grids to show only the selected student's records,
        /// and pre-populates the StudentId field in both editors so the user can quickly add new records.
        /// </summary>
        private void LinkStudentData(int studentId)
        {
            var idStr = studentId.ToString();

            // Pre-populate StudentId in both editors
            _txtGradeStudentId.Text = idStr;
            _txtAttStudentId.Text = idStr;

            // Also seed the search boxes so SEARCH button reflects the filter
            _txtGradeSearch.Text = idStr;
            _txtAttSearch.Text = idStr;

            // Pre-load filtered data (background — no tab-switch needed)
            try { LoadGrades(idStr); } catch { }
            try { LoadAttendances(idStr); } catch { }
        }

        // ─────────────────────────────────────────────────────────────────────
        //  COURSE CRUD
        // ─────────────────────────────────────────────────────────────────────
        private void LoadCourses(string filter = "")
        {
            using var db = new AppDbContext();
            var q = db.Courses.AsQueryable();
            if (!string.IsNullOrWhiteSpace(filter))
                q = q.Where(c => c.Name.Contains(filter) || c.Code.Contains(filter));
            _dgvCourses.DataSource = q
                .Select(c => new { c.CourseId, c.Name, c.Code, c.Description, c.Credits })
                .ToList();
        }

        private void ClearCourse()
            => _txtCourseId.Text = _txtCourseName.Text = _txtCourseCode.Text =
               _txtCourseDesc.Text = _txtCourseCredits.Text = "";

        private void BtnCourseAdd_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(_txtCourseId.Text, out var cid)) { Warn("Course ID is required."); return; }
            if (string.IsNullOrWhiteSpace(_txtCourseName.Text)) { Warn("Course name is required."); return; }
            if (!int.TryParse(_txtCourseCredits.Text, out var cr)) { Warn("Credits must be a number."); return; }
            try
            {
                using var db = new AppDbContext();
                db.Courses.Add(new Course
                {
                    CourseId = cid,
                    Name = _txtCourseName.Text.Trim(),
                    Code = _txtCourseCode.Text.Trim(),
                    Description = _txtCourseDesc.Text.Trim(),
                    Credits = cr
                });
                db.SaveChanges();
                Info("Course added."); LoadCourses(); ClearCourse();
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                Err($"Save Error: {msg}");
            }
        }

        private void BtnCourseUpdate_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(_txtCourseId.Text, out var id) || id <= 0) { Warn("Select a course first."); return; }
            if (!int.TryParse(_txtCourseCredits.Text, out var cr)) { Warn("Credits must be a number."); return; }
            try
            {
                using var db = new AppDbContext();
                var c = db.Courses.Find(id);
                if (c == null) { Warn("Course not found."); return; }
                c.Name = _txtCourseName.Text.Trim(); c.Code = _txtCourseCode.Text.Trim();
                c.Description = _txtCourseDesc.Text.Trim(); c.Credits = cr;
                db.SaveChanges();
                Info("Course updated."); LoadCourses(); ClearCourse();
            }
            catch (Exception ex) { Err(ex.Message); }
        }

        private void BtnCourseDelete_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(_txtCourseId.Text, out var id) || id <= 0) { Warn("Select a course first."); return; }
            if (Ask("Delete this course?") != DialogResult.Yes) return;
            try
            {
                using var db = new AppDbContext();
                var c = db.Courses.Find(id);
                if (c != null) { db.Courses.Remove(c); db.SaveChanges(); }
                Info("Course deleted."); LoadCourses(); ClearCourse();
            }
            catch (Exception ex) { Err(ex.Message); }
        }

        private void DgvCourses_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = _dgvCourses.Rows[e.RowIndex];
            _txtCourseId.Text = row.Cells["CourseId"]?.Value?.ToString() ?? "";
            _txtCourseName.Text = row.Cells["Name"]?.Value?.ToString() ?? "";
            _txtCourseCode.Text = row.Cells["Code"]?.Value?.ToString() ?? "";
            _txtCourseDesc.Text = row.Cells["Description"]?.Value?.ToString() ?? "";
            _txtCourseCredits.Text = row.Cells["Credits"]?.Value?.ToString() ?? "";
        }

        // ─────────────────────────────────────────────────────────────────────
        //  GRADE CRUD
        // ─────────────────────────────────────────────────────────────────────
        private void LoadGrades(string filter = "")
        {
            using var db = new AppDbContext();
            var q = db.Grades.Include(g => g.Student).Include(g => g.Course).AsQueryable();
            if (!string.IsNullOrWhiteSpace(filter) && int.TryParse(filter, out var sid))
                q = q.Where(g => g.StudentId == sid);
            _dgvGrades.DataSource = q
                .Select(g => new
                {
                    g.GradeId,
                    Student = g.Student.Name,
                    Course = g.Course.Name,
                    g.Score,
                    LetterGrade = g.Score >= 90 ? "A" : g.Score >= 80 ? "B" : g.Score >= 70 ? "C" : g.Score >= 60 ? "D" : "F",
                    g.Semester
                }).ToList();
        }

        private void ClearGrade()
            => _txtGradeId.Text = _txtGradeStudentId.Text = _txtGradeCourseId.Text =
               _txtGradeScore.Text = _txtGradeLetter.Text = _txtGradeSemester.Text = "";

        private string CalculateLetterGrade(double score) => score switch
        {
            >= 90 => "A",
            >= 80 => "B",
            >= 70 => "C",
            >= 60 => "D",
            _ => "F"
        };

        private void BtnGradeAdd_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(_txtGradeStudentId.Text, out var sid)) { Warn("Valid Student ID required."); return; }
            if (!int.TryParse(_txtGradeCourseId.Text, out var cid)) { Warn("Valid Course ID required."); return; }
            if (!double.TryParse(_txtGradeScore.Text, out var score)) { Warn("Score must be a number."); return; }
            try
            {
                using var db = new AppDbContext();
                db.Grades.Add(new Grade
                {
                    StudentId = sid,
                    CourseId = cid,
                    Score = score,
                    LetterGrade = CalculateLetterGrade(score),
                    Semester = _txtGradeSemester.Text.Trim()
                });
                db.SaveChanges();
                Info("Grade added."); LoadGrades(); ClearGrade();
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                Err($"Save Error: {msg}");
            }
        }

        private void BtnGradeUpdate_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(_txtGradeId.Text, out var id) || id <= 0) { Warn("Select a grade first."); return; }
            if (!double.TryParse(_txtGradeScore.Text, out var score)) { Warn("Score must be a number."); return; }
            try
            {
                using var db = new AppDbContext();
                var g = db.Grades.Find(id);
                if (g == null) { Warn("Grade not found."); return; }
                g.Score = score;
                g.LetterGrade = CalculateLetterGrade(score);
                g.Semester = _txtGradeSemester.Text.Trim();
                db.SaveChanges();
                Info("Grade updated."); LoadGrades(); ClearGrade();
            }
            catch (Exception ex) { Err(ex.Message); }
        }

        private void BtnGradeDelete_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(_txtGradeId.Text, out var id) || id <= 0) { Warn("Select a grade first."); return; }
            if (Ask("Delete this grade?") != DialogResult.Yes) return;
            try
            {
                using var db = new AppDbContext();
                var g = db.Grades.Find(id);
                if (g != null) { db.Grades.Remove(g); db.SaveChanges(); }
                Info("Grade deleted."); LoadGrades(); ClearGrade();
            }
            catch (Exception ex) { Err(ex.Message); }
        }

        private void DgvGrades_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = _dgvGrades.Rows[e.RowIndex];
            _txtGradeId.Text = row.Cells["GradeId"]?.Value?.ToString() ?? "";
            _txtGradeScore.Text = row.Cells["Score"]?.Value?.ToString() ?? "";
            _txtGradeLetter.Text = row.Cells["LetterGrade"]?.Value?.ToString() ?? "";
            _txtGradeSemester.Text = row.Cells["Semester"]?.Value?.ToString() ?? "";
        }

        // ─────────────────────────────────────────────────────────────────────
        //  ATTENDANCE CRUD
        // ─────────────────────────────────────────────────────────────────────
        private void LoadAttendances(string filterStudentId = "", string filterCourseId = "")
        {
            using var db = new AppDbContext();
            var q = db.Attendances.Include(a => a.Student).Include(a => a.Course).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filterStudentId) && int.TryParse(filterStudentId, out var sid))
                q = q.Where(a => a.StudentId == sid);

            if (!string.IsNullOrWhiteSpace(filterCourseId) && int.TryParse(filterCourseId, out var cid))
                q = q.Where(a => a.CourseId == cid);

            _dgvAttendances.DataSource = q
                .Select(a => new
                {
                    a.AttendanceId,
                    a.StudentId,
                    Student = a.Student.Name,
                    a.CourseId,
                    Course = a.Course.Name,
                    a.Date,
                    a.IsPresent,
                    a.Notes
                }).ToList();
        }

        private void ClearAttendance()
            => _txtAttId.Text = _txtAttStudentId.Text = _txtAttCourseId.Text =
               _txtAttDate.Text = _txtAttNotes.Text = "";

        private void BtnAttAdd_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(_txtAttStudentId.Text, out var sid)) { Warn("Valid Student ID required."); return; }
            if (!int.TryParse(_txtAttCourseId.Text, out var cid)) { Warn("Valid Course ID required."); return; }
            if (!DateTime.TryParse(_txtAttDate.Text, out var date)) { Warn("Valid date required (yyyy-MM-dd)."); return; }
            try
            {
                using var db = new AppDbContext();
                db.Attendances.Add(new Attendance
                {
                    StudentId = sid,
                    CourseId = cid,
                    Date = date,
                    IsPresent = _chkAttPresent.Checked,
                    Notes = _txtAttNotes.Text.Trim()
                });
                db.SaveChanges();
                Info("Attendance recorded."); LoadAttendances(); ClearAttendance();
            }
            catch (Exception ex) { Err(ex.Message); }
        }

        private void BtnAttUpdate_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(_txtAttId.Text, out var id) || id <= 0) { Warn("Select a record first."); return; }
            if (!DateTime.TryParse(_txtAttDate.Text, out var date)) { Warn("Valid date required (yyyy-MM-dd)."); return; }
            try
            {
                using var db = new AppDbContext();
                var a = db.Attendances.Find(id);
                if (a == null) { Warn("Record not found."); return; }
                a.Date = date;
                a.IsPresent = _chkAttPresent.Checked;
                a.Notes = _txtAttNotes.Text.Trim();
                db.SaveChanges();
                Info("Attendance updated."); LoadAttendances(); ClearAttendance();
            }
            catch (Exception ex) { Err(ex.Message); }
        }

        private void BtnAttDelete_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(_txtAttId.Text, out var id) || id <= 0) { Warn("Select a record first."); return; }
            if (Ask("Delete this attendance record?") != DialogResult.Yes) return;
            try
            {
                using var db = new AppDbContext();
                var a = db.Attendances.Find(id);
                if (a != null) { db.Attendances.Remove(a); db.SaveChanges(); }
                Info("Record deleted."); LoadAttendances(); ClearAttendance();
            }
            catch (Exception ex) { Err(ex.Message); }
        }

        private void DgvAttendances_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = _dgvAttendances.Rows[e.RowIndex];
            _txtAttId.Text = row.Cells["AttendanceId"]?.Value?.ToString() ?? "";
            _txtAttDate.Text = row.Cells["Date"]?.Value is DateTime d
                                        ? d.ToString("yyyy-MM-dd") : "";
            _chkAttPresent.Checked = row.Cells["IsPresent"]?.Value is true;
            _txtAttNotes.Text = row.Cells["Notes"]?.Value?.ToString() ?? "";
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Message helpers
        // ─────────────────────────────────────────────────────────────────────
        private static void Info(string m) => MessageBox.Show(m, "SMS", MessageBoxButtons.OK, MessageBoxIcon.Information);
        private static void Warn(string m) => MessageBox.Show(m, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        private static void Err(string m) => MessageBox.Show(m, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        private static DialogResult Ask(string m) => MessageBox.Show(m, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
    }
}
