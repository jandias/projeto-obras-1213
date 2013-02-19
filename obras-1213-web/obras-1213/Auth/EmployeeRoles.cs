using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using obras_1213.Models.Db;
using System.Data.SqlClient;
using System.Data;

namespace obras_1213.Auth
{
    public sealed class EmployeeRoles : RoleProvider
    {
        private string applicationName;

        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);

            applicationName = GetConfigValue(config["applicationName"], System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
        }

        private string GetConfigValue(string configValue, string defaultValue)
        {
            if (String.IsNullOrEmpty(configValue))
            {
                return defaultValue;
            }
            return configValue;
        }

        public override string ApplicationName
        {
            get
            {
                return applicationName;
            }
            set
            {
                applicationName = value;
            }
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            return new string[] { "receptionist" };
        }

        public override string[] GetRolesForUser(string username)
        {
            return IsUserInRole(username, "receptionist") ? GetAllRoles() : new string[] { };
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            try
            {
                if (!roleName.Equals("receptionist"))
                {
                    return false;
                }
                using (SqlConnection conn = Utils.NewConnection())
                {
                    using (SqlCommand cmd = new SqlCommand("FuncionarioRecepcionista", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@funcionario", Int32.Parse(username));
                        SqlParameter isReceptionist = cmd.Parameters.Add("@recepcionista", SqlDbType.Int);
                        isReceptionist.Direction = ParameterDirection.Output;
                        cmd.ExecuteNonQuery();
                        return ((int)isReceptionist.Value) != 0;
                    }
                }
            }
            catch (Exception e)
            {
                throw new ProviderException("Erro ao procurar o role.", e);
            }
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            return roleName.Equals("receptionist");
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }
    }
}
