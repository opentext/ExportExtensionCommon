using System;
using System.Collections.Generic;
using RightDocs.Common;
using System.Drawing;

namespace ExportExtensionCommon
{
    public class EECExportDestination : CustomExportDestination, IDisposable
    {
        public List<CustomExportDestinationField> Fields { get; set; }
        protected SIEEFactory factory;
        protected SIEEDescription description;
        protected EECWriterSettings settings;
        protected SIEEWriterControl control;

        public EECExportDestination()
        {
            Fields = new List<CustomExportDestinationField>();
        }
        protected void Initialize(SIEEFactory f)
        {
            factory = f;
            SIEEFactoryManager.Add(factory);
            settings = new EECWriterSettings();
            settings.SetFactory(factory);
            description = f.CreateDescription();
            control = new SIEEWriterControl(factory);
        }
        public override CustomExportDestinationSettings Settings
        {
            get { return settings; }
            set
            { 
                settings = value as EECWriterSettings;
                settings.SetFactory(factory);
            }
        }
        public override System.Windows.Forms.Control GetUi()
        {
            control.ExportDestinationSettings = settings;
            return control;
        }
        public override void Reload()
        {
            Fields.Clear();

            EECWriterSettings writerSettings = (EECWriterSettings)control.ExportDestinationSettings;

            SIEEFieldlist fieldlist = writerSettings.CreateSchema();

            if (fieldlist == null) 
                throw (new Exception("No valid definition to create schema"));

            foreach (SIEEField field in fieldlist)
            {
                if (field is SIEETableField)
                {
                    CustomExportDestinationTable newTable = new CustomExportDestinationTable(field.Name);
                    newTable.ExternalId = field.ExternalId;
                    foreach (SIEEField columnField in ((SIEETableField)field).Columns)
                    {
                        CustomExportDestinationField newCol = new CustomExportDestinationField(columnField.Name);
                        newCol.ExternalId = columnField.ExternalId;
                        newTable.Fields.Add(newCol);
                    }
                    Fields.Add(newTable);
                }
                else
                {
                    CustomExportDestinationField newField = new CustomExportDestinationField(field.Name);
                    newField.ExternalId = field.ExternalId;
                    Fields.Add(newField);
                }
            }
        }
        public override List<CustomExportDestinationField> GetFields() { return Fields; }
        public override Type ExportModule { get { return typeof(SIEEWriterExport); } }
        
        // Properties and methods that derive their values from the description object
        public override string TypeName { get { return description.TypeName; } }
        public override string DefaultNewName { get { return description.DefaultNewName; } }
        public override bool SupportsFields { get { return description.SupportsFields; } }
        public override bool SupportsTables { get { return description.SupportsTables; } }
        public override bool SupportsMapping { get { return description.SupportsMapping; } }
        public override bool SupportsReload { get { return description.SupportsReload; } }
        public override bool SupportsCancel { get { return description.SupportsReload; } }
        public override Image Image { get { return description.Image; } }

        public override object Backup()
        {
            EECExportDestination clone = base.Backup() as EECExportDestination;
            SIEESettings s = (SIEESettings)SIEESerializer.Clone(settings.GetEmbeddedSettings());
            clone.settings.SetEmbeddedSettings(s);
            return clone;
        }
        public override void RestoreFrom(object o)
        {
            EECExportDestination backupExportDestination = o as EECExportDestination;
            base.RestoreFrom(backupExportDestination as CustomExportDestination);
            this.settings.SetEmbeddedSettings(backupExportDestination.settings.GetEmbeddedSettings());
        }
        public override string GetLocation()
        {
            return description.GetLocation(settings.GetEmbeddedSettings());
        }

        public override void OpenLocation()
        {
            if (settings == null) return;
            description.OpenLocation(GetLocation());
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) control.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
