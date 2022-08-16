using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PulseApp.Controls
{
    public sealed partial class ValidationTextBox : UserControl
    {
        public ValidationTextBox()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(ValidationTextBox), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty ErrorHintProperty =
            DependencyProperty.Register(nameof(ErrorHint), typeof(string), typeof(ValidationTextBox), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register(nameof(State), typeof(ValidationTextBoxState), typeof(ValidationTextBox), new PropertyMetadata(ValidationTextBoxState.StandBy, OnStateChanged));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public string ErrorHint
        {
            get { return (string)GetValue(ErrorHintProperty); }
            set { SetValue(ErrorHintProperty, value); }
        }

        public ValidationTextBoxState State
        {
            get { return (ValidationTextBoxState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ValidationTextBox thisControl)
            {
                VisualStateManager.GoToState(thisControl, e.NewValue.ToString(), true);
            }
        }

        private void em_TextBoxPaste_OnClick(object sender, RoutedEventArgs e)
        {
            xe_TextBox.PasteFromClipboard();
        }
    }

    public enum ValidationTextBoxState
    {
        StandBy,
        Processing,
        Error
    }
}
