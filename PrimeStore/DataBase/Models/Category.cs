using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Models
{
    class Category
    {
        private int categoryId;
        private string name;

        public virtual int CategoryId
        {
            get
            {
                return categoryId;
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


    }
}
