using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomerBook.Models
{
    public class CustomerModel
    {
        //To change label title value  
        //[DisplayName("Member Name")]
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Birthday { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Company { get; set; }
        public string Adress { get; set; }
        public string Job { get; set; }

        public HttpPostedFileBase ImageFile { get; set; }
    }
}