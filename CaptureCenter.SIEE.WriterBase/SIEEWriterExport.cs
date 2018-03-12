using System;
using System.Xml;

using RightDocs.Common;
using DOKuStar.Runtime;
using DOKuStar.Runtime.Processor;
using DOKuStar.Runtime.Sitemap;
using DOKuStar.Data.Xml;
using DOKuStar.Runtime.ScriptingHost;
using DOKuStar.Data.Xml.WorkflowExtensions;
using System.Collections.Generic;
using DOKuStar.Diagnostics.Tracing;

namespace ExportExtensionCommon
{
    /// The SIEEWriterExport basically doe this: It converts the incoming OCC batch into an SIEEBatch and
    /// calls the export function that is drawn from the factory. It also handles retries as defined by 
    /// properties from the destination.

    [ComponentSettings(typeof(EECWriterSettings))]
    [ComponentDescription("SIEE_Export", "Transformer", "OCC: SIEE Adapter", "OpenText")]
    public sealed class SIEEWriterExport : BaseTransformer, IConfigurable2<EECWriterSettings>, IDisposable
    {
        private EECWriterSettings writerSettings;

        // This list is used for unit testing. If it's empty, it is ignored
        // If if contains values, they are field names and only those fields get
        // values assigned. (From OCC document to SIEE document
        public List<string> FieldMapping4UnitTest { get; set; }

        public SIEEWriterExport() : base() 
        { 
            FieldMapping4UnitTest = new List<string>(); 
        }
        
        public void Configure(EECWriterSettings settings) 
        { 
            this.writerSettings = settings; 
        }

        public override XmlDocument transform(XmlDocument data, IParameters parameters)
        {
            // The SIEEBatch is created from the schema as defined in the setting object. It contains all
            // fields regardless of whether they have been mapped to OCC fields.
            SIEEFieldlist schema = (SIEEFieldlist)SIEESerializer.StringToObject(writerSettings.SerializedSchema);

            // This class has no initialization by which the factory could be set beforehand. We therefore 
            // load the factory from the SIEE_FactoryManager. (This was the only reason to invent the
            // SIEE_FactoryManager in the first place.

            SIEEFactory factory = SIEEFactoryManager.GetFromSettingsTypename(writerSettings.SettingsTypename);
            writerSettings.SetFactory(factory);

            // Create the SIEE objects wee need            
            SIEEExport myExport = factory.CreateExport();
            SIEEDescription description = factory.CreateDescription();

            DataPool pool = new DataPool(data);
            SIEEBatch batch = new SIEEBatch();
            int maxRetryCount = description.NumberOfRetries;
            string batchId = pool.RootNode.Fields["cc_BatchId"].Value;
            string profile = pool.RootNode.Fields["cc_ProfileName"].Value;

            SIEEExport.Trace.WriteInfo("Start exporting batch " + batchId);

            ExportStateParams exportStateParams = null;
            Dictionary<SIEEDocument, Document> siee2dataPool = new Dictionary<SIEEDocument, Document>();
            Dictionary<SIEEDocument, int> annotationNumber = new Dictionary<SIEEDocument, int>();

            for (int i = 0; i < pool.RootNode.Documents.Count; i++)
            {
                Document document = pool.RootNode.Documents[i];
                SIEEDocument sieeDocument = documentToFieldlist(new SIEEFieldlist(schema), document, batchId, profile);
                sieeDocument.DocumentId = String.Format("{0:D4}", i);
                sieeDocument.DocumentClass = document.Name;

                sieeDocument.SIEEAnnotation = sieeDocument.NewSIEEAnnotation = null;
                int anNo = findAnnotation(document);
                annotationNumber[sieeDocument] = anNo;
                if (anNo != 0) sieeDocument.SIEEAnnotation = document.Annotations[annotationName(anNo-1)].Value;

                 exportStateParams = DataPoolWorkflowStateExtensions.GetExportStateParams(document);
                // Process only documents with state "ToBeProcessed" (not yet exported documents or documents whose export failed).
                if (exportStateParams.state == ExportState.ToBeProcessed)
                {
                    siee2dataPool[sieeDocument] = document;
                    batch.Add(sieeDocument);
                }
            }

            try
            {
                SIEESettings settings = writerSettings.GetEmbeddedSettings();
                myExport.ExportBatch(settings, batch);
            }
            catch (Exception e)
            {
                SIEEExport.Trace.WriteError("SIEEWriterExport: Batch " + batchId + " failed", e);
                throw;
            }

            foreach (SIEEDocument doc in batch)
            {
                Document occDocument = siee2dataPool[doc];
                int anNo = annotationNumber[doc];
                if (doc.NewSIEEAnnotation != null)
                    occDocument.Annotations.Add(new Annotation(pool, annotationName(anNo), doc.NewSIEEAnnotation));

                exportStateParams = DataPoolWorkflowStateExtensions.GetExportStateParams(occDocument);

                if (doc.Succeeded)
                {
                    occDocument.Annotations.Add(new Annotation(pool, "TargetDocumentId", doc.TargetDocumentId));
                    occDocument.Annotations.Add(new Annotation(pool, "TargetType", description.TypeName));
                    exportStateParams.state = ExportState.Succeeded;
                }
                else
                {
                    exportStateParams.message = "Export failed: " + doc.ErrorMsg;
                    if (doc.NonRecoverableError) throw new Exception("Fatal export error: " + doc.ErrorMsg);
                }

                // Set delay time for start of retry
                if (exportStateParams.repetitionCount == 0)
                    exportStateParams.delaySeconds = description.StartTimeForRetry;

                DataPoolWorkflowStateExtensions.HandleExportStateParams(occDocument, maxRetryCount, exportStateParams);
            }
            SIEEExport.Trace.WriteInfo("Done exporting batch " + batchId);
            return data;
        }

        /// This is the core function that copies the field values from an OCC datapool document into an SIEEFieldlist. 
        /// Input is a fieldlist as derived from the Schema. This means it contains all Schema fields. In the document
        /// there may (a) be additional fields and (b) the might be no field connected to a given Schema field. 
        /// Additional fields are simply ingnored. Fields in the fieldlist that have no correspondence in the document
        /// are left unchanged. That means, if the field has a value that that is passed to the export. Normally schema
        /// fields should have "null" assigned to the Value property of a fields. The SIEEExport function needs to
        /// handle this case.
        private SIEEDocument documentToFieldlist(SIEEFieldlist fieldlist, Document doc, string batchId, string profile)
        {
            CustomExportDestinationField edf;
            CustomExportDestinationTable edt;

            foreach (Field dataPoolField in doc.Fields)
            {
                if (dataPoolField is LookupList)
                {
                    foreach (Field dataPoolSubfield in dataPoolField.Fields)
                        setFieldValue(fieldlist, dataPoolSubfield);
                    continue;
                }
                if (dataPoolField is Table)
                {
                    edt = this.writerSettings.FieldsMapper.GetExternalTable((Table)dataPoolField);
                    if (edt == null)
                        continue;
                    SIEETableField tf = (SIEETableField)fieldlist.GetFieldByName(edt.Name);
                    if (tf == null)
                        continue;
                    FieldCollection rows = ((Table)dataPoolField).Rows;
                    for (int i = 0; i != rows.Count; i++)
                    {
                        TableRow r = (TableRow)rows[i];
                        SIEETableFieldRow tf_row = new SIEETableFieldRow();
                        foreach (Field col in r.Columns)
                        {
                            edf = this.writerSettings.FieldsMapper.GetExternalTableCell(((Table)dataPoolField), col);
                            if (edf == null)
                                continue;

                            if (tf.ColumnExists(edf.Name))
                                tf_row.Add(edf.Name, col.Value);
                        }
                        tf.AddRow(tf_row);
                    }
                    continue;
                }
                // regular field
                setFieldValue(fieldlist, dataPoolField);
            }

            SIEEFieldlist auxFields = new SIEEFieldlist();
            foreach (Field dataPoolField in doc.Fields)
            {
                if (!(dataPoolField is LookupList) && !(dataPoolField is Table))
                    auxFields.Add(new SIEEField(dataPoolField.Name, null, dataPoolField.Value));
            }

            SourceInstance[] si = doc.GetInputSourceInstances();
            Annotation a = doc.Annotations["exportName"];

            SIEEDocument document = new SIEEDocument()
            {
                Fieldlist = fieldlist,
                AuxFields = auxFields,
                PDFFileName = doc.GetExportPdfSource().Url,
                InputFileName = si.Length > 0 ? doc.GetInputSourceInstances()[0].Id : "",
                BatchId = batchId,
                ScriptingName = a?.Value,
                Profile = profile,
            };
            return document;
        }

        private void setFieldValue(SIEEFieldlist fieldlist, Field dataPoolField)
        {
            // Get field mapping. Normally the field is mapped by OCC.
            // For unit test it may also be set by FieldMapping4UnitTest
            CustomExportDestinationField edf = null;

            if (FieldMapping4UnitTest_Contains(dataPoolField.Name))
                edf = new CustomExportDestinationField() { Name = dataPoolField.Name };
            else
                edf = this.writerSettings.FieldsMapper.GetExternalField(dataPoolField);

            if (edf == null) return;

            SIEEField field = fieldlist.GetFieldByName(edf.Name);
            if (field == null) return;
            field.Value = dataPoolField.Value;
            if (field.Cardinality != 0)
            {
                int cnt = 0;
                foreach (IField f in dataPoolField.Fields)
                    if (field.Cardinality < 0 || cnt++ < field.Cardinality) // Cardinality < 0: infinite
                        field.ValueList.Add(f.Value);

                if (field.Cardinality > 0 && dataPoolField.Fields.Count > field.Cardinality)
                    SIEEExport.Trace.WriteError(
                        "Value list truncated. Field=" + edf.Name +
                        " Cardinality=" + field.Cardinality +
                        " FieldCcount=" + dataPoolField.Fields.Count
                    );
            }
        }

        private bool FieldMapping4UnitTest_Contains(string filename)
        {
            System.Text.RegularExpressions.Regex expr;
            foreach (string s in FieldMapping4UnitTest)
            {
                expr = new System.Text.RegularExpressions.Regex(s);
                if (expr.Match(filename).Success) return true;
            }
            return false;
        }


        /// The annotation business.
        /// OCC does not copy back the contents of an annotation. We therefore create a new annotation
        /// each time the value changes. An SIEEDocument has two fields:
        ///     SIEEAnnotation -> contains the value from the data pool (null if there was none)
        ///     NewSIEEAnntoation -> new value to be returned
        /// This function finds the last valid annotation.
        private int findAnnotation(Document doc)
        {
            int cnt = 0;
            for (int i = 0; i < 9999; i++)
            {
                if (doc.Annotations[annotationName(i)] == null) break;
                cnt++;
            }
            return cnt;
        }
        private string annotationName(int number)
        {
            return "SIEEAnnotation" + number.ToString("D4");
        }

        public void Dispose()
        {
            /// Dispose actions
        }
    }
}
