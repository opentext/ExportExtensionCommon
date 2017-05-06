using System;
using System.Collections.Generic;

namespace ExportExtensionCommon
{
    [Serializable]
    public class SIEEField: ICloneable
    {
        public string Name { get; set; }
        public string ExternalId { get; set; }
        public int Cardinality { get; set; }
        public string Value { get; set; }
        public List<string> ValueList { get; set; }

        public SIEEField() 
        {
            Cardinality = 0;
            ValueList = new List<string>();
        }

        public SIEEField (SIEEField f)
        {
            Name = f.Name;
            ExternalId = f.ExternalId;
            Value = f.Value;
            Cardinality = f.Cardinality;
            ValueList = f.ValueList;
        }

        public SIEEField(string name, string externalId, string val) 
        {
            Name = name;
            ExternalId = externalId;
            Value = val;
        }

        public virtual string ToString(bool data)
        {
            return (data) ?
                String.Format("{0,-15} \"{1}\"", Name + "=", Value) :
                String.Format("Name={0,-15} ExternalId={1}", Name, ExternalId.Substring(0,Math.Min(50, ExternalId.Length)));
        }

        public object Clone()
        {
            return new SIEEField(this);
        }
    }
 }
