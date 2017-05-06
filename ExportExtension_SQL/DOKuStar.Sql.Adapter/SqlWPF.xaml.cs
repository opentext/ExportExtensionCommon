using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ExportExtensionCommon;

namespace CaptureCenter.SqlEE
{
    public partial class SqlEEControlWPF : SIEEUserControl
    {
        public SqlEEControlWPF()
        {
            InitializeComponent();
        }

        #region Connection tab
        private void passwordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ((SqlEEViewModel)DataContext).CT.PasswordChangedHandler();
        }

        private void Button_Test_Click(object sender, RoutedEventArgs e)
        {
            ((SqlEEViewModel)DataContext).CT.TestButtonHandler();
        }

        private void Button_Login_Click(object sender, RoutedEventArgs e)
        {
            ((SqlEEViewModel)DataContext).LoginButtonHandler();
        }
        #endregion

        #region Table tab
        private void Button_SelectAll_Click(object sender, RoutedEventArgs e)
        {
            ((SqlEEViewModel)DataContext).TT.SelectAllHandler();
        }

        private void Button_DeselectAll_Click(object sender, RoutedEventArgs e)
        {
            ((SqlEEViewModel)DataContext).TT.DeselectAllHandler();
        }
        #endregion

        #region Tab handling
        private Dictionary<string, bool> tabActivation = null;

        private void initializeTabActivation()
        {
            tabActivation = new Dictionary<string, bool>();
            foreach (string name in ((SqlEEViewModel)DataContext).Tabnames.Keys) tabActivation[name] = false;
        }

        void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((SqlEEViewModel)DataContext).Tabnames == null) return;
            if (tabActivation == null) initializeTabActivation();
            foreach (string tabName in tabActivation.Keys)
            {
                TabItem pt = (TabItem)LogicalTreeHelper.FindLogicalNode((DependencyObject)sender, tabName);
                if (pt.IsSelected)
                {
                    if (tabActivation[tabName]) return;
                    tabActivation[tabName] = true;
                    try { ((SqlEEViewModel)DataContext).ActivateTab(tabName); }
                    finally { tabActivation[tabName] = false; }
                    return;
                }
            }
        }

        private void Button_AddTokenToFile(object sender, RoutedEventArgs e)
        {
            SqlEEViewModel vm = ((SqlEEViewModel)DataContext);
            vm.DT.AddTokenToFileHandler((string)((Button)sender).Tag);
        }
        #endregion
    }
}
