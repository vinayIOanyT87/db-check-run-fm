namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;

	using FMBusinessObjects.Exceptions;

	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class TankGroupsClass : ITankGroups, IDependency
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		public TankGroupsClass()
		{
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, TankGroupClass tankGroup)
		{

			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (tankGroup == null)
			{
				throw new ArgumentNullException("tankGroup");
			}

			if (!security.HasRight(RIGHT.MODIFY_TANK_DATA) && !security.HasRight(RIGHT.MODIFY_PRODUCTS))
			{
				throw new FMInsufficientRightsException();
			}

			if (this.GetIdentityGuid(security, tankGroup.ID) != Guid.Empty)
			{
				throw new Exception("Tank Group Exists");
			}

			tankGroup.SiteGuid = security.SiteGuid;
			tankGroup.CreatedDate = DateTimeOffset.Now;
			tankGroup.CreatedBy = security.UserID;
			tankGroup.UpdatedDate = tankGroup.CreatedDate;
			tankGroup.UpdatedBy = security.UserID;
			tankGroup.Deleted = false;
			tankGroup.IdentityGuid = Guid.NewGuid();

			using (var cmd = new SqlCommand())
			{
				tankGroup.InsertSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			var tankMaps = new TankMapsClass();
			tankMaps.ModifyCollection(security, tankGroup.IdentityGuid, tankGroup.ID, tankGroup.TankMapCollection, null);

			return tankGroup.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, TankGroupClass tankGroup)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (tankGroup == null)
			{
				throw new ArgumentNullException("tankGroup");
			}

			if (!security.HasRight(RIGHT.MODIFY_TANK_DATA) && !security.HasRight(RIGHT.MODIFY_PRODUCTS))
			{
				throw new FMInsufficientRightsException();
			}

			Guid identityGuid = GetIdentityGuid(security, tankGroup.ID);

			if (identityGuid != Guid.Empty && identityGuid != tankGroup.IdentityGuid)
			{
				throw new Exception("Tank Group Exists");
			}

			TankGroupClass oldTankGroup = Get(security, tankGroup.IdentityGuid);

			if (oldTankGroup.IdentityGuid == Guid.Empty)
			{
				throw (new Exception("Tank Group Not Found"));
			}

			var dependencies = new DependenciesClass(security);
			dependencies.Update(security, tankGroup);

			tankGroup.UpdatedDate = DateTimeOffset.Now;
			tankGroup.UpdatedBy = security.UserID;
			
			using (var cmd = new SqlCommand())
			{
				tankGroup.UpdateSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			var tankMaps = new TankMapsClass();
			tankMaps.ModifyCollection(security, tankGroup.IdentityGuid, tankGroup.ID, tankGroup.TankMapCollection, oldTankGroup.TankMapCollection);
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid identityGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.MODIFY_TANK_DATA) && !security.HasRight(RIGHT.MODIFY_PRODUCTS) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			TankGroupClass tankGroup = Get(security, identityGuid);

			if (tankGroup.IdentityGuid == Guid.Empty)
			{
				throw new Exception("Tank Group Not Found");
			}

			// Purge Dependencies
			var dependencies = new DependenciesClass(security);
			dependencies.Purge(security, tankGroup);

			var tankMaps = new TankMapsClass();
			tankMaps.ModifyCollection(security, tankGroup.IdentityGuid, tankGroup.ID, null, tankGroup.TankMapCollection);

			using (var cmd = new SqlCommand())
			{
				tankGroup.PurgeSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		public TankGroupClass Get(SecurityClass security, Guid identityGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_TANK_DATA) 
				&& !security.HasRight(RIGHT.MODIFY_TANK_DATA)
				&& !security.HasRight(RIGHT.VIEW_PRODUCTS) 
				&& !security.HasRight(RIGHT.MODIFY_PRODUCTS))
			{
				throw new FMInsufficientRightsException();
			}

			var tankGroup = new TankGroupClass { IdentityGuid = identityGuid };

			using (var cmd = new SqlCommand())
			{
				tankGroup.SelectSQL(cmd, ContextUtil.IsInTransaction);
				tankGroup.Load(this.ConsolidatedDA.GetDataSet(cmd, security));
			}

			var tankMaps = new TankMapsClass();
			tankGroup.TankMapCollection = tankMaps.EnumerateByAssignedToTankGroupGuid(security, tankGroup.IdentityGuid);

			return tankGroup;
		}

		public Guid GetIdentityGuid(SecurityClass security, string id)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_TANK_DATA) 
				&& !security.HasRight(RIGHT.MODIFY_TANK_DATA)
				&& !security.HasRight(RIGHT.VIEW_PRODUCTS) 
				&& !security.HasRight(RIGHT.MODIFY_PRODUCTS))
			{
				throw new FMInsufficientRightsException();
			}

			var tankGroup = new TankGroupClass { ID = id, SiteGuid = security.SiteGuid };

			using (var cmd = new SqlCommand())
			{
				tankGroup.SelectByIDSQL(cmd, ContextUtil.IsInTransaction);
				tankGroup.Load(this.ConsolidatedDA.GetDataSet(cmd, security));
			}
			return tankGroup.IdentityGuid;
		}

		public TankGroupCollectionClass Enumerate(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_TANK_DATA) 
				&& !security.HasRight(RIGHT.MODIFY_TANK_DATA)
				&& !security.HasRight(RIGHT.VIEW_PRODUCTS) 
				&& !security.HasRight(RIGHT.MODIFY_PRODUCTS) 
				&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			var tankGroup = new TankGroupClass { SiteGuid = security.SiteGuid };

			using (var cmd = new SqlCommand())
			{
				tankGroup.EnumerateSQL(cmd);
				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);
				var tankGroupCollection = new TankGroupCollectionClass();

				DataTable table = set.Tables[0];

				while (table.Rows.Count != 0)
				{
					tankGroup = new TankGroupClass();
					tankGroup.Load(set);
					tankGroupCollection.Add(tankGroup);
					table.Rows.RemoveAt(0);
				}

				return tankGroupCollection;
			}
		}

		public TankGroupCollectionClass EnumerateByProduct(SecurityClass security, Guid productGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_TANK_DATA) 
				&& !security.HasRight(RIGHT.MODIFY_TANK_DATA)
				&& !security.HasRight(RIGHT.VIEW_PRODUCTS) 
				&& !security.HasRight(RIGHT.MODIFY_PRODUCTS)
			    && !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA) 
				&& !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			var tankGroup = new TankGroupClass { SiteGuid = security.SiteGuid, ProductGuid = productGuid };

			using (var cmd = new SqlCommand())
			{
				tankGroup.EnumerateByProductSQL(cmd);
				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);
				var tankGroupCollection = new TankGroupCollectionClass();

				DataTable table = set.Tables[0];

				while (table.Rows.Count != 0)
				{
					tankGroup = new TankGroupClass();
					tankGroup.Load(set);
					tankGroupCollection.Add(tankGroup);
					table.Rows.RemoveAt(0);
				}

				return tankGroupCollection;
			}
		}


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

			if (preOperation && Object is EntityToSiteMapClass)
			{
				var entityToSiteMap = (EntityToSiteMapClass)Object;

				if ( entityToSiteMap.TypeID != ENTITY_TYPE.TANK_GROUP )
				{
					return;
				}

				if (Guid.Empty != GetIdentityGuid(security, entityToSiteMap.ID))
				{
					throw (new Exception("Tank Group Exists - " + entityToSiteMap.ID));
				}
			}
		}

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

			// Tank Update - check for change in Product
			if (Object is TankClass)
			{
				var tank = (TankClass)Object;
				var tankGroups = new TankGroupsClass();
				var tankMaps = new TankMapsClass();
				TankMapCollectionClass tankMapCollection = tankMaps.EnumerateByTankGuid(security, tank.IdentityGuid);

				foreach (TankMapClass tankMap in tankMapCollection)
				{
					TankGroupClass tankGroup = tankGroups.Get(security, tankMap.IdentityGuid);

					if (tankGroup.ProductGuid == tank.ProductGuid)
					{
						break;
					}

					tankMaps.Purge(security, tankMap.IdentityGuid, tankMap.TankGuid);
				}
			}
		}

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

			if (Object is SiteClass)
			{
				TankGroupCollectionClass tankGroupCollection = Enumerate(security);

				foreach (TankGroupClass tankGroup in tankGroupCollection)
				{
					Purge(security, tankGroup.IdentityGuid);
				}
				
				return;
			}

			if (Object is TankClass)
			{
				var tank = (TankClass)Object;
				var tankMaps = new TankMapsClass();
				TankMapCollectionClass tankMapCollection = tankMaps.EnumerateByTankGuid(security, tank.IdentityGuid);

				foreach (TankMapClass tankMap in tankMapCollection)
				{
					tankMaps.Purge(security, tankMap.IdentityGuid, tankMap.TankGuid);
				}

				return;
			}

			if (Object is EntityToSiteMapClass)
			{
				var entityToSiteMap = (EntityToSiteMapClass)Object;

				if ( entityToSiteMap.TypeID == ENTITY_TYPE.PRODUCT )
				{
					TankGroupCollectionClass tankGroupCollection = EnumerateByProduct(security, entityToSiteMap.IdentityGuid);

					foreach (TankGroupClass tankGroup in tankGroupCollection)
					{
						tankGroup.Load(Get(security, tankGroup.IdentityGuid));
						tankGroup.ProductGuid = Guid.Empty;
						tankGroup.ProductID = "{None}";
						Modify(security, tankGroup);
					}
				}
			}
		}
	}
}
