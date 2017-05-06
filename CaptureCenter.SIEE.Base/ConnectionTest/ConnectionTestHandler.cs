using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Threading;
using System.Windows;
using System.Text.RegularExpressions;

namespace ExportExtensionCommon
{
    public class TestFunctionDefinition
    {
        public string Name { get; set; }
        public ConnectionTestHandler.ConnectionTestFunction Function { get; set; }
        public bool ContinueOnError { get; set; } = false;
    }

    public class ConnectionTestHandler
    {
        #region Construction and launcher
        public delegate bool ConnectionTestFunction(ref string message);
        private bool stopTest = false;

        VmTestResultDialogProxy vmTestResultDialogProxy;
        public object CallingViewModel { get; set; }
        public List<TestFunctionDefinition> TestList { get; set; }

        public ConnectionTestHandler(VmTestResultDialog vmTestResultDialog)
        {
            vmTestResultDialogProxy = new VmTestResultDialogProxy() {
                VmTestResultDialog = vmTestResultDialog,
                Dispatcher = Dispatcher.CurrentDispatcher
            };
            TestList = new List<TestFunctionDefinition>();
        }

        public void LaunchTests()
        {
            Task.Factory.StartNew(runAllTests);
        }
        #endregion

        /// ##############################################################
        /// All of the below run a background task
        /// ##############################################################

        #region Proxy for view model
        /// The view model cannot be accessed from the background task. It needs to access it via
        /// the current dispatcher. This is what the proxy is used for. It uses the UI dispatcher to
        /// modify the view model

        private class VmTestResultDialogProxy
        {
            public Dispatcher Dispatcher { get; set; }
            public VmTestResultDialog VmTestResultDialog { get; set; }

            public bool IsRunning
            {  set { Dispatcher.BeginInvoke(new Action(() => VmTestResultDialog.IsRunning = value)); } }

            public void AddResult(VmTestResult tr)
            {
                Dispatcher.BeginInvoke(new Action(() => {
                    VmTestResultDialog.Results.Add(tr);
                    VmTestResultDialog.LastIndex = 
                    VmTestResultDialog.Results.Count - 1;
                }));
            }
        }
        #endregion

        #region Test execution
        private void runAllTests()
        {
            vmTestResultDialogProxy.IsRunning = true;
            try
            {
                foreach (TestFunctionDefinition tfd in TestList)
                {
                    bool success = runOneTest(tfd.Name, tfd.Function);
                    if (!(tfd.ContinueOnError ? true : success) || stopTest) break;
                }
            }
            catch (Exception e)
            {
                SIEEMessageBox.Show("Unhandled exception in connection test.\nReason: " + e.Message, 
                    "Unhandled exception", MessageBoxImage.Error);
            }
            finally { vmTestResultDialogProxy.IsRunning = false; }
        }

        public void StopTest()
        {
            vmTestResultDialogProxy.IsRunning = false;
            stopTest = true;
        }

        private bool runOneTest(string title, ConnectionTestFunction test)
        {
            VmTestResult tr = new VmTestResult() { Name = title };
            vmTestResultDialogProxy.AddResult(tr);

            string msg = string.Empty;
            DateTime start = DateTime.Now;
            bool result = test(ref msg); // execute the test function
            TimeSpan timeUsed = DateTime.Now - start;
            tr.Result = result;

            var regex = new Regex(
               "(\\<script(.+?)\\</script\\>)",
               RegexOptions.Singleline | RegexOptions.IgnoreCase
            );
            msg = regex.Replace(msg, "");

            tr.Details = 
                "<!DOCTYPE html>\n<p id=\"messageText\"> Time: " 
                + timeUsed.TotalMilliseconds / 1000.0 + 
                " seconds\n<br>" 
                + msg + "</p>";
            return result;
        }
        #endregion
    }
}
