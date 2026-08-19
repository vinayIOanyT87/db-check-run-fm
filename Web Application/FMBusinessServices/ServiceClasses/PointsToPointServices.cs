// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PointsToPointServices.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Constants;

	using DataAccessLayer;

	/// <summary>
	/// 
	/// </summary>
	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]

	public class PointsToPointServices : IPointsToPointServices
	{
		/// <summary>
		/// Purges the by point unique identifier.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="pointGuid">The point unique identifier.</param>
		/// <exception cref="System.ArgumentNullException">
		/// Security
		/// or
		/// PointGuid
		/// </exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void PurgeByPointGuid(SecurityClass security, Guid pointGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (pointGuid == null)
			{
				throw new ArgumentNullException("PointGuid");
			}

			var consolidatedDA = new ConsolidatedDAClass();

			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = "DELETE FROM map.tblPointToPointService WHERE PointGuid = @PointGuid";
				cmd.Parameters.AddWithValue("@PointGuid", pointGuid);
				consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// Enumerates the host name by point tag unique identifier.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="tagGuidList">The tag unique identifier list.</param>
		/// <returns></returns>
		public Dictionary<string, List<Guid>> EnumerateHostNameByPointTagGuid(SecurityClass security, List<Guid> tagGuidList)
		{
			var hostNameToPointTagGuidListDictionary = new Dictionary<string, List<Guid>>();

			var consolidatedDA = new ConsolidatedDAClass();

			using (SqlCommand cmd = new SqlCommand())
			{
				DataSet set;

				// Execute the stored procedure, passing in the list (table) of pointTags
				cmd.CommandType = CommandType.Text;

				cmd.CommandText = "SELECT DISTINCT ps.Hostname,pt.PointTagGuid FROM @PointTagTable ptt "
										+ " LEFT JOIN dbo.tblPointTag pt ON pt.PointTagGuid = ptt.Guid"
										+ " LEFT JOIN map.tblPointToPointService ptps ON ptps.PointGuid = pt.PointGuid"
										+ " LEFT JOIN dbo.tblPointService ps ON ps.PointServiceGuid = ptps.PointServiceGuid"
										+ " ORDER BY ps.Hostname";

				var pointTagTable = new DataTable();
				pointTagTable.Columns.Add("Guid", typeof(Guid));
				foreach (var tagGuid in tagGuidList)
				{
					var row = pointTagTable.NewRow();
					row[0] = tagGuid;

					pointTagTable.Rows.Add(row);
				}

				SqlParameter tableValuedParameter = cmd.Parameters.Add("@PointTagTable", SqlDbType.Structured);
				tableValuedParameter.Value = pointTagTable;
				tableValuedParameter.TypeName = "dbo.GuidListType";

				set = consolidatedDA.GetDataSet(cmd, security);

				if (set.Tables.Count == 1)
				{
					foreach (DataRow row in set.Tables[0].Rows)
					{
						List<Guid> pointTagGuidList = null;
						var hostName = row.IsNull("Hostname") ? "" : row["Hostname"] as string;
						if(!hostNameToPointTagGuidListDictionary.TryGetValue(hostName, out pointTagGuidList))
						{
							pointTagGuidList = new List<Guid>();
                     hostNameToPointTagGuidListDictionary.Add(hostName, pointTagGuidList);
						}

						if (pointTagGuidList != null)
						{
						    if (!row.IsNull("PointTagGuid"))
						    {
						        pointTagGuidList.Add((Guid)row["PointTagGuid"]);
						    }
						}
					}
				}
			}

			return hostNameToPointTagGuidListDictionary;
		}

		/// <summary>
		/// Enumerates the host name by point value identifier.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="tagGuidList">The tag unique identifier list.</param>
		/// <returns></returns>
		public Dictionary<string, List<PointValueIdentifier>> EnumerateHostNameByPointValueIdentifier(SecurityClass security, List<PointValueIdentifier> pointValueIdentifiers)
		{
			var pointValueGuidToHostNameDictionary = new Dictionary<Guid, string>();
			var hostNameToPointValueIdentifierDictionary = new Dictionary<string, List<PointValueIdentifier>>();

			var consolidatedDA = new ConsolidatedDAClass();

			using (SqlCommand cmd = new SqlCommand())
			{
				DataSet set;

				// Execute the query passing in the list (table) of pointValueGuids
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.CommandText = "usp_EnumerateHostByPointValueIdentifiers";
				var pointTagTable = new DataTable();
				pointTagTable.Columns.Add("Guid", typeof(Guid));
				foreach (var pointValueIdentifier in pointValueIdentifiers)
				{
					if (pointValueIdentifier.IdentityGuid == SystemDataPointVirtualPoint.TagSiteDataGuid
					|| pointValueIdentifier.IdentityGuid == SystemDataPointVirtualPoint.TagUserDataGuid
					|| pointValueIdentifier.IdentityGuid == SystemDataPointVirtualPoint.TagDateTimeDataGuid
					|| pointValueIdentifier.IdentityGuid == SystemDataPointVirtualPoint.TaglicenseExpiryDataGuid)
					{
						continue;
					}


					var row = pointTagTable.NewRow();
					row[0] = pointValueIdentifier.IdentityGuid;

					pointTagTable.Rows.Add(row);
				}

				SqlParameter tableValuedParameter = cmd.Parameters.Add("@PointValueIdentityTable", SqlDbType.Structured);
				tableValuedParameter.Value = pointTagTable;
				tableValuedParameter.TypeName = "dbo.GuidListType";

				set = consolidatedDA.GetDataSet(cmd, security);

				if (set.Tables.Count == 1)
				{
					foreach (DataRow row in set.Tables[0].Rows)
					{
						var hostName = row.IsNull("Hostname") ? "" : row["Hostname"] as string;

						pointValueGuidToHostNameDictionary.Add((Guid)row["Guid"], hostName);
					}
				}

				foreach(var pointValueIdentifier in pointValueIdentifiers)
				{
					string hostName;
					if(pointValueGuidToHostNameDictionary.TryGetValue(pointValueIdentifier.IdentityGuid, out hostName))
					{ 
						List<PointValueIdentifier> pointValueIdentifierList = null;
						if (!hostNameToPointValueIdentifierDictionary.TryGetValue(hostName, out pointValueIdentifierList))
						{
							pointValueIdentifierList = new List<PointValueIdentifier>(pointValueIdentifiers.Count);
							hostNameToPointValueIdentifierDictionary.Add(hostName, pointValueIdentifierList);
						}

						pointValueIdentifierList.Add(pointValueIdentifier);
					}
				}
			}

			return hostNameToPointValueIdentifierDictionary;
		}


		/// <summary>
		/// Enumerates the host name by point unique identifier.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="pointGuidList">The point unique identifier list.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentNullException">Security</exception>
		public Dictionary<string, List<Guid>> EnumerateHostNameByPointGuid(SecurityClass security, List<Guid> pointGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			var hostNameToPointGuidListDictionary = new Dictionary<string, List<Guid>>();

			var consolidatedDA = new ConsolidatedDAClass();

			using (SqlCommand cmd = new SqlCommand())
			{
				DataSet set;

				// Execute the stored procedure, passing in the list (table) of pointTags
				cmd.CommandType = CommandType.Text;

				cmd.CommandText = "SELECT DISTINCT ps.Hostname,p.PointGuid FROM dbo.tblPoint p "
										+ " INNER JOIN @PointTable pt ON pt.Guid = p.PointGuid"
										+ " INNER JOIN map.tblPointToPointService ptps ON ptps.PointGuid = p.PointGuid"
										+ " LEFT JOIN dbo.tblPointService ps ON ps.PointServiceGuid = ptps.PointServiceGuid"
										+ " ORDER BY ps.Hostname";

				var pointTable = new DataTable();
				pointTable.Columns.Add("Guid", typeof(Guid));
				foreach (var tagGuid in pointGuidList)
				{
					var row = pointTable.NewRow();
					row[0] = tagGuid;

					pointTable.Rows.Add(row);
				}

				SqlParameter tableValuedParameter = cmd.Parameters.Add("@PointTable", SqlDbType.Structured);
				tableValuedParameter.Value = pointTable;
				tableValuedParameter.TypeName = "dbo.GuidListType";

				set = consolidatedDA.GetDataSet(cmd, security);

				if (set.Tables.Count == 1)
				{
					foreach (DataRow row in set.Tables[0].Rows)
					{
						pointGuidList = null;
						var hostName = row["Hostname"] as string;
						if (!string.IsNullOrEmpty(hostName)
						&& !hostNameToPointGuidListDictionary.TryGetValue(hostName, out pointGuidList))
						{
							pointGuidList = new List<Guid>();
							hostNameToPointGuidListDictionary.Add(hostName, pointGuidList);
						}

						if (pointGuidList != null)
						{
							pointGuidList.Add((Guid)row["PointGuid"]);
						}
					}
				}
			}

			return hostNameToPointGuidListDictionary;
		}


	}
}