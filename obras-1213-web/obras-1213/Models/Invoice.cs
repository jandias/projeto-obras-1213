using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace obras_1213.Models
{
    public class Invoice
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public string State { get; set; }
        public int Discount { get; set; }
        public decimal Total { get; set; }
        public int WorkID { get; set; }
        public int ShopID { get; set; }
        public int CustomerID { get; set; }

        public Invoice() { }
        public Invoice(int id, DateTime date, string state, int discount, decimal total,
            int workId, int shopId, int customerId)
        {
            ID = id;
            Date = date;
            State = state;
            Discount = discount;
            Total = total;
            WorkID = workId;
            ShopID = shopId;
            CustomerID = customerId;
        }

        private Customer customer;
        public Customer Customer
        {
            get
            {
                if (customer != null)
                {
                    return customer;
                }
                customer = Customer.Find(CustomerID);
                return customer;
            }
        }

        public bool Paid
        {
            get
            {
                return State.Equals("paga");
            }
            set
            {
                if (!State.Equals("paga") && value == true)
                {
                    try
                    {
                        using (SqlConnection conn = Db.Utils.NewConnection("REPEATABLE READ"))
                        {
                            using (SqlCommand cmd = new SqlCommand("PagarFactura", conn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                SqlParameter retVal = cmd.Parameters.Add("RetVal", SqlDbType.Int);
                                retVal.Direction = ParameterDirection.ReturnValue;
                                cmd.Parameters.AddWithValue("@factura", ID).Direction = ParameterDirection.Input;
                                cmd.ExecuteNonQuery();
                                if ((int)retVal.Value == 0)
                                {
                                    State = "paga";
                                }
                            }
                        }
                    }
                    catch (SqlException e)
                    {
                        throw new ModelException("Erro na base de dados: " + e.Message, e);
                    }
                }
            }
        }

        public IEnumerable<InvoiceLine> Lines
        {
            get
            {
                using (SqlConnection conn = Db.Utils.NewConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(
                        "select nLinha, descricaoLinha, precoUnit, quant, totalLinha " +
                        " from LinhaFactura " +
                        " where factura=@id order by nLinha asc ", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", ID);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                yield return new InvoiceLine(dr.GetInt32(0), ID, dr.GetString(1), dr.GetDecimal(2),
                                    dr.GetFloat(3), dr.GetDecimal(4));
                            }
                        }
                    }
                }
            }
        }

        public static Invoice Find(Work work)
        {
            try
            {
                using (SqlConnection conn = Db.Utils.NewConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(
                        "select numFact, dataFact, estadoFact, desconto, totalFactura, cliente " +
                        "from Factura where obra = @obra and oficina = @oficina", conn))
                    {
                        cmd.Parameters.AddWithValue("@obra", work.ID);
                        cmd.Parameters.AddWithValue("@oficina", work.ShopID);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                return new Invoice(dr.GetInt32(0), dr.GetDateTime(1), dr.GetString(2),
                                    dr.GetInt32(3), dr.GetDecimal(4), work.ID, work.ShopID, dr.GetInt32(5));
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