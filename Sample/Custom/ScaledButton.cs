using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Custom;

public class ScaledButton : ButtonSam.Maui.Button
{
    protected override void AnimationMouseOverStart()
    {
        //base.AnimationMouseOverStart();
        Scale = 1.1;
        BackgroundColor = Colors.Pink;
    }

    protected override void AnimationMouseOverRestore()
    {
        //base.AnimationMouseOverRestore();
        Scale = 1.0;
        BackgroundColor = Colors.Transparent;
    }
}
