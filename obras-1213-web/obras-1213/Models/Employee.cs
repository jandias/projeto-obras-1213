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

        private string profession;
        public string Profession
        {
            get
            {
                if (null != profession)
                {
                    return profession;
                }
                try
                {
                    using (SqlConnection conn = Db.Utils.NewConnection())
                    {
                        using (SqlCommand cmd = new SqlCommand(
                            "select designacaoProf from Profissao where idProf = @prof", conn))
                        {
                            cmd.Parameters.AddWithValue("@prof", ProfessionID);
                            using (SqlDataReader dr = cmd.ExecuteReader())
                            {
                                if (dr.Read())
                                {
                                    profession = dr.GetString(0);
                                    return profession;
                                }
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new ModelException("Erro na base de dados.", ex.InnerException);
                }
                return "";
            }
        }

        public Employee()
        {
        }

        public Employee(int id, int nif, string name, int phone, string email, string state, int professionId)
        {
            ID = id;
            NIF = nif;
            Name = name;
            Phone = phone;
            Email = email;
            State = state;
            ProfessionID = professionId;
        }

        public static Employee Find(int number)
        {
            try
            {
                using (SqlConnection conn = Db.Utils.NewConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(
                        "select nifFunc, nomeFunc, telefoneFunc, emailFunc, estadoFunc, profissao " +
                        "from Funcionario where codFunc = @EmployeeNumber", conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeNumber", number);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                return new Employee(number, dr.GetInt32(0), dr.GetString(1), dr.GetInt32(2),
                                    dr.GetString(3), dr.GetString(4), dr.GetInt32(5));
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

        public static IEnumerable<Employee> AllEmployees()
        {
            using (SqlConnection conn = Db.Utils.NewConnection())
            {
                using (SqlCommand cmd = new SqlCommand(
                    "select codFunc, nifFunc, nomeFunc, telefoneFunc, emailFunc, estadoFunc, profissao " +
                    "from Funcionario", conn))
                {
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            yield return new Employee(dr.GetInt32(0), dr.GetInt32(1), dr.GetString(2), dr.GetInt32(3),
                                dr.GetString(4), dr.GetString(5), dr.GetInt32(6));
                        }
                    }
                }
            }
        }
    }
}