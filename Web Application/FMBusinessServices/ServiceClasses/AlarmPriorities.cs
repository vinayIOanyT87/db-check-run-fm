using System;
using System.ServiceModel;
using System.Data;
using System.Data.SqlClient;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;

using FMBusinessServices.DataAccessLayer;
using FMBusinessObjects.Exceptions;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class AlarmPrioritiesClass : IDependency, IAlarmPriorities
	{
		#region Private data Members
		private ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		public AlarmPrioritiesClass()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
		}
		#endregion

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, AlarmPriorityClass alarmPriority)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (alarmPriority == null)
			{
				throw new ArgumentNullException("alarmPriority");
			}

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			if (GetIdentityGuid(security, alarmPriority.ID) != Guid.Empty)
			{
				throw (new Exception("Alarm Priority Exists"));
			}

			this.Validate(alarmPriority);

			alarmPriority.SiteGuid = security.SiteGuid;
			alarmPriority.CreatedDate = DateTimeOffset.Now;
			alarmPriority.CreatedBy = security.UserID;
			alarmPriority.UpdatedDate = alarmPriority.CreatedDate;
			alarmPriority.UpdatedBy = security.UserID;
			alarmPriority.IdentityGuid = Guid.NewGuid();

			using (var cmd = new SqlCommand())
			{
				alarmPriority.InsertSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}

			// Create Entity to Site Map
			var entityToSiteMaps = new EntityToSiteMaps();
			var entityToSiteMap = new EntityToSiteMapClass(alarmPriority);
			entityToSiteMaps.Add(security, entityToSiteMap, GetType().GUID);

			return alarmPriority.IdentityGuid;
		}

        // The only difference between the import and add is that the import uses the  guid on the alarmPriority object instead of
        // generating a new guid
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public Guid Import(SecurityClass security, AlarmPriorityClass alarmPriority)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (alarmPriority == null)
            {
                throw new ArgumentNullException("alarmPriority");
            }

            if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
            {
                throw new FMInsufficientRightsException();
            }

            if (GetIdentityGuid(security, alarmPriority.ID) != Guid.Empty)
            {
                throw (new Exception("Alarm Priority Exists"));
            }

            this.Validate(alarmPriority);

            alarmPriority.SiteGuid = security.SiteGuid;
            alarmPriority.CreatedDate = DateTimeOffset.Now;
            alarmPriority.CreatedBy = security.UserID;
            alarmPriority.UpdatedDate = alarmPriority.CreatedDate;
            alarmPriority.UpdatedBy = security.UserID;

            using (var cmd = new SqlCommand())
            {
                alarmPriority.InsertSQL(cmd);
                this.consolidatedDA.ExecuteQuery(security, cmd);
            }

            // Create Entity to Site Map
            var entityToSiteMaps = new EntityToSiteMaps();
            var entityToSiteMap = new EntityToSiteMapClass(alarmPriority);
            entityToSiteMaps.Add(security, entityToSiteMap, GetType().GUID);

            return alarmPriority.IdentityGuid;
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, AlarmPriorityClass alarmPriority)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (alarmPriority == null)
			{
				throw new ArgumentNullException("alarmPriority");
			}

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			this.Validate(alarmPriority);

			Guid guid = GetIdentityGuid(security, alarmPriority.ID);

			if (guid != Guid.Empty && guid != alarmPriority.IdentityGuid)
			{
				throw (new Exception("Alarm Priority Exists"));
			}

			AlarmPriorityClass oldAlarmPriority = Get(security, alarmPriority.IdentityGuid);

			if (oldAlarmPriority.IdentityGuid == Guid.Empty)
			{
				throw (new Exception("Alarm Priority Not Found"));
			}

			alarmPriority.UpdatedDate = DateTimeOffset.Now;
			alarmPriority.UpdatedBy = security.UserID;

			using (var cmd = new SqlCommand())
			{
				alarmPriority.UpdateSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}


		    if (alarmPriority.SiteGuid != oldAlarmPriority.SiteGuid)
		    {
		        var entityToSiteMaps = new EntityToSiteMaps();
		        EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(
		            security,
		            alarmPriority.EntityType,
		            alarmPriority.IdentityGuid);
                
		        // Purge from EntityToSiteMap
		        foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
		        {
		            entityToSiteMap.ID = alarmPriority.ID;
		            entityToSiteMaps.Purge(security, entityToSiteMap);
		        }

		        // Create Entity to Site Map
		        var newEntityToSiteMap = new EntityToSiteMapClass(alarmPriority);
		        entityToSiteMaps.Add(security, newEntityToSiteMap, GetType().GUID);
		    }
		}

		public AlarmPriorityClass Get(SecurityClass security, Guid guid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			var alarmPriority = new AlarmPriorityClass { IdentityGuid = guid };

		    using (var cmd = new SqlCommand())
			{
				alarmPriority.SelectSQL(cmd, ContextUtil.IsInTransaction);
				alarmPriority.Load(this.consolidatedDA.GetDataSet(cmd, security));
			}

			return alarmPriority;
		}

		public Guid GetIdentityGuid(SecurityClass security, string alarmPriorityID)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			var alarmPriority = new AlarmPriorityClass { ID = alarmPriorityID, SiteGuid = security.SiteGuid };

		    using (var cmd = new SqlCommand())
			{
				alarmPriority.SelectByIDSQL(cmd, security, ContextUtil.IsInTransaction);
				alarmPriority.Load(this.consolidatedDA.GetDataSet(cmd, security));
			}

			return alarmPriority.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid alarmPriorityGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			AlarmPriorityClass alarmPriority = Get(security, alarmPriorityGuid);

			if (alarmPriority.IdentityGuid == Guid.Empty)
			{
				throw (new Exception("Alarm Priority Not Found"));
			}

			// Purge Dependencies
			var dependencies = new DependenciesClass(security);
			dependencies.Purge(security, alarmPriority);

			// Purge from EntityToSiteMap
			var entityToSiteMaps = new EntityToSiteMaps();
			EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(security, alarmPriority.EntityType, alarmPriorityGuid);
			foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
			{
				entityToSiteMap.ID = alarmPriority.ID;
				entityToSiteMaps.Purge(security, entityToSiteMap);
			}

			// Purge AlarmPriorityEmailGroupMaps
			var alarmPriorityEmailGroupMaps = new AlarmPriorityEmailGroupMapsClass();
			var emailGroups = new EmailGroupsClass();
			EmailGroupCollectionClass emailGroupCollection = emailGroups.EnumerateByAlarmPriority(security, alarmPriorityGuid);

			foreach (EmailGroupClass emailGroup in emailGroupCollection)
			{
				alarmPriorityEmailGroupMaps.Purge(security, alarmPriority.ID, emailGroup.IdentityGuid, alarmPriorityGuid);
			}

			using (var cmd = new SqlCommand())
			{
				alarmPriority.PurgeSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		public AlarmPriorityCollectionClass Enumerate(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) &&
				!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) &&
				!security.HasRight(RIGHT.VIEW_ALARM_EVENT_LOGS) &&
				!security.HasRight(RIGHT.OPERATE_VIEW_ALARM_HISTORY) &&
				!security.HasRight(RIGHT.OPERATE_VIEW_ALARM_SUMMARY) &&
				!security.HasRight(RIGHT.OPERATE_VIEW_GRAPHICS) &&
				!security.HasRight(RIGHT.OPERATE_VIEW_IM_REPORTS) &&
				!security.HasRight(RIGHT.OPERATE_VIEW_MOVEMENT_HISTORY) &&
				!security.HasRight(RIGHT.OPERATE_VIEW_MOVEMENT_SUMMARY)  &&
				!security.HasRight(RIGHT.OPERATE_VIEW_POINTS) &&
				!security.HasRight(RIGHT.OPERATE_VIEW_POINT_GROUPS) &&
				!security.HasRight(RIGHT.OPERATE_VIEW_TRENDS) &&
				!security.HasRight(RIGHT.OPERATE_VIEW_UNPUBLISHED))
			{
				throw new FMInsufficientRightsException();
			}

			var alarmPriority = new AlarmPriorityClass();
			DataSet dataSet;

			using (var cmd = new SqlCommand())
			{
				alarmPriority.EnumerateSQL(cmd, security);
				dataSet = this.consolidatedDA.GetDataSet(cmd, security);
			}

			var alarmPriorityCollection = new AlarmPriorityCollectionClass();

			DataTable table = dataSet.Tables[0];

			while (table.Rows.Count != 0)
			{
				alarmPriority = new AlarmPriorityClass();
				alarmPriority.Load(dataSet);
				alarmPriorityCollection.Add(alarmPriority);
				table.Rows.RemoveAt(0);
			}

			return alarmPriorityCollection;
		}

		public AlarmPriorityCollectionClass EnumerateByEmailGroup(SecurityClass security, Guid groupGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			var alarmPriority = new AlarmPriorityClass();
			alarmPriority.SiteGuid = security.SiteGuid;

			DataSet dataSet;

			using (var cmd = new SqlCommand())
			{
				alarmPriority.EnumerateByEmailGroupSQL(cmd, groupGuid, ContextUtil.IsInTransaction);
				dataSet = this.consolidatedDA.GetDataSet(cmd, security);
			}

			var alarmPriorityCollection = new AlarmPriorityCollectionClass();

			DataTable table = dataSet.Tables[0];

			while (table.Rows.Count != 0)
			{
				alarmPriority = new AlarmPriorityClass();
				alarmPriority.Load(dataSet);
				alarmPriorityCollection.Add(alarmPriority);
				table.Rows.RemoveAt(0);
			}

			return alarmPriorityCollection;
		}

		#region Private methods
		private void Validate(AlarmPriorityClass alarmPriority)
		{
			if (alarmPriority.ID.Length == 0)
			{
				throw (new Exception("ID Required"));
			}

			if (alarmPriority.ID == "{None}" || alarmPriority.ID == "{Unassigned}" || alarmPriority.ID == "{All}")
			{
				throw new Exception("ID is reserved key word " + alarmPriority.ID);
			}
		}
		#endregion

		#region Dependency methods
		void IDependency.Insert(SecurityClass security, BaseDataObject dataObject, bool preOperation)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (dataObject == null)
			{
				throw new ArgumentNullException("dataObject");
			}
		}

		void IDependency.Update(SecurityClass security, BaseDataObject dataObject)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (dataObject == null)
			{
				throw new ArgumentNullException("dataObject");
			}

			if (typeof(SiteClass).IsInstanceOfType(dataObject))
			{
				var site = (SiteClass)dataObject;
				AlarmPriorityCollectionClass alarmPriorityCollection = Enumerate(security);
				var entityToSiteMaps = new EntityToSiteMaps();

				foreach (AlarmPriorityClass alarmPriority in alarmPriorityCollection)
				{
					if (site.SiteGuid == alarmPriority.SiteGuid)
					{
						EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(security, alarmPriority.EntityType, alarmPriority.IdentityGuid);

						foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
						{
							if (entityToSiteMap.SiteGuid != site.SiteGuid)
							{
								entityToSiteMap.ID = alarmPriority.ID;
								entityToSiteMaps.Purge(security, entityToSiteMap);
							}
						}
					}
				}
			}
		}

		void IDependency.Purge(SecurityClass security, BaseDataObject dataObject)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (dataObject == null)
			{
				throw new ArgumentNullException("dataObject");
			}

			// Purge AlarmPriorities
			if (typeof(SiteClass).IsInstanceOfType(dataObject))
			{
				var site = (SiteClass)dataObject;
				AlarmPriorityCollectionClass alarmPriorityCollection = Enumerate(security);
				var entityToSiteMaps = new EntityToSiteMaps();

				foreach (AlarmPriorityClass alarmPriority in alarmPriorityCollection)
				{
					if (site.SiteGuid == alarmPriority.SiteGuid)
					{
						this.Purge(security, alarmPriority.IdentityGuid);
					}
					else
					{
						var entityToSiteMap = new EntityToSiteMapClass(alarmPriority);
						entityToSiteMap.SiteGuid = site.SiteGuid;
						entityToSiteMaps.Purge(security, entityToSiteMap);
					}
				}
			}
		}
		#endregion
	}
}