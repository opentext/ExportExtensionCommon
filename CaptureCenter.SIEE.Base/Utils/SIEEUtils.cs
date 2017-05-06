using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using System.Security;
using System.Runtime.InteropServices;

namespace ExportExtensionCommon
{
    public static class SIEEUtils
    {
        private static string occInstallDir = "";

        public static string Get_OCCInstallDir()
        {
            if (occInstallDir != null && occInstallDir != "") return occInstallDir;
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\Wow6432Node\Opentext\Capture Center");
            string version = key.GetSubKeyNames().OrderBy(x => x).Last();
            occInstallDir = (string)key.OpenSubKey(version).GetValue("HomePath");
            return occInstallDir;
        }

        public static string GetUsecuredString(SecureString ss)
        {
            if (ss == null) return "";
            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(ss);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        public static string StoreResourceFile(Type type, string resourceName, bool binary = false)
        {
            string path = Path.Combine(Path.GetTempPath(), resourceName);
            Stream inStream = type.Assembly.GetManifestResourceStream("OCC_SIEE." + resourceName);
            Stream outStream = File.Open(path, FileMode.Create);

            if (binary)
            {
                BinaryReader br = new BinaryReader(inStream);
                byte[] content = br.ReadBytes((int)inStream.Length);
                BinaryWriter bw = new BinaryWriter(outStream);
                bw.Write(content);
                bw.Close(); // closes outstream too
            }
            else
            {
                StreamReader sr = new StreamReader(inStream);
                string content = sr.ReadToEnd();
                StreamWriter sw = new StreamWriter(outStream);
                sw.Write(content);
                sw.Close(); // closes outstream too
            }
            inStream.Close();
            return path;
        }

        public static void DeleteAllFilesFromFolder(string folder)
        {
            foreach (string f in Directory.GetFiles(folder)) 
                File.Delete(f);
        }

        public static bool CompareFiles(string file1, string file2)
        {
            int file1byte, file2byte;

            if (file1 == file2)
                { return true; }

            FileStream fs1 = new FileStream(file1, FileMode.Open, FileAccess.Read);
            FileStream fs2 = new FileStream(file2, FileMode.Open, FileAccess.Read);

            if (fs1.Length != fs2.Length)
            {
                fs1.Close(); 
                fs2.Close();
                return false;
            }
            // Read and compare a byte from each file until either a
            // non-matching set of bytes is found or until the end of
            // file1 is reached.
            do
            {
                // Read one byte from each file.
                file1byte = fs1.ReadByte();
                file2byte = fs2.ReadByte();
            }
            while ((file1byte == file2byte) && (file1byte != -1));
            
            fs1.Close(); 
            fs2.Close();

            // Return the success of the comparison. "file1byte" is 
            // equal to "file2byte" at this point only if the files are 
            // the same.
            return ((file1byte - file2byte) == 0);
        }

        public static bool StringListEqual(List<string> a, List<string> b)
        {
            if (a == null && b == null) return true;
            if (a == null || b == null) return false;

            bool areEqual = false;
            if (a.Count == b.Count)
            {
                areEqual = true;
                for (int i = 0; i != a.Count; i++)
                    if (a[i] != a[i]) { areEqual = false; break; }
            }
            return areEqual;
        }

        public static string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }

        public static List<T> GetLocalTestDefinintions<T>(List<T> defaultDefinitions)
        {
            string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            string executionPath = Directory.GetParent(path).Parent.Parent.FullName;
            string testSystems = Path.Combine(executionPath, "TestSystems.xml");

            if (File.Exists(testSystems))
                return (List<T>)Serializer.DeserializeFromXmlFile(testSystems, typeof(List<T>), System.Text.Encoding.Unicode);

            return defaultDefinitions;
        }
    }
}
