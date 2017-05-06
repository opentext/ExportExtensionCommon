using System.Windows.Forms;
using System.Threading;

namespace ExportExtensionCommon
{
    public partial class SIEEControl : UserControl
    {
        private SIEEUserControl wpfControl = null;
        private SIEEFactory factory;

        public SIEEControl(SIEEFactory factory)
        {
            InitializeComponent();
            // During OCC's profile delete the Appartmenttype is not STA, for whatever reason.
            // So don't do anything
            if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
                return;

            this.factory = factory;
            wpfControl = factory.CreateWpfControl();
            SIEEUtilsWPF.EmbedWPFControl(this, wpfControl);
            wpfControl.SieeControl = this; // connect controls -> setDefaults needs the control
        }

        private SIEESettings settings; // JS-TT
        public virtual SIEESettings GetSettings()
        {
            return settings;
        }

        public virtual void SetSettings(SIEESettings s)
        {
            settings = s;

            SIEEViewModel vmCommon = factory.CreateViewModel(settings);
            wpfControl.SetDataContext(vmCommon);
            vmCommon.Initialize(wpfControl);
        }
    }
}
