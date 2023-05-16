using ButtonSam.Maui.Core;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonSam.Maui.Internal
{
    public partial class ClickHandler : ViewHandler<Click, Microsoft.UI.Xaml.Controls.Panel>
    {
        public Button Button => VirtualView.Button;

        protected override Panel CreatePlatformView()
        {
            var n = new ClickPanel();
            n.Background = Colors.Transparent.ToPlatform();
            n.IsHitTestVisible = true;
            n.IsTapEnabled = true;
            n.PointerPressed += N_PointerPressed;
            n.PointerReleased += N_PointerReleased;
            n.PointerExited += N_PointerExited;
            return n;
        }

        private void N_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Button.OnInteractive(new InteractiveEventArgs
            {
                State = GestureStatus.Canceled,
            });
        }

        private void N_PointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Button.OnInteractive(new InteractiveEventArgs
            {
                State = GestureStatus.Started,
            });
        }

        private void N_PointerReleased(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Button.OnInteractive(new InteractiveEventArgs
            {
                State = GestureStatus.Completed,
            });
        }
    }

    public class ClickPanel : Microsoft.UI.Xaml.Controls.Panel
    {
    }
}
