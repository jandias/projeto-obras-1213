using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace obras_1213.Models.Db
{
    public class Utils
    {
        public static SqlConnection NewConnection(string isolationLevel)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ObrasDb"].ConnectionString);
            conn.Open();
            // Assegurar um nível de isolamento por omissão para todas as instruções.
            using (SqlCommand cmd = new SqlCommand("SET TRANSACTION ISOLATION LEVEL " + isolationLevel, conn))
            {
                cmd.ExecuteNonQuery();
            }
            return conn;
        }

        public static SqlConnection NewConnection()
        {
            return NewConnection("READ COMMITTED");
        }
    }
}
