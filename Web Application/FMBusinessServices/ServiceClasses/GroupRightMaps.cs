namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;
	using System.Data.SqlClient;

	using FMBusinessObjects.Exceptions;

	/// <summary>
	/// Summary description for GroupRightMapsClass.
	/// </summary>
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class GroupRightMapsClass : IGroupRightMaps
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		public GroupRightMapsClass()
		{
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Add(SecurityClass security, GroupRightMapClass groupRightMap)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (groupRightMap == null)
			{
				throw new ArgumentNullException("groupRightMap");
			}

			if (!security.HasRight(RIGHT.MODIFY_USER_GROUPS) && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			DataSet set;

			using (SqlCommand cmd = groupRightMap.SelectSQLCmd(ContextUtil.IsInTransaction))
			{
				set = ConsolidatedDA.GetDataSet(cmd, security);
			}

			if (set.Tables[0].Rows.Count != 0)
			{
				return;
			}

			groupRightMap.ID = groupRightMap.Right.ToString();
			groupRightMap.SiteGuid = security.SiteGuid;
			groupRightMap.CreatedDate = DateTimeOffset.Now;
			groupRightMap.CreatedBy = security.UserID;

			using (SqlCommand cmd = groupRightMap.InsertSQLCmd_)
			{
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// This method determines if a group has a specific security right assigned to it.
		/// </summary>
		/// <param name="security">A SecurityClass instance</param>
		/// <param name="bInTransaction">A bool indicating if this call is wrapped in a transaction</param>
		/// <param name="groupGuid">A Guid representing the unique id of the group</param>
		/// <param name="right">The security right</param>
		/// <returns>A bool indicating whether or not this group has this right assigned</returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public bool GroupHasRight(SecurityClass security, bool bInTransaction, Guid groupGuid, RIGHT right)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (groupGuid == Guid.Empty)
			{
				throw new ArgumentNullException("groupGuid");
			}

			DataSet set;
			var groupRightMap = new GroupRightMapClass();

			using (SqlCommand cmd = groupRightMap.SelectSQLCmd(ContextUtil.IsInTransaction, groupGuid, right))
			{
				set = ConsolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			if (table.Rows == null || table.Rows.Count == 0)
			{
				return false;
			}

			DataRow row = table.Rows[0];
			return DataObject.getValue<bool>(row["HasRight"], false);
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid groupGuid, RIGHT right)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.MODIFY_USER_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			if (GroupClass.IsAdminGroupGuid(groupGuid) 
				&& (right == RIGHT.MODIFY_USER_GROUPS || right == RIGHT.VIEW_USER_GROUPS))
			{
				throw new Exception("[Cannot Purge] Administrator [" + SecurityClass.RightID(right) + "]");
			}

			var groupRightMap = new GroupRightMapClass
			                    {
				                    GroupGuid = groupGuid,
				                    Right = right,
				                    ID = SecurityClass.RightID(right)
			                    };

			var dependencies = new DependenciesClass(security);
			dependencies.Purge(security, groupRightMap);

			using (SqlCommand cmd = groupRightMap.PurgeSQLCmd)
			{
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}
	}
}
