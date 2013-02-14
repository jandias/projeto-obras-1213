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
    }
}