namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.ServiceModel;
	using System.Data;
	using System.Data.SqlClient;
	using System.Security;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.BusinessInterfaces;

	using FMBusinessServices.DataAccessLayer;

	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class GroupLedgerViewMapsClass : IGroupLedgerViewMaps
	{
		#region Private data members
		private readonly ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		public GroupLedgerViewMapsClass()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
		}
		#endregion

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Add(SecurityClass security, GroupLedgerViewMapClass groupLedgerViewMap)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (groupLedgerViewMap == null)
			{
				throw new ArgumentNullException("groupLedgerViewMap");
			}

			groupLedgerViewMap.CreatedDate = DateTimeOffset.Now;
			groupLedgerViewMap.CreatedBy = security.UserID;

			using (var cmd = new SqlCommand())
			{
				groupLedgerViewMap.IdentityGuid = Guid.NewGuid();
				groupLedgerViewMap.InsertSQL(cmd);
				
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid groupGuid, Guid listViewGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var groupLedgerViewMap = new GroupLedgerViewMapClass { GroupGuid = groupGuid, ListViewGuid = listViewGuid };

			using (var cmd = new SqlCommand())
			{
				groupLedgerViewMap.PurgeSQL(cmd);
				consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		public GroupLedgerViewMapCollectionClass EnumerateByListViewGuid(SecurityClass security, Guid listViewGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var groupLedgerViewMap = new GroupLedgerViewMapClass { ListViewGuid = listViewGuid };
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				groupLedgerViewMap.EnumerateByListViewGuid(cmd);
				set = consolidatedDA.GetDataSet(cmd, security);
			}

			var groupLedgerViewMapCollection = new GroupLedgerViewMapCollectionClass();
			DataTable table = set.Tables[0];

			while (table.Rows.Count != 0)
			{
				groupLedgerViewMap = new GroupLedgerViewMapClass();
				groupLedgerViewMap.Load(set);
				groupLedgerViewMapCollection.Add(groupLedgerViewMap);
				table.Rows.RemoveAt(0);
			}

			return groupLedgerViewMapCollection;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void ModifyCollection(SecurityClass security,
												Guid listViewGuid,
												GroupLedgerViewMapCollectionClass newGroupLedgerViewMapCollection,
												GroupLedgerViewMapCollectionClass existingGroupLedgerViewMapCollection)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (newGroupLedgerViewMapCollection != null)
			{
				foreach (GroupLedgerViewMapClass groupLedgerViewMap in newGroupLedgerViewMapCollection)
				{
					GroupLedgerViewMapClass newGroupLedgerViewMap = groupLedgerViewMap;

					newGroupLedgerViewMap.ListViewGuid = listViewGuid;

					if (existingGroupLedgerViewMapCollection != null)
					{
						int existingItem;

						for (existingItem = 0; existingItem < existingGroupLedgerViewMapCollection.Count; existingItem++)
						{
							GroupLedgerViewMapClass existingGroupLedgerViewMap = existingGroupLedgerViewMapCollection[existingItem];

							if (existingGroupLedgerViewMap.GroupGuid == newGroupLedgerViewMap.GroupGuid &&
							    existingGroupLedgerViewMap.ListViewGuid == newGroupLedgerViewMap.ListViewGuid)
							{
								break;
							}
						}

						if (existingItem == existingGroupLedgerViewMapCollection.Count)
						{
							this.Add(security, newGroupLedgerViewMap);
						}
						else
						{
							existingGroupLedgerViewMapCollection.RemoveAt(existingItem);
						}
					}
					else
					{
						this.Add(security, newGroupLedgerViewMap);
					}
				}
			}

			if (existingGroupLedgerViewMapCollection != null)
			{
				foreach (GroupLedgerViewMapClass existingGroupLedgerViewMap in existingGroupLedgerViewMapCollection)
				{
					this.Purge(security, existingGroupLedgerViewMap.GroupGuid, existingGroupLedgerViewMap.ListViewGuid);
				}
			}
		}
	}
}