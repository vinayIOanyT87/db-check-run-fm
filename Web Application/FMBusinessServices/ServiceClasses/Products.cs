// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Products.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	Defines the ProductsDictionaryClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	using DataAccessLayer;
	using InternalClasses;
	using System.Diagnostics;


	/// <summary>
	/// Summary description for ProductsClass.
	/// </summary>
	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class ProductsClass : FMServiceBase, IDependency, IProducts
	{
		/// <summary>
		/// The consolidated data access.
		/// </summary>
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		/// <summary>
		/// This method validate the Authorized customer group, assigned companies, authorized
		/// customers for a given product.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="product"></param>
		private void Validate(SecurityClass security, ProductClass product)
		{
				if (product.ID == string.Empty)
				{
					throw (new Exception("ID Required"));
				}

				if (product.ID == "{None}" || product.ID == "{Unassigned}" || product.ID == "{All}")
				{
					throw new Exception("ID is reserved key word " + product.ID);
				}

				CompanyMapsClass companyMaps = new CompanyMapsClass();

				if (product.AuthorizedCustomerGroupCollection != null)
				{
					foreach (ProductMapClass currentGroup in product.AuthorizedCustomerGroupCollection)
					{
						CompanyMapCollectionClass currentGroupAssignedCompanies = companyMaps.EnumerateByAssignedToGuidAndType(security, currentGroup.AssignedToGuid, COMPANY_MAP_TYPE.COMPANY_GROUP_COMPANY_MAP);

						if (currentGroupAssignedCompanies != null)
						{
								foreach (CompanyMapClass currentGroupAssignedCompany in currentGroupAssignedCompanies)
								{
									if (product.AuthorizedCustomerCollection != null)
									{
										// Ensure that the Product isn't assigned to a Company and a CompanyGroup to which a Company is assigned
										foreach (ProductMapClass authorizedCompany in product.AuthorizedCustomerCollection)
										{
												if (authorizedCompany.AssignedToGuid == currentGroupAssignedCompany.AssignedGuid)
												{
													throw new Exception("[Assigned to Company] " + authorizedCompany.AssignedToID + " [and Company Group] " + currentGroup.AssignedToID);
												}
										}
									}
								}
						}

						// Ensure that the Product isn't assigned to two Company Groups with the same company
						// Each Group must be checked against each other
						if (product.AuthorizedCustomerGroupCollection != null)
						{
								foreach (ProductMapClass group in product.AuthorizedCustomerGroupCollection)
								{
									if (currentGroup.AssignedToGuid == group.AssignedToGuid)
									{
										continue;
									}

									CompanyMapCollectionClass groupAssignedCompanies = companyMaps.EnumerateByAssignedToGuidAndType(security, group.AssignedToGuid, COMPANY_MAP_TYPE.COMPANY_GROUP_COMPANY_MAP);

									if (currentGroupAssignedCompanies != null)
									{
										foreach (CompanyMapClass currentGroupAssignedCompany in currentGroupAssignedCompanies)
										{
												if (groupAssignedCompanies != null)
												{
													foreach (CompanyMapClass groupAssignedCompany in groupAssignedCompanies)
													{
														if (currentGroupAssignedCompany.AssignedGuid == groupAssignedCompany.AssignedGuid)
														{
																throw new Exception("[Company] " + currentGroupAssignedCompany.AssignedID + " [Assigned to Company Group] " + currentGroupAssignedCompany.AssignedToID + " [and] " + groupAssignedCompany.AssignedToID);
														}
													}
												}
										}
									}
								}
						}
					}
				}

				//Ensure Total Blend Percentage does not exceed 100%
				double blendSum = 0.0;

				if (product.ProductType == ProductType.BlendProduct)
				{
					//call all Components
					foreach (var productMap in product.ComponentCollection)
					{
						blendSum += productMap.BlendPercentage;
					}
					//if that number is greater than 100, throw error.*/
					if (blendSum > 100.0)
					{
						throw new ApplicationException("Blend composition is over 100 Percent");
					}
				}

			this.ValidateUserData(security, product);
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, ProductClass product)
		{
				if (security == null)
					throw new ArgumentNullException(nameof(security));

				if (product == null)
					throw new ArgumentNullException(nameof(product));

				if (!security.HasRight(RIGHT.MODIFY_PRODUCTS) &&
					!security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT) &&
					!security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
					throw new FMInsufficientRightsException();

				this.Validate(security, product);

				if (this.GetIdentityGuid(security, product.ID) != Guid.Empty)
					throw (new Exception("Product Exists"));

			// Set UserData(list type) to defaults if they are blanks
			UserDataFieldsClass.SetDefaults(security, product.UserData, ENTITY_TYPE.PRODUCT);

				product.SiteGuid = security.SiteGuid;
				product.CreatedDate = DateTimeOffset.Now;
				product.CreatedBy = security.UserID;
				product.UpdatedDate = product.CreatedDate;
				product.UpdatedBy = security.UserID;
				product.IdentityGuid = Guid.NewGuid();

				using (SqlCommand cmd = new SqlCommand())
				{
					product.InsertSQL(cmd);
					this.ConsolidatedDA.ExecuteQuery(security, cmd);
				}

				// Create Entity to Site Map
				EntityToSiteMaps entityToSiteMaps = new EntityToSiteMaps();
				EntityToSiteMapClass entityToSiteMap = new EntityToSiteMapClass(product);
				entityToSiteMaps.Add(security, entityToSiteMap, this.GetType().GUID);

				ApplicationStringMapsClass applicationStringMaps = new ApplicationStringMapsClass();
				applicationStringMaps.ModifyCollection(security, product.IdentityGuid, product.ProductMessageCollection, null);
				applicationStringMaps.ModifyCollection(security, product.IdentityGuid, product.HazardousMaterialMessageCollection, null);

				ProductMapsClass productMaps = new ProductMapsClass();
				productMaps.ModifyCollection(security, product.IdentityGuid, product.ID, true, product.AuthorizedCustomerCollection, null);
				productMaps.ModifyCollection(security, product.IdentityGuid, product.ID, true, product.AuthorizedCustomerGroupCollection, null);
				productMaps.ModifyCollection(security, product.IdentityGuid, product.ID, false, product.ComponentCollection, null);

				return product.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, ProductClass product)
		{
				if (security == null)
					throw new ArgumentNullException(nameof(security));

				if (product == null)
					throw new ArgumentNullException(nameof(product));

			if (!security.HasRight(RIGHT.MODIFY_PRODUCTS) 
				&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
					&& !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

				this.Validate(security, product);

				ProductClass oldProduct = Get(security, product.IdentityGuid);

				// Verify ID does not exist
				Guid identityGuid = this.GetIdentityGuid(security, product.ID);
				if (identityGuid != Guid.Empty
				&& identityGuid != product.IdentityGuid)
					throw (new Exception("Product Exists"));

				if (oldProduct.IdentityGuid == Guid.Empty)
					throw (new Exception("Product Not Found"));

				product.UpdatedDate = DateTimeOffset.Now;
				product.UpdatedBy = security.UserID;

				EntityToSiteMaps entityToSiteMaps = new EntityToSiteMaps();
				if (product.SiteGuid != oldProduct.SiteGuid)
					entityToSiteMaps.PurgeAll(security, ENTITY_TYPE.PRODUCT, product.MasterRecordGuid);

				using (SqlCommand cmd = new SqlCommand())
				{
					product.UpdateSQL(cmd);
					this.ConsolidatedDA.ExecuteQuery(security, cmd);
				}


				if (product.SiteGuid != oldProduct.SiteGuid)
				{
					// Create Entity to Site Map
					EntityToSiteMapClass newEntityToSiteMap = new EntityToSiteMapClass(product);
					Guid currentSiteContext = security.SiteGuid;
					//When changing ownership of an entity that supports Cascading Assignment, need to make sure that the base mapping is created with the AssignedFromSiteGuid being the same as the Owner Site Guid (and the AssignedToSiteGuid), and not be set with the Site Context Guid which in the case of a Change of Ownership would be different from the new Owner Site Guid.
					//The securityParam SiteGuid swap below effectively does so by supplying the EntityToSiteMaps.Add() operation with the correct SiteGuid to use to set the AssignedFromSiteGuid.
					security.SiteGuid = product.SiteGuid;
					entityToSiteMaps.Add(security, newEntityToSiteMap, this.GetType().GUID);
					security.SiteGuid = currentSiteContext;
				}

				ApplicationStringMapsClass applicationStringMaps = new ApplicationStringMapsClass();
				applicationStringMaps.ModifyCollection(security, product.IdentityGuid, product.ProductMessageCollection, oldProduct.ProductMessageCollection);
				applicationStringMaps.ModifyCollection(security, product.IdentityGuid, product.HazardousMaterialMessageCollection, oldProduct.HazardousMaterialMessageCollection);

				ProductMapsClass productMaps = new ProductMapsClass();
				productMaps.ModifyCollection(security, product.IdentityGuid, product.ID, true, product.AuthorizedCustomerGroupCollection, oldProduct.AuthorizedCustomerGroupCollection);
				productMaps.ModifyCollection(security, product.IdentityGuid, product.ID, true, product.AuthorizedCustomerCollection, oldProduct.AuthorizedCustomerCollection);
				if (product.IdentityGuid == product.MasterRecordGuid)	//Product component collection is not subject to Record Versioning, and therefore can only be modified on the master records.
					productMaps.ModifyCollection(security, product.IdentityGuid, product.ID, false, product.ComponentCollection, oldProduct.ComponentCollection);

				this.PropagateUpdate(security, product);

			// TODO: Temporary commented out so that QA does not test change queue features.
				// ChangeQueueRecordsClass.ProcessChangeQueueRecords(security, ChangeQueueEventType.Modify, Product);
		}



		/// <summary>
		/// Propagates the latest updates made to a Product record to its child record versions.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="product">The product whose changes are to be propagated.</param>
		/// <exception cref="System.ArgumentNullException">security</exception>
		public void PropagateUpdate(SecurityClass security, ProductClass product)
		{
				if (security == null)
				{
					throw new ArgumentNullException(nameof(security));
				}

				using (var cmd = new SqlCommand())
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "erv.usp_PropagateProductRevisionByEntityRecordChange";
					cmd.Parameters.Add("@SourceProductGuid", SqlDbType.UniqueIdentifier);
					cmd.Parameters["@SourceProductGuid"].Value = product.IdentityGuid;
					this.ConsolidatedDA.ExecuteQuery(security, cmd);

					// Next, enqueue a replication of global changes up to a master record version.
					// if the change was made to a child record.
					if (product.IdentityGuid != product.MasterRecordGuid)
					{
						cmd.CommandText = "erv.usp_AddGlobalSpecificQueueRecord";
						cmd.Parameters.Clear();
						cmd.Parameters.Add("@EntityTypeId", SqlDbType.NVarChar, 100);
						cmd.Parameters["@EntityTypeId"].Value = ProductClass.ENTITY_TYPE_ID;
						cmd.Parameters.Add("@EntityGuid", SqlDbType.UniqueIdentifier);
						cmd.Parameters["@EntityGuid"].Value = product.IdentityGuid;
						cmd.Parameters.Add("@UserId", SqlDbType.NVarChar, 100);
						cmd.Parameters["@UserId"].Value = security.UserID;
						this.ConsolidatedDA.ExecuteQuery(security, cmd);
					}
				}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid productGuid)
		{
				if (security == null)
					throw new ArgumentNullException(nameof(security));

				if (!security.HasRight(RIGHT.MODIFY_PRODUCTS) &&
					!security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
					throw new FMInsufficientRightsException();

				ProductClass product = this.Get(security, productGuid);
				if (product.IdentityGuid == Guid.Empty)
					throw (new Exception("Product Not Found"));

				if (product.IdentityGuid != product.MasterRecordGuid)
					throw (new Exception("Cannot delete a Product child record version directly"));

				ProductMapsClass productMaps = new ProductMapsClass();
				productMaps.ModifyCollection(security, product.IdentityGuid, product.ID, true, null, product.AuthorizedCustomerCollection);
				productMaps.ModifyCollection(security, product.IdentityGuid, product.ID, false, null, product.ComponentCollection);

				// Purge from EntityToSiteMap
				var entityToSiteMaps = new EntityToSiteMaps();
				entityToSiteMaps.PurgeAll(security, ENTITY_TYPE.PRODUCT, product.MasterRecordGuid);

				ApplicationStringMapsClass applicationStringMaps = new ApplicationStringMapsClass();
				applicationStringMaps.ModifyCollection(security, product.IdentityGuid, null, product.ProductMessageCollection);
				applicationStringMaps.ModifyCollection(security, product.IdentityGuid, null, product.HazardousMaterialMessageCollection);

				DependenciesClass dependencies = new DependenciesClass(security);
				dependencies.Purge(security, product);

				using (SqlCommand cmd = new SqlCommand())
				{
					product.PurgeSQL(cmd);
					this.ConsolidatedDA.ExecuteQuery(security, cmd);
				}
		}


		public ProductClass GetBasicInfo(SecurityClass security, Guid productGuid, Guid siteGuid)
		{
				if (security == null)
				{
					throw new ArgumentNullException(nameof(security));
				}

				DataSet set;
				using (SqlCommand cmd = new SqlCommand())
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "dbo.usp_GetProductByGuid";
					cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
					cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);
					cmd.Parameters["@TargetSiteGuid"].Value = siteGuid;
					cmd.Parameters["@ProductGuid"].Value = productGuid;
					set = this.ConsolidatedDA.GetDataSet(cmd, security);
				}

				if (set.Tables[0].Rows.Count == 0)
				{
					return null;
				}

				DataRow row = set.Tables[0].Rows[0];
				ProductClass product = new ProductClass
											{
													IdentityGuid = DataObject.getValue(row["ProductGuid"], Guid.Empty),
													MasterRecordGuid =
														DataObject.getValue(row["_MasterRecordGuid"], Guid.Empty),
													ID = DataObject.getValue(row["ProductId"], string.Empty),
													SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty)
											};

				return product;
		}


		public ProductClass Get(SecurityClass security, Guid productGuid, bool hideHiddenProducts = false, bool LoadProcessVariables = true)
		{
			return this.GetByProductAuthorizedCompanies(security, productGuid, true, hideHiddenProducts, LoadProcessVariables);
		}

		public ProductClass GetMinimalProductData(SecurityClass security, Guid productGuid)
		{
			return this.GetByInfoAuthorizedCompanies(security, productGuid, true, false, false, false);
		}

		public ProductClass GetByProductAuthorizedCompanies(SecurityClass security, Guid productGuid, bool getAuthorizedCompanies, bool hideHiddenProducts = false, bool LoadProcessVariables = true)
		{
				return this.GetByInfoAuthorizedCompanies(security, productGuid, false, getAuthorizedCompanies, hideHiddenProducts, LoadProcessVariables);
		}

		/// <summary>
		/// The get by info authorized companies.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="productGuid">
		/// The product GUID.
		/// </param>
		/// <param name="getMinimalInfo">
		/// The get minimal info.
		/// </param>
		/// <param name="getAuthorizedCompanies">
		/// The get authorized companies.
		/// </param>
		/// <param name="hideHiddenProducts">If true, only products that are not hidden will be returned as part of the component collection</param>
		/// <returns>
		/// The <see cref="ProductClass"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Null argument exception.
		/// </exception>
		/// <exception cref="Exception">
		/// Access denied.
		/// </exception>
		public ProductClass GetByInfoAuthorizedCompanies(SecurityClass security, Guid productGuid, bool getMinimalInfo, bool getAuthorizedCompanies, bool hideHiddenProducts = false, bool LoadProcessVariables = true)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

				if (!security.HasRight(RIGHT.VIEW_PRODUCTS) 
				&& !security.HasRight(RIGHT.MODIFY_PRODUCTS)
					&& !security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
					&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
					&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT) 
				&& !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
					&& !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA) 
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
					&& !security.HasRight(RIGHT.PERFORM_CLOSEOUT) 
				&& !security.HasRight(RIGHT.MODIFY_FINANCIAL_DATA)
					&& !security.HasRight(RIGHT.MODIFY_STANDING_OFFERS) 
				&& !security.HasRight(RIGHT.MODIFY_DISPATCH)
					&& !security.HasRight(RIGHT.VIEW_DISPATCH) 
				&& !security.HasRight(RIGHT.MODIFY_INCOMING_TRUCK_DATA)
					&& !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA)
				&& !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
					&& !security.HasRight(RIGHT.VIEW_ORDERS)
					&& !security.HasRight(RIGHT.MODIFY_ORDERS)
					&& !security.HasRight(RIGHT.CREATE_ORDERS)
					&& !security.HasRight(RIGHT.VIEW_SUPPLY_ORDERS)
					&& !security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS)
					&& !security.HasRight(RIGHT.CREATE_SUPPLY_ORDERS)
					&& !security.HasRight(RIGHT.VIEW_TANK_DATA)	// Tanks and Tank Groups enumerate products, so allow users with those rights to call this method
				&& !security.HasRight(RIGHT.MODIFY_TANK_DATA))
				{
					throw new FMInsufficientRightsException();
				}

				var sites = new SitesClass();
				SiteClass site = sites.Get(security, security.LoginSiteGuid, false, false, false);

				var product = new ProductClass(site);
				DataSet set;

				using (var cmd = new SqlCommand())
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "dbo.usp_GetProductByGuid";

					cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
					cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);

					cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
					cmd.Parameters["@ProductGuid"].Value = productGuid;
					set = this.ConsolidatedDA.GetDataSet(cmd, security);
				}

				product.Load(set);

				if (getMinimalInfo == false)
				{
					var productMaps = new ProductMapsClass();

				// Use MasterRecordGuid to retrieve Blend component mappings. Blend component mappings are not covered by Record Versioning.
					if (product.ProductType == ProductType.BlendProduct) 
					{
						product.ComponentCollection = productMaps.EnumerateByAssignedToGuidAndType(security, product.MasterRecordGuid, PRODUCT_MAP_TYPE.BLEND_COMPONENT_MAP, hideHiddenProducts);
					}

					if (getAuthorizedCompanies)
					{
						product.AuthorizedCustomerCollection = productMaps.EnumerateByAssignedGuidAndTypeAndInstr(security, product.IdentityGuid, PRODUCT_MAP_TYPE.PRODUCT_COMPANY_MAP, LoadProcessVariables);
						product.AuthorizedCustomerGroupCollection = productMaps.EnumerateByAssignedGuidAndTypeAndInstr(security, product.IdentityGuid, PRODUCT_MAP_TYPE.PRODUCT_COMPANY_GROUP_MAP, LoadProcessVariables);
						product.AuthorizedSupplierCollection = productMaps.EnumerateByAssignedGuidAndTypeAndInstr(security, product.IdentityGuid, PRODUCT_MAP_TYPE.SUPPLIER_PRODUCT_COMPANY_MAP, LoadProcessVariables);
					}

					var applicationStringMaps = new ApplicationStringMapsClass();
					product.ProductMessageCollection = applicationStringMaps.EnumerateByAssignedToGuidAndType(security, product.IdentityGuid, STRING_MAP_TYPE.PRODUCT_MESSAGE);
					product.HazardousMaterialMessageCollection = applicationStringMaps.EnumerateByAssignedToGuidAndType(security, product.IdentityGuid, STRING_MAP_TYPE.DOT_HAZARDOUS_MESSAGE);
				}

				return product;
		}

		public Guid GetIdentityGuid(SecurityClass security, string id)
		{
				ProductClass product = this.GetByID(security, id);
				return product.IdentityGuid;
		}

		public Guid GetMasterRecordGuidFromID(SecurityClass security, string id)
		{
				Guid result = Guid.Empty;
				ProductClass product = this.GetByID(security, id);
				if (product != null)
					result = product.MasterRecordGuid;
				return result;
		}

		public Guid GetMasterRecordGuid(SecurityClass security, Guid productGuid)
		{
				Guid result = Guid.Empty;
				ProductClass product = this.GetByInfoAuthorizedCompanies(security, productGuid, true, false, false,true);
				if (product != null)
					result = product.MasterRecordGuid;
				return result;
		}

		/// <summary>
		/// The get by ID.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="id">
		/// The ID.
		/// </param>
		/// <returns>
		/// The <see cref="ProductClass"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Null argument exception.
		/// </exception>
		/// <exception cref="Exception">
		/// Access denied.
		/// </exception>
		public ProductClass GetByID(SecurityClass security, string id)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

				if (!security.HasRight(RIGHT.VIEW_PRODUCTS) 
				&& !security.HasRight(RIGHT.MODIFY_PRODUCTS)
					&& !security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
					&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
					&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT) 
				&& !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA)
					&& !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING) 
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
					&& !security.HasRight(RIGHT.PERFORM_CLOSEOUT) 
				&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
					&& !security.HasRight(RIGHT.MODIFY_FINANCIAL_DATA) 
				&& !security.HasRight(RIGHT.MODIFY_STANDING_OFFERS)
					&& !security.HasRight(RIGHT.MODIFY_DISPATCH) 
				&& !security.HasRight(RIGHT.VIEW_DISPATCH)
					&& !security.HasRight(RIGHT.VIEW_ORDERS)
					&& !security.HasRight(RIGHT.MODIFY_ORDERS)
					&& !security.HasRight(RIGHT.CREATE_ORDERS)
					&& !security.HasRight(RIGHT.VIEW_SUPPLY_ORDERS)
					&& !security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS)
					&& !security.HasRight(RIGHT.CREATE_SUPPLY_ORDERS)
					&& !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA)
				&& !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA) &&
				!security.HasRight(RIGHT.VIEW_AUTO_DISTRIBUTION_CONFIGURATION) &&
				!security.HasRight(RIGHT.MODIFY_AUTO_DISTRIBUTION_CONFIGURATION))
				{
					throw new FMInsufficientRightsException();
				}

			if (string.IsNullOrEmpty(id))
			{
				return null;
			}

				var sites = new SitesClass();
				SiteClass site = sites.Get(security, security.LoginSiteGuid, false, false, false);

				var product = new ProductClass(site);

				if (id == "{Unassigned}" || id == "{None}" || id == "{All}")
				{
					return product;
				}

				DataSet set;

				using (var cmd = new SqlCommand())
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "dbo.usp_GetProductsById";
					cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
					cmd.Parameters.Add("@Id", SqlDbType.NVarChar, 30);

					cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
					cmd.Parameters["@ID"].Value = id;
					set = this.ConsolidatedDA.GetDataSet(cmd, security);
				}

				product.Load(set);
				return product;
		}

		/// <summary>
		/// The get by code.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="code">
		/// The code.
		/// </param>
		/// <returns>
		/// The <see cref="ProductClass"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Null argument exception.
		/// </exception>
		/// <exception cref="Exception">
		/// Access denied.
		/// </exception>
		public ProductClass GetByCode(SecurityClass security, string code)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_PRODUCTS)
				&& !security.HasRight(RIGHT.MODIFY_PRODUCTS)
				&& !security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
				&& !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA)
				&& !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
				&& !security.HasRight(RIGHT.PERFORM_CLOSEOUT)
				&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
				&& !security.HasRight(RIGHT.MODIFY_FINANCIAL_DATA)
				&& !security.HasRight(RIGHT.MODIFY_STANDING_OFFERS)
				&& !security.HasRight(RIGHT.MODIFY_DISPATCH)
				&& !security.HasRight(RIGHT.VIEW_DISPATCH)
				&& !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA)
				&& !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			var sites = new SitesClass();
			SiteClass site = sites.Get(security, security.LoginSiteGuid, false, false, false);

			var product = new ProductClass(site);

			if (string.IsNullOrEmpty(code))
			{
				return product;
			}

			DataSet set;

			using (var cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetProductsByCode";
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@ProductCode", SqlDbType.NVarChar, 15);
				cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				cmd.Parameters["@ProductCode"].Value = code;
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			product.Load(set);
			return product;
		}

		/// <summary>
		/// The get ID and GUID.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="productGuid">
		/// The product GUID.
		/// </param>
		/// <param name="site">
		/// The site.
		/// </param>
		/// <returns>
		/// The <see cref="ProductClass"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Invalid arguments.
		/// </exception>
		/// <exception cref="Exception">
		/// Access denied exception.
		/// </exception>
		public ProductClass GetIdAndGuid(SecurityClass security, Guid productGuid, SiteClass site)
		{
			if ( security == null )
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_PRODUCTS) &&
				!security.HasRight(RIGHT.MODIFY_PRODUCTS) &&
				!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) &&
				!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) &&
				!security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT) &&
				!security.HasRight(RIGHT.VIEW_BILLS_OF_LADING) &&
				!security.HasRight(RIGHT.VIEW_TRANSACTION_DATA) &&
				!security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) &&
				!security.HasRight(RIGHT.PERFORM_CLOSEOUT) &&
				!security.HasRight(RIGHT.MODIFY_FINANCIAL_DATA) &&
				!security.HasRight(RIGHT.MODIFY_STANDING_OFFERS) &&
				!security.HasRight(RIGHT.MODIFY_DISPATCH) &&
				!security.HasRight(RIGHT.VIEW_DISPATCH) &&
				!security.HasRight(RIGHT.MODIFY_INCOMING_TRUCK_DATA) &&
				!security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA) &&
				!security.HasRight(RIGHT.VIEW_TANK_DATA) &&
				!security.HasRight(RIGHT.MODIFY_TANK_DATA) &&
				!security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA))
			{
				throw new FMInsufficientRightsException();
			}

				ProductClass product = null;
				DataSet dataSet;
				using (var cmd = new SqlCommand())
				{
					// product.SelectIdAndGuidSql(sqlCommand, ContextUtil.IsInTransaction);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "dbo.usp_GetProductIdAndGuidByGuid";
					cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
					cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);

					cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
					cmd.Parameters["@ProductGuid"].Value = productGuid;
					dataSet = this.ConsolidatedDA.GetDataSet(cmd, security);
				}

				if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
				{
					DataRow row = dataSet.Tables[0].Rows[0];
					product = new ProductClass
									{
										ID				= row.IsNull("ProductID") ? string.Empty : (string)row["ProductID"],
										IdentityGuid		= DataObject.getValue<Guid>(row["ProductGuid"], Guid.Empty),
										MasterRecordGuid	= DataObject.getValue<Guid>(row["_MasterRecordGuid"], Guid.Empty),
										SiteGuid			= DataObject.getValue<Guid>(row["SiteGuid"], Guid.Empty)
									};
					product.ID = DataObject.getValue<string>(row["ProductID"], string.Empty);
				}

			return product;
		}

		/// <summary>
		/// The enumerate by type.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="productType">
		/// The type.
		/// </param>
		/// <param name="hideHiddenProducts">If true, only products that are not marked as hidden will be returned</param>
		/// <param name="limit">decides how many records to return</param>
		/// <returns>
		/// The <see cref="ProductCollectionClass"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Null argument exception.
		/// </exception>
		/// <exception cref="Exception">
		/// Access denied exception.
		/// </exception>
		public ProductCollectionClass EnumerateByType(SecurityClass security, ProductType productType, bool hideHiddenProducts = false, int? limit = null)
		{
				if (security == null)
				{
					throw new ArgumentNullException(nameof(security));
				}

				if (!security.HasRight(RIGHT.VIEW_PRODUCTS) 
				&& !security.HasRight(RIGHT.MODIFY_PRODUCTS)
					&& !security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
					&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
					&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT) 
				&& !security.HasRight(RIGHT.VIEW_DISPATCH)
					&& !security.HasRight(RIGHT.MODIFY_DISPATCH)
				&& !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
				&& !security.HasRight(RIGHT.VIEW_TANK_DATA) // Tanks and Tank Groups enumerate products, so allow users with those rights to call this method
				&& !security.HasRight(RIGHT.MODIFY_TANK_DATA))
				{
					throw new FMInsufficientRightsException();
				}

			ProductCollectionClass productCollection = this.EnumerateByTypeAndFilter(security, productType, null, hideHiddenProducts, limit);

				return productCollection;
		}

		/// <summary>
		/// The enumerate by type 1.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="productType">
		/// The type.
		/// </param>
		/// <returns>
		/// The <see cref="DataSet"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Null Argument Exception.
		/// </exception>
		/// <exception cref="Exception">
		/// Access denied exception.
		/// </exception>
		public DataSet EnumerateByType1(SecurityClass security, ProductType productType)
		{

				if (security == null)
				{
					throw new ArgumentNullException(nameof(security));
				}

				if (!security.HasRight(RIGHT.VIEW_PRODUCTS) 
				&& !security.HasRight(RIGHT.MODIFY_PRODUCTS)
					&& !security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
					&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
					&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT) 
				&& !security.HasRight(RIGHT.VIEW_DISPATCH)
					&& !security.HasRight(RIGHT.MODIFY_DISPATCH)
				&& !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
				&& !security.HasRight(RIGHT.EXPORT_ENTERPRISE_DATA))
				{
					throw new FMInsufficientRightsException();
				}

				DataSet set;

				using (var cmd = new SqlCommand())
				{
					// Product.EnumerateByTypeSQL(cmd, security);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "dbo.usp_GetProductsByTypeAndFilter";
					cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
					cmd.Parameters.Add("@ProductType", SqlDbType.Int);
					cmd.Parameters.Add("@SearchFilter", SqlDbType.NVarChar, 100);

					cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
					cmd.Parameters["@ProductType"].Value = DBNull.Value;

				if (productType != ProductType.MaxProduct)
				{
					cmd.Parameters["@ProductType"].Value = (int) productType;
				}

					cmd.Parameters["@SearchFilter"].Value = DBNull.Value;
					set = this.ConsolidatedDA.GetDataSet(cmd, security);
				}

				return set;
		}

		/// <summary>
		/// The enumerate by inhibit accounting.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="productType">Product type.</param>
		/// <param name="inhibitAccounting">
		/// The inhibit accounting.
		/// </param>
		/// <returns>
		/// The <see cref="ProductCollectionClass"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">Null argument exception.
		/// </exception>
		/// <exception cref="Exception">Access denied.
		/// </exception>
		public ProductCollectionClass EnumerateByTypeAndInhibitAccounting(SecurityClass security, ProductType productType, bool inhibitAccounting)
		{
				if (security == null)
				{
					throw new ArgumentNullException(nameof(security));
				}

				if (!security.HasRight(RIGHT.VIEW_PRODUCTS) 
				&& !security.HasRight(RIGHT.MODIFY_PRODUCTS)
					&& !security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
					&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
					&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
					&& !security.HasRight(RIGHT.VIEW_DISPATCH)
					&& !security.HasRight(RIGHT.MODIFY_DISPATCH)
				&& !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA))
				{
					throw new FMInsufficientRightsException();
				}

				var sites = new SitesClass( );
				SiteClass site = sites.Get(security, security.LoginSiteGuid, false);

				var productCollection = new ProductCollectionClass();

			using (var cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetProductsByTypeAndInhibitAccounting";

				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@ProductType", SqlDbType.Int);
				cmd.Parameters.Add("@InhibitAccounting", SqlDbType.Bit);

				cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				cmd.Parameters["@ProductType"].Value = DBNull.Value;
				cmd.Parameters["@InhibitAccounting"].Value = inhibitAccounting;

				if (productType != ProductType.MaxProduct)
				{
					cmd.Parameters["@ProductType"].Value = (int) productType;
				}

				DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);

				DataTable table = set.Tables[0];

				while ( table.Rows.Count != 0 )
				{
					var product = new ProductClass(site);
					product.Load(set);
					productCollection.Add(product);
					table.Rows.RemoveAt(0);
				}
			}

				return productCollection;
		}

		/// <summary>
		/// This method will return a product object collection of the products that meet the security, type,
		/// filter, and by group products criterion. This method is the same as the EnumerateByRole method 
		/// with the exception that the user has supplied a filter to narrow the search on the list of products.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="productType">
		/// The product Type.
		/// </param>
		/// <param name="filter">
		/// The filter.
		/// </param>
		/// <param name="hideHiddenProducts">If true, only products that are not marked as hidden will be returned</param>
		/// <param name="limit">decides how many records to return</param>
		/// <returns>
		/// The <see cref="ProductCollectionClass"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Null argument exception.
		/// </exception>
		/// <exception cref="Exception">
		/// Access denied exception.
		/// </exception>
		public ProductCollectionClass EnumerateByTypeAndFilter(SecurityClass security, ProductType productType, string filter, bool hideHiddenProducts = false, int? limit = null)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

				if (!security.HasRight(RIGHT.VIEW_PRODUCTS) 
				&& !security.HasRight(RIGHT.MODIFY_PRODUCTS)
					&& !security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
					&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
					&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
				&& !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
				&& !security.HasRight(RIGHT.VIEW_TANK_DATA) // Tanks and Tank Groups enumerate products, so allow users with those rights to call this method
				&& !security.HasRight(RIGHT.MODIFY_TANK_DATA))
				{
					throw new FMInsufficientRightsException();
				}

			DataSet set;
				using (var cmd = new SqlCommand())
				{
					// DataSet Set = ConsolidatedDA.GetDataSet(Product.EnumerateByTypeAndFilterSQL(security, filter), security);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "dbo.usp_GetProductsByTypeAndFilter";
					cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
					cmd.Parameters.Add("@ProductType", SqlDbType.Int);
					cmd.Parameters.Add("@SearchFilter", SqlDbType.NVarChar, 100);
					cmd.Parameters.Add("@Limit", SqlDbType.Int);
						
					cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
					cmd.Parameters["@ProductType"].Value = DBNull.Value;

					if (limit.Equals(null))
					{
						cmd.Parameters["@Limit"].Value = DBNull.Value;
					}
					else
					{
						cmd.Parameters["@Limit"].Value = limit;
					}

					if (productType != ProductType.MaxProduct)
					{
						cmd.Parameters["@ProductType"].Value = (int)productType;
					}

					cmd.Parameters["@SearchFilter"].Value = DBNull.Value;

					if (!string.IsNullOrEmpty(filter))
					{
						filter = "%" + filter + "%";
						cmd.Parameters["@SearchFilter"].Value = filter;
					}

					if (hideHiddenProducts)
					{
						cmd.Parameters.Add("@HideHiddenProducts", SqlDbType.Bit).Value = 1;
					}

					set = this.ConsolidatedDA.GetDataSet(cmd, security);
				}

				var sites = new SitesClass();
				SiteClass site = sites.Get(security, security.LoginSiteGuid, false, false, false);

				var productCollection = new ProductCollectionClass();
				DataTable table = set.Tables[0];

				while (table.Rows.Count != 0)
				{
					var product = new ProductClass(site);
					product.Load(set);
					productCollection.Add(product);
					table.Rows.RemoveAt(0);
				}

				return productCollection;
		}

		/// <summary>
		/// This method will return a product object collection of the products that meet the security, tanks/manager/product
		/// relationship.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="managerId">
		/// The manager ID.
		/// </param>
		/// <param name="hideHiddenProducts">
		/// If true, only products that are not hidden will be returned
		/// </param>
		/// <returns>
		/// The <see cref="ProductCollectionClass"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Null argument exception.
		/// </exception>
		/// <exception cref="Exception">
		/// Access denied exception
		/// </exception>
		public ProductCollectionClass EnumerateByManagerAndTanks(SecurityClass security, string managerId, bool hideHiddenProducts = false)
		{
				if (security == null)
				{
					throw new ArgumentNullException(nameof(security));
				}

				if (!security.HasRight(RIGHT.VIEW_PRODUCTS)
				&& !security.HasRight(RIGHT.MODIFY_PRODUCTS)
				&& !security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
				&& !security.HasRight(RIGHT.PERFORM_CLOSEOUT)
				&& !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA))
				{
					throw new FMInsufficientRightsException();
				}

			DataSet dataSet;
				using (var cmd = new SqlCommand())
				{
					// product.EnumerateByManagerAndTanksSQL(cmd, security, managerID);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "dbo.usp_GetProductByManagerAndTanks";
					cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
					cmd.Parameters.Add("@ManagerId", SqlDbType.NVarChar, 100);

					cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
					cmd.Parameters["@ManagerId"].Value = DBNull.Value;

					if (!string.IsNullOrEmpty(managerId))
					{
						cmd.Parameters["@ManagerId"].Value = managerId;
					}

					if (hideHiddenProducts)
					{
						cmd.Parameters.Add("@HideHiddenProducts", SqlDbType.Bit).Value = 1;
					}

					dataSet = this.ConsolidatedDA.GetDataSet(cmd, security);
				}

				var productCollection = new ProductCollectionClass();

				var sites = new SitesClass();
				SiteClass site = sites.Get(security, security.LoginSiteGuid, false, false, false);

				DataTable table = dataSet.Tables[0];

				while (table.Rows.Count != 0)
				{
					var product = new ProductClass(site);
					product.Load(dataSet);
					productCollection.Add(product);
					table.Rows.RemoveAt(0);
				}

				return productCollection;
		}

		/// <summary>
		/// The enumerate by site.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <returns>
		/// The <see cref="ProductCollectionClass"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Null argument exception.
		/// </exception>
		/// <exception cref="Exception">
		/// Access denied.
		/// </exception>
		public ProductCollectionClass EnumerateBySite(SecurityClass security)
		{
				if (security == null)
				{
					throw new ArgumentNullException(nameof(security));
				}

				if (!security.HasRight(RIGHT.VIEW_PRODUCTS)
				&& !security.HasRight(RIGHT.MODIFY_PRODUCTS)
				&& !security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
				&& !security.HasRight(RIGHT.PERFORM_CLOSEOUT)
				&& !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA))
				{
					throw new FMInsufficientRightsException();
				}

			DataSet dataSet;

				using (var cmd = new SqlCommand())
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "dbo.usp_GetProductsBySite";
					cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
					cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
					dataSet = this.ConsolidatedDA.GetDataSet(cmd, security);
				}

				var productCollection = new ProductCollectionClass();

				var sites = new SitesClass();
				SiteClass site = sites.Get(security, security.LoginSiteGuid, false, false, false);

				DataTable table = dataSet.Tables[0];

				while (table.Rows.Count != 0)
				{
					DataRow row = table.Rows[0];
					var product = new ProductClass(site)
											{
												ID = DataObject.getValue<string>(row["ProductID"], string.Empty),
												SiteGuid = DataObject.getValue<Guid>(row["SiteGuid"], Guid.Empty),
												IdentityGuid = DataObject.getValue<Guid>(row["ProductGuid"], Guid.Empty),
												MasterRecordGuid = DataObject.getValue<Guid>(row["_MasterRecordGuid"], Guid.Empty),
												AssignedToSiteGuid = DataObject.getValue<Guid>(row["AssignedToSiteGuid"], Guid.Empty),
												AssignedFromSiteGuid = DataObject.getValue<Guid>(row["AssignedFromSiteGuid"], Guid.Empty),
												AssignedFromSiteId = DataObject.getValue<string>(row["AssignedFromSiteId"], string.Empty)
											};
					productCollection.Add(product);
					table.Rows.RemoveAt(0);
				}

				return productCollection;
		}

		/// <summary>
		/// The enumerate by site.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <returns>
		/// The <see cref="ProductCollectionClass"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Null argument exception.
		/// </exception>
		/// <exception cref="Exception">
		/// Access denied.
		/// </exception>
		public List<string> EnumerateIdBySite(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_PRODUCTS)
				&& !security.HasRight(RIGHT.MODIFY_PRODUCTS)
				&& !security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
				&& !security.HasRight(RIGHT.PERFORM_CLOSEOUT)
				&& !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
				&& !security.HasRight(RIGHT.VIEW_POINTS)
				&& !security.HasRight(RIGHT.ENABLE_POINTS)
				&& !security.HasRight(RIGHT.DISABLE_POINTS))
			{
				throw new FMInsufficientRightsException();
			}

			DataSet dataSet;

			using (var cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetProductsBySite";
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				dataSet = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			var productIdList = new List<string>();


			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var ID = DataObject.getValue<string>(row["ProductID"], string.Empty);
				productIdList.Add(ID);
			}

			return productIdList;
		}



		/// <summary>
		/// The enumerate by filter.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="filter">
		/// The filter.
		/// </param>
		/// <param name="hideHiddenProducts">If true, only products that are not hidden will be returned</param>
		/// <returns>
		/// The <see cref="ProductCollectionClass"/>.
		/// </returns>
		public ProductCollectionClass EnumerateByFilter(SecurityClass security, string filter, bool hideHiddenProducts = false)
		{
				return this.EnumerateByFilterAndLocalize(security, filter, true, hideHiddenProducts);
		}

		/// <summary>
		/// The enumerate.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="hideHiddenProducts"></param>
		/// <returns>
		/// The <see cref="ProductCollectionClass"/>.
		/// </returns>
		public ProductCollectionClass Enumerate(SecurityClass security, bool hideHiddenProducts = false, SiteClass site = null)
		{
				return this.EnumerateByFilterAndLocalize(security, null, true, hideHiddenProducts, site);
		}

		/// <summary>
		/// The enumerate 2.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="targetSiteGuid">
		/// The target site GUID.
		/// </param>
		/// <returns>
		/// The <see cref="ProductCollectionClass"/>.
		/// </returns>
		public ProductCollectionClass Enumerate2(SecurityClass security, Guid targetSiteGuid)
		{
				return this.EnumerateByFilterAndLocalize2(security, targetSiteGuid, null, true);
		}

		/// <summary>
		/// The enumerate by filter and localize.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="filter">
		/// The filter.
		/// </param>
		/// <param name="localize">
		/// The localize.
		/// </param>
		/// <param name="hideHiddenProducts">If true, only products that are not hidden will be returned</param>
		/// <returns>
		/// The <see cref="ProductCollectionClass"/>.
		/// </returns>
		public ProductCollectionClass EnumerateByFilterAndLocalize(SecurityClass security, string filter, bool localize, bool hideHiddenProducts = false, SiteClass site = null)
		{
				return this.EnumerateByFilterAndLocalize2(security, security.SiteGuid, filter, localize, hideHiddenProducts, site);
		}

		/// <summary>
		/// The enumerate by filter and localize 2.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="targetSiteGuid">
		/// The target site GUID.
		/// </param>
		/// <param name="filter">
		/// The filter.
		/// </param>
		/// <param name="localize">
		/// The localize.
		/// </param>
		/// <param name="hideHiddenProducts">If true, only products not marked as hidden will be returned</param>
		/// <returns>
		/// The <see cref="ProductCollectionClass"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Null argument exception.
		/// </exception>
		/// <exception cref="Exception">
		/// Access denied exception.
		/// </exception>
		public ProductCollectionClass EnumerateByFilterAndLocalize2(SecurityClass security, Guid targetSiteGuid, string filter, bool localize, bool hideHiddenProducts = false, SiteClass site = null)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_PRODUCTS)
			&& !security.HasRight(RIGHT.MODIFY_PRODUCTS)
			&& !security.HasRight(RIGHT.VIEW_COMPANY_DATA)
			&& !security.HasRight(RIGHT.MODIFY_COMPANY_DATA)
			&& !security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
			&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
			&& !security.HasRight(RIGHT.VIEW_INVENTORY_DATA)
			&& !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA)
			&& !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
			&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
			&& !security.HasRight(RIGHT.VIEW_TICKETING_DATA)
			&& !security.HasRight(RIGHT.MODIFY_TICKETING_DATA)
			&& !security.HasRight(RIGHT.VIEW_REPORTS)
			&& !security.HasRight(RIGHT.MODIFY_REPORTS)
			&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
			&& !security.HasRight(RIGHT.MODIFY_ALLOCATIONS)
			&& !security.HasRight(RIGHT.VIEW_ALLOCATIONS)
			&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES)
			&& !security.HasRight(RIGHT.VIEW_TRANSACTION_ALIASES)
			&& !security.HasRight(RIGHT.VIEW_ORDERS)
			&& !security.HasRight(RIGHT.MODIFY_ORDERS)
			&& !security.HasRight(RIGHT.CREATE_ORDERS)
			&& !security.HasRight(RIGHT.VIEW_SUPPLY_ORDERS)
			&& !security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS)
			&& !security.HasRight(RIGHT.CREATE_SUPPLY_ORDERS)
			&& !security.HasRight(RIGHT.PERFORM_CLOSEOUT)
			&& !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA)
			&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
			&& !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA) 
			&& !security.HasRight(RIGHT.VIEW_AUTO_DISTRIBUTION_CONFIGURATION) 
			&& !security.HasRight(RIGHT.MODIFY_AUTO_DISTRIBUTION_CONFIGURATION)
			&& !security.HasRight(RIGHT.IMPORT_TRANSACTION))
			{
				throw new FMInsufficientRightsException();
			}

			DataSet set;

			using (var cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetProductsByIdFilter";
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@ProductIDFilter", SqlDbType.NVarChar, 255);

				cmd.Parameters["@TargetSiteGuid"].Value = targetSiteGuid;
				cmd.Parameters["@ProductIDFilter"].Value = DBNull.Value;

				if (!string.IsNullOrEmpty(filter))
				{
					filter = "%" + filter + "%";
				cmd.Parameters["@ProductIDFilter"].Value = filter;
				}

				if (hideHiddenProducts)
				{
					cmd.Parameters.Add("@HideHiddenProducts", SqlDbType.Bit).Value = 1;
				}

				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			var productCollection = new ProductCollectionClass();

			if (site == null)
			{
				var sites = new SitesClass();

				if (localize)
				{
					site = sites.Get(security, security.SiteGuid, false, false, false);
				}
			}

			DataTable table = set.Tables[0];

			while (table.Rows.Count != 0)
			{
				ProductClass product = localize ? new ProductClass(site) : new ProductClass();

				product.Load(set);
				productCollection.Add(product);
				table.Rows.RemoveAt(0);
			}

			return productCollection;
		}

		/// <summary>
		/// This method will enumerate all products at all sites. It only returns minimal data to be
		/// used as refences.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <returns>Returns all products at all sites.</returns>
		public DataSet EnumerateProductsAtAllSites(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_PRODUCTS)
				&& !security.HasRight(RIGHT.MODIFY_PRODUCTS)
				&& !security.HasRight(RIGHT.VIEW_COMPANY_DATA)
				&& !security.HasRight(RIGHT.MODIFY_COMPANY_DATA)
				&& !security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.VIEW_INVENTORY_DATA)
				&& !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA)
				&& !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
				&& !security.HasRight(RIGHT.VIEW_TICKETING_DATA)
				&& !security.HasRight(RIGHT.MODIFY_TICKETING_DATA)
				&& !security.HasRight(RIGHT.VIEW_REPORTS)
				&& !security.HasRight(RIGHT.MODIFY_REPORTS)
				&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
				&& !security.HasRight(RIGHT.MODIFY_ALLOCATIONS)
				&& !security.HasRight(RIGHT.VIEW_ALLOCATIONS)
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES)
				&& !security.HasRight(RIGHT.VIEW_TRANSACTION_ALIASES)
				&& !security.HasRight(RIGHT.VIEW_ORDERS)
				&& !security.HasRight(RIGHT.MODIFY_ORDERS)
				&& !security.HasRight(RIGHT.CREATE_ORDERS)
				&& !security.HasRight(RIGHT.VIEW_SUPPLY_ORDERS)
				&& !security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS)
				&& !security.HasRight(RIGHT.CREATE_SUPPLY_ORDERS)
				&& !security.HasRight(RIGHT.PERFORM_CLOSEOUT)
				&& !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA)
				&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
				&& !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
				&& !security.HasRight(RIGHT.VIEW_AUTO_DISTRIBUTION_CONFIGURATION)
				&& !security.HasRight(RIGHT.MODIFY_AUTO_DISTRIBUTION_CONFIGURATION)
				&& !security.HasRight(RIGHT.IMPORT_TRANSACTION))
			{
				throw new FMInsufficientRightsException();
			}

			DataSet dataSet;

			using (var cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_EnumerateProductsAllSites";

				dataSet = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			return dataSet;
		}


		public ProductCollectionClass EnumerateUndelegated(SecurityClass security)
		{
				ProductCollectionClass productCollection = new ProductCollectionClass();
				if (security == null)
				{
					throw new ArgumentNullException(nameof(security));
				}

				DataSet set;
				using (SqlCommand cmd = new SqlCommand())
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "dbo.usp_GetUndelegatedProducts";
					cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
					cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
					set = this.ConsolidatedDA.GetDataSet(cmd, security);
				}

				if (set.Tables[0].Rows.Count > 0)
				{
					while (set.Tables[0].Rows.Count != 0)
					{
						DataRow row = set.Tables[0].Rows[0];
						var product = new ProductClass
											{
												IdentityGuid = DataObject.getValue(row["ProductGuid"], Guid.Empty),
												MasterRecordGuid =
														DataObject.getValue(row["_MasterRecordGuid"], Guid.Empty),
												ID = DataObject.getValue(row["ProductId"], string.Empty),
												SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty),
												AssignedToSiteGuid =
														DataObject.getValue(row["SiteGuid"], Guid.Empty),
												AssignedFromSiteGuid =
														DataObject.getValue(row["AssignedFromSiteGuid"], Guid.Empty),
												AssignedFromSiteId =
														DataObject.getValue<string>(
															row["AssignedFromSiteId"],
															string.Empty)
											};
						//This query is limited to master records, i.e. SiteOwner, AssignedFromSite, and AssignedToSite are the same.
						productCollection.Add(product);
						set.Tables[0].Rows.RemoveAt(0);
					}
				}
				return productCollection;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Import(SecurityClass securityParam, ProductClass product)
		{

				if (securityParam == null)
				{
					throw new ArgumentNullException(nameof(securityParam));
				}

				if (product == null)
				{
					throw new ArgumentNullException(nameof(product));
				}

				SecurityClass security = securityParam.Clone();

				CompaniesClass companies = new CompaniesClass();
				CompanyGroupsClass companyGroups = new CompanyGroupsClass();
				AdditiveProfiles additiveProfiles = new AdditiveProfiles();
				ApplicationStringsClass applicationStrings = new ApplicationStringsClass();

				SitesClass sites = new SitesClass();
				SiteClass site = sites.Get(security, security.SiteGuid, false, false, false);

				try
				{
					product.IdentityGuid = this.GetIdentityGuid(security, product.ID);

					// If the entity exists and is not owned by this site, do not update it.
					ProductClass checkProduct = this.GetByInfoAuthorizedCompanies(security, product.IdentityGuid, true, false,true);

					if (product.IdentityGuid != Guid.Empty && checkProduct.SiteGuid != security.SiteGuid)
					{
						return;
					}

					product.SetSiteUnits(site);

					var customerList = companies.EnumerateExt(security, byGroupCompanies: false, bLocalize: false);

					foreach (ProductMapClass authorizedCustomer in product.AuthorizedCustomerCollection)
					{
						//Guid identityGuid = Companies.GetIdentityGuid(security, AuthorizedCustomer.AssignedToID);

						var company = customerList.Find(x => x.ID == authorizedCustomer.AssignedToID);

					if (company == null)
					{
						continue;
					}

						Guid identityGuid = company.IdentityGuid;

						if (identityGuid == Guid.Empty)
						{
								CompanyClass customer = new CompanyClass(site) { ID = authorizedCustomer.AssignedToID };
								CompanyRoleMapClass role = new CompanyRoleMapClass { Role = COMPANY_ROLE.CUSTOMER_SHIPTO };

								customer.RoleCollection.Add(role);
								identityGuid = companies.Add(security, customer);
						}

						authorizedCustomer.AssignedToGuid = identityGuid;
						authorizedCustomer.Type = PRODUCT_MAP_TYPE.PRODUCT_COMPANY_MAP;

						if (authorizedCustomer.AdditiveProfileID != string.Empty)
						{
								identityGuid = additiveProfiles.GetIdentityGuid(security, authorizedCustomer.AdditiveProfileID);

								if (identityGuid == Guid.Empty)
								{
									AdditiveProfileClass additiveProfile = new AdditiveProfileClass
																						{
																							ID =
																									authorizedCustomer
																									.AdditiveProfileID
																						};
									identityGuid = additiveProfiles.Add(security, additiveProfile);
								}

								authorizedCustomer.AdditiveProfileGuid = identityGuid;
						}
					}

					foreach (ProductMapClass authorizedCustomerGroup in product.AuthorizedCustomerGroupCollection)
					{
						Guid identityGuid = companyGroups.GetIdentityGuid(security, authorizedCustomerGroup.AssignedToID);
						if (identityGuid == Guid.Empty)
						{
								CompanyGroupClass customerGroup = new CompanyGroupClass
																			{
																				ID = authorizedCustomerGroup.AssignedToID
																			};
								identityGuid = companyGroups.Add(security, customerGroup);
						}

						authorizedCustomerGroup.AssignedToGuid = identityGuid;
						authorizedCustomerGroup.Type = PRODUCT_MAP_TYPE.PRODUCT_COMPANY_GROUP_MAP;

						if (authorizedCustomerGroup.AdditiveProfileID != string.Empty)
						{
								identityGuid = additiveProfiles.GetIdentityGuid(security, authorizedCustomerGroup.AdditiveProfileID);
								if (identityGuid == Guid.Empty)
								{
									AdditiveProfileClass additiveProfile = new AdditiveProfileClass
																						{
																							ID = authorizedCustomerGroup.AdditiveProfileID
																						};
									identityGuid = additiveProfiles.Add(security, additiveProfile);
								}
								authorizedCustomerGroup.AdditiveProfileGuid = identityGuid;
						}
					}

					foreach (ProductMapClass blendComponent in product.ComponentCollection)
					{
						Guid identityGuid = this.GetIdentityGuid(security, blendComponent.AssignedID);
						if (identityGuid == Guid.Empty)
						{
								ProductClass component = new ProductClass(site)
																{
																	ID = blendComponent.AssignedID,
																	ProductType = ProductType.ComponentProduct
																};
								identityGuid = this.Add(security, component);
						}

						blendComponent.AssignedGuid = identityGuid;
						blendComponent.Type = PRODUCT_MAP_TYPE.BLEND_COMPONENT_MAP;
					}

					foreach (ApplicationStringMapClass productMessage in product.ProductMessageCollection)
					{
						Guid identityGuid = applicationStrings.GetIdentityGuid(security, STRING_TYPE.PRODUCT_MESSAGE, productMessage.ID);
						if (identityGuid == Guid.Empty)
						{
								ApplicationStringClass applicationString = new ApplicationStringClass
																						{
																							Type = STRING_TYPE.PRODUCT_MESSAGE,
																							ID = productMessage.ID
																						};
								identityGuid = applicationStrings.Add(security, applicationString);
						}
						productMessage.ApplicationStringGuid = identityGuid;
						productMessage.Type = STRING_MAP_TYPE.PRODUCT_MESSAGE;
					}

					foreach (ApplicationStringMapClass hazardousMessage in product.HazardousMaterialMessageCollection)
					{
						Guid identityGuid = applicationStrings.GetIdentityGuid(security, STRING_TYPE.DOT_HAZARDOUS_MESSAGE, hazardousMessage.ID);
						if (identityGuid == Guid.Empty)
						{
								ApplicationStringClass applicationString = new ApplicationStringClass
																						{
																							Type = STRING_TYPE.DOT_HAZARDOUS_MESSAGE,
																							ID = hazardousMessage.ID
																						};
								identityGuid = applicationStrings.Add(security, applicationString);
						}

						hazardousMessage.ApplicationStringGuid = identityGuid;
						hazardousMessage.Type = STRING_MAP_TYPE.DOT_HAZARDOUS_MESSAGE;
					}

					if (product.IdentityGuid == Guid.Empty)
					{
						this.Add(security, product);
					}
					else
					{
						this.Modify(security, product);
					}
				}
				catch (Exception ex)
				{
					throw new Exception("[Product Import Error ID] : " + product.ID + ", " + ex.Message);
				}
		}

		void IDependency.Insert(SecurityClass security, BaseDataObject Object, bool preOperation)
		{
				if (security == null)
					throw new ArgumentNullException(nameof(security));

				if (Object == null)
					throw new ArgumentNullException(nameof(Object));
		}

		void IDependency.Update(SecurityClass security, BaseDataObject Object)
		{
				if (security == null)
					throw new ArgumentNullException(nameof(security));

				if (Object == null)
					throw new ArgumentNullException(nameof(Object));

				var site = Object as SiteClass;
				if (site != null)
				{
					ProductCollectionClass productCollection = this.Enumerate(security);
					EntityToSiteMaps entityToSiteMaps = new EntityToSiteMaps();
					foreach (ProductClass product in productCollection)
					{
						if (site.SiteGuid == product.SiteGuid)
						{
								EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(security, product.EntityType, product.IdentityGuid);
								foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
								{
									if (entityToSiteMap.SiteGuid != site.SiteGuid)
									{
										entityToSiteMap.ID = product.ID;
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
					throw new ArgumentNullException(nameof(security));

				if (Object == null)
					throw new ArgumentNullException(nameof(Object));

				var site = Object as SiteClass;
				if (site != null)
				{
					ProductCollectionClass productCollection = this.Enumerate2(security, site.SiteGuid);
					EntityToSiteMaps entityToSiteMaps = new EntityToSiteMaps();
					foreach (ProductClass product in productCollection)
					{
						if (site.SiteGuid == product.SiteGuid && product.MasterRecordGuid == product.IdentityGuid)
						{
								this.Purge(security, product.IdentityGuid);
						}
						else
						{
								EntityToSiteMapClass entityToSiteMap = new EntityToSiteMapClass(product)
																					{
																						SiteGuid = site.SiteGuid
																					};
								entityToSiteMaps.Purge(security, entityToSiteMap);
						}
					}
				}

				else
				{
					var applicationString = Object as ApplicationStringClass;
					if (applicationString != null)
					{
						ApplicationStringMapCollectionClass messageCollection = new ApplicationStringMapCollectionClass();
						ApplicationStringMapsClass applicationStringMaps = new ApplicationStringMapsClass();

						if (applicationString.Type == STRING_TYPE.PRODUCT_MESSAGE)
								messageCollection = applicationStringMaps.EnumerateByApplicationStringGuidAndType(security, applicationString.IdentityGuid, STRING_MAP_TYPE.PRODUCT_MESSAGE);
						else if (applicationString.Type == STRING_TYPE.DOT_HAZARDOUS_MESSAGE)
								messageCollection = applicationStringMaps.EnumerateByApplicationStringGuidAndType(security, applicationString.IdentityGuid, STRING_MAP_TYPE.DOT_HAZARDOUS_MESSAGE);

						foreach (ApplicationStringMapClass message in messageCollection)
								applicationStringMaps.Purge(security,
									message.IdentityGuid,
									message.Type);
					}
				}
		}
	}
}
