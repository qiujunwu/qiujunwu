using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ESRI.AGS.DatabaseSupport
{
    public class Utility
    {
        public static void appendSQL(ref string where, string append)
        {
            where += (where == "") ? " where " + append : " and " + append;
        }
    }

    public class Database
    {
        private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        public static IDbConnection DbService()
        {
            var connection = new SqlConnection();
            connection.ConnectionString = ConnectionString;
            connection.Open();
            return connection;
        }
    }
}