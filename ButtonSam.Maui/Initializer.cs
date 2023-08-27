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
            x.AddHandler(typeof(ButtonBase), typeof(ButtonHandler));
            x.AddHandler(typeof(ClickView), typeof(ClickViewHandler));
        });

        ButtonHandler.Mapper.ModifyMapping(nameof(View.BackgroundColor), (h, v, origin) =>
        {
            if (h is IButtonHandler bh && v is View view && bh.OverrideBackgroundColor(view.BackgroundColor))
                return;

            origin?.Invoke(h, v);
        });

        ButtonHandler.CommandMapper.ModifyMapping(nameof(ILayoutHandler.Add), (h, v, value, origin) =>
        {
            if (h is IButtonHandler bh && bh.OverrideAdd(value))
                return;

            origin?.Invoke(h, v, value);
        });

        ButtonHandler.CommandMapper.ModifyMapping(nameof(ILayoutHandler.Insert), (h, v, value, origin) =>
        {
            if (h is IButtonHandler bh && bh.OverrideInsert(value))
                return;

            origin?.Invoke(h, v, value);
        });
        return builder;
    }
}
