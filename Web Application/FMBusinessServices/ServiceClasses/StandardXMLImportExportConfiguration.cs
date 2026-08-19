using System;
using System.Data;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Web;

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.DataObjects;

using FMBusinessServices.DataAccessLayer;

namespace FMBusinessServices.ServiceClasses
{
	[ServiceBehavior]
	public class StandardXMLImportExportConfiguration : IStandardXMLImportExportConfiguration
	{
		public void SaveConfiguration( SecurityClass security, ImportFilter filter )
		{
			DeleteConfiguration( security, filter );
			InsertConfiguration( security, filter );
		}

		protected void DeleteConfiguration( SecurityClass security, ImportFilter filter )
		{
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandText = "DELETE FROM tblStandardImportConfig WHERE name = @Name";
				cmd.Parameters.AddWithValue("@Name", filter.Name);
				ConsolidatedDAClass da = new ConsolidatedDAClass();
				da.ExecuteQuery(security, cmd);
			}
		}

		protected void InsertConfiguration( SecurityClass security, ImportFilter filter )
		{
			string sql = "INSERT INTO tblStandardImportConfig (Name, KeyName, KeyValue) VALUES (@name, @keyname, @keyvalue)";

			HardwareKeyClass hardwareKey = new HardwareKeyClass();

			SqlConnectionStringBuilder connectionBuilder = new SqlConnectionStringBuilder( ConsolidatedDAClass.ConnectionString );
			if (hardwareKey.IsDescKey() && (security.UserID != DBAccess.ServiceLoginAccess))
			{
				connectionBuilder.IntegratedSecurity = false;
				connectionBuilder.UserID = security.UserID + "|" + security.SiteID;
				connectionBuilder.Password = DBAccess.GetDBPasswordAccess( security.Password );
			}
			SqlConnection conn = new SqlConnection( connectionBuilder.ConnectionString );
			conn.Open();
			System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand( sql, conn );

			cmd.Parameters.Add( "@name", System.Data.SqlDbType.NVarChar, 50 );
			cmd.Parameters.Add( "@keyname", System.Data.SqlDbType.NVarChar, 50 );
			cmd.Parameters.Add( "@keyvalue", System.Data.SqlDbType.NVarChar, 50 );
			cmd.Prepare();

			InsertList( cmd, filter.Name, "Alias", filter.AliasList );
			InsertList( cmd, filter.Name, "Carrier", filter.CarrierList );
			InsertList( cmd, filter.Name, "Consumer", filter.ConsumerList );
			InsertList( cmd, filter.Name, "Manager", filter.ManagerList );
			InsertList( cmd, filter.Name, "Owner", filter.OwnerList );
			InsertList( cmd, filter.Name, "Product", filter.ProductList );
			InsertList( cmd, filter.Name, "Supplier", filter.SupplierList );

			cmd.Parameters["@name"].Value = filter.Name;
			cmd.Parameters["@keyname"].Value = "IncludeDeletedTransactions";
			cmd.Parameters["@keyvalue"].Value = filter.IncludeDeletedTransactions.ToString();

			cmd.ExecuteNonQuery();

			conn.Close();

		}

		protected void InsertList( System.Data.SqlClient.SqlCommand cmd, string name, string key, StringCollection list )
		{
			if (list == null)
			{
				return;
			}
			foreach (string keyValue in list)
			{
				cmd.Parameters["@name"].Value = name;
				cmd.Parameters["@keyname"].Value = key;
				cmd.Parameters["@keyvalue"].Value = keyValue;

				cmd.ExecuteNonQuery();
			}
		}

		public ImportFilter GetConfiguration( SecurityClass security, string name )
		{
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandText = "SELECT KeyName, KeyValue " + 
					"FROM tblStandardImportConfig " + 
					"WHERE name = @nameParam " +
					"ORDER BY KeyName";

				cmd.Parameters.Add(new SqlParameter("@nameParam", System.Data.SqlDbType.NVarChar, 50, name));

				ConsolidatedDAClass da = new ConsolidatedDAClass();
				DataSet ds = da.GetDataSet(cmd, security);

				ImportFilter filter = new ImportFilter();
				filter.Name = name;
				foreach (DataRow row in ds.Tables[0].Rows)
				{
					string keyName = row["KeyName"] as string;
					string keyValue = row["KeyValue"] as string;

					switch (keyName)
					{
						case "Alias":
							if (filter.AliasList == null)
							{
								filter.AliasList = new System.Collections.Specialized.StringCollection();
							}
							filter.AliasList.Add(keyValue);
							break;
						case "Carrier":
							if (filter.CarrierList == null)
							{
								filter.CarrierList = new System.Collections.Specialized.StringCollection();
							}
							filter.CarrierList.Add(keyValue);
							break;
						case "Consumer":
							if (filter.ConsumerList == null)
							{
								filter.ConsumerList = new System.Collections.Specialized.StringCollection();
							}
							filter.ConsumerList.Add(keyValue);
							break;
						case "Manager":
							if (filter.ManagerList == null)
							{
								filter.ManagerList = new System.Collections.Specialized.StringCollection();
							}
							filter.ManagerList.Add(keyValue);
							break;
						case "Owner":
							if (filter.OwnerList == null)
							{
								filter.OwnerList = new System.Collections.Specialized.StringCollection();
							}
							filter.OwnerList.Add(keyValue);
							break;
						case "Product":
							if (filter.ProductList == null)
							{
								filter.ProductList = new System.Collections.Specialized.StringCollection();
							}
							filter.ProductList.Add(keyValue);
							break;
						case "Supplier":
							if (filter.SupplierList == null)
							{
								filter.SupplierList = new System.Collections.Specialized.StringCollection();
							}
							filter.SupplierList.Add(keyValue);
							break;
						case "IncludeDeletedTransactions":
							filter.IncludeDeletedTransactions = bool.Parse(keyValue);
							break;
						default:
							throw new NotSupportedException("Unknown Criterion \"" + keyName + "\" for Standard XML Import/Export");
					}
				}

				return filter;
			}
		}
	}
}
