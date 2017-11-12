using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;

using DOKuStar.Data.Xml;
using RightDocs.Common;

namespace ExportExtensionCommon
{
    [TestClass]
    public class SIEEBaseTest
    {
        public SIEEBaseTest()
        {
            SIEEMessageBox.Suppress = true;
            // Initialize Factory manager with test export extension
            SIEEFactoryManager.Add(new Test_SIEEFactory());
        }

        #region Empty field handling
        /// The purpose of this test is to ensure that export fields receive the correct values.
        /// 1. SIEE_Fields mapped to OCC fields receive their values from the OCC fields
        /// 2. SIEE_Fields not mapped keep their values from the schema (actually should be null)
        /// 
        /// The implementation of the test is rather complex because it has to do what OCC does. 
        /// (But I think it's instructive at the same time.)
        /// 
        /// The test creates a schema with three fields and exports a document with three fields.
        /// One field of the schema is mapped to the document. The other two fields in the schema
        /// are not mapped. One has a value of null the other of "".

        [TestMethod]
        [TestCategory("SIEE Base")]
        public void t01_EmptyFieldHandling()
        {
            // First we create the runtime document that is to be exported.
            DataPool pool = createDataPool();

            Document doc = pool.RootNode.Documents[0];
            doc.Fields["field1"].Value = "field1value";  // should show up
            doc.Fields.Add(new Field(pool, "field2", "field2value")); // should be ignored
            doc.Fields.Add(new Field(pool, "field3", "field3value")); // should be ignored

            // Create an xml document from the data pool
            XmlDocument data = pool.RootNode.InnerXmlNode.OwnerDocument;

            EECWriterSettings adapterSettings = createWriterSettings(new SIEEFieldlist() {
                { new SIEEField() { Name = "field1", ExternalId = "" } },
                { new SIEEField() { Name = "field2", ExternalId = "" } },
                { new SIEEField() { Name = "field3", Value = "default value dor field3" } }
            });

            SIEEWriterExport adapterExport = new SIEEWriterExport();
            adapterExport.FieldMapping4UnitTest.Add("field1");  // we just want to simulate one mapped field
            adapterExport.Configure(adapterSettings);

            SIEEFieldlist lastFieldList = null;
            Test_SIEEExport.ExportFunc = (settings, document, name, fl) =>
            {
                lastFieldList = fl;
            };
            adapterExport.transform(data, null);    // do the export

            // The test export actually has not exported anything but stored the field list internally.
            // We execute the assertions on this result field list.
            SIEEFieldlist fieldlist = lastFieldList;
            Assert.AreEqual("field1value", fieldlist.Where(n => n.Name == "field1").First().Value, "field1 != null");
            Assert.IsNull(fieldlist.Where(n => n.Name == "field2").First().Value, "field2 == null");
            Assert.AreEqual("default value dor field3", fieldlist.Where(n => n.Name == "field3").First().Value, "field3 has default value");
        }
        #endregion

        #region Serialize Schema to XML
        [TestMethod]
        [TestCategory("SIEE Base")]
        public void t02_SerializeSchemaToXML()
        {
            SIEEFieldlist s1 = new SIEEFieldlist();
            s1.Add(new SIEEField("1", "ex1", "value1"));
            s1.Add(new SIEEField("2", "ex2", "value2"));
            s1.Add(new SIEEField("3", "ex3", "value3"));

            SIEEFieldlist s2 = new SIEEFieldlist();
            s2.Add(new SIEEField("1.1", "ex1.1", "value1.1"));
            s2.Add(new SIEEField("1.2", "ex1.2", "value1.2"));

            s1.Add(new SIEETableField("4", "ex4", "value4", s2));

            string ser = Serializer.SerializeToXmlString(s1, System.Text.Encoding.Unicode);

            SIEEFieldlist deser = (SIEEFieldlist)Serializer.DeserializeFromXmlString(ser, typeof(SIEEFieldlist), System.Text.Encoding.Unicode);

            SIEEField f1 = deser.GetFieldByName("1");
            Assert.AreEqual("value1", f1.Value);
            SIEEField f2 = deser.GetFieldByName("2");
            Assert.AreEqual("2", f2.Name);
            SIEEField f3 = deser.GetFieldByName("3");
            Assert.AreEqual("ex3", f3.ExternalId);

            SIEETableField f4 = deser.GetFieldByName("4") as SIEETableField;
            Assert.AreEqual("ex4", f4.ExternalId);

            SIEEField f5 = f4.Columns[0];
            Assert.AreEqual("value1.1", f5.Value);
        }
        #endregion

        #region Work with Schema
        [TestMethod]
        [TestCategory("SIEE Base")]
        public void t03_WorkWithSchema()
        {
            SIEEFieldlist schema = new SIEEFieldlist();

            Assert.IsNotNull(schema);

            schema.Add(new SIEEField("1", "ex1", "value1"));
            schema.Add(new SIEEField("2", "ex2", "value2"));
            schema.Add(new SIEEField("3", "ex3", "value3"));

            bool b = schema.Exists("2");
            Assert.IsTrue(b);

            SIEEField f = schema.GetFieldByName("2");
            Assert.AreEqual("2", f.Name);

            f = schema.GetFieldByName("x");
            Assert.IsNull(f);

            schema.MakeFieldnamesOCCCompliant();
        }
        #endregion

        #region Document name composer (Basic)
        [TestMethod]
        [TestCategory("SIEE Base")]
        public void t04_DocumentNameComposer_Basic()
        {
            SIEEFieldlist fl = new SIEEFieldlist();
            var td = new[]
            {
                new {n=01, spec=@"abc", result=@"abc"},
                new {n=02, spec=@"a\\bc", result=@"a\bc"},
                new {n=03, spec=@"a\bc", result=@"a\bc"},
                new {n=04, spec=@"a\<bc", result=@"a<bc",},
                new {n=05, spec=@"abc\", result=@"abc\"},

                new {n=10, spec=@"abc_<:Field_1>_def", result=@"abc_Hello_def"},
                new {n=11, spec=@"abc_<unknown>_def", result=@"abc_<unknown>_def"},
                new {n=12, spec=@"<:Field_1> <:Field_2>", result=@"Hello World"},
                new {n=13, spec=@"<:Field_1>", result=@"Hello"},

                new {n=20, spec=@"x<:Field_1", result=@"x<:Field_1"},
                new {n=21, spec=@"<:Field_1", result=@"<:Field_1"},
                new {n=22, spec=@"<<:Field_1>", result=@"<<:Field_1>"},

                new {n=30, spec=@"abc\>de", result=@"abc\>de"},
                new {n=31, spec=@"abc\\de", result=@"abc\de"},
            };
            List<KeyValuePair<string, string>> valueList = new List<KeyValuePair<string, string>>();
            valueList.Add(new KeyValuePair<string, string>("Field_1", "Hello"));
            valueList.Add(new KeyValuePair<string, string>("Field_2", "World"));
            NameSpecParser nsp = new NameSpecParser("", "", valueList);

            int doOnly = 0;
            for (int i = 0; i != td.Length; i++)
            {
                if (doOnly != 0 && td[i].n != doOnly) continue;
                string result = nsp.Convert(td[i].spec);
                Assert.AreEqual(td[i].result, result, td[i].n + ": Value");
            }
            Assert.AreEqual(0, doOnly, "Not all batches executed");
        }
        #endregion

        #region Document name find numbers
        [TestMethod]
        [TestCategory("SIEE Base")]
        public void t05_DocumentNameFindNumbers()
        {
            var td = new[]
            {
                new {n=01, from=0, to=100, exception=false },
                new {n=02, from=900, to=1100, exception=false },
                new {n=03, from=16777216, to=16777216, exception=true },
                new {n=04, from=16777215, to=16777215, exception=false },
            };
            DocumentNameFindNumber dnfn = new DocumentNameFindNumber(DNFN_probe);

            int doOnly = 0;
            for (int i = 0; i != td.Length; i++)
            {
                if (doOnly != 0 && td[i].n != doOnly) continue;

                for (int n = td[i].from; n <= td[i].to; n++)
                {
                    lastExistingNumber = n;
                    bool gotException = false;
                    int result = 0;
                    try { result = dnfn.GetNextFileName("Somename"); }
                    catch { gotException = true; }
                    Assert.AreEqual(td[i].exception, gotException, td[i].n + "->" + n + ": Exception");
                    if (gotException) continue;
                    Assert.AreEqual(lastExistingNumber + 1, result, td[i].n + "->" + n + ": Value");
                }
            }
            Assert.AreEqual(0, doOnly, "Not all batches executed");
        }
        private int lastExistingNumber;
        private bool DNFN_probe(string filename, int number)
        {
            return number <= lastExistingNumber;
        }
        #endregion

        #region Name spec parser
        [TestMethod]
        [TestCategory("SIEE Base")]
        public void t06_TestNameSpecParser()
        {
            NameSpecParser nsp = new NameSpecParser();

            // test data and test loop
            var td = new[]
            {
                new { n=01, spec="abc", result= "abc"},
                new { n=02, spec="a<BatchId>b", result = "a_BatchId_b"},
                new { n=03, spec="a<DocumentNumber>b", result = "a_DocumentNumber_b"},
                new { n=04, spec="a<Guid>b", result = "a_Guid_b"},
                new { n=05, spec="a<Host>b", result = "a_Host_b"},
                new { n=06, spec="a<Date>b", result = "a_Date_b"},
                new { n=07, spec="a<Time>b", result = "a_Time_b"},
                new { n=06, spec="a<UniqueId>b", result = "a_UniqueId_b"},
                new { n=07, spec="a<:Field>b", result = "a:Field:b"},
                new { n=09, spec="a<some thing>b", result = "a<some thing>b"},
                new { n=03, spec="a<PageNumber>b", result = "a_PageNumber_b"},

                new { n=20, spec=@"a\\b\", result = @"a\b\"},
                new { n=21, spec=@"a\<b<", result = @"a<b<"},
                new { n=22, spec=@"a>>b>", result = @"a>>b>"},
                new { n=23, spec=@"a<1<2\34>b", result = @"a<1<2\34>b"},
                new { n=24, spec=@"a<b", result = @"a<b"},
                new { n=24, spec=@"ab<", result = @"ab<"},
                new { n=25, spec=@"a\b", result = @"a\b"},
                new { n=26, spec=@"<>", result = @"<>"},
            };
            int doOnly = 0;

            // Test the parsing function
            for (int i = 0; i != td.Length; i++)
            {
                if (doOnly != 0 && td[i].n != doOnly) continue;
                List<NameSpecParser.SubstituteItem> r = nsp.Parse(td[i].spec);
                setSubstitutionValues(r);
                string finalString = nsp.ComposeResultString(r);
                Assert.AreEqual(td[i].result, finalString, "case: " + td[i].n);
            }
            Assert.AreEqual(doOnly, 0, "Not all tests executed");

            /// Test the substitution process
            nsp.BatchId = "42";
            nsp.DocumentNumber = "0042";
            nsp.ValueList = new List<KeyValuePair<string, string>>();
            nsp.ValueList.Add(new KeyValuePair<string, string>("myField", "myFieldValue"));

            Assert.AreEqual("-42-", nsp.Convert("-<BatchId>-"));
            Assert.AreEqual("-0042-", nsp.Convert("-<DocumentNumber>-"));
            Assert.AreEqual(true, (DateTime.Parse(nsp.Convert("<Date>")) - DateTime.Now).Days < 1);
            Assert.AreEqual(true, int.Parse(nsp.Convert("<Time>")) - int.Parse(DateTime.Now.ToString("HHmmss")) > -5);
            Assert.AreEqual(System.Environment.MachineName, nsp.Convert("<Host>"));
            Guid newGuid;
            Assert.AreEqual(true, Guid.TryParse(nsp.Convert("<Guid>"), out newGuid));
            Assert.AreEqual(true, nsp.Convert("<UniqueId>").Length == 11);
            Assert.AreEqual("-myFieldValue-", nsp.Convert("-<:myField>-"));
            Assert.AreEqual("-noField-", nsp.Convert("-<:noField>-"));

            nsp.ValueList.Add(new KeyValuePair<string, string>("name", "SomeValue"));
            Assert.AreEqual("SomeValue", nsp.Convert("<:name>"));
        }

        private void setSubstitutionValues(List<NameSpecParser.SubstituteItem> r)
        {
            string result = string.Empty;
            foreach (NameSpecParser.SubstituteItem s in r)
            {
                if (s.SubstitutionType == NameSpecParser.SubstitutionType.Const)
                {
                    s.FinalValue = s.Parameter;
                    continue;
                }
                if (s.SubstitutionType == NameSpecParser.SubstitutionType.Field)
                {
                    s.FinalValue = ":" + s.Parameter + ":";
                    continue;
                }
                s.FinalValue = "_" + s.SubstitutionType.ToString() + "_";
            }
        }
        #endregion

        #region Tree view 
        [TestMethod] [TestCategory("SIEE Base")]
        public void t07_TreeViewViewModel()
        {
            SIEETreeView Folders = new SIEETreeView(null);
            TVIViewModel tviVM;
            TVIViewModel tviVMa;

            string testPath = Path.GetTempPath();
            testPath = testPath.Substring(0, testPath.Length - 1);      // Remove trailing "\"
            findFolderBruteForce(testPath);

            string startPath = testPath.Split('\\').First() + @"\";     // e.g. "C:\"
            FilesystemFolder startFsf = new FilesystemFolder(null, new DirectoryInfo(startPath));

            // Select a folder and verify results
            Folders.AddItem(new TVIViewModel(startFsf, null, true));
            Assert.AreEqual(startPath, Folders[0].GetDisplayNamePath(), "t0");
            tviVM = Folders.FindNodeInTree(testPath);
            verifySelectedNode(tviVM, testPath, "t1");

            // Serialize and reinstantiate
            List<string> serializedPath = tviVM.GetSerializedPath();
            Folders.Clear();
            Folders.AddItem(new TVIViewModel(startFsf, null, true));
            tviVM = Folders.InitializeTree(serializedPath, typeof(FilesystemFolder));
            verifySelectedNode(tviVM, testPath, "t2");
            tviVMa = Folders.FindNodeInTree(testPath);
            verifySelectedNode(tviVMa, testPath, "t3");
        }

        private void verifySelectedNode(TVIViewModel node, string path, string testName)
        {
            if (node == null) throw new Exception("verifySelectedNode, testName=" + testName);
            string tail = path.Split('\\').Last();
            FilesystemFolder fsf = node.Tvim as FilesystemFolder;
            Assert.AreEqual(tail.ToLower(), node.DisplayName.ToLower());
            Assert.AreEqual(tail.ToLower(), fsf.DisplayName.ToLower());
            Assert.AreEqual(path.Split('\\').Length, fsf.Depth + 1);
            Assert.AreEqual(fsf.FolderPath.ToLower(), path.ToLower());
        }

        private void findFolderBruteForce(string path)
        {
            DirectoryInfo di = null;
            foreach(string elem in path.Split('\\'))
            {
                if (di == null)
                {
                    di = new DirectoryInfo(elem + "\\");
                    continue;
                }
                var x = di.GetDirectories();
                di = di.GetDirectories().Where(n => n.Name.ToLower() == elem.ToLower()).FirstOrDefault();

                if (di == null)
                    throw new Exception("Subfolder not found. Path=" + path + " folder=" + elem);
            }
        }
        #endregion

        #region FilesystemFolder
        /// A test model to test the TreViewModel. It parses the directory
        public class FilesystemFolder : TVIModel
        {
            public FilesystemFolder() { } // for xml serializer

            public FilesystemFolder(FilesystemFolder parent, DirectoryInfo di)
            {
                DirInfo = di;
                if (parent == null)
                    FolderPath = di.Name;
                else
                {
                    FolderPath = parent.FolderPath + (parent.Depth == 0 ? "" : @"\") + di.Name;
                }
            }

            public FilesystemFolder(TVIModel parent, string name)
            {
                foreach (DirectoryInfo di in ((FilesystemFolder)parent).DirInfo.GetDirectories())
                    if (di.Name.ToLower() == name.ToLower())
                    {
                        DirInfo = di;
                        FolderPath = Path.Combine(((FilesystemFolder)parent).FolderPath, di.Name);
                        break;
                    }
            }

            #region Properties
            private DirectoryInfo dirInfo;
            [XmlIgnore]
            public DirectoryInfo DirInfo
            {
                get { return dirInfo; }
                set { dirInfo = value; DisplayName = Id = dirInfo.Name; }
            }
            public string FolderPath { get; set; }
            #endregion

            #region Functions
            public override List<TVIModel> GetChildren()
            {
                List<TVIModel> result = new List<TVIModel>();
                foreach (DirectoryInfo di in DirInfo.GetDirectories())
                    result.Add(new FilesystemFolder(this, di));
                return result;
            }

            public override string GetPathConcatenationString() { return @"\"; }
            public override string GetTypeName() { return "Folder"; }

            public override TVIModel Clone()
            {
                return this.MemberwiseClone() as FilesystemFolder;
            }

            public override string GetPath(List<TVIModel> path, Pathtype pt)
            {
                string result = string.Empty;
                for (int i = 0; i != path.Count; i++)
                {
                    result += pt == Pathtype.DisplayName ? path[i].DisplayName : path[i].Id;
                    if (i > 0) result += GetPathConcatenationString();
                }
                return result;
            }
            public override bool IsSame(string id)
            {
                string Id1 = Id.ToLower();
                string id1 = id.ToLower();
                return (Id1 == id1 || Id1 + @"\" == id1 || Id1 == id1 + @"\");
            }
            #endregion
        }
        #endregion

        #region SIEESerializer
        [TestMethod]
        [TestCategory("SIEE Base")]
        public void t08_SIEESerializer()
        {
            // Create fieldlist
            SIEEFieldlist fieldlist = new SIEEFieldlist();
            fieldlist.Add(new SIEEField { Name = "Field_1", ExternalId = "Ext_1" });
            fieldlist.Add(new SIEEField { Name = "Field_2", ExternalId = "Ext_2" });
            SIEETableField tf = new SIEETableField { Name = "Table", ExternalId = "Ext_Table" };
            tf.Columns.Add(new SIEEField { Name = "TabField_1", ExternalId = "TabExt_1" });
            tf.Columns.Add(new SIEEField { Name = "TabField_2", ExternalId = "TabExt_2" });
            fieldlist.Add(tf);

            // Serialize
            string s1 = SIEESerializer.ObjectToString(fieldlist);
            // Deserialize
            SIEEFieldlist f1 = (SIEEFieldlist)SIEESerializer.StringToObject(s1);
            // Serialize the newly created field list
            string s2 = SIEESerializer.ObjectToString(f1);

            // final compare
            string txt1 = fieldlist.ToString(data: false);
            string txt2 = f1.ToString(data: false);
            Assert.AreEqual(s1, s2);
        }
        #endregion

        #region SIEEAnnotation handling
        [TestMethod]
        [TestCategory("SIEE Base")]
        public void t09_SIEEAnnotation()
        {
            // Create a data pool
            DataPool pool = createDataPool();
            pool.RootNode.Documents[0].Fields["field1"].Value = "field1value";

            // Create an xml document from the data pool
            XmlDocument data;
            data = pool.RootNode.InnerXmlNode.OwnerDocument;

            // We use a dedicated SIEE_Adapter for this test.  We must first register it in the FactoryManager.
            SIEEFactory factory = new Test_SIEEFactory();
            SIEEFactoryManager.Add(factory);

            // We use a default SIEE_Adapter_Settings object and set the Schema
            EECWriterSettings adapterSettings = createWriterSettings(new SIEEFieldlist() {
                { new SIEEField() { Name = "field1", ExternalId = "" } },
                { new SIEEField() { Name = "field2", ExternalId = "" } },
            });

            SIEEWriterExport adapterExport = new SIEEWriterExport();
            adapterExport.Configure(adapterSettings);

            Test_SIEEExport.ExportFunc = (settings, doc, name, fieldlist) =>
            {
                int val = 0;
                if (doc.SIEEAnnotation != null) val = int.Parse(doc.SIEEAnnotation);
                if (val <= 3) doc.NewSIEEAnnotation = (val+1).ToString();
                throw new Exception("Some exception");
            };
            int count = 1;
            pool = new DataPool(adapterExport.transform(data, null));
            t09_testAnnotation(pool, count++);
            pool = new DataPool(adapterExport.transform(pool.RootNode.InnerXmlNode.OwnerDocument, null));
            t09_testAnnotation(pool, count++);
            pool = new DataPool(adapterExport.transform(pool.RootNode.InnerXmlNode.OwnerDocument, null));
            t09_testAnnotation(pool, count);
            pool = new DataPool(adapterExport.transform(pool.RootNode.InnerXmlNode.OwnerDocument, null));
            t09_testAnnotation(pool, count);
        }
        
        private void t09_testAnnotation(DataPool pool, int val)
        {
            Document doc = pool.RootNode.Documents[0];
            string annotation = doc.Annotations["SIEEAnnotation" + (val - 1).ToString("D4")].Value;
            Assert.AreEqual(val.ToString(), annotation);
        }
        #endregion

        #region List field handling
        [TestMethod]
        [TestCategory("SIEE Base")]
        public void t10_ListFieldHandling()
        {
            // First we create the runtime document that is to be exported.
            DataPool pool = createDataPool();
            Document doc = pool.RootNode.Documents[0];
            doc.Fields["field1"].Value = "field1value";
            doc.Fields.Add(new Field(pool, "field2", "field2value"));
            doc.Fields.Add(new Field(pool, "field3", "field3value"));
            doc.Fields.Add(new Field(pool, "field4", "field4value"));
            doc.Fields.Add(new Field(pool, "field5", "field5value"));
            doc.Fields.Add(new Field(pool, "field6", "field6value"));

            addFieldList(pool, doc.Fields["field1"], 2);    // to ba ignored
                                                            // field2 --> no list
            addFieldList(pool, doc.Fields["field3"], 2);    // fewer subfields
            addFieldList(pool, doc.Fields["field4"], 4);    // exact
            addFieldList(pool, doc.Fields["field5"], 6);    // more sub fields
            addFieldList(pool, doc.Fields["field6"], 42);   // no limits

            // Create an xml document from the data pool
            XmlDocument data;
            data = pool.RootNode.InnerXmlNode.OwnerDocument;

            EECWriterSettings adapterSettings = createWriterSettings(new SIEEFieldlist() {
                { new SIEEField() { Name = "field1",  } },
                { new SIEEField() { Name = "field2", Cardinality = 2 } },
                { new SIEEField() { Name = "field3", Cardinality = 3 } },
                { new SIEEField() { Name = "field4", Cardinality = 4 } },
                { new SIEEField() { Name = "field5", Cardinality = 5 } },
                { new SIEEField() { Name = "field6", Cardinality = -1 } },
            });

            SIEEWriterExport adapterExport = new SIEEWriterExport();
            adapterExport.FieldMapping4UnitTest.Add("field1");
            adapterExport.FieldMapping4UnitTest.Add("field2");
            adapterExport.FieldMapping4UnitTest.Add("field3");
            adapterExport.FieldMapping4UnitTest.Add("field4");
            adapterExport.FieldMapping4UnitTest.Add("field5");
            adapterExport.FieldMapping4UnitTest.Add("field6");

            adapterExport.Configure(adapterSettings);

            SIEEFieldlist lastFieldList = null;
            Test_SIEEExport.ExportFunc = (settings, document, name, fl) =>
            {
                lastFieldList = fl;
            };
            adapterExport.transform(data, null);    // do the export

            // The test export actually has not exported anything but stored the field list internally.
            // We execute the assertions on this result field list.
            SIEEFieldlist fieldlist = lastFieldList;

            verfiyValueList(lastFieldList, "field1", "field1value", 0);
            verfiyValueList(lastFieldList, "field2", "field2value", 0);
            verfiyValueList(lastFieldList, "field3", "field3value", 2);
            verfiyValueList(lastFieldList, "field4", "field4value", 4);
            verfiyValueList(lastFieldList, "field5", "field5value", 5);
            verfiyValueList(lastFieldList, "field6", "field6value", 42);
        }

        private void addFieldList(DataPool pool, IField f, int count)
        {
            for (int i = 0; i < count; i++)
                f.Fields.Add(new Field(pool, "ignore", f.Name + "_" + i.ToString()));
        }

        private void verfiyValueList(SIEEFieldlist fieldlist, string fieldname, string value, int count)
        {
            SIEEField field = fieldlist.GetFieldByName(fieldname);
            Assert.AreEqual(value, field.Value);
            Assert.AreEqual(count, field.ValueList.Count);
        }
        #endregion

        #region Target document id handling
        [TestMethod]
        [TestCategory("SIEE Base")]
        public void t11_TargetDocumentId()
        {
            // Create xml document
            XmlDocument data = createDataPool().RootNode.InnerXmlNode.OwnerDocument;

            // We use a dedicated SIEE_Adapter for this test.  We must first register it in the FactoryManager.
            SIEEFactory factory = new Test_SIEEFactory();
            SIEEFactoryManager.Add(factory);

            // We use a default SIEE_Adapter_Settings object and set the Schema
            EECWriterSettings adapterSettings = createWriterSettings(new SIEEFieldlist());

            SIEEWriterExport adapterExport = new SIEEWriterExport();
            adapterExport.Configure(adapterSettings);

            Test_SIEEExport.ExportFunc = (settings, doc, name, fieldlist) =>
            {
                doc.TargetDocumentId = "4711";
            };

            DataPool pool = new DataPool(adapterExport.transform(data, null));
            Assert.AreEqual("4711", pool.RootNode.Documents[0].Annotations["SIEETargetDocumentId"].Value);
            Assert.AreEqual("SIEE_Adapter", pool.RootNode.Documents[0].Annotations["SIEETargetType"].Value);
        }
        #endregion

        #region Utilities
        private DataPool createDataPool()
        {
            DataPool pool = new DataPool();
            Document d = new Document(pool, "someDocument");
            Field f = new Field(pool, "field1");
            d.Fields.Add(f);
            Source scr = new Source(pool, "Some not existing path");
            d.Sources.Add(scr);
            d.NamedSources["pdf"] = scr;
            d.Sources.Add(new Source(pool, "Some other not existing path"));
            pool.RootNode.Documents.Add(d);
            pool.RootNode.Fields.Add(new Field(pool, "cc_BatchId", "someBatch"));
            return pool;
        }
        #endregion

        #region Test factory
        public class Test_SIEEFactory : SIEEFactory
        {
            public override SIEESettings CreateSettings() { return new Test_SIEESettings(); }
            public override SIEEExport CreateExport() { return new Test_SIEEExport(); }
            public override SIEEUserControl CreateWpfControl() { return null; } // not needed
            public override SIEEViewModel CreateViewModel(SIEESettings settings) { return null; } // not needed
        }
        public class Test_SIEEExport : SIEEExport
        {
            public delegate void ExportDocumentFunction(SIEESettings settings, SIEEDocument document, string name, SIEEFieldlist fieldlist);
            public static ExportDocumentFunction ExportFunc;
            public override void ExportDocument(SIEESettings settings, SIEEDocument document, string name, SIEEFieldlist fieldlist)
            {
                ExportFunc(settings, document, name, fieldlist);
            }
        }
        [Serializable]
        public class Test_SIEESettings : SIEESettings { } // just there to index the SIEE_FactoryManager

        private EECWriterSettings createWriterSettings(SIEEFieldlist schema)
        {
            EECWriterSettings adapterSettings = new EECWriterSettings();
            adapterSettings.SerializedSchema = SIEESerializer.ObjectToString(schema);
            Test_SIEESettings myTestSettings = new Test_SIEESettings();
            adapterSettings.SettingsTypename = myTestSettings.GetType().ToString();
            string xmlString = Serializer.SerializeToXmlString(myTestSettings, System.Text.Encoding.Unicode);
            adapterSettings.SerializedSettings = SIEESerializer.ObjectToString(xmlString);
            adapterSettings.FieldsMapper = new CustomFieldsMapper(); // (Empty), does nothing but must he there
            return adapterSettings;
        }
        #endregion
    }
}
