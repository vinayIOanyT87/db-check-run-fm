/// <summary>
/// File name:	PhysicalInventoryListProcessor.cs
/// Purpose:	To decipher the request to retrieve the transaction
///				data object.
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	Richard R. Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:			By:						Reason:
///		----------		--------------------	----------------------------------
///		
/// </summary>

namespace FMBusinessServices.ServiceClasses
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.ServiceRequests;

    using FMBusinessServices.DataAccessLayer;

    public class PhysicalInventoryListProcessorClass : IPhysicalInventoryListProcessor
	{
		#region Attributes
		private ConsolidatedDAClass consolidatedDA;
		#endregion Attributes

		#region Constructor
		/// <summary>
		/// This is the default constructor for the physical inventory list processor class.
		/// It must initialize the accounting service implemention class in the base class
		/// and set the request command with the service request string.
		/// </summary>
		public PhysicalInventoryListProcessorClass()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
		}
		#endregion

		#region Public Methods
		public PhysicalInventoryListDO Process(PhysicalInventoryListSR sr)
		{
			PhysicalInventoryListDO inventoryDO = new PhysicalInventoryListDO();

			TransactionAliasClass transAlias = new TransactionAliasClass();

			DataSet dataSet = null;
			using (SqlCommand cmd = new SqlCommand())
			{
				const string PARAM_NAME_TRANSTYPEID = "@TransTypeID";
				const SqlDbType PARAM_TYPE_TRANSTYPEID = SqlDbType.SmallInt;

				// The following prepares for the subquery for the Aliasname
				cmd.CommandText = "SELECT tblTransactionAliases.AliasName FROM tblTransactionAliases WHERE " +
										transAlias.AppendSiteWhereClause(cmd, sr.Security, "tblTransactionAliases", "TransactionAliasGuid");

				cmd.Parameters.Add( "@TargetSiteGuid", SqlDbType.UniqueIdentifier );
				cmd.Parameters["@TargetSiteGuid"].Value = sr.Security.SiteGuid;

				cmd.CommandText += DataObject.AddParameter(cmd, true, "tblTransactionAliases.LookupTransTypeIndex", PARAM_NAME_TRANSTYPEID, PARAM_TYPE_TRANSTYPEID, (short)TransactionTypes.T14_PhysicalInventory);
				// cmd now should have the subqueries and parameters for it.

				inventoryDO.GetPhysicalInventorySelectSQL(sr.Site, sr.Manager, sr.Product, sr.FirstDate,
																		sr.LastDate, sr.Security.LoginSiteGuid,
																		sr.Security.SiteGuid, sr.Security.UserGuid, cmd);

				dataSet = this.consolidatedDA.GetDataSet(cmd, sr.Security);
			}

			if (dataSet != null && dataSet.Tables.Count > 0)
			{
				DataTable table = dataSet.Tables[0];

				if (table.Rows != null)
				{
					foreach (DataRow row in table.Rows)
					{
						PhysicalInventoryLineItemDO lineItem = new PhysicalInventoryLineItemDO();
						lineItem.InventoryDate = DataObject.getValue<DateTime>(row["InventoryDate"], DateTime.Today);
						lineItem.GrossQuantity = DataObject.getDouble(row["GrossQuantity"]);
						lineItem.NetQuantity = DataObject.getDouble(row["NetQuantity"]);
						lineItem.MassQuantity = DataObject.getDouble(row["MassQuantity"]);

						inventoryDO.LineItems.Add(lineItem);
					}
				}
			}

			return inventoryDO;
		}
		#endregion
	}
}