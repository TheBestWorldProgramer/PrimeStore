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
        private int userId;
        private string login;
        private string firstName;
        private string lastName;
        private string email;
        private Role role;
        private ICollection<Product> products;

        public virtual int UserId
        {
            get
            {
                return userId;
            }
        }

        public virtual string Login
        {
            get
            {
                return login;
            }

            set
            {
                login = value;
            }
        }

        public virtual string FirstName
        {
            get
            {
                return firstName;
            }

            set
            {
                firstName = value;
            }
        }

        public virtual string LastName
        {
            get
            {
                return lastName;
            }

            set
            {
                lastName = value;
            }
        }

        public virtual string Email
        {
            get
            {
                return email;
            }

            set
            {
                email = value;
            }
        }

        public virtual Role Role
        {
            get
            {
                return role;
            }
        }

        public virtual ICollection<Product> Products
        {
            get
            {
                return products?? new List<Product>();
            }
        }
    }
}
