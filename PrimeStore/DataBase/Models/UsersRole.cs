using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Models
{
    class UsersRole
    {
        public virtual User user { get; set; }
        public virtual Role role { get; set; }
    }
}
