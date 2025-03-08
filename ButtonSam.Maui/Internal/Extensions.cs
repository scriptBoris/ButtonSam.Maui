namespace ButtonSam.Maui.Internal;

internal static class Extensions
{
    public static InternalTimer CreateAndStartTimer(this IDispatcher dispatcher, TimeSpan interval, Action callback)
    {
        var platformTimer = dispatcher.CreateTimer();
        platformTimer.Interval = interval;

        var wrapper = new InternalTimer(platformTimer, callback);
        wrapper.Start();
        return wrapper;
    }
}