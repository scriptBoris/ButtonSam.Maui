using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonSam.Maui.Core
{
    public class HandleInteractiveCompletedArgs
    {
        public required InteractiveEventArgs Input { get; set; }
        public bool? IsPressed { get; set; }
    }
}
