using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Models
{
    class Role
    {
        private int roleId;
        private string name;

        public virtual int RoleId
        {
            get
            {
                return roleId;
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
