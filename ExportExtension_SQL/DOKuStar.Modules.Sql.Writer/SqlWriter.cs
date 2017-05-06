using System;
using RightDocs.Common;
using ExportExtensionCommon;

namespace CaptureCenter.SqlEE
{
    [CustomExportDestinationDescription("SQLWriter", "ExportExtensionInterface", "SIEE based Writer for SQL Export", "OpenText")]
    public class SqlEEWriter: EECExportDestination
    {
        public SqlEEWriter() : base()
        {
            Initialize(new SqlEEFactory());
        }
    }
}