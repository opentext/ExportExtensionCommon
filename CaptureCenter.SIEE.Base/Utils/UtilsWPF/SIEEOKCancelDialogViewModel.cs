
namespace ExportExtensionCommon
{
    public class SIEEOKCancelDialogViewModel : ModelBase
    {
        private SIEEViewModel content;
        public SIEEViewModel Content
        {
            get { return content; }
            set { SetField(ref content, value); }
        }
        private string title = "Dialog";
        public string Title
        {
            get { return title; }
            set { SetField(ref title, value); }
        }
        private string leftButtonText = "Ok";
        public string LeftButtonText
        {
            get { return leftButtonText; }
            set { SetField(ref leftButtonText, value);  }
        }
        private string rightButtonText = "Cancel";
        public string RightButtonText
        {
            get { return rightButtonText; }
            set { SetField(ref rightButtonText, value); }
        }

        public static bool LaunchOkCancelDialog(
            string title, System.Windows.Controls.UserControl content, SIEEViewModel vm,
            string leftButtonText = "Ok", string rightButtonText = "Cancel")
        {
            SIEEOkCancelDialog dlg = CreateOkCancelDialog(title, content, vm, leftButtonText, rightButtonText);
            return dlg.ShowDialog() == true;
        }

        public static SIEEOkCancelDialog CreateOkCancelDialog(
            string title, System.Windows.Controls.UserControl content, SIEEViewModel vm,
            string leftButtonText = "Ok", string rightButtonText = "Cancel")
        {
            SIEEOkCancelDialog dlg
               = new SIEEOkCancelDialog();
            SIEEOKCancelDialogViewModel dlgViewModel = new SIEEOKCancelDialogViewModel();
            dlgViewModel.Title = title;
            dlgViewModel.LeftButtonText = leftButtonText;
            dlgViewModel.RightButtonText = rightButtonText;
            dlg.DataContext = dlgViewModel;

            content.DataContext = vm;
            dlgViewModel.Content = vm;
            dlg.AddContent(content);

            dlg.ShowInTaskbar = false;
            if (System.Windows.Application.Current == null)
                dlg.Topmost = true;
            else
                dlg.Owner = System.Windows.Application.Current.MainWindow;

            return dlg;
        }
    }
}
