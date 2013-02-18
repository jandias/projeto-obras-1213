using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace obras_1213.Models.View
{
    public class CommsViewModel
    {
        public IEnumerable<SelectListItem> ShopsToSelect
        {
            get
            {
                yield return new SelectListItem() { Value = "-1", Text = "--seleccione oficina--" };
                foreach (Shop s in Shop.FindAll())
                {
                    yield return new SelectListItem() { Value = s.ID.ToString(), Text = s.Name };
                }
            }
        }

        public IEnumerable<SelectListItem> DepartmentsToSelect
        {
            get
            {
                yield return new SelectListItem() { Value = "-1", Text = "--seleccione departamento--" };
            }
        }

        public static IEnumerable<SelectListItem> GetDepartments(int shopId) {

            yield return new SelectListItem() { Value = "-1", Text = "--seleccione departamento--" };
            foreach (Department d in Department.FindAll(shopId))
            {
                yield return new SelectListItem() { Value = d.ID.ToString(), Text = d.Name };
            }
        }

    }
}