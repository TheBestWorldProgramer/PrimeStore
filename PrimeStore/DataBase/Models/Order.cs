using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Models
{
    class Order
    {
        private int orderId;
        private User user;
        private Product product;

        public virtual User User
        {
            get
            {
                return user;
            }
        }
        public virtual Product Product
        {
            get
            {
                return product;
            }
        }
        public virtual int OrderId
        {
            get
            {
                return orderId;
            }
        }
    }
}
