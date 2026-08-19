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

namespace FMBusinessServices.ServiceClasses
{
	using FMBusinessObjects.Exceptions;

	/// <summary>
	/// Summary description for ProductGroupsClass.
	/// </summary>
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class ProductGroupsClass : IDependency, IProductGroups
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		public ProductGroupsClass()
		{
		}

		private void Validate(ProductGroupClass ProductGroup)
		{
			if (ProductGroup.ID == "")
				throw (new Exception("ID Required"));

			if (ProductGroup.ID == "{None}"
			|| ProductGroup.ID == "{Unassigned}"
			|| ProductGroup.ID == "{All}")
				throw new Exception("ID is reserved key word " + ProductGroup.ID);
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, ProductGroupClass ProductGroup)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (ProductGroup == null)
				throw new ArgumentNullException("ProductGroup");

			if (!security.HasRight(RIGHT.MODIFY_PRODUCTS))
				throw new FMInsufficientRightsException();

			Validate(ProductGroup);

			if (GetIdentityGuid(security, ProductGroup.ID) != Guid.Empty)
				throw (new Exception("Product Group Exists"));

			ProductGroup.SiteGuid = security.SiteGuid;
			ProductGroup.CreatedDate = DateTimeOffset.Now;
			ProductGroup.CreatedBy = security.UserID;
			ProductGroup.UpdatedDate = ProductGroup.CreatedDate;
			ProductGroup.UpdatedBy = security.UserID;
			ProductGroup.Deleted = false;

			using (SqlCommand cmd = new SqlCommand())
			{
				ProductGroup.IdentityGuid = Guid.NewGuid();
				ProductGroup.InsertSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}


			// Create Entity to Site Map
			EntityToSiteMaps EntityToSiteMaps = new EntityToSiteMaps();
			EntityToSiteMapClass EntityToSiteMap = new EntityToSiteMapClass(ProductGroup);
			EntityToSiteMaps.Add(security, EntityToSiteMap, GetType().GUID);

			ProductMapsClass ProductMaps = new ProductMapsClass();
			ProductMaps.ModifyCollection(security, ProductGroup.IdentityGuid, ProductGroup.ID, false, ProductGroup.ProductMapCollection, null);

			ApplicationStringMapsClass ApplicationStringMaps = new ApplicationStringMapsClass();
			ApplicationStringMaps.ModifyCollection(security, ProductGroup.IdentityGuid, ProductGroup.EntryMessageCollection, null);
			ApplicationStringMaps.ModifyCollection(security, ProductGroup.IdentityGuid, ProductGroup.ExitMessageCollection, null);

			return ProductGroup.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, ProductGroupClass productGroup)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (productGroup == null)
				throw new ArgumentNullException("ProductGroup");

			if (!security.HasRight(RIGHT.MODIFY_PRODUCTS))
				throw new FMInsufficientRightsException();

			Validate(productGroup);

			Guid identityGuid = GetIdentityGuid(security, productGroup.ID);
			if (identityGuid != Guid.Empty
				&& identityGuid != productGroup.IdentityGuid)
				throw (new Exception("Product Group Exists"));

			ProductGroupClass oldProductGroup = Get(security, productGroup.IdentityGuid);
			if (oldProductGroup.IdentityGuid == Guid.Empty)
				throw (new Exception("Product Group Not Found"));

         productGroup.UpdatedDate = DateTimeOffset.Now;
			productGroup.UpdatedBy = security.UserID;

			using (SqlCommand cmd = new SqlCommand())
			{
				productGroup.UpdateSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}


			EntityToSiteMaps EntityToSiteMaps = new EntityToSiteMaps();
			EntityToSiteMapCollectionClass EntityToSiteMapCollection = EntityToSiteMaps.EnumerateByTypeIDAndGuid(security, productGroup.EntityType, productGroup.IdentityGuid);

			if (productGroup.SiteGuid != oldProductGroup.SiteGuid)
			{
				// Purge from EntityToSiteMap
				foreach (EntityToSiteMapClass EntityToSiteMap in EntityToSiteMapCollection)
				{
					EntityToSiteMap.ID = productGroup.ID;
					EntityToSiteMaps.Purge(security, EntityToSiteMap);
				}

				// Create Entity to Site Map
				EntityToSiteMapClass NewEntityToSiteMap = new EntityToSiteMapClass(productGroup);
				EntityToSiteMaps.Add(security, NewEntityToSiteMap, GetType().GUID);
			}

			ProductMapsClass ProductMaps = new ProductMapsClass();
			ProductMaps.ModifyCollection(security, productGroup.IdentityGuid, productGroup.ID, false, productGroup.ProductMapCollection, oldProductGroup.ProductMapCollection);

			ApplicationStringMapsClass ApplicationStringMaps = new ApplicationStringMapsClass();
			ApplicationStringMaps.ModifyCollection(security, productGroup.IdentityGuid, productGroup.EntryMessageCollection, oldProductGroup.EntryMessageCollection);
			ApplicationStringMaps.ModifyCollection(security, productGroup.IdentityGuid, productGroup.ExitMessageCollection, oldProductGroup.ExitMessageCollection);

		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid identityGuid)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (!security.HasRight(RIGHT.MODIFY_PRODUCTS) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			ProductGroupClass ProductGroup = Get(security, identityGuid);
			if (ProductGroup.IdentityGuid == Guid.Empty)
				throw (new Exception("Product Group Not Found"));


			ProductMapsClass ProductMaps = new ProductMapsClass();
			ProductMaps.ModifyCollection(security, ProductGroup.IdentityGuid, ProductGroup.ID, false, null, ProductGroup.ProductMapCollection);

			ApplicationStringMapsClass ApplicationStringMaps = new ApplicationStringMapsClass();
			ApplicationStringMaps.ModifyCollection(security, ProductGroup.IdentityGuid, null, ProductGroup.EntryMessageCollection);
			ApplicationStringMaps.ModifyCollection(security, ProductGroup.IdentityGuid, null, ProductGroup.ExitMessageCollection);


			// Purge from EntityToSiteMap
			EntityToSiteMaps EntityToSiteMaps = new EntityToSiteMaps();
			EntityToSiteMapCollectionClass EntityToSiteMapCollection = EntityToSiteMaps.EnumerateByTypeIDAndGuid(security, ProductGroup.EntityType, identityGuid);

			foreach (EntityToSiteMapClass EntityToSiteMap in EntityToSiteMapCollection)
			{
				EntityToSiteMap.ID = ProductGroup.ID;
				EntityToSiteMaps.Purge(security, EntityToSiteMap);
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				ProductGroup.PurgeSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		public ProductGroupClass Get(SecurityClass security, Guid identityGuid)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (!security.HasRight(RIGHT.VIEW_PRODUCTS)
			&& !security.HasRight(RIGHT.MODIFY_PRODUCTS)
			&& !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
			&& !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA)
			&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
			&& !security.HasRight(RIGHT.MODIFY_DISPATCH)
			&& !security.HasRight(RIGHT.VIEW_DISPATCH))
				throw new FMInsufficientRightsException();

			ProductGroupClass ProductGroup = new ProductGroupClass();
			ProductGroup.IdentityGuid = identityGuid;

			using (SqlCommand cmd = new SqlCommand())
			{
				ProductGroup.SelectSQL(cmd, ContextUtil.IsInTransaction);
				ProductGroup.Load(this.ConsolidatedDA.GetDataSet(cmd, security));
			}

			ProductMapsClass ProductMaps = new ProductMapsClass();
			ProductGroup.ProductMapCollection = ProductMaps.EnumerateByAssignedToGuidAndType(security, ProductGroup.IdentityGuid, PRODUCT_MAP_TYPE.PRODUCT_GROUP_MAP);

			ApplicationStringMapsClass ApplicationStringMaps = new ApplicationStringMapsClass();
			ProductGroup.EntryMessageCollection = ApplicationStringMaps.EnumerateByAssignedToGuidAndType(security, ProductGroup.IdentityGuid, STRING_MAP_TYPE.ENTRY_MESSAGE);
			ProductGroup.ExitMessageCollection = ApplicationStringMaps.EnumerateByAssignedToGuidAndType(security, ProductGroup.IdentityGuid, STRING_MAP_TYPE.EXIT_MESSAGE);

			return ProductGroup;
		}

		public Guid GetIdentityGuid(SecurityClass security, string ID)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (!security.HasRight(RIGHT.VIEW_PRODUCTS)
			&& !security.HasRight(RIGHT.MODIFY_PRODUCTS))
				throw new FMInsufficientRightsException();

			ProductGroupClass ProductGroup = new ProductGroupClass();
			ProductGroup.ID = ID;
			ProductGroup.SiteGuid = security.SiteGuid;

			using (SqlCommand cmd = new SqlCommand())
			{
				ProductGroup.SelectByIDSQL(cmd, ContextUtil.IsInTransaction, security);
				ProductGroup.Load(this.ConsolidatedDA.GetDataSet(cmd, security));
			}

			return ProductGroup.IdentityGuid;
		}

		public ProductGroupCollectionClass Enumerate(SecurityClass security)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			// if the user has rights to the transaction then access to the products is assumed
			if (!security.HasRight(RIGHT.VIEW_TRANSACTION_DATA) && !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
			    && !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) && !security.HasRight(RIGHT.PERFORM_CLOSEOUT)
			    && !security.HasRight(RIGHT.VIEW_PRODUCTS) && !security.HasRight(RIGHT.MODIFY_PRODUCTS)
			    && !security.HasRight(RIGHT.MODIFY_DISPATCH) && !security.HasRight(RIGHT.VIEW_DISPATCH)
				 && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) &&
				!security.HasRight(RIGHT.VIEW_AUTO_DISTRIBUTION_CONFIGURATION) &&
				!security.HasRight(RIGHT.MODIFY_AUTO_DISTRIBUTION_CONFIGURATION))
			{
				throw new FMInsufficientRightsException();
			}

			ProductGroupClass ProductGroup = new ProductGroupClass();

			DataSet Set;
			using (SqlCommand cmd = new SqlCommand())
			{
				ProductGroup.EnumerateSQL(cmd, security);
				Set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			ProductGroupCollectionClass ProductGroupCollection = new ProductGroupCollectionClass();

			DataTable Table = Set.Tables[0];
			while (Table.Rows.Count != 0)
			{
				ProductGroup = new ProductGroupClass();
				ProductGroup.Load(Set);
				ProductGroupCollection.Add(ProductGroup);
				Table.Rows.RemoveAt(0);
			}

			return ProductGroupCollection;
		}

		void IDependency.Insert(SecurityClass security, BaseDataObject Object, bool preOperation)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (Object == null)
				throw new ArgumentNullException("Object");
		}

		void IDependency.Update(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (Object == null)
				throw new ArgumentNullException("Object");

			if (typeof(SiteClass).IsInstanceOfType(Object))
			{
				SiteClass Site = (SiteClass)Object;
				ProductGroupCollectionClass ProductGroupCollection = Enumerate(security);
				EntityToSiteMaps EntityToSiteMaps = new EntityToSiteMaps();
				foreach (ProductGroupClass ProductGroup in ProductGroupCollection)
				{
					if (Site.SiteGuid == ProductGroup.SiteGuid)
					{
						EntityToSiteMapCollectionClass EntityToSiteMapCollection = EntityToSiteMaps.EnumerateByTypeIDAndGuid(security, ProductGroup.EntityType, ProductGroup.IdentityGuid);
						foreach (EntityToSiteMapClass EntityToSiteMap in EntityToSiteMapCollection)
						{
							if (EntityToSiteMap.SiteGuid != Site.SiteGuid)
							{
								EntityToSiteMap.ID = ProductGroup.ID;
								EntityToSiteMaps.Purge(security, EntityToSiteMap);
							}
						}
					}
				}
			}
		}

		void IDependency.Purge(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (Object == null)
				throw new ArgumentNullException("Object");

			// Purge ProductGroups
			if (typeof(SiteClass).IsInstanceOfType(Object))
			{
				SiteClass Site = (SiteClass)Object;
				ProductGroupCollectionClass ProductGroupCollection = Enumerate(security);
				EntityToSiteMaps EntityToSiteMaps = new EntityToSiteMaps();
				foreach (ProductGroupClass ProductGroup in ProductGroupCollection)
				{
					if (Site.SiteGuid == ProductGroup.SiteGuid)
						Purge(security, ProductGroup.IdentityGuid);
					else
					{
						EntityToSiteMapClass EntityToSiteMap = new EntityToSiteMapClass(ProductGroup);
						EntityToSiteMap.SiteGuid = Site.SiteGuid;
						EntityToSiteMaps.Purge(security, EntityToSiteMap);
					}
				}
			}

			else if (typeof(ApplicationStringClass).IsInstanceOfType(Object))
			{
				ApplicationStringClass ApplicationString = (ApplicationStringClass)Object;

				ApplicationStringMapCollectionClass MessageCollection = new ApplicationStringMapCollectionClass();
				ApplicationStringMapsClass ApplicationStringMaps = new ApplicationStringMapsClass();

				if (ApplicationString.Type == STRING_TYPE.ENTRY_MESSAGE)
					MessageCollection = ApplicationStringMaps.EnumerateByApplicationStringGuidAndType(security, ApplicationString.IdentityGuid, STRING_MAP_TYPE.ENTRY_MESSAGE);
				else if (ApplicationString.Type == STRING_TYPE.EXIT_MESSAGE)
					MessageCollection = ApplicationStringMaps.EnumerateByApplicationStringGuidAndType(security, ApplicationString.IdentityGuid, STRING_MAP_TYPE.EXIT_MESSAGE);

				foreach (ApplicationStringMapClass Message in MessageCollection)
					ApplicationStringMaps.Purge(security,
						Message.IdentityGuid,
						Message.Type);

			}
		}
	}
}
