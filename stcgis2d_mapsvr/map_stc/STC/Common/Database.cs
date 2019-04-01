using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace map_stc.STC.Common
{
    public class Database
    {
        private static readonly string SqlConnectionString = ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString;

        private static readonly string OleConnectionString = ConfigurationManager.ConnectionStrings["OleConnectionString"].ConnectionString;

        public static IDbConnection DbService()
        {
            if(!String.IsNullOrWhiteSpace(SqlConnectionString))
            {
                var connection = new System.Data.SqlClient.SqlConnection();
                connection.ConnectionString = SqlConnectionString;
                connection.Open();
                return connection;
            }else
            {
                var connection = new System.Data.OleDb.OleDbConnection();
                connection.ConnectionString = OleConnectionString;
                connection.Open();
                return connection;
            }            
        }

        
        
    }
}