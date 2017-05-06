using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Globalization;
using DOKuStar.Diagnostics.Tracing;
using ExportExtensionCommon;

using System.IO;

namespace CaptureCenter.SqlEE
{
    public class SqlEEViewModel_CT :ModelBase
    {
        #region Construction
        private SqlEEViewModel vm;
        private SqlEESettings settings;

        public SqlEEViewModel_CT(SqlEEViewModel vm)
        {
            this.vm = vm;
            settings = vm.SqlEESettings;
            logonMethods = new List<LogonMethod>()
            {
                new LogonMethod() { Name = "SQL server authentication", Type = SqlEESettings.LogonType.SqlUser },
                new LogonMethod() { Name = "Windows authentication", Type = SqlEESettings.LogonType.WindowsUser },
            };
            SelectedLogonMethod = logonMethods[0];

            Cultures = CultureInfo.GetCultures(CultureTypes.AllCultures).OrderBy(n => n.DisplayName).ToList();
        }

        public void Initialize(UserControl control)
        {
            findPasswordBox(control);
        }

        public bool ActivateTab() { return true; }
        #endregion

        #region Properties ConnectionTab
        private const string Instance_name = "Instance";
        public string Instance
        {
            get { return settings.Instance; }
            set { settings.Instance = value; SendPropertyChanged(); }
        }

        private const string Catalog_name = "Catalog";
        public string Catalog
        {
            get { return settings.Catalog; }
            set { settings.Catalog = value; SendPropertyChanged(); }
        }

        public class LogonMethod
        {
            public string Name { get; set; }
            public SqlEESettings.LogonType Type { get; set; }
        }

        private List<LogonMethod> logonMethods;
        public List<LogonMethod> LogonMethods
        {
            get { return logonMethods; }
            set { logonMethods = value; SendPropertyChanged(); }
        }

        private const string SelectedLogonMethod_name = "SelectedLogonMethod";
        private LogonMethod selectedLogonMethod;
        public LogonMethod SelectedLogonMethod
        {
            get { return selectedLogonMethod; }
            set {
                selectedLogonMethod = value;
                settings.LoginType = selectedLogonMethod.Type;
                ShowUserNamePassword = settings.LoginType == SqlEESettings.LogonType.SqlUser;
                RaisePropertyChanged(showUserNamePassword_name);
                SendPropertyChanged();
            }
        }

        private string showUserNamePassword_name = "showUserNamePassword";
        private bool showUserNamePassword;
        public bool ShowUserNamePassword
        {
            get { return showUserNamePassword; }
            set { showUserNamePassword = value; SendPropertyChanged(); }
        }

        string Username_name = "Username";
        public string Username
        {
            get { return settings.Username; }
            set { settings.Username = value; SendPropertyChanged(); }
        }

        private List<CultureInfo> cultures;
        public List<CultureInfo> Cultures
        {
            get { return cultures; }
            set { SetField(ref cultures, value); }
        }
        public CultureInfo SelectedCulture
        {
            get { return new CultureInfo(settings.SelectedCultureInfoName); }
            set
            {
                settings.SelectedCultureInfoName = value.Name;
                SendPropertyChanged();
            }
        }
        #endregion

        #region Password
        string Password_name = "Password";
        public string Password
        {
            get {
                if (settings.Password == null) return string.Empty;
                return PasswordEncryption.Decrypt(settings.Password);
            }
            set {
                settings.Password = PasswordEncryption.Encrypt(value);
                SendPropertyChanged("Password");
            }
        }

        private PasswordBox passwordBox;
        private void findPasswordBox(UserControl control)
        {
            passwordBox = (PasswordBox)LogicalTreeHelper.FindLogicalNode(control, "passwordBox");
        }
        public void PasswordChangedHandler()
        {
            Password = SIEEUtils.GetUsecuredString(passwordBox.SecurePassword);
        }
        #endregion

        #region Functions Connection
        public ISqlClient GetSqlClient()
        {
            return vm.SqlClient;
        }
        public bool IsConnectionRelevant(string property)
        {
            return
                property == Instance_name ||
                property == Catalog_name ||
                property == SelectedLogonMethod_name ||
                property == Username_name ||
                property == Password_name;
        }

        public void LoginButtonHandler()
        {
            Login();
            vm.TT.LoadTables(vm.SqlClient.GetTablenames());
        }

        public void Login()
        {
            settings.Login(vm.SqlClient);
        }

        private ConnectionTestResultDialog connectionTestResultDialog;
        private ConnectionTestHandler ConnectionTestHandler;

        // Set up objects, start tests (running in the backgroud) and launch the dialog
        public void TestButtonHandler()
        {
            VmTestResultDialog vmConnectionTestResultDialog = new VmTestResultDialog();
            ConnectionTestHandler = new SqlEEConnectionTestHandler(vmConnectionTestResultDialog);
            ConnectionTestHandler.CallingViewModel = this;

            connectionTestResultDialog = new ConnectionTestResultDialog(ConnectionTestHandler);
            connectionTestResultDialog.DataContext = vmConnectionTestResultDialog;
            connectionTestResultDialog.ShowInTaskbar = false;

            //The test environment is Winforms, we then set the window to topmost.
            //In OCC we we can set the owner property
            if (Application.Current == null)
                connectionTestResultDialog.Topmost = true;
            else
                connectionTestResultDialog.Owner = Application.Current.MainWindow;

            ConnectionTestHandler.LaunchTests();
            connectionTestResultDialog.ShowDialog();
        }

        #endregion

    }
}
