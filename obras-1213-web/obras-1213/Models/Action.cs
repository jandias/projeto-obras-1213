using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace obras_1213.Models
{
    public class Action
    {
        public int ID { get; set; }
        public int DepartmentID { get; set; }
        public int ShopID { get; set; }
        public string Description { get; set; }
        public float EstimatedTime { get; set; }

        public Action()
        {
        }

        public Action(int id, int department, int shop, string description, float time)
        {
            ID = id;
            DepartmentID = department;
            ShopID = shop;
            Description = description;
            EstimatedTime = time;
        }

        public static Action Find(int id)
        {
            try
            {
                using (SqlConnection conn = Db.Utils.NewConnection)
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "select departamento, oficina, designacao, horasEstimadas " +
                        "from Acto where idA = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                return new Action(id, dr.GetInt32(0), dr.GetInt32(1), dr.GetString(2),
                                    dr.GetFloat(3));
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