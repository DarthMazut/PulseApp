using MochaCore.DialogsEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.Dialogs
{
    public class EnterNameDialogProperties : DialogProperties
    {
        public string? ProvidedName { get; set; }

        public string Extension { get; set; } = "xyz";
    }
}
