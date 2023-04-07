using Microsoft.Maui.Handlers;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonSam.Maui.Internal
{
    public partial class ClickHandler : ViewHandler<Click, Microsoft.UI.Xaml.Controls.Viewbox>
    {
        protected override Viewbox CreatePlatformView()
        {
            var n = new Viewbox();
            return n;
        }
    }
}
