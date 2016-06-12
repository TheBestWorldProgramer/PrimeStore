using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Models
{
    class Product
    {
        private int productId;
        private Discount discount;
        private string name;
        private decimal price;
        private string detail;
        private string path;

        public virtual Discount Discount
        {
            get
            {
                return discount;
            }
            set
            {
                discount = value;
            }
        }
        public virtual int ProductId
        {
            get
            {
                return productId;
            }
        }
        public virtual string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        public virtual decimal Price
        {
            get
            {
                return price;
            }
            set
            {
                price = value;
            }
        }
        public virtual string Detail
        {
            get
            {
                return detail;
            }
            set
            {
                detail = value;
            }
        }
        public virtual string Path
        {
            get
            {
                return path;
            }
            set
            {
                path = value;
            }
        }

    }
}
