namespace WorldStreetsRenderLayerSample
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.winformsMap = new ThinkGeo.MapSuite.WinForms.WinformsMap();
            this.SuspendLayout();
            // 
            // winformsMap
            // 
            this.winformsMap.BackColor = System.Drawing.Color.White;
            this.winformsMap.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.Default;
            this.winformsMap.CurrentScale = 590591790D;
            this.winformsMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.winformsMap.DrawingQuality = ThinkGeo.MapSuite.Drawing.DrawingQuality.Default;
            this.winformsMap.Location = new System.Drawing.Point(0, 0);
            this.winformsMap.MapFocusMode = ThinkGeo.MapSuite.WinForms.MapFocusMode.Default;
            this.winformsMap.MapResizeMode = ThinkGeo.MapSuite.Shapes.MapResizeMode.PreserveScale;
            this.winformsMap.MapUnit = ThinkGeo.MapSuite.GeographyUnit.DecimalDegree;
            this.winformsMap.MaximumScale = 80000000000000D;
            this.winformsMap.MinimumScale = 200D;
            this.winformsMap.Name = "winformsMap";
            this.winformsMap.Size = new System.Drawing.Size(919, 475);
            this.winformsMap.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
            this.winformsMap.TabIndex = 0;
            this.winformsMap.Text = "winformsMap";
            this.winformsMap.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            this.winformsMap.ThreadingMode = ThinkGeo.MapSuite.WinForms.MapThreadingMode.Default;
            this.winformsMap.ZoomLevelSnapping = ThinkGeo.MapSuite.WinForms.ZoomLevelSnappingMode.Default;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(919, 475);
            this.Controls.Add(this.winformsMap);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.FormLoad);
            this.ResumeLayout(false);

        }

        #endregion

        private ThinkGeo.MapSuite.WinForms.WinformsMap winformsMap;
    }
}

