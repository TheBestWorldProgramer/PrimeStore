using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Models
{
    class ProductCategory
    {
        private Category category;
        private Product product;

        public virtual Category Category
        {
            get
            {
                return category;
            }
        }

        public virtual Product Product
        {
            get
            {
                return product;
            }

        }
    }
}
