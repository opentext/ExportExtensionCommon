using System;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Controls;

namespace ExportExtensionCommon
{
    public class SIEEUtilsWPF
    {
        public static void EmbedWPFControl(System.Windows.Forms.Control formsControl, System.Windows.Controls.UserControl wpfControl)
        {
            ElementHost elementHost = new ElementHost()
            {
                AutoSize = true,
                //BackColor = System.Drawing.Color.Red;
                Dock = System.Windows.Forms.DockStyle.Fill,
                Child = wpfControl,
            };
        formsControl.Controls.Add(elementHost);
        }

        public static void ShowDialog(Window dlg)
        {
            dlg.ShowInTaskbar = false;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            // The test environment is Winforms, we then set the window to topmost.
            // In OCC we we can set the owner property
            if (Application.Current == null)
                dlg.Topmost = true;
            else
                dlg.Owner = Application.Current.MainWindow;

            dlg.ShowDialog();
        }
    }

    public static class SIEEMessageBox
    {
        public static bool Suppress { get; set; }
        public static string LastMessage { get; set; }

        public static void Show(string s, string title, MessageBoxImage image)
        {
            LastMessage = s;
            if (!Suppress)
            {
                MessageBox.Show(s, title, MessageBoxButton.OK, image);
            }
        }
    }

    #region Converter
    public class OneWayConverter
    {
        public object ConvertBack(object value, Type targetTypeobject, object p, CultureInfo c)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(bool), typeof(String))]
    public class BoolToResultStringConverter : OneWayConverter, IValueConverter
    {
        public object Convert(object value, System.Type targetType, object p, CultureInfo c)
        {
            if (value == null) return "";
            return (bool)value ? "Ok" : "Error";
        }
    }

    [ValueConversion(typeof(bool), typeof(String))]
    public class BoolToResultColorConverter : OneWayConverter, IValueConverter
    {
        public object Convert(object value, System.Type targetType, object p, CultureInfo c)
        {
            if (value == null) return "Black";
            return (bool)value ? "Green" : "Red";
        }
    }

    [ValueConversion(typeof(bool), typeof(String))]
    public class BoolToReadOnlyForegroundColorConverter : OneWayConverter, IValueConverter
    {
        public object Convert(object value, System.Type targetType, object p, CultureInfo c)
        {
            if (value == null) return "Black";
            return (bool)value ? "Black" : "Gray";
        }
    }

    [ValueConversion(typeof(bool), typeof(String))]
    public class BoolToReadOnlyBackgroundColorConverter : OneWayConverter, IValueConverter
    {
        public object Convert(object value, System.Type targetType, object p, CultureInfo c)
        {
            if (value == null) return "White";
            return (bool)value ? "White" : "#f8f8f8";
        }
    }

    [ValueConversion(typeof(bool), typeof(String))]
    public class BoolToRunningCursorConverter : OneWayConverter, IValueConverter
    {
        public object Convert(object value, System.Type targetType, object p, CultureInfo c)
        {
            return (bool)value ? "Wait" : "Arrow";
        }
    }

    [ValueConversion(typeof(bool), typeof(String))]
    public class BoolToVisibilityConverter : OneWayConverter, IValueConverter
    {
        public object Convert(object value, System.Type targetType, object p, CultureInfo c)
        {
            return (bool)value ? "Visible" : "Collapsed";
        }
    }

    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object p, CultureInfo c)
        {
            if (targetType != typeof(object) && targetType != typeof(bool) && targetType != typeof(bool?))
                throw new InvalidOperationException("The target must be bool");
            return !(bool)value;
        }
        public object ConvertBack(object value, Type targetTypeobject, object p, CultureInfo c)
        {
            return Convert(value, targetTypeobject, p, c);
        }
    }

    [ValueConversion(typeof(bool), typeof(String))]
    public class BoolToHiddenOrVisibleConverter : OneWayConverter, IValueConverter
    {
        public object Convert(object value, System.Type targetType, object p, CultureInfo c)
        {
            if (value == null) return "Hidden";
            return (bool)value ? "Hidden" : "Visible";
        }
    }

    [ValueConversion(typeof(bool), typeof(String))]
    public class InverseBoolToHiddenOrVisibleConverter : OneWayConverter, IValueConverter
    {
        public object Convert(object value, System.Type targetType, object p, CultureInfo c)
        {
            if (value == null) return "Visible";
            return (bool)value ? "Visible" : "Hidden";
        }
    }

    [ValueConversion(typeof(bool), typeof(String))]
    public class BoolToCollapsedOrVisibleConverter : OneWayConverter, IValueConverter
    {
        public object Convert(object value, System.Type targetType, object p, CultureInfo c)
        {
            if (value == null) return "Collapsed";
            return (bool)value ? "Collapsed" : "Visible";
        }
    }

    [ValueConversion(typeof(bool), typeof(String))]
    public class InverseBoolToCollapsedOrVisibleConverter : OneWayConverter, IValueConverter
    {
        public object Convert(object value, System.Type targetType, object p, CultureInfo c)
        {
            if (value == null) return "Visible";
            return (bool)value ? "Visible" : "Collapsed";
        }
    }

    public class BoolArrayToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            foreach (object o in values)
            {
                if (!(o is bool)) continue;
                if (!((bool)o)) return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(bool), typeof(String))]
    public class InverseBoolToHiddenBlackGrayConverter : OneWayConverter, IValueConverter
    {
        public object Convert(object value, System.Type targetType, object p, CultureInfo c)
        {
            if (value == null) return "Gray";
            return (bool)value ? "Gray" : "Black";
        }
    }

    [ValueConversion(typeof(bool), typeof(String))]
    public class BoolToHiddenBlackGrayConverter : OneWayConverter, IValueConverter
    {
        public object Convert(object value, System.Type targetType, object p, CultureInfo c)
        {
            if (value == null) return "Black";
            return (bool)value ? "Black" : "Gray";
        }
    }

    [ValueConversion(typeof(bool), typeof(String))]
    public class BoolAndMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] value, System.Type targetType, object p, CultureInfo c)
        {
            bool result = true;
            foreach (bool b in value) result &= b;
            return result;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object Parameter, CultureInfo c)
        {
            throw new Exception("BoolAndMultiConverter.ConvertBack() not implemented");
        }
    }

    [ValueConversion(typeof(bool), typeof(String))]
    public class BoolOrMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] value, System.Type targetType, object p, CultureInfo c)
        {
            bool result = false;
            foreach (bool b in value) result |= b;
            return result;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object Parameter, CultureInfo c)
        {
            throw new Exception("BoolOrMultiConverter.ConvertBack() not implemented");
        }
    }

    #endregion

    #region Validators
    public class NotEmptyValidation : ValidationRule
    {
        public override ValidationResult Validate
          (object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value == null || (string)value == "")
                return new ValidationResult(false, "value cannot be empty.");
            return ValidationResult.ValidResult;
        }
    }
    #endregion

}
