using System.Collections.ObjectModel;

namespace ExportExtensionCommon
{
    /// This is the view model for the TestResultDialog.
    /// Just a collection of properties.
 
    public class VmTestResultDialog : ModelBase
    {
        public VmTestResultDialog() { }

        private int lastIndex = 0;
        public int LastIndex
        {
            get { return lastIndex; }
            set { SetField(ref lastIndex, value); }
        }

        private bool isRunning = false;
        public bool IsRunning
        {
            get { return isRunning; }
            set { SetField(ref isRunning, value); }
        }

        public ObservableCollection<VmTestResult> results = new ObservableCollection<VmTestResult>();
        public ObservableCollection<VmTestResult> Results
        {
            get { return results; }
            set { SetField(ref results, value); }
        }
    }

    /// View model to represent one test, just a set of properties, too.
    public class VmTestResult : ModelBase
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { SetField(ref name, value); }
        }

        private bool? result;
        public bool? Result
        {
            get { return result; }
            set { SetField(ref result, value); }
        }

        private string details;
        public string Details
        {
            get { return details; }
            set { SetField(ref details, value); }
        }
    }
}
