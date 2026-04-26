using MaterialSkin;
using MaterialSkin.Controls;
using Microsoft.EntityFrameworkCore;
using SMS.Data;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SMS
{
    public partial class Dashboard : MaterialForm
    {
        // ── Palette ───────────────────────────────────────────────────────────
        static readonly Color BgMain = Color.FromArgb(18, 18, 28);
        static readonly Color BgSide = Color.FromArgb(22, 22, 38);
        static readonly Color BgCard = Color.FromArgb(28, 30, 50);
        static readonly Color BgTop = Color.FromArgb(22, 22, 38);
        static readonly Color Indigo = Color.FromArgb(48, 63, 159);
        static readonly Color Teal = Color.FromArgb(0, 150, 136);
        static readonly Color Purple = Color.FromArgb(103, 58, 183);
        static readonly Color Blue = Color.FromArgb(21, 101, 192);
        static readonly Color Amber = Color.FromArgb(245, 124, 0);
        static readonly Color TextMain = Color.White;
        static readonly Color TextSub = Color.FromArgb(160, 170, 210);

        // ── Layout ───────────────────────────────────────────────────────────
        private Panel _pnlSidebar = new();
        private Panel _pnlContent = new();
        private Panel _pnlTopBar = new();
        private Label _lblTitle = new();
        private Label _lblClock = new();

        // ── Pages ─────────────────────────────────────────────────────────────
        private Panel _pgDash = new(), _pgStud = new(), _pgCrs = new(),
                      _pgGrd = new(), _pgAtt = new();
        private Panel[] _pages = null!;
        private string[] _pageTitles = { "Dashboard", "Students", "Courses", "Grades", "Attendance" };

        // ── Stat value labels (updated by LoadStats) ──────────────────────────
        private Label _valStudents = new(), _valCourses = new(), _valGrades = new(),
                      _valAttRate = new(), _valAvgScore = new();

        // ── Nav buttons ───────────────────────────────────────────────────────
        private NavButton[] _navBtns = null!;

        // ── DataGridViews ────────────────────────────────────────────────────
        private DataGridView _dgvRecent = new(), _dgvStud = new(), _dgvCrs = new(),
                             _dgvGrd = new(), _dgvAtt = new();

        // ─────────────────────────────────────────────────────────────────────
        //  Constructor
        // ─────────────────────────────────────────────────────────────────────
        public Dashboard()
        {
            InitializeComponent();

            var skin = MaterialSkinManager.Instance;
            skin.AddFormToManage(this);
            skin.Theme = MaterialSkinManager.Themes.DARK;
            skin.ColorScheme = new ColorScheme(
                Primary.Indigo800, Primary.Indigo900, Primary.Indigo500,
                Accent.Teal200, TextShade.WHITE);

            BackColor = BgMain;
            _pages = new[] { _pgDash, _pgStud, _pgCrs, _pgGrd, _pgAtt };

            BuildSidebar();
            BuildContentShell();
            BuildDashboardPage();
            // Create isolated BindingSources to shield DGV from background thread resets
            var bsStud = new BindingSource(); _dgvStud.DataSource = bsStud;
            var bsCrs = new BindingSource(); _dgvCrs.DataSource = bsCrs;
            var bsGrd = new BindingSource(); _dgvGrd.DataSource = bsGrd;
            var bsAtt = new BindingSource(); _dgvAtt.DataSource = bsAtt;

            BuildSubPage(_pgStud, _dgvStud, bsStud, 1);
            BuildSubPage(_pgCrs, _dgvCrs, bsCrs, 2);
            BuildSubPage(_pgGrd, _dgvGrd, bsGrd, 3);
            BuildSubPage(_pgAtt, _dgvAtt, bsAtt, 4);

            // Clock timer
            var clk = new System.Windows.Forms.Timer { Interval = 1000 };
            clk.Tick += (_, _) => _lblClock.Text = DateTime.Now.ToString("dddd, dd MMM yyyy   HH:mm:ss");
            clk.Start();

            FormClosed += (_, _) => Application.Exit();
            Shown += (_, _) => SetPage(0);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Sidebar
        // ─────────────────────────────────────────────────────────────────────
        private void BuildSidebar()
        {
            _pnlSidebar.Width = 240;
            _pnlSidebar.Dock = DockStyle.Left;
            _pnlSidebar.BackColor = BgSide;

            // ── Header block (painted) ────────────────────────────────────────
            var hdr = new Panel { Height = 110, BackColor = Indigo };
            hdr.Paint += (_, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using var f1 = new Font("Segoe UI", 26F, FontStyle.Bold);
                using var f2 = new Font("Segoe UI", 8F);
                using var w = new SolidBrush(Color.White);
                using var lw = new SolidBrush(Color.FromArgb(200, 220, 255));
                g.DrawString("SMS", f1, w, 60, 20);
                g.DrawString("Student Management", f2, lw, 52, 74);
                g.DrawString("System", f2, lw, 92, 86);
            };

            // ── Sign-out button ───────────────────────────────────────────────
            var btnOut = new MaterialButton
            {
                Text = "SIGN OUT",
                Dock = DockStyle.Bottom,
                Height = 52,
                AutoSize = false
            };
            btnOut.Click += (_, _) =>
            {
                this.Hide();
                foreach (Form f in Application.OpenForms)
                    if (f is LoginForm) { f.Show(); return; }
                Application.Exit();
            };

            // ── Separator line ────────────────────────────────────────────────
            var sep = new Panel
            {
                Height = 1,
                Dock = DockStyle.Bottom,
                BackColor = Color.FromArgb(40, 44, 70)
            };

            // ── Nav FlowLayoutPanel ───────────────────────────────────────────
            var flNav = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = BgSide,
                Padding = new Padding(0, 12, 0, 0)
            };

            var navDefs = new (string icon, string label)[]
            {
                ("⊞", "Dashboard"),
                ("♟", "Students"),
                ("☰", "Courses"),
                ("★", "Grades"),
                ("◉", "Attendance"),
            };

            _navBtns = new NavButton[navDefs.Length];
            for (int i = 0; i < navDefs.Length; i++)
            {
                int idx = i;
                _navBtns[i] = new NavButton
                { NavIcon = navDefs[i].icon, NavLabel = navDefs[i].label, Width = 240 };
                _navBtns[i].Click += (_, _) => SetPage(idx);
                flNav.Controls.Add(_navBtns[i]);
            }

            // ── Add to sidebar (order matters for DockStyle layout) ───────────
            // Processed in REVERSE add-order: hdr last-added = processed first = top
            _pnlSidebar.Controls.Add(sep);      // index 0 – bottom, inner (above btnOut)
            _pnlSidebar.Controls.Add(btnOut);   // index 1 – bottom, outer (very bottom)
            _pnlSidebar.Controls.Add(flNav);    // index 2 – fill remaining middle
            _pnlSidebar.Controls.Add(hdr);      // index 3 – top, outer (very top) ✓

            Controls.Add(_pnlContent);   // Fill – added first (processed last)
            Controls.Add(_pnlSidebar);   // Left – added last (processed first) ✓
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Content shell (top-bar + page swap area)
        // ─────────────────────────────────────────────────────────────────────
        private void BuildContentShell()
        {
            _pnlContent.Dock = DockStyle.Fill;
            _pnlContent.BackColor = BgMain;

            // ── Top bar ───────────────────────────────────────────────────────
            _pnlTopBar.Height = 72;
            _pnlTopBar.BackColor = BgTop;
            _pnlTopBar.Paint += (_, e) =>
            {
                using var p = new Pen(Color.FromArgb(40, 44, 70));
                e.Graphics.DrawLine(p, 0, _pnlTopBar.Height - 1, _pnlTopBar.Width, _pnlTopBar.Height - 1);
            };

            _lblTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            _lblTitle.ForeColor = TextMain;
            _lblTitle.BackColor = Color.Transparent;
            _lblTitle.AutoSize = true;
            _lblTitle.Location = new Point(28, 18);

            _lblClock.Font = new Font("Segoe UI", 10F);
            _lblClock.ForeColor = TextSub;
            _lblClock.BackColor = Color.Transparent;
            _lblClock.AutoSize = true;
            _lblClock.Text = DateTime.Now.ToString("dddd, dd MMM yyyy   HH:mm:ss");
            _pnlTopBar.SizeChanged += (_, _) =>
                _lblClock.Location = new Point(_pnlTopBar.Width - _lblClock.Width - 28, 26);

            _pnlTopBar.Controls.Add(_lblTitle);
            _pnlTopBar.Controls.Add(_lblClock);

            // Add Refresh button to Top Bar
            var btnRefreshAll = new MaterialButton
            {
                Text = "REFRESH DASHBOARD",
                AutoSize = false,
                Width = 180,
                Height = 36,
                Type = MaterialButton.MaterialButtonType.Outlined,
                Location = new Point(0, 18)
            };
            btnRefreshAll.Click += (_, _) => { LoadStats(); SetPage(0); };
            _pnlTopBar.Controls.Add(btnRefreshAll);
            _pnlTopBar.SizeChanged += (_, _) =>
                btnRefreshAll.Location = new Point(_lblClock.Left - btnRefreshAll.Width - 20, 18);

            // ── Page panels (Fill) added before top-bar so top-bar wins top slot
            foreach (var pg in _pages)
            {
                pg.Dock = DockStyle.Fill; pg.BackColor = BgMain; pg.Visible = false;
                _pnlContent.Controls.Add(pg);    // index 0-4 (processed last – fill)
            }
            _pnlContent.Controls.Add(_pnlTopBar); // index 5 (processed first – top) ✓
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Dashboard page (stat cards + recent grid)
        // ─────────────────────────────────────────────────────────────────────
        private void BuildDashboardPage()
        {
            _pgDash.Padding = new Padding(28, 20, 28, 20);

            // ── 0. Recent Students Grid (Fill - Processed LAST) ──
            _dgvRecent.Dock = DockStyle.Fill;
            StyleDgv(_dgvRecent);
            _pgDash.Controls.Add(_dgvRecent); // index 0

            // ── 1. Recent Students Section (Top) ──
            var lblRecent = new Label
            {
                Text = "Recent Students",
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = TextMain,
                BackColor = Color.Transparent,
                AutoSize = false,
                Height = 40,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.BottomLeft
            };
            _pgDash.Controls.Add(lblRecent); // index 1

            // ── 2. Charts Row (Top) ──
            var tlpCharts = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 320,
                RowCount = 1,
                ColumnCount = 2,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 10, 0, 20)
            };
            tlpCharts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45f));
            tlpCharts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55f));

            var pie = new SimplePieChart { Dock = DockStyle.Fill };
            var bar = new SimpleBarChart { Dock = DockStyle.Fill };
            tlpCharts.Controls.Add(pie, 0, 0);
            tlpCharts.Controls.Add(bar, 1, 0);
            _pgDash.Controls.Add(tlpCharts); // index 2

            // ── 3. Stat cards row (Top - Processed FIRST = Very Top) ──
            var tlpCards = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 148,
                RowCount = 1,
                ColumnCount = 5,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 0, 0, 16)
            };
            for (int i = 0; i < 5; i++)
                tlpCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20f));

            tlpCards.Controls.Add(MakeStatCard("Total Students", "🎓", Indigo, out _valStudents), 0, 0);
            tlpCards.Controls.Add(MakeStatCard("Total Courses", "📚", Teal, out _valCourses), 1, 0);
            tlpCards.Controls.Add(MakeStatCard("Grades Recorded", "★", Purple, out _valGrades), 2, 0);
            tlpCards.Controls.Add(MakeStatCard("Attendance Rate", "📅", Blue, out _valAttRate), 3, 0);
            tlpCards.Controls.Add(MakeStatCard("Average Score", "📊", Amber, out _valAvgScore), 4, 0);
            _pgDash.Controls.Add(tlpCards); // index 3 (Highest index = Topmost position)

            // Status info (sync time)
            var lblStatus = new Label
            {
                Text = "Synced: " + DateTime.Now.ToString("HH:mm"),
                Font = new Font("Segoe UI", 9F),
                ForeColor = TextSub,
                AutoSize = true,
                BackColor = Color.Transparent
            };
            _pgDash.Controls.Add(lblStatus);

            // Dynamic scaling on resize
            _pgDash.SizeChanged += (_, _) =>
            {
                lblStatus.Location = new Point(_pgDash.Width - lblStatus.Width - 28, lblRecent.Top + 14);

                // Scale fonts if maximized
                float scale = WindowState == FormWindowState.Maximized ? 1.2f : 1.0f;
                lblRecent.Font = new Font("Segoe UI", 13F * scale, FontStyle.Bold);
            };

            // Update LoadStats to fill charts
            _loadCharts = () =>
            {
                try
                {
                    using var db = new AppDbContext();

                    var deptData = db.Students
                        .GroupBy(s => s.Department)
                        .Select(g => new { Name = g.Key ?? "Unspecified", Count = (double)g.Count() })
                        .ToList()
                        .ToDictionary(x => x.Name, x => x.Count);
                    pie.Data = deptData;

                    var levelData = db.Students
                        .GroupBy(s => s.Level)
                        .Select(g => new
                        {
                            Name = g.Key ?? "Unknown",
                            Avg = g.SelectMany(s => s.Grades).Select(gr => gr.Score).DefaultIfEmpty(0).Average()
                        })
                        .ToList()
                        .ToDictionary(x => x.Name, x => x.Avg);
                    bar.Data = levelData;

                    pie.Invalidate();
                    bar.Invalidate();
                }
                catch { }
            };
        }

        private Action? _loadCharts;

        // ─────────────────────────────────────────────────────────────────────
        //  Stat card factory
        // ─────────────────────────────────────────────────────────────────────
        private Panel MakeStatCard(string title, string icon, Color accent, out Label valueLbl)
        {
            var card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = BgCard,
                Margin = new Padding(0, 0, 10, 0)
            };

            // Gradient background
            card.Paint += (s, e) =>
            {
                using var br = new LinearGradientBrush(card.ClientRectangle,
                    Color.FromArgb(35, 38, 62), Color.FromArgb(28, 30, 50), 90F);
                e.Graphics.FillRectangle(br, card.ClientRectangle);
            };

            var stripe = new Panel { Height = 4, Dock = DockStyle.Top, BackColor = accent };
            card.Controls.Add(stripe);

            // Faint oversized icon (top-right decorator)
            var iconLbl = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 26F),
                ForeColor = Color.FromArgb(50, accent.R, accent.G, accent.B),
                BackColor = Color.Transparent,
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(card.Width - 60, 15)
            };
            card.Controls.Add(iconLbl);

            // Big value
            valueLbl = new Label
            {
                Text = "—",
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = TextMain,
                BackColor = Color.Transparent,
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Location = new Point(14, 24)
            };
            card.Controls.Add(valueLbl);

            // Caption
            var capLbl = new Label
            {
                Text = title.ToUpper(),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = TextSub,
                BackColor = Color.Transparent,
                AutoSize = true,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                Location = new Point(16, 95)
            };
            card.Controls.Add(capLbl);

            // Force repositioning on resize
            card.SizeChanged += (s, e) =>
            {
                iconLbl.Location = new Point(card.Width - iconLbl.Width - 12, 12);
                capLbl.Location = new Point(16, card.Height - capLbl.Height - 15);
            };

            return card;
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Sub-pages (Students / Courses / Grades / Attendance)
        // ─────────────────────────────────────────────────────────────────────
        private void BuildSubPage(Panel page, DataGridView dgv, BindingSource bs, int pageIndex)
        {
            page.Padding = new Padding(28, 20, 28, 20);

            // Grid (Fill – processed last)
            dgv.Dock = DockStyle.Fill;
            StyleDgv(dgv);
            page.Controls.Add(dgv);      // index 0

            // Action toolbar (Top – processed first = at top)
            var toolbar = new Panel
            { Height = 48, Dock = DockStyle.Top, BackColor = Color.Transparent };

            var btnRefresh = new MaterialButton
            { Text = "⟳  REFRESH", AutoSize = false, Width = 130, Height = 38, Location = new Point(0, 5) };
            btnRefresh.Click += (_, _) => LoadSubData(bs, pageIndex);

            var btnMgmt = new MaterialButton
            {
                Text = "⚙  OPEN MANAGEMENT",
                HighEmphasis = true,
                AutoSize = false,
                Width = 220,
                Height = 38,
                Location = new Point(140, 5)
            };
            btnMgmt.Click += (_, _) =>
            {
                new SMS(pageIndex - 1).ShowDialog();
                LoadSubData(bs, pageIndex); // Automatically refresh grid when management closes
            };

            toolbar.Controls.Add(btnRefresh);
            toolbar.Controls.Add(btnMgmt);
            page.Controls.Add(toolbar);  // index 1 (highest – processed first = top) ✓
        }

        // PageIndexOf has been replaced by isolated lexical scoping.

        // ─────────────────────────────────────────────────────────────────────
        //  Navigation
        // ─────────────────────────────────────────────────────────────────────
        private void SetPage(int idx)
        {
            foreach (var p in _pages) p.Visible = false;
            foreach (var b in _navBtns) { b.IsActive = false; b.Invalidate(); }

            _pages[idx].Visible = true;
            _navBtns[idx].IsActive = true;
            _navBtns[idx].Invalidate();
            _lblTitle.Text = _pageTitles[idx];

            if (idx == 0) LoadStats();
            else LoadSubData(
                (BindingSource)(
                    idx == 1 ? _dgvStud.DataSource :
                    idx == 2 ? _dgvCrs.DataSource :
                    idx == 3 ? _dgvGrd.DataSource : _dgvAtt.DataSource
                )!, idx);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Data loading
        // ─────────────────────────────────────────────────────────────────────
        private void LoadStats()
        {
            try
            {
                using var db = new AppDbContext();
                _valStudents.Text = db.Students.Count().ToString();
                _valCourses.Text = db.Courses.Count().ToString();
                _valGrades.Text = db.Grades.Count().ToString();

                if (db.Attendances.Any())
                {
                    double rate = db.Attendances.Count(a => a.IsPresent) * 100.0
                                  / db.Attendances.Count();
                    _valAttRate.Text = $"{rate:0.0}%";
                }
                else _valAttRate.Text = "N/A";

                _valAvgScore.Text = db.Grades.Any()
                    ? $"{db.Grades.Average(g => g.Score):0.0}"
                    : "N/A";

                _loadCharts?.Invoke();

                _dgvRecent.DataSource = db.Students
                    .OrderByDescending(s => s.StudentId)
                    .Take(10)
                    .Select(s => new
                    {
                        s.StudentId,
                        s.Name,
                        Age = DateTime.Today.Year - s.BirthDate.Year,
                        s.Department
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Dashboard", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSubData(BindingSource bs, int idx)
        {
            try
            {
                using var db = new AppDbContext();
                bs.DataSource = idx switch
                {
                    1 => (object)db.Students
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
                             .ToList(),
                    2 => db.Courses
                             .Select(c => new { c.CourseId, c.Name, c.Code, c.Description, c.Credits })
                             .ToList(),
                    3 => db.Grades.Include(g => g.Student).Include(g => g.Course)
                             .Select(g => new
                             {
                                 g.GradeId,
                                 Student = g.Student.Name,
                                 Course = g.Course.Name,
                                 g.Score,
                                 LetterGrade = g.Score >= 90 ? "A" : g.Score >= 80 ? "B" : g.Score >= 70 ? "C" : g.Score >= 60 ? "D" : "F",
                                 g.Semester
                             })
                             .ToList(),
                    4 => db.Attendances.Include(a => a.Student).Include(a => a.Course)
                             .Select(a => new
                             {
                                 a.AttendanceId,
                                 Student = a.Student.Name,
                                 Course = a.Course.Name,
                                 a.Date,
                                 a.IsPresent,
                                 a.Notes
                             })
                             .ToList(),
                    _ => null!
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        //  DataGrid styling
        // ─────────────────────────────────────────────────────────────────────
        private static void StyleDgv(DataGridView dgv)
        {
            var hdr = Color.FromArgb(48, 63, 159);
            var bg = Color.FromArgb(28, 30, 50);
            var altBg = Color.FromArgb(33, 35, 58);
            var white = Color.White;
            var sel = Color.FromArgb(63, 81, 181);
            var bord = Color.FromArgb(40, 44, 70);

            dgv.BackgroundColor = bg;
            dgv.GridColor = bord;
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
            dgv.ColumnHeadersHeight = 44;
            dgv.RowTemplate.Height = 42;

            dgv.ColumnHeadersDefaultCellStyle.BackColor = hdr;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = white;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(12, 0, 0, 0);
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = hdr;
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = white;

            dgv.DefaultCellStyle.BackColor = bg;
            dgv.DefaultCellStyle.ForeColor = white;
            dgv.DefaultCellStyle.SelectionBackColor = sel;
            dgv.DefaultCellStyle.SelectionForeColor = white;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10.5F);
            dgv.DefaultCellStyle.Padding = new Padding(12, 4, 0, 4);

            dgv.AlternatingRowsDefaultCellStyle.BackColor = altBg;
            dgv.AlternatingRowsDefaultCellStyle.ForeColor = white;
            dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = sel;
            dgv.AlternatingRowsDefaultCellStyle.SelectionForeColor = white;
        }

        // ─────────────────────────────────────────────────────────────────────
        //  NavButton — custom-painted sidebar item
        // ─────────────────────────────────────────────────────────────────────
        private sealed class NavButton : Panel
        {
            private bool _hover;
            private float _transition = 0f; // 0.0 (normal) to 1.0 (hover)
            private readonly System.Windows.Forms.Timer _animTimer;
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public string NavIcon { get; set; } = "○";

            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public string NavLabel { get; set; } = "";

            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public bool IsActive { get; set; }

            public NavButton()
            {
                DoubleBuffered = true;
                Height = 58;
                Cursor = Cursors.Hand;

                _animTimer = new System.Windows.Forms.Timer { Interval = 16 }; // ~60fps
                _animTimer.Tick += (_, _) =>
                {
                    if (_hover && _transition < 1.0f) { _transition += 0.15f; if (_transition > 1f) _transition = 1f; Invalidate(); }
                    else if (!_hover && _transition > 0.0f) { _transition -= 0.15f; if (_transition < 0f) _transition = 0f; Invalidate(); }
                    else if (!_hover && _transition <= 0.0f || _hover && _transition >= 1.0f) _animTimer.Stop();
                };

                MouseEnter += (_, _) => { _hover = true; _animTimer.Start(); };
                MouseLeave += (_, _) => { _hover = false; _animTimer.Start(); };
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // Background color interpolation
                Color baseBg = IsActive ? Color.FromArgb(48, 63, 159) : Color.FromArgb(22, 22, 38);
                Color hoverBg = Color.FromArgb(38, 42, 68);

                int r = (int)(baseBg.R + (hoverBg.R - baseBg.R) * _transition);
                int gB = (int)(baseBg.G + (hoverBg.G - baseBg.G) * _transition);
                int b = (int)(baseBg.B + (hoverBg.B - baseBg.B) * _transition);

                using (var br = new SolidBrush(Color.FromArgb(r, gB, b)))
                    g.FillRectangle(br, 0, 0, Width, Height);

                // Active left indicator bar (cyan accent)
                if (IsActive)
                {
                    using var bar = new SolidBrush(Color.FromArgb(100, 220, 255));
                    g.FillRectangle(bar, 0, 12, 4, Height - 24);
                }

                var fmt = new StringFormat
                { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

                // Icon
                using var iconFont = new Font("Segoe UI Symbol", 14F);
                using var iconBr = new SolidBrush(IsActive ? Color.White : Color.FromArgb(130, 140, 200));
                g.DrawString(NavIcon, iconFont, iconBr, new RectangleF(12, 0, 36, Height), fmt);

                // Label
                using var lblFmt = new StringFormat
                { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                using var lblFont = new Font("Segoe UI", 11F, IsActive ? FontStyle.Bold : FontStyle.Regular);
                using var lblBr = new SolidBrush(IsActive ? Color.White : Color.FromArgb(150, 160, 210));
                g.DrawString(NavLabel, lblFont, lblBr, new RectangleF(55, 0, Width - 60, Height), lblFmt);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Custom Charts
        // ─────────────────────────────────────────────────────────────────────
        private class SimplePieChart : Panel
        {
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public Dictionary<string, double> Data { get; set; } = new();
            public SimplePieChart() { DoubleBuffered = true; BackColor = Color.Transparent; ResizeRedraw = true; }

            protected override void OnPaint(PaintEventArgs e)
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                var rect = new Rectangle(50, 50, Math.Min(Width, Height) - 100, Math.Min(Width, Height) - 100);
                if (Data.Count == 0) return;

                float total = (float)Data.Values.Sum();
                float startAngle = 0;
                Color[] colors = { Color.FromArgb(48, 63, 159), Color.FromArgb(0, 150, 136), Color.FromArgb(103, 58, 183), Color.FromArgb(21, 101, 192), Color.FromArgb(245, 124, 0) };

                int i = 0;
                foreach (var kvp in Data)
                {
                    float sweep = (float)(kvp.Value / total * 360);
                    using var br = new SolidBrush(colors[i % colors.Length]);
                    g.FillPie(br, rect, startAngle, sweep);

                    // Legend
                    using var f = new Font("Segoe UI", 9F);
                    using var bText = new SolidBrush(Color.FromArgb(160, 170, 210));
                    g.FillRectangle(br, Width - 140, 60 + i * 25, 12, 12);
                    g.DrawString($"{kvp.Key} ({kvp.Value})", f, bText, Width - 120, 58 + i * 25);

                    startAngle += sweep; i++;
                }
                using var fTitle = new Font("Segoe UI", 11F, FontStyle.Bold);
                using var bTitle = new SolidBrush(Color.White);
                g.DrawString("Department Distribution", fTitle, bTitle, 20, 10);
            }
        }

        private class SimpleBarChart : Panel
        {
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public Dictionary<string, double> Data { get; set; } = new();
            public SimpleBarChart() { DoubleBuffered = true; BackColor = Color.Transparent; ResizeRedraw = true; }

            protected override void OnPaint(PaintEventArgs e)
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                if (Data.Count == 0) return;

                float max = (float)Data.Values.Max(); if (max == 0) max = 100;
                int i = 0; int barW = 45; int gap = 30;
                int startX = 60; int bottom = Height - 60;

                foreach (var kvp in Data)
                {
                    int x = startX + i * (barW + gap);
                    float h = (float)(kvp.Value / max * (bottom - 60));
                    var r = new RectangleF(x, bottom - h, barW, h);

                    using var br = new LinearGradientBrush(r, Color.FromArgb(0, 150, 136), Color.FromArgb(0, 77, 64), 90F);
                    g.FillRectangle(br, r);

                    using var f = new Font("Segoe UI", 8F);
                    using var bText = new SolidBrush(Color.FromArgb(160, 170, 210));
                    g.DrawString(kvp.Key, f, bText, x - 5, bottom + 10);
                    g.DrawString($"{kvp.Value:0}", f, bText, x + 5, bottom - h - 20);
                    i++;
                }
                using var fTitle = new Font("Segoe UI", 11F, FontStyle.Bold);
                using var bTitle = new SolidBrush(Color.White);
                g.DrawString("Performance by Level", fTitle, bTitle, 20, 10);
            }
        }
    }
}
