using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EmployerServicePortal.Models
{
    public class CompanyEmployee
    {
        public string Title { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Middle Names")]
        public string MiddleName { get; set; }

        public string PhoneNo { get; set; }

        [Display(Name = "Primary Contact No")]
        public string PrimaryContactNo { get; set; }

        [Display(Name = "Date of Birth")]
        public string DateOfBirth { get; set; }

        [Display(Name = "Marital Status")]
        public string MaritalStatus { get; set; }

        [Display(Name = "State of Origin")]
        public string StateOfOrigin { get; set; }

        [Display(Name = "Date of Employment")]
        public string DateOfEmployment { get; set; }


        [Display(Name = "E-Mail")]
        public string Email { get; set; }

        [Display(Name = "Local Government Authority")]
        public string LGA { get; set; }

        [Display(Name = "E-Mail")]
        public string Address { get; set; }

        [Display(Name = "Address 2")]
        public string Address2 { get; set; }
        public string City { get; set; }
        public string Religion { get; set; }
        public string Qualification { get; set; }

        public string Picture { get; set; }
        public string Sex { get; set; }
        public string State { get; set; }

        [Display(Name = "P_I_N")]
        public string PIN { get; set; }
        public string dPIN { get; set; }
        public string Name { get; set; }
        public string Nationality { get; set; }
        public string Status { get; set; }
        public string Verified { get; set; }


    }
}