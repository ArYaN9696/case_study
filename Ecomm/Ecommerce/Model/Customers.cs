using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Model
{
    public class Customers
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        // Default constructor
        public Customers() { }

        // Parameterized constructor
        public Customers(string name, string email, string password)
        {
           // CustomerId = customerId;
            Name = name;
            Email = email;
            Password = password;
        }
    }
}
