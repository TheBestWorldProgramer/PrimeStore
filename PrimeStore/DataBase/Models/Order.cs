using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Models
{
    class Order
    {
        public virtual int OrderId { get; set; }
        public virtual User user { get; set; }
        public virtual Product product { get; set; }
    }
}
