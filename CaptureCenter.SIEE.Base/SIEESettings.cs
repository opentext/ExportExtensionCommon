using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ExportExtensionCommon
{
    /// Base class for OCC settings objects
    [Serializable]
    public class SIEESettings : ICloneable
    {
        public SIEESettings() { }
        
        /// Creates a schema (fieldlist) and return a reference to it.
        /// Needs to be overridden in derived classes.
        public virtual SIEEFieldlist CreateSchema() { return null; }

        public SIEEFieldlist CreateSchemaAndRectifyFieldNames()
        {
            SIEEFieldlist schema = CreateSchema();
            schema.MakeFieldnamesOCCCompliant();
            return schema;
        }

        /// The document name specification string defines how the resulting document should be built.
        /// It contains elements like <Date> or <BatchId> that are substituted with concrete values.
        /// When the export destination does not support it, it should return null.
        /// Function needs to be overridden in derived classes.
        public virtual string GetDocumentNameSpec() { return null; }

        public override string ToString()
        {
            string res = "";
            foreach (var prop in GetType().GetProperties())
            {
                try {  res += prop.Name + " = " + prop.GetValue(this, null) + Environment.NewLine;  }
                catch { } // take care of set-only properties
            }
            return res; 
        }

        public virtual object Clone()
        {
            throw new Exception("Clone not overridden in SIEE settings");
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        protected void SendPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (this.PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetField<T>(ref T field, T value, [CallerMemberName]string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            field = value;
            RaisePropertyChanged(propertyName);
        }

        public void RaisePropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }

 }
