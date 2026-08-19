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
	public class AliasAssignmentProcessorClass : IAliasAssignmentProcessor
	{
		#region Private data members
		private ConsolidatedDAClass consolidatedDA;
		#endregion

		public AliasAssignmentProcessorClass()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
		}

		public AliasAssignmentListDO Process(AccountingServiceRequest accountingSR)
		{
			AliasAssignmentListDO aliasAssignmentListDO = new AliasAssignmentListDO();
			aliasAssignmentListDO.OwnerSite = accountingSR.Security.SiteID;

			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				aliasAssignmentListDO.GetSelectCommand(cmd);

				dataSet = this.consolidatedDA.GetDataSet(cmd, accountingSR.Security);
			}

			foreach (DataRow row in dataSet.Tables[0].Rows)
			{
				AliasAssignmentDO assignment = new AliasAssignmentDO();
				assignment.TransactionAliasGuid = (row.IsNull("TransactionAliasGuid") == true) ? Guid.Empty : (Guid)row["TransactionAliasGuid"];
				assignment.AliasName = (row.IsNull("AliasName") == true) ? "" : (string)row["AliasName"];
				assignment.AssignedSite = (row.IsNull("SiteOwner") == true) ? "" : (string)row["SiteOwner"];
				assignment.AliasCustomName = (row.IsNull("AliasCustomName") == true) ? "" : (string)row["AliasCustomName"];

				aliasAssignmentListDO.AliasAssignmentList.Add(assignment);
			}

			return aliasAssignmentListDO;
		}
	}
}