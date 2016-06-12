using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Models
{
    class Password
    {
        private int passwordId;
        private string pas_word;
        private User user;

        public virtual User User
        {
            get
            {
                return user;
            }
        }
        public virtual int PasswordId
        {
            get
            {
                return passwordId;
            }
        }
        public virtual string Pas_word
        {
            get
            {
                return pas_word;
            }
            set
            {
                pas_word = value;
            }
        }
    }
}
