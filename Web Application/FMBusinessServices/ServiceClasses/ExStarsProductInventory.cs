namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessServices.DataAccessLayer;

	public class ExStarsProductInventory  : ExStarsSqlBase
	{
		public ExStarsProductInventoryListClass ProductInventoryList { get; protected set; }
		public ExStarsInventoryStatus HasInventory
		{
			get
			{
				return (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
					? ExStarsInventoryStatus.hasInventory
					: ExStarsInventoryStatus.noActivity;
			}
		}


		#region Constants and Fields

		private readonly DataSet dataSet = null;

		#endregion

		public ExStarsProductInventory(ExStarsSiteConfigExpanded config, DateTime inventoryDate)
			: base(config)
		{
			ExStarsProductInventoryClass productInventory = new ExStarsProductInventoryClass();
			this.ProductInventoryList = new ExStarsProductInventoryListClass();

			using (var cmd = new SqlCommand())
			{
				try
				{ 
					productInventory.GetProductInventorySql(cmd, inventoryDate, config.SiteGuid,  config.ManagerCompanyGuid);
					dataSet = this.ConsolidatedDa.GetDataSet(cmd, config.Security);

					if (dataSet.Tables.Count == 0 ||  dataSet.Tables[0].Rows.Count == 0)
					{
						throw new ExStarsProductInventoryException( "Cannot find Inventory for site {0} on {1}", config.Site.SiteID, inventoryDate.ToString("MMMM dd, yyyy"));
					}
					DataTable table = dataSet.Tables[0];
					table.TableName = "ProductInventory";
					LoadResults(table, this.ProductInventoryList);
				}
				catch (ExStarsProductInventoryException)
				{
					// pass the exception up
					throw;
				}
				catch (Exception e)
				{
					throw new ExStarsSqlException(e, "SQL error: {0}", cmd.CommandText);
				}
			}
		}

		private static void LoadResults(DataTable table, ExStarsProductInventoryListClass productInventoryList)
		{
			foreach (DataRow row in table.Rows)
			{

				ExStarsProductInventoryClass productInventory = new ExStarsProductInventoryClass();
				string product = DataObject.getValue(row["Product"], string.Empty);
				productInventory.SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty);
				productInventory.ManagerCompanyGuid = DataObject.getValue(row["ManagerCompanyGuid"], Guid.Empty);
				productInventory.ProductGuid = DataObject.getValue(row["ProductGuid"], Guid.Empty);
				productInventory.TaxCode = DataObject.getValue(row["TaxCode"], "");
				productInventory.AviationFuelFlag = DataObject.getValue(row["AviationFuelFlag"], false);
				productInventory.PriorInventoryExists = DataObject.getValue(row["PriorInventoryExists"], false);
				productInventory.GrossVolume = DataObject.getValue(row["TotalGrossVolume"], 0.0);
				productInventory.NetVolume = DataObject.getValue(row["TotalNetVolume"], 0.0);
				productInventory.ReportYear = DataObject.getValue(row["ReportYear"], 0);
				productInventory.ReportMonth = DataObject.getValue(row["ReportMonth"], 0);
				productInventory.ReportDay = DataObject.getValue(row["ReportDay"], 0);
				productInventory.Count = DataObject.getValue(row["Count"], 0);
				productInventoryList.Add(productInventory);
			}
		}

#if true
		public void SetBeginningInventoryRecorded(ExStarsProductInventoryClass productInventory)
		{
			productInventory.PriorInventoryExists = true;
			using (var cmd = new SqlCommand("[DBO].[gsp_ExStarsProductPriorInventoryInsert]"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@ManagerCompanyGuid", this.Config.ManagerCompanyGuid);
				cmd.Parameters.AddWithValue("@SiteGuid", this.Config.Site.SiteGuid);
				cmd.Parameters.AddWithValue("@TaxCode", productInventory.TaxCode);
				cmd.Parameters.AddWithValue("@PriorInventoryExists", productInventory.PriorInventoryExists);
				cmd.Parameters.AddWithValue("@UpdatedBy", this.Config.Security.UserID);

				this.ExecuteNonQuery( cmd);
			}
		}
#else
		public void SetBeginningInventoryRecorded(ExStarsProductInventoryClass productInventory)
		{
			using (var cmd = new SqlCommand())
			{
				string sql = string.Format(
					"UPDATE [dbo].[tblExStarsProductPriorInventory] " +
					"SET [PriorInventoryExists] = 1 , [UpdatedDate]=GETDATE(), [UpdatedBy]='{3}' " +
					"WHERE [SiteGuid] = '{0}' AND [ManagerCompanyGuid] = '{1}' AND [TaxCode] = '{2}'"
					, productInventory.SiteGuid
					, productInventory.ManagerCompanyGuid
					, productInventory.TaxCode
					, this.Config.Security.UserID);
				this.ExecuteNonQuery(sql);
			}			
		}
#endif
	}
}