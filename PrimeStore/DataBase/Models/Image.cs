using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Models
{
    class Image
    {
        private Product product;
        private string path;
        public virtual Product Product
        {
            get
            {
                return product;
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
