using System.Collections.Generic;

namespace ExportExtensionCommon
{
    public static class SIEEFactoryManager
    {
        private static Dictionary<string, SIEEFactory> bySettingsType = new Dictionary<string, SIEEFactory>();
        private static Dictionary<string, SIEEFactory> byTypeName = new Dictionary<string, SIEEFactory>();

        public static List<string> GetKeysFromTypeName() 
        { 
            return new List<string>(byTypeName.Keys); 
        }

        public static void Add(SIEEFactory f) 
        {
            string key = f.CreateSettings().GetType().Name;
            if (!bySettingsType.ContainsKey(key))
            {
                bySettingsType.Add(key, f);
                key = f.CreateSettings().GetType().ToString();
                byTypeName.Add(key, f);
            }
        }

        public static SIEEFactory GetFromSettings(SIEESettings s)
        {
            string key = s.GetType().Name;
            if (bySettingsType.ContainsKey(key)) return bySettingsType[key];
            return null;
        }

        public static SIEEFactory GetFromSettingsTypename(string key)
        {
            return byTypeName[key];
        }
    }
}
