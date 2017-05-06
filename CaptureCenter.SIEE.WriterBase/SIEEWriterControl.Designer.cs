using System.Windows.Forms;

namespace ExportExtensionCommon
{
    partial class SIEEWriterControl
    {
        /// Required designer variable.
        private System.ComponentModel.IContainer components = null;

        /// Clean up any resources being used.
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) { components.Dispose(); }
            base.Dispose(disposing);
        }

        #region Component Designer generated code --> Modified !!

        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        private void InitializeComponent()
        {
            this.SuspendLayout();

           // EmbeddedControl
            this.embeddedControl.Location = new System.Drawing.Point(2, 2);
            this.embeddedControl.Padding = new System.Windows.Forms.Padding(2,2,2,2);
            this.embeddedControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.embeddedControl.Name = "EmbeddedControl";
            this.embeddedControl.TabIndex = 0;

            // SIEE_Control
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.embeddedControl);
            this.Name = "SIEE_Control";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion

    }
}
