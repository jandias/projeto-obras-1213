using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace obras_1213.Models
{
    public class Department
    {
        public int ID { get; set; }
        public int ShopID { get; set; }
        public string Name { get; set; }

        public Department(int id, int shopId, string name)
        {
            ID = id;
            ShopID = shopId;
            Name = name;
        }

        public static IEnumerable<Department> FindAll( int shopId )
        {
            using (SqlConnection conn = Db.Utils.NewConnection())
            {
                using (SqlCommand cmd = new SqlCommand(
                    "select codDep, nomeDep " +
                    "from Departamento " +
                    "where oficina=@shop", conn))
                {
                    cmd.Parameters.AddWithValue("@shop", shopId);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            yield return new Department(dr.GetInt32(0), shopId, dr.GetString(1));
                        }
                    }
                }
            }
        }
    }
}
