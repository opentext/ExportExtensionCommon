namespace ExportExtensionCommon
{
    partial class Preferences
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_settings = new System.Windows.Forms.Button();
            this.lbl_settings = new System.Windows.Forms.Label();
            this.txt_settings = new System.Windows.Forms.TextBox();
            this.txt_values = new System.Windows.Forms.TextBox();
            this.lbl_values = new System.Windows.Forms.Label();
            this.btn_values = new System.Windows.Forms.Button();
            this.btn_cancel = new System.Windows.Forms.Button();
            this.btn_ok = new System.Windows.Forms.Button();
            this.txt_document = new System.Windows.Forms.TextBox();
            this.lbl_document = new System.Windows.Forms.Label();
            this.btn_document = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_settings
            // 
            this.btn_settings.Location = new System.Drawing.Point(577, 31);
            this.btn_settings.Margin = new System.Windows.Forms.Padding(4);
            this.btn_settings.Name = "btn_settings";
            this.btn_settings.Size = new System.Drawing.Size(31, 28);
            this.btn_settings.TabIndex = 0;
            this.btn_settings.Text = "...";
            this.btn_settings.UseVisualStyleBackColor = true;
            this.btn_settings.Click += new System.EventHandler(this.btn_settings_Click);
            // 
            // lbl_settings
            // 
            this.lbl_settings.AutoSize = true;
            this.lbl_settings.Location = new System.Drawing.Point(32, 37);
            this.lbl_settings.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_settings.Name = "lbl_settings";
            this.lbl_settings.Size = new System.Drawing.Size(128, 17);
            this.lbl_settings.TabIndex = 1;
            this.lbl_settings.Text = "Default settings file";
            // 
            // txt_settings
            // 
            this.txt_settings.Location = new System.Drawing.Point(233, 34);
            this.txt_settings.Margin = new System.Windows.Forms.Padding(4);
            this.txt_settings.Name = "txt_settings";
            this.txt_settings.Size = new System.Drawing.Size(319, 22);
            this.txt_settings.TabIndex = 2;
            this.txt_settings.TextChanged += new System.EventHandler(this.txt_settings_TextChanged);
            // 
            // txt_values
            // 
            this.txt_values.Location = new System.Drawing.Point(233, 81);
            this.txt_values.Margin = new System.Windows.Forms.Padding(4);
            this.txt_values.Name = "txt_values";
            this.txt_values.Size = new System.Drawing.Size(319, 22);
            this.txt_values.TabIndex = 5;
            this.txt_values.TextChanged += new System.EventHandler(this.txt_values_TextChanged);
            // 
            // lbl_values
            // 
            this.lbl_values.AutoSize = true;
            this.lbl_values.Location = new System.Drawing.Point(32, 84);
            this.lbl_values.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_values.Name = "lbl_values";
            this.lbl_values.Size = new System.Drawing.Size(127, 17);
            this.lbl_values.TabIndex = 4;
            this.lbl_values.Text = "Default values file";
            // 
            // btn_values
            // 
            this.btn_values.Location = new System.Drawing.Point(577, 78);
            this.btn_values.Margin = new System.Windows.Forms.Padding(4);
            this.btn_values.Name = "btn_values";
            this.btn_values.Size = new System.Drawing.Size(31, 28);
            this.btn_values.TabIndex = 3;
            this.btn_values.Text = "...";
            this.btn_values.UseVisualStyleBackColor = true;
            this.btn_values.Click += new System.EventHandler(this.btn_values_Click);
            // 
            // btn_cancel
            // 
            this.btn_cancel.Location = new System.Drawing.Point(533, 199);
            this.btn_cancel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(75, 27);
            this.btn_cancel.TabIndex = 7;
            this.btn_cancel.Text = "Cancel";
            this.btn_cancel.UseVisualStyleBackColor = true;
            this.btn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
            // 
            // btn_ok
            // 
            this.btn_ok.Enabled = false;
            this.btn_ok.Location = new System.Drawing.Point(439, 199);
            this.btn_ok.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_ok.Name = "btn_ok";
            this.btn_ok.Size = new System.Drawing.Size(75, 27);
            this.btn_ok.TabIndex = 6;
            this.btn_ok.Text = "Ok";
            this.btn_ok.UseVisualStyleBackColor = true;
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
            // 
            // txt_document
            // 
            this.txt_document.Location = new System.Drawing.Point(233, 132);
            this.txt_document.Margin = new System.Windows.Forms.Padding(4);
            this.txt_document.Name = "txt_document";
            this.txt_document.Size = new System.Drawing.Size(319, 22);
            this.txt_document.TabIndex = 10;
            this.txt_document.TextChanged += new System.EventHandler(this.txt_document_TextChanged);
            // 
            // lbl_document
            // 
            this.lbl_document.AutoSize = true;
            this.lbl_document.Location = new System.Drawing.Point(32, 135);
            this.lbl_document.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_document.Name = "lbl_document";
            this.lbl_document.Size = new System.Drawing.Size(119, 17);
            this.lbl_document.TabIndex = 9;
            this.lbl_document.Text = "Default document";
            // 
            // btn_document
            // 
            this.btn_document.Location = new System.Drawing.Point(577, 129);
            this.btn_document.Margin = new System.Windows.Forms.Padding(4);
            this.btn_document.Name = "btn_document";
            this.btn_document.Size = new System.Drawing.Size(31, 28);
            this.btn_document.TabIndex = 8;
            this.btn_document.Text = "...";
            this.btn_document.UseVisualStyleBackColor = true;
            this.btn_document.Click += new System.EventHandler(this.btn_document_Click);
            // 
            // Preferences
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(630, 244);
            this.Controls.Add(this.txt_document);
            this.Controls.Add(this.lbl_document);
            this.Controls.Add(this.btn_document);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.btn_ok);
            this.Controls.Add(this.txt_values);
            this.Controls.Add(this.lbl_values);
            this.Controls.Add(this.btn_values);
            this.Controls.Add(this.txt_settings);
            this.Controls.Add(this.lbl_settings);
            this.Controls.Add(this.btn_settings);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::ExportExtensionCommon.Properties.Settings.Default, "PreferencesLocation", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Location = global::ExportExtensionCommon.Properties.Settings.Default.PreferencesLocation;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Preferences";
            this.Text = "SIEE Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_settings;
        private System.Windows.Forms.Label lbl_settings;
        private System.Windows.Forms.TextBox txt_settings;
        private System.Windows.Forms.TextBox txt_values;
        private System.Windows.Forms.Label lbl_values;
        private System.Windows.Forms.Button btn_values;
        private System.Windows.Forms.Button btn_cancel;
        private System.Windows.Forms.Button btn_ok;
        private System.Windows.Forms.TextBox txt_document;
        private System.Windows.Forms.Label lbl_document;
        private System.Windows.Forms.Button btn_document;
    }
}
