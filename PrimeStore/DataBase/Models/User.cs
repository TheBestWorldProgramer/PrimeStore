using DataBase.Models;
using NHibernate.Mapping;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Models
{
    class User
    {
        public virtual int UserId { get; set; }
        public virtual string Login { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Email { get; set; }
        public virtual Role role { get; set; }
        public virtual ICollection<Product> product { get; set; }
    }
}
