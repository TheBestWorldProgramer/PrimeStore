using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Models
{
    class ProductCategory
    {
        public virtual Category category { get; set; }
        public virtual Product product { get; set; }
    }
}
