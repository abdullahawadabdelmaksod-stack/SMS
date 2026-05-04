using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SMS
{
    /// <summary>
    /// A custom WinForms panel that draws a professional circular ring-gauge
    /// with an animated fill arc, large centre value, caption, and sub-text.
    /// </summary>
    internal class RingGaugePanel : Panel
    {
        // ── Public properties ─────────────────────────────────────────────────
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Caption   { get; set; } = "";
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SubText   { get; set; } = "";
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color  Accent    { get; set; } = Color.FromArgb(0, 150, 136);
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double MaxValue  { get; set; } = 100.0;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool   IsPercent { get; set; } = false;

        // ── Animation state ───────────────────────────────────────────────────
        private double _targetValue   = 0;
        private double _currentValue  = 0;
        private System.Windows.Forms.Timer _animTimer = new();

        // ── Palette ───────────────────────────────────────────────────────────
        private static readonly Color BgCard  = Color.FromArgb(28, 30, 50);
        private static readonly Color TrackCl = Color.FromArgb(40, 44, 70);
        private static readonly Color TextMain= Color.White;
        private static readonly Color TextSub = Color.FromArgb(160, 170, 210);

        public RingGaugePanel()
        {
            DoubleBuffered = true;
            BackColor      = BgCard;

            _animTimer.Interval = 16; // ~60 fps
            _animTimer.Tick += (_, _) =>
            {
                double step = (_targetValue - _currentValue) * 0.12;
                if (Math.Abs(step) < 0.005) { _currentValue = _targetValue; _animTimer.Stop(); }
                else _currentValue += step;
                Invalidate();
            };
        }

        /// <summary>Sets a new value and starts the fill animation.</summary>
        public void SetValue(double value)
        {
            _targetValue = Math.Clamp(value, 0, MaxValue);
            _animTimer.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g  = e.Graphics;
            g.SmoothingMode      = SmoothingMode.AntiAlias;
            g.TextRenderingHint  = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var rc = ClientRectangle;
            if (rc.Width <= 4 || rc.Height <= 4) return;

            // ── Gradient card background ──────────────────────────────────────
            using var bgBrush = new LinearGradientBrush(rc,
                Color.FromArgb(35, 38, 62), Color.FromArgb(24, 26, 44), 90F);
            g.FillRectangle(bgBrush, rc);

            // ── Accent top stripe ─────────────────────────────────────────────
            using var stripe = new LinearGradientBrush(
                new Rectangle(0, 0, rc.Width, 3), Accent,
                Color.FromArgb(Accent.R / 2, Accent.G / 2, Accent.B / 2),
                LinearGradientMode.Horizontal);
            g.FillRectangle(stripe, 0, 0, rc.Width, 3);

            // ── Ring geometry ─────────────────────────────────────────────────
            int ringSize  = Math.Min(rc.Width, rc.Height) - 32;
            ringSize      = Math.Max(60, ringSize);
            int ringX     = (rc.Width  - ringSize) / 2;
            int ringY     = 12;
            int thickness = Math.Max(8, ringSize / 8);
            var ringRect  = new Rectangle(ringX, ringY, ringSize, ringSize);

            const float startAngle = 135f;
            const float totalSweep = 270f;

            // Track (background arc)
            using var trackPen = new Pen(TrackCl, thickness) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            g.DrawArc(trackPen, ringRect, startAngle, totalSweep);

            // Filled arc
            double pct    = MaxValue > 0 ? _currentValue / MaxValue : 0;
            float  sweep  = (float)(pct * totalSweep);
            if (sweep > 0.5f)
            {
                using var fillPen = new Pen(Accent, thickness) { StartCap = LineCap.Round, EndCap = LineCap.Round };
                // Gradient tint on the fill pen using a path
                using var fillPath = new GraphicsPath();
                fillPath.AddArc(ringRect, startAngle, sweep);
                using var pgBrush = new PathGradientBrush(fillPath);
                pgBrush.CenterColor    = Color.FromArgb(220, Accent);
                pgBrush.SurroundColors = new[] { Color.FromArgb(160, Accent) };
                g.DrawArc(fillPen, ringRect, startAngle, sweep);
            }

            // ── Centre value text ─────────────────────────────────────────────
            string valStr = IsPercent
                ? $"{_currentValue:F0}%"
                : $"{_currentValue:F2}";

            int centreX = ringX + ringSize / 2;
            int centreY = ringY + ringSize / 2;

            using var valFont = new Font("Segoe UI", Math.Max(14f, ringSize * 0.18f), FontStyle.Bold);
            using var valBrush = new SolidBrush(Accent);
            var valSz  = g.MeasureString(valStr, valFont);
            g.DrawString(valStr, valFont, valBrush,
                centreX - valSz.Width / 2, centreY - valSz.Height / 2 - 4);

            // ── Sub-text inside ring ──────────────────────────────────────────
            using var subFont  = new Font("Segoe UI", Math.Max(7f, ringSize * 0.09f));
            using var subBrush = new SolidBrush(TextSub);
            var subSz = g.MeasureString(SubText, subFont);
            g.DrawString(SubText, subFont, subBrush,
                centreX - subSz.Width / 2, centreY + valSz.Height / 2 - 6);

            // ── Caption below ring ────────────────────────────────────────────
            int capY = ringY + ringSize + 6;
            if (capY + 20 < rc.Height)
            {
                using var capFont  = new Font("Segoe UI", 10f, FontStyle.Bold);
                using var capBrush = new SolidBrush(TextMain);
                var capSz = g.MeasureString(Caption, capFont);
                g.DrawString(Caption, capFont, capBrush,
                    centreX - capSz.Width / 2, capY);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _animTimer.Dispose();
            base.Dispose(disposing);
        }
    }
}
