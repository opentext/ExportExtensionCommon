using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ExportExtensionCommon;
using DOKuStar.Diagnostics.Tracing;
using System.IO;
using System.Xml.Serialization;

namespace CaptureCenter.SqlEE
{
    public class SqlEEViewModel_TT : ModelBase
    {
        #region Construction
        private SqlEEViewModel vm;
        private SqlEESettings settings;

        public SqlEEViewModel_TT(SqlEEViewModel vm)
        {
            this.vm = vm;
            settings = vm.SqlEESettings;
        }

        public void Initialize(UserControl control) { }
 
        public bool ActivateTab()
        {
            if (Columns == null) Columns = Columns = createColumnViewModel();
            return false;
        }
        #endregion

        #region Properties
        private List<string> tables;
        public List<string> Tables
        {
            get { return tables; }
            set { SetField(ref tables, value); }
        }

        public string SelectedTable
        {
            get { return settings.SelectedTable; }
            set {
                settings.SelectedTable = value;
                if (value != null) LoadColumns();
                SendPropertyChanged();
            }
        }

        private List<ColumnViewModel> columns;
        public List<ColumnViewModel> Columns
        {
            get { return columns; }
            set { SetField(ref columns, value); }
        }
        #endregion

        #region Functions
        public void LoadTables(List<string> tables)
        {
            Tables = tables;
            if (Tables.Where(n => n == SelectedTable).Count() == 0)
                if (Tables.Count > 0)
                    SelectedTable = Tables[0];
                else
                    SelectedTable = null;
        }

        private void LoadColumns()
        {
            if (settings.SelectedTable == null) return; // can happen when DB loaded that has no tables

            settings.Columns.Clear();
            foreach (SqlColumn col in vm.SqlClient.GetColumns(settings.SelectedTable))
            {
                ColumnDescription cd = new ColumnDescription()
                {
                    Name = col.Name,
                    SqlTypeName = col.SqlType == null ? null : col.SqlType.SqlTypeName,
                    Use = true,
                };
                settings.Columns.Add(cd);
            }
            Columns = createColumnViewModel();
        }

        private List<ColumnViewModel> createColumnViewModel()
        {
            List<ColumnViewModel> result = new List<ColumnViewModel>();
            foreach (ColumnDescription colDes in settings.Columns)
                result.Add(new ColumnViewModel() { ColumnDescription = colDes });

            return result;
        }
        #endregion

        #region Event handlers
        public void SelectAllHandler()
        {
            foreach (ColumnViewModel col in Columns)
                if (col.IsUsable) col.Use = true;
        }

        public void DeselectAllHandler()
        {
            foreach (ColumnViewModel col in Columns)
                col.Use = false;
        }
        #endregion
    }

    public class ColumnViewModel : ModelBase
    {
        private ColumnDescription columnDescription;
        public ColumnDescription ColumnDescription
        {
            get { return columnDescription; }
            set { SetField(ref columnDescription, value); }
        }

        public bool IsDocument
        {
            get { return ColumnDescription.IsDocument; }
            set {
                ColumnDescription.IsDocument = value;
                SendPropertyChanged();
                RaisePropertyChanged(IsUsable_name);
            }
        }

        public bool Use
        {
            get { return ColumnDescription.Use; }
            set { ColumnDescription.Use = value;  SendPropertyChanged(); }
        }

        public bool DocumentAllowed
        {
            get { return isUsable() && ColumnDescription.SqlTypeName == "varbinary"; }
        }

        private string IsUsable_name = "IsUsable";
        public bool IsUsable { get { return isUsable() && !IsDocument; } }

        private bool isUsable()
        {
            return ColumnDescription.SqlTypeName != null;
        }
    }
}
