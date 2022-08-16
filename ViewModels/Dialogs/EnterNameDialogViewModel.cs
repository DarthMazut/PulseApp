using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MochaCore.DialogsEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.Dialogs
{
    public partial class EnterNameDialogViewModel : ObservableObject, ICustomDialog<EnterNameDialogProperties>, IDialogInitialize
    {
        public ICustomDialogModule<EnterNameDialogProperties> DialogModule { get; set; }

        [ObservableProperty]
        private string? _newNameText;

        [ObservableProperty]
        private Func<string?, string?> _validationFunction = (input) =>
        {
            return string.IsNullOrWhiteSpace(input) ? "Value cannot be empty." : null;
        };

        public void Initialize()
        {
            DialogModule.Opening += OnDialogOpening;
        }

        public void Uninitialize()
        {
            DialogModule.Opening -= OnDialogOpening;
        }


        private void OnDialogOpening(object? sender, EventArgs e)
        {
            NewNameText = DialogModule.Properties.ProvidedName;
        }

        [RelayCommand]
        private void DiscardChanges()
        {
            DialogModule.Close();
        }

        [RelayCommand]
        private void AcceptValue()
        {
            DialogModule.Properties.ProvidedName = NewNameText;
            DialogModule.Close();
        }
    }
}
