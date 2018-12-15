using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployerServicePortal.Models
{
    public class Batch
    {
        [NotNull]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string BatchId{ get; set; }

        [NotNull]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string BatchName { get; set; }

        [NotNull]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ContactName { get; set; }

        [NotNull]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ContactName2 { get; set; }

        [NotNull]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ContactEmail { get; set; }

        [NotNull]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string AddressId { get; set; }

        [NotNull]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ContactPhone { get; set; }

        [NotNull]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ContactPhone2 { get; set; }

        [NotNull]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ContactAddress { get; set; }

        [NotNull]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ContactAddress1 { get; set; }

        [NotNull]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ContactState { get; set; }

        [NotNull]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ContactLGA { get; set; }


        [NotNull]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string statementOption { get; set; }

        [NotNull]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public List<string> id { get; set; }
    }

    public sealed class NotNullAttribute : RequiredAttribute
    {
        public NotNullAttribute()
        {
            AllowEmptyStrings = true;
        }
    }
}