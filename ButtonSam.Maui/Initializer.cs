using ButtonSam.Maui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonSam.Maui;

public static class Initializer
{
    public static MauiAppBuilder UseButtonSam(this MauiAppBuilder builder)
    {
        builder.ConfigureMauiHandlers(x =>
        {
#if ANDROID
            x.AddHandler(typeof(ButtonBase), typeof(Platforms.Android.ButtonHandler));
#elif IOS
            x.AddHandler(typeof(ButtonBase), typeof(Platforms.iOS.ButtonHandler));
#elif WINDOWS
            x.AddHandler(typeof(ButtonBase), typeof(Platforms.Windows.ButtonHandler));
#else
            throw new NotImplementedException();
#endif
        });

        return builder;
    }
}
