using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Data;
using System.Data.SqlClient;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ServiceRequests;
using FMBusinessServices.DataAccessLayer;

namespace FMBusinessServices.ServiceClasses
{
	public class AllOwnerCloseoutsProcessorClass : IAllOwnerCloseoutsProcessor
	{
		#region Private data members
		private ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		public AllOwnerCloseoutsProcessorClass()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
		}
		#endregion

		#region Public methods
		public AllOwnerCloseoutsDO Process(AccountingServiceRequest accountingSR)
		{
			AllOwnerCloseoutsDO allCloseoutDO = new AllOwnerCloseoutsDO();

			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				allCloseoutDO.GetSelectCommand(cmd);
				dataSet = this.consolidatedDA.GetDataSet(cmd, accountingSR.Security);
			}

			if ((dataSet != null) && (dataSet.Tables != null) && (dataSet.Tables.Count > 0))
			{
				DataTable dataTable = dataSet.Tables[0];

				if (dataTable.Rows != null)
				{
					foreach (DataRow row in dataTable.Rows)
					{
						OwnerCloseoutDO closeout = new OwnerCloseoutDO();

						closeout.SiteName = (row.IsNull("Site") == true) ? "" : (string)row["Site"];
						closeout.SiteGuid = (row.IsNull("SiteGuid") == true) ? Guid.Empty : (Guid)row["SiteGuid"];
						closeout.ManagerName = (row.IsNull("ManagerName") == true) ? "" : (string)row["ManagerName"];
						closeout.ManagerGuid = (row.IsNull("ManagerCompanyGuid") == true) ? Guid.Empty : (Guid)row["ManagerCompanyGuid"];
						closeout.ProductName = (row.IsNull("ProductName") == true) ? "" : (string)row["ProductName"];
						closeout.ProductGuid = (row.IsNull("ProductGuid") == true) ? Guid.Empty : (Guid)row["ProductGuid"];

						if (row.IsNull("CloseoutDate") == false)
						{
							closeout.CloseoutDate = (DateTimeOffset)row["CloseoutDate"];
						}

						allCloseoutDO.CloseoutList.Add(closeout);
					}
				}
			}

			return allCloseoutDO;
		}
		#endregion
	}
}