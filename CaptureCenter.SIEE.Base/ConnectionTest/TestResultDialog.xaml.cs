using System.Windows;

namespace ExportExtensionCommon
{
    public partial class ConnectionTestResultDialog : Window
    {
        public ConnectionTestResultDialog(ConnectionTestHandler connectionTestHandler)
        {
            this.connectionTestHandler = connectionTestHandler;
            InitializeComponent();
        }
        private ConnectionTestHandler connectionTestHandler;

        private void Button_Cancel_Click(object sender, RoutedEventArgs e) { connectionTestHandler.StopTest(); }
        private void Button_Close_Click(object sender, RoutedEventArgs e) { Close(); }
    }
}
