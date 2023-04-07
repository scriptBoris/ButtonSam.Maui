using ButtonSam.Maui.Internal;
using Microsoft.Maui.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace ButtonSam.Maui.Internal
{
    public partial class ClickHandler : ViewHandler<Click, UIKit.UIView>
    {
        protected override UIView CreatePlatformView()
        {
            var n = new ClickView();
            return n;
        }
    }

    public class ClickView : UIView
    {

    }
}
