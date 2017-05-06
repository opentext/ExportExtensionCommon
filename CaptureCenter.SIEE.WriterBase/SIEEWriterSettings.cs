using System;
using RightDocs.Common;

namespace ExportExtensionCommon
{
    /// The EECWriterSettings class is just a container to hold the real settings that are
    /// of type SIEESettings. The class always keeps a serialized version of the siee_settings
    /// in a string, so serialization of the SIEESettings does not depend on the capabilities
    /// of the OCC/DOKuStar serialization mechanism (which appeared to be limited).
    /// 
    /// Actually also the Schema that is created from the settings is stored in serialized form
    /// for use by the Export function. I'm not sure whether that is the best way to handle things.
    [Serializable]
    public class EECWriterSettings : CustomExportDestinationSettings
    {
        public string SerializedSchema { get; set; }
        public string SerializedSettings { get; set; }
        public string SettingsTypename { get; set; }

        private SIEESettings sieeSettings;
        private SIEEFactory factory;

        public EECWriterSettings()
        {
            SerializedSettings = "";
            SerializedSchema = "";
            sieeSettings = null;
        }

        public void SetFactory (SIEEFactory f) 
        { 
            factory = f;
            this.SettingsTypename = factory.CreateSettings().GetType().ToString();
        }

        /// Various functions in the SIEE_Adapter need to access the true settings, i.e. the
        /// embedded settings. If there is no embedded settings yet, it will be recreated from
        /// the serialized version. If even that does not exist a brand new object is created
        /// from the factory.
        public SIEESettings GetEmbeddedSettings()
        {
            if (sieeSettings != null)
                return sieeSettings;

            if (!string.IsNullOrEmpty(SerializedSettings))
            {
                string xmlString = (string)SIEESerializer.StringToObject(SerializedSettings);
                sieeSettings = (SIEESettings)Serializer.DeserializeFromXmlString(xmlString, factory.CreateSettings().GetType(), System.Text.Encoding.Unicode);
            }
            else
            {
                sieeSettings = factory.CreateSettings();
            }
            return sieeSettings;
        }

        public void SetEmbeddedSettings(SIEESettings s)
        {
            sieeSettings = s;

            // maintain a base64 string version for serialization
            string xmlString = Serializer.SerializeToXmlString(s, System.Text.Encoding.Unicode);
            SerializedSettings = SIEESerializer.ObjectToString(xmlString);
        }

        public SIEEFieldlist CreateSchema()
        {
            GetEmbeddedSettings();  // get latest settings
            if (sieeSettings == null) return null;
            
            SIEEFieldlist schema = sieeSettings.CreateSchemaAndRectifyFieldNames();
            SerializedSchema = SIEESerializer.ObjectToString(schema);

            SetEmbeddedSettings(sieeSettings);  // settings may have changed during schema creation
 
            return schema;
        }
    }
}
