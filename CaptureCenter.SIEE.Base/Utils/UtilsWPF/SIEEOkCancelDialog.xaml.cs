using System;
using System.Windows;
using System.Windows.Controls;

namespace ExportExtensionCommon
{
    public partial class SIEEOkCancelDialog : Window
    {
        private Grid content;

        public SIEEOkCancelDialog()
        {
            InitializeComponent();
            DataContext = new SIEEOKCancelDialogViewModel();
            ShowInTaskbar = false;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            content = (Grid)LogicalTreeHelper.FindLogicalNode(this, "contentFrame");
        }

        public void AddContent(UserControl content)
        {
            this.content.Children.Add(content);
            if (DataContext == null || content.DataContext == null) throw new Exception("No DataContext set");
            (DataContext as SIEEOKCancelDialogViewModel).Content = content.DataContext as SIEEViewModel;
        }

        private void Button_Left_Click(object sender, RoutedEventArgs e) { DialogResult = true; Close(); }
        private void Button_Right_Click(object sender, RoutedEventArgs e) { DialogResult = false; Close(); }
    }
}
