using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Models
{
    class Product
    {
        public virtual int ProductId { get; set; }
        public virtual Discount discount { get; set; }
        public virtual string Name { get; set; }
        public virtual decimal Price { get; set; }
        public virtual string Detail { get; set; }
        public virtual string Path { get; set; }
    }
}
