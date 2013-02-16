using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace obras_1213.Models.View
{
    public class WorkViewModel
    {
        public Work Work { get; set; }

        public WorkViewModel(Work w)
        {
            Work = w;
        }

        public IEnumerable<SelectListItem> PartsToSelect
        {
            get
            {
                foreach (Part p in Part.FindAll())
                {
                    yield return new SelectListItem() { Value = p.ID, Text = p.Description };
                }
            }
        }

        public IEnumerable<SelectListItem> ActionsToSelect
        {
            get
            {
                foreach (Action a in Action.FindAll())
                {
                    yield return new SelectListItem() { Value = a.ID.ToString(), Text = a.Description };
                }
            }
        }

        public IEnumerable<SelectListItem> EmployeeToSelect
        {
            get
            {
                foreach (Employee e in Employee.AllEmployees())
                {
                    yield return new SelectListItem() { Value = e.ID.ToString(), Text = e.Name };
                }
            }
        }

        public IEnumerable<SelectListItem> CustomersToSelect
        {
            get
            {
                foreach (Customer c in Customer.FindAll())
                {
                    yield return new SelectListItem() { Value = c.ID.ToString(), Text = c.Nome };
                }
            }
        }
    }
}
