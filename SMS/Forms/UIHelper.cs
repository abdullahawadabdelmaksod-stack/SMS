using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using MaterialSkin;

namespace SMS
{
    /// <summary>Shared UI utilities.</summary>
    internal static class UIHelper
    {
        /// <summary>
        /// Pre-renders the PictureBox image into a smooth anti-aliased circular Bitmap
        /// and replaces pic.Image with it.
        /// The output is opaque (card-background colour outside the circle) so there is
        /// never a transparency mismatch against the MaterialCard surface.
        /// </summary>
        public static void MakeCircular(PictureBox pic)
        {
            if (pic.Width <= 0 || pic.Height <= 0 || pic.Image == null)
                return;

            int side = Math.Min(pic.Width, pic.Height);   // ensure square crop

            // ── Card surface colour for the dark theme ────────────────────────────
            var skin = MaterialSkinManager.Instance;
            Color bg  = skin.Theme == MaterialSkinManager.Themes.DARK
                ? Color.FromArgb(45, 45, 54)    // approximate MaterialCard dark surface
                : Color.FromArgb(250, 250, 250);

            // ── Render source image clipped to a perfect circle ───────────────────
            var circular = new Bitmap(side, side, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(circular))
            {
                g.SmoothingMode      = SmoothingMode.AntiAlias;
                g.InterpolationMode  = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;

                // Fill background with the card colour (opaque — no transparency mismatch)
                g.Clear(bg);

                // Clip to ellipse and draw the source image
                using var path = new GraphicsPath();
                path.AddEllipse(0, 0, side, side);
                g.SetClip(path);
                g.DrawImage(pic.Image, new Rectangle(0, 0, side, side));
                g.ResetClip();

                // Anti-alias edge ring in bg colour
                using var pen = new Pen(Color.FromArgb(200, bg.R, bg.G, bg.B), 2.5f);
                g.DrawEllipse(pen, 1, 1, side - 2, side - 2);
            }

            var old = pic.Image;
            pic.Image = circular;
            old.Dispose();

            // BackColor matches the image background — no visible seam
            pic.BackColor = bg;
        }
    }
}
