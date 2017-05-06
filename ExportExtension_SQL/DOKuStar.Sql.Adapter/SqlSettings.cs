using System;
using System.Collections.Generic;
using System.Globalization;
using DOKuStar.Diagnostics.Tracing;
using ExportExtensionCommon;

namespace CaptureCenter.SqlEE
{
    [Serializable]
    public class SqlEESettings : SIEESettings
    {
        #region Contruction
        public SqlEESettings()
        {
            // Connection
            Instance = Catalog = Username = password = "";
            loginPossible = false;
            SelectedCultureInfoName = CultureInfo.CurrentCulture.Name;

            // Tables
            SelectedTable = null;
            Columns = new List<ColumnDescription>();

            // Document
            UseSpecification = true;
            Specification = "<BATCHID>_<DOCUMENTNUMBER>";
        }
        #endregion

        #region Properties Connection
        private string instance;
        public string Instance
        {
            get { return instance; }
            set { SetField(ref instance, value); }
        }

        private string catalog;
        public string Catalog
        {
            get { return catalog; }
            set { SetField(ref catalog, value); }
        }

        public enum LogonType { SqlUser, WindowsUser };
        private LogonType loginType;
        public LogonType LoginType
        {
            get { return loginType; }
            set { SetField(ref loginType, value); }
        }

        private string username;
        public string Username
        {
            get { return username; }
            set { SetField(ref username, value); }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set { SetField(ref password, value); ; }
        }

        private bool loginPossible;
        public bool LoginPossible
        {
            get { return loginPossible; }
            set { SetField(ref loginPossible, value); }
        }

        private string selectedCultureInfoName;
        public string SelectedCultureInfoName
        {
            get { return selectedCultureInfoName; }
            set { selectedCultureInfoName = value; SendPropertyChanged(); }
        }
        #endregion

        #region Properties Table
        private string selectedTable;
        public string SelectedTable
        {
            get { return selectedTable; }
            set { SetField(ref selectedTable, value); }
        }

        private List<ColumnDescription> columns;
        public List<ColumnDescription> Columns
        {
            get { return columns; }
            set { SetField(ref columns, value); }
        }
        #endregion

        #region Properties Document
        private bool useInputFileName;
        public bool UseInputFileName
        {
            get { return useInputFileName; }
            set { SetField(ref useInputFileName, value); }
        }

        private bool useSpecification;
        public bool UseSpecification
        {
            get { return useSpecification; }
            set { SetField(ref useSpecification, value); RaisePropertyChanged(specification_Name); }
        }

        private string specification_Name = "Specification";
        private string specification;
        public string Specification
        {
            get { return useSpecification ? specification : null; }
            set { SetField(ref specification, value); }
        }
        #endregion

        #region Functions
        public override SIEEFieldlist CreateSchema()
        {
            SIEEFieldlist schema = new SIEEFieldlist();
            foreach(ColumnDescription col in Columns)
                if (col.Use && !(col.SqlTypeName == null || col.IsDocument))
                    schema.Add(new SIEEField { Name = col.Name, ExternalId = col.Name });

            return schema;
        }

        public void Login(ISqlClient sqlClient)
        {
            if (LoginType == SqlEESettings.LogonType.SqlUser)
                sqlClient.Connect(Instance, Catalog, Username, PasswordEncryption.Decrypt(Password));
            else
                sqlClient.Connect(Instance, Catalog);
        }

        public override object Clone()
        {
            return this.MemberwiseClone() as SqlEESettings;
        }

        public override string GetDocumentNameSpec()
        {
            return Specification;
        }
        #endregion
    }

    [Serializable]
    public class ColumnDescription
    {
        public string Name { get; set; }
        public string SqlTypeName { get; set; }
        public object ValueObject { get; set; }
        public bool IsDocument { get; set; } = false;
        public bool Use { get; set; } = false;
    }
}