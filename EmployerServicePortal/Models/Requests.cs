using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EmployerServicePortal.Models
{
    public class Requests
    {
        public string ID { get; set; }
        public string RequestID { get; set; }
        public string Category { get; set; }
        public string Comment { get; set; }
        public string lastmodifieddate { get; set; }
        public string LastModifiedByRoleID { get; set; }
        public string Creator { get; set; }
        public string CreatorRoles { get; set; }
        public string LastModifierRole { get; set; }
        public string CurrentStatus { get; set; }
        public string Datecreated { get; set; }
        public string LastModifiedBy { get; set; }
        public string CurrentAssignedToID { get; set; }
        public string CurrentAssignedRoleID { get; set; }
    }
}