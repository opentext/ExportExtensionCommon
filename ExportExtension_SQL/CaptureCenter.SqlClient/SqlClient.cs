using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using System.Xml;

/// This
/// 
namespace CaptureCenter.SqlEE
{
    #region SqlClient interface
    public interface ISqlClient
    {
        string DefaultTable { get; set; }
        void Connect(string instance, string database, string username, string password);
        void Connect(string instance, string database);
        void Disconnect();
        void SetCulture(CultureInfo cultureInfo);
        List<string> GetTablenames();
        List<SqlColumn> GetColumns(string tablename = null);
        void Insert(List<SqlColumn> columns, string tablename = null);
        void GetOneRow(List<SqlColumn> columns, string tablename = null);
        void ClearTable(string tablename = null);
        void SetObjectValues(List<SqlColumn> columns);
        
    }

    public class SqlType
    {
        public string SqlTypeName { get; set; }     // Typename provided by SQL type inquiry
        public Type DotNetType { get; set; }        // Used to convert strings to values
        public SqlDbType SqlDbType { get; set; }    // Defined in System.Data
    }

    // Interface object for a column from an SQL table
    // Used as meta data and to transfer values
    public class SqlColumn  
    {
        public string Name { get; set; }            // Column name
        public SqlType SqlType { get; set; }        // Column type: is null if type is not supported
        public bool IsNullable { get; set; }        // Is null value allowed? 
        public string ValueString { get; set; }     // Value as string (coming from OCC)
        public object ValueObject { get; set; }     // Value as object (transfered to data base)
        public bool IsDocument { get; set; }        // Indicate this column holds the document
    }
    #endregion

    public class SqlClient : ISqlClient, IDisposable
    {
        #region Construction
        private SqlConnection conn = null;
        public string DefaultTable { get; set; }

        public CultureInfo cultureInfo;

        public SqlClient()
        {
            cultureInfo = CultureInfo.InvariantCulture;
        }
        #endregion

        #region Connection
        public void Connect(string instance, string database, string username, string password)
        {
            conn = new SqlConnection();
            conn.ConnectionString = "Data Source=" + instance + ";" +
                "Initial Catalog=" + database + ";" +
                "User id=" + username + ";" +
                "Password=" + password + ";";
            conn.Open();
        }

        public void Connect(string instance, string database)
        {
            conn = new SqlConnection();
            conn.ConnectionString = "Data Source=" + instance + ";" +
                "Initial Catalog=" + database + ";" + "Integrated Security = True;";
            conn.Open();
        }

        public void Disconnect()
        {
            conn.Dispose();
        }

        public void SetCulture(CultureInfo cultureInfo)
        {
            this.cultureInfo = cultureInfo;
        }
        #endregion

        #region Functions
        public List<string> GetTablenames()
        {
            List<string> result = new List<string>();

            DataTable schema = conn.GetSchema("Tables");

            int nameColumn = 0; // = x.Columns.Where(n => n. == ")
            foreach (DataColumn dc in schema.Columns)
            {
                if(dc.ColumnName == "TABLE_NAME") break;
                nameColumn++;
            }
            foreach (DataRow dr in schema.Rows)
                result.Add((string)dr.ItemArray[nameColumn]);

            return result;
        }

        public List<SqlColumn> GetColumns(string tablename = null)
        {
            if (tablename == null) tablename = DefaultTable;
            List<SqlColumn> result = new List<SqlColumn>();

            SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM " + tablename, conn);
            adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            DataTable table = new DataTable();
            adapter.Fill(table);

            foreach (DataColumn dc in table.Columns)
            {
                string sqlType = getColumnProperty(tablename, dc, "DATA_TYPE");
                bool isNullable = getColumnProperty(tablename, dc, "IS_NULLABLE").ToUpper() == "YES";

                result.Add(new SqlColumn()
                {
                    Name = dc.ColumnName,
                    SqlType = SqlTypes.GetSqlType(sqlType),
                    IsNullable = isNullable,
                });
            }
            return result;
        }

        private string getColumnProperty(string tablename, DataColumn dc, string propertyName)
        {
            string sqlCmddString = @"
                    SELECT " + propertyName + @"
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE
                    TABLE_NAME = '" + tablename + @"'
                    AND COLUMN_NAME = '" + dc.ColumnName + @"'";
            SqlCommand command = new SqlCommand(sqlCmddString, conn);
            return command.ExecuteScalar() as string;
        }

        public void Insert(List<SqlColumn> columns, string tablename = null)
        {
            if (tablename == null) tablename = DefaultTable;

            // Create sql command
            string sqlCmddString = "INSERT INTO " + tablename + " (";
            int i = 0;
            foreach (SqlColumn col in columns) // column names
            {
                if (col.ValueObject == null) continue;
                if (i++ > 0) sqlCmddString += ",";
                sqlCmddString += col.Name;
            }
            sqlCmddString += ") VALUES (";
            i = 0;
            foreach (SqlColumn col in columns) // value references
            {
                if (col.ValueObject == null) continue;
                if (i++ > 0) sqlCmddString += ",";
                sqlCmddString += "@" + col.Name;
            }
            sqlCmddString += ")";

            // Create command and add actual parameter values
            SqlCommand command = new SqlCommand(sqlCmddString, conn);
            foreach (SqlColumn col in columns)
                if (col.ValueObject != null)
                    command.Parameters.Add("@" + col.Name, col.SqlType.SqlDbType).Value = col.ValueObject;

            // Exevute command
            command.ExecuteNonQuery();
        }

        public void SetObjectValues(List<SqlColumn> columns)
        {
            foreach (SqlColumn col in columns)
            {
                if (col.IsDocument)
                {
                    if (!SqlTypes.CanStoreDocument(col.SqlType))
                        throw new Exception("Cannot store document in type " + col.SqlType.SqlTypeName + ".");
                    using (FileStream stream = new FileStream(col.ValueString, FileMode.Open, FileAccess.Read))
                    using (BinaryReader reader = new BinaryReader(stream))
                        col.ValueObject = reader.ReadBytes((int)stream.Length);
                    continue;
                }
                if (col.ValueString == null)
                {
                    if (col.IsNullable) continue;
                    else throw new Exception("column " + col.Name + "is not nullable");
                }
                col.ValueObject = SqlTypes.ConvertStringToObject(col.ValueString, col.SqlType, cultureInfo);
            }
        }
        #endregion

        #region Unit test functions
        public void GetOneRow(List<SqlColumn> columns, string tablename = null)
        {
            if (tablename == null) tablename = DefaultTable;
            string sqlCmddString = "SELECT TOP 1 * FROM " + tablename;
            SqlCommand command = new SqlCommand(sqlCmddString, conn);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    foreach (SqlColumn col in columns)
                    {
                        col.ValueObject = reader[col.Name];
                        col.ValueString = col.ValueObject.ToString();
                    }
                }
            }
        }

        public void ClearTable(string tablename = null)
        {
            if (tablename == null) tablename = DefaultTable;
            string sqlCmddString = "TRUNCATE TABLE " + tablename;
            SqlCommand command = new SqlCommand(sqlCmddString, conn);
            command.ExecuteNonQuery();
        }

        public void ExecuteNonQueryCommand(string sqlCmddString)
        {
            SqlCommand command = new SqlCommand(sqlCmddString, conn);
            command.ExecuteNonQuery();
        }
        #endregion

        #region IDisposable
        private bool _disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                    if (conn != null)
                        try { conn.Close(); conn.Dispose(); }
                        catch { }

                _disposed = true;
            }
        }

        ~SqlClient()
        {
            Dispose(false);
        }
        #endregion
    }

    #region Types
    /// This "type thing" is a bit of a pain. I have tried to concentrate all type handlin in this class.
    /// (There is more type specific stuff in the unit test.)
    /// Important to know: All SQL types eventually map to .Net types. Within this code only these .Net
    /// types are handled, i.e. all switch statements have .net type cases.
    /// https://msdn.microsoft.com/de-de/library/cc716729(v=vs.110).aspx
    public class SqlTypes
    {
        // .net type names (for use in switch statements)
        public const string String_TypeName         = "System.String";
        public const string DateTime_TypeName       = "System.DateTime";
        public const string Decimal_TypeName        = "System.Decimal";
        public const string Double_TypeName         = "System.Double";
        public const string ByteArray_TypeName      = "System.Byte[]";
        //public const string DateTimeOffset_TypeName = "System.DateTimeOffset";
        //public const string Int16_TypeName          = "System.Int16";
        //public const string Int32_TypeName          = "System.Int32";
        //public const string Int64_TypeName          = "System.Int64";
        //public const string Single_TypeName         = "System.Single";
        //public const string Object_TypeName         = "System.Object";
        //public const string TimeSpan_TypeName       = "System.TimeSpan";
        //public const string Byte_TypeName           = "System.Byte";
        //public const string Guid_TypeName           = "System.Guid";

        public static SqlType GetSqlType(string sqlTypeName)
        {
            return typeMap.Where(n => n.SqlTypeName == sqlTypeName).FirstOrDefault();
        }

        public static bool CanStoreDocument(SqlType sqlType)
        {
            return sqlType.DotNetType == typeof(Byte[]);
        }

        public static object ConvertStringToObject(string value, SqlType sqlType, CultureInfo cultureInfo)
        {
            switch (sqlType.DotNetType.ToString())
            {
                case String_TypeName: return value;

                case DateTime_TypeName:
                    DateTime dt;
                    if (DateTime.TryParse(value, cultureInfo, DateTimeStyles.None, out dt))
                        return dt;
                    else
                        throw new Exception("Format violation, type DateTime (" + value + ")");

                case Decimal_TypeName:
                    decimal dc;
                    if (Decimal.TryParse(value, NumberStyles.Number, cultureInfo,  out dc))
                        return dc;
                    else
                        throw new Exception("Format violation, type Decimal (" + value + ")");

                case Double_TypeName:
                    double db;
                    if (Double.TryParse(value, NumberStyles.Number, cultureInfo, out db))
                        return db;
                    else
                        throw new Exception("Format violation, type Double (" + value + ")");

                case ByteArray_TypeName: return Encoding.ASCII.GetBytes(value);

                default:
                    throw new Exception("Type not implemented: " + sqlType.SqlTypeName);
            }
        }

        private static List<SqlType> typeMap = new List<SqlType>()
        {
            //new SqlType() { SqlTypeName = "bigint", DotNetType = typeof(Int64), SqlDbType = SqlDbType.BigInt },
            //new SqlType() { SqlTypeName = "binary", DotNetType = typeof(Byte[]), SqlDbType = SqlDbType.VarBinary },
            //new SqlType() { SqlTypeName = "bit", DotNetType = typeof(Boolean), SqlDbType = SqlDbType.Bit },
            new SqlType() { SqlTypeName = "char", DotNetType = typeof(String), SqlDbType = SqlDbType.Char },
            new SqlType() { SqlTypeName = "date", DotNetType = typeof(DateTime), SqlDbType = SqlDbType.Date }, // ok
            //new SqlType() { SqlTypeName = "datetime", DotNetType = typeof(DateTime), SqlDbType = SqlDbType.DateTime },
            //new SqlType() { SqlTypeName = "datetime2", DotNetType = typeof(DateTime), SqlDbType = SqlDbType.DateTime2 },
            //new SqlType() { SqlTypeName = "datetimeoffset", DotNetType = typeof(DateTimeOffset), SqlDbType = SqlDbType.DateTimeOffset },
            new SqlType() { SqlTypeName = "decimal", DotNetType = typeof(Decimal), SqlDbType = SqlDbType.Decimal },
            new SqlType() { SqlTypeName = "float", DotNetType = typeof(Double), SqlDbType = SqlDbType.Float },
            //new SqlType() { SqlTypeName = "image", DotNetType = typeof(Byte[]), SqlDbType = SqlDbType.Image },
            //new SqlType() { SqlTypeName = "int", DotNetType = typeof(Int32), SqlDbType = SqlDbType.Int },
            //new SqlType() { SqlTypeName = "money", DotNetType = typeof(Decimal), SqlDbType = SqlDbType.Money },
            new SqlType() { SqlTypeName = "nchar", DotNetType = typeof(String), SqlDbType = SqlDbType.NChar },
            new SqlType() { SqlTypeName = "ntext", DotNetType = typeof(String), SqlDbType = SqlDbType.NText },
            //new SqlType() { SqlTypeName = "numeric", DotNetType = typeof(Decimal), SqlDbType = SqlDbType.Decimal },
            new SqlType() { SqlTypeName = "nvarchar", DotNetType = typeof(String), SqlDbType = SqlDbType.NVarChar },
            //new SqlType() { SqlTypeName = "real", DotNetType = typeof(Single), SqlDbType = SqlDbType.Real },
            //new SqlType() { SqlTypeName = "rowversion", DotNetType = typeof(Byte[]), SqlDbType = SqlDbType.Timestamp },
            //new SqlType() { SqlTypeName = "smalldatetime", DotNetType = typeof(DateTime), SqlDbType = SqlDbType.Date },
            //new SqlType() { SqlTypeName = "smallint", DotNetType = typeof(Int16), SqlDbType = SqlDbType.SmallInt },
            //new SqlType() { SqlTypeName = "smallmoney", DotNetType = typeof(Decimal), SqlDbType = SqlDbType.SmallMoney },
            //new SqlType() { SqlTypeName = "sql_variant", DotNetType = typeof(Object), SqlDbType = SqlDbType.Variant },
            //new SqlType() { SqlTypeName = "text", DotNetType = typeof(String), SqlDbType = SqlDbType.Text },
            //new SqlType() { SqlTypeName = "time", DotNetType = typeof(TimeSpan), SqlDbType = SqlDbType.Time },
            //new SqlType() { SqlTypeName = "timestamp", DotNetType = typeof(Byte[]), SqlDbType = SqlDbType.Timestamp },
            //new SqlType() { SqlTypeName = "tinyint", DotNetType = typeof(Byte), SqlDbType = SqlDbType.TinyInt },
            //new SqlType() { SqlTypeName = "uniqueidentifier", DotNetType = typeof(Guid), SqlDbType = SqlDbType.UniqueIdentifier },
            new SqlType() { SqlTypeName = "varbinary", DotNetType = typeof(Byte[]), SqlDbType = SqlDbType.VarBinary }, // ok
            new SqlType() { SqlTypeName = "varchar", DotNetType = typeof(String), SqlDbType = SqlDbType.VarChar }, // ok
            //new SqlType() { SqlTypeName = "xml", DotNetType = typeof(Xml), SqlDbType = SqlDbType.Xml },
        };
    }
    #endregion
}
