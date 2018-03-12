namespace ExportExtensionCommon
{
    partial class Capture
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
            this.btn_cancel = new System.Windows.Forms.Button();
            this.btn_ok = new System.Windows.Forms.Button();
            this.btn_document = new System.Windows.Forms.Button();
            this.btn_data = new System.Windows.Forms.Button();
            this.toolTip_loadData = new System.Windows.Forms.ToolTip(this.components);
            this.btn_addDocument = new System.Windows.Forms.Button();
            this.lbl_numberOfDocs = new System.Windows.Forms.Label();
            this.lbl_docName = new System.Windows.Forms.Label();
            this.panel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // btn_cancel
            // 
            this.btn_cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_cancel.Location = new System.Drawing.Point(625, 505);
            this.btn_cancel.Margin = new System.Windows.Forms.Padding(2);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(56, 22);
            this.btn_cancel.TabIndex = 4;
            this.btn_cancel.Text = "Cancel";
            this.btn_cancel.UseVisualStyleBackColor = true;
            this.btn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
            // 
            // btn_ok
            // 
            this.btn_ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_ok.Enabled = false;
            this.btn_ok.Location = new System.Drawing.Point(554, 505);
            this.btn_ok.Margin = new System.Windows.Forms.Padding(2);
            this.btn_ok.Name = "btn_ok";
            this.btn_ok.Size = new System.Drawing.Size(56, 22);
            this.btn_ok.TabIndex = 3;
            this.btn_ok.Text = "Ok";
            this.btn_ok.UseVisualStyleBackColor = true;
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
            // 
            // btn_document
            // 
            this.btn_document.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_document.Location = new System.Drawing.Point(11, 494);
            this.btn_document.Margin = new System.Windows.Forms.Padding(2);
            this.btn_document.Name = "btn_document";
            this.btn_document.Size = new System.Drawing.Size(124, 33);
            this.btn_document.TabIndex = 5;
            this.btn_document.Text = "Select document...";
            this.btn_document.UseVisualStyleBackColor = true;
            this.btn_document.Click += new System.EventHandler(this.btn_document_Click);
            // 
            // btn_data
            // 
            this.btn_data.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_data.Location = new System.Drawing.Point(160, 494);
            this.btn_data.Margin = new System.Windows.Forms.Padding(2);
            this.btn_data.Name = "btn_data";
            this.btn_data.Size = new System.Drawing.Size(124, 33);
            this.btn_data.TabIndex = 6;
            this.btn_data.Text = "Load data...";
            this.toolTip_loadData.SetToolTip(this.btn_data, "Load data from an XML if you have.\r\nOtherwise type data in.");
            this.btn_data.UseVisualStyleBackColor = true;
            this.btn_data.Click += new System.EventHandler(this.btn_data_Click);
            // 
            // btn_addDocument
            // 
            this.btn_addDocument.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_addDocument.Location = new System.Drawing.Point(310, 494);
            this.btn_addDocument.Margin = new System.Windows.Forms.Padding(2);
            this.btn_addDocument.Name = "btn_addDocument";
            this.btn_addDocument.Size = new System.Drawing.Size(124, 33);
            this.btn_addDocument.TabIndex = 7;
            this.btn_addDocument.Text = "Add Document";
            this.toolTip_loadData.SetToolTip(this.btn_addDocument, "Load data from an XML if you have.\r\nOtherwise type data in.");
            this.btn_addDocument.UseVisualStyleBackColor = true;
            this.btn_addDocument.Click += new System.EventHandler(this.btn_addDocument_Click);
            // 
            // lbl_numberOfDocs
            // 
            this.lbl_numberOfDocs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbl_numberOfDocs.AutoSize = true;
            this.lbl_numberOfDocs.Location = new System.Drawing.Point(314, 475);
            this.lbl_numberOfDocs.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_numberOfDocs.Name = "lbl_numberOfDocs";
            this.lbl_numberOfDocs.Size = new System.Drawing.Size(108, 13);
            this.lbl_numberOfDocs.TabIndex = 8;
            this.lbl_numberOfDocs.Text = "0 Documents loaded ";
            // 
            // lbl_docName
            // 
            this.lbl_docName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbl_docName.AutoSize = true;
            this.lbl_docName.Location = new System.Drawing.Point(15, 475);
            this.lbl_docName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_docName.Name = "lbl_docName";
            this.lbl_docName.Size = new System.Drawing.Size(19, 13);
            this.lbl_docName.TabIndex = 9;
            this.lbl_docName.Text = "<>";
            // 
            // panel
            // 
            this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel.AutoScroll = true;
            this.panel.Location = new System.Drawing.Point(12, 12);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(670, 450);
            this.panel.TabIndex = 10;
            // 
            // Capture
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 542);
            this.Controls.Add(this.panel);
            this.Controls.Add(this.lbl_docName);
            this.Controls.Add(this.lbl_numberOfDocs);
            this.Controls.Add(this.btn_addDocument);
            this.Controls.Add(this.btn_data);
            this.Controls.Add(this.btn_document);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.btn_ok);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::ExportExtensionCommon.Properties.Settings.Default, "CaptureLocation", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            //this.Location = global::ExportExtensionCommon.Properties.Settings.Default.CaptureLocation;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(650, 200);
            this.Name = "Capture";
            this.Text = "Capture";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_cancel;
        private System.Windows.Forms.Button btn_ok;
        private System.Windows.Forms.Button btn_document;
        private System.Windows.Forms.Button btn_data;
        private System.Windows.Forms.ToolTip toolTip_loadData;
        private System.Windows.Forms.Button btn_addDocument;
        private System.Windows.Forms.Label lbl_numberOfDocs;
        private System.Windows.Forms.Label lbl_docName;
        private System.Windows.Forms.Panel panel;
    }
}