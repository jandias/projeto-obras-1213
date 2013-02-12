﻿using obras_1213.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace obras_1213.Controllers
{
    public class BaseController : Controller
    {
        protected Employee Employee
        {
            get
            {
                return (Employee)Session["employee"];
            }
            set
            {
                Session["employee"] = value;
            }
        }
    }
}