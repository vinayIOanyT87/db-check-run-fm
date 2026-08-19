namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessServices.DataAccessLayer;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessServices.InternalClasses;

	using FMBusinessObjects.Exceptions;

	/// <summary>
	/// Summary description for UserDataFieldsClass.
	/// </summary>
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class UserDataFieldsClass : IUserDataFields, IDependency
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		protected void UpdateUserDataListValues(SecurityClass security, UserDataFieldClass newUserDataField, UserDataFieldClass oldUserDataField)
		{
			var userDataListValues = new UserDataListValuesClass();

			if (newUserDataField != null)
			{
				foreach (UserDataListValueClass newUserDataListValue in newUserDataField.UserDataListValueCollection)
				{
					newUserDataListValue.UserDataFieldGuid = newUserDataField.IdentityGuid;

					if (oldUserDataField != null)
					{
						int index = 0;

						foreach (UserDataListValueClass oldUserDataListValue in oldUserDataField.UserDataListValueCollection)
						{
							if (oldUserDataListValue.ID == newUserDataListValue.ID)
							{
								break;
							}

							index++;
						}

						if (index < oldUserDataField.UserDataListValueCollection.Count)
						{
							oldUserDataField.UserDataListValueCollection.RemoveAt(index);
						}
						else
						{
							userDataListValues.Add(security, newUserDataListValue, newUserDataField.UserDataEntityType);
						}
					}
					else
					{
						userDataListValues.Add(security, newUserDataListValue, newUserDataField.UserDataEntityType);
					}
				}
			}

			if (oldUserDataField != null)
			{
				foreach (UserDataListValueClass oldUserDataListValue in oldUserDataField.UserDataListValueCollection)
				{
					userDataListValues.Purge(security, oldUserDataListValue.UserDataFieldGuid, oldUserDataListValue.ID, oldUserDataField.UserDataEntityType);
				}
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Add(SecurityClass security, UserDataFieldClass userDataField)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (userDataField == null)
			{
				throw new ArgumentNullException("userDataField");
			}

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) 
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES)
			    && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			if (GetIdentityGuid(
				security,
				userDataField.UserDataEntityType,
				userDataField.TransactionAliasGuid,
				userDataField.Number,
				userDataField.DispatchField) != Guid.Empty)
			{
				throw (new Exception("UserDataField Exists"));
			}

			// If EntityAssignmentMap exists it must be purged provided this isn't
			// UserData associated with a TransactionAlias
			if (userDataField.UserDataEntityType != ENTITY_TYPE.TRANSACTION_ALIAS &&
				userDataField.UserDataEntityType != ENTITY_TYPE.TRANSACTION_ALIAS_LINE_ITEM)
			{
				var entityToSiteMaps = new EntityToSiteMaps();
				EntityToSiteMapCollectionClass entityToSiteMapCollection = 
										entityToSiteMaps.EnumerateByTypeIDAndSiteGuid(security, userDataField.EntityType, security.SiteGuid);
				
				foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
				{
					entityToSiteMap.ID = userDataField.ID;
					entityToSiteMaps.Purge(security, entityToSiteMap);
				}

				// SiteGuid for TransactionAlias and TransactionAliasLineItem is set to TransactionAlias.SiteGuid
				userDataField.SiteGuid = security.SiteGuid;
			}

			userDataField.CreatedDate = DateTimeOffset.Now;
			userDataField.CreatedBy = security.UserID;
			userDataField.UpdatedDate = userDataField.CreatedDate;
			userDataField.UpdatedBy = security.UserID;
			userDataField.IdentityGuid = Guid.NewGuid();

			using (var cmd = new SqlCommand())
			{
				userDataField.InsertSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			this.UpdateUserDataListValues(security, userDataField, null);
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, UserDataFieldClass userDataField)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (userDataField == null)
			{
				throw new ArgumentNullException("userDataField");
			}

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) 
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES)
			    && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			Guid identityGuid = GetIdentityGuid(security, 
												userDataField.UserDataEntityType, 
												userDataField.TransactionAliasGuid, 
												userDataField.Number, 
												userDataField.DispatchField);

			if (identityGuid != Guid.Empty && identityGuid != userDataField.IdentityGuid)
			{
				throw (new Exception("UserDataField Exists"));
			}

			// If EntityAssignmentMap exists it must be purged provided this isn't
			// UserData associated with a TransactionAlias
			var transactionAlias = new TransactionAliasClass();

			if (userDataField.UserDataEntityType != transactionAlias.EntityType &&
				userDataField.UserDataEntityType != ENTITY_TYPE.TRANSACTION_ALIAS_LINE_ITEM)
			{
				var entityToSiteMaps = new EntityToSiteMaps();
				EntityToSiteMapCollectionClass entityToSiteMapCollection = 
										entityToSiteMaps.EnumerateByTypeIDAndSiteGuid(security, userDataField.EntityType, security.SiteGuid);

				foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
				{
					entityToSiteMaps.Purge(security, entityToSiteMap);
				}
			}

			UserDataFieldClass oldUserDataField = Get(security, userDataField.IdentityGuid, userDataField.UserDataEntityType);

			if (oldUserDataField.IdentityGuid == Guid.Empty)
			{
				throw (new Exception("UserDataField Not Found"));
			}

			userDataField.UpdatedDate = DateTimeOffset.Now;
			userDataField.UpdatedBy = security.UserID;

			using (var cmd = new SqlCommand())
			{
				userDataField.UpdateSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			this.UpdateUserDataListValues(security, userDataField, oldUserDataField);
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid identityGuid, ENTITY_TYPE userDataFieldEntityType)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) 
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES)
			    && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			UserDataFieldClass userDataField = Get(security, identityGuid, userDataFieldEntityType);
			
			if (userDataField.IdentityGuid == Guid.Empty)
			{
				return;
			}

			this.UpdateUserDataListValues(security, null, userDataField);

			var dependencies = new DependenciesClass(security);
			dependencies.Purge(security, userDataField);

			using (var cmd = new SqlCommand())
			{
				userDataField.PurgeSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		public UserDataFieldClass Get(SecurityClass security, Guid identityGuid, ENTITY_TYPE entityType)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) 
				&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
			    && !security.HasRight(RIGHT.VIEW_TRANSACTION_ALIASES) 
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES)
			    && !security.HasRight(RIGHT.VIEW_DISPATCH) && !security.HasRight(RIGHT.MODIFY_DISPATCH)
			    && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			var userDataField = new UserDataFieldClass { IdentityGuid = identityGuid, UserDataEntityType = entityType };

			using (var cmd = new SqlCommand())
			{
				userDataField.SelectSQL(cmd, ContextUtil.IsInTransaction);
				userDataField.Load(ConsolidatedDA.GetDataSet(cmd, security));
			}

			if (userDataField.UserDataType == USER_DATA_TYPE.LIST)
			{
				var userDataListValues = new UserDataListValuesClass();
				userDataField.UserDataListValueCollection = userDataListValues.Enumerate(security, userDataField.IdentityGuid, userDataField.UserDataEntityType);
			}

			return userDataField;
		}

		public Guid GetIdentityGuid(SecurityClass security, ENTITY_TYPE entityType, Guid transactionAliasGuid, int number, bool dispatchField)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) 
				&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
			    && !security.HasRight(RIGHT.VIEW_TRANSACTION_ALIASES) 
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES)
			    && !security.HasRight(RIGHT.VIEW_DISPATCH) && !security.HasRight(RIGHT.MODIFY_DISPATCH)
			    && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			var userDataField = new UserDataFieldClass
			                    {
				                    SiteGuid = security.SiteGuid,
				                    UserDataEntityType = entityType,
				                    TransactionAliasGuid = transactionAliasGuid,
				                    Number = number,
				                    DispatchField = dispatchField
			                    };

			using (var cmd = new SqlCommand())
			{
				userDataField.SelectByIDSQL(cmd, ContextUtil.IsInTransaction);
				userDataField.Load(ConsolidatedDA.GetDataSet(cmd, security));
			}

			return userDataField.IdentityGuid;
		}

		public UserDataFieldCollectionClass Enumerate(SecurityClass security, ENTITY_TYPE userDataEntityType)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var userDataField = new UserDataFieldClass { SiteGuid = security.SiteGuid, UserDataEntityType = userDataEntityType };

			using (var cmd = new SqlCommand())
			{
				userDataField.EnumerateSQL(cmd);
				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

				var userDataFieldCollection = new UserDataFieldCollectionClass();
				var userDataListValues = new UserDataListValuesClass();

				DataTable table = set.Tables[0];

				while (table.Rows.Count != 0)
				{
					userDataField = new UserDataFieldClass { UserDataEntityType = userDataEntityType };
					userDataField.Load(set);

					if (userDataField.UserDataType == USER_DATA_TYPE.LIST)
					{
						userDataField.UserDataListValueCollection = 
									userDataListValues.Enumerate(security, userDataField.IdentityGuid, userDataField.UserDataEntityType);
					}

					userDataFieldCollection.Add(userDataField);
					table.Rows.RemoveAt(0);
				}

				return userDataFieldCollection;
			}
		}

		public UserDataFieldCollectionClass EnumerateByEntityType(	SecurityClass security, 
																	ENTITY_TYPE entityType, 
																	Guid transactionAliasGuid, 
																	bool byUser, 
																	bool dispatchField)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var userDataField = new UserDataFieldClass
			                    {
				                    SiteGuid = security.SiteGuid,
				                    UserDataEntityType = entityType,
				                    TransactionAliasGuid = transactionAliasGuid,
				                    DispatchField = dispatchField
			                    };

			using (var cmd = new SqlCommand())
			{
				userDataField.EnumerateByEntityTypeIDSQL(cmd, security, byUser, ContextUtil.IsInTransaction);
				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

				var userDataFieldCollection = new UserDataFieldCollectionClass();
				var userDataListValues = new UserDataListValuesClass();
				DataTable table = set.Tables[0];

				while (table.Rows.Count != 0)
				{
					userDataField = new UserDataFieldClass { UserDataEntityType = entityType };
					userDataField.Load(set);

					if (userDataField.UserDataType == USER_DATA_TYPE.LIST)
					{
						userDataField.UserDataListValueCollection = 
							userDataListValues.Enumerate(security, userDataField.IdentityGuid, entityType);
					}

					userDataFieldCollection.Add(userDataField);
					table.Rows.RemoveAt(0);
				}

				return userDataFieldCollection;
			}
		}

		/// <summary>
		/// This method goes through the given user data list.  
		/// For each value, if it is blank and it is a list type, set it to the default value which is item 0
		/// </summary>
		/// <param name="security"></param>
		/// <param name="userDataValueList"></param>
		/// <param name="entityType"></param>
		internal static void SetDefaults(SecurityClass security, UserDataClass userDataValueList, ENTITY_TYPE entityType)
		{
			var systemSettingBusObj = new SystemSettingsClass();

			SystemSettingClass systemSettingDataObj = systemSettingBusObj.Get(security);

			if (systemSettingDataObj.UserDataListDefaultToFirstValue)
			{
				var userDataService = new UserDataFieldsClass();
				userDataService.SetDefaults(security, userDataValueList, entityType, Guid.Empty, false);
			}
		}

		/// <summary>
		/// This method goes through the given user data list.  
		/// For each value, if it is blank and it is a list type, set it to the default value which is item 0
		/// </summary>
		/// <param name="security"></param>
		/// <param name="userDataValueList"></param>
		/// <param name="entityType"></param>
		/// <param name="transactionAliasGuid"></param>
		/// <param name="byUser"></param>
		internal void SetDefaults(
			SecurityClass security,
			UserDataClass userDataValueList,
			ENTITY_TYPE entityType,
			Guid transactionAliasGuid,
			bool byUser)
		{
			// Get the user data definition
			UserDataFieldCollectionClass userDataFieldDefintionList = EnumerateByEntityType(
				security,
				entityType,
				transactionAliasGuid,
				byUser,
				false);

			var userDataListValuesService = new UserDataListValuesClass();

			for (int userDataFieldIndex = 0; userDataFieldIndex < userDataFieldDefintionList.Count; userDataFieldIndex++)
			{
				UserDataFieldClass currentUserDataFieldDefinition = userDataFieldDefintionList[userDataFieldIndex];

				if (currentUserDataFieldDefinition.UserDataType == USER_DATA_TYPE.LIST
				    && string.IsNullOrEmpty(userDataValueList[userDataFieldIndex]))
				{
					UserDataListValueCollectionClass listValues = userDataListValuesService.Enumerate(
						security,
						currentUserDataFieldDefinition.IdentityGuid, entityType);
					if ((listValues != null) && (listValues.Count > 0))
					{
						userDataValueList[userDataFieldIndex] = listValues[0].ID;
					}
				}
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

				// Verify there are no UserDataFields owned by this Site
				var userDataField = new UserDataFieldClass();
				
				if (entityToSiteMap.TypeID == userDataField.EntityType)
				{
					ArrayList userDataEntityTypes = UserDataFieldClass.GetUserDataEntityTypes();
					int totalCount = 0;

					foreach (ENTITY_TYPE userDataEntityType in userDataEntityTypes)
					{
						UserDataFieldCollectionClass userDataFieldCollection = Enumerate(security, userDataEntityType);
						totalCount += userDataFieldCollection.Count;
					}

					if (totalCount != 0)
					{
						throw new Exception("UserDataField Exist - " + entityToSiteMap.ID);
					}
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

			// See Sites.Modify, this call only occurs with SiteGroup is changed.
			var siteObject = Object as SiteClass;

			if (siteObject != null)
			{
				SiteClass site = siteObject;
				var userDataField = new UserDataFieldClass();
				var entityToSiteMaps = new EntityToSiteMaps();
				EntityToSiteMapCollectionClass entityToSiteMapCollection = 
												entityToSiteMaps.EnumerateByTypeIDAndGuid(security, userDataField.EntityType, site.IdentityGuid);
				
				foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
				{
					entityToSiteMap.ID = userDataField.ID;
					entityToSiteMaps.Purge(security, entityToSiteMap);
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

			var siteObject = Object as SiteClass;

			if (siteObject != null)
			{
				SiteClass site = siteObject;
				var entityToSiteMaps = new EntityToSiteMaps();
				var userDataField = new UserDataFieldClass();
				var entityToSiteMapCollection = 	entityToSiteMaps.EnumerateByTypeIDAndSiteGuid(security, userDataField.EntityType, site.IdentityGuid);
				
				foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
				{
					entityToSiteMap.ID = userDataField.ID;
					entityToSiteMaps.Purge(security, entityToSiteMap);
				}

				var userDataFieldEntityTypes = new ArrayList();

				foreach (ENTITY_TYPE userDataFieldEntityType in userDataFieldEntityTypes)
				{
					UserDataFieldCollectionClass userDataFieldCollection = Enumerate(security, userDataFieldEntityType);

					foreach (var fieldClass in userDataFieldCollection)
					{
						var existingUserDataField = (UserDataFieldClass)fieldClass;
						Purge(security, existingUserDataField.IdentityGuid, existingUserDataField.UserDataEntityType);
					}
				}
			}
		}
	}
}
