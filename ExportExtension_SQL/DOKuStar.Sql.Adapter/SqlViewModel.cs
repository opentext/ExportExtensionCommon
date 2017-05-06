using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ExportExtensionCommon;
using DOKuStar.Diagnostics.Tracing;
using System.IO;

namespace CaptureCenter.SqlEE
{
    public class SqlEEViewModel : SIEEViewModel
    {
        #region Construction
        public SqlEESettings SqlEESettings;
        public ISqlClient SqlClient { get; set; }

        public SqlEEViewModel_CT CT { get; set; }
        public SqlEEViewModel_TT TT { get; set; }
        public SqlEEViewModel_DT DT { get; set; }

        public SqlEEViewModel(SIEESettings settings, ISqlClient sqlClient)
        {
            SqlEESettings = settings as SqlEESettings;
            SqlClient = sqlClient;

            CT = new SqlEEViewModel_CT(this);
            TT = new SqlEEViewModel_TT(this);
            DT = new SqlEEViewModel_DT(this);

            SelectedTab = 0;
            IsRunning = false;
            DataLoaded = false;

            if (SqlEESettings.LoginPossible) LoginButtonHandler();

            CT.PropertyChanged += (s, e) =>
            {
                if (CT.IsConnectionRelevant(e.PropertyName))
                {
                    SqlEESettings.LoginPossible = false;
                    DataLoaded = false;
                    TabNamesReset();
                }
            };
        }

        public override void Initialize(UserControl control)
        {
            CT.Initialize(control);
            TT.Initialize(control);
            DT.Initialize(control);
            initializeTabnames(control);
            ActivateTab(SelectedTab);
        }

        public override SIEESettings Settings
        {
            get { return SqlEESettings; }
        }
        #endregion

        #region Properties (general)
        // The settings in this view model just control the visibility and accessibility of the various tabs
        private int selectedTab;
        public int SelectedTab
        {
            get { return selectedTab; }
            set { selectedTab = value; SendPropertyChanged(); }
        }
        private bool dataLoaded;
        public bool DataLoaded
        {
            get { return dataLoaded; }
            set { dataLoaded = value; SendPropertyChanged(); }
        }
        #endregion

        #region Event handler
        public void LoginButtonHandler()
        {
            IsRunning = true;
            SqlEESettings.LoginPossible = false;
            try { CT.LoginButtonHandler();}
            catch (Exception e) { SIEEMessageBox.Show(e.Message, "Login error", MessageBoxImage.Error); }
            finally { IsRunning = false; }
            DataLoaded = true;
            SqlEESettings.LoginPossible = true;
            SelectedTab = 1;
        }
        #endregion

        #region Tab activation
        public Dictionary<string, bool> Tabnames;
        // Retrieve tabitem names from user control
        private void initializeTabnames(UserControl control)
        {
            Tabnames = new Dictionary<string, bool>();
            TabControl tc = (TabControl)LogicalTreeHelper.FindLogicalNode(control, "mainTabControl");
            foreach (TabItem tabItem in LogicalTreeHelper.GetChildren(tc)) Tabnames[tabItem.Name] = false;
        }

        public void ActivateTab(string tabName)
        {
            if (Tabnames[tabName]) return;
            IsRunning = true;
            try
            {
                switch (tabName)
                {
                    case "connectionTabItem":   { Tabnames[tabName] = CT.ActivateTab(); break; }
                    case "tableTabItem":        { Tabnames[tabName] = TT.ActivateTab(); break; }
                    case "documentTabItem":     { Tabnames[tabName] = DT.ActivateTab(Settings.CreateSchema()); break; }
                }
            }
            catch (Exception e)
            {
                SIEEMessageBox.Show(e.Message, "Error in " + tabName, MessageBoxImage.Error);
                DataLoaded = false;
                SelectedTab = 0;
                TabNamesReset();
            }
            finally { IsRunning = false; }
        }

        private void ActivateTab(int index)
        {
            ActivateTab(tabNameMap[index]);
        }

        private Dictionary<int, string> tabNameMap = new Dictionary<int, string>()
        {
            { 0, "connectionTabItem" },
            { 1, "tableTabItem" },
            { 2, "documentTabItem" },
        };

        private void TabNamesReset()
        {
            foreach (string tn in Tabnames.Keys.ToList()) Tabnames[tn] = false;
        }
        #endregion
    }
}
