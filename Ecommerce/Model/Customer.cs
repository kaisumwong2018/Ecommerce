using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Model
{
    public class Customer
    {
        [Key]//[Key] is set bottom line as primary key

        public int CustomerId { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public int LoginTry { get; set; }
        public DateTime LastLoginTime { get; set; }
        public string? Status { get; set; }
        public string? FullName { get; set; }
        public string? NRIC { get; set; }
        public int AddressId { get; set; }
        public Address? Address { get; set; }
        //U Can Link "Address" Class 
    }
}
