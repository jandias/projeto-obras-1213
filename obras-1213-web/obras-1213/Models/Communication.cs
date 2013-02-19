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

        public string Type { get; set; }
        public string Text { get; set; }
        public string UrlPrint { get; set; }
        public string UrlMedia { get; set; }
        public string Author { get; set; }
        public string PublishDate { get; set; }
        public string EnforcementDate { get; set; }

        public Communication() { }

        public Communication( int departmentID, int shopID, string contents )
        {
            DepartmentID = departmentID;
            ShopID = shopID;
            Contents = contents;
        }

        public Communication(int departmentID, int shopID, string author, string publish, string enforce, string type, 
            string text, string print, string media )
        {
            DepartmentID = departmentID;
            ShopID = shopID;
            Text = text;
            UrlPrint = print;
            UrlMedia = media;
            Author = author;
            PublishDate = publish;
            EnforcementDate = enforce;
        }

        public bool Insert()
        {
            try
            {
                using (SqlConnection conn = Db.Utils.NewConnection())
                {
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
                using (SqlConnection conn = Db.Utils.NewConnection())
                {
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

        public static IEnumerable<Communication> List(DateTime publishDate)
        {
            using (SqlConnection conn = Db.Utils.NewConnection())
            {
                using (SqlCommand cmd = new SqlCommand(
                    "with xmlnamespaces ('si2.isel.pt/2013/TrabFinal' as myns) " +
                    "select  " +
                    "	departamento,  " +
                    "	oficina,  " +
                    "	conteudoCom.value('(/myns:comunicado/myns:autor)[1]', 'varchar(max)') as autor, " +
                    "	conteudoCom.value('(/myns:comunicado/myns:data)[1]', 'varchar(max)') as dataPublicacao, " +
                    "	conteudoCom.value('(/myns:comunicado/myns:data/@entrada-em-vigor)[1]', 'varchar(max)') as dataEntradaVigor, " +
                    "	conteudoCom.value('(/myns:comunicado/myns:tipo)[1]', 'varchar(max)') as tipo, " +
                    "	conteudoCom.value('(/myns:comunicado/myns:conteudo)[1]', 'varchar(max)') as conteudo, " +
                    "	conteudoCom.value('(/myns:comunicado/myns:urlPrint)[1]', 'varchar(max)') as urlPrint, " +
                    "	conteudoCom.value('(/myns:comunicado/myns:urlMedia)[1]', 'varchar(max)') as urlMedia " +
                    "from Comunicado " +
                    "where conteudoCom.value('(/myns:comunicado/myns:data)[1]', 'datetime') = @data ", 
                    conn))
                {
                    cmd.Parameters.AddWithValue("@data", publishDate);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            yield return new Communication(dr.GetInt32(0), dr.GetInt32(1),
                                dr.IsDBNull(2) ? "" : dr.GetString(2), dr.IsDBNull(3) ? "" : dr.GetString(3),
                                dr.IsDBNull(4) ? "" : dr.GetString(4), dr.IsDBNull(5) ? "" : dr.GetString(5),
                                dr.IsDBNull(6) ? "" : dr.GetString(6), dr.IsDBNull(7) ? "" : dr.GetString(7),
                                dr.IsDBNull(8) ? "" : dr.GetString(8));
                        }
                    }
                }
            }
        }
    }
}