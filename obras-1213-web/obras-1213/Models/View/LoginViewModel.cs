using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace obras_1213.Models.View
{
    public class LoginViewModel
    {
        [Required(ErrorMessage="Tem de indicar o número de funcionário.")]
        public int EmployeeID { get; set; }
        [Required(ErrorMessage = "É preciso definir a oficina.")]
        public int ShopID { get; set; }

        public IEnumerable<SelectListItem> ShopsToSelect
        {
            get
            {
                foreach (Shop s in Shop.FindAll())
                {
                    yield return new SelectListItem() { Value = s.ID.ToString(), Text = s.Name };
                }
            }
        }
    }
}