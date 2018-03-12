using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

namespace ExportExtensionCommon
{
    public class SIEECommands
    {
        private static RoutedUICommand openTabs = new RoutedUICommand(
            "open all tabs", "openTabs", typeof(SIEECommands),
            new InputGestureCollection(new List<InputGesture>() {
                new KeyGesture(Key.O, ModifierKeys.Alt | ModifierKeys.Control)
            }));
        public static RoutedUICommand OpenTabs { get { return openTabs; } }

        private static RoutedUICommand loadDefaultsFile = new RoutedUICommand(
            "get file name of defaults file", "loadDefaultsFile", typeof(SIEECommands),
            new InputGestureCollection(new List<InputGesture>() {
                new KeyGesture(Key.F, ModifierKeys.Alt | ModifierKeys.Control)
        }));
        public static RoutedUICommand LoadDefaultsFile { get { return loadDefaultsFile; } }

        private static RoutedUICommand loadDefaults = new RoutedUICommand(
            "load defaults", "loadDefaults", typeof(SIEECommands),
            new InputGestureCollection(new List<InputGesture>() {
                new KeyGesture(Key.F, ModifierKeys.Alt)
        }));
        public static RoutedUICommand LoadDefaults { get { return loadDefaults; } }
    }

    public class SIEEUserControl : UserControl
    {
        public SIEEUserControl()
        {
            CommandBindings.Add(new CommandBinding(
                SIEECommands.OpenTabs,
                (s, e) => { ((SIEEViewModel)DataContext).OpenTabs(s, e); }));
            CommandBindings.Add(new CommandBinding(
                SIEECommands.LoadDefaultsFile,
                (s, e) => { ((SIEEViewModel)DataContext).LoadDefaultsFile(s, e); }));
            CommandBindings.Add(new CommandBinding(
                SIEECommands.LoadDefaults,
                (s, e) => { ((SIEEViewModel)DataContext).LoadDefaults(s, e); }));
        }


        public SIEEControl SieeControl { get; set; }

        public void SetDataContext(SIEEViewModel viewModel)
        {
            DataContext = viewModel;
            viewModel.Control = this;
        }
    }
}
