﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EmployerServicePortal.Models
{
    public class Users
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}