using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EmployerServicePortal.Models
{
    public class ForgotPassword
    {
        public string Message { get; set; }
        public string LoginStat { get; set; }
        public string RSAPIN { get; set; }
    }
}