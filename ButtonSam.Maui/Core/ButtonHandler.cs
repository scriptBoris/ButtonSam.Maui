using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonSam.Maui.Core
{
    public interface IButtonHandler
    {
        bool OverrideBackgroundColor(Color color);
        bool OverrideAdd(object? value);
        bool OverrideInsert(object? value);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public partial class ButtonHandler
    {
    }
}
