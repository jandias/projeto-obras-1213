using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace obras_1213.Models.View
{
    public class NewWorkViewModel
    {
        [Required(ErrorMessage = "É preciso indicar a oficina a que a obra pertence.")]
        public int ShopID { get; set; }
        [Required(ErrorMessage="É necessário atribuir uma viatura à obra.")]
        public string CarLicensePlate { get; set; }
        [Required(ErrorMessage = "Tem de definir pelo menos um acto a executar.")]
        [AllowHtml]
        public string[] Actions { get; set; }

        public IEnumerable<SelectListItem> ShopsForSelect
        {
            get
            {
                foreach (Shop s in Shop.FindAll())
                {
                    yield return new SelectListItem() { Value = s.ID.ToString(), Text = s.Name };
                }
            }
        }

        public IEnumerable<SelectListItem> ActionsForSelect
        {
            get
            {
                foreach (Action a in Action.FindAll())
                {
                    yield return new SelectListItem() { Value = a.SerializeToString(), Text = a.Description };
                }
            }
        }

        public IEnumerable<SelectListItem> CarsForSelect
        {
            get
            {
                foreach (Car c in Car.FindAll())
                {
                    yield return new SelectListItem() { Value = c.License, Text = c.Brand + " " + c.Model + ", " + c.License };
                }
            }
        }
    }
}