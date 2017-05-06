using System;
using System.Windows.Controls;
using ExportExtensionCommon;

namespace CaptureCenter.SqlEE
{
    public class SqlEEFactory : SIEEFactory
    {
        public override SIEESettings CreateSettings() { return new SqlEESettings(); }
        public override SIEEUserControl CreateWpfControl() { return new SqlEEControlWPF(); }
        public override SIEEViewModel CreateViewModel(SIEESettings settings) { return new SqlEEViewModel(settings, new SqlClient()); }
        public override SIEEExport CreateExport() { return new SqlEEExport(new SqlClient()); }
        public override SIEEDescription CreateDescription() { return new SqlEEDescription(); }
    }

    class SqlEEDescription : SIEEDescription
    {
        public override string TypeName { get { return "Sql"; } }
    }
}
