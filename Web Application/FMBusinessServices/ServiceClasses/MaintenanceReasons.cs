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
	using FMBusinessObjects.Exceptions;
	using FMBusinessServices.InternalClasses;

	/// <summary>
	/// Summary description for MaintenanceReasonsClass.
	/// </summary>
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class MaintenanceReasonsClass : IDependency, IMaintenanceReasons
	{
		private readonly ConsolidatedDAClass consolidatedDA;

		public MaintenanceReasonsClass()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
		}

		protected void Validate(SecurityClass security, MaintenanceReasonClass maintenanceReason)
		{
			if (string.IsNullOrEmpty(maintenanceReason.ID))
			{
				throw new Exception("ID Required");
			}

			if (maintenanceReason.ID == "{None}"
				|| maintenanceReason.ID == "{Unassigned}"
				|| maintenanceReason.ID == "{All}")
			{
				throw new Exception("ID is reserved key word " + maintenanceReason.ID);
			}

			if (maintenanceReason.Description == "")
			{
				throw new Exception("Description Required");
			}

			Guid identityGuid = GetIdentityGuid(security, maintenanceReason.ID);

			if (identityGuid != Guid.Empty && identityGuid != maintenanceReason.IdentityGuid)
			{
				throw (new Exception("Maintenance Reason Exists"));
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, MaintenanceReasonClass maintenanceReason)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (maintenanceReason == null)
			{
				throw new ArgumentNullException("maintenanceReason");
			}

			if (!security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD) &&
				!security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD) &&
				!security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD) &&
				!security.HasRight(RIGHT.ADD_QUALITYTAG_RECORD))
			{
				throw new FMInsufficientRightsException();
			}

			this.Validate(security, maintenanceReason);

			maintenanceReason.SiteGuid = security.SiteGuid;
			maintenanceReason.CreatedDate = DateTimeOffset.Now;
			maintenanceReason.CreatedBy = security.UserID;
			maintenanceReason.UpdatedDate = maintenanceReason.CreatedDate;
			maintenanceReason.UpdatedBy = security.UserID;
			maintenanceReason.IdentityGuid = Guid.NewGuid();

			using (var cmd = new SqlCommand())
			{
				maintenanceReason.InsertSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}

			// Create Entity to Site Map
			var entityToSiteMaps = new EntityToSiteMaps();
			var entityToSiteMap = new EntityToSiteMapClass(maintenanceReason);
			entityToSiteMaps.Add(security, entityToSiteMap, maintenanceReason.IdentityGuid);

			return maintenanceReason.IdentityGuid;
		}

		#region DML
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, MaintenanceReasonClass maintenanceReason)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (maintenanceReason == null)
			{
				throw new ArgumentNullException("maintenanceReason");
			}

			if (!security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD) && !security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD))
			{
				throw new FMInsufficientRightsException();
			}

			this.Validate(security, maintenanceReason);

			MaintenanceReasonClass oldMaintenanceReason = maintenanceReason;

			if (oldMaintenanceReason.IdentityGuid == Guid.Empty)
			{
				throw (new Exception("Maintenance Reason Not Found"));
			}

			maintenanceReason.UpdatedDate = DateTimeOffset.Now;
			maintenanceReason.UpdatedBy = security.UserID;

			using (var cmd = new SqlCommand())
			{
				maintenanceReason.UpdateSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}

			if (maintenanceReason.SiteGuid != oldMaintenanceReason.SiteGuid)
			{
				// Purge from EntityToSiteMap
				var entityToSiteMaps = new EntityToSiteMaps();
				EntityToSiteMapCollectionClass entityToSiteMapCollection = 
					entityToSiteMaps.EnumerateByTypeIDAndGuid(security, ENTITY_TYPE.MAINTENANCE_REASON, maintenanceReason.IdentityGuid);
					
				foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
				{
					entityToSiteMap.ID = maintenanceReason.ID;
					entityToSiteMaps.Purge(security, entityToSiteMap);
				}

				// Create Entity to Site Map
				var newEntityToSiteMap = new EntityToSiteMapClass(maintenanceReason);
				entityToSiteMaps.Add(security, newEntityToSiteMap, maintenanceReason.IdentityGuid);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid maintenanceReasonGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD))
			{
				throw new FMInsufficientRightsException();
			}

			MaintenanceReasonClass maintenanceReason = Get(security, maintenanceReasonGuid);

			if (maintenanceReason.IdentityGuid == Guid.Empty)
			{
				throw (new Exception("Maintenance Reason Not Found"));
			}

			// Purge from EntityToSiteMap
			var entityToSiteMaps = new EntityToSiteMaps();
			EntityToSiteMapCollectionClass entityToSiteMapCollection = 
						entityToSiteMaps.EnumerateByTypeIDAndGuid(security, ENTITY_TYPE.MAINTENANCE_REASON, maintenanceReason.IdentityGuid);
			
			foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
			{
				entityToSiteMap.ID = maintenanceReason.ID;
				entityToSiteMaps.Purge(security, entityToSiteMap);
			}

			using (var cmd = new SqlCommand())
			{
				maintenanceReason.PurgeSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		void IDependency.Insert(SecurityClass security, BaseDataObject insertObject, bool preOperation)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (insertObject == null)
			{
				throw new ArgumentNullException("insertObject");
			}

			if (preOperation && insertObject is EntityToSiteMapClass)
			{
				var entityToSiteMap = (EntityToSiteMapClass)insertObject;

				if ( entityToSiteMap.TypeID != ENTITY_TYPE.MAINTENANCE_REASON )
				{
					return;
				}

				if (Guid.Empty != GetIdentityGuid(security, entityToSiteMap.ID))
				{
					throw (new Exception("Maintenance Reason Exists"));
				}
			}
		}

		void IDependency.Update(SecurityClass security, BaseDataObject updateObject)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (updateObject == null)
			{
				throw new ArgumentNullException("updateObject");
			}
		}

		void IDependency.Purge(SecurityClass security, BaseDataObject purgeObject)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (purgeObject == null)
			{
				throw new ArgumentNullException("purgeObject");
			}
		}
		#endregion

		#region Gets and Enumerates
		public MaintenanceReasonClass Get(SecurityClass security, Guid maintenanceReasonGuid)
		{
			const bool GetSchedulesFlag = false;
			const bool GetMemberSites = false;

			var sites = new SitesClass();
			SiteClass site = sites.GetByMemberAndProcessVariables(security, security.SiteGuid, GetMemberSites, GetSchedulesFlag);

			return this.GetBySite(security, maintenanceReasonGuid, site);
		}

		public MaintenanceReasonClass GetBySite(SecurityClass security, Guid maintenanceReasonGuid, SiteClass site)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (site == null)
			{
				throw new ArgumentNullException("site");
			}

			if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) 
				&& !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
			    && !security.HasRight(RIGHT.ADD_QUALITYTAG_RECORD) 
				&& !security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD)
			    && !security.HasRight(RIGHT.VIEW_MAINTENANCE_RECORD) 
				&& !security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD))
			{
				throw new FMInsufficientRightsException();
			}

			var maintenanceReason = new MaintenanceReasonClass { IdentityGuid = maintenanceReasonGuid };

			using (var cmd = new SqlCommand())
			{
				maintenanceReason.SelectSQL(cmd, ContextUtil.IsInTransaction);
				maintenanceReason.Load(this.consolidatedDA.GetDataSet(cmd, security));
			}

			return maintenanceReason;
		}

		public Guid GetIdentityGuid(SecurityClass security, string id)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) 
				&& !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
			    && !security.HasRight(RIGHT.VIEW_MAINTENANCE_RECORD) 
				&& !security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD)
			    && !security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD) 
				&& !security.HasRight(RIGHT.VIEW_QUALITYTAG_RECORD)
			    && !security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD))
			{
				throw new FMInsufficientRightsException();
			}

			var maintenanceReason = new MaintenanceReasonClass { ID = id, SiteGuid = security.SiteGuid };

			using (var cmd = new SqlCommand())
			{
				maintenanceReason.SelectIDSQL(security, cmd, ContextUtil.IsInTransaction);
				maintenanceReason.Load(this.consolidatedDA.GetDataSet(cmd, security));
			}

			return maintenanceReason.IdentityGuid;
		}

		public MaintenanceReasonCollectionClass Enumerate(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			this.CheckSecurity(security);

			var maintenanceReason = new MaintenanceReasonClass();		
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				maintenanceReason.EnumerateSQL(cmd, ContextUtil.IsInTransaction);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			var maintenanceReasonCollection = new MaintenanceReasonCollectionClass();
			DataTable table = set.Tables[0];

			while (table.Rows.Count != 0)
			{
				maintenanceReason = new MaintenanceReasonClass();
				maintenanceReason.Load(set);
				maintenanceReasonCollection.Add(maintenanceReason);
				table.Rows.RemoveAt(0);
			}

			return maintenanceReasonCollection;
		}

		/// <summary>
		/// This method will enumerate the maintenance reasons by
		/// a site.
		/// </summary>
		/// <param name="security"></param>
		/// <returns></returns>
		public MaintenanceReasonCollectionClass EnumerateBySite(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			this.CheckSecurity(security);

			DataSet set;

			var maintenanceReason = new MaintenanceReasonClass { SiteGuid = security.SiteGuid };

			using (var cmd = new SqlCommand())
			{
				maintenanceReason.EnumerateBySiteSQL(cmd, ContextUtil.IsInTransaction);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			var maintenanceReasonCollection = new MaintenanceReasonCollectionClass();
			DataTable table = set.Tables[0];

			while (table.Rows.Count != 0)
			{
				maintenanceReason = new MaintenanceReasonClass
				{
					SiteGuid = security.SiteGuid
				};

				maintenanceReason.Load(set);
				maintenanceReasonCollection.Add(maintenanceReason);

				table.Rows.RemoveAt(0);
			}

			return maintenanceReasonCollection;
		}
		#endregion

		#region Private methods
		private void CheckSecurity(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) &&
				!security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA) &&
				!security.HasRight(RIGHT.VIEW_MAINTENANCE_RECORD) &&
				!security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD) &&
				!security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD) &&
				!security.HasRight(RIGHT.ADD_QUALITYTAG_RECORD))
			{
				throw new FMInsufficientRightsException();
			}
		}
		#endregion
	}
}
