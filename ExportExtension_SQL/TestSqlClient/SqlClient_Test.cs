using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Globalization;
using System.Data;
using ExportExtensionCommon;

namespace CaptureCenter.SqlEE
{
    [TestClass]
    public partial class SqlEEClient_Test
    {
        #region Construction

        // Test configuration
        private string testSystem = "Default";
        private string testTable = "MyTable";

        // Construction
        public SqlEEClient_Test()
        {
            connectionDefinitions = SIEEUtils.GetLocalTestDefinintions(connectionDefinitions);
            testConnection = connectionDefinitions.Where(n => n.TestSystemName == testSystem).First();

            testDocument = Path.Combine(Path.GetTempPath(), "Document.pdf");
            File.WriteAllBytes(testDocument, Properties.Resources.Document);
            createColumnMetadata(testDocument);
            createDefaultTable(testTable, columnMetadata);
        }

        private ConnectionDefinition testConnection;
        private string testDocument;

        private List<ConnectionDefinition> connectionDefinitions = new List<ConnectionDefinition>() {
            { new ConnectionDefinition()
                {
                    TestSystemName = "Default",
                    Instance = @"",
                    Database = "",
                    Username = "",
                    Password = "",
                    SSOAllowed = true,
                }
            }};

        [Serializable]
        public class ConnectionDefinition
        {
            public ConnectionDefinition() { }
            public string TestSystemName { get; set; }
            public string Instance { get; set; }
            public string Database { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public bool SSOAllowed { get; set; }
        }
        #endregion

        #region Experimentation
        /// Test something (play around)
        [TestMethod]
        [TestCategory("SQL Client Tests")]
        public void TestTest()
        {
            //createDefaultTable();
            typeNames();
        }

        private void typeNames()
        {
            List<Type> typeList = new List<Type>() {
                typeof(Byte[]),
                typeof(String),
                typeof(DateTime),
                typeof(Decimal),
                typeof(Double),
                typeof(Boolean),
                typeof(DateTimeOffset),
                typeof(Int16),
                typeof(Int32),
                typeof(Int64),
                typeof(Single),
                typeof(Object),
                typeof(TimeSpan),
                typeof(Byte),
                typeof(Guid),
                //typeof(Xml),
            };
            string tn = string.Empty;

            foreach (Type t in typeList)
            {
                tn += t.ToString() + "\n";
            }
        }
        #endregion

        #region Connection
        /// Test various connection settings
        [TestMethod]
        [TestCategory("SQL Client Tests")]
        public void T01_Connection()
        {
            SqlClient sqlClient = new SqlClient();
            ConnectionDefinition cd = testConnection;

            // Case 1: Regular connection
            t01_connect(sqlClient, cd);

            // Case 2: Wrong password
            cd.Password += "wrong";
            bool err = false;
            try { t01_connect(sqlClient, cd); }
            catch { err = true; }
            Assert.IsTrue(err);

            sqlClient.Disconnect();

            if (cd.SSOAllowed)
                sqlClient.Connect(cd.Instance, cd.Database);

            sqlClient.Disconnect();
        }

        private void t01_connect(SqlClient sqlClient, ConnectionDefinition cd)
        {
            sqlClient.Connect(cd.Instance, cd.Database, cd.Username, cd.Password);
        }
        #endregion

        #region GetTables
        /// Get table definition
        [TestMethod]
        [TestCategory("SQL Client Tests")]
        public void T02_GetTables()
        {
            ISqlClient sqlClient = getClient();
            List<string> tables = sqlClient.GetTablenames();
            Assert.IsTrue(tables.Count >= 1);
            Assert.IsTrue(tables.Contains(testTable));
        }
        #endregion

        #region GetColumns
        /// Get column definition
        [TestMethod]
        [TestCategory("SQL Client Tests")]
        public void T03_GetColumns()
        {
            ISqlClient sqlClient = getClient();
            List<SqlColumn> columns = sqlClient.GetColumns(testTable);

            Assert.AreEqual(columnMetadata.Count, columns.Count);
            foreach (SqlColumn cd in columns)
            {
                ColumnMetadata cmd = columnMetadata.Where(n => n.Fieldname == cd.Name).First();
                if (cmd.Implemented)
                    Assert.AreEqual(SqlTypes.GetSqlType(cmd.Typename).SqlDbType, cd.SqlType.SqlDbType);
                else
                    Assert.IsNull(cd.SqlType);
            }
        }
        #endregion

        #region Insert
        /// Insert a row into a table. Verifies data type handling
        [TestMethod]
        [TestCategory("SQL Client Tests")]
        public void T04_Insert()
        {
            ISqlClient sqlClient = getClient();
            sqlClient.ClearTable(testTable);

            // Get column definitions (from data base)
            List<SqlColumn> columns = sqlClient.GetColumns(testTable);

            // Add some information from test meta data definition
            foreach (ColumnMetadata cmd in columnMetadata)
            {
                SqlColumn col = columns.Where(n => n.Name == cmd.Fieldname).First();
                col.ValueString = cmd.TestValue;
                col.IsDocument = cmd.IsDocument;
            }

            // Remove unsupported columns
            List<SqlColumn> toBeRemoved = columns.Where(n => n.SqlType == null).ToList();
            foreach (SqlColumn col in toBeRemoved)
                columns.Remove(col);

            // Convert strings to objects and insert row
            sqlClient.SetObjectValues(columns);
            sqlClient.Insert(columns);

            // Read back row from data base
            List<SqlColumn> result = new List<SqlColumn>();
            foreach (SqlColumn col in columns)
                result.Add(new SqlColumn() { Name = col.Name, SqlType = col.SqlType });
            sqlClient.GetOneRow(result);

            // Verify that input values and output values match
            foreach (SqlColumn col in columns)
                compareSqlColumn(col, result.Where(n => n.Name == col.Name).First());

            // Verify that docuoment has been stored properly as blob
            string resultDocument = Path.Combine(Path.GetTempPath(), "Document1.pdf");
            File.WriteAllBytes(resultDocument,
                columns.Where(n => n.Name == "MyDocument").First().ValueObject as Byte[]);
            Assert.IsTrue(SIEEUtils.CompareFiles(testDocument, resultDocument));
            File.Delete(resultDocument);

            // Clear table
            sqlClient.ClearTable(testTable);
        }

        private void compareSqlColumn(SqlColumn c1, SqlColumn c2)
        {
            Assert.AreEqual(c1.SqlType, c2.SqlType);
            CultureInfo ci = new CultureInfo("en-US");

            switch (c1.SqlType.DotNetType.ToString())
            {
                case SqlTypes.String_TypeName:
                    Assert.AreEqual(c1.ValueObject, c2.ValueObject);
                    Assert.AreEqual(c1.ValueString, c2.ValueString);
                    break;

                case SqlTypes.DateTime_TypeName:
                    Assert.AreEqual(c1.ValueObject, c2.ValueObject);
                    Assert.AreEqual(DateTime.Parse(c1.ValueString), DateTime.Parse(c1.ValueString));
                    break;

                case SqlTypes.Decimal_TypeName:
                    Assert.AreEqual(c1.ValueObject, c2.ValueObject);
                    decimal dc = decimal.Parse(c1.ValueString, NumberStyles.Number, ci);
                    Assert.AreEqual(dc.ToString(), c2.ValueString);
                    break;

                case SqlTypes.Double_TypeName:
                    Assert.AreEqual(c1.ValueObject, c2.ValueObject);
                    double db = double.Parse(c1.ValueString, NumberStyles.Number, ci);
                    Assert.AreEqual(db.ToString(), c2.ValueString);
                    break;

                case SqlTypes.ByteArray_TypeName:
                    Byte[] ba1 = c1.ValueObject as Byte[];
                    Byte[] ba2 = c2.ValueObject as Byte[];
                    Assert.AreEqual(ba1.Length, ba2.Length);
                    for (int i = 0; i < ba1.Length; i++)
                        Assert.AreEqual(ba1[i], ba2[i]);
                    break;

                default:
                    throw new Exception("Type not implemented: " + c1.SqlType.SqlTypeName);
            }
        }
        #endregion

        #region CultureInfo
        /// Insert a row into a table
        [TestMethod]
        [TestCategory("SQL Client Tests")]
        public void T05_CultureInfo()
        {
            ISqlClient sqlClient = getClient();

            foreach (SqlColumn col in sqlClient.GetColumns(testTable).Where(n => n.SqlType != null))
            {
                List<SqlColumn> lc = new List<SqlColumn>();
                lc.Add(col);

                switch (col.SqlType.DotNetType.ToString())
                {
                    case SqlTypes.DateTime_TypeName:
                        col.ValueString = "03.04.2016";
                        sqlClient.SetCulture(new CultureInfo("de-DE"));
                        sqlClient.SetObjectValues(lc);
                        Assert.AreEqual(4, ((DateTime)col.ValueObject).Month);

                        sqlClient.SetCulture(new CultureInfo("en-US"));
                        sqlClient.SetObjectValues(lc);
                        Assert.AreEqual(3, ((DateTime)col.ValueObject).Month);
                        break;

                    case SqlTypes.Decimal_TypeName:
                        col.ValueString = "1.234,56";
                        sqlClient.SetCulture(new CultureInfo("de-DE"));
                        sqlClient.SetObjectValues(lc);
                        Assert.AreEqual((decimal)1234.56, col.ValueObject);

                        col.ValueString = "1,234.56";
                        sqlClient.SetCulture(new CultureInfo("en-US"));
                        sqlClient.SetObjectValues(lc);
                        Assert.AreEqual((decimal)1234.56, col.ValueObject);
                        break;

                    case SqlTypes.Double_TypeName:
                        col.ValueString = "1.234,5678";
                        sqlClient.SetCulture(new CultureInfo("de-DE"));
                        sqlClient.SetObjectValues(lc);
                        Assert.AreEqual(1234.5678, col.ValueObject);

                        col.ValueString = "1,234.5678";
                        sqlClient.SetCulture(new CultureInfo("en-US"));
                        sqlClient.SetObjectValues(lc);
                        Assert.AreEqual(1234.5678, col.ValueObject);
                        break;

                    default: break;
                }
            }
        }
        #endregion

        #region Column meta data handling
        [TestMethod]
        [TestCategory("SQL Client Tests")]
        public void T06_ColumnMetaDataHandling()
        {
            SqlClient sqlClient = (SqlClient)getClient();

            // Create a small Column meta data list, one column to play with and
            // the other so there is at least one valid column.
            List<ColumnMetadata> cm = new List<ColumnMetadata>();

            cm.Add(new ColumnMetadata() // used to play around with
            {
                Typename = "decimal",
                Fieldname = "MyDecimal",
            });
            cm.Add(new ColumnMetadata() // make sure one value is there
            {
                Typename = "ntext",
                Fieldname = "MyText",
            });
            string table = "_tmpTable";
            List<SqlColumn> columns;

            //  Read property from table
            cm[0].IsNullable = true;
            createDefaultTable(table, cm);
            columns = sqlClient.GetColumns(table);
            Assert.IsTrue(columns[0].IsNullable);

            cm[0].IsNullable = false;
            createDefaultTable(table, cm);
            columns = sqlClient.GetColumns(table);
            Assert.IsFalse(columns[0].IsNullable);

            // Test value conversion (nullable)
            cm[0].IsNullable = true;
            createDefaultTable(table, cm);
            columns = sqlClient.GetColumns(table);
            columns[0].ValueString = null;
            columns[1].ValueString = "Hello World";

            sqlClient.SetObjectValues(columns); // make sure no exception
            sqlClient.Insert(columns, table);   // insertion should work 

            // Test value conversion (not nullable)
            cm[0].IsNullable = false;
            createDefaultTable(table, cm);
            columns = sqlClient.GetColumns(table);
            columns[0].ValueString = null;
            columns[1].ValueString = "Hello World";
            
            bool exception = false;
            try { sqlClient.SetObjectValues(columns); } // conversion should fail
            catch { exception = true; }
            Assert.IsTrue(exception);

            columns[0].IsNullable = true;   // force conversion to succeed
            sqlClient.SetObjectValues(columns);
            
            try { sqlClient.Insert(columns, table); }   // insertion must fail now
            catch { exception = true; }
            Assert.IsTrue(exception);

            deleteTable(sqlClient, table);
        }
        #endregion

        #region Utilities
        private ISqlClient getClient()
            {
                ISqlClient sqlClient = new SqlClient();
                sqlClient.Connect(testConnection.Instance, testConnection.Database, testConnection.Username, testConnection.Password);
                sqlClient.DefaultTable = testTable;
                sqlClient.SetCulture(new CultureInfo("en-US"));
                return sqlClient;
            }

            private void createDefaultTable(string testTable, List<ColumnMetadata> metadata)
            {
                SqlClient sqlClient = getClient() as SqlClient;
                deleteTable(sqlClient, testTable);

                string sqlCommand;
                sqlCommand = "CREATE TABLE " + testTable + " (";
                foreach (ColumnMetadata cmd in metadata)
                {
                    sqlCommand += "[" + cmd.Fieldname + "] [" + cmd.Typename + "]" + cmd.Size + " ";
                    if (!cmd.IsNullable) sqlCommand += "NOT NULL";
                    sqlCommand += ",";
                }
                sqlCommand += ")";

                sqlClient.ExecuteNonQueryCommand(sqlCommand);
            }

            private void deleteTable(SqlClient sqlClient, string name)
            {
                string sqlCommand = "DROP TABLE " + name;
                try { sqlClient.ExecuteNonQueryCommand(sqlCommand); }
                catch { }
            }
            #endregion

        #region Column meta data
        private class ColumnMetadata
        {
            public string Typename { get; set; }
            public string Fieldname { get; set; }
            public string Size { get; set; }
            public string TestValue { get; set; }
            public bool IsNullable { get; set; } = true;
            public bool Implemented { get; set; } = true;
            public bool IsDocument { get; set; } = false;
        }

        private List<ColumnMetadata> columnMetadata;

        private void createColumnMetadata(string testDocument)
        {
            columnMetadata = new List<ColumnMetadata>();

            // Maintain same order as in SqlClient.cs -> typeMap
            columnMetadata.Add(new ColumnMetadata() {
                Typename = "bigint",Fieldname = "MyBigint", Size = "",
                TestValue = "9999999999",
                Implemented = false,
            });
            columnMetadata.Add(new ColumnMetadata() {
                Typename = "char", Fieldname = "MyChar",  Size = "(10)", 
                TestValue = "0123456789",
            });
            columnMetadata.Add(new ColumnMetadata() {
                Typename = "date", Fieldname = "MyDate", Size = "",
                TestValue = "01.02.2016",
            });
            columnMetadata.Add(new ColumnMetadata(){
                Typename = "decimal", Fieldname = "MyDecimal", Size = "(10,2)",
                TestValue = "1,234.56",
            });
            columnMetadata.Add(new ColumnMetadata() {
                Typename = "float", Fieldname = "MyFloat", Size = "",
                TestValue = "1,234.5678",
            });
            columnMetadata.Add(new ColumnMetadata() {
                Typename = "nchar", Fieldname = "MyNchar", Size = "(10)",
                TestValue = "0123456789",
            });
            columnMetadata.Add(new ColumnMetadata() {
                Typename = "ntext", Fieldname = "MyNtext",  Size = "", 
                TestValue = "Hello World",
            });
            columnMetadata.Add(new ColumnMetadata() {
                Typename = "nvarchar", Fieldname = "MyNvarchar",  Size = "(MAX)", 
                TestValue = "Hello World",
            });
            columnMetadata.Add(new ColumnMetadata() {
                Typename = "varbinary", Fieldname = "MyDocument", Size = "(MAX)",
                TestValue = testDocument, IsDocument = true,
            });
            columnMetadata.Add(new ColumnMetadata() {
                Typename = "varchar", Fieldname = "MyText", Size = "(50)",
                TestValue = "Hello World",
            });
        }
        #endregion
    }
}
