using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace obras_1213.Models
{
    public class Shop
    {
        public int ID { get; set; }
        public int NIF { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int Phone { get; set; }
        public int Fax { get; set; }
        public int ManagerID { get; set; }

        public Shop(int id, int nif, string name, string address, int phone, int fax, int manager)
        {
            ID = id;
            NIF = nif;
            Name = name;
            Address = address;
            Phone = phone;
            Fax = fax;
            ManagerID = manager;
        }

        public static string FindAllXml()
        {
            try
            {
                using (SqlConnection conn = Db.Utils.NewConnection)
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "select " +
                        "	Oficina.codOfic, Oficina.faxOfic, Oficina.moradaOfic, Oficina.nifOfic, Oficina.nomeOfic, Oficina.telefoneOfic, " +
                        "	Responsavel.nomeFunc " +
                        "from Oficina " +
                        "join Funcionario as Responsavel on Responsavel.codFunc=Oficina.responsavel " +
                        "for xml auto, root('Oficinas')", conn))
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                return dr.GetString(0);
                            }
                        }
                    }
                }
            }
            catch (SqlException)
            {
            }
            return "";
        }

        public static IEnumerable<Shop> FindAll()
        {
            using (SqlConnection conn = Db.Utils.NewConnection)
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "select codOfic, nifOfic, nomeOfic, moradaOfic, telefoneOfic, faxOfic, responsavel " +
                    "from Oficina", conn))
                {
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            yield return new Shop(dr.GetInt32(0), dr.GetInt32(1), dr.GetString(2), dr.GetString(3),
                                dr.GetInt32(4), dr.GetInt32(5), dr.GetInt32(6));
                        }
                    }
                }
            }
        }
    }
}
