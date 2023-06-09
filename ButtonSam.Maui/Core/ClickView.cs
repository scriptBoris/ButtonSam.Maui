using ButtonSam.Maui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonSam.Maui.Core
{
    public interface IClickViewHandler
    {
        void UpdateTapColor(Color color);
    }

    public partial class ClickViewHandler
    {
        private static readonly PropertyMapper<ClickView> Mapper = new();
        public ClickViewHandler() : base(Mapper)
        {

        }
    }

    public class ClickView : View
    {
        internal ClickView(ButtonBase btn)
        {
            Button = btn;
        }

        public ButtonBase Button { get; private set; }
    }
}
