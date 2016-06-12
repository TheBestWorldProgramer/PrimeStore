using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Models
{
    class UsersRole
    {
        private User user;
        private Role role;

        public virtual User User
        {
            get
            {
                return user;
            }
        }

        public virtual Role Role
        {
            get
            {
                return role;
            }
        }
    }
}
