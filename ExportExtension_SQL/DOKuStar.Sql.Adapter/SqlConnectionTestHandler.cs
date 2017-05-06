using System;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using ExportExtensionCommon;

namespace CaptureCenter.SqlEE
{
    public class SqlEEConnectionTestHandler : ConnectionTestHandler
    {
        public SqlEEConnectionTestHandler(VmTestResultDialog vmTestResultDialog) : base(vmTestResultDialog)
        {
            TestList.Add(new TestFunctionDefinition()
                { Name = "Try to log in", Function = TestFunction_Login });
            TestList.Add(new TestFunctionDefinition()
                { Name = "Try to read tables", Function = TestFunction_Read });
        }

        #region The test fucntions
        private bool TestFunction_Login(ref string errorMsg)
        {
            SqlEEViewModel_CT vmConnection = (SqlEEViewModel_CT)CallingViewModel;
            try
            {
                vmConnection.Login();
            }
            catch (Exception e)
            {
                errorMsg = "Could not log in. \n" + e.Message;
                if (e.InnerException != null)
                    errorMsg += "\n" + e.InnerException.Message;
                return false;
            }
            return true;
        }

        private bool TestFunction_Read(ref string errorMsg)
        {
            SqlEEViewModel_CT vmConnection = (SqlEEViewModel_CT)CallingViewModel;
            ISqlClient sqlClient = vmConnection.GetSqlClient();
            try
            {
                List<string> tables = sqlClient.GetTablenames();
                errorMsg = tables.Count.ToString() + " Tables found";
                return true;
            }
            catch (Exception e)
            {
                errorMsg = "Could not read solutions\n" + e.Message;
                if (e.InnerException != null)
                    errorMsg += "\n" + e.InnerException.Message;
                return false;
            }
        }
        #endregion
    }
}
