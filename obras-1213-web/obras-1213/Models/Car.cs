using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace obras_1213.Models
{
    public class Car
    {
        public string License { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string OwnerName { get; set; }

        public Car()
        {
        }

        public Car(string license, string brand, string model, string owner)
        {
            License = license;
            Brand = brand;
            Model = model;
            OwnerName = owner;
        }

        public static Car Find(string license)
        {
            try
            {
                using (SqlConnection conn = Db.Utils.NewConnection)
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "select marca, modelo, nomeProprietario " +
                        "from Veiculo where matricula = @license", conn))
                    {
                        cmd.Parameters.AddWithValue("@license", license);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                return new Car(license, dr.GetString(0), dr.GetString(1), dr.GetString(2));
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

        public static IEnumerable<Car> FindAll()
        {
            using (SqlConnection conn = Db.Utils.NewConnection)
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "select marca, modelo, nomeProprietario, matricula " +
                    "from Veiculo", conn))
                {
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            yield return new Car(dr.GetString(3), dr.GetString(0), dr.GetString(1), dr.GetString(2));
                        }
                    }
                }
            }
        }
    }
}