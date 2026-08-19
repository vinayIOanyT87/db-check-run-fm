using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;
using FMBusinessServices.DataAccessLayer;
using FMBusinessObjects.BusinessInterfaces;

namespace FMBusinessServices.ServiceClasses
{
	public class ExistTransactionAssetProcessorClass : IExistTransactionAssetProcessor
	{
		#region Attributes
		protected static object singleton = new object();

		private ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructor
		public ExistTransactionAssetProcessorClass()
		{
			consolidatedDA = new ConsolidatedDAClass();
		}
		#endregion

		public IntegerDO Process(ExistTransactionAssetSR sr)
		{
			IntegerDO result = new IntegerDO();

			if (sr == null)
			{
				throw new Exception("Expected ExistTransactionAssetSR but found " + sr.GetType());
			}

			lock (singleton)
			{

				using (SqlCommand command = new SqlCommand())
				{
					command.CommandText = @"fm_ADF_ExistTransactionAsset";
					command.CommandType = CommandType.StoredProcedure;

					command.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
					command.Parameters.Add("@Product", SqlDbType.NVarChar, 60);
					command.Parameters.Add("@Tank", SqlDbType.NVarChar, 60);
					command.Parameters.Add("@InventoryDate", SqlDbType.Date);
					command.Parameters.Add("@AliasName", SqlDbType.NVarChar, 60);
					command.Parameters.Add("@TransactionLineItemGuid", SqlDbType.UniqueIdentifier);

					command.Parameters["@SiteGuid"].Value = sr.SiteGuid;
					command.Parameters["@Product"].Value = sr.Product;
					command.Parameters["@Tank"].Value = sr.Tank;
					command.Parameters["@InventoryDate"].Value = sr.InventoryDate.Date;
					command.Parameters["@AliasName"].Value = sr.AliasName;
					command.Parameters["@TransactionLineItemGuid"].Value = sr.TransactionLineItemGuid;

					DataSet ds = this.consolidatedDA.GetDataSet(command, sr.Security);

					if (ds != null)
					{
						if (ds.Tables.Count > 0)
						{
							DataRow dr = ds.Tables[0].Rows[0];
							result.Value = DataObject.getValue<int>(dr["Count"], 0);
						}
					}
				}
			}

			return result;
		}
	}
}