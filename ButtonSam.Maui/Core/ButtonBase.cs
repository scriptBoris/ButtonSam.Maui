using ButtonSam.Maui.Core;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ButtonSam.Maui.Core;

public abstract class ButtonBase : InteractiveContainer
{
    #region bindable props
    // is pressed
    public static readonly BindablePropertyKey IsPressedProperty = BindableProperty.CreateReadOnly(
        nameof(IsPressed),
        typeof(bool),
        typeof(ButtonBase),
        false
    );
    public bool IsPressed
    {
        get => (bool)GetValue(IsPressedProperty.BindableProperty);
        protected set => SetValue(IsPressedProperty, value);
    }

    // is mouse over
    public static readonly BindablePropertyKey IsMouseOverProperty = BindableProperty.CreateReadOnly(
        nameof(IsMouseOver),
        typeof(bool),
        typeof(ButtonBase),
        false
    );
    public bool IsMouseOver
    {
        get => (bool)GetValue(IsMouseOverProperty.BindableProperty);
        protected set => SetValue(IsMouseOverProperty, value);
    }
    #endregion bindable props

    // precaclculations
    protected override bool OnGesturePressed(InteractiveEventArgs args)
    {
        if (!IsPressed)
        {
            bool isPressedToInteractive = this.HitTestToInteractive(new Point(args.X, args.Y));
            if (isPressedToInteractive)
                return false;
        }

        return IsEnabled;
    }

    protected override bool OnGestureRelease(InteractiveEventArgs args)
    {
        return true;
    }

    protected override bool OnGestureRunning(InteractiveEventArgs args)
    {
        if (args.DeviceInputType == DeviceInputTypes.Mouse)
        {
            bool matchInteractive = this.HitTestToInteractive(new Point(args.X, args.Y));

            // calculate Exited for nested
            if (IsMouseOver && matchInteractive)
            {
                args.NextFakeState = GestureTypes.Exited;
                return false;
            }
            // calculate Entered for nested
            else if (!IsMouseOver && !matchInteractive)
            {
                args.NextFakeState = GestureTypes.Entered;
                return false;
            }
        }

        return true;
    }

    protected override bool OnGestureEntered(InteractiveEventArgs args)
    {
        return true;
    }

    protected override bool OnGestureExited(InteractiveEventArgs args)
    {
        return true;
    }

    protected override bool OnGestureCanceled(InteractiveEventArgs args)
    {
        return true;
    }
}

public static class ViewExtensions
{
    public static Task<bool> TransitAnimation(this View view,
        string animName,
        double start,
        double end,
        uint length,
        Easing? easing,
        Action<double> f)
    {
        var taskCompletionSource = new TaskCompletionSource<bool>();
        view.Animate(animName, f, start, end, length: length, easing: easing, finished: (x, b) =>
        {
            taskCompletionSource.SetResult(!b);
        });
        return taskCompletionSource.Task;
    }

    public static Color ApplyTint(this Color from, Color to, double percent)
    {
        return Color.FromRgba(
            from.Red + percent * (to.Red - from.Red),
            from.Green + percent * (to.Green - from.Green),
            from.Blue + percent * (to.Blue - from.Blue),
            from.Alpha + percent * (to.Alpha - from.Alpha)
        );
    }

    public static bool HitTestToInteractive(this Layout self, Point point)
    {
#if ANDROID
        return false;
#endif
        double den = Microsoft.Maui.Devices.DeviceDisplay.Current.MainDisplayInfo.Density;
        double x = point.X * den;
        double y = point.Y * den;

        List<View> hitTestResult;
#if WINDOWS
        hitTestResult = HitTest(self, x, y);
#else
        hitTestResult = [];
#endif

        foreach (var v in hitTestResult)
        {
            if (v == self)
                continue;

            if (v.InputTransparent)
                continue;

            if (v.GestureRecognizers.Count > 0)
                return true;

            switch (v)
            {
                case ButtonBase:
                case ICheckBox:
                case ISwitch:
                case IDatePicker:
                case ITimePicker:
                    return true;

                default:
                    break;
            }
        }
        return false;
    }

#if WINDOWS
    public static List<View> HitTest(View from, double x, double y)
    {
        double xp = x;
        double yp = y;

        var winhandl = Application.Current?.Windows.LastOrDefault()?.Handler as WindowHandler;
        var win = winhandl?.PlatformView;
        if (win == null)
            return [];

        if (from.Handler?.PlatformView is not Microsoft.UI.Xaml.UIElement native)
            return [];

        var transform = native.TransformToVisual(win.Content);
        var transpoint = transform.TransformPoint(new Windows.Foundation.Point(0, 0));
        xp += transpoint.X;
        yp += transpoint.Y;

        var res = Microsoft.UI.Xaml.Media.VisualTreeHelper
            .FindElementsInHostCoordinates(
                new Windows.Foundation.Point(xp, yp), 
                native
            );
        var tree = from.GetVisualTreeDescendants().ToList();
        var hitTestResult = new List<View>();

        if (res != null)
        {
            foreach (var element in res)
            {
                var match = tree.FirstOrDefault(x => (x as View)?.Handler?.PlatformView == element);
                if (match != null)
                {
                    hitTestResult.Add((View)match);
                    tree.Remove(match);
                }
            }
        }

        hitTestResult.Reverse();
        return hitTestResult;
    }
#endif
}
