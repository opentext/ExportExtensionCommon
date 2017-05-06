using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Linq;
using System.IO;
using Microsoft.Win32;
using System.Data;

namespace ExportExtensionCommon
{
    public partial class Capture : Form
    {
        public SIEEDefaultValues DefaultFieldValues { get; set; }
        public string DefaultDocument { get; set; }
        private Dictionary<string, Control> tb_dict = new Dictionary<string, Control>();
        private SIEESettings settings;
        private SIEEFieldlist schema;
        private SIEEBatch batch = new SIEEBatch();
        private string document;
        private Dictionary<DataGridView, DataTable> gridToTableMap = new Dictionary<DataGridView, DataTable>();

        public Capture(SIEESettings settings, SIEEFieldlist schema)
        {
            InitializeComponent();
            this.settings = settings;
            this.schema = schema;
            addFields(schema);
        }

        public DialogResult MyShowDialog()
        {
            btn_data.Enabled = DefaultFieldValues.ExtensionExists(settings.GetType().Name);
            document = DefaultDocument;
            lbl_docName.Text = Path.GetFileName(document);
            return ShowDialog();
        }

        public string GetDocument()
        {
            return document;
        }
        public SIEEBatch GetData() { return batch; }
        
        private SIEEFieldlist GetFieldlistFromUI()
        {
            TextBox tb;
            DataGridView dgv;
            SIEEFieldlist fl = new SIEEFieldlist(schema);
            SIEETableFieldRow tfr;

            foreach (SIEEField f in fl)
            {
                Control c = tb_dict[f.Name];
                if (c is TextBox)
                {
                    tb = (TextBox)c;
                    f.Value = tb.Text;
                    continue;
                }
                // else c is DataGridView
                dgv = (DataGridView)c;
                SIEETableField tf = (SIEETableField)f;
                DataTable table = gridToTableMap[dgv];
                foreach (DataRow row in table.Rows)
                {
                    tfr = new SIEETableFieldRow();
                    foreach (DataColumn col in table.Columns) 
                    {
                        tfr[col.ColumnName] =  row[col] is DBNull ? "" : (string)row[col]; 
                    }
                    tf.AddRow(tfr);
                }
            }
            return fl;
        }
   
        private void addFields (SIEEFieldlist fl)
        {
            Label l;
            TextBox tb;
            DataGridView dgv;
            DataTable table;
            int ypos = 0;
            const int labelwidth = 300;
            int labelheight;
            int entryheight;
            int gridheight;
            int cnt = 0;
            tb_dict.Clear();

            foreach (SIEEField f in fl)
            {
                l = new Label();
                l.Location = new System.Drawing.Point(0, ypos);
                l.Name = "label_" + f.Name;
                labelheight = l.Size.Height;
                l.Size = new System.Drawing.Size(labelwidth, labelheight);
                l.TabIndex = cnt;
                l.Text = f.Name;
                this.panel.Controls.Add(l);

                if (! (f is SIEETableField))
                {
                    tb = new TextBox();
                    tb.Location = new System.Drawing.Point(labelwidth + 30, ypos);
                    tb.Name = f.Name;
                    entryheight = tb.Size.Height > labelheight ? tb.Size.Height : labelheight;
                    tb.Size = new System.Drawing.Size(panel.Size.Width - (labelwidth + 30), entryheight);
                    tb.TabIndex = cnt;
                    this.panel.Controls.Add(tb);
                    tb_dict.Add(f.Name, tb);
                    tb.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;
                    ypos += entryheight;
                }
                else
                {
                    SIEETableField tf = (SIEETableField)f;
                    dgv = new DataGridView();
                    dgv.Location = new System.Drawing.Point(labelwidth + 30, ypos);
                    dgv.Name = tf.Name;
                    gridheight = dgv.Size.Height;
                    dgv.Size = new System.Drawing.Size(panel.Size.Width - (labelwidth + 30), gridheight);
                    dgv.TabIndex = cnt;
                    dgv.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    table = new DataTable();
                    gridToTableMap[dgv] = table;
                    dgv.DataSource = table;
                    foreach (SIEEField col in tf.Columns) { table.Columns.Add(col.Name); }
                    this.panel.Controls.Add(dgv);
                    tb_dict.Add(f.Name, dgv);
                    dgv.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;
                    //dgv.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.Fill);
                    ypos += 10 + gridheight;
                }
                cnt++;
            }
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btn_document_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "Document (*.pdf)|*.pdf";
            ofd.Title = "Select document";
            
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                document = ofd.FileName;
                lbl_docName.Text = Path.GetFileName(document);
            }
        }

        private void btn_data_Click(object sender, EventArgs e)
        {
            string type = settings.GetType().Name;
            DataRow row;

            foreach (Control c in this.panel.Controls)
            {
                if (!DefaultFieldValues.Exists(type, c.Name)) continue;
                if (c.GetType().Name == "TextBox")
                {
                    ((TextBox)c).Text = DefaultFieldValues.Get(type, c.Name);
                    continue;
                }
                // if c.GetType().Name = "DataGridView
                DataTable table = gridToTableMap[(DataGridView)c];
                table.Clear();
                foreach (string line in DefaultFieldValues.Get(type, c.Name).Split('\n'))
                {
                    int i = 0;
                    row = table.NewRow();
                    foreach (string v in line.Split(';')) { row[i++] = v.Trim(); }
                    table.Rows.Add(row);
                }
            }
        }

        private void btn_addDocument_Click(object sender, EventArgs e)
        {
            // rather complex loops to make document name unique within batch
            string doc = Path.GetFileNameWithoutExtension(document);
            bool done = false;
            while (!done)
            {
                done = true; int cnt = 0;
                foreach (SIEEDocument d in batch)
                {
                    if (Path.GetFileNameWithoutExtension(d.InputFileName) == doc)
                    {
                        doc = doc + String.Format("_{0:D4}", cnt++);
                        done = false;
                    }
                }
            }
            doc = Path.GetFileNameWithoutExtension(doc);
            batch.Add(new SIEEDocument {
                InputFileName = doc,
                PDFFileName = document,
                BatchId = "4711",
                DocumentId = batch.Count.ToString(),
                Fieldlist = GetFieldlistFromUI()
            });
            lbl_numberOfDocs.Text = batch.Count + " Documents loaded";
            btn_ok.Enabled = true;
        }
    }
}
