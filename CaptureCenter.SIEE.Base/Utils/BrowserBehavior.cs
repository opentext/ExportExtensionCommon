using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ExportExtensionCommon
{
    // http://stackoverflow.com/questions/4202961/can-i-bind-html-to-a-wpf-web-browser-control
    public class BrowserBehavior
    {
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
                "Html",
                typeof(string),
                typeof(BrowserBehavior),
                new FrameworkPropertyMetadata(OnHtmlChanged));

        [AttachedPropertyBrowsableForType(typeof(WebBrowser))]
        public static string GetHtml(WebBrowser d)
        {
            return (string)d.GetValue(HtmlProperty);
        }

        public static void SetHtml(WebBrowser d, string value)
        {
            d.SetValue(HtmlProperty, value);
        }

        //static void OnHtmlChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        //{
        //    WebBrowser webBrowser = dependencyObject as WebBrowser;
        //    if (webBrowser != null)
        //        webBrowser.NavigateToString(e.NewValue as string ?? "&nbsp;");
        //}

        // http://stackoverflow.com/questions/35757729/control-style-in-webbrowser-control/35758665#35758665
        static void OnHtmlChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            WebBrowser webBrowser = dependencyObject as WebBrowser;
            if (webBrowser == null) return;
            webBrowser.NavigateToString(e.NewValue as string ?? "&nbsp;");
            webBrowser.LoadCompleted += WebBrowserOnLoadCompleted;
        }

        private static void WebBrowserOnLoadCompleted(object sender, NavigationEventArgs navigationEventArgs)
        {
            var webBrowser = sender as WebBrowser;
            if (webBrowser == null) return;

            var document = webBrowser.Document as mshtml.HTMLDocument;
            if (document != null)
            {
                var head = document.getElementsByTagName("head").OfType<mshtml.HTMLHeadElement>().FirstOrDefault();
                if (head != null)
                {
                    var styleSheet = (mshtml.IHTMLStyleSheet)document.createStyleSheet("", 0);
                    styleSheet.cssText = @"
#messageText {  
font: message-box;
}
* {
background-color: #f0f0f0; 
}
";
                    /*
                    font-family: Arial, Helvetica, sans-serif; 
font-size: 16px; 
*/
                }
            }
            webBrowser.LoadCompleted -= WebBrowserOnLoadCompleted;
        }
    }
}
