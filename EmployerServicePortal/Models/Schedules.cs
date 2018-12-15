using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EmployerServicePortal.Models
{
    public class Schedules
    {
        public string SN { get; set; }
        public string ClientName { get; set; }
        public string RSAPIN { get; set; }
        public string EmployeeMandatory { get; set; }
        public string EmployeeMandatory1 { get; set; }
        public string EmployeeVC { get; set; }
        public string EmployeeVC1 { get; set; }
        public string Total { get; set; }
        public string Status { get; set; }
    }
}