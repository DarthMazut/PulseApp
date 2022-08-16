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
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PulseApp.Controls
{
    public sealed partial class ValidationInput : UserControl
    {
        #region DEPENDENCY PROPERTY

        public static readonly DependencyProperty HeaderTextProperty =
            DependencyProperty.Register(nameof(HeaderText), typeof(string), typeof(ValidationInput), new PropertyMetadata(null));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(ValidationInput), new PropertyMetadata(null, OnTextChanged));

        public static readonly DependencyProperty ButtonCommandProperty =
            DependencyProperty.Register(nameof(ButtonCommand), typeof(ICommand), typeof(ValidationInput), new PropertyMetadata(null));

        public static readonly DependencyProperty ButtonGlyphProperty =
            DependencyProperty.Register(nameof(ButtonGlyph), typeof(string), typeof(ValidationInput), new PropertyMetadata(null));

        public static readonly DependencyProperty IsButtonVisibleProperty =
            DependencyProperty.Register(nameof(IsButtonVisible), typeof(bool), typeof(ValidationInput), new PropertyMetadata(false));

        public static readonly DependencyProperty IsErrorProperty =
            DependencyProperty.Register(nameof(IsError), typeof(bool), typeof(ValidationInput), new PropertyMetadata(false));

        public static readonly DependencyProperty ErrorTextProperty =
            DependencyProperty.Register(nameof(ErrorText), typeof(string), typeof(ValidationInput), new PropertyMetadata(null));

        public static readonly DependencyProperty ValidationFunctionProperty =
            DependencyProperty.Register(nameof(ValidationFunction), typeof(Func<string?, string?>), typeof(ValidationInput), new PropertyMetadata(null));
        
        public static readonly DependencyProperty SelectAllTextOnFocusProperty =
            DependencyProperty.Register(nameof(SelectAllTextOnFocus), typeof(bool), typeof(ValidationInput), new PropertyMetadata(false));

        public string? HeaderText
        {
            get { return (string)GetValue(HeaderTextProperty); }
            set { SetValue(HeaderTextProperty, value); }
        }

        public string? Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public ICommand? ButtonCommand
        {
            get { return (ICommand)GetValue(ButtonCommandProperty); }
            set { SetValue(ButtonCommandProperty, value); }
        }

        public string? ButtonGlyph
        {
            get { return (string)GetValue(ButtonGlyphProperty); }
            set { SetValue(ButtonGlyphProperty, value); }
        }

        public bool? IsButtonVisible
        {
            get { return (bool)GetValue(IsButtonVisibleProperty); }
            set { SetValue(IsButtonVisibleProperty, value); }
        }

        public bool? IsError
        {
            get { return (bool)GetValue(IsErrorProperty); }
            set { SetValue(IsErrorProperty, value); }
        }

        public string? ErrorText
        {
            get { return (string)GetValue(ErrorTextProperty); }
            set { SetValue(ErrorTextProperty, value); }
        }

        public Func<string?, string?>? ValidationFunction
        {
            get { return (Func<string?, string?>)GetValue(ValidationFunctionProperty); }
            set { SetValue(ValidationFunctionProperty, value); }
        }

        public bool SelectAllTextOnFocus
        {
            get { return (bool)GetValue(SelectAllTextOnFocusProperty); }
            set { SetValue(SelectAllTextOnFocusProperty, value); }
        }

        #endregion


        public ValidationInput()
        {
            this.InitializeComponent();
            xe_TextBox.GotFocus += (s, e) =>
            {
                if (SelectAllTextOnFocus)
                {
                    xe_TextBox.SelectAll();
                }
            };
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ValidationInput thisControl)
            {
                string? validationResult = thisControl.ValidationFunction?.Invoke(thisControl.Text);
                thisControl.ErrorText = validationResult;
                if (validationResult == null)
                {
                    VisualStateManager.GoToState(thisControl, "Normal", true);
                    thisControl.IsError = false;
                    
                }
                else
                {
                    thisControl.IsError = true;
                    VisualStateManager.GoToState(thisControl, "Error", true);
                }
            }
        }
    }
}
