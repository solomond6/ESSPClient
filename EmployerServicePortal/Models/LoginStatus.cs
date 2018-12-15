using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EmployerServicePortal.Models
{
    public class LoginStatus
    {
        public string ErrorMessage { get; set; }
        public string LoginStat { get; set; }
        public string WebUserID { get; set; }
        public string EMPLOYER_ID { get; set; }
        public string EnforceChange { get; set; }
        public string CUSTODIAN_ID { get; set; }
        public string LastLogin { get; set; }
        public string email { get; set; }
        public string BrowserUsed { get; set; }
        public string ROLE_ID { get; set; }
        public string FULLNAME { get; set; }
    }
}