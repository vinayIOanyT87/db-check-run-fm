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
	public class ImportProcessorClass : IImportProcessor
	{
		#region Private data members
		private ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		public ImportProcessorClass ( )
		{
			this.consolidatedDA = new ConsolidatedDAClass ( );
		}
		#endregion

		public ImportExportListDO Process ( ImportSR sr )
		{
			if (sr.ImportName == null)
			{
				return this.GetImportTypeList ( sr );
			}

			return null;
		}

		#region Protected Methods
		protected ImportExportListDO GetImportTypeList ( ImportSR sr )
		{
			ImportExportListDO importListDO = new ImportExportListDO ( );
			importListDO.Site = sr.Site.ToString ( );

			DataSet dataSet = null;
			//Return a list of the configured external systems plus the standard XML import type.
			using (SqlCommand cmd = new SqlCommand())
			{
				importListDO.GetSelectCommand(cmd);
				dataSet = this.consolidatedDA.GetDataSet(cmd, sr.Security);
			}

			if (( dataSet != null ) && ( dataSet.Tables != null ) && ( dataSet.Tables.Count > 0 ))
			{
				DataTable table = dataSet.Tables[0];

				if (table.Rows != null)
				{
					foreach (DataRow row in table.Rows)
					{
						ImportExportListItemDO item = new ImportExportListItemDO ( );
						item.ImportAllowed = ( row.IsNull ( "Import" ) ) ? false : (bool) row["Import"];

						if (item.ImportAllowed == true)
						{
							item.DisplayName = ( row.IsNull ( "ImportExportName" ) ) ? null : (string) row["ImportExportName"];
							item.PluginType = (row.IsNull("PluginType")) ? null : (string)row["PluginType"];
							item.ImportExportConfigGuid = (row.IsNull("ImportExportConfigGuid") == true) ? Guid.Empty : (Guid)row["ImportExportConfigGuid"];

							importListDO.ImportExportList.Add ( item );
						}
					}
				}
			}

			return importListDO;
		}
		#endregion Protected Methods
	}
}