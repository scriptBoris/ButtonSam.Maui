using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonSam.Maui.Internal
{
    public interface IClickHandler
    {
        void UpdateTapColor(Color color);
    }

    public partial class ClickHandler
    {
        private static readonly PropertyMapper<Click> Mapper = new();
        public ClickHandler() : base(Mapper)
        {

        }
    }

    public class Click : View
    {
        internal Click(Button btn)
        {
            Button = btn;
        }

        public Button Button { get; private set; }
    }
}
