using System;
using System.Windows.Forms;
using System.Drawing;

namespace ExportExtensionCommon
{
    public partial class Configure : Form
    {
        private SIEEControl control;

        public Configure()
        {
            InitializeComponent();
        }

        public DialogResult MyShowDialog()
        {
            SIEESettings settings = control.GetSettings();
            Type t = settings.GetType();
            return ShowDialog();
        }

        public void AddControl (SIEEControl ctrl)
        {
            control = ctrl;
            control.Location = new System.Drawing.Point(0, 0);
            control.Name = "SIEEControl";
            control.TabIndex = 1;
            control.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
            control.Dock = DockStyle.Fill;
            control.BackColor = Color.FromName("LightGray"); // Remove later
            this.panel.Controls.Add(control);
        }

        public SIEESettings Settings
        {
            get { return control.GetSettings(); }
            set { control.SetSettings(value); }
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

    }
}
