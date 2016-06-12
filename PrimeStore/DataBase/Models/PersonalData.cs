using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Models
{
    class PersonalData
    {
        public virtual User user { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual string City { get; set; }
        public virtual string Country { get; set; }
        public virtual int ZipCode { get; set; }
        public virtual string Street { get; set; }

    }
}
