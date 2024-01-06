using ButtonSam.Maui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonSam.Maui;

public static class Initializer
{
    internal static bool UseDebugInfo { get; private set; }

    public static MauiAppBuilder UseButtonSam(this MauiAppBuilder builder, bool useDebugOutputInfo = false)
    {
        UseDebugInfo = useDebugOutputInfo;

        builder.ConfigureMauiHandlers(x =>
        {
#if ANDROID
            x.AddHandler(typeof(InteractiveContainer), typeof(Platforms.Android.ButtonHandler));
#elif IOS
            x.AddHandler(typeof(InteractiveContainer), typeof(Platforms.iOS.ButtonHandler));
#elif WINDOWS
            x.AddHandler(typeof(InteractiveContainer), typeof(Platforms.Windows.ButtonHandler));
#else
            throw new NotImplementedException();
#endif
        });

        return builder;
    }
}
