using System;
using System.Data;
using System.Data.SqlClient;
using System.ServiceModel;


using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessServices.DataAccessLayer;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	using System.Collections.Generic;

	using FMBusinessObjects.Exceptions;

	using FMCore;

	/// <summary>
	/// Summary description for ApplicationStringsClass.
	/// </summary>
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class ApplicationStringsClass : IDependency, IApplicationStrings
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();



		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, ApplicationStringClass applicationString)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (applicationString == null)
			{
				throw new ArgumentNullException("applicationString");
			}

			if (((applicationString.Type == STRING_TYPE.ADDITIVE_PROFILE
				|| applicationString.Type == STRING_TYPE.DOT_HAZARDOUS_MESSAGE
				|| applicationString.Type == STRING_TYPE.PRODUCT_GROUP
				|| applicationString.Type == STRING_TYPE.PRODUCT_MESSAGE)
				&& !security.HasRight(RIGHT.MODIFY_PRODUCTS))
				|| ((applicationString.Type == STRING_TYPE.ALARM_EVENT_CATEGORY
				|| applicationString.Type == STRING_TYPE.EMAIL_ADDRESS)
				&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
				|| (applicationString.Type == STRING_TYPE.ALLOCATION_GROUP
				&& !security.HasRight(RIGHT.MODIFY_ALLOCATIONS))
				|| (applicationString.Type == STRING_TYPE.COMPANY_TYPE
				&& !security.HasRight(RIGHT.MODIFY_COMPANY_DATA))
                || (applicationString.Type == STRING_TYPE.FUEL_CARD_TYPE
                && !security.HasRight(RIGHT.MODIFY_FUEL_CARD_DATA))
				&& !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
				throw new FMInsufficientRightsException();

			if (GetIdentityGuid(security, applicationString.Type, applicationString.ID) != Guid.Empty)
				throw (new Exception("Application String Exists"));

			applicationString.SiteGuid = security.SiteGuid;
			applicationString.CreatedDate = DateTimeOffset.Now;
			applicationString.CreatedBy = security.UserID;
			applicationString.UpdatedDate = applicationString.CreatedDate;
			applicationString.UpdatedBy = security.UserID;
			applicationString.IdentityGuid = Guid.NewGuid();

			using (var cmd = new SqlCommand())
			{
				applicationString.InsertSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			// Create Entity to Site Map
			if (applicationString.Type != STRING_TYPE.SHIPTO_STATE
		    && applicationString.Type != STRING_TYPE.SITE_CERTIFICATE)
			{
				var entityToSiteMaps = new EntityToSiteMaps();
				var entityToSiteMap = new EntityToSiteMapClass(applicationString);
				entityToSiteMaps.Add(security, entityToSiteMap, GetType().GUID);
			}
			return applicationString.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, ApplicationStringClass applicationString)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (applicationString == null)
			{
				throw new ArgumentNullException("applicationString");
			}

			if (((applicationString.Type == STRING_TYPE.ADDITIVE_PROFILE
			|| applicationString.Type == STRING_TYPE.DOT_HAZARDOUS_MESSAGE
			|| applicationString.Type == STRING_TYPE.PRODUCT_GROUP
			|| applicationString.Type == STRING_TYPE.PRODUCT_MESSAGE)
			&& !security.HasRight(RIGHT.MODIFY_PRODUCTS))
			|| ((applicationString.Type == STRING_TYPE.ALARM_EVENT_CATEGORY
			|| applicationString.Type == STRING_TYPE.EMAIL_ADDRESS)
			&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			|| (applicationString.Type == STRING_TYPE.ALLOCATION_GROUP
			&& !security.HasRight(RIGHT.MODIFY_ALLOCATIONS))
			|| (applicationString.Type == STRING_TYPE.COMPANY_TYPE
			&& !security.HasRight(RIGHT.MODIFY_COMPANY_DATA))
            || (applicationString.Type == STRING_TYPE.FUEL_CARD_TYPE
            && !security.HasRight(RIGHT.MODIFY_FUEL_CARD_DATA))
			&& !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
				throw new FMInsufficientRightsException();

			Guid guid = GetIdentityGuid(security, applicationString.Type, applicationString.ID);
			if (guid != Guid.Empty && guid != applicationString.IdentityGuid)
			{
				throw (new Exception("Application String Exists"));
			}

			ApplicationStringClass oldApplicationString = Get(security, applicationString.IdentityGuid);

          if (oldApplicationString.IdentityGuid == Guid.Empty)
		    {
		        throw (new Exception("Application String Not Found"));
		    }

			applicationString.UpdatedDate = DateTimeOffset.Now;
			applicationString.UpdatedBy = security.UserID;

			using (var cmd = new SqlCommand())
			{
				applicationString.UpdateSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}


			if (applicationString.SiteGuid != oldApplicationString.SiteGuid)
			{
					var entityToSiteMaps = new EntityToSiteMaps();
					EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(security, applicationString.EntityType, applicationString.IdentityGuid);

				// Purge from EntityToSiteMap
				foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
				{
					entityToSiteMap.ID = applicationString.ID;
					entityToSiteMaps.Purge(security, entityToSiteMap);
				}

				// Create Entity to Site Map
				var newEntityToSiteMap = new EntityToSiteMapClass(applicationString);
				entityToSiteMaps.Add(security, newEntityToSiteMap, GetType().GUID);
			}
		}

		public ApplicationStringClass Get(SecurityClass security, Guid guid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var applicationString = new ApplicationStringClass();
			applicationString.IdentityGuid = guid;

			using (var cmd = new SqlCommand())
			{
				applicationString.SelectSQL(cmd, ContextUtil.IsInTransaction);
				applicationString.Load(ConsolidatedDA.GetDataSet(cmd, security));
			}

			if (((applicationString.Type == STRING_TYPE.ADDITIVE_PROFILE
			|| applicationString.Type == STRING_TYPE.DOT_HAZARDOUS_MESSAGE
			|| applicationString.Type == STRING_TYPE.PRODUCT_GROUP
			|| applicationString.Type == STRING_TYPE.PRODUCT_MESSAGE)
			&& !security.HasRight(RIGHT.VIEW_PRODUCTS)
			&& !security.HasRight(RIGHT.MODIFY_PRODUCTS))
			|| ((applicationString.Type == STRING_TYPE.ALARM_EVENT_CATEGORY
			|| applicationString.Type == STRING_TYPE.EMAIL_ADDRESS)
			&& !security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
			&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			|| (applicationString.Type == STRING_TYPE.ALLOCATION_GROUP
			&& !security.HasRight(RIGHT.VIEW_ALLOCATIONS)
			&& !security.HasRight(RIGHT.MODIFY_ALLOCATIONS))
			|| (applicationString.Type == STRING_TYPE.COMPANY_TYPE
			&& !security.HasRight(RIGHT.VIEW_COMPANY_DATA)
			&& !security.HasRight(RIGHT.MODIFY_COMPANY_DATA))
            || (applicationString.Type == STRING_TYPE.FUEL_CARD_TYPE
            && !security.HasRight(RIGHT.VIEW_FUEL_CARD_DATA)
            && !security.HasRight(RIGHT.MODIFY_FUEL_CARD_DATA))
			&& !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
			{	
                throw new FMInsufficientRightsException();
			}

			return applicationString;
		}

		public Guid GetIdentityGuid(SecurityClass security, STRING_TYPE type, string String)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var applicationString = new ApplicationStringClass();
			applicationString.Type = type;
			applicationString.ID = String;
			applicationString.SiteGuid = security.SiteGuid;
			using (var cmd = new SqlCommand())
			{
				applicationString.SelectByIDAndTypeSQL(security, ContextUtil.IsInTransaction, cmd);
				applicationString.Load(ConsolidatedDA.GetDataSet(cmd, security));
			}

			return applicationString.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid applicationStringGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			ApplicationStringClass applicationString = Get(security, applicationStringGuid);

            if (applicationString.IdentityGuid == Guid.Empty)
            {
                throw (new Exception("Application String Not Found"));
            }

			if (((applicationString.Type == STRING_TYPE.ADDITIVE_PROFILE
			|| applicationString.Type == STRING_TYPE.DOT_HAZARDOUS_MESSAGE
			|| applicationString.Type == STRING_TYPE.PRODUCT_GROUP
			|| applicationString.Type == STRING_TYPE.PRODUCT_MESSAGE)
			&& !security.HasRight(RIGHT.MODIFY_PRODUCTS))
			|| ((applicationString.Type == STRING_TYPE.ALARM_EVENT_CATEGORY
			|| applicationString.Type == STRING_TYPE.EMAIL_ADDRESS)
			&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			|| (applicationString.Type == STRING_TYPE.ALLOCATION_GROUP
			&& !security.HasRight(RIGHT.MODIFY_ALLOCATIONS))
			|| (applicationString.Type == STRING_TYPE.COMPANY_TYPE
			&& !security.HasRight(RIGHT.MODIFY_COMPANY_DATA))
            || (applicationString.Type == STRING_TYPE.FUEL_CARD_TYPE
            && !security.HasRight(RIGHT.MODIFY_FUEL_CARD_DATA))
			&& !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
			{	
                throw new FMInsufficientRightsException();
			}


			// Purge Dependencies
			var dependencies = new DependenciesClass(security);
			dependencies.Purge(security, applicationString);

			// Purge from EntityToSiteMap
			if (applicationString.Type != STRING_TYPE.SHIPTO_STATE
			&& applicationString.Type != STRING_TYPE.SITE_CERTIFICATE)
			{
				var entityToSiteMaps = new EntityToSiteMaps();
				EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(security, applicationString.EntityType, applicationStringGuid);

				foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
				{
					entityToSiteMap.ID = applicationString.ID;
					entityToSiteMaps.Purge(security, entityToSiteMap);
				}
			}

			using (var cmd = new SqlCommand())
			{
				applicationString.PurgeSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void ModifyCollection(SecurityClass security,
												Guid siteGuid,
												STRING_TYPE stringType,
												ApplicationStringCollectionClass newApplicationStringCollection,
												ApplicationStringCollectionClass existingApplicationStringCollection)
		{
			if (((stringType == STRING_TYPE.ADDITIVE_PROFILE
			|| stringType == STRING_TYPE.DOT_HAZARDOUS_MESSAGE
			|| stringType == STRING_TYPE.PRODUCT_GROUP
			|| stringType == STRING_TYPE.PRODUCT_MESSAGE)
			&& !security.HasRight(RIGHT.MODIFY_PRODUCTS))
			|| ((stringType == STRING_TYPE.ALARM_EVENT_CATEGORY
			|| stringType == STRING_TYPE.EMAIL_ADDRESS)
			&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			|| (stringType == STRING_TYPE.ALLOCATION_GROUP
			&& !security.HasRight(RIGHT.MODIFY_ALLOCATIONS))
			|| (stringType == STRING_TYPE.COMPANY_TYPE
			&& !security.HasRight(RIGHT.MODIFY_COMPANY_DATA))
			&& !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA)
			|| (stringType == STRING_TYPE.SITE_CERTIFICATE
			&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)))
			{
				throw new FMInsufficientRightsException();
			}

			Guid currentSiteGuid = security.SiteGuid;
			security.SiteGuid = siteGuid;

			if (newApplicationStringCollection != null)
			{
				if (newApplicationStringCollection.Equals(existingApplicationStringCollection))
					return;

				foreach (ApplicationStringClass applicationString in newApplicationStringCollection)
				{
					applicationString.SiteGuid = siteGuid;

					if (existingApplicationStringCollection != null)
					{
						int Item;
						for (Item = 0; Item < existingApplicationStringCollection.Count; Item++)
						{
							ApplicationStringClass existingApplicationString = existingApplicationStringCollection[Item];

							if (existingApplicationString.IdentityGuid == applicationString.IdentityGuid)
							{
								if (existingApplicationString.ID != applicationString.ID)
								{
									Modify(security, applicationString);
								}
								break;
							}
						}

						if (Item == existingApplicationStringCollection.Count)
							Add(security, applicationString);
						else
							existingApplicationStringCollection.RemoveAt(Item);
					}
					else
						Add(security, applicationString);
				}
			}

			if (existingApplicationStringCollection != null)
			{
				foreach (ApplicationStringClass applicationString in existingApplicationStringCollection)
				{
					Purge(security, applicationString.IdentityGuid);
				}
			}

			security.SiteGuid = currentSiteGuid;
		}


		public ApplicationStringCollectionClass Enumerate(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}


			// Additive Profile and Email Address are not included here because
			// they are manipulated by objects dedicated to their functionality 
			STRING_TYPE[] types ={	STRING_TYPE.DOT_HAZARDOUS_MESSAGE,
											STRING_TYPE.PRODUCT_MESSAGE,
											STRING_TYPE.ALLOCATION_GROUP,
											STRING_TYPE.PRODUCT_GROUP,
											STRING_TYPE.COMPANY_TYPE,
											STRING_TYPE.ALARM_EVENT_CATEGORY,
											STRING_TYPE.POINT_TEMPLATE_TYPE,
											STRING_TYPE.POINT_CATEGORY};


			var applicationStringCollection = new ApplicationStringCollectionClass();
			foreach (STRING_TYPE type in types)
			{
				var applicationString = new ApplicationStringClass();
				applicationString.SiteGuid = security.SiteGuid;
				applicationString.Type = type;

				DataSet set;

				using (SqlCommand cmd = new SqlCommand())
				{
					applicationString.EnumerateByTypeSQL(cmd, security);
					set = ConsolidatedDA.GetDataSet(cmd, security);
				}

				DataTable table = set.Tables[0];
				while (table.Rows.Count != 0)
				{
					applicationString = new ApplicationStringClass();
					applicationString.Load(set);
					applicationStringCollection.Add(applicationString);
					table.Rows.RemoveAt(0);
				}
			}

			return applicationStringCollection;
		}

        /// <summary>
        /// This method will enumerate all application string for company type.  Mininum
        /// data is returned.
        /// </summary>
        /// <param name="security"></param>
        /// <returns>Returns a data set of all application strings.</returns>
        public DataSet EnumerateAllCompanyTypes(SecurityClass security)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            DataSet dataSet;

            using (SqlCommand cmd = new SqlCommand())
            {
                var applicationString = new ApplicationStringClass();
                applicationString.EnumerateAllCompanyTypeSql(cmd, ContextUtil.IsInTransaction);
                dataSet = ConsolidatedDA.GetDataSet(cmd, security);
            }

            return dataSet;
        }

        public ApplicationStringCollectionClass EnumerateByType(SecurityClass security, STRING_TYPE type)
		{
			if (security == null)
				throw new ArgumentNullException("security");

			if (!security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
				&& !security.HasRight(RIGHT.VIEW_ALARM_EVENT_LOGS))
			{
				if (((type == STRING_TYPE.ADDITIVE_PROFILE
				|| type == STRING_TYPE.DOT_HAZARDOUS_MESSAGE
				|| type == STRING_TYPE.PRODUCT_GROUP
				|| type == STRING_TYPE.PRODUCT_MESSAGE)
				&& !security.HasRight(RIGHT.VIEW_PRODUCTS)
				&& !security.HasRight(RIGHT.MODIFY_PRODUCTS))
				|| ((type == STRING_TYPE.ALARM_EVENT_CATEGORY
				|| type == STRING_TYPE.EMAIL_ADDRESS)
				&& !security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.VIEW_POINT_TEMPLATES)
				&& !security.HasRight(RIGHT.VIEW_POINTS))
				|| (type == STRING_TYPE.ALLOCATION_GROUP
				&& !security.HasRight(RIGHT.VIEW_ALLOCATIONS)
				&& !security.HasRight(RIGHT.MODIFY_ALLOCATIONS))
				|| (type == STRING_TYPE.COMPANY_TYPE
				&& !security.HasRight(RIGHT.VIEW_COMPANY_DATA)
				&& !security.HasRight(RIGHT.MODIFY_COMPANY_DATA))
                || (type == STRING_TYPE.FUEL_CARD_TYPE
                && !security.HasRight(RIGHT.VIEW_FUEL_CARD_DATA)
                && !security.HasRight(RIGHT.MODIFY_FUEL_CARD_DATA))
				&& !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
				{ 
					throw new FMInsufficientRightsException();
				}
			}

			var applicationString = new ApplicationStringClass();
			applicationString.Type = type;
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				applicationString.EnumerateByTypeSQL(cmd, security);
				set = ConsolidatedDA.GetDataSet(cmd, security);
			}
			var applicationStringCollection = new ApplicationStringCollectionClass();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				applicationString = new ApplicationStringClass();
				applicationString.Load(set);
				applicationStringCollection.Add(applicationString);
				table.Rows.RemoveAt(0);
			}

			return applicationStringCollection;
		}

		public ApplicationStringCollectionClass EnumerateByTypeAndSite(SecurityClass security, STRING_TYPE Type, Guid? siteGuid)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (!security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
				&& !security.HasRight(RIGHT.VIEW_ALARM_EVENT_LOGS))
			{
				if (((Type == STRING_TYPE.ADDITIVE_PROFILE
						|| Type == STRING_TYPE.DOT_HAZARDOUS_MESSAGE
						|| Type == STRING_TYPE.PRODUCT_GROUP
						|| Type == STRING_TYPE.PRODUCT_MESSAGE)
					&& !security.HasRight(RIGHT.VIEW_PRODUCTS)
					&& !security.HasRight(RIGHT.MODIFY_PRODUCTS))
					|| ((Type == STRING_TYPE.ALARM_EVENT_CATEGORY
							|| Type == STRING_TYPE.EMAIL_ADDRESS)
						&& !security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
						&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
					|| (Type == STRING_TYPE.ALLOCATION_GROUP
						&& !security.HasRight(RIGHT.VIEW_ALLOCATIONS)
						&& !security.HasRight(RIGHT.MODIFY_ALLOCATIONS))
					|| (Type == STRING_TYPE.COMPANY_TYPE
						&& !security.HasRight(RIGHT.VIEW_COMPANY_DATA)
						&& !security.HasRight(RIGHT.MODIFY_COMPANY_DATA))
					&& !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
					throw new FMInsufficientRightsException();
			}

			ApplicationStringClass ApplicationString = new ApplicationStringClass();
			ApplicationString.Type = Type;
			DataSet Set = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				ApplicationString.EnumerateByTypeAndSiteSQL(cmd, siteGuid);
				Set = ConsolidatedDA.GetDataSet(cmd, security);
			}
			ApplicationStringCollectionClass ApplicationStringCollection = new ApplicationStringCollectionClass();

			DataTable Table = Set.Tables[0];
			while (Table.Rows.Count != 0)
			{
				ApplicationString = new ApplicationStringClass();
				ApplicationString.Load(Set);
				ApplicationStringCollection.Add(ApplicationString);
				Table.Rows.RemoveAt(0);
			}

			return ApplicationStringCollection;
		}

		public Dictionary<Guid, ApplicationStringClass> EnumerateByApplicationStringGuids(SecurityClass security, List<Guid> applicationStringGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security rights

			if (applicationStringGuidList == null || applicationStringGuidList.Count < 1)
			{
				return new Dictionary<Guid, ApplicationStringClass>();
			}

			DataSet dataSet = null;
			var applicationString = new ApplicationStringClass();
			using (SqlCommand cmd = new SqlCommand())
			{
				applicationString.EnumerateByApplicationStringGuidListSQL(cmd, ContextUtil.IsInTransaction, applicationStringGuidList);
				dataSet = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			var applicationStringDictionary = new Dictionary<Guid, ApplicationStringClass>();

			DataTable table = dataSet.Tables[0];

			while (table.Rows.Count != 0)
			{
				var appString = new ApplicationStringClass();

				appString.Load(dataSet);
				applicationStringDictionary.Add(appString.IdentityGuid, appString);
				table.Rows.RemoveAt(0);
			}

			return applicationStringDictionary;
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
				ApplicationStringCollectionClass applicationStringCollection = Enumerate(security);
				var entityToSiteMaps = new EntityToSiteMaps();
				foreach (ApplicationStringClass applicationString in applicationStringCollection)
				{
					if (site.SiteGuid == applicationString.SiteGuid)
					{
						EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(security, applicationString.EntityType, applicationString.IdentityGuid);
						foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
						{
							if (entityToSiteMap.SiteGuid != site.SiteGuid)
							{
								entityToSiteMap.ID = applicationString.ID;
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


			// Purge ApplicationStrings
			if (typeof(SiteClass).IsInstanceOfType(Object))
			{
				var site = (SiteClass)Object;
				ApplicationStringCollectionClass applicationStringCollection = Enumerate(security);
				var entityToSiteMaps = new EntityToSiteMaps();
				foreach (ApplicationStringClass applicationString in applicationStringCollection)
				{
					if (site.SiteGuid == applicationString.SiteGuid)
					{
						Purge(security, applicationString.IdentityGuid);
					}
					else
					{
						var entityToSiteMap = new EntityToSiteMapClass(applicationString);
						entityToSiteMap.SiteGuid = site.SiteGuid;
						entityToSiteMaps.Purge(security, entityToSiteMap);
					}
				}
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Import(SecurityClass security, ApplicationStringClass ApplicationString)
		{
			security.ThrowIfNull("security");
			ApplicationString.ThrowIfNull("ApplicationString");

			try
			{

				ApplicationStringClass systemString = this.Get(security, ApplicationString.IdentityGuid);


				if (systemString.IdentityGuid == Guid.Empty)
				{
                    if (security == null)
                    {
                        throw new ArgumentNullException("security");
                    }

                    if (ApplicationString == null)
                    {
                        throw new ArgumentNullException("applicationString");
                    }

                    if (((ApplicationString.Type == STRING_TYPE.ADDITIVE_PROFILE
                       || ApplicationString.Type == STRING_TYPE.DOT_HAZARDOUS_MESSAGE
                       || ApplicationString.Type == STRING_TYPE.PRODUCT_GROUP
                       || ApplicationString.Type == STRING_TYPE.PRODUCT_MESSAGE)
                       && !security.HasRight(RIGHT.MODIFY_PRODUCTS))
                       || ((ApplicationString.Type == STRING_TYPE.ALARM_EVENT_CATEGORY
                       || ApplicationString.Type == STRING_TYPE.EMAIL_ADDRESS)
                       && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
                       || (ApplicationString.Type == STRING_TYPE.ALLOCATION_GROUP
                       && !security.HasRight(RIGHT.MODIFY_ALLOCATIONS))
                       || (ApplicationString.Type == STRING_TYPE.COMPANY_TYPE
                       && !security.HasRight(RIGHT.MODIFY_COMPANY_DATA))
                           || (ApplicationString.Type == STRING_TYPE.FUEL_CARD_TYPE
                           && !security.HasRight(RIGHT.MODIFY_FUEL_CARD_DATA))
                       && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
                        throw new FMInsufficientRightsException();

                    if (GetIdentityGuid(security, ApplicationString.Type, ApplicationString.ID) != Guid.Empty)
                        throw (new Exception("Application String Exists"));

                    ApplicationString.SiteGuid = security.SiteGuid;
                    ApplicationString.CreatedDate = DateTimeOffset.Now;
                    ApplicationString.CreatedBy = security.UserID;
                    ApplicationString.UpdatedDate = ApplicationString.CreatedDate;
                    ApplicationString.UpdatedBy = security.UserID;

                    using (var cmd = new SqlCommand())
                    {
                        ApplicationString.InsertSQL(cmd);
                        ConsolidatedDA.ExecuteQuery(security, cmd);
                    }

                    // Create Entity to Site Map
                    if (ApplicationString.Type != STRING_TYPE.SHIPTO_STATE
                     && ApplicationString.Type != STRING_TYPE.SITE_CERTIFICATE)
                    {
                        var entityToSiteMaps = new EntityToSiteMaps();
                        var entityToSiteMap = new EntityToSiteMapClass(ApplicationString);
                        entityToSiteMaps.Add(security, entityToSiteMap, GetType().GUID);
                    }
            }
            
				else
				{

					if (systemString.ID == ApplicationString.ID) {
						throw new Exception("Application String was found in the database.");
					} else {
					//Only thing you can changes isthe id
					systemString.ID = ApplicationString.ID;
					this.Modify(security, systemString);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("[Application String Import Error ID] : " + ApplicationString.ID + ", " + ex.Message);
			}

		}

	}
}
