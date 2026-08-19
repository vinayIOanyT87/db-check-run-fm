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
    using System.Collections.Generic;

    [SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class EmailGroupsClass : IDependency, IEmailGroups
	{
		#region Private data members
		private ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		public EmailGroupsClass()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
		}
		#endregion

		#region Private methods
		private void Validate(EmailGroupClass emailGroup)
		{
			if (emailGroup.ID.Length == 0)
			{
				throw (new Exception("ID Required"));
			}

			if (emailGroup.ID == "{None}" || emailGroup.ID == "{Unassigned}" || emailGroup.ID == "{All}")
			{
				throw new Exception("ID is reserved key word " + emailGroup.ID);
			}
		}

		private void UpdatePriorities(SecurityClass security, EmailGroupClass emailGroup)
		{
			// Assign/Unassign AlarmPriorities
			var alarmPriorityEmailGroupMaps = new AlarmPriorityEmailGroupMapsClass();
			var alarmPriorities = new AlarmPrioritiesClass();

			AlarmPriorityCollectionClass existingPriorityCollection = alarmPriorities.EnumerateByEmailGroup(security, emailGroup.IdentityGuid);
			AlarmPriorityCollectionClass newPriorityCollection = emailGroup.PriorityCollection;

			if (newPriorityCollection != null)
			{
				foreach (AlarmPriorityClass newPriority in newPriorityCollection)
				{
					int existingItem;
					for (existingItem = 0; existingItem < existingPriorityCollection.Count; existingItem++)
					{
						AlarmPriorityClass existingPriority = existingPriorityCollection[existingItem];

						if (existingPriority.IdentityGuid == newPriority.IdentityGuid)
						{
							break;
						}
					}

					if (existingItem == existingPriorityCollection.Count)
					{
						var alarmPriorityEmailGroupMap = new AlarmPriorityEmailGroupMapClass();
						alarmPriorityEmailGroupMap.AlarmPriorityGuid = newPriority.IdentityGuid;
						alarmPriorityEmailGroupMap.EmailGroupGuid = emailGroup.IdentityGuid;
						alarmPriorityEmailGroupMap.ID = newPriority.ID;

						alarmPriorityEmailGroupMaps.Add(security, alarmPriorityEmailGroupMap);
					}
					else
					{
						existingPriorityCollection.RemoveAt(existingItem);
					}
				}
			}

			foreach (AlarmPriorityClass existingPriority in existingPriorityCollection)
			{
				alarmPriorityEmailGroupMaps.Purge(security, existingPriority.ID, emailGroup.IdentityGuid, existingPriority.IdentityGuid);
			}
		}
		#endregion

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, EmailGroupClass emailGroup)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (emailGroup == null)
			{
				throw new ArgumentNullException("emailGroup");
			}

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			this.Validate(emailGroup);

			if (Guid.Empty != GetIdentityGuid(security, emailGroup.ID))
			{
				throw (new Exception("E-mail Group Exists"));
			}

			emailGroup.SiteGuid = security.SiteGuid;
			emailGroup.CreatedDate = DateTimeOffset.Now;
			emailGroup.CreatedBy = security.UserID;
			emailGroup.UpdatedDate = emailGroup.CreatedDate;
			emailGroup.UpdatedBy = security.UserID;
			emailGroup.IdentityGuid = Guid.NewGuid();

			using (var cmd = new SqlCommand())
			{
				emailGroup.InsertSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}

			// Create Entity to Site Map
			var entityToSiteMaps = new EntityToSiteMaps();
			var entityToSiteMap = new EntityToSiteMapClass(emailGroup);
			entityToSiteMaps.Add(security, entityToSiteMap, GetType().GUID);

			var applicationStringMaps = new ApplicationStringMapsClass();
			applicationStringMaps.ModifyCollection(security, emailGroup.IdentityGuid, emailGroup.CategoryCollection, null);
			applicationStringMaps.ModifyCollection(security, emailGroup.IdentityGuid, emailGroup.EmailAddressCollection, null);

			this.UpdatePriorities(security, emailGroup);

			return emailGroup.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, EmailGroupClass emailGroup)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (emailGroup == null)
			{
				throw new ArgumentNullException("emailGroup");
			}

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			this.Validate(emailGroup);

			// Verify ID does not exist
			Guid guid = GetIdentityGuid(security, emailGroup.ID);

			if (guid != Guid.Empty && guid != emailGroup.IdentityGuid)
			{
				throw (new Exception("E-mail Group Exists"));
			}

			EmailGroupClass oldEmailGroup = this.Get(security, emailGroup.IdentityGuid);

 			if (oldEmailGroup.IdentityGuid == Guid.Empty)
			{
				throw (new Exception("E-mail Group Not Found"));
			}

			emailGroup.UpdatedDate = DateTimeOffset.Now;
			emailGroup.UpdatedBy = security.UserID;

			using (SqlCommand cmd = new SqlCommand())
			{
				emailGroup.UpdateSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}

			EntityToSiteMaps entityToSiteMaps = new EntityToSiteMaps();
			EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(security, emailGroup.EntityType, emailGroup.IdentityGuid);

			if (emailGroup.SiteGuid != oldEmailGroup.SiteGuid)
			{
				// Purge from EntityToSiteMap
				foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
				{
					entityToSiteMap.ID = emailGroup.ID;
					entityToSiteMaps.Purge(security, entityToSiteMap);
				}

				// Create Entity to Site Map
				EntityToSiteMapClass newEntityToSiteMap = new EntityToSiteMapClass(emailGroup);
				entityToSiteMaps.Add(security, newEntityToSiteMap, GetType().GUID);
			}

			ApplicationStringMapsClass applicationStringMaps = new ApplicationStringMapsClass();
			applicationStringMaps.ModifyCollection(security, emailGroup.IdentityGuid, emailGroup.CategoryCollection, oldEmailGroup.CategoryCollection);
			applicationStringMaps.ModifyCollection(security, emailGroup.IdentityGuid, emailGroup.EmailAddressCollection, oldEmailGroup.EmailAddressCollection);

			this.UpdatePriorities(security, emailGroup);
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid emailGroupGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			EmailGroupClass emailGroup = this.Get(security, emailGroupGuid);

			if (emailGroup.IdentityGuid == Guid.Empty)
			{
				throw (new Exception("E-mail Group Not Found"));
			}

			// Purge Dependencies
			var dependencies = new DependenciesClass(security);
			dependencies.Purge(security, emailGroup);

			// Purge from EntityToSiteMap
			var entityToSiteMaps = new EntityToSiteMaps();

			EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(security, emailGroup.EntityType, emailGroupGuid);

			foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
			{
				entityToSiteMap.ID = emailGroup.ID;
				entityToSiteMaps.Purge(security, entityToSiteMap);
			}

			var applicationStringMaps = new ApplicationStringMapsClass();
			applicationStringMaps.ModifyCollection(security, emailGroup.IdentityGuid, null, emailGroup.CategoryCollection);
			applicationStringMaps.ModifyCollection(security, emailGroup.IdentityGuid, null, emailGroup.EmailAddressCollection);

			emailGroup.PriorityCollection = null;
			UpdatePriorities(security, emailGroup);

			using (SqlCommand cmd = new SqlCommand())
			{
				emailGroup.PurgeSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		public EmailGroupClass Get(SecurityClass security, Guid emailGroupGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			var emailGroup = new EmailGroupClass();
			emailGroup.IdentityGuid = emailGroupGuid;

			using (var cmd = new SqlCommand())
			{
				emailGroup.SelectSQL(cmd, ContextUtil.IsInTransaction);
				emailGroup.Load(this.consolidatedDA.GetDataSet(cmd, security));
			}

			var applicationStringMaps = new ApplicationStringMapsClass();
			emailGroup.CategoryCollection = applicationStringMaps.EnumerateByAssignedToGuidAndType(security, emailGroupGuid, STRING_MAP_TYPE.ALARM_EVENT_CATEGORY);
			emailGroup.EmailAddressCollection = applicationStringMaps.EnumerateByAssignedToGuidAndType(security, emailGroupGuid, STRING_MAP_TYPE.EMAIL_ADDRESS);

			var alarmPriorities = new AlarmPrioritiesClass();
			emailGroup.PriorityCollection = alarmPriorities.EnumerateByEmailGroup(security, emailGroupGuid);

			return emailGroup;
		}

		public Guid GetIdentityGuid(SecurityClass security, string id)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			var emailGroup = new EmailGroupClass();
			emailGroup.ID = id;
			emailGroup.SiteGuid = security.SiteGuid;

			using (var cmd = new SqlCommand())
			{
				emailGroup.SelectByIDSQL(cmd, security, ContextUtil.IsInTransaction);
				emailGroup.Load(this.consolidatedDA.GetDataSet(cmd, security));
			}

			return emailGroup.IdentityGuid;
		}

        /// <summary>
        /// This method will enumerate all the Email Groups with the email, category, and priority information.
        /// </summary>
        /// <param name="security">The security object.</param>
        /// <returns>Returns a list of all email groups.</returns>
        public List<EmailGroupClass> EnumerateWithEmailCatAndPriorityInfo(SecurityClass security)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
            {
                throw new FMInsufficientRightsException();
            }

            var emailGroup = new EmailGroupClass();
            DataSet dataSet;

            using (var cmd = new SqlCommand())
            {
                emailGroup.EnumerateSQL(cmd, security);
                dataSet = this.consolidatedDA.GetDataSet(cmd, security);
            }

            var alarmPriorities = new AlarmPrioritiesClass();
            var applicationStringMaps = new ApplicationStringMapsClass();
            var emailGroupCollection = new List<EmailGroupClass>();

            DataTable table = dataSet.Tables[0];

            while (table.Rows.Count != 0)
            {
                emailGroup = new EmailGroupClass();
                emailGroup.Load(dataSet);

                emailGroup.CategoryCollection = applicationStringMaps.EnumerateByAssignedToGuidAndType(security, emailGroup.IdentityGuid, STRING_MAP_TYPE.ALARM_EVENT_CATEGORY);
                emailGroup.EmailAddressCollection = applicationStringMaps.EnumerateByAssignedToGuidAndType(security, emailGroup.IdentityGuid, STRING_MAP_TYPE.EMAIL_ADDRESS);
                emailGroup.PriorityCollection = alarmPriorities.EnumerateByEmailGroup(security, emailGroup.IdentityGuid);

                emailGroupCollection.Add(emailGroup);
                table.Rows.RemoveAt(0);
            }

            return emailGroupCollection;
        }

        public EmailGroupCollectionClass Enumerate(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			var emailGroup = new EmailGroupClass();
			DataSet dataSet;

			using (var cmd = new SqlCommand())
			{
				emailGroup.EnumerateSQL(cmd, security);
				dataSet = this.consolidatedDA.GetDataSet(cmd, security);
			}

			var emailGroupCollection = new EmailGroupCollectionClass();

			DataTable table = dataSet.Tables[0];

			while (table.Rows.Count != 0)
			{
				emailGroup = new EmailGroupClass();
				emailGroup.Load(dataSet);
				emailGroupCollection.Add(emailGroup);

				table.Rows.RemoveAt(0);
			}

			return emailGroupCollection;
		}

		public EmailGroupCollectionClass EnumerateByAlarmPriority(SecurityClass security, Guid alarmPriorityGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			var emailGroup = new EmailGroupClass();
			emailGroup.SiteGuid = security.SiteGuid;

			DataSet dataSet;

			using (var cmd = new SqlCommand())
			{
				emailGroup.EnumerateByAlarmPrioritySQL(cmd, alarmPriorityGuid);
				dataSet = this.consolidatedDA.GetDataSet(cmd, security);
			}

			var emailGroupCollection = new EmailGroupCollectionClass();

			DataTable table = dataSet.Tables[0];

			while (table.Rows.Count != 0)
			{
				emailGroup = new EmailGroupClass();
				emailGroup.Load(dataSet);
				emailGroupCollection.Add(emailGroup);

				table.Rows.RemoveAt(0);
			}

			return emailGroupCollection;
		}

		#region Dependency methods
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

			if (typeof(SiteClass).IsInstanceOfType(Object))
			{
				var site = (SiteClass)Object;
				EmailGroupCollectionClass emailGroupCollection = Enumerate(security);
				var entityToSiteMaps = new EntityToSiteMaps();

				foreach (EmailGroupClass emailGroup in emailGroupCollection)
				{
					if (site.SiteGuid == emailGroup.SiteGuid)
					{
						EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(security, emailGroup.EntityType, emailGroup.IdentityGuid);

						foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
						{
							if (entityToSiteMap.SiteGuid != site.SiteGuid)
							{
								entityToSiteMap.ID = emailGroup.ID;
								entityToSiteMaps.Purge(security, entityToSiteMap);
							}
						}
					}
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

			// Deleted Groups
			if (typeof(SiteClass).IsInstanceOfType(Object))
			{
				var site = (SiteClass)Object;
				EmailGroupCollectionClass emailGroupCollection = Enumerate(security);
				var entityToSiteMaps = new EntityToSiteMaps();

				foreach (EmailGroupClass emailGroup in emailGroupCollection)
				{
					if (site.SiteGuid == emailGroup.SiteGuid)
					{
						this.Purge(security, emailGroup.IdentityGuid);
					}
					else
					{
						var entityToSiteMap = new EntityToSiteMapClass(emailGroup);
						entityToSiteMap.SiteGuid = site.SiteGuid;
						entityToSiteMaps.Purge(security, entityToSiteMap);
					}
				}
			}
		}
		#endregion
	}
}