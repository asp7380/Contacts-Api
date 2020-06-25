using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contacts_Api.Models
{
    public class Phone
    {
        public int Id { get; set; }
        public int fId { get; set; }
        public string phoneNbr { get; set; }
        public string phoneType { get; set; }
    }
}