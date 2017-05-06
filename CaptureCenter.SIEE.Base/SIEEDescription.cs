using System.Drawing;

namespace ExportExtensionCommon
{
    public class SIEEDescription
    {
        public virtual string TypeName { get { return "SIEE_Adapter"; } }
        public virtual string DefaultNewName { get { return TypeName; } }
        public virtual bool SupportsFields { get { return true; } }
        public virtual bool SupportsTables { get { return true; } }
        public virtual bool SupportsMapping { get { return true; } }
        public virtual bool SupportsReload { get { return true; } }
        public virtual string GetLocation(SIEESettings s) { return "unknown"; }
        public virtual int NumberOfRetries { get { return 12; } }
        public virtual int StartTimeForRetry { get { return 30; } }
        public virtual void OpenLocation(string location) { }
        public virtual Image Image { get { return Properties.Resources.application; } }
        /* For SIEE  use only */
        public virtual void ClearLocation(SIEESettings settings) { }
    }
}
