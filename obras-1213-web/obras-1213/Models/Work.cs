using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace obras_1213.Models
{
    public class Work
    {
        public int ID { get; set; }
        [Required(ErrorMessage="É preciso indicar a oficina da obra.")]
        public int ShopID { get; set; }
        [Required(ErrorMessage = "É preciso indicar a date de registo.")]
        public DateTime IssuanceDate { get; set; }
        public decimal PredictedValue { get; set; }
        public float PredictedTime { get; set; }
        public string CarLicense { get; set; }

        private string state;

        public Work(int shop, string car)
        {
            ShopID = shop;
            CarLicense = car;
            IssuanceDate = DateTime.Now;
            state = "marcada";
            PredictedValue = 0;
            PredictedTime = 0;
        }

        public Work(int id, int shop, DateTime date, string state, decimal value, float time, string car)
        {
            ID = id;
            ShopID = shop;
            IssuanceDate = date;
            this.state = state;
            PredictedValue = value;
            PredictedTime = time;
            CarLicense = car;
        }

        public string State 
        {
            get
            {
                return state;
            }
            set
            {
                if (!state.Equals(value))
                {
                    try
                    {
                        using (SqlConnection conn = Db.Utils.NewConnection)
                        {
                            conn.Open();
                            using (SqlCommand cmd = new SqlCommand(
                                "UPDATE Obra SET estadoO = @estado WHERE codO=@obra AND oficina=@oficina",
                                conn))
                            {
                                cmd.Parameters.AddWithValue("@estado", value);
                                cmd.Parameters.AddWithValue("@obra", ID);
                                cmd.Parameters.AddWithValue("@oficina", ShopID);
                                if (cmd.ExecuteNonQuery() > 0)
                                {
                                    state = value;
                                    if (state.Equals("facturada"))
                                    {

                                    }
                                    else if (state.Equals("paga"))
                                    {
                                    }
                                }
                                else
                                {
                                    throw new ModelException("Não foi possível alterar o estado da obra.");
                                }
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

        public bool Invoiced
        {
            get
            {
                return State.Equals("facturada");
            }
        }

        public bool Paid
        {
            get
            {
                return State.Equals("paga");
            }
        }

        public bool Closed
        {
            get
            {
                return State.Equals("facturada") || State.Equals("paga") || State.Equals("concluída");
            }
        }

        private Invoice WorkInvoice;
        public Invoice Invoice
        {
            get
            {
                if (!Invoiced && !Paid)
                {
                    return null;
                }
                if (WorkInvoice != null)
                {
                    return WorkInvoice;
                }
                WorkInvoice = Invoice.Find(this);
                return WorkInvoice;
            }
        }

        private Car WorkCar;
        public Car Car
        {
            get
            {
                if (WorkCar != null)
                {
                    return WorkCar;
                }
                WorkCar = Car.Find(CarLicense);
                return WorkCar;
            }
        }

        public IEnumerable<WorkAction> Actions
        {
            get
            {
                using (SqlConnection conn = Db.Utils.NewConnection)
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "select oc.acto, oc.departamento, oc.funcionario, oc.horasRealizadas, oc.estaConcluido, " +
	                    " a.oficina, a.designacaoA, a.horasEstimadas from ObraContem oc " +
                        " join Acto a on oc.acto=a.idA and oc.departamento=a.departamento and oc.oficina=a.oficina " +
                        " where oc.obra=@id ", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", ID);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                yield return new WorkAction(dr.GetInt32(0), dr.GetInt32(1), dr.GetInt32(5), dr.GetString(6),
                                    dr.GetFloat(7), dr.GetInt32(2), ID, dr.GetFloat(3), dr.GetBoolean(4));
                            }
                        }
                    }
                }
            }
        }

        public IEnumerable<WorkPart> Parts
        {
            get
            {
                using (SqlConnection conn = Db.Utils.NewConnection)
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "select p.refP, p.designacaoP, p.precoP, r.quantP from Reserva r " +
                        " join Peca p on r.peca=p.refP " +
                        " where r.obra=@id ", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", ID);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                yield return new WorkPart(dr.GetString(0), dr.GetString(1), dr.GetDecimal(2), dr.GetInt32(3), this);
                            }
                        }
                    }
                }
            }
        }

        public bool AddPart(WorkPart part)
        {
            try
            {
                using (SqlConnection conn = Db.Utils.NewConnection)
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("AdicionaPecaObra", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter retVal = cmd.Parameters.Add("RetVal", SqlDbType.Int);
                        retVal.Direction = ParameterDirection.ReturnValue;
                        cmd.Parameters.AddWithValue("@peca", part.ID).Direction = ParameterDirection.Input;
                        cmd.Parameters.AddWithValue("@oficina", ShopID).Direction = ParameterDirection.Input;
                        cmd.Parameters.AddWithValue("@obra", ID).Direction = ParameterDirection.Input;
                        cmd.Parameters.AddWithValue("@quantidade", part.Quantity).Direction = ParameterDirection.Input;
                        cmd.ExecuteNonQuery();
                        return (int)retVal.Value == 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new ModelException("Erro na base de dados: " + ex.Message, ex.InnerException);
            }
        }

        public static Work Find(int id)
        {
            try
            {
                using (SqlConnection conn = Db.Utils.NewConnection)
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "select oficina, dataRegistoO, estadoO, valorEstimado, totalHorasEstimado, veiculo " +
                        "from Obra where codO = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                return new Work(id, dr.GetInt32(0), dr.GetDateTime(1), dr.GetString(2),
                                    dr.GetDecimal(3), dr.GetFloat(4), dr.GetString(5));
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

        public static IEnumerable<Work> FindAll()
        {
            using (SqlConnection conn = Db.Utils.NewConnection)
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "select oficina, dataRegistoO, estadoO, valorEstimado, totalHorasEstimado, veiculo, codO " +
                    "from Obra", conn))
                {
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            yield return new Work(dr.GetInt32(6), dr.GetInt32(0), dr.GetDateTime(1), dr.GetString(2),
                                dr.GetDecimal(3), dr.GetFloat(4), dr.GetString(5));
                        }
                    }
                }
            }
        }

        public static IEnumerable<Work> FindAll(bool open, DateTime since)
        {
            string sql = 
                    "select oficina, dataRegistoO, estadoO, valorEstimado, totalHorasEstimado, veiculo, codO " +
                    "from Obra where estadoO " + (open ? "" : "not") + " in ('marcada', 'em realização', 'espera peças') " +
                    " and dataRegistoO >= @data ";
            using (SqlConnection conn = Db.Utils.NewConnection)
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@data", since);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            yield return new Work(dr.GetInt32(6), dr.GetInt32(0), dr.GetDateTime(1), dr.GetString(2),
                                dr.GetDecimal(3), dr.GetFloat(4), dr.GetString(5));
                        }
                    }
                }
            }
        }

        [XmlRoot(ElementName="actos")]
        public class Actos
        {
            public class InfoActo
            {
                public int id { get; set; }
                public int departamento { get; set; }
            }

            [XmlElement("acto")]
            public InfoActo[] acto { get; set; }

            public Actos()
            {
            }

            public Actos(string[] actions)
            {
                acto = new InfoActo[actions.Length];
                for (int i = 0; i < actions.Length; ++i)
                {
                    Action a = Action.Deserialize(actions[i]);
                    acto[i] = new InfoActo() { departamento = a.DepartmentID, id = a.ID };
                }
            }
        }

        public static int Insert(int shopId, string carLicense, string[] actions)
        {
            int newWork = 0;
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(Actos));
                StringWriter sw = new StringWriter();
                ser.Serialize(sw, new Actos(actions));
                string axml = sw.ToString();
                sw.Close();
                axml = sw.ToString();

                using (SqlConnection conn = Db.Utils.NewConnection)
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("RegistarObra", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter retVal = cmd.Parameters.Add("RetVal", SqlDbType.Int);
                        retVal.Direction = ParameterDirection.ReturnValue;
                        cmd.Parameters.AddWithValue("@veiculo", carLicense).Direction = ParameterDirection.Input;
                        cmd.Parameters.AddWithValue("@oficina", shopId).Direction = ParameterDirection.Input;
                        cmd.Parameters.AddWithValue("@actosDoc", axml).Direction = ParameterDirection.Input;
                        SqlParameter idObra = cmd.Parameters.Add("@idObra", SqlDbType.Int);
                        idObra.Direction = ParameterDirection.Output;
                        cmd.ExecuteNonQuery();
                        if ((int)retVal.Value == 0)
                        {
                            newWork = (int)idObra.Value;
                        }
                        else
                        {
                            string msg;
                            switch ((int)retVal.Value)
                            {
                                case -1:
                                    msg = "Identificador de veículo ou oficina inválido.";
                                    break;
                                case -2:
                                    msg = "Pelo menos uma referência de acto, departamento ou oficina é inválida.";
                                    break;
                                case -3:
                                    msg = "Não foi possível inserir um registo na base de dados.";
                                    break;
                                case -4:
                                    msg = "Não existem funcionários disponíveis para todos os actos.";
                                    break;
                                default:
                                    msg = "Impossível adicionar a obra, erro " + retVal.Value.ToString();
                                    break;
                            }
                            throw new ModelException(msg);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new ModelException("Erro na base de dados: " + ex.Message, ex.InnerException);
            }
            return newWork;
        }

        public bool AddAction(int actionId, int employeeId)
        {
            try
            {
                Action a = Action.Find(actionId);
                using (SqlConnection conn = Db.Utils.NewConnection)
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("AdicionaActoObra", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter retVal = cmd.Parameters.Add("RetVal", SqlDbType.Int);
                        retVal.Direction = ParameterDirection.ReturnValue;
                        cmd.Parameters.AddWithValue("@obra", ID).Direction = ParameterDirection.Input;
                        cmd.Parameters.AddWithValue("@acto", a.ID).Direction = ParameterDirection.Input;
                        cmd.Parameters.AddWithValue("@departamento", a.DepartmentID).Direction = ParameterDirection.Input;
                        cmd.Parameters.AddWithValue("@oficina", ShopID).Direction = ParameterDirection.Input;
                        cmd.Parameters.AddWithValue("@funcionario", employeeId).Direction = ParameterDirection.Input;
                        cmd.ExecuteNonQuery();
                        if ((int)retVal.Value == 0)
                        {
                            return true;
                        }
                        else
                        {
                            string msg;
                            switch ((int)retVal.Value)
                            {
                                case -1:
                                    msg = "Não foi possível inserir a informação na base de dados. Será que a combinação já existe?";
                                    break;
                                case -2:
                                    msg = "Pelo menos uma referência é inválida.";
                                    break;
                                case -3:
                                    msg = "O funcionário não pode estar no departamento do acto.";
                                    break;
                                default:
                                    msg = "Impossível adicionar o acto, erro " + retVal.Value.ToString();
                                    break;
                            }
                            throw new ModelException(msg);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new ModelException(
                    ex.Number == 3609 /* a validação falhou num trigger, que abortou a transacção */
                        ? "Não é possível adicionar o acto e funcionário escolhidos."
                        : "Erro na base de dados: " + ex.Message,
                    ex.InnerException);
            }
        }

        public bool Facturar(int cliente)
        {
            try
            {
                using (SqlConnection conn = Db.Utils.NewConnection)
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("FacturarObra", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter retVal = cmd.Parameters.Add("RetVal", SqlDbType.Int);
                        retVal.Direction = ParameterDirection.ReturnValue;
                        cmd.Parameters.AddWithValue("@obra", ID).Direction = ParameterDirection.Input;
                        cmd.Parameters.AddWithValue("@oficina", ShopID).Direction = ParameterDirection.Input;
                        cmd.Parameters.AddWithValue("@cliente", cliente).Direction = ParameterDirection.Input;
                        cmd.ExecuteNonQuery();
                        if ((int)retVal.Value == 0)
                        {
                            return true;
                        }
                        else
                        {
                            string msg;
                            switch ((int)retVal.Value)
                            {
                                case -2:
                                    msg = "Pelo menos uma referência é inválida.";
                                    break;
                                case -3:
                                    msg = "A obra já foi facturada.";
                                    break;
                                default:
                                    msg = "Impossível adicionar o acto, erro " + retVal.Value.ToString();
                                    break;
                            }
                            throw new ModelException(msg);
                        }
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