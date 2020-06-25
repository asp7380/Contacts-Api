using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contacts_Api.Models
{
    public class Address
    {
        public int Id { get; set; }
        public int fId { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        //public string PhoneNbr { get; set; }
        //public string PhoneType { get; set; }
        //public string Email { get; set; }
    }
}