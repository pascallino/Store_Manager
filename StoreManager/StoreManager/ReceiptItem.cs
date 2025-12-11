using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager
{
    public class ReceiptItem
    {
        public string Name { get; set; }
        public int TotalQty { get; set; }
        public decimal LineTotal { get; set; }
    }

}
