using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Microsoft.Win32;

namespace FMBackupUtility
{
    class DBAdminConnect
    {
        
        public static string ConnectionString
        {
           get
           {
			   string connectionString;

			   connectionString = ConfigurationManager.AppSettings["ConnectionString"];

			   if (string.IsNullOrEmpty(connectionString))
			   {
				   throw new ArgumentNullException("Connection string not configured in configuration file.");
			   }

			   return connectionString;
  
           }
        }
       
        public static string getConnectionString(string db)
        {
 
            SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder(ConnectionString);
            connectionString.Add("Initial Catalog", db);
            return connectionString.ToString();
        }
 
    }
}
