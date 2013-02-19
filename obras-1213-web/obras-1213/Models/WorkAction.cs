using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

        public WorkAction() { }
        public WorkAction(int actionid, int department, int shop, string description, float timePredicted,
                          int employee, int work, float timeWorked, bool isCompleted)
            : base(actionid, department, shop, description, timePredicted)
        {
            EmployeeID = employee;
            TimeWorked = timeWorked;
            Completed = isCompleted;
            WorkID = work;
        }

        public bool CurrentUserCanComplete
        {
            get
            {
                Employee currentEmployee = (Employee)HttpContext.Current.Session["employee"];
                if (currentEmployee == null)
                {
                    int id;
                    if (Int32.TryParse(HttpContext.Current.User.Identity.Name, out id))
                    {
                        currentEmployee = Employee.Find(id);
                    }
                }
                return (currentEmployee != null ? currentEmployee.ID == Employee.ID : false);
            }
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

        public bool CommitTime()
        {
            try
            {
                using (SqlConnection conn = Db.Utils.NewConnection)
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "UPDATE ObraContem SET horasRealizadas = @horas " +
                        "WHERE obra=@obra AND oficina=@oficina AND acto=@acto AND departamento=@departamento",
                        conn))
                    {
                        cmd.Parameters.AddWithValue("@horas", TimeWorked);
                        cmd.Parameters.AddWithValue("@obra", WorkID);
                        cmd.Parameters.AddWithValue("@oficina", ShopID);
                        cmd.Parameters.AddWithValue("@acto", ID);
                        cmd.Parameters.AddWithValue("@departamento", DepartmentID);
                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            return true;
                        }
                        else
                        {
                            throw new ModelException("Não foi possível alterar o estado do acto.");
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new ModelException("Erro na base de dados: " + ex.Message, ex.InnerException);
            }
        }

        public bool Remove()
        {
            try
            {
                using (SqlConnection conn = Db.Utils.NewConnection)
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("RetiraActoObra", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@obra", WorkID).Direction = ParameterDirection.Input;
                        cmd.Parameters.AddWithValue("@acto", ID).Direction = ParameterDirection.Input;
                        cmd.Parameters.AddWithValue("@departamento", DepartmentID).Direction = ParameterDirection.Input;
                        cmd.Parameters.AddWithValue("@oficina", ShopID).Direction = ParameterDirection.Input;
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new ModelException("Erro na base de dados: " + ex.Message, ex.InnerException);
            }
        }

        public bool Complete()
        {
            try
            {
                using (SqlConnection conn = Db.Utils.NewConnection)
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "UPDATE ObraContem SET estaConcluido = @concluido " +
                        "WHERE obra=@obra AND oficina=@oficina AND acto=@acto AND departamento=@departamento",
                        conn))
                    {
                        cmd.Parameters.AddWithValue("@concluido", 1);
                        cmd.Parameters.AddWithValue("@obra", WorkID);
                        cmd.Parameters.AddWithValue("@oficina", ShopID);
                        cmd.Parameters.AddWithValue("@acto", ID);
                        cmd.Parameters.AddWithValue("@departamento", DepartmentID);
                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            Completed = true;
                            return true;
                        }
                        else
                        {
                            throw new ModelException("Não foi possível alterar o estado do acto.");
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new ModelException("Erro na base de dados: " + ex.Message, ex.InnerException);
            }
        }
    }
}