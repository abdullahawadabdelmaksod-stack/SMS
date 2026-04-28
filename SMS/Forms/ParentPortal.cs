using MaterialSkin;
using MaterialSkin.Controls;
using Microsoft.EntityFrameworkCore;
using SMS.Data;
using SMS.Models;
using SMS.Services;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SMS
{
    public class ParentPortal : MaterialForm
    {
        // ── Palette ──────────────────────────────────────────────────────────
        private static readonly Color BgMain   = Color.FromArgb(18, 18, 28);
        private static readonly Color BgCard   = Color.FromArgb(28, 30, 50);
        private static readonly Color BgDark   = Color.FromArgb(22, 22, 38);
        private static readonly Color Indigo   = Color.FromArgb(48, 63, 159);
        private static readonly Color Teal     = Color.FromArgb(0, 150, 136);
        private static readonly Color Purple   = Color.FromArgb(103, 58, 183);
        private static readonly Color TextMain = Color.White;
        private static readonly Color TextSub  = Color.FromArgb(160, 170, 210);
        private static readonly Color GradeA   = Color.FromArgb(38, 166, 91);
        private static readonly Color GradeB   = Color.FromArgb(0, 150, 136);
        private static readonly Color GradeC   = Color.FromArgb(245, 177, 0);
        private static readonly Color GradeD   = Color.FromArgb(245, 124, 0);
        private static readonly Color GradeF   = Color.FromArgb(229, 57, 53);

        // ── Shared cached fonts ───────────────────────────────────────────────
        private static readonly Font FontBold12 = new("Segoe UI", 12F, FontStyle.Bold);
        private static readonly Font FontReg12  = new("Segoe UI", 12F);
        // keep old names pointing to bigger fonts so all references auto-upgrade
        private static readonly Font FontBold10 = FontBold12;
        private static readonly Font FontReg10  = FontReg12;

        // ── Controls ─────────────────────────────────────────────────────────
        private MaterialTextBox2 _txtSearch     = new();
        private MaterialButton   _btnSearch     = new();
        private Label            _lblStatus     = new();
        private Label            _lblNoResult   = new();
        private Panel            _pnlResult     = new();   // fill area
        private Panel            _pnlContent    = new();   // shown after search
        private Panel            _pnlInfo       = new();   // student card
        private DataGridView     _dgvGrades     = new();
        private DataGridView     _dgvAttendance = new();

        // Named labels inside the info card — kept as fields for fast update
        private Label _lblName = new();
        private Label _lblInfo = new();
        private Label _lblGPA  = new();   // kept for backward compat (hidden)
        private Label _lblAtt  = new();   // kept for backward compat (hidden)

        // ── Services ──────────────────────────────────────────────────────────
        private readonly GradeService      _gradeSvc = new();
        private readonly AttendanceService _attSvc   = new();
        private readonly ReportService     _reportSvc = new();

        // ── Data ──────────────────────────────────────────────────────────────
        private Student? _currentStudent;

        // ── Ring gauges ───────────────────────────────────────────────────────
        private RingGaugePanel _gaugeGPA = new();
        private RingGaugePanel _gaugeAtt = new();
        
        private MaterialSkin.Controls.MaterialButton _btnDownloadPdf = new();

        public ParentPortal()
        {
            InitializeComponent();
            BuildUI();
        }

        // ── MaterialSkin setup ────────────────────────────────────────────────
        private void InitializeComponent()
        {
            var skin = MaterialSkinManager.Instance;
            skin.AddFormToManage(this);
            skin.Theme       = MaterialSkinManager.Themes.DARK;
            skin.ColorScheme = new ColorScheme(
                Primary.Indigo800, Primary.Indigo900, Primary.Indigo500,
                Accent.Teal200, TextShade.WHITE);

            Text          = "Parent Portal";
            Size          = new Size(1080, 720);
            MinimumSize   = new Size(800, 560);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor     = BgMain;
            FormClosed   += (_, _) => Application.Exit();
        }

        // ── Full UI assembly ──────────────────────────────────────────────────
        private void BuildUI()
        {
            // Dock order: Fill added first, Top panels added after (last-added = topmost)
            BuildResultArea();   // Dock=Fill  → first
            BuildFooter();       // Dock=Bottom
            BuildSearchBar();    // Dock=Top
            BuildHeader();       // Dock=Top → last, renders at very top
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Header
        // ─────────────────────────────────────────────────────────────────────
        private void BuildHeader()
        {
            var hdr = new Panel { Dock = DockStyle.Top, Height = 88, BackColor = Indigo };
            hdr.Paint += (_, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                var rc = hdr.ClientRectangle;
                if (rc.Width <= 0) return;

                using var grad = new LinearGradientBrush(rc,
                    Color.FromArgb(26, 35, 100), Color.FromArgb(72, 88, 200),
                    LinearGradientMode.Horizontal);
                g.FillRectangle(grad, rc);

                using var accent = new Pen(Color.FromArgb(80, 180, 255), 2);
                g.DrawLine(accent, 0, hdr.Height - 1, hdr.Width, hdr.Height - 1);

                using var f1 = new Font("Segoe UI", 24F, FontStyle.Bold);
                using var f2 = new Font("Segoe UI", 11F);
                using var w  = new SolidBrush(Color.White);
                using var lw = new SolidBrush(Color.FromArgb(190, 210, 255));
                g.DrawString("🎓  Parent Portal  ·  بوابة ولي الأمر", f1, w, 24, 14);
                g.DrawString("Search your son's grades and attendance records  |  ابحث عن درجات وسجلات حضور ابنك", f2, lw, 28, 60);
            };

            // Live clock — right-aligned
            var clock = new Label
            {
                Font      = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(190, 210, 255),
                BackColor = Color.Transparent,
                AutoSize  = true,
                Text      = DateTime.Now.ToString("ddd, dd MMM  HH:mm:ss")
            };
            var t = new System.Windows.Forms.Timer { Interval = 1000 };
            t.Tick += (_, _) => clock.Text = DateTime.Now.ToString("ddd, dd MMM  HH:mm:ss");
            t.Start();

            hdr.Controls.Add(clock);
            hdr.Resize += (_, _) => clock.Location = new Point(hdr.Width - clock.Width - 18, 10);

            Controls.Add(hdr);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Search bar
        // ─────────────────────────────────────────────────────────────────────
        private void BuildSearchBar()
        {
            var bar = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 70,
                BackColor = BgDark
            };
            bar.Paint += (_, e) =>
            {
                using var pen = new Pen(Color.FromArgb(40, 44, 70));
                e.Graphics.DrawLine(pen, 0, bar.Height - 1, bar.Width, bar.Height - 1);
            };

            // Search textbox + SEARCH button only (Sign Out is in the footer)
            var tlp = new TableLayoutPanel
            {
                Dock        = DockStyle.Fill,
                ColumnCount = 2,
                RowCount    = 1,
                BackColor   = Color.Transparent,
                Padding     = new Padding(20, 14, 20, 14)
            };
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); // textbox fills
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F)); // search btn

            _txtSearch = new MaterialTextBox2
            {
                Hint      = "🔍  Enter Student ID (numbers only)...  |  أدخل رقم الطالب (أرقام فقط)...",
                Dock      = DockStyle.Fill,
                BackColor = BgCard,
                ForeColor = TextMain,
                Font      = new Font("Segoe UI", 12F)
            };
            _txtSearch.KeyPress += (_, e) => { if (e.KeyChar == (char)13) SearchStudent(); };

            _btnSearch = new MaterialButton
            {
                Text         = "SEARCH  ·  بحث",
                HighEmphasis = true,
                AutoSize     = false,
                Dock         = DockStyle.Fill,
                Margin       = new Padding(8, 0, 0, 0)
            };
            _btnSearch.Click += (_, _) => SearchStudent();

            tlp.Controls.Add(_txtSearch, 0, 0);
            tlp.Controls.Add(_btnSearch, 1, 0);

            bar.Controls.Add(tlp);
            Controls.Add(bar);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Footer status bar
        // ─────────────────────────────────────────────────────────────────────
        private void BuildFooter()
        {
            var footer = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 42,
                BackColor = Color.FromArgb(14, 14, 22)
            };
            footer.Paint += (_, e) =>
            {
                using var pen = new Pen(Color.FromArgb(40, 44, 70));
                e.Graphics.DrawLine(pen, 0, 0, footer.Width, 0);
            };

            // Status text — fills left side
            _lblStatus = new Label
            {
                Text      = "  Ready — enter a Student ID to search  |  جاهز - أدخل رقم الطالب للبحث",
                Font      = new Font("Segoe UI", 8.5F),
                ForeColor = TextSub,
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Sign Out — anchored to right of footer
            var btnSignout = new MaterialButton
            {
                Text     = "SIGN OUT  ·  خروج",
                AutoSize = false,
                Width    = 110,
                Height   = 30,
                Dock     = DockStyle.Right,
                Margin   = new Padding(0, 6, 12, 6)
            };
            btnSignout.Click += (_, _) =>
            {
                Hide();
                foreach (Form f in Application.OpenForms)
                    if (f is LoginForm) { f.Show(); return; }
                Application.Exit();
            };

            // Add Sign Out first (Dock=Right processed before Fill)
            footer.Controls.Add(btnSignout);
            footer.Controls.Add(_lblStatus);
            Controls.Add(footer);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Result area  (Fill)
        // ─────────────────────────────────────────────────────────────────────
        private void BuildResultArea()
        {
            _pnlResult = new Panel { Dock = DockStyle.Fill, BackColor = BgMain, Padding = new Padding(20) };

            // ── Empty-state label ────────────────────────────────────────────
            _lblNoResult = new Label
            {
                Text      = "🔍\r\nEnter a Student ID above to view records\r\nأدخل رقم الطالب أعلاه لعرض السجلات",
                Font      = new Font("Segoe UI", 13F),
                ForeColor = TextSub,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize  = false,
                Size      = new Size(420, 90),
                Anchor    = AnchorStyles.None
            };
            _pnlResult.Controls.Add(_lblNoResult);
            _pnlResult.Resize += (_, _) => _lblNoResult.Location = new Point(
                (_pnlResult.ClientSize.Width  - _lblNoResult.Width)  / 2,
                (_pnlResult.ClientSize.Height - _lblNoResult.Height) / 2);

            // ── Content panel (hidden until search) ──────────────────────────
            _pnlContent = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = BgMain,
                AutoScroll = true,
                Visible   = false
            };
            // Dock=Top: LAST control added = topmost visually.
            // Add bottom sections first, top sections last.
            BuildAttendanceGrid(_pnlContent);  // bottom
            BuildGradesGrid(_pnlContent);      // above attendance
            BuildStatsBar(_pnlContent);        // GPA + Att cards, above grids
            BuildInfoCard(_pnlContent);        // top — student name & info

            _pnlResult.Controls.Add(_pnlContent);
            Controls.Add(_pnlResult);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Student info card  (Dock=Top inside _pnlContent)
        // ─────────────────────────────────────────────────────────────────────
        private void BuildInfoCard(Panel container)
        {
            _pnlInfo = new MaterialSkin.Controls.MaterialCard
            {
                Dock      = DockStyle.Top,
                Height    = 90,
                BackColor = BgCard,
                Padding   = new Padding(18, 8, 18, 8)
            };

            // Indigo → Teal accent stripe + bottom separator
            _pnlInfo.Paint += (_, e) =>
            {
                var rc = _pnlInfo.ClientRectangle;
                if (rc.Width <= 0) return;
                using var stripe = new LinearGradientBrush(
                    new Rectangle(0, 0, Math.Max(1, rc.Width), 3),
                    Indigo, Teal, LinearGradientMode.Horizontal);
                e.Graphics.FillRectangle(stripe, 0, 0, rc.Width, 3);
                using var sep = new Pen(Color.FromArgb(40, 44, 70));
                e.Graphics.DrawLine(sep, 0, rc.Height - 1, rc.Width, rc.Height - 1);
            };

            // Student name — large, bold, left-aligned
            _lblName = new MaterialSkin.Controls.MaterialLabel
            {
                Text      = "—",
                Font      = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = TextMain,
                AutoSize  = true,
                Location  = new Point(18, 12)
            };

            // Sub-info line — department / level / ID
            _lblInfo = new MaterialSkin.Controls.MaterialLabel
            {
                Text      = "ID: —  ·  Department (القسم): —  ·  Level (المستوى): —",
                Font      = new Font("Segoe UI", 11F),
                ForeColor = TextSub,
                AutoSize  = true,
                Location  = new Point(18, 52)
            };

            _btnDownloadPdf = new MaterialSkin.Controls.MaterialButton
            {
                Text      = "DOWNLOAD PDF  ·  تحميل",
                Type      = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined,
                AutoSize  = false,
                Width     = 220,
                Height    = 36,
                Location  = new Point(_pnlInfo.Width - 240, 30),
                Anchor    = AnchorStyles.Top | AnchorStyles.Right,
                UseAccentColor = true
            };
            _btnDownloadPdf.Click += BtnDownloadPdf_Click;

            _pnlInfo.Controls.AddRange(new Control[] { _lblName, _lblInfo, _btnDownloadPdf });

            // Spacer below card
            var spacer = new Panel { Dock = DockStyle.Top, Height = 12, BackColor = BgMain };

            container.Controls.Add(spacer);
            container.Controls.Add(_pnlInfo);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Stats bar — GPA + Attendance ring gauges, above the grids
        // ─────────────────────────────────────────────────────────────────────
        private void BuildStatsBar(Panel container)
        {
            var bar = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 180,
                BackColor = BgMain,
                Padding   = new Padding(8, 6, 8, 6)
            };

            var tlp = new TableLayoutPanel
            {
                Dock        = DockStyle.Fill,
                ColumnCount = 2,
                RowCount    = 1,
                BackColor   = Color.Transparent
            };
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            _gaugeGPA = new RingGaugePanel
            {
                Caption   = "GPA  ·  المعدل",
                SubText   = "out of 4.0  |  من 4.0",
                Accent    = Teal,
                MaxValue  = 4.0,
                Dock      = DockStyle.Fill,
                Margin    = new Padding(4, 0, 4, 0)
            };

            _gaugeAtt = new RingGaugePanel
            {
                Caption   = "Attendance  ·  الحضور",
                SubText   = "of sessions  |  من الجلسات",
                Accent    = Purple,
                MaxValue  = 100.0,
                IsPercent = true,
                Dock      = DockStyle.Fill,
                Margin    = new Padding(4, 0, 4, 0)
            };

            tlp.Controls.Add(_gaugeGPA, 0, 0);
            tlp.Controls.Add(_gaugeAtt, 1, 0);
            bar.Controls.Add(tlp);

            var spacer = new Panel { Dock = DockStyle.Top, Height = 8, BackColor = BgMain };
            container.Controls.Add(spacer);
            container.Controls.Add(bar);
        }


        // ─────────────────────────────────────────────────────────────────────
        //  Grades grid  (Dock=Top)
        // ─────────────────────────────────────────────────────────────────────
        private void BuildGradesGrid(Panel container)
        {
            var title = SectionTitle("📚  Course Grades  ·  درجات المقررات");
            _dgvGrades = MakeGrid();
            _dgvGrades.Dock   = DockStyle.Top;
            _dgvGrades.Height = 220;
            StyleGrid(_dgvGrades, Indigo);

            _dgvGrades.CellFormatting += (_, e) =>
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0 || e.Value == null) return;
                var col = _dgvGrades.Columns[e.ColumnIndex].Name;
                if (col == "LetterGrade")
                {
                    e.CellStyle.Font      = FontBold10;
                    e.CellStyle.ForeColor = GradeColor(e.Value.ToString()!);
                }
                else if (col == "Score")
                {
                    e.CellStyle.ForeColor = ScoreColor(Convert.ToDouble(e.Value));
                }
            };
            _dgvGrades.DataError += (_, e) => e.ThrowException = false;

            var spacer = new Panel { Dock = DockStyle.Top, Height = 10, BackColor = BgMain };

            // Add in reverse (last-added = top in Dock=Top chain)
            container.Controls.Add(spacer);
            container.Controls.Add(_dgvGrades);
            container.Controls.Add(title);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Attendance grid  (Dock=Top)
        // ─────────────────────────────────────────────────────────────────────
        private void BuildAttendanceGrid(Panel container)
        {
            var title = SectionTitle("📅  Attendance Records  ·  سجلات الحضور");
            _dgvAttendance = MakeGrid();
            _dgvAttendance.Dock   = DockStyle.Top;
            _dgvAttendance.Height = 220;
            StyleGrid(_dgvAttendance, Teal);

            _dgvAttendance.CellFormatting += (_, e) =>
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0 || e.Value == null) return;
                if (_dgvAttendance.Columns[e.ColumnIndex].Name == "Status")
                    e.CellStyle.ForeColor = e.Value.ToString()!.Contains("Present") ? GradeA : GradeF;
            };
            _dgvAttendance.DataError += (_, e) => e.ThrowException = false;

            var spacer = new Panel { Dock = DockStyle.Top, Height = 10, BackColor = BgMain };
            var bottom = new Panel { Dock = DockStyle.Top, Height = 20, BackColor = BgMain };

            container.Controls.Add(bottom);
            container.Controls.Add(spacer);
            container.Controls.Add(_dgvAttendance);
            container.Controls.Add(title);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Helpers
        // ─────────────────────────────────────────────────────────────────────
        private Panel SectionTitle(string text)
        {
            var p = new Panel { Dock = DockStyle.Top, Height = 38, BackColor = BgMain };
            var l = new Label
            {
                Text = text, Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = TextMain, AutoSize = true, Location = new Point(0, 10)
            };
            p.Controls.Add(l);
            return p;
        }

        private DataGridView MakeGrid() => new DataGridView
        {
            BackColor               = BgCard,
            GridColor               = Color.FromArgb(40, 44, 70),
            BorderStyle             = BorderStyle.None,
            CellBorderStyle         = DataGridViewCellBorderStyle.SingleHorizontal,
            ReadOnly                = true,
            AllowUserToAddRows      = false,
            AllowUserToDeleteRows   = false,
            AllowUserToResizeRows   = false,
            SelectionMode           = DataGridViewSelectionMode.FullRowSelect,
            AutoGenerateColumns     = true,
            AutoSizeColumnsMode     = DataGridViewAutoSizeColumnsMode.Fill,
            RowHeadersVisible       = false,
            EnableHeadersVisualStyles = false,
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        };

        private void StyleGrid(DataGridView dgv, Color header)
        {
            dgv.BackgroundColor = BgCard;
            var alt = Color.FromArgb(33, 35, 58);
            var sel = Color.FromArgb(63, 81, 181);

            dgv.DefaultCellStyle.BackColor          = BgCard;
            dgv.DefaultCellStyle.ForeColor          = Color.White;
            dgv.DefaultCellStyle.SelectionBackColor = sel;
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv.DefaultCellStyle.Font               = FontReg10;
            dgv.DefaultCellStyle.Padding            = new Padding(10, 3, 0, 3);

            dgv.AlternatingRowsDefaultCellStyle.BackColor          = alt;
            dgv.AlternatingRowsDefaultCellStyle.ForeColor          = Color.White;
            dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = sel;
            dgv.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;

            dgv.ColumnHeadersDefaultCellStyle.BackColor          = header;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor          = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font               = FontBold12;
            dgv.ColumnHeadersDefaultCellStyle.Padding            = new Padding(12, 0, 0, 0);
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = header;
            dgv.ColumnHeadersHeight  = 46;
            dgv.RowTemplate.Height   = 44;
        }

        private Color GradeColor(string letter) => letter switch
        {
            "A" => GradeA, "B" => GradeB, "C" => GradeC, "D" => GradeD, _ => GradeF
        };
        private Color ScoreColor(double s) =>
            s >= 90 ? GradeA : s >= 80 ? GradeB : s >= 70 ? GradeC : s >= 60 ? GradeD : GradeF;

        // ─────────────────────────────────────────────────────────────────────
        //  Search logic
        // ─────────────────────────────────────────────────────────────────────
        private void SearchStudent()
        {
            if (!_btnSearch.Enabled) return; // Prevent double-clicks

            string input = _txtSearch.Text.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                SetStatus("  ⚠  Student ID is required  |  رقم الطالب مطلوب", TextSub);
                MessageBox.Show("Please enter a Student ID to search.\nالرجاء إدخال رقم الطالب للبحث.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(input, out int id) || id <= 0)
            {
                SetStatus("  ⚠  Invalid ID format  |  صيغة الرقم غير صحيحة", TextSub);
                MessageBox.Show("Please enter a valid numeric Student ID.\nالرجاء إدخال رقم طالب رقمي صحيح أكبر من الصفر.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _btnSearch.Text    = "SEARCHING…  ·  جاري البحث…";
            _btnSearch.Enabled = false;
            SetStatus("  Searching…  |  جاري البحث…", TextSub);
            Application.DoEvents();

            try
            {
                using var db = new AppDbContext();
                var s = db.Students
                    .Include(x => x.Grades).ThenInclude(g => g.Course)
                    .Include(x => x.Attendances).ThenInclude(a => a.Course)
                    .FirstOrDefault(x => x.StudentId == id);

                if (s == null)
                {
                    SetStatus($"  ✘  No student found with ID {id}  |  لم يتم العثور على طالب بالرقم {id}", GradeF);
                    MessageBox.Show("No student found with that ID.\r\nلم يتم العثور على طالب بهذا الرقم.", "Not Found",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                _currentStudent = s;

                _lblNoResult.Visible = false;
                _pnlContent.Visible  = true;

                // Update info card
                _lblName.Text = s.Name;
                _lblInfo.Text = $"ID: {s.StudentId}  ·  {s.Department}  ·  Level (المستوى): {s.Level}";

                // ── Use GradeService + AttendanceService ────────────────────────────────
                double gpa = _gradeSvc.GetGPA(s.StudentId);
                double att = _attSvc.GetAttendanceRate(s.StudentId);

                // Feed the ring gauges (animated fill)
                _gaugeGPA.SetValue(gpa);
                _gaugeAtt.SetValue(att);


                // Load grids
                _dgvGrades.DataSource = s.Grades.Select(g => new
                {
                    Code        = g.Course.Code,
                    Course      = g.Course.Name,
                    Score       = g.Score,
                    LetterGrade = g.LetterGrade,
                    Semester    = g.Semester
                }).ToList();

                _dgvAttendance.DataSource = s.Attendances
                    .OrderByDescending(a => a.Date)
                    .Select(a => new
                    {
                        Course = a.Course.Name,
                        Date   = a.Date.ToShortDateString(),
                        Status = a.IsPresent ? "✔  Present  ·  حاضر" : "✘  Absent  ·  غائب",
                        Notes  = a.Notes
                    }).ToList();

                SetStatus($"  ✔  {s.Name}  ·  {s.Grades.Count} grades (درجات) · {s.Attendances.Count} sessions (جلسات)  ·  {DateTime.Now:HH:mm:ss}", GradeA);
            }
            catch (Exception ex)
            {
                SetStatus("  ✘  Error loading data  |  خطأ في تحميل البيانات", GradeF);
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _btnSearch.Text    = "SEARCH  ·  بحث";
                _btnSearch.Enabled = true;
            }
        }

        private void SetStatus(string msg, Color color)
        {
            _lblStatus.Text      = msg;
            _lblStatus.ForeColor = color;
        }

        private void BtnDownloadPdf_Click(object? sender, EventArgs e)
        {
            if (_currentStudent == null) return;

            using var sfd = new SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                FileName = $"Report_{_currentStudent.StudentId}_{DateTime.Now:yyyyMMdd}.pdf",
                Title = "Save Student Report"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _reportSvc.GenerateStudentReportPdf(_currentStudent, sfd.FileName);
                    MessageBox.Show("Report generated successfully!\nتم إنشاء التقرير بنجاح!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(sfd.FileName) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error generating PDF: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        [System.Runtime.InteropServices.DllImport("Gdi32.dll")]
        private static extern IntPtr CreateRoundRectRgn(int x1, int y1, int x2, int y2, int w, int h);
    }
}