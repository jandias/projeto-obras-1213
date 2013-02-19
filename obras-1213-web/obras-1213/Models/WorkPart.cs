using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace obras_1213.Models
{
    public class WorkPart : Part
    {
        [Required(ErrorMessage="Tem de especificar a quantidade de peças a usar.")]
        public int Quantity { get; set; }

        private Work parentWork;

        public WorkPart()
        {
        }

        public WorkPart(string id, string description, decimal price, int quantity, Work work)
            : base(id, description, price)
        {
            Quantity = quantity;
            parentWork = work;
        }

        public bool Remove()
        {
            try
            {
                using (SqlConnection conn = Db.Utils.NewConnection("READ COMMITTED"))
                {
                    using (SqlCommand cmd = new SqlCommand("RetiraPecaObra", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter retVal = cmd.Parameters.Add("RetVal", SqlDbType.Int);
                        retVal.Direction = ParameterDirection.ReturnValue;
                        cmd.Parameters.AddWithValue("@peca", ID).Direction = ParameterDirection.Input;
                        cmd.Parameters.AddWithValue("@oficina", parentWork.ShopID).Direction = ParameterDirection.Input;
                        cmd.Parameters.AddWithValue("@obra", parentWork.ID).Direction = ParameterDirection.Input;
                        cmd.ExecuteNonQuery();
                        if ((int)retVal.Value == 0)
                        {
                            return true;
                        }
                        return false;
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new ModelException("Erro na base de dados: " + ex.Message, ex.InnerException);
            }
        }
    }
}