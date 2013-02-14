using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace obras_1213.Models
{
    public class WorkPart : Part
    {
        [Required(ErrorMessage="Tem de especificar a quantidade de peças a usar.")]
        public int Quantity { get; set; }

        public WorkPart()
        {
        }

        public WorkPart(string id, string description, decimal price, int quantity)
            : base(id, description, price)
        {
            Quantity = quantity;
        }
    }
}