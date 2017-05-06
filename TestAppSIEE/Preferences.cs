using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ExportExtensionCommon
{
    public partial class Preferences : Form
    {
        public Preferences()
        {
            InitializeComponent();
            txt_settings.Text = Properties.Settings.Default.DefaultSettings;
            txt_values.Text = Properties.Settings.Default.DefaultValues;
            txt_document.Text = Properties.Settings.Default.DefaultDocument;
            btn_ok.Enabled = false;
         }

        private void btn_settings_Click(object sender, EventArgs e)
        {
            myFileDialog("Select default settings file", "Document (*.xml)|*.xml", txt_settings);
        }
        private void btn_values_Click(object sender, EventArgs e)
        {
            myFileDialog("Select default values file", "Document (*.xml)|*.xml", txt_values);
        }
        private void btn_document_Click(object sender, EventArgs e)
        {
            myFileDialog("Select default document", "Document (*.pdf)|*.pdf", txt_document);
        }
        private void myFileDialog(string title, string filter, TextBox tbox)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = filter;
            ofd.Title = title;
            if (ofd.ShowDialog() == DialogResult.OK) { tbox.Text = ofd.FileName; }
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.DefaultSettings = txt_settings.Text;
            Properties.Settings.Default.DefaultValues = txt_values.Text;
            Properties.Settings.Default.DefaultDocument = txt_document.Text;
            this.DialogResult = DialogResult.OK;
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void txt_settings_TextChanged(object sender, EventArgs e)
        {
            btn_ok.Enabled = true;
        }

        private void txt_values_TextChanged(object sender, EventArgs e)
        {
            btn_ok.Enabled = true;
        }

        private void txt_document_TextChanged(object sender, EventArgs e)
        {
            btn_ok.Enabled = true;
        }

        private void txt_occ_TextChanged(object sender, EventArgs e)
        {
            btn_ok.Enabled = true;
        }
    }
}
