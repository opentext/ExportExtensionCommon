using System;

namespace ExportExtensionCommon
{
    public class SIEEDocument
    {
        public SIEEFieldlist Fieldlist { get; set; }
        public SIEEFieldlist AuxFields { get; set; } = new SIEEFieldlist();
        public string BatchId { get; set; }
        public string Profile { get; set; }
        public string DocumentClass { get; set; }
        public string DocumentId { get; set; }      // consecutive number set by SIEEWriter.transform
        public string ScriptingName { get; set; }   // proposed name from scripting (Annotation ExportName)
        public string InputFileName { get; set; }   // name the input file (first image name)
        public string PDFFileName { get; set; }     // file name of the PDF
        public string SIEEAnnotation { get; set; }
        public string NewSIEEAnnotation { get; set; }

        public bool Succeeded { get; set; } = false;
        public string ErrorMsg { get; set; } = "Unknown";
        public bool NonRecoverableError { get; set; } = false;

        public string TargetDocumentId { get; set; }    // id of the document in the target system (whatever that is)

        public override string ToString()
        {
            return "Document Name = " + InputFileName + Environment.NewLine + Fieldlist.ToString(data: true);
        }
    }
}
