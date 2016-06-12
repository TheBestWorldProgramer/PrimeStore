using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Models
{
    class Image
    {
        public virtual Product product { get; set; }
        public virtual string Path { get; set; }
    }
}
