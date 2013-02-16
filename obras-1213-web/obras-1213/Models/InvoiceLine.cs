using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace obras_1213.Models
{
    public class InvoiceLine
    {
        public int ID { get; set; }
        public int InvoiceID { get; set; }
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
        public float Quantity { get; set; }
        public decimal LineTotal { get; set; }

        public InvoiceLine(int id, int invoiceId, string description, decimal unitPrice, float quantity, decimal total)
        {
            ID = id;
            InvoiceID = invoiceId;
            Description = description;
            UnitPrice = unitPrice;
            Quantity = quantity;
            LineTotal = total;
        }
    }
}