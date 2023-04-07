using ButtonSam.Maui.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonSam.Maui
{
    public static class Initializer
    {
        public static MauiAppBuilder UseButtonSam(this MauiAppBuilder builder)
        {
            builder.ConfigureMauiHandlers(x =>
            {
                x.AddHandler(typeof(Button), typeof(ButtonHandler));
                x.AddHandler(typeof(Click), typeof(ClickHandler));
            });
            return builder;
        }
    }
}
