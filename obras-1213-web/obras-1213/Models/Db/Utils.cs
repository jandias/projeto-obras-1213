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
        public static SqlConnection NewConnection
        {
            get 
            { 
                return new SqlConnection(ConfigurationManager.ConnectionStrings["ObrasDb"].ConnectionString); 
            }
        }

        public static DataTable GetData(SqlCommand cmd)
        {
            DataTable dt = new DataTable();
            using (cmd)
            {
                cmd.CommandType = CommandType.Text;
                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    sda.SelectCommand = cmd;
                    sda.Fill(dt);
                }
            }
            return dt;
        }

        public static SqlDataReader GetReader(string command)
        {
            using (SqlConnection conn = NewConnection)
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(command, conn))
                {
                    return cmd.ExecuteReader();
                }
            }
        }
    }
}
