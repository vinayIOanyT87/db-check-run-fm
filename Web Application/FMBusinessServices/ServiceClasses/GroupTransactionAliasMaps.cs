using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.Web;

using FMBusinessServices.DataAccessLayer;
using FMBusinessServices.InternalClasses;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using System.Data.SqlClient;

namespace FMBusinessServices.ServiceClasses
{
	/// <summary>
	/// Summary description for GroupTransactionAliasMapsClass.
	/// </summary>
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class GroupTransactionAliasMapsClass : IGroupTransactionAliasMaps, IDependency
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		public GroupTransactionAliasMapsClass()
		{
		}

		protected void Validate(GroupTransactionAliasMapClass GroupTransactionAliasMap)
		{
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Add(SecurityClass security, GroupTransactionAliasMapClass GroupTransactionAliasMap)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (GroupTransactionAliasMap == null)
				throw new ArgumentNullException("GroupTransactionAliasMap");

			Validate(GroupTransactionAliasMap);

			GroupTransactionAliasMap.CreatedDate = DateTimeOffset.Now;
			GroupTransactionAliasMap.CreatedBy = security.UserID;
			using (SqlCommand cmd = new SqlCommand())
			{
				GroupTransactionAliasMap.InsertSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, GroupTransactionAliasMapClass GroupTransactionAliasMap)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (GroupTransactionAliasMap == null)
				throw new ArgumentNullException("GroupTransactionAliasMap");

			Validate(GroupTransactionAliasMap);

			GroupTransactionAliasMap.UpdatedDate = DateTimeOffset.Now;
			GroupTransactionAliasMap.UpdatedBy = security.UserID;
			using (SqlCommand cmd = new SqlCommand())
			{
				GroupTransactionAliasMap.UpdateSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid groupGuid, Guid transactionAliasGuid)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			GroupTransactionAliasMapClass GroupTransactionAliasMap = new GroupTransactionAliasMapClass();
			GroupTransactionAliasMap.GroupGuid = groupGuid;
			GroupTransactionAliasMap.TransactionAliasGuid = transactionAliasGuid;
			using (SqlCommand cmd = new SqlCommand())
			{
				GroupTransactionAliasMap.PurgeSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		public GroupTransactionAliasMapCollectionClass EnumerateByTransactionAliasGuid(SecurityClass security, Guid transactionAliasGuid)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			GroupTransactionAliasMapClass GroupTransactionAliasMap = new GroupTransactionAliasMapClass();
			GroupTransactionAliasMap.TransactionAliasGuid = transactionAliasGuid;

			using (SqlCommand cmd = new SqlCommand())
			{
				GroupTransactionAliasMap.EnumerateByTransactionAliasGuidSQL(cmd);
				DataSet Set = ConsolidatedDA.GetDataSet(cmd, security);

				GroupTransactionAliasMapCollectionClass GroupTransactionAliasMapCollection = new GroupTransactionAliasMapCollectionClass();

				DataTable Table = Set.Tables[0];
				while (Table.Rows.Count != 0)
				{
					GroupTransactionAliasMap = new GroupTransactionAliasMapClass();
					GroupTransactionAliasMap.Load(Set);
					GroupTransactionAliasMapCollection.Add(GroupTransactionAliasMap);
					Table.Rows.RemoveAt(0);
				}

				return GroupTransactionAliasMapCollection;
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void ModifyCollection(
			SecurityClass security,
			GroupTransactionAliasMapCollectionClass NewGroupTransactionAliasMapCollection,
			GroupTransactionAliasMapCollectionClass ExistingGroupTransactionAliasMapCollection)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (NewGroupTransactionAliasMapCollection != null)
			{
				for (int NewItem = 0; NewItem < NewGroupTransactionAliasMapCollection.Count; NewItem++)
				{
					int ExistingItem;
					GroupTransactionAliasMapClass NewGroupTransactionAliasMap = NewGroupTransactionAliasMapCollection[NewItem];

					if (ExistingGroupTransactionAliasMapCollection != null)
					{
						for (ExistingItem = 0; ExistingItem < ExistingGroupTransactionAliasMapCollection.Count; ExistingItem++)
						{
							GroupTransactionAliasMapClass ExistingGroupTransactionAliasMap = ExistingGroupTransactionAliasMapCollection[ExistingItem];
							if (ExistingGroupTransactionAliasMap.GroupGuid == NewGroupTransactionAliasMap.GroupGuid &&
						 ExistingGroupTransactionAliasMap.TransactionAliasGuid == NewGroupTransactionAliasMap.TransactionAliasGuid &&
						 ExistingGroupTransactionAliasMap.Right != NewGroupTransactionAliasMap.Right)
							{
								Modify(security, NewGroupTransactionAliasMap);
								break;
							}
							if (ExistingGroupTransactionAliasMap.GroupGuid == NewGroupTransactionAliasMap.GroupGuid &&
								 ExistingGroupTransactionAliasMap.TransactionAliasGuid == NewGroupTransactionAliasMap.TransactionAliasGuid &&
						 ExistingGroupTransactionAliasMap.Right == NewGroupTransactionAliasMap.Right)
							{
								break;
							}
						}

						if (ExistingItem == ExistingGroupTransactionAliasMapCollection.Count)
						{
							Add(security, NewGroupTransactionAliasMap);
						}
						else
							ExistingGroupTransactionAliasMapCollection.RemoveAt(ExistingItem);
					}
					else
					{
						Add(security, NewGroupTransactionAliasMap);
					}
				}
			}

			if (ExistingGroupTransactionAliasMapCollection != null)
			{
				foreach (GroupTransactionAliasMapClass ExistingGroupTransactionAliasMap in ExistingGroupTransactionAliasMapCollection)
				{
					Purge(security, ExistingGroupTransactionAliasMap.GroupGuid, ExistingGroupTransactionAliasMap.TransactionAliasGuid);
				}
			}
		}


		/// <summary>
		/// The insert.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="Object">
		/// The object.
		/// </param>
		/// <param name="preOperation">
		/// The pre operation.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Invalid arguement.
		/// </exception>
		/// <exception cref="Exception">
		/// Access denied.
		/// </exception>
		void IDependency.Insert(SecurityClass security, BaseDataObject Object, bool preOperation)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (Object == null)
			{
				throw new ArgumentNullException("Object");
			}
		}

	
		/// <summary>
		/// The update.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="Object">
		/// The object.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Invalid argument.
		/// </exception>
		void IDependency.Update(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (Object == null)
			{
				throw new ArgumentNullException("Object");
			}
		}

		/// <summary>
		/// The purge.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="Object">
		/// The object.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Invalid argument.
		/// </exception>
		void IDependency.Purge(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (Object == null)
			{
				throw new ArgumentNullException("Object");
			}

			if (Object is GroupRightMapClass)
			{
				var groupRightMap = (GroupRightMapClass) Object;
				if (groupRightMap.Right == RIGHT.MODIFY_TRANSACTION_DATA)
				{
					using (var cmd = new SqlCommand())
					{
						var groupTransactionAliasMap = new GroupTransactionAliasMapClass();
						groupTransactionAliasMap.GroupGuid = groupRightMap.GroupGuid;
						groupTransactionAliasMap.Right = GroupTransactionAliasMapClass.RIGHT.MODIFY;
						groupTransactionAliasMap.PurgeByGroupAndRightSQL(cmd);
						this.ConsolidatedDA.ExecuteQuery(security, cmd);
					}
				}

				if(groupRightMap.Right == RIGHT.VIEW_TRANSACTION_DATA)
				{
					using (var cmd = new SqlCommand())
					{
						var groupTransactionAliasMap = new GroupTransactionAliasMapClass();
						groupTransactionAliasMap.GroupGuid = groupRightMap.GroupGuid;
						groupTransactionAliasMap.Right = GroupTransactionAliasMapClass.RIGHT.VIEW;
						groupTransactionAliasMap.PurgeByGroupAndRightSQL(cmd);
						this.ConsolidatedDA.ExecuteQuery(security, cmd);
					}
				}
			}
		}
	}
}
