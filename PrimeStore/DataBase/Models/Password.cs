using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Models
{
    class Password
    {
        public virtual int PasswordId { get; set; }
        public virtual string Pas_word { get; set; }
        public virtual User user { get; set; }
    }
}
