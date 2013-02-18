using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace obras_1213.Models
{
    public class Communication
    {
        public int ID { get; set; }
        public int DepartmentID { get; set; }
        public int ShopID { get; set; }
        public string Contents { get; set; }

        public Communication() { }

        public Communication( int departmentID, int shopID, string contents )
        {
            DepartmentID = departmentID;
            ShopID = shopID;
            Contents = contents;
        }

        public bool Insert()
        {
            try
            {
                using (SqlConnection conn = Db.Utils.NewConnection)
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "INSERT INTO Comunicado (departamento, oficina, conteudoCom) " +
                        "VALUES (@dep, @shop, @contents) ", conn))
                    {
                        cmd.Parameters.AddWithValue("@dep", DepartmentID);
                        cmd.Parameters.AddWithValue("@shop", ShopID);
                        cmd.Parameters.AddWithValue("@contents", Contents);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new ModelException("Erro na base de dados: " + ex.Message, ex.InnerException);
            }
        }

        public static string FindAllAsXml()
        {
            try
            {
                using (SqlConnection conn = Db.Utils.NewConnection)
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "WITH XMLNAMESPACES ('si2.isel.pt/2013/TrabFinal' as myns)" +
                        "select codOfic as [@codOfic], " +
                        "(select codDep as [@codDep]," +
                        "  (select idCom as [@idCom], conteudoCom.query('" +
                        "    myns:comunicado/*') from Comunicado where Comunicado.oficina = Oficina.codOfic AND Comunicado.departamento = Departamento.codDep" +
                        "    for xml path ('myns:comunicado'), type) as [*]" +
                        "  from Departamento where Departamento.oficina = Oficina.codOfic" +
                        "  for xml path ('myns:departamento'), type) as [*]" +
                        " from Oficina for xml path ('myns:oficina'), root('myns:comunicados')", conn))
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if ( dr.Read())
                            {
                                return dr.GetString(0);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new ModelException("Erro na base de dados: " + ex.Message, ex.InnerException);
            }
            return "";
        }

    }
}