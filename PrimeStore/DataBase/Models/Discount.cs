using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Models
{
    class Discount
    {
        private int discountId;
        private float procent;

        public virtual int DiscountId
        {
            get
            {
                return discountId;
            }
        }

        public virtual float Procent
        {
            get
            {
                return procent;
            }
            set
            {
                procent = value;
            }
        }
    }
}
