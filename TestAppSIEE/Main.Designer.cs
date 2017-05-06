namespace ExportExtensionCommon
{
    partial class Main
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.btn_configure = new System.Windows.Forms.Button();
            this.btn_capture = new System.Windows.Forms.Button();
            this.btn_export = new System.Windows.Forms.Button();
            this.lbl_message = new System.Windows.Forms.Label();
            this.toolTip_Configure = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip_Capture = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip_Export = new System.Windows.Forms.ToolTip(this.components);
            this.richTextBox_settings = new System.Windows.Forms.RichTextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openLocationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cbox_extensionSelector = new System.Windows.Forms.ToolStripComboBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lbl_status = new System.Windows.Forms.ToolStripStatusLabel();
            this.chkbox_clear = new System.Windows.Forms.CheckBox();
            this.btn_exit = new System.Windows.Forms.Button();
            this.btn_font = new System.Windows.Forms.Button();
            this.lbl_location = new System.Windows.Forms.Label();
            this.pict_Icon = new System.Windows.Forms.PictureBox();
            this.chbox_reloadConfiguration = new System.Windows.Forms.CheckBox();
            this.chbox_ignoreEmtpyFields = new System.Windows.Forms.CheckBox();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pict_Icon)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_configure
            // 
            resources.ApplyResources(this.btn_configure, "btn_configure");
            this.btn_configure.Name = "btn_configure";
            this.toolTip_Configure.SetToolTip(this.btn_configure, resources.GetString("btn_configure.ToolTip"));
            this.btn_configure.UseVisualStyleBackColor = true;
            this.btn_configure.Click += new System.EventHandler(this.btn_configure_Click);
            // 
            // btn_capture
            // 
            resources.ApplyResources(this.btn_capture, "btn_capture");
            this.btn_capture.Name = "btn_capture";
            this.toolTip_Capture.SetToolTip(this.btn_capture, resources.GetString("btn_capture.ToolTip"));
            this.btn_capture.UseVisualStyleBackColor = true;
            this.btn_capture.Click += new System.EventHandler(this.btn_capture_Click);
            // 
            // btn_export
            // 
            resources.ApplyResources(this.btn_export, "btn_export");
            this.btn_export.Name = "btn_export";
            this.toolTip_Export.SetToolTip(this.btn_export, resources.GetString("btn_export.ToolTip"));
            this.btn_export.UseVisualStyleBackColor = true;
            this.btn_export.Click += new System.EventHandler(this.btn_export_Click);
            // 
            // lbl_message
            // 
            resources.ApplyResources(this.lbl_message, "lbl_message");
            this.lbl_message.Name = "lbl_message";
            // 
            // richTextBox_settings
            // 
            resources.ApplyResources(this.richTextBox_settings, "richTextBox_settings");
            this.richTextBox_settings.Name = "richTextBox_settings";
            this.richTextBox_settings.ReadOnly = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.cbox_extensionSelector});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.preferencesToolStripMenuItem,
            this.openLocationToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // preferencesToolStripMenuItem
            // 
            this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            resources.ApplyResources(this.preferencesToolStripMenuItem, "preferencesToolStripMenuItem");
            this.preferencesToolStripMenuItem.Click += new System.EventHandler(this.preferencesToolStripMenuItem_Click);
            // 
            // openLocationToolStripMenuItem
            // 
            this.openLocationToolStripMenuItem.Name = "openLocationToolStripMenuItem";
            resources.ApplyResources(this.openLocationToolStripMenuItem, "openLocationToolStripMenuItem");
            this.openLocationToolStripMenuItem.Click += new System.EventHandler(this.openLocationToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // cbox_extensionSelector
            // 
            resources.ApplyResources(this.cbox_extensionSelector, "cbox_extensionSelector");
            this.cbox_extensionSelector.Name = "cbox_extensionSelector";
            this.cbox_extensionSelector.SelectedIndexChanged += new System.EventHandler(this.cbox_extensionSelector_SelectedIndexChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lbl_status});
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            // 
            // lbl_status
            // 
            this.lbl_status.Name = "lbl_status";
            resources.ApplyResources(this.lbl_status, "lbl_status");
            // 
            // chkbox_clear
            // 
            resources.ApplyResources(this.chkbox_clear, "chkbox_clear");
            this.chkbox_clear.Checked = global::ExportExtensionCommon.Properties.Settings.Default.ClearSetting;
            this.chkbox_clear.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkbox_clear.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::ExportExtensionCommon.Properties.Settings.Default, "ClearSetting", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkbox_clear.Name = "chkbox_clear";
            this.chkbox_clear.UseVisualStyleBackColor = true;
            // 
            // btn_exit
            // 
            resources.ApplyResources(this.btn_exit, "btn_exit");
            this.btn_exit.Name = "btn_exit";
            this.btn_exit.UseVisualStyleBackColor = true;
            this.btn_exit.Click += new System.EventHandler(this.btn_exit_Click);
            // 
            // btn_font
            // 
            resources.ApplyResources(this.btn_font, "btn_font");
            this.btn_font.Name = "btn_font";
            this.btn_font.UseVisualStyleBackColor = true;
            this.btn_font.Click += new System.EventHandler(this.btn_font_Click);
            // 
            // lbl_location
            // 
            resources.ApplyResources(this.lbl_location, "lbl_location");
            this.lbl_location.Name = "lbl_location";
            this.lbl_location.Click += new System.EventHandler(this.lbl_location_Click);
            // 
            // pict_Icon
            // 
            resources.ApplyResources(this.pict_Icon, "pict_Icon");
            this.pict_Icon.Name = "pict_Icon";
            this.pict_Icon.TabStop = false;
            // 
            // chbox_reloadConfiguration
            // 
            resources.ApplyResources(this.chbox_reloadConfiguration, "chbox_reloadConfiguration");
            this.chbox_reloadConfiguration.Checked = global::ExportExtensionCommon.Properties.Settings.Default.ReloadConfiguration;
            this.chbox_reloadConfiguration.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::ExportExtensionCommon.Properties.Settings.Default, "ReloadConfiguration", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chbox_reloadConfiguration.Name = "chbox_reloadConfiguration";
            this.chbox_reloadConfiguration.UseVisualStyleBackColor = true;
            // 
            // chbox_ignoreEmtpyFields
            // 
            resources.ApplyResources(this.chbox_ignoreEmtpyFields, "chbox_ignoreEmtpyFields");
            this.chbox_ignoreEmtpyFields.Name = "chbox_ignoreEmtpyFields";
            this.chbox_ignoreEmtpyFields.UseVisualStyleBackColor = true;
            // 
            // Main
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chbox_ignoreEmtpyFields);
            this.Controls.Add(this.chbox_reloadConfiguration);
            this.Controls.Add(this.pict_Icon);
            this.Controls.Add(this.lbl_location);
            this.Controls.Add(this.btn_font);
            this.Controls.Add(this.btn_exit);
            this.Controls.Add(this.chkbox_clear);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.richTextBox_settings);
            this.Controls.Add(this.lbl_message);
            this.Controls.Add(this.btn_export);
            this.Controls.Add(this.btn_capture);
            this.Controls.Add(this.btn_configure);
            this.Controls.Add(this.menuStrip1);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::ExportExtensionCommon.Properties.Settings.Default, "MainLocation", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Location = global::ExportExtensionCommon.Properties.Settings.Default.MainLocation;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Main";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_Closing);
            this.Load += new System.EventHandler(this.Main_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pict_Icon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_configure;
        private System.Windows.Forms.Button btn_capture;
        private System.Windows.Forms.Button btn_export;
        private System.Windows.Forms.Label lbl_message;
        private System.Windows.Forms.ToolTip toolTip_Configure;
        private System.Windows.Forms.ToolTip toolTip_Capture;
        private System.Windows.Forms.ToolTip toolTip_Export;
        private System.Windows.Forms.RichTextBox richTextBox_settings;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openLocationToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox cbox_extensionSelector;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lbl_status;
        private System.Windows.Forms.CheckBox chkbox_clear;
        private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem;
        private System.Windows.Forms.Button btn_exit;
        private System.Windows.Forms.Button btn_font;
        private System.Windows.Forms.Label lbl_location;
        private System.Windows.Forms.PictureBox pict_Icon;
        private System.Windows.Forms.CheckBox chbox_reloadConfiguration;
        private System.Windows.Forms.CheckBox chbox_ignoreEmtpyFields;
    }
}

