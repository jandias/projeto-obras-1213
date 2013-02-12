using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace obras_1213.Models
{
    public class Part
    {
        public string ID { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public Part()
        {
        }

        public Part(string id, string description, decimal price)
        {
            ID = id;
            Description = description;
            Price = price;
        }

        public static Part Find(string id)
        {
            try
            {
                using (SqlConnection conn = Db.Utils.NewConnection)
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "select designacaoP, precoP " +
                        "from Peca where refP = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                return new Part(id, dr.GetString(0), dr.GetDecimal(1));
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