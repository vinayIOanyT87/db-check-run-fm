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
	public class ImportExportPluginProcessorClass : IImportExportPluginProcessor
	{
		#region Private data members
		private ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		public ImportExportPluginProcessorClass ( )
		{
			this.consolidatedDA = new ConsolidatedDAClass ( );
		}
		#endregion

		public ImportExportPluginDO Process ( ImportExportPluginSR sr )
		{
			ImportExportPluginDO pluginDO = new ImportExportPluginDO ( );

			DataSet dataSet = null;
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandText = pluginDO.getSelectCommand ( );
				dataSet = this.consolidatedDA.GetDataSet( cmd, sr.Security);
			}

			if (( dataSet != null ) && ( dataSet.Tables != null ) && ( dataSet.Tables.Count > 0 ))
			{
				DataTable table = dataSet.Tables[0];

				if (table.Rows != null)
				{
					foreach (DataRow row in table.Rows)
					{
						ImportExportPluginItemDO item	= new ImportExportPluginItemDO ( );
						item.PluginType					= DataObject.getValue<string>(row["PluginType"], "");
						item.ConfigURL						= DataObject.getValue<string>(row["ConfigURL"], "");
						item.RunURL							= DataObject.getValue<string>(row["RunURL"], "");
						item.Import							= DataObject.getValue<bool>(row["Import"], false);
						item.Export							= DataObject.getValue<bool>(row["Export"], false);

						pluginDO.PluginList.Add ( item );
					}
				}
			}

			return pluginDO;
		}
	}
}