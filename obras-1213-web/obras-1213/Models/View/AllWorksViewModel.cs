using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace obras_1213.Models.View
{
    public class AllWorksViewModel
    {
        public DateTime OpenSince { get; set; }
        public DateTime ClosedSince { get; set; }

        public IEnumerable<Work> ClosedWorks
        {
            get
            {
                return Work.FindAll(false, ClosedSince);
            }
        }

        public IEnumerable<Work> OpenWorks
        {
            get
            {
                return Work.FindAll(true, OpenSince);
            }
        }

        public AllWorksViewModel()
        {
            OpenSince = DateTime.Now.AddMonths(-1);
            ClosedSince = DateTime.Now.AddMonths(-1);
        }
    }
}