using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrsnlMgmt.Mvvm.Model
{
    public class LookupItem
    {
        public int Id { get; set; }
        public string DisplayMember { get; set; }
        public object Tag { get; set; }
    }
}
