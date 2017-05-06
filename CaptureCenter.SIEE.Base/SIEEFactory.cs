using System;

namespace ExportExtensionCommon
{
    [Serializable]
    public abstract class SIEEFactory
    {
        public abstract SIEESettings CreateSettings();
        public abstract SIEEUserControl CreateWpfControl();
        public abstract SIEEViewModel CreateViewModel(SIEESettings settings);
        public abstract SIEEExport CreateExport();
        
        public virtual SIEEDescription CreateDescription() 
        { 
            return new SIEEDescription(); 
        }
    }
}