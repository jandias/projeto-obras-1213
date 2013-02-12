using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace obras_1213.Models
{
    public class Employee
    {
        public int ID { get; set; }
        public int NIF { get; set; }
        public string Name { get; set; }
        public int Phone { get; set; }
        public string Email { get; set; }
        public string State { get; set; }
        public int ProfessionID { get; set; }

        public static Employee Find(int number)
        {
            try
            {
                using (SqlConnection conn = Db.Utils.NewConnection)
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "select nifFunc, nomeFunc, telefoneFunc, emailFunc, estadoFunc, profissao " +
                        "from Funcionario where codFunc = @EmployeeNumber", conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeNumber", number);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                Employee emp = new Employee();
                                emp.ID = number;
                                emp.NIF = dr.GetInt32(0);
                                emp.Name = dr.GetString(1);
                                emp.Phone = dr.GetInt32(2);
                                emp.Email = dr.GetString(3);
                                emp.State = dr.GetString(4);
                                emp.ProfessionID = dr.GetInt32(5);
                                return emp;
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new ModelException("Erro na base de dados.", ex.InnerException);
            }
            return null;
        }
    }
}