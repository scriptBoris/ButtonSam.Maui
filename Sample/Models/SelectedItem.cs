using Sample.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Models
{
    public class SelectedItem : BaseNotify
    {
        public required string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}
