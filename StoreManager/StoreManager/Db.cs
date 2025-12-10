using System.Data.SqlClient;
using System.Configuration;

namespace StoreManager
{
    public static class DB
    {
        private static string conn = ConfigurationManager.ConnectionStrings["db"].ConnectionString;


        public static SqlConnection GetCon()
        {
            return new SqlConnection(conn);
        }
    }
}
