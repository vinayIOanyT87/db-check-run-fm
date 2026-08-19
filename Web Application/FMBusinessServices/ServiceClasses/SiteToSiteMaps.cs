using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.Web;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessServices.DataAccessLayer;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	using FMBusinessObjects.Exceptions;



	/// <summary>
	/// Summary description for SiteToSiteMapsClass.
	/// </summary>
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class SiteToSiteMapsClass : ISiteToSiteMaps
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		public SiteToSiteMapsClass()
		{
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Add(SecurityClass security, SiteToSiteMapClass SiteToSiteMap)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (SiteToSiteMap == null)
				throw new ArgumentNullException("SiteToSiteMap");

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
				throw new FMInsufficientRightsException();

			using (SqlCommand cmd = new SqlCommand())
			{
				SiteToSiteMap.SelectSQL(cmd, ContextUtil.IsInTransaction);
				DataSet Set = ConsolidatedDA.GetDataSet(cmd, security);
				if (Set.Tables[0].Rows.Count != 0)
					return;
			}

			SiteToSiteMap.CreatedDate = DateTimeOffset.Now;
			SiteToSiteMap.CreatedBy = security.UserID;
			using (SqlCommand cmd = new SqlCommand())
			{
				SiteToSiteMap.InsertSQL(cmd);
				try
				{
					ConsolidatedDA.ExecuteQuery(security, cmd);
				}
				catch (Exception e)
				{
					if (e.Message == ConsolidatedDAClass.StatementTerminatedMaximumRecursionErrorMessage)
					{
						var dataDictionaries = new DataDictionariesClass();
						string errorString = (security.UseDataDictionary) ? dataDictionaries.Get(security.SiteGuid, "Site Assignment Would Create Recursive Site Hierarchy") : "Site Assignment Would Create Recursive Site Hierarchy";
						throw new ConsolidatedDAException(String.Format(errorString, SiteToSiteMap.ChildSiteID, SiteToSiteMap.ParentSiteID));
					}

					throw e;
				}
			}
		}


		private bool IsRootNode(DataRowCollection rows, Guid parentSiteGuid)
		{
			foreach (DataRow row in rows)
			{
				if (parentSiteGuid != (Guid)row["ParentSiteGuid"]
				    && parentSiteGuid == (Guid)row["ChildSiteGuid"])
				{
					return false;
				}
			}

			return true;
		}

		public Dictionary<Guid, object> GetSiteHierarchy(SecurityClass security, bool ignoreEnterprise = false)
		{

			var configurationSettings = new ConfigurationSettingsClass();
			string isEnterprise = configurationSettings.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_IsEnterprise);

			var siteHierarchy = new Dictionary<Guid, object>();
			var rootNodes = new Dictionary<Guid, object>();

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "SELECT STS.ParentSiteGuid, STS.ChildSiteGuid, S.SiteGroupFlag FROM map.tblSiteToSite STS"
														  + " LEFT JOIN dbo.tblSites S ON S.SiteGuid = STS.ChildSiteGuid"
										  + " WHERE STS.ChildSiteGuid IN (SELECT SiteGuid FROM dbo.tblSitesAncillaryData)"
														  + " AND STS.ParentSiteGuid IN (SELECT SiteGuid FROM dbo.tblSitesAncillaryData)"
										  + " AND S.Enabled = CAST(1 AS BIT)";
				if (!ignoreEnterprise)
				{
					cmd.CommandText += " AND S.Enterprise = CAST(" + isEnterprise + " AS BIT)";
				}

				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

				DataTable table = set.Tables[0];
				foreach (DataRow row in table.Rows)
				{
					var parentSiteGuid = (Guid)row["ParentSiteGuid"];
					var childSiteGuid = (Guid)row["ChildSiteGuid"];
					var siteGroupFlag = (bool)row["SiteGroupFlag"];

					object parentGroupNode = null;
					if (!rootNodes.TryGetValue(parentSiteGuid, out parentGroupNode))
					{
						if (parentSiteGuid != childSiteGuid
						|| siteGroupFlag)
						{
							parentGroupNode = new Dictionary<Guid, object>();
						}

						rootNodes.Add(parentSiteGuid, parentGroupNode);
					}

					// Each site is mapped to itself
					if (parentSiteGuid != childSiteGuid)
					{
						if (siteGroupFlag)
						{
							object childGroupNode;
							if (!rootNodes.TryGetValue(childSiteGuid, out childGroupNode))
							{
								childGroupNode = new Dictionary<Guid, object>();
								rootNodes.Add(childSiteGuid, childGroupNode);
							}

							((Dictionary<Guid, object>)parentGroupNode).Add(childSiteGuid, childGroupNode);
						}
						else
						{
							((Dictionary<Guid, object>)parentGroupNode).Add(childSiteGuid, null);
						}
					}
				}

				foreach (var nodeGuid in rootNodes.Keys)
				{
					if (IsRootNode(table.Rows, nodeGuid))
					{
						siteHierarchy.Add(nodeGuid, rootNodes[nodeGuid]);
					}
				}
			}

			return siteHierarchy;
		}


		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid ParentSiteGuid, Guid ChildSiteGuid)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
				throw new FMInsufficientRightsException();

			SiteToSiteMapClass SiteToSiteMap = new SiteToSiteMapClass();
			SiteToSiteMap.ParentSiteGuid = ParentSiteGuid;
			SiteToSiteMap.ChildSiteGuid = ChildSiteGuid;
			using (SqlCommand cmd = new SqlCommand())
			{
				SiteToSiteMap.PurgeSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		public SiteToSiteMapCollectionClass EnumerateByParentSite(SecurityClass security, Guid SiteGuid)
		{
			SiteToSiteMapCollectionClass siteToSiteMapCollection = new SiteToSiteMapCollectionClass();
			SiteToSiteMapClass siteToSiteMap = new SiteToSiteMapClass();
			siteToSiteMap.ParentSiteGuid = SiteGuid;
			using (SqlCommand cmd = new SqlCommand())
			{
				siteToSiteMap.EnumerateByParentSiteSQL(cmd);
				DataSet Set = ConsolidatedDA.GetDataSet(cmd, security);
				SiteCollectionClass SiteCollection = new SiteCollectionClass();

				DataTable Table = Set.Tables[0];
				while (Table.Rows.Count != 0)
				{
					siteToSiteMap = new SiteToSiteMapClass();
					siteToSiteMap.LoadObject(Set);

					// Every Site is mapped to itself and should be skipped here
					if (SiteGuid != siteToSiteMap.ChildSiteGuid)
						siteToSiteMapCollection.Add(siteToSiteMap);

					Table.Rows.RemoveAt(0);
				}

				return siteToSiteMapCollection;
			}
		}

		public SiteToSiteMapCollectionClass EnumerateByChildSite(SecurityClass security, Guid SiteGuid)
		{
			SiteToSiteMapCollectionClass siteToSiteMapCollection = new SiteToSiteMapCollectionClass();
			SiteToSiteMapClass siteToSiteMap = new SiteToSiteMapClass();
			siteToSiteMap.ChildSiteGuid = SiteGuid;
			using (SqlCommand cmd = new SqlCommand())
			{
				siteToSiteMap.EnumerateByChildSiteSQL(cmd);
				DataSet Set = ConsolidatedDA.GetDataSet(cmd, security);
				SiteCollectionClass SiteCollection = new SiteCollectionClass();

				DataTable Table = Set.Tables[0];
				while (Table.Rows.Count != 0)
				{
					siteToSiteMap = new SiteToSiteMapClass();
					siteToSiteMap.LoadObject(Set);

					// Every Site is mapped to itself and should be skipped here
					if (SiteGuid != siteToSiteMap.ChildSiteGuid)
						siteToSiteMapCollection.Add(siteToSiteMap);

					Table.Rows.RemoveAt(0);
				}

				return siteToSiteMapCollection;
			}
		}

		public Int64? GetMaxSiteToSiteMapRowVersion(SecurityClass security)
		{
			var consolidatedDA = new ConsolidatedDAClass();
			DataSet set = null;

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "SELECT MAX(RowVersion) AS RowVersion FROM"
				                  + " (SELECT MAX(UpdatedRowVersion) AS RowVersion FROM track.tblSiteToSite"
				                  + " UNION SELECT MAX(InsertedRowVersion) AS RowVersion FROM track.tblSiteToSite"
				                  + " UNION SELECT MAX(DeletedRowVersion) AS RowVersion FROM track.tblSiteToSite "
										+ " UNION SELECT MAX( _RowVersion ) AS RowVersion FROM map.tblSiteToSite) RowVersions";
				set = consolidatedDA.GetDataSet(cmd, security);
			}

			if (set == null || set.Tables.Count == 0 || set.Tables[0].Rows.Count == 0)
			{
				return null;
			}

			DataTable table = set.Tables[0];
			DataRow row = table.Rows[0];

			if (row.IsNull("RowVersion"))
			{
				return null;
			}

			return BaseDataObject.RowVersionToInt64(row["RowVersion"] as byte[]);
		}


	}
}