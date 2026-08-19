/// <summary>
/// File name:	ImportExportConfigProcessor.cs
/// Purpose:	To decipher the request to retrieve the import/export configuration
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
using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Data.SqlClient;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ServiceRequests;
using FMBusinessServices.DataAccessLayer;
using FMBusinessObjects.Exceptions;

namespace FMBusinessServices.ServiceClasses
{
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class ImportExportConfigProcessorClass : IImportExportConfigProcessor
	{
		#region Attributes
		private ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the import/export configuration processor class.
		/// It must initialize the accounting service implemention class in the base class
		/// and set the request command with the service request string.
		/// </summary>
		public ImportExportConfigProcessorClass ( )
		{
			this.consolidatedDA = new ConsolidatedDAClass ( );
		}
		#endregion

		#region Public Methods
		[OperationBehavior( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public ImportExportListDO Process( ImportExportConfigSR importExportListSR )
		{
			// If no list is provided, it is a query.
			if (importExportListSR.ImportExportList != null)
			{
				//The list was provided, so save the list to the DB.
				this.UpdateConfiguration ( importExportListSR );
			}

			return this.GetConfiguration ( importExportListSR );
		}
		#endregion

		#region Private Methods
		private ImportExportListDO GetConfiguration ( ImportExportConfigSR sr )
		{
			ImportExportListDO listDO = new ImportExportListDO ( );
			listDO.Site = sr.Site;
			using (SqlCommand cmd = new SqlCommand())
			{
				listDO.GetSelectCommand(cmd);

				DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, sr.Security);

				if ((dataSet != null) && (dataSet.Tables != null) && (dataSet.Tables.Count > 0))
				{
					DataTable table = dataSet.Tables[0];

					if (table.Rows != null)
					{
						foreach (DataRow row in table.Rows)
						{
							ImportExportListItemDO listItem = new ImportExportListItemDO();

							listItem.Site = (row.IsNull("Site") == true) ? null : (string)row["Site"];
							listItem.DisplayName = (row.IsNull("ImportExportName") == true) ? null : (string)row["ImportExportName"];
							listItem.ExportAllowed = (row.IsNull("Export") == true) ? false : (bool)row["Export"];
							listItem.ImportAllowed = (row.IsNull("Import") == true) ? false : (bool)row["Import"];
							listItem.LastExported = (row.IsNull("LastExported") == true) ? null : (string)row["LastExported"];
							listItem.PluginType = (row.IsNull("PluginType") == true) ? null : (string)row["PluginType"];
							listItem.ImportExportConfigGuid = (row.IsNull("ImportExportConfigGuid") == true) ? Guid.Empty : (Guid)row["ImportExportConfigGuid"];

							listDO.ImportExportList.Add(listItem);
						}
					}
				}

				return listDO;
			}
		}

		private void SaveConfiguration ( ImportExportConfigSR sr )
		{
			using (SqlCommand cmd = new SqlCommand())
			{
				sr.ImportExportList.InsertSQL(cmd);
				ImportExportListDO importExportListDO = sr.ImportExportList;

				foreach (ImportExportListItemDO item in importExportListDO.ImportExportList)
				{
					cmd.Parameters["@site"].Value = item.Site;
					cmd.Parameters["@importExportName"].Value = item.DisplayName;
					cmd.Parameters["@pluginType"].Value = item.PluginType;
					cmd.Parameters["@configName"].Value = item.DisplayName;
					cmd.Parameters["@lastExported"].Value = item.LastExported;

					this.consolidatedDA.ExecuteQuery(sr.Security, cmd);
				}
			}
		}

		private void DeleteConfiguration ( ImportExportConfigSR sr )
		{
			using (SqlCommand cmd = new SqlCommand())
			{
				sr.ImportExportList.GetDeleteCommand(cmd);
				cmd.Parameters["@Site"].Value = sr.Site;
				this.consolidatedDA.ExecuteQuery(sr.Security, cmd);
			}
		}

		private void UpdateConfiguration ( ImportExportConfigSR sr )
		{
			this.DeleteConfiguration ( sr );
			this.SaveConfiguration ( sr );
		}
		#endregion
	}
}