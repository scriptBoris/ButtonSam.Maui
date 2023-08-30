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
        void DirectSetBackgroundColor(Color color);
        bool TryAnimationRippleStart(float x, float y);
        bool TryAnimationRippleEnd();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public partial class ButtonHandler
    {
    }
}
