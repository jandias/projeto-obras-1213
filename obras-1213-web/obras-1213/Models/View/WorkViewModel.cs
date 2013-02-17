using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace obras_1213.Models.View
{
    public class WorkViewModel
    {
        public Work Work { get; set; }

        public WorkViewModel()
        {
        }

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

        public string SerializeToString()
        {
            XmlSerializer ser = new XmlSerializer(typeof(WorkViewModel));
            using (StringWriter sw = new StringWriter())
            {
                ser.Serialize(sw, this);
                return sw.ToString();
            }
        }

        public static WorkViewModel Deserialize(Stream xmlStream)
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(WorkViewModel));
                return (WorkViewModel)ser.Deserialize(xmlStream);
            }
            catch (Exception e)
            {
                throw new ModelException("Erro de processamento: " + e.Message, e);
            }
        }
    }
}
