using System;
using System.Windows.Forms;
using RightDocs.Common;

namespace ExportExtensionCommon
{
    /// This file implements thee functions, it creates the control itself, it provides
    /// the ExportDestinationSettins property as required by OCC and implements the hack 
    /// to load default values from an XML file.
    public partial class SIEEWriterControl : UserControl, ICustomExportDestinationControl
    {
        /// The control is just a container for the real control as created from the factory,
        /// the embeddedControl. It is created here and initialized in the ControlDesign file.
        private SIEEControl embeddedControl = null;

        public SIEEWriterControl(SIEEFactory f) 
        { 
            embeddedControl = new SIEEControl(f);
            InitializeComponent();
        }

        /// The settings object contains embedded settings similiar to the control containing
        /// an embedded control. When the ExportDestinationSettings are read or written, the
        /// embedded settings need to go into the embedded control.
        private EECWriterSettings settings;

        public CustomExportDestinationSettings ExportDestinationSettings
        {
            get
            {
                settings.SetEmbeddedSettings(embeddedControl.GetSettings());
                return settings;
            }
            set
            {
                settings = value as EECWriterSettings;
                embeddedControl.SetSettings(settings.GetEmbeddedSettings());
            }
        }
    }
}
