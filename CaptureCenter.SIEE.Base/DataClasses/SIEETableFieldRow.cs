using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ExportExtensionCommon
{
    [Serializable]
    public class SIEETableFieldRow : Dictionary<string,string>, ICloneable 
    {
        public SIEETableFieldRow() { }

        // CA2229: Implement serialization constructors
        protected SIEETableFieldRow(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public object Clone()
        {
            SIEETableFieldRow clone = new SIEETableFieldRow();
            foreach(KeyValuePair<string, string> pair in this)
            {
                clone.Add(pair.Key, pair.Value);
            }
            return clone;
        }
    }

}
