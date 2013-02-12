using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace obras_1213.Models
{
    public class WorkPart : Part
    {
        public int Quantity { get; set; }

        public WorkPart(string id, string description, decimal price, int quantity)
            : base(id, description, price)
        {
            Quantity = quantity;
        }
    }
}