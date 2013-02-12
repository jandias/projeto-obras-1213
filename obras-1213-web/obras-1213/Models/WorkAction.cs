using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace obras_1213.Models
{
    public class WorkAction : Action
    {
        public int WorkID { get; set; }
        public int EmployeeID { get; set; }
        public float TimeWorked { get; set; }
        public bool Completed { get; set; }

        public WorkAction(int actionid, int department, int shop, string description, float timePredicted,
                          int employee, int work, float timeWorked, bool isCompleted)
            : base(actionid, department, shop, description, timePredicted)
        {
            EmployeeID = employee;
            TimeWorked = timeWorked;
            Completed = isCompleted;
            WorkID = work;
        }

        private Employee employee;
        public Employee Employee
        {
            get
            {
                if (null != employee)
                {
                    return employee;
                }
                employee = Employee.Find(EmployeeID);
                return employee;
            }
        }
    }
}