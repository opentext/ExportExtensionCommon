using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

#if ProcessSuite
using CaptureCenter.ProcessSuite;
#endif
#if CMIS
using CaptureCenter.CMIS;
#endif
#if SQL
using CaptureCenter.SqlEE;
#endif
#if EDOCS
using CaptureCenter.eDocs;
#endif
#if HELLO_WORLD
using CaptureCenter.HelloWorld;
#endif
#if SPO
using CaptureCenter.SPO;
#endif
#if AX
using CaptureCenter.ApplicationExtender;
#endif

namespace ExportExtensionCommon
{
    public partial class Main : Form
    {
        #region Construction
        private SIEEControl control;
        private SIEESettings settings;
        private SIEEExport export;
        private SIEEDescription description;

        private SIEEFieldlist schema;
        private SIEEBatch batch;
        private string document;

        private SIEEDefaultValues defaultSettings = null;
        private SIEEDefaultValues defaultFieldValues = null;
        private string defaultDocument;

        Dictionary<string, string> factoryNameMap = new Dictionary<string, string>();

        public Main()
        {
            InitializeComponent();
        }
        #endregion

        #region Form load and close
        private void Main_Load(object sender, EventArgs e)
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            if (!Directory.Exists(basePath))
                return;

#if ProcessSuite
            SIEEFactoryManager.Add(new ProcessSuiteFactory());
#endif
#if CMIS
            SIEEFactoryManager.Add(new CMISFactory());
#endif
#if SQL
            SIEEFactoryManager.Add(new SqlEEFactory());
#endif
#if EDOCS
            SIEEFactoryManager.Add(new eDocsFactory());
#endif
#if HELLO_WORLD
            SIEEFactoryManager.Add(new HelloWorldFactory());
#endif
#if SPO
            SIEEFactoryManager.Add(new SPOFactory());
#endif
#if AX
            SIEEFactoryManager.Add(new AXFactory());
#endif

            foreach (string ext in SIEEFactoryManager.GetKeysFromTypeName())
            {
                string name = ext.Split('.').Last();
                cbox_extensionSelector.Items.Add(name);
                factoryNameMap[name] = ext;
            }
            cbox_extensionSelector.Text = Properties.Settings.Default.CurrentExtension;

            if (Properties.Settings.Default.MainSize.Width != 0)
                Size = Properties.Settings.Default.MainSize;

            if (Properties.Settings.Default.MaintRichTextFont != null)
                richTextBox_settings.Font = Properties.Settings.Default.MaintRichTextFont;

            richTextBox_settings.ForeColor = Properties.Settings.Default.MainRichTextColor;

            setDefaults();
        }

        private void Main_Closing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.MainSize = Size;
            Properties.Settings.Default.Save();
        }
        #endregion

        #region Defaults
        private void setDefaults()
        {
            defaultSettings = new SIEEDefaultValues();

            if (!string.IsNullOrEmpty(Properties.Settings.Default.DefaultSettings))
            {
                try 
                { 
                    defaultSettings.Initialize(Properties.Settings.Default.DefaultSettings); 
                }
                catch (Exception e)
                {
                    MessageBox.Show("Could not load default settings from "
                        + Properties.Settings.Default.DefaultSettings + ":\n" + e.Message);
                    defaultSettings = null;
                }
            }

            defaultFieldValues = new SIEEDefaultValues();
            
            if (!string.IsNullOrEmpty(Properties.Settings.Default.DefaultValues))
            {
                try { defaultFieldValues.Initialize(Properties.Settings.Default.DefaultValues); }
                catch (Exception e)
                {
                    MessageBox.Show("Could not load default values from "
                        + Properties.Settings.Default.DefaultValues + ":\n" + e.Message);
                    defaultFieldValues = null;
                }
            }
            defaultDocument = Properties.Settings.Default.DefaultDocument;
        }
        #endregion

        #region Configure
        private void btn_configure_Click(object sender, EventArgs e)
        {
            Configure confDlg = new Configure();
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                cbox_extensionSelector_SelectedIndexChanged(null, null);
        
                confDlg.AddControl(control);
                control.Size = confDlg.Size;

                // Simulate serialization by SIEE and by OCC
                confDlg.Settings = (SIEESettings)SIEESerializer.Clone(settings);
                string xmlString = Serializer.SerializeToXmlString(settings, System.Text.Encoding.Unicode);

                if (Properties.Settings.Default.ConfigSize.Width != 0)
                    confDlg.Size = Properties.Settings.Default.ConfigSize;

                if (confDlg.MyShowDialog() == DialogResult.OK)
                {
                    settings = confDlg.Settings;
                    try
                    {
                        schema = settings.CreateSchema();
                        schema.MakeFieldnamesOCCCompliant();
                        verifySchema(schema);
                    }
                    catch (Exception e1)
                    {
                        MessageBox.Show("Error loading configuration\n" + e1.Message);
                        return;
                    }
                    lbl_message.Text = "Settings and schema";
                    lbl_location.Text = description.GetLocation(settings);
                    richTextBox_settings.Text = settings.ToString() + Environment.NewLine +
                            "---------------" + Environment.NewLine +
                            schema.ToString(data: false) + Environment.NewLine +
                            "---------------" + Environment.NewLine +
                            "Location = " + description.GetLocation(settings);
                    btn_export.Enabled = false;
                    btn_capture.Enabled = true;
                    lbl_status.Text = "Configuration ready";

                    Properties.Settings.Default.ConfigSize = confDlg.Size;
                    saveCurrentSettings();
                }
            }
            finally { Cursor.Current = Cursors.Default; }
        }

        private void verifySchema(SIEEFieldlist schema)
        {
            if (schema.Where(n => n.ExternalId == null || n.ExternalId == string.Empty).Count() > 0)
                throw new Exception("ExternalID null or empty");
            if (schema.Select(n => n.ExternalId).Distinct().Count() != schema.Count())
                throw new Exception("ExternamIDs are not distinct");
        }
        #endregion

        #region Capture
        private void btn_capture_Click(object sender, EventArgs e)
        {
            Capture captureDlg = new Capture(settings, schema);
            captureDlg.DefaultFieldValues = defaultFieldValues;
            captureDlg.DefaultDocument = defaultDocument;
            if (Properties.Settings.Default.CaptuerSize.Width != 0)
                captureDlg.Size = Properties.Settings.Default.CaptuerSize;

            if (captureDlg.MyShowDialog() == DialogResult.OK)
            {
                btn_export.Enabled = true;
                lbl_message.Text = "Data";
                batch = captureDlg.GetData();
                document = captureDlg.GetDocument();
                richTextBox_settings.Text = batch.ToString();
                lbl_status.Text = batch.Count + " document(s) ready for export";
            }
            Properties.Settings.Default.CaptuerSize = captureDlg.Size;
            //captureDlg.Dispose();
        }
        #endregion

        #region Export
        private void btn_export_Click(object sender, EventArgs e)
        {
            if (description == null)
                return;

            if (chkbox_clear.Checked) 
                description.ClearLocation(settings);

            if (chbox_ignoreEmtpyFields.Checked)
            {
                List<SIEEField> toBeRemoved = new List<SIEEField>();
                foreach (SIEEDocument doc in batch)
                {
                    toBeRemoved = doc.Fieldlist.Where(n => !(n is SIEETableField) && n.Value == string.Empty).ToList();
                    foreach (SIEEField f in toBeRemoved)
                        doc.Fieldlist.Remove(f);
                }
            }

            Cursor.Current = Cursors.WaitCursor;
            DateTime startTime = DateTime.Now;
            export.ExportBatch(settings, batch);
            TimeSpan duration = DateTime.Now - startTime;
            Cursor.Current = Cursors.Default;

            string timeTakenString = "Time taken: " + duration.Milliseconds + " milliseconds";
            if (batch.ExportSucceeded())
                SIEEMessageBox.Show("Success!\n" + timeTakenString, 
                    "Export result", System.Windows.MessageBoxImage.None);
            else
                SIEEMessageBox.Show("Failed:\n" + batch.ErrorMessage() + "\n" + timeTakenString, 
                    "Export result", System.Windows.MessageBoxImage.Error);
            export.Term();
        }
        #endregion

        #region Other event handler
        private void cbox_extensionSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            string ext = cbox_extensionSelector.SelectedItem.ToString();
            loadExportExtention(SIEEFactoryManager.GetFromSettingsTypename(factoryNameMap[ext]));
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Simplified Interface for OCC Export Extension");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btn_exit_Click(sender, e);
        }

        private void openLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (description == null) return;
            description.OpenLocation(description.GetLocation(settings));
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Preferences preferencesDlg = new Preferences();
            if (preferencesDlg.ShowDialog() == DialogResult.OK)
            {
                setDefaults();
            }
        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            saveCurrentSettings();
            Application.Exit();
        }

        private void btn_font_Click(object sender, EventArgs e)
        {
            changeFont();
        }

        private void lbl_location_Click(object sender, EventArgs e)
        {
            if (description == null) return;
            description.OpenLocation(description.GetLocation(settings));
        }
        #endregion

        #region Functions
        private void loadExportExtention(SIEEFactory factory)
        {
            try
            {
                control = new SIEEControl(factory);
                settings = (SIEESettings)factory.CreateSettings();
                export = (SIEEExport)factory.CreateExport();
                description = (SIEEDescription)factory.CreateDescription();
            }
            catch (Exception ex) {  MessageBox.Show("Factory error.\n" + ex.Message);  }

            if (chbox_reloadConfiguration.Checked && Properties.Settings.Default.SavedConfigurationType == description.TypeName)
                try
                {
                    settings = (SIEESettings)SIEESerializer.StringToObject(Properties.Settings.Default.SavedConfiguration);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Loading saved configuration failed. Rason:\n" + e.Message);
                }
            btn_configure.Enabled = false;
            btn_capture.Enabled = false;
            btn_export.Enabled = false;

            try { SIEESerializer.StringToObject(SIEESerializer.ObjectToString(settings)); }
            catch (Exception ex)
            {
                MessageBox.Show("Serialization for settings object failed:\n" + ex.Message);
                cbox_extensionSelector.SelectedText = Properties.Settings.Default.CurrentExtension;
                return;
            }
            pict_Icon.Image = description.Image;
            btn_configure.Enabled = true;
            Properties.Settings.Default.CurrentExtension = cbox_extensionSelector.Text;
            lbl_status.Text = "Extension selected";
        }

        private void saveCurrentSettings()
        {
            if (description != null)
            {
                Properties.Settings.Default.SavedConfigurationType = description.TypeName;
                Properties.Settings.Default.SavedConfiguration = SIEESerializer.ObjectToString(settings);
            }
            Properties.Settings.Default.Save();
        }

        private void changeFont()
        {
            FontDialog dialog = new FontDialog();
            dialog.ShowColor = true;

            dialog.Font = richTextBox_settings.Font;
            dialog.Color = richTextBox_settings.ForeColor;

            if (dialog.ShowDialog() != DialogResult.Cancel)
            {
                richTextBox_settings.Font = dialog.Font;
                richTextBox_settings.ForeColor = dialog.Color;
                Properties.Settings.Default.MaintRichTextFont = dialog.Font;
                Properties.Settings.Default.MainRichTextColor = dialog.Color;
            }
        }
        #endregion
    }
}
