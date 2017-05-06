using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml.Serialization;

namespace ExportExtensionCommon
{
    [Serializable]
    [
        XmlInclude(typeof(SIEETableField)),
    ]
    public class SIEEFieldlist : Collection<SIEEField>, ICloneable
    {
        public SIEEFieldlist() 
        { 
        }

        public SIEEFieldlist(SIEEFieldlist fl) 
        {
            foreach (SIEEField f in fl)
            {
                if (f is SIEETableField)
                    Add(new SIEETableField((SIEETableField)f));
                else
                    Add(new SIEEField(f));
            }
        }

        public bool Exists(string name) 
        {
            return (this.GetFieldByName(name) != null);
        }

        public SIEEField GetFieldByName(string name)
        {
            return this.FirstOrDefault(n => n.Name == name);
        }

        public void SetField(string name, string value)
        {
            SIEEField f = this.GetFieldByName(name);
            if (f != null)
            {
                f.Value = value;
            }
        }

        public string ToString(bool data)
        {
            string res = "";
            foreach (SIEEField f in this)
            {
                if (res != "")
                    res += Environment.NewLine;

                res += (f is SIEETableField) ? (f as SIEETableField).ToString(data) : f.ToString(data);
            }
            return res;
        }
        
        public void MakeFieldnamesOCCCompliant()
        {
            foreach (SIEEField field in this)
            {
                field.Name = ConvertToLegalOCCFieldname(field.Name);
            }

            MakeFieldNamesUnique(this);
            foreach (SIEEField field in this)
            {
                if (field is SIEETableField)
                {
                    SIEETableField tablefield = (SIEETableField)field;
                    SIEEFieldlist columns = tablefield.Columns;
                    foreach (SIEEField f1 in columns) 
                    { 
                        f1.Name = ConvertToLegalOCCFieldname(f1.Name); 
                    }
                    MakeFieldNamesUnique(columns);
                    tablefield.Columns = columns;
                }
            }
        }

        public List<string> GetScalarFieldNames()
        {
            List<string> result = new List<string>();
            foreach (SIEEField f in this)
            {
                if (f is SIEETableField) continue;
                result.Add(f.Name);
            }
            return result;
        }

        public List<KeyValuePair<string,string>> ToKeyValuePairs()
        {
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();
            foreach (SIEEField f in this)
            {
                if (f is SIEETableField) continue;
                result.Add(new KeyValuePair<string, string>(f.Name, f.Value));
            }
            return result;
        }

        private void MakeFieldNamesUnique(SIEEFieldlist fields)
        {
            foreach (SIEEField field in fields)
            {
                int cnt = 0;
                foreach (SIEEField f1 in fields)
                {
                    if (field.Equals(f1)) 
                        continue;

                    if (field.Name == f1.Name) 
                        f1.Name += "_" + cnt++.ToString();
                }
            }
        }

        private static string ConvertToLegalOCCFieldname(string s)
        {
            string res = "";
            foreach (char c in Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(s)))
            {
                if (Char.IsLetter(c) || Char.IsDigit(c) || c == '_') res += c;
            }

            if (Char.IsDigit(res[0]))
                res = "_" + res;

            return res;
        } 

        public object Clone()
        {
            return new SIEEFieldlist(this);
        }
    }
}
