namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections;
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

	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class TankMapsClass : ITankMaps
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		public TankMapsClass()
		{
		}

		protected void Validate(TankMapClass tankMap)
		{
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Add(SecurityClass security, TankMapClass tankMap)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (tankMap == null)
			{
				throw new ArgumentNullException("tankMap");
			}

			Validate(tankMap);

			tankMap.CreatedDate = DateTimeOffset.Now;
			tankMap.CreatedBy = security.UserID;
			tankMap.UpdatedDate = tankMap.CreatedDate;
			tankMap.UpdatedBy = security.UserID;
			
			using (var cmd = new SqlCommand())
			{
				tankMap.InsertSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid assignedToTankGroupGuid, Guid tankGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var tankMap = new TankMapClass { IdentityGuid = assignedToTankGroupGuid, TankGuid = tankGuid };

			using (var cmd = new SqlCommand())
			{
				tankMap.PurgeSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		public TankMapCollectionClass EnumerateByAssignedToTankGroupGuid(SecurityClass security, Guid assignedToTankGroupGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var tankMap = new TankMapClass { IdentityGuid = assignedToTankGroupGuid };

			using (var cmd = new SqlCommand())
			{
				tankMap.EnumerateByAssignedToTankGroupGuidSQL(cmd, ContextUtil.IsInTransaction);
				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);
				var tankMapCollection = new TankMapCollectionClass();

				DataTable table = set.Tables[0];

				while (table.Rows.Count != 0)
				{
					tankMap = new TankMapClass();
					tankMap.Load(set);
					tankMapCollection.Add(tankMap);
					table.Rows.RemoveAt(0);
				}

				return tankMapCollection;
			}
		}

		public TankMapCollectionClass EnumerateByTankGuid(SecurityClass security, Guid tankGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var tankMap = new TankMapClass { TankGuid = tankGuid };

			using (var cmd = new SqlCommand())
			{
				tankMap.EnumerateByTankGuidSQL(cmd);
				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);
				var tankMapCollection = new TankMapCollectionClass();

				DataTable table = set.Tables[0];

				while (table.Rows.Count != 0)
				{
					tankMap = new TankMapClass();
					tankMap.Load(set);
					tankMapCollection.Add(tankMap);
					table.Rows.RemoveAt(0);
				}

				return tankMapCollection;
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void ModifyCollection(
									SecurityClass security,
									Guid identityGuid,
									string id,
									TankMapCollectionClass newTankMapCollection,
									TankMapCollectionClass existingTankMapCollection)
		{

			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (newTankMapCollection != null)
			{
				for (int newItem = 0; newItem < newTankMapCollection.Count; newItem++)
				{
					TankMapClass newTankMap = newTankMapCollection.ElementAt(newItem);

					newTankMap.IdentityGuid = identityGuid;
					newTankMap.ID = id;

					if (existingTankMapCollection != null)
					{
						int existingItem;
						for (existingItem = 0; existingItem < existingTankMapCollection.Count; existingItem++)
						{
							TankMapClass existingTankMap = existingTankMapCollection.ElementAt(existingItem);

							if (existingTankMap.TankGuid == newTankMap.TankGuid)
							{
								break;
							}
						}

						if (existingItem == existingTankMapCollection.Count)
						{
							Add(security, newTankMap);
						}
						else
						{
							existingTankMapCollection.RemoveAt(existingItem);
						}
					}
					else
					{
						Add(security, newTankMap);
					}
				}
			}

			if (existingTankMapCollection != null)
			{
				foreach (TankMapClass existingTankMap in existingTankMapCollection)
				{
					Purge(security, existingTankMap.IdentityGuid, existingTankMap.TankGuid);

				}
			}
		}
	}

}
