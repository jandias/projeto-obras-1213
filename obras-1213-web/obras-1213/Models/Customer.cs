using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace obras_1213.Models
{
    public class Customer
    {
        public int ID { get; set; }
        [Required(ErrorMessage="É necessário especificar o NIF")]
        public int NIF { get; set; }
        [Required(ErrorMessage = "É necessário especificar o nome")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "É necessário especificar a morada")]
        public string Morada { get; set; }
        [Required(ErrorMessage = "É necessário especificar o telefone")]
        public string Telefone { get; set; }
        [Required(ErrorMessage = "É necessário especificar o email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "É necessário especificar o tipo do cliente")]
        public string Tipo { get; set; }

        public Customer()
        {
        }

        public Customer(int id, int nif, string nome, string morada, string telefone, string email, string tipo)
        {
            ID = id;
            NIF = nif;
            Nome = nome;
            Morada = morada;
            Telefone = telefone;
            Email = email;
            Tipo = tipo;
        }

        public bool Update()
        {
            try
            {
                using (SqlConnection conn = Db.Utils.NewConnection)
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "update Cliente set " + 
                        "nifCli=@nif, nomeCli=@nome, moradaCli=@morada, telefoneCli=@telefone, emailCli=@email, tipoCli=@tipo " +
                        "where numCli = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", ID);
                        cmd.Parameters.AddWithValue("@nif", NIF);
                        cmd.Parameters.AddWithValue("@nome", Nome);
                        cmd.Parameters.AddWithValue("@morada", Morada);
                        cmd.Parameters.AddWithValue("@telefone", Telefone);
                        cmd.Parameters.AddWithValue("@email", Email);
                        cmd.Parameters.AddWithValue("@tipo", Tipo);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new ModelException("Erro na base de dados.", ex.InnerException);
            }
        }

        public bool Insert()
        {
            try
            {
                using (SqlConnection conn = Db.Utils.NewConnection)
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "INSERT INTO Cliente (nifCli, nomeCli, moradaCli, telefoneCli, emailCli, tipoCli) " +
                        "VALUES (@nif, @nome, @morada, @telefone, @email, @tipo) ", conn))
                    {
                        cmd.Parameters.AddWithValue("@nif", NIF);
                        cmd.Parameters.AddWithValue("@nome", Nome);
                        cmd.Parameters.AddWithValue("@morada", Morada);
                        cmd.Parameters.AddWithValue("@telefone", Telefone);
                        cmd.Parameters.AddWithValue("@email", Email);
                        cmd.Parameters.AddWithValue("@tipo", Tipo);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new ModelException("Erro na base de dados: " + ex.Message, ex.InnerException);
            }
        }

        public static Customer Find(int id)
        {
            try
            {
                using (SqlConnection conn = Db.Utils.NewConnection)
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "select nifCli, nomeCli, moradaCli, telefoneCli, emailCli, tipoCli " +
                        "from Cliente where numCli = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                return new Customer(id, dr.GetInt32(0), dr.GetString(1), dr.GetString(2),
                                    dr.GetString(3), dr.GetString(4), dr.GetString(5));
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

        public static IEnumerable<Customer> FindAll()
        {
            using (SqlConnection conn = Db.Utils.NewConnection)
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "select nifCli, nomeCli, moradaCli, telefoneCli, emailCli, tipoCli, numCli " +
                    "from Cliente", conn))
                {
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            yield return new Customer(dr.GetInt32(6), dr.GetInt32(0), dr.GetString(1), dr.GetString(2),
                                dr.GetString(3), dr.GetString(4), dr.GetString(5));
                        }
                    }
                }
            }
        }
    }
}