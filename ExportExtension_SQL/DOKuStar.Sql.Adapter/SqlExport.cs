using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using ExportExtensionCommon;
using DOKuStar.Diagnostics.Tracing;

namespace CaptureCenter.SqlEE
{
    public class SqlEEExport : SIEEExport
    {
        private SqlEESettings mySettings;
        private ISqlClient sqlClient;

        public SqlEEExport(ISqlClient sqlClient)
        {
            this.sqlClient = sqlClient;
        }
       
        public override void Init(SIEESettings settings)
        {
            base.Init(settings);
            mySettings = settings as SqlEESettings;
            mySettings.Login(sqlClient);
            sqlClient.DefaultTable = mySettings.SelectedTable;
            sqlClient.SetCulture(new CultureInfo(mySettings.SelectedCultureInfoName));
        }

        public override void ExportDocument(SIEESettings settings, SIEEDocument document, string name, SIEEFieldlist fieldlist)
        {
            List<SqlColumn> columns = new List<SqlColumn>();

            foreach (ColumnDescription colDes in mySettings.Columns.Where(n => n.SqlTypeName != null))
            {
                SqlColumn col = new SqlColumn()
                {
                    Name = colDes.Name,
                    SqlType = SqlTypes.GetSqlType(colDes.SqlTypeName),
                    IsDocument = colDes.IsDocument,
                };
                if (colDes.IsDocument)
                    col.ValueString = document.PDFFileName;
                else
                {
                    SIEEField f = fieldlist.Where(n => n.ExternalId == colDes.Name).FirstOrDefault();
                    col.ValueString = f == null ? null : f.Value;
                }
                columns.Add(col);
            }
            try { sqlClient.SetObjectValues(columns); }
            catch (Exception e)
            {
                document.nonRecoverableError = true;
                throw e;
            }
            sqlClient.Insert(columns);
        }
    }
}
