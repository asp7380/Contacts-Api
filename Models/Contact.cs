using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contacts_Api.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public string First { get; set; }
        public string Middle { get; set; }
        public string Last { get; set; }
        public string Email { get; set; }
        public IList<Address> address { get; set; }
        public IList<Phone> phone { get; set; }
    }
}