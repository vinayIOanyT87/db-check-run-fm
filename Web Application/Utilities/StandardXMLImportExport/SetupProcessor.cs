using System;

using FM7Accounting;
using XMLImport;

namespace StandardXMLImportExport
{
	/// <summary>
	/// Summary description for SetupProcessor.
	/// </summary>
	public class SetupProcessor
	{
		#region Attributes
		protected AccountingDA accountingDA;
		#endregion

		public SetupProcessor()
		{
			accountingDA = new AccountingDA();
		}

		#region Public Methods
		public ImportFilter GetConfiguration(string name)
		{
			string sql = "SELECT KeyName, KeyValue " + 
				"FROM tblStandardImportConfig " + 
				"WHERE name = '" + name + "' " +
				"ORDER BY KeyName";
			System.Data.DataSet ds = accountingDA.GetDataSet(sql);
			
			ImportFilter filter = new ImportFilter();
			filter.Name = name;
			foreach(System.Data.DataRow row in ds.Tables[0].Rows)
			{
				string keyName = (string) row["KeyName"];
				string keyValue = (string) row["KeyValue"];

				switch(keyName)
				{
					case "Alias":
						if(filter.AliasList == null)
						{
							filter.AliasList = new System.Collections.Specialized.StringCollection();
						}
						filter.AliasList.Add(keyValue);
						break;
					case "Carrier":
						if(filter.CarrierList == null)
						{
							filter.CarrierList = new System.Collections.Specialized.StringCollection();
						}
						filter.CarrierList.Add(keyValue);
						break;
					case "Consumer":
						if(filter.ConsumerList == null)
						{
							filter.ConsumerList = new System.Collections.Specialized.StringCollection();
						}
						filter.ConsumerList.Add(keyValue);
						break;
					case "Manager":
						if(filter.ManagerList == null)
						{
							filter.ManagerList = new System.Collections.Specialized.StringCollection();
						}
						filter.ManagerList.Add(keyValue);
						break;
					case "Owner":
						if(filter.OwnerList == null)
						{
							filter.OwnerList = new System.Collections.Specialized.StringCollection();
						}
						filter.OwnerList.Add(keyValue);
						break;
					case "Product":
						if(filter.ProductList == null)
						{
							filter.ProductList = new System.Collections.Specialized.StringCollection();
						}
						filter.ProductList.Add(keyValue);
						break;
					case "Supplier":
						if(filter.SupplierList == null)
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

		public void SaveConfiguration(ImportFilter filter)
		{
			DeleteConfiguration(filter);
			InsertConfiguration(filter);
		}

		public void DeleteConfiguration(ImportFilter filter)
		{
			string sql = "DELETE FROM tblStandardImportConfig WHERE name = '" + filter.Name + "'";
			accountingDA.ExecuteQuery(sql);
		}

		#endregion Public Methods

		#region Non-public Methods
		protected void InsertConfiguration(ImportFilter filter)
		{
			string sql = "INSERT INTO tblStandardImportConfig (Name, KeyName, KeyValue) VALUES (@name, @keyname, @keyvalue)";

			System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(accountingDA.ConnectionString);
			conn.Open();
			System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn);

			cmd.Parameters.Add("@name", System.Data.SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@keyname", System.Data.SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@keyvalue", System.Data.SqlDbType.NVarChar, 50);
			cmd.Prepare();

			InsertList(cmd, filter.Name, "Alias", filter.AliasList);
			InsertList(cmd, filter.Name, "Carrier", filter.CarrierList);
			InsertList(cmd, filter.Name, "Consumer", filter.ConsumerList);
			InsertList(cmd, filter.Name, "Manager", filter.ManagerList);
			InsertList(cmd, filter.Name, "Owner", filter.OwnerList);
			InsertList(cmd, filter.Name, "Product", filter.ProductList);
			InsertList(cmd, filter.Name, "Supplier", filter.SupplierList);

			cmd.Parameters["@name"].Value = filter.Name;
			cmd.Parameters["@keyname"].Value = "IncludeDeletedTransactions";
			cmd.Parameters["@keyvalue"].Value = filter.IncludeDeletedTransactions.ToString();

			cmd.ExecuteNonQuery();

			conn.Close();
		}

		protected void InsertList(System.Data.SqlClient.SqlCommand cmd, string name, string key, 
			System.Collections.Specialized.StringCollection list)
		{
			if(list == null)
			{
				return;
			}
			foreach(string keyValue in list)
			{
				cmd.Parameters["@name"].Value = name;
				cmd.Parameters["@keyname"].Value = key;
				cmd.Parameters["@keyvalue"].Value = keyValue;

				cmd.ExecuteNonQuery();
			}
		}

		#endregion Non-public Methods
	}
}
