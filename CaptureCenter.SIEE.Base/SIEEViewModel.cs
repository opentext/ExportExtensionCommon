using System;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Input;

namespace ExportExtensionCommon
{
    public class SIEEViewModel : ModelBase
    {
        public SIEEViewModel()
        {
            IsRunning = false;
            initializeDefaultSetting();
        }

        public SIEEUserControl Control { get; set; } = null;

        private bool isRunning;
        public bool IsRunning
        {
            get { return isRunning; }
            set { SetField(ref isRunning, value); }
        }

        public virtual SIEESettings Settings { get { return null; } }
        public virtual void Initialize(UserControl control) { }
        public virtual void OpenTabs(object sender, ExecutedRoutedEventArgs e) { }

        private static SIEEDefaultValues defaultSettings;
        private void initializeDefaultSetting()
        {
            defaultSettings = new SIEEDefaultValues();
            var ds = Properties.Settings.Default.DefaultSettings;
            if (!string.IsNullOrEmpty(ds))
                try { defaultSettings.Initialize(ds); }
                catch (Exception e)
                {
                    SIEEMessageBox.Show(
                        "Could not load default settings from " + ds + ":\n" + e.Message,
                        "Default settings",
                        System.Windows.MessageBoxImage.Error);
                }
        }

        public void LoadDefaultsFile(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Document (*.xml)|*.xml";
            dialog.Title = "Select default settings file";
            if (dialog.ShowDialog() == true)
            {
                Properties.Settings.Default.DefaultSettings = dialog.FileName;
                Properties.Settings.Default.Save();
                try { defaultSettings.Initialize(dialog.FileName); }
                catch (Exception ex)
                {
                    SIEEMessageBox.Show(
                        "Can't load " + dialog.FileName + ":\n" + ex.Message,
                        "Default settings",
                        System.Windows.MessageBoxImage.Error);
                }
            }
        }

        public virtual void LoadDefaults(object sender, ExecutedRoutedEventArgs e)
        {
            if (Control != null)
                LoadDefaults(Control.SieeControl.GetSettings());
        }

        public void LoadDefaults(SIEESettings settings)
        {
            try { SetDefaults(settings.GetType(), this); }
            catch (Exception ex)
            {
                SIEEMessageBox.Show(
                    "Cannot set defaults. Reason:\n" + ex.Message,
                    "Load default values",
                    System.Windows.MessageBoxImage.Error);
            }
        }

        public void SetDefaults(Type settingsType, SIEEViewModel vm)
        {
            foreach (KeyValuePair<string, string> propSetting in defaultSettings.GetPropertiesDict(settingsType.Name))
            {
                string propName = propSetting.Key;
                string propValue = propSetting.Value;

                SIEEDefaultValues.ObjectAndPropertyInfo opi = SIEEDefaultValues.FindProperty(vm, propName.Split('.'));
                if (opi.PropertyInfo == null) continue;

                var newValue = Convert.ChangeType(propValue, opi.PropertyInfo.PropertyType);
                opi.PropertyInfo.SetValue(opi.Object, newValue, null);
            }
        }

    }
}
