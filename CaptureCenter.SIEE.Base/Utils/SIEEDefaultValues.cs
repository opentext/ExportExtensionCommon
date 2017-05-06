using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Reflection;
using DOKuStar.Diagnostics.Tracing;

namespace ExportExtensionCommon
{
    public class SIEEDefaultValues
    {
        private Dictionary<string, Dictionary<string, string>> defaults;

        public SIEEDefaultValues()
        {
            defaults = new Dictionary<string, Dictionary<string, string>>();
        }

        public void Initialize(string defaultValuesFile)
        {
            string ext, prop, val;
            XElement fl = XElement.Load(defaultValuesFile);
            foreach (XElement f in fl.Elements())
            {
                ext = f.Attribute("Extension").Value;
                prop = f.Attribute("Property").Value;
                val = f.Attribute("Value").Value;
                if (!defaults.ContainsKey(ext))
                    defaults[ext] = new Dictionary<string, string>();
                defaults[ext][prop] = val;
            }
        }

        public bool ExtensionExists(string ext) 
        {
            return defaults.ContainsKey(ext);
        }

        public bool Exists(string ext, string prop)
        {
            return defaults.ContainsKey(ext) && defaults[ext].ContainsKey(prop);
        }

        public string Get(string ext, string prop)
        {
            return defaults[ext][prop];
        }

        public Dictionary<string,string> GetPropertiesDict(string settingsType)
        {
            if (!defaults.ContainsKey(settingsType)) throw new Exception("No defaults defined for " + settingsType);
            return defaults[settingsType];
        }

        public class ObjectAndPropertyInfo
        {
            public object Object { get; set; }
            public PropertyInfo PropertyInfo { get; set; }
        } 

        public static ObjectAndPropertyInfo FindProperty(object o, string[] propName)
        {
            PropertyInfo pi = o.GetType().GetProperties().Where(p => p.Name == propName[0]).FirstOrDefault();
            if (propName.Length == 1 || pi == null)
            {
                return new ObjectAndPropertyInfo() { Object = o, PropertyInfo = pi };
            }
            return FindProperty(pi.GetValue(o, null), propName.Skip(1).ToArray());
        }

        public static void setDefaults(string type, SIEEFieldlist fieldlist, SIEEDefaultValues defaultFieldValues)
        {
            foreach (SIEEField field in fieldlist)
            {
                if (!defaultFieldValues.Exists(type, field.Name))
                    continue;

                field.Value = defaultFieldValues.Get(type, field.Name);
                if (field is SIEETableField)
                {
                    ((SIEETableField)field).Set_tabValue(field.Value);
                    continue;
                }
            }
        }
    }
}
