using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace obras_1213.Models
{
    public class Work
    {
        public int ID { get; set; }
        public int ShopID { get; set; }
        public DateTime IssuanceDate { get; set; }
        public string State { get; set; }
        public decimal PredictedValue { get; set; }
        public float PredictedTime { get; set; }
        public string CarLicense { get; set; }

        public Work()
        {
        }

        public Work(int id, int shop, DateTime date, string state, decimal value, float time, string car)
        {
            ID = id;
            ShopID = shop;
            IssuanceDate = date;
            State = state;
            PredictedValue = value;
            PredictedTime = time;
            CarLicense = car;
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
                        " join Acto a on oc.acto=a.idA and oc.departamento=a.idA and oc.oficina=a.oficina " +
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
                                yield return new WorkPart(dr.GetString(0), dr.GetString(1), dr.GetDecimal(2), dr.GetInt32(3));
                            }
                        }
                    }
                }
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
    }
}