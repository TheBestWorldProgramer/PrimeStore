using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Models
{
    class PersonalData
    {
        private User user;
        private string phoneNumber;
        private string city;
        private string country;
        private int zipCode;
        private string street;

        public virtual User User
        {
            get
            {
                return user;
            }
        }
        public virtual string PhoneNumber
        {
            get
            {
                return phoneNumber;
            }
            set
            {
                phoneNumber = value;
            }
        }
        public virtual string City
        {
            get
            {
                return city;
            }
            set
            {
                city = value;
            }
        }
        public virtual string Country
        {
            get
            {
                return country;
            }
            set
            {
                country = value;
            }
        }
        public virtual int ZipCode
        {
            get
            {
                return zipCode;
            }
            set
            {
                zipCode = value;
            }
        }
        public virtual string Street
        {
            get
            {
                return street;
            }
            set
            {
                street = value;
            }
        }
    }
}
