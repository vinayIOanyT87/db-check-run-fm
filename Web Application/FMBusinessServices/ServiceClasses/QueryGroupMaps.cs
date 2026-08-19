using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.Web;
using System.Xml.Serialization;
using System.Data.SqlClient;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessServices.DataAccessLayer;
using FMBusinessServices.InternalClasses;
using FMBusinessServices.ServiceClasses;

namespace FMBusinessServices.ServiceClasses
{
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class QueryGroupMapsClass : IQueryGroupMaps
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		public QueryGroupMapsClass()
		{
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Add(SecurityClass security, QueryGroupMapClass QueryGroupMap)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (QueryGroupMap == null)
				throw new ArgumentNullException("QueryGroupMap");

			DataSet Set;
			using (SqlCommand cmd = new SqlCommand())
			{
				QueryGroupMap.SelectSQL(cmd, ContextUtil.IsInTransaction);
				Set = ConsolidatedDA.GetDataSet(cmd, security);
			}

			if (Set.Tables[0].Rows.Count != 0)
				return;

			QueryGroupMap.SiteGuid = security.SiteGuid;
			QueryGroupMap.CreatedDate = DateTimeOffset.Now;
			QueryGroupMap.CreatedBy = security.UserID;
			QueryGroupMap.UpdatedDate = QueryGroupMap.CreatedDate;
			QueryGroupMap.UpdatedBy = security.UserID;
			QueryGroupMap.Deleted = false;

			using (SqlCommand cmd = new SqlCommand())
			{
				QueryGroupMap.IdentityGuid = Guid.NewGuid();
				QueryGroupMap.InsertSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public GroupCollectionClass EnumerateAssignedGroups(SecurityClass security, Guid queryStorageGuid)
		{
			QueryGroupMapClass queryGroupMap = new QueryGroupMapClass();
			GroupCollectionClass coll = new GroupCollectionClass();

			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			queryGroupMap.IdentityGuid = queryStorageGuid;

			DataSet set;
			using (var cmd = new SqlCommand())
			{
				queryGroupMap.EnumerateGroupsSQL(cmd, ContextUtil.IsInTransaction);
				set = ConsolidatedDA.GetDataSet(cmd, security);
			}

			foreach (DataRow row in set.Tables[0].Rows)
			{
				GroupClass g = new GroupClass();
				g.ID = DataObject.getValue<string>(row["GroupID"], string.Empty);
				g.IdentityGuid = DataObject.getValue<Guid>(row["GroupGuid"], Guid.Empty);

				coll.Add(g);
			}

			return coll;


		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid queryGuid, Guid groupGuid)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			QueryGroupMapClass QueryGroupMap = new QueryGroupMapClass();
			QueryGroupMap.QueryStorageGuid = queryGuid;
			QueryGroupMap.GroupGuid = groupGuid;

			using (SqlCommand cmd = new SqlCommand())
			{
				QueryGroupMap.PurgeSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}
	}
}
