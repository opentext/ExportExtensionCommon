using System;
using System.Collections.Generic;

namespace ExportExtensionCommon
{
    [Serializable]
    public class SIEETableField : SIEEField, ICloneable
    {
        public SIEEFieldlist Columns { get; set; }
//        private Dictionary<int, SIEETableFieldRow> tabValue = new Dictionary<int, SIEETableFieldRow>();
        private List<SIEETableFieldRow> tabValue = new List<SIEETableFieldRow>();

         public SIEETableField () 
        { 
            Columns = new SIEEFieldlist(); 
        }

        public SIEETableField (SIEETableField tf) : base(tf)
        {
            Columns = new SIEEFieldlist();
            foreach (SIEEField f in tf.Columns) 
            { 
                Columns.Add(new SIEEField(f)); 
            }
        }

        public SIEETableField(string name, string externalId, string val, SIEEFieldlist columns) 
            :base(name, externalId, val)
        {
            Columns = new SIEEFieldlist();
            foreach (SIEEField f in columns)
            {
                Columns.Add(new SIEEField(f));
            }
        }

        public void Clear() 
        { 
            tabValue.Clear();
        }
 
        public bool ColumnExists(string name) 
        { 
            return Columns.Exists(name);
        }

        public void AddRow(SIEETableFieldRow tr)
        {
            tabValue.Add(tr);
        }

        //private SIEETableFieldRow this[int i]
        //{
        //    get 
        //    { 
        //        return tabValue[i]; 
        //    }
        //    set 
        //    { 
        //        tabValue[i] = value; 
        //    }
        //}


        public List<SIEETableFieldRow> GetRows()
        {
            return  tabValue;
        }

        public void Set_tabValue(string txt)
        {
            List<string> idx2field = new List<string>();
            
            foreach (SIEEField f in Columns) 
            { 
                idx2field.Add(f.Name); 
            }

            Clear();
            SIEETableFieldRow row;

            foreach (string line in txt.Split('\n'))
            {
                row = new SIEETableFieldRow();
                int j = 0;
                foreach (string v in line.Split(';'))
                {
                    string name = idx2field[j];
                    row[name] = v.Trim();
                    j++;
                }
                tabValue.Add(row);
            }
        }

        public override string ToString(bool data)
        {
            String res = "";

            if (data)
            {
                res += Name + " ( ";
                
                foreach (SIEEField f in Columns) 
                    res += f.Name + " ";

                res += ") = " + Environment.NewLine + "[";

                foreach (SIEETableFieldRow row in GetRows())
                {
                    res += Environment.NewLine;
                    foreach (string key in row.Keys)
                    {
                        res += "\"" + row[key] + "\" ";
                    }
                }
            }
            else
            {
                res += String.Format("Name={0,-15} ExternalId={1}{2}[", Name, ExternalId, Environment.NewLine);
                foreach (SIEEField col in Columns)
                    res += String.Format("{0}\tName={1,-15} ExternalId={2}", Environment.NewLine, col.Name, col.ExternalId);
            }
            res += Environment.NewLine + "]";
            return res;
        }

        new object Clone()
        {
            return new SIEETableField(this);
        }
    }
}
