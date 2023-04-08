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
        private bool isPressed;
        public Button Button => (Button)VirtualView.Button;

        protected override UIView CreatePlatformView()
        {
            var n = new ClickView();
            var g = new GestureClick(OnTap);

            n.UserInteractionEnabled = true;
            n.AddGestureRecognizer(g);
            return n;
        }

        public void OnTap(UILongPressGestureRecognizer press)
        {
            switch (press.State)
            {
                case UIGestureRecognizerState.Began:
                    isPressed = true;
                    Button.OnTapStart();
                    break;

                case UIGestureRecognizerState.Changed:
                    var coordinate = press.LocationInView(PlatformView);
                    bool isInside = PlatformView.PointInside(coordinate, null);
                    if (!isInside && isPressed) 
                    {
                        isPressed = false;
                        Button.OnTapFinish();
                    }
                    break;

                case UIGestureRecognizerState.Ended:
                    Button.OnTapFinish();
                    if (isPressed)
                        Button.ThrowTap();

                    isPressed = false;
                    break;

                case UIGestureRecognizerState.Cancelled:
                case UIGestureRecognizerState.Failed:
                    isPressed = false;
                    Button.OnTapFinish();
                    break;
                default:
                    break;
            }
        }
    }

    public class ClickView : UIView
    {
    }

    public class GestureClick : UILongPressGestureRecognizer
    {
        public GestureClick(Action<UILongPressGestureRecognizer> action) : base(action)
        {
            MinimumPressDuration = 0;
        }
    }
}
