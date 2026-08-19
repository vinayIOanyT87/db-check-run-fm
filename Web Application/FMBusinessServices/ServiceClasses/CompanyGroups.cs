using System;
using System.Data;
using System.Data.SqlClient;
using System.Security;
using System.ServiceModel;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessServices.DataAccessLayer;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	using FMBusinessObjects.Exceptions;

	/// <summary>
	/// Summary description for CompanyGroupsClass.
	/// </summary>
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class CompanyGroupsClass : IDependency, ICompanyGroups
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass ( );

		private void Validate ( SecurityClass security, CompanyGroupClass companyGroup )
		{
			if (string.IsNullOrEmpty(companyGroup.ID))
			{
				throw ( new Exception ( "ID Required" ) );
			}

			if (companyGroup.ID == "{None}" || companyGroup.ID == "{Unassigned}" || companyGroup.ID == "{All}")
			{
				throw new Exception ( "ID is reserved key word " + companyGroup.ID );
			}


			// Preclude the same product from being assigned to a company more than once
			var productMaps = new ProductMapsClass ( );
			var companyMaps = new CompanyMapsClass ( );

			foreach (ProductMapClass authorizedProduct in companyGroup.AuthorizedProductCollection)
			{
				ProductMapCollectionClass existingAuthorizedProductCollection = productMaps.EnumerateByAssignedGuidAndType ( security, authorizedProduct.AssignedGuid, PRODUCT_MAP_TYPE.PRODUCT_COMPANY_GROUP_MAP );
				foreach (ProductMapClass existingAuthorizedProduct in existingAuthorizedProductCollection)
				{
					if (existingAuthorizedProduct.AssignedToGuid != companyGroup.IdentityGuid)
					{
						CompanyMapCollectionClass companyAssignments = companyMaps.EnumerateByAssignedToGuidAndType(security, existingAuthorizedProduct.AssignedToGuid, COMPANY_MAP_TYPE.COMPANY_GROUP_COMPANY_MAP );
						foreach (CompanyMapClass companyAssignment in companyAssignments)
						{
							foreach (CompanyMapClass currentGroupCompanyAssignment in companyGroup.AssignedCompanyCollection)
							{
								if (companyAssignment.AssignedGuid == currentGroupCompanyAssignment.AssignedGuid)
								{
									throw new Exception ( "[Product] " + authorizedProduct.AssignedID + " [and Company] " + companyAssignment.AssignedID + " [Assigned to Company Group] " + existingAuthorizedProduct.AssignedToID );
								}
							}
						}
					}
				}

				existingAuthorizedProductCollection = productMaps.EnumerateByAssignedGuidAndType ( security, authorizedProduct.AssignedGuid, PRODUCT_MAP_TYPE.PRODUCT_COMPANY_MAP );
				foreach (ProductMapClass existingAuthorizedProduct in existingAuthorizedProductCollection)
				{
					foreach (CompanyMapClass companyMap in companyGroup.AssignedCompanyCollection)
					{
						if (companyMap.AssignedGuid == existingAuthorizedProduct.AssignedToGuid)
						{
							throw new Exception ( "[Product] " + authorizedProduct.AssignedID + " [Assigned to Company] " + existingAuthorizedProduct.AssignedToID );
						}
					}
				}
			}

		}

		[OperationBehavior( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public Guid Add( SecurityClass security, CompanyGroupClass companyGroup )
		{
			if (security == null)
			{
				throw new ArgumentNullException ( "security" );
			}

			if (companyGroup == null)
			{
				throw new ArgumentNullException ( "companyGroup" );
			}

			if (!security.HasRight(RIGHT.MODIFY_COMPANY_DATA) && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			Validate ( security, companyGroup );

			if (GetIdentityGuid(security, companyGroup.ID) != Guid.Empty)
			{
				throw ( new Exception ( "CompanyGroup Exists" ) );
			}

			companyGroup.SiteGuid = security.SiteGuid;
			companyGroup.CreatedDate = DateTimeOffset.Now;
			companyGroup.CreatedBy = security.UserID;
			companyGroup.UpdatedDate = companyGroup.CreatedDate;
			companyGroup.UpdatedBy = security.UserID;
			companyGroup.Deleted = false;
			companyGroup.IdentityGuid = Guid.NewGuid();

			using (var cmd = new SqlCommand())
			{
				companyGroup.InsertSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			// Create Entity to Site Map
			var entityToSiteMaps = new EntityToSiteMaps ( );
			var entityToSiteMap = new EntityToSiteMapClass ( companyGroup );
			entityToSiteMaps.Add ( security, entityToSiteMap, GetType().GUID );

			var companyMaps = new CompanyMapsClass ( );
			companyMaps.ModifyCollection ( security, companyGroup.IdentityGuid, companyGroup.ID, companyGroup.AssignedCompanyCollection, null );

			var productMaps = new ProductMapsClass ( );
			productMaps.ModifyCollection(security, companyGroup.IdentityGuid, companyGroup.ID, false, companyGroup.AuthorizedProductCollection, null);

			return companyGroup.IdentityGuid;
		}

		[OperationBehavior( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public void Modify( SecurityClass security, CompanyGroupClass companyGroup )
		{
			if (security == null)
			{
				throw new ArgumentNullException ( "security" );
			}

			if (companyGroup == null)
			{
				throw new ArgumentNullException ( "companyGroup" );
			}

			if (!security.HasRight(RIGHT.MODIFY_COMPANY_DATA) && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			Validate ( security, companyGroup );

			Guid identityGuid = GetIdentityGuid ( security, companyGroup.ID );
			if (identityGuid != Guid.Empty && identityGuid != companyGroup.IdentityGuid)
			{
				throw ( new Exception ( "CompanyGroup Exists" ) );
			}

			CompanyGroupClass oldCompanyGroup = Get(security, companyGroup.IdentityGuid);

			if (security.SiteGuid != companyGroup.SiteGuid
					&& oldCompanyGroup.IdentityGuid == companyGroup.IdentityGuid
					&& oldCompanyGroup.IdentityGuid != Guid.Empty)
			{
				FMInsufficientRightsException f = new FMInsufficientRightsException();
				throw f;
			}

			if (oldCompanyGroup.IdentityGuid == Guid.Empty)
			{
				throw ( new Exception ( "CompanyGroup Not Found" ) );
			}

			companyGroup.UpdatedDate = DateTimeOffset.Now;
			companyGroup.UpdatedBy = security.UserID;

			using(var cmd = new SqlCommand())
			{
				companyGroup.UpdateSQL(cmd);
				ConsolidatedDA.ExecuteQuery (security, cmd);
			}


			if (companyGroup.SiteGuid != oldCompanyGroup.SiteGuid)
			{
				var entityToSiteMaps = new EntityToSiteMaps();
				EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(security, companyGroup.EntityType, companyGroup.IdentityGuid);

				// Purge from EntityToSiteMap
				foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
				{
					entityToSiteMap.ID = companyGroup.ID;
					entityToSiteMaps.Purge ( security, entityToSiteMap );
				}

				// Create Entity to Site Map
				var newEntityToSiteMap = new EntityToSiteMapClass ( companyGroup );
				entityToSiteMaps.Add ( security, newEntityToSiteMap, GetType().GUID );
			}

			var companyMaps = new CompanyMapsClass ( );
			companyMaps.ModifyCollection ( security, companyGroup.IdentityGuid, companyGroup.ID, companyGroup.AssignedCompanyCollection, oldCompanyGroup.AssignedCompanyCollection );

			var productMaps = new ProductMapsClass ( );
			productMaps.ModifyCollection(security, companyGroup.IdentityGuid, companyGroup.ID, false, companyGroup.AuthorizedProductCollection, oldCompanyGroup.AuthorizedProductCollection);

		}

		[OperationBehavior( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public void Purge( SecurityClass security, Guid identityGuid )
		{
			if (security == null)
			{
				throw new ArgumentNullException ( "security" );
			}

			if (!security.HasRight(RIGHT.MODIFY_COMPANY_DATA) && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA)
			    && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			CompanyGroupClass companyGroup = Get(security, identityGuid);
			if (companyGroup.IdentityGuid == Guid.Empty)
			{
				throw ( new Exception ( "CompanyGroup Not Found" ) );
			}

			var companyMaps = new CompanyMapsClass ( );
			companyMaps.ModifyCollection(security, companyGroup.IdentityGuid, companyGroup.ID, null, companyGroup.AssignedCompanyCollection);

			var productMaps = new ProductMapsClass ( );
			productMaps.ModifyCollection(security, companyGroup.IdentityGuid, companyGroup.ID, false, null, companyGroup.AuthorizedProductCollection);

			// Purge from EntityToSiteMap
			var entityToSiteMaps = new EntityToSiteMaps ( );

			EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(security, companyGroup.EntityType, identityGuid);
			
			foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
			{
				entityToSiteMap.ID = companyGroup.ID;
				entityToSiteMaps.Purge ( security, entityToSiteMap );
			}

			using (var cmd = new SqlCommand())
			{
				companyGroup.PurgeSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		public CompanyGroupClass Get ( SecurityClass security, Guid identityGuid )
		{
			if (security == null)
			{
				throw new ArgumentNullException ( "security" );
			}

			if (!security.HasRight(RIGHT.VIEW_COMPANY_DATA) && !security.HasRight(RIGHT.MODIFY_COMPANY_DATA)
			    && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			var companyGroup = new CompanyGroupClass ( );
			companyGroup.IdentityGuid = identityGuid;

			using (var cmd = new SqlCommand())
			{
				companyGroup.SelectSQL(cmd, ContextUtil.IsInTransaction);
				companyGroup.Load(ConsolidatedDA.GetDataSet(cmd, security));
			}

			var companyMaps = new CompanyMapsClass ( );
			companyGroup.AssignedCompanyCollection = companyMaps.EnumerateByAssignedToGuidAndType(security, companyGroup.IdentityGuid, COMPANY_MAP_TYPE.COMPANY_GROUP_COMPANY_MAP);

			var productMaps = new ProductMapsClass ( );
			companyGroup.AuthorizedProductCollection = productMaps.EnumerateByAssignedToGuidAndType(security, companyGroup.IdentityGuid, PRODUCT_MAP_TYPE.PRODUCT_COMPANY_GROUP_MAP);

			return companyGroup;
		}

		public CompanyGroupClass GetByProductIdentityGuid ( SecurityClass security, Guid productGuid )
		{
			if (security == null)
			{
				throw new ArgumentNullException ( "security" );
			}

			if (!security.HasRight(RIGHT.VIEW_COMPANY_DATA) && !security.HasRight(RIGHT.MODIFY_COMPANY_DATA)
			    && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			var productMaps = new ProductMapsClass ( );
			ProductMapCollectionClass productMapCollection = productMaps.EnumerateByAssignedGuidAndType(security, productGuid, PRODUCT_MAP_TYPE.PRODUCT_COMPANY_GROUP_MAP);

			// Never should be greater than 1, see Add and Modify
			if (productMapCollection.Count == 0)
			{
				return null;
			}

			CompanyGroupClass companyGroup = Get ( security, productMapCollection[0].IdentityGuid );
			return companyGroup;
		}

		public Guid GetIdentityGuid ( SecurityClass security, string id )
		{
			if (security == null)
			{
				throw new ArgumentNullException ( "security" );
			}

			if (!security.HasRight(RIGHT.VIEW_COMPANY_DATA) && !security.HasRight(RIGHT.MODIFY_COMPANY_DATA)
			    && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			var companyGroup = new CompanyGroupClass { ID = id, SiteGuid = security.SiteGuid };

			using (var cmd = new SqlCommand())
			{
				companyGroup.SelectByIDSQL(cmd, security, ContextUtil.IsInTransaction);
				companyGroup.Load(ConsolidatedDA.GetDataSet(cmd, security));
			}

			return companyGroup.IdentityGuid;
		}

		public CompanyGroupCollectionClass Enumerate ( SecurityClass security )
		{
			if (security == null)
			{
				throw new ArgumentNullException ( "security" );
			}

			if (!security.HasRight(RIGHT.VIEW_COMPANY_DATA) && !security.HasRight(RIGHT.MODIFY_COMPANY_DATA)
			    && !security.HasRight(RIGHT.VIEW_PRODUCTS) && !security.HasRight(RIGHT.MODIFY_PRODUCTS)
				 && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) &&
				!security.HasRight(RIGHT.VIEW_AUTO_DISTRIBUTION_CONFIGURATION) &&
				!security.HasRight(RIGHT.MODIFY_AUTO_DISTRIBUTION_CONFIGURATION))
			{
				throw new FMInsufficientRightsException();
			}

			var companyGroup = new CompanyGroupClass ( );
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				companyGroup.EnumerateSQL(cmd, security);
				set = ConsolidatedDA.GetDataSet(cmd, security);
			}

			var companyGroupCollection = new CompanyGroupCollectionClass ( );

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				companyGroup = new CompanyGroupClass ( );
				companyGroup.Load ( set );
				companyGroupCollection.Add ( companyGroup );
				table.Rows.RemoveAt ( 0 );
			}

			return companyGroupCollection;
		}

		void IDependency.Insert ( SecurityClass security, BaseDataObject Object, bool preOperation )
		{
			if (security == null)
			{
				throw new ArgumentNullException ( "security" );
			}

			if (Object == null)
			{
				throw new ArgumentNullException ( "Object" );
			}
		}

		void IDependency.Update ( SecurityClass security, BaseDataObject Object )
		{
			if (security == null)
			{
				throw new ArgumentNullException ( "security" );
			}

			if (Object == null)
			{
				throw new ArgumentNullException ( "Object" );
			}

			if (typeof ( SiteClass ).IsInstanceOfType ( Object ))
			{
				var site = (SiteClass) Object;
				CompanyGroupCollectionClass companyGroupCollection = Enumerate ( security );
				var entityToSiteMaps = new EntityToSiteMaps ( );
				foreach (CompanyGroupClass companyGroup in companyGroupCollection)
				{
					if (site.SiteGuid == companyGroup.SiteGuid)
					{
						EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(security, companyGroup.EntityType, companyGroup.IdentityGuid);
						foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
						{
							if (entityToSiteMap.SiteGuid != site.SiteGuid)
							{
								entityToSiteMap.ID = companyGroup.ID;
								entityToSiteMaps.Purge ( security, entityToSiteMap );
							}
						}
					}
				}
			}
		}

		void IDependency.Purge ( SecurityClass security, BaseDataObject Object )
		{
			if (security == null)
			{
				throw new ArgumentNullException ( "security" );
			}

			if (Object == null)
			{
				throw new ArgumentNullException ( "Object" );
			}

			// Purge CompanyGroups
			if (typeof ( SiteClass ).IsInstanceOfType ( Object ))
			{
				var site = (SiteClass) Object;
				CompanyGroupCollectionClass companyGroupCollection = Enumerate ( security );
				var entityToSiteMaps = new EntityToSiteMaps ( );
				foreach (CompanyGroupClass companyGroup in companyGroupCollection)
				{
					if (site.SiteGuid == companyGroup.SiteGuid)
					{
						Purge ( security, companyGroup.IdentityGuid );
					}
					else
					{
						var entityToSiteMap = new EntityToSiteMapClass
						                      {
							                      TypeID = companyGroup.EntityType,
							                      SiteGuid = site.SiteGuid,
							                      IdentityGuid = companyGroup.IdentityGuid
						                      };
						entityToSiteMaps.Purge ( security, entityToSiteMap );
					}
				}
			}
		}
	}
}