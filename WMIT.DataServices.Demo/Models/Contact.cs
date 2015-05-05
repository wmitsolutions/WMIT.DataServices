using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WMIT.DataServices.Model;

namespace WMIT.DataServices.Demo.Models
{
    public class Contact : Entity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public List<Address> Addresses { get; set; }
    }

    public class Address : Entity
    {
        public string Description { get; set; }

        public int ContactId { get; set; }
        public Contact Contact { get; set; }
    }
}