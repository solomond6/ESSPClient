using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployerServicePortal.Models
{
    public class BatchList
    {
        public string ID { get; set; }
        public string AddressId { get; set; }
        public string BatchName { get; set; }
        public string RecipientName { get; set; }
        public string Phone { get; set; }
        public string RecipientName2 { get; set; }
        public string Phone2 { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public string LGA { get; set; }
    }
}