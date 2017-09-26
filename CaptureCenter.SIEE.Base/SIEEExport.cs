using System;
using DOKuStar.Diagnostics.Tracing;

namespace ExportExtensionCommon
{

    public abstract class SIEEExport
    {
        public static readonly ITrace Trace = TraceManager.GetTracer(typeof(SIEEExport));

        private bool isInitialized = false;
        public virtual void Init(SIEESettings settings) { }
        public virtual void Term() { }
        public abstract void ExportDocument(SIEESettings settings, SIEEDocument document, string name, SIEEFieldlist fieldlist);

        public SIEEExport() 
        {
        }

        /// Get the document name based on the filename settings
        public virtual string getDocumentName(SIEESettings settings, SIEEDocument doc)
        {
            // Annotation (exportName) has highest priority
            if (doc.ScriptingName != null) return doc.ScriptingName;

            // Check input filename option
            string nameSpec = settings.GetDocumentNameSpec();
            if (nameSpec == null) return doc.InputFileName;

            // Take name from file name specification
            NameSpecParser nsp = new NameSpecParser(doc.BatchId, doc.DocumentId, doc.Fieldlist.ToKeyValuePairs());
            return nsp.Convert(nameSpec);
        }

        public virtual void ExportBatch(SIEESettings settings, SIEEBatch batch)
        {
            if (!isInitialized)
            {
                Init(settings);
                isInitialized = true;
            }
            try 
            {
                foreach (SIEEDocument doc in batch)
                {
                    try
                    {
                        ExportDocument(settings, doc, getDocumentName(settings, doc), doc.Fieldlist);
                        doc.Succeeded = true;
                    }
                    catch (Exception e)
                    {
                        Trace.WriteError("SIEEExport failed for batch " + doc.BatchId, e);
                        if (e.InnerException != null)
                            Trace.WriteError("SIEEExport failed for batch (details) " + doc.BatchId, e.InnerException);
                        doc.Succeeded = false;
                        doc.ErrorMsg = e.Message + (e.InnerException != null ?  "\n" + e.InnerException.Message :  null);
                   }
                }
            }
            finally 
            { 
                Term(); 
            }
        }
    }
}
