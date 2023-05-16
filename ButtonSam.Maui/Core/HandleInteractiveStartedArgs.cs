using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonSam.Maui.Core
{
    public class HandleInteractiveStartedArgs
    {
        public required InteractiveEventArgs Input { get; set; }
        public float? StartX { get; set; }
        public float? StartY { get; set; }
        public bool? IsPressed { get; set; }
    }
}
