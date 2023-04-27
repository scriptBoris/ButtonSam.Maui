using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonSam.Maui.Core
{
    public struct InteractiveEventArgs
    {
        public float X { get; set; }
        public float Y { get; set; }
        public required GestureStatus State { get; set; }
    }
}
