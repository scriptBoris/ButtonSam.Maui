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
        private bool isPressed;
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
            isPressed = false;
            Button.OnClickedFinish();
        }

        private void N_PointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            isPressed = true;
            Button.OnClickedStart();
        }

        private void N_PointerReleased(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Button.OnClickedFinish();
            if (isPressed)
                Button.ThrowTap();

            isPressed = false;
        }
    }

    public class ClickPanel : Microsoft.UI.Xaml.Controls.Panel
    {
        protected override void OnDisconnectVisualChildren()
        {
            base.OnDisconnectVisualChildren();
        }
    }
}
