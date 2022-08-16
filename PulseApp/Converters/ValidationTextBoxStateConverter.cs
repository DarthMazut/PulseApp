using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PulseApp.Controls;

namespace PulseApp.Converters
{
    public class ValidationTextBoxStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ViewModels.ValidationTextBoxState state)
            {
                return state switch
                {
                    ViewModels.ValidationTextBoxState.Processing => ValidationTextBoxState.Processing,
                    ViewModels.ValidationTextBoxState.StandBy => ValidationTextBoxState.StandBy,
                    ViewModels.ValidationTextBoxState.Error => ValidationTextBoxState.Error,
                    _ => ValidationTextBoxState.StandBy
                };
            }

                throw new Exception();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
