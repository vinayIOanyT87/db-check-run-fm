namespace FMBusinessServices.DataAccessLayer
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using FMBusinessObjects.DataObjects;
	internal static class ExStarsProductInventoryDAO
	{
		internal static void 
			GetProductInventorySql(this ExStarsProductInventoryClass productInventory, SqlCommand cmd, DateTime inventoryDate, Guid siteGuid, Guid managerCompanyGuid)
		{
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "dbo.[gsp_ExStarsEndingInventorySelect]";
			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
			cmd.Parameters.AddWithValue("@ManagerCompanyGuid", managerCompanyGuid);
			cmd.Parameters.AddWithValue("@InventoryDate", inventoryDate);
			cmd.CommandTimeout = 120;			
		}

	}
}