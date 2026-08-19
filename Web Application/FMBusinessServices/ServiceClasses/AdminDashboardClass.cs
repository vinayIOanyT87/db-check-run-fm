using System;

namespace FMBusinessServices.ServiceClasses
{
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;

	using FMBusinessObjects.BusinessInterfaces;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	using FMBusinessServices.DataAccessLayer;

	public class AdminDashboardClass : IAdminDashboard
	{
		private readonly ConsolidatedDAClass consolidatedDa;

		readonly Guid siteAdminGuid = new Guid("00000000-0000-0000-0000-000000000001");

		public AdminDashboardClass()
		{
			this.consolidatedDa = new ConsolidatedDAClass();
		}


		public DataSet GetNodeHealthSummary(SecurityClass security, string nodeHealth, string orderBy, string siteID, string nodeName)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.ACCESS_ADMIN_DASHBOARD))
			{
				throw new FMInsufficientRightsException();
			}

			DataSet dataSet;

			using (var cmd = new SqlCommand())
			{
				this.GetNodeHealthSummary(cmd, security,  nodeHealth, orderBy, siteID,  nodeName);
				dataSet = this.consolidatedDa.GetDataSet(cmd, security);
			}

			return dataSet;
		}

		private void GetNodeHealthSummary(SqlCommand cmd, SecurityClass security, string nodeHealth, string orderBy, string siteID, string nodeName)
		{
			if (string.IsNullOrWhiteSpace(nodeHealth))
				nodeHealth = string.Empty;

			if (string.IsNullOrWhiteSpace(orderBy))
				orderBy = string.Empty;

			if (string.IsNullOrWhiteSpace(siteID))
				siteID = string.Empty;

			if (string.IsNullOrWhiteSpace(nodeName))
				nodeName = string.Empty;

			cmd.CommandText = "dbo.usp_GetNodeHealth";
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = security.SiteGuid;
			cmd.Parameters.Add("@NodeHealth", SqlDbType.NVarChar).Value = nodeHealth;
			cmd.Parameters.Add("@OrderBy", SqlDbType.NVarChar).Value = orderBy;
			cmd.Parameters.Add("@SiteID", SqlDbType.NVarChar).Value = siteID;
			cmd.Parameters.Add("@NodeName", SqlDbType.NVarChar).Value = nodeName;
		}

	}
}