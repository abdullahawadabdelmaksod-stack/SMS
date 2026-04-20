namespace SMS
{
    partial class Dashboard
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode       = AutoScaleMode.Font;
            ClientSize          = new System.Drawing.Size(1440, 880);
            MinimumSize         = new System.Drawing.Size(1100, 680);
            Name                = "Dashboard";
            Text                = "SMS – Dashboard";
            Padding             = new Padding(4, 107, 4, 5);
            ResumeLayout(false);
        }
    }
}
