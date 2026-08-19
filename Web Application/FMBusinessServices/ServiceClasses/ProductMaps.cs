using System;
using System.Collections.Generic;
using System.Data;
using System.Security;
using System.ServiceModel;
using System.Data.SqlClient;

using FMBusinessServices.DataAccessLayer;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	using FMBusinessObjects.Exceptions;
	using System.Diagnostics;

	/// <summary>
	/// Summary description for ProductMapsClass.
	/// </summary>
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class ProductMapsClass : IDependency, IProductMaps
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		protected void Validate(ProductMapClass productMap)
		{
			if (productMap.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP
			|| productMap.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP
			|| productMap.Type == PRODUCT_MAP_TYPE.PRESET_INJECTOR_MAP
				|| productMap.Type == PRODUCT_MAP_TYPE.PRESET_FLOW_CONTROLLED_ADDITIVE_MAP)
			{
				if (string.IsNullOrEmpty(productMap.MeterID))
				{
					if (string.IsNullOrEmpty(productMap.Meter.ID))
					{
						throw (new Exception("Meter ID Required"));
					}
					else
					{
						productMap.MeterID = productMap.Meter.ID;
					}
				}
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, ProductMapClass productMap)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (productMap == null)
			{
				throw new ArgumentNullException(nameof(productMap));
			}

			this.Validate(productMap);

			Guid existingProductMapGuid = this.GetIdentityGuid(security, productMap.AssignedToGuid, productMap.AssignedGuid, productMap.Type);

			if (existingProductMapGuid != Guid.Empty)
			{
				throw new Exception("Product Map exists");
			}

			// If the product map contains a meter, 
			// add it to the database if the meter ID doesn't already exist
			// If the meter ID does exist, look it up and remember the guid.
			if ((productMap.Meter != null) && (productMap.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP
				|| productMap.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP
				|| productMap.Type == PRODUCT_MAP_TYPE.PRESET_INJECTOR_MAP
				|| productMap.Type == PRODUCT_MAP_TYPE.PRESET_FLOW_CONTROLLED_ADDITIVE_MAP
				|| productMap.Type == PRODUCT_MAP_TYPE.OFFLOAD_EXTERNAL_METER_MAP))
			{
				MetersClass meters = new MetersClass();

				Guid existingMeterGuid = Guid.Empty;

				// Only components can share meters, and the meter can only be shared for the same load arm. 
				// Only try to look for existing meters if the type of map we're adding is a component
				if (productMap.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP || productMap.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP)
				{
					existingMeterGuid = meters.GetIdentityGuidForLoadArmComponentMeter(security, productMap.Meter.ID, productMap.AssignedToGuid);
				}

				if (existingMeterGuid != Guid.Empty)
				{
					productMap.AssignedToMeterGuid = existingMeterGuid;
				}
				else
				{
					productMap.AssignedToMeterGuid = meters.Add(security, productMap.Meter);
				}
			}

			productMap.SiteGuid = security.SiteGuid;
			productMap.CreatedDate = DateTimeOffset.Now;
			productMap.CreatedBy = security.UserID;
			productMap.UpdatedDate = productMap.CreatedDate;
			productMap.UpdatedBy = security.UserID;
			productMap.IdentityGuid = Guid.NewGuid();

			using (SqlCommand cmd = new SqlCommand())
			{
				productMap.InsertSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			ProcessVariablesClass processVariables = new ProcessVariablesClass();
			processVariables.ModifyCollection(security, productMap.IdentityGuid, productMap.Permissives.Inputs, null);
			processVariables.ModifyCollection(security, productMap.IdentityGuid, productMap.Permissives.Outputs, null);
			processVariables.ModifyCollection(security, productMap.IdentityGuid, productMap.ProcessVariableCollection, null);

			return productMap.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, ProductMapClass productMap)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (productMap == null)
			{
				throw new ArgumentNullException(nameof(productMap));
			}

			this.Validate(productMap);

			Guid existingProductMapGuid = this.GetIdentityGuid(security, productMap.AssignedToGuid, productMap.AssignedGuid, productMap.Type);

			if (existingProductMapGuid != Guid.Empty && existingProductMapGuid != productMap.IdentityGuid)
			{
				throw new Exception("Product Map exists");
			}

			ProductMapClass existingProductMap = this.Get(security, productMap.IdentityGuid, productMap.Type);

			if (existingProductMap == null || existingProductMap.IdentityGuid == Guid.Empty)
			{
				throw new Exception("Product Map not found");
			}

			// The meter assigned to this product map may need to be added, deleted, or modified. 
			// If the meter needs to be deleted, we have to delete it after the product map update
			// to avoid a foreign key violation.
			// ModifyMeter will return a non-empty guid if a delete needs to occur
			if ((productMap.Meter != null && string.IsNullOrEmpty(productMap.Meter.ID) == false) 
				&& (productMap.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP
					|| productMap.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP
					|| productMap.Type == PRODUCT_MAP_TYPE.PRESET_INJECTOR_MAP
					|| productMap.Type == PRODUCT_MAP_TYPE.PRESET_FLOW_CONTROLLED_ADDITIVE_MAP
					|| productMap.Type == PRODUCT_MAP_TYPE.OFFLOAD_EXTERNAL_METER_MAP))
			{
				Guid meterGuidToBeDeleted = this.ModifyMeter(security, productMap);

				// If we detected that the meter should be deleted, delete it
				if (meterGuidToBeDeleted != Guid.Empty)
				{
					MetersClass meters = new MetersClass();
					meters.Purge(security, meterGuidToBeDeleted);
				}
			}

			productMap.UpdatedDate = DateTimeOffset.Now;
			productMap.UpdatedBy = security.UserID;

			using (SqlCommand cmd = new SqlCommand())
			{
				productMap.UpdateSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			UNIT_TYPE inputUnitType = UNIT_TYPE.MAX_UNIT;
			UNIT_TYPE outputUnitType = UNIT_TYPE.MAX_UNIT;

			ProcessVariablesClass processVariables = new ProcessVariablesClass();

			ProcessVariableCollectionClass oldProcessVariableCollection;

			if (productMap.Type == PRODUCT_MAP_TYPE.PRESET_INJECTOR_MAP)
			{
				oldProcessVariableCollection = processVariables.EnumerateByUnit(security, productMap.IdentityGuid, UNIT_TYPE.PRODUCT_MAP_PRESET_INJECTOR);
			}
			else if (productMap.Type == PRODUCT_MAP_TYPE.OFFLOAD_EXTERNAL_METER_MAP)
			{
				oldProcessVariableCollection = processVariables.EnumerateByUnit(security, productMap.IdentityGuid, UNIT_TYPE.PRODUCT_MAP_OFFLOAD_EXTERNAL_METER);
			}
			else
			{
				oldProcessVariableCollection = processVariables.EnumerateByUnit(security, productMap.IdentityGuid, UNIT_TYPE.PRODUCT_MAP_PRESET_EXTERNAL_COMPONENT);
			}

			processVariables.ModifyCollection(security, productMap.IdentityGuid, productMap.ProcessVariableCollection, oldProcessVariableCollection);

			if (productMap.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP
			|| productMap.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP)
			{
				inputUnitType = UNIT_TYPE.COMPONENT_INPUT_PERMISSIVE;
				outputUnitType = UNIT_TYPE.COMPONENT_OUTPUT_PERMISSIVE;
			}
			else if (productMap.Type == PRODUCT_MAP_TYPE.PRESET_INJECTOR_MAP)
			{
				inputUnitType = UNIT_TYPE.ADDITIVE_INPUT_PERMISSIVE;
				outputUnitType = UNIT_TYPE.ADDITIVE_OUTPUT_PERMISSIVE;
			}
			else if (productMap.Type == PRODUCT_MAP_TYPE.PRESET_RECIPE_MAP)
			{
				inputUnitType = UNIT_TYPE.RECIPE_INPUT_PERMISSIVE;
				outputUnitType = UNIT_TYPE.RECIPE_OUTPUT_PERMISSIVE;
			}
			else if (productMap.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP)
			{
				inputUnitType = UNIT_TYPE.EXTERNAL_COMPONENT_INPUT_PERMISSIVE;
				outputUnitType = UNIT_TYPE.EXTERNAL_COMPONENT_OUTPUT_PERMISSIVE;
			}
			else if (productMap.Type == PRODUCT_MAP_TYPE.PRESET_FLOW_CONTROLLED_ADDITIVE_MAP)
			{
				inputUnitType = UNIT_TYPE.FLOW_CONTROLLED_ADDITIVE_INPUT_PERMISSIVE;
				outputUnitType = UNIT_TYPE.FLOW_CONTROLLED_ADDITIVE_OUTPUT_PERMISSIVE;
			}
			else if (productMap.Type == PRODUCT_MAP_TYPE.OFFLOAD_EXTERNAL_METER_MAP)
			{
				inputUnitType = UNIT_TYPE.EXTERNAL_METER_INPUT_PERMISSIVE;
				outputUnitType = UNIT_TYPE.EXTERNAL_METER_OUTPUT_PERMISSIVE;
			}

			if (inputUnitType != UNIT_TYPE.MAX_UNIT)
			{
				ProcessVariableCollectionClass oldInputPermissives = processVariables.EnumerateByUnit(security, productMap.IdentityGuid, inputUnitType);
				processVariables.ModifyCollection(security, productMap.IdentityGuid, productMap.Permissives.Inputs, oldInputPermissives);
			}

			if (outputUnitType != UNIT_TYPE.MAX_UNIT)
			{
				ProcessVariableCollectionClass oldOutputPermissives = processVariables.EnumerateByUnit(security, productMap.IdentityGuid, outputUnitType);
				processVariables.ModifyCollection(security, productMap.IdentityGuid, productMap.Permissives.Outputs, oldOutputPermissives);
			}

		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid identityGuid, PRODUCT_MAP_TYPE type)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			ProductMapClass productMap = this.Get(security, identityGuid, type);

			ProcessVariablesClass processVariables = new ProcessVariablesClass();
			processVariables.ModifyCollection(security, productMap.IdentityGuid, null, productMap.Permissives.Inputs);
			processVariables.ModifyCollection(security, productMap.IdentityGuid, null, productMap.Permissives.Outputs);
			processVariables.ModifyCollection(security, productMap.IdentityGuid, null, productMap.ProcessVariableCollection);

			using (SqlCommand cmd = new SqlCommand())
			{
				productMap.PurgeSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			// If a meter is assigned to the product map, delete it, but only if it's not assigned to any other product maps
			if (productMap.AssignedToMeterGuid != Guid.Empty)
			{
				MetersClass meters = new MetersClass();

				if (!meters.AssignedToAnyProductMap(security, productMap.AssignedToMeterGuid))
				{
					meters.Purge(security, productMap.AssignedToMeterGuid);
				}
			}
		}

		public string GetSpecialInstructions(SecurityClass security, Guid identityGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			SitesClass sites = new SitesClass();
			SiteClass site = sites.GetByMemberAndProcessVariables(security, security.SiteGuid, false, false);

			ProductMapClass productMap = new ProductMapClass(site)
			{
				IdentityGuid = identityGuid,
				Type = PRODUCT_MAP_TYPE.PRODUCT_COMPANY_MAP
			};

			DataSet set;
			using (var cmd = new SqlCommand())
			{
				//Product.SelectSQL(cmd, ContextUtil.IsInTransaction);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "map.usp_GetProductToCompanyMapByGuid";
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@ProductToCompanyGuid", SqlDbType.UniqueIdentifier);

				cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				cmd.Parameters["@ProductToCompanyGuid"].Value = identityGuid;
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			productMap.Load(set);

			if (productMap.IdentityGuid == Guid.Empty)
			{
				productMap.IdentityGuid = identityGuid;
				productMap.Type = PRODUCT_MAP_TYPE.PRODUCT_COMPANY_GROUP_MAP;

				using (SqlCommand cmd = new SqlCommand())
				{
					productMap.SelectSQL(cmd, security.SiteGuid);
					productMap.Load(this.ConsolidatedDA.GetDataSet(cmd, security));
				}
			}

			return productMap.SpecialInstructions;
		}

		public ProductMapClass Get(SecurityClass security, Guid identityGuid, PRODUCT_MAP_TYPE type)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

			SitesClass sites = new SitesClass();
			SiteClass site = sites.GetByMemberAndProcessVariables(security, security.SiteGuid, false, false);

			ProductMapClass productMap = new ProductMapClass(site) { IdentityGuid = identityGuid, Type = type };


			DataSet set;
			using (SqlCommand cmd = new SqlCommand())
			{
				bool isProdToComMapping = false;
				if (type == PRODUCT_MAP_TYPE.PRODUCT_COMPANY_MAP)
				{
					isProdToComMapping = true;
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "map.usp_GetProductToCompanyMapByGuid";
					cmd.Parameters.Add("@ProductToCompanyGuid", SqlDbType.UniqueIdentifier);
					cmd.Parameters["@ProductToCompanyGuid"].Value = identityGuid;
				}
				else if (type == PRODUCT_MAP_TYPE.SUPPLIER_PRODUCT_COMPANY_MAP)
				{
					isProdToComMapping = true;
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "map.usp_GetProdToSupplierProdComMapByGuid";
					cmd.Parameters.Add("@ProductToSupplierProductCompanyGuid", SqlDbType.UniqueIdentifier);
					cmd.Parameters["@ProductToSupplierProductCompanyGuid"].Value = identityGuid;
				}
				else if (type == PRODUCT_MAP_TYPE.UNAVAILABLE_INVENTORY_COMPANY_MAP)
				{
					isProdToComMapping = true;
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "map.usp_GetProdToUnavailableInventoryComMapByGuid";
					cmd.Parameters.Add("@ProductToUnavailableInventoryCompanyGuid", SqlDbType.UniqueIdentifier);
					cmd.Parameters["@ProductToUnavailableInventoryCompanyGuid"].Value = identityGuid;
				}

				if (isProdToComMapping)
				{
					//ProductMap.SelectSQL(cmd, security.SiteGuid);
					cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
					cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				}
				else
				{
					productMap.SelectSQL(cmd, security.SiteGuid);
				}
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}
			productMap.Load(set);

			if (productMap.AssignedToMeterGuid != Guid.Empty)
			{
				MetersClass meters = new MetersClass();
				productMap.Meter = meters.Get(security, productMap.AssignedToMeterGuid);
			}

			UNIT_TYPE inputUnitType = UNIT_TYPE.MAX_UNIT;
			UNIT_TYPE outputUnitType = UNIT_TYPE.MAX_UNIT;

			ProcessVariablesClass processVariables = new ProcessVariablesClass();

			if (productMap.Type == PRODUCT_MAP_TYPE.PRESET_INJECTOR_MAP)
			{
				productMap.ProcessVariableCollection = processVariables.EnumerateByUnit(security, productMap.IdentityGuid, UNIT_TYPE.PRODUCT_MAP_PRESET_INJECTOR);
			}
			else if (productMap.Type == PRODUCT_MAP_TYPE.OFFLOAD_EXTERNAL_METER_MAP)
			{
				productMap.ProcessVariableCollection = processVariables.EnumerateByUnit(security, productMap.IdentityGuid, UNIT_TYPE.PRODUCT_MAP_OFFLOAD_EXTERNAL_METER);
			}
			else
			{
				productMap.ProcessVariableCollection = processVariables.EnumerateByUnit(security, productMap.IdentityGuid, UNIT_TYPE.PRODUCT_MAP_PRESET_EXTERNAL_COMPONENT);
			}

			if (productMap.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP
				|| productMap.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP)
			{
				inputUnitType = UNIT_TYPE.COMPONENT_INPUT_PERMISSIVE;
				outputUnitType = UNIT_TYPE.COMPONENT_OUTPUT_PERMISSIVE;
			}
			else if (productMap.Type == PRODUCT_MAP_TYPE.PRESET_INJECTOR_MAP)
			{
				inputUnitType = UNIT_TYPE.ADDITIVE_INPUT_PERMISSIVE;
				outputUnitType = UNIT_TYPE.ADDITIVE_OUTPUT_PERMISSIVE;
			}
			else if (productMap.Type == PRODUCT_MAP_TYPE.PRESET_RECIPE_MAP)
			{
				inputUnitType = UNIT_TYPE.RECIPE_INPUT_PERMISSIVE;
				outputUnitType = UNIT_TYPE.RECIPE_OUTPUT_PERMISSIVE;
			}
			else if (productMap.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP)
			{
				inputUnitType = UNIT_TYPE.EXTERNAL_COMPONENT_INPUT_PERMISSIVE;
				outputUnitType = UNIT_TYPE.EXTERNAL_COMPONENT_OUTPUT_PERMISSIVE;
			}
			else if (productMap.Type == PRODUCT_MAP_TYPE.PRESET_FLOW_CONTROLLED_ADDITIVE_MAP)
			{
				inputUnitType = UNIT_TYPE.FLOW_CONTROLLED_ADDITIVE_INPUT_PERMISSIVE;
				outputUnitType = UNIT_TYPE.FLOW_CONTROLLED_ADDITIVE_OUTPUT_PERMISSIVE;
			}
			else if (productMap.Type == PRODUCT_MAP_TYPE.OFFLOAD_EXTERNAL_METER_MAP)
			{
				inputUnitType = UNIT_TYPE.EXTERNAL_METER_INPUT_PERMISSIVE;
				outputUnitType = UNIT_TYPE.EXTERNAL_METER_OUTPUT_PERMISSIVE;
			}

			if (inputUnitType != UNIT_TYPE.MAX_UNIT)
				productMap.Permissives.Inputs = processVariables.EnumerateByUnit(security, productMap.IdentityGuid, inputUnitType);

			if (outputUnitType != UNIT_TYPE.MAX_UNIT)
				productMap.Permissives.Outputs = processVariables.EnumerateByUnit(security, productMap.IdentityGuid, outputUnitType);

			return productMap;
		}

		public Guid GetIdentityGuid(SecurityClass security, Guid assignedToGuid, Guid assignedGuid, PRODUCT_MAP_TYPE type)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_PRODUCTS) && !security.HasRight(RIGHT.MODIFY_PRODUCTS)
					&& !security.HasRight(RIGHT.VIEW_ORDERS) && !security.HasRight(RIGHT.MODIFY_ORDERS)
					&& !security.HasRight(RIGHT.CREATE_ORDERS) && !security.HasRight(RIGHT.VIEW_SUPPLY_ORDERS)
					&& !security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS) && !security.HasRight(RIGHT.CREATE_SUPPLY_ORDERS))
			{
				throw new FMInsufficientRightsException();
			}

			ProductMapClass productMap = new ProductMapClass
			{
				AssignedToGuid = assignedToGuid,
				AssignedGuid = assignedGuid,
				Type = type
			};

			DataSet set;
			using (SqlCommand cmd = new SqlCommand())
			{
				productMap.SelectIdentityGuidSQL(cmd);
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			if (set.Tables.Count == 1 && set.Tables[0].Rows.Count == 1)
			{
				return (Guid)set.Tables[0].Rows[0][0];
			}

			return Guid.Empty;
		}

		/// <summary>
		/// Using the specified company guid, retrieve all product maps that are associated with the company
		/// and that have special instruction text
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <param name="assignedToCompanyGuid">Identifies the company</param>
		/// <returns>All product maps that are associated with the company and that have special instruction text</returns>
		public ProductMapCollectionClass EnumerateSpecialInstructionsByAssignedToCompany(SecurityClass security, Guid assignedToCompanyGuid)
		{
			ProductMapCollectionClass productMapsWithSpecialInstructions = new ProductMapCollectionClass();

			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			ProductMapClass productMap = new ProductMapClass
			{
				AssignedToGuid = assignedToCompanyGuid,
				Type = PRODUCT_MAP_TYPE.PRODUCT_COMPANY_MAP,
				SiteGuid = security.SiteGuid
			};

			DataSet set;

			using (SqlCommand cmd = new SqlCommand())
			{
				productMap.EnumerateSpecialInstructionsByAssignedToCompanySQL(cmd);
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			if (set != null)
			{
				DataTable dataTable = set.Tables[0];

				foreach (DataRow row in dataTable.Rows)
				{
					ProductMapClass matchingProductMap = new ProductMapClass { Type = PRODUCT_MAP_TYPE.PRODUCT_COMPANY_MAP };
					matchingProductMap.Load(row);
					productMapsWithSpecialInstructions.Add(matchingProductMap);
				}
			}

			return productMapsWithSpecialInstructions;
		}

		public ProductMapCollectionClass EnumerateByAssignedToGuidAndType(SecurityClass security, Guid assignedToGuid, PRODUCT_MAP_TYPE type, bool hideHiddenProducts = false)
		{
			return this.EnumerateByAssignedToGuidAndTypeInstr(security, assignedToGuid, type, true, hideHiddenProducts);
		}

		/// <summary>
		/// The enumerate by type.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="type">
		/// The type.
		/// </param>
		/// <param name="getProcessVars">
		/// The b get process variables.
		/// </param>
		/// <returns>
		/// The <see cref="ProductMapCollectionClass"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException"> Argument null exception
		/// </exception>
		public ProductMapCollectionClass EnumerateByType(
			SecurityClass security, PRODUCT_MAP_TYPE type, bool getProcessVars)
		{
			const string SecurityString = "security";
			if (security == null)
			{
				throw new ArgumentNullException(SecurityString);
			}

			var sites = new SitesClass();
			var site = sites.GetByMemberAndProcessVariables(security, security.SiteGuid, false, false);

			// ProductMap.AssignedToGuid = assignedToGuid;
			var processVariables = new ProcessVariablesClass();

			DataSet set;
			using (var cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				switch (type)
				{
					case PRODUCT_MAP_TYPE.PRODUCT_COMPANY_MAP:
						cmd.CommandText = "map.usp_GetProductToCompanyMapBySite ";
						break;
					case PRODUCT_MAP_TYPE.SUPPLIER_PRODUCT_COMPANY_MAP:
						cmd.CommandText = "map.usp_GetProdToSupplierProdComMapBySite ";
						break;
					case PRODUCT_MAP_TYPE.UNAVAILABLE_INVENTORY_COMPANY_MAP:
						cmd.CommandText = "map.usp_GetProdToUnavailableInventoryComMapBySite ";
						break;
				}

				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			var productMapCollection = new ProductMapCollectionClass();

			var table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				var productMap = new ProductMapClass(site) { Type = type };

				productMap.Load(set);

				if (productMap.AssignedToMeterGuid != Guid.Empty)
				{
					var meters = new MetersClass();
					productMap.Meter = meters.Get(security, productMap.AssignedToMeterGuid);
				}

				if (getProcessVars)
				{
					if (productMap.Type == PRODUCT_MAP_TYPE.PRESET_INJECTOR_MAP)
						productMap.ProcessVariableCollection = processVariables.EnumerateByUnit(security, productMap.IdentityGuid, UNIT_TYPE.PRODUCT_MAP_PRESET_INJECTOR);
					else if (productMap.Type == PRODUCT_MAP_TYPE.OFFLOAD_EXTERNAL_METER_MAP)
						productMap.ProcessVariableCollection = processVariables.EnumerateByUnit(security, productMap.IdentityGuid, UNIT_TYPE.PRODUCT_MAP_OFFLOAD_EXTERNAL_METER);
					else
						productMap.ProcessVariableCollection = processVariables.EnumerateByUnit(security, productMap.IdentityGuid, UNIT_TYPE.PRODUCT_MAP_PRESET_EXTERNAL_COMPONENT);
				}

				productMapCollection.Add(productMap);

				table.Rows.RemoveAt(0);
			}

			return productMapCollection;
		}

		public ProductMapCollectionClass EnumerateByAssignedToGuidAndTypeInstr(SecurityClass security, Guid assignedToGuid, PRODUCT_MAP_TYPE type, bool bGetProcessVariables, bool hideHiddenProducts = false)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}


			SitesClass sites = new SitesClass();
			SiteClass site = sites.GetByMemberAndProcessVariables(security, security.SiteGuid, false, false);

			ProductMapClass productMap = new ProductMapClass
			{
				AssignedToGuid = assignedToGuid,
				Type = type,
				SiteGuid = security.SiteGuid
			};

			UNIT_TYPE inputUnitType = UNIT_TYPE.MAX_UNIT;
			UNIT_TYPE outputUnitType = UNIT_TYPE.MAX_UNIT;

			if (productMap.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP
			|| productMap.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP)
			{
				inputUnitType = UNIT_TYPE.COMPONENT_INPUT_PERMISSIVE;
				outputUnitType = UNIT_TYPE.COMPONENT_OUTPUT_PERMISSIVE;
			}

			else if (productMap.Type == PRODUCT_MAP_TYPE.PRESET_INJECTOR_MAP)
			{
				inputUnitType = UNIT_TYPE.ADDITIVE_INPUT_PERMISSIVE;
				outputUnitType = UNIT_TYPE.ADDITIVE_OUTPUT_PERMISSIVE;
			}

			else if (productMap.Type == PRODUCT_MAP_TYPE.PRESET_RECIPE_MAP)
			{
				inputUnitType = UNIT_TYPE.RECIPE_INPUT_PERMISSIVE;
				outputUnitType = UNIT_TYPE.RECIPE_OUTPUT_PERMISSIVE;
			}

			else if (productMap.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP)
			{
				inputUnitType = UNIT_TYPE.EXTERNAL_COMPONENT_INPUT_PERMISSIVE;
				outputUnitType = UNIT_TYPE.EXTERNAL_COMPONENT_OUTPUT_PERMISSIVE;
			}

			else if (productMap.Type == PRODUCT_MAP_TYPE.PRESET_FLOW_CONTROLLED_ADDITIVE_MAP)
			{
				inputUnitType = UNIT_TYPE.FLOW_CONTROLLED_ADDITIVE_INPUT_PERMISSIVE;
				outputUnitType = UNIT_TYPE.FLOW_CONTROLLED_ADDITIVE_INPUT_PERMISSIVE;
			}
			else if (productMap.Type == PRODUCT_MAP_TYPE.OFFLOAD_EXTERNAL_METER_MAP)
			{
				inputUnitType = UNIT_TYPE.EXTERNAL_METER_INPUT_PERMISSIVE;
				outputUnitType = UNIT_TYPE.EXTERNAL_METER_OUTPUT_PERMISSIVE;
			}

			ProcessVariablesClass processVariables = new ProcessVariablesClass();

			DataSet set;
			using (SqlCommand cmd = new SqlCommand())
			{
				bool isProdToComMapping = false;
				if (type == PRODUCT_MAP_TYPE.PRODUCT_COMPANY_MAP)
				{
					isProdToComMapping = true;
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "map.usp_GetProductToCompanyMapByComGuid";
				}
				else if (type == PRODUCT_MAP_TYPE.SUPPLIER_PRODUCT_COMPANY_MAP)
				{
					isProdToComMapping = true;
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "map.usp_GetProdToSupplierProdComMapByComGuid";
				}
				else if (type == PRODUCT_MAP_TYPE.UNAVAILABLE_INVENTORY_COMPANY_MAP)
				{
					isProdToComMapping = true;
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "map.usp_GetProdToUnavailableInventoryComMapByComGuid";
				}

				if (isProdToComMapping)
				{
					//ProductMap.EnumerateByAssignedToGuidAndTypeSQL(cmd, ContextUtil.IsInTransaction, security.SiteGuid);
					cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
					cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
					cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);
					cmd.Parameters["@CompanyGuid"].Value = assignedToGuid;

					if (hideHiddenProducts)
					{
						cmd.Parameters.Add("@HideHiddenProducts", SqlDbType.Bit).Value = 1;
					}
				}
				else
				{
					productMap.EnumerateByAssignedToGuidAndTypeSQL(cmd, ContextUtil.IsInTransaction, security.SiteGuid, hideHiddenProducts);
				}

				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			ProductMapCollectionClass productMapCollection = new ProductMapCollectionClass();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				productMap = new ProductMapClass(site) { Type = type };

				productMap.Load(set);

				if (productMap.AssignedToMeterGuid != Guid.Empty)
				{
					MetersClass meters = new MetersClass();
					productMap.Meter = meters.Get(security, productMap.AssignedToMeterGuid);
				}

				if (inputUnitType != UNIT_TYPE.MAX_UNIT)
				{
					productMap.Permissives.Inputs = processVariables.EnumerateByUnit(security, productMap.IdentityGuid, inputUnitType);
				}

				if (outputUnitType != UNIT_TYPE.MAX_UNIT)
				{
					productMap.Permissives.Outputs = processVariables.EnumerateByUnit(security, productMap.IdentityGuid, outputUnitType);
				}

				if (bGetProcessVariables)
				{
					if (productMap.Type == PRODUCT_MAP_TYPE.PRESET_INJECTOR_MAP)
					{
						productMap.ProcessVariableCollection = processVariables.EnumerateByUnit(security, productMap.IdentityGuid, UNIT_TYPE.PRODUCT_MAP_PRESET_INJECTOR);
					}
					else if (productMap.Type == PRODUCT_MAP_TYPE.OFFLOAD_EXTERNAL_METER_MAP)
					{
						productMap.ProcessVariableCollection = processVariables.EnumerateByUnit(security, productMap.IdentityGuid, UNIT_TYPE.PRODUCT_MAP_OFFLOAD_EXTERNAL_METER);
					}
					else
					{
						productMap.ProcessVariableCollection = processVariables.EnumerateByUnit(security, productMap.IdentityGuid, UNIT_TYPE.PRODUCT_MAP_PRESET_EXTERNAL_COMPONENT);
					}
				}

				productMapCollection.Add(productMap);

				table.Rows.RemoveAt(0);
			}

			return productMapCollection;
		}

		public ProductMapCollectionClass EnumerateByAssignedGuidAndType(SecurityClass security, Guid assignedGuid, PRODUCT_MAP_TYPE type)
		{
			return this.EnumerateByAssignedGuidAndTypeAndInstr(security, assignedGuid, type);
		}

		public ProductMapCollectionClass EnumerateByAssignedGuidAndTypeAndInstr(SecurityClass security, Guid assignedGuid, PRODUCT_MAP_TYPE type, bool LoadProcessVariables = true)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			SitesClass sites = new SitesClass();
			SiteClass site = sites.GetByMemberAndProcessVariables(security, security.SiteGuid, false, false);

			ProductMapClass productMap = new ProductMapClass
			{
				AssignedGuid = assignedGuid,
				Type = type,
				SiteGuid = security.SiteGuid
			};

			ProcessVariablesClass processVariables = new ProcessVariablesClass();

			DataSet set;
			using (SqlCommand cmd = new SqlCommand())
			{
				bool isProdToComMapping = false;
				if (type == PRODUCT_MAP_TYPE.PRODUCT_COMPANY_MAP)
				{
					isProdToComMapping = true;
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "map.usp_GetProductToCompanyMapByProdGuid";
				}
				else if (type == PRODUCT_MAP_TYPE.SUPPLIER_PRODUCT_COMPANY_MAP)
				{
					isProdToComMapping = true;
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "map.usp_GetProdToSupplierProdComMapByProdGuid";
				}
				else if (type == PRODUCT_MAP_TYPE.UNAVAILABLE_INVENTORY_COMPANY_MAP)
				{
					isProdToComMapping = true;
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "map.usp_GetProdToUnavailableInventoryComMapByProdGuid";
				}

				if (type == PRODUCT_MAP_TYPE.PRODUCT_COMPANY_GROUP_MAP)
				{
					isProdToComMapping = true;
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "map.usp_GetProductToCompanyGroupMapByProdGuid";
				}

				if (isProdToComMapping)
				{
					//ProductMap.EnumerateByAssignedGuidAndTypeSQL(cmd, security.SiteGuid);
					cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
					cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
					cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);
					cmd.Parameters["@ProductGuid"].Value = assignedGuid;
				}
				else
				{
					productMap.EnumerateByAssignedGuidAndTypeSQL(cmd, security.SiteGuid);
				}

				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			ProductMapCollectionClass productMapCollection = new ProductMapCollectionClass();
			DataTable table = set.Tables[0];
			while (table != null && table.Rows.Count != 0)
			{
				productMap = new ProductMapClass(site) { Type = type };
				productMap.Load(set);
				if (LoadProcessVariables == true)
				{
					if (productMap.Permissives.InputUnitType != UNIT_TYPE.MAX_UNIT)
					{
						productMap.Permissives.Inputs = processVariables.EnumerateByUnit(security, productMap.IdentityGuid, productMap.Permissives.InputUnitType);
					}

					if (productMap.Permissives.OutputUnitType != UNIT_TYPE.MAX_UNIT)
					{
						productMap.Permissives.Outputs = processVariables.EnumerateByUnit(security, productMap.IdentityGuid, productMap.Permissives.OutputUnitType);
					}

					if (productMap.Type == PRODUCT_MAP_TYPE.PRESET_INJECTOR_MAP)
					{
						productMap.ProcessVariableCollection = processVariables.EnumerateByUnit(security, productMap.IdentityGuid, UNIT_TYPE.PRODUCT_MAP_PRESET_INJECTOR);
					}
					else if (productMap.Type == PRODUCT_MAP_TYPE.OFFLOAD_EXTERNAL_METER_MAP)
					{
						productMap.ProcessVariableCollection = processVariables.EnumerateByUnit(security, productMap.IdentityGuid, UNIT_TYPE.PRODUCT_MAP_OFFLOAD_EXTERNAL_METER);
					}
					else
					{
						productMap.ProcessVariableCollection = processVariables.EnumerateByUnit(security, productMap.IdentityGuid, UNIT_TYPE.PRODUCT_MAP_PRESET_EXTERNAL_COMPONENT);
					}
				}
				productMapCollection.Add(productMap);
				table.Rows.RemoveAt(0);
			}

			return productMapCollection;
		}

		public ProductMapCollectionClass EnumerateByType(SecurityClass security, PRODUCT_MAP_TYPE type)
		{
			const bool BLocalize = true;
			return this.EnumerateByTypeAndLocalize(security, type, BLocalize);
		}

		public ProductMapCollectionClass EnumerateByTypeAndLocalize(SecurityClass security, PRODUCT_MAP_TYPE type, bool bLocalize)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			SiteClass site = null;

			if (bLocalize)
			{
				SitesClass sites = new SitesClass();
				site = sites.GetByMemberAndProcessVariables(security, security.SiteGuid, false, false);
			}


			ProductMapClass productMap = new ProductMapClass { Type = type };

			ProcessVariablesClass processVariables = new ProcessVariablesClass();

			DataSet set;
			using (SqlCommand cmd = new SqlCommand())
			{
				bool isProdToComMapping = false;
				if (type == PRODUCT_MAP_TYPE.PRODUCT_COMPANY_MAP)
				{
					isProdToComMapping = true;
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "map.usp_GetProductToCompanyMapByGuid";
					cmd.Parameters.Add("@ProductToCompanyGuid", SqlDbType.UniqueIdentifier);
					cmd.Parameters["@ProductToCompanyGuid"].Value = DBNull.Value;
				}
				else if (type == PRODUCT_MAP_TYPE.SUPPLIER_PRODUCT_COMPANY_MAP)
				{
					isProdToComMapping = true;
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "map.usp_GetProdToSupplierProdComMapByGuid";
					cmd.Parameters.Add("@ProductToSupplierProductCompanyGuid", SqlDbType.UniqueIdentifier);
					cmd.Parameters["@ProductToSupplierProductCompanyGuid"].Value = DBNull.Value;
				}
				else if (type == PRODUCT_MAP_TYPE.UNAVAILABLE_INVENTORY_COMPANY_MAP)
				{
					isProdToComMapping = true;
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "map.usp_GetProdToUnavailableInventoryComMapByGuid";
					cmd.Parameters.Add("@ProductToUnavailableInventoryCompanyGuid", SqlDbType.UniqueIdentifier);
					cmd.Parameters["@ProductToUnavailableInventoryCompanyGuid"].Value = DBNull.Value;
				}

				if (isProdToComMapping)
				{
					//ProductMap.EnumerateByTypeSQL(cmd, security);
					cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
					cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				}
				else
				{
					// VRU/VCU stores the product master record and we need to retrieve it
					if (type == PRODUCT_MAP_TYPE.VRU_VCU_TRACKING)
					{
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.CommandText = "map.usp_GetProductToVruTrackingConfigBySiteGuid";
						cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
						cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
					}
					else
					{
						productMap.EnumerateByTypeSQL(cmd, security);
					}
				}

				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			ProductMapCollectionClass productMapCollection = new ProductMapCollectionClass();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				if (bLocalize)
				{
					productMap = new ProductMapClass(site) { Type = type };
				}
				else
				{
					productMap = new ProductMapClass { Type = type };
				}


				productMap.Load(set);

				if (productMap.Permissives.InputUnitType != UNIT_TYPE.MAX_UNIT)
				{
					productMap.Permissives.Inputs = processVariables.EnumerateByUnit(security, productMap.IdentityGuid, productMap.Permissives.InputUnitType);
				}

				if (productMap.Permissives.OutputUnitType != UNIT_TYPE.MAX_UNIT)
				{
					productMap.Permissives.Outputs = processVariables.EnumerateByUnit(security, productMap.IdentityGuid, productMap.Permissives.OutputUnitType);
				}

				if (productMap.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP)
				{
					productMap.ProcessVariableCollection = processVariables.EnumerateByUnit(security, productMap.IdentityGuid, UNIT_TYPE.PRODUCT_MAP_PRESET_EXTERNAL_COMPONENT);
				}
				else if (productMap.Type == PRODUCT_MAP_TYPE.PRESET_INJECTOR_MAP)
				{
					productMap.ProcessVariableCollection = processVariables.EnumerateByUnit(security, productMap.IdentityGuid, UNIT_TYPE.PRODUCT_MAP_PRESET_INJECTOR);
				}

				productMapCollection.Add(productMap);
				table.Rows.RemoveAt(0);
			}

			return productMapCollection;
		}

		public ProductMapCollectionClass EnumerateByAdditiveProfileGuid(SecurityClass security, PRODUCT_MAP_TYPE productMapType, Guid additiveProfileGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			SitesClass sites = new SitesClass();
			var site = sites.GetByMemberAndProcessVariables(security, security.SiteGuid, false, false);

			ProductMapClass productMap = new ProductMapClass { AdditiveProfileGuid = additiveProfileGuid, Type = productMapType };

			DataSet set;
			using (var cmd = new SqlCommand())
			{
				// Product to company maps use a special record versioning aware stored procedure
				// rather than the standard SQL 
				if (productMapType == PRODUCT_MAP_TYPE.PRODUCT_COMPANY_MAP)
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "map.usp_GetProductToCompanyMapByAdditiveProfile";
					cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier).Value = security.SiteGuid;
					cmd.Parameters.Add("@AdditiveProfileGuid", SqlDbType.UniqueIdentifier).Value = additiveProfileGuid;
				}
				else
				{
					productMap.EnumerateByAdditiveProfileGuidSQL(cmd, security.SiteGuid);
				}

				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			ProductMapCollectionClass productMapCollection = new ProductMapCollectionClass();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				productMap = new ProductMapClass(site) { Type = productMapType };
				productMap.Load(set);
				productMapCollection.Add(productMap);
				table.Rows.RemoveAt(0);
			}

			return productMapCollection;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void ModifyCollection(SecurityClass security,
												Guid identityGuid,
												string id,
												bool byAssignedGuid,
												ProductMapCollectionClass newProductMapCollection,
												ProductMapCollectionClass existingProductMapCollection)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			ProcessVariablesClass processVariables = new ProcessVariablesClass();

			if (newProductMapCollection != null)
			{
				// ReSharper disable once ForCanBeConvertedToForeach
				for (int newItem = 0; newItem < newProductMapCollection.Count; newItem++)
				{
					ProductMapClass newProductMap = newProductMapCollection[newItem];

					if (byAssignedGuid)
					{
						newProductMap.AssignedGuid = identityGuid;
						newProductMap.AssignedID = id;
					}
					else
					{
						newProductMap.AssignedToGuid = identityGuid;
						newProductMap.AssignedToID = id;
					}

					if (existingProductMapCollection != null)
					{
						int existingItem;
						for (existingItem = 0; existingItem < existingProductMapCollection.Count; existingItem++)
						{
							ProductMapClass existingProductMap = existingProductMapCollection[existingItem];
							if (existingProductMap.IdentityGuid == newProductMap.IdentityGuid
								 || (newProductMap.IdentityGuid == Guid.Empty
								 && existingProductMap.AssignedToGuid == newProductMap.AssignedToGuid
								 && existingProductMap.AssignedGuid == newProductMap.AssignedGuid
								 // We have to check to see if the meter guids are the same before allowing a simple modify. 
								 // This is because if one map has a meter assigned to it, and then you delete the map and add another, 
								 // the meter assigned to the old map won't be deleted. 
								 // Performing this check will force the old map to be deleted and the new map to be added.
								 && existingProductMap.AssignedToMeterGuid == newProductMap.AssignedToMeterGuid
								 && existingProductMap.Type == newProductMap.Type))
							{
								// ReSharper disable CompareOfFloatsByEqualityOperator
								if (existingProductMap.AssignedGuid != newProductMap.AssignedGuid
									 || existingProductMap.AssignedToGuid != newProductMap.AssignedToGuid
									 || existingProductMap.Type != newProductMap.Type
									 || existingProductMap.BlendPercentage != newProductMap.BlendPercentage
									 || existingProductMap._AdditiveRate.SIValue != newProductMap._AdditiveRate.SIValue
									 || existingProductMap.Ratio != newProductMap.Ratio
									 || existingProductMap._AdditiveCycleVolume.SIValue != newProductMap._AdditiveCycleVolume.SIValue
									 || existingProductMap.DesiredTreatRate != newProductMap.DesiredTreatRate
									 || existingProductMap.Tolerance != newProductMap.Tolerance
						  || existingProductMap.PresetNumber != newProductMap.PresetNumber
						  || existingProductMap.TankOrGroupGuid != newProductMap.TankOrGroupGuid
						  || existingProductMap.Meter.ID != newProductMap.Meter.ID
									 || existingProductMap.Meter.NumberOfDigits != newProductMap.Meter.NumberOfDigits
									 || existingProductMap.Meter.RotatesBackwardsFlag != newProductMap.Meter.RotatesBackwardsFlag
									 || existingProductMap.Meter.ReceiptMeterFlag != newProductMap.Meter.ReceiptMeterFlag
									 || existingProductMap.Meter.DcuID != newProductMap.Meter.DcuID
									 || existingProductMap.Meter.DcuBatteryVoltage != newProductMap.Meter.DcuBatteryVoltage
									 || existingProductMap.Meter.DcuBatteryCurrent != newProductMap.Meter.DcuBatteryCurrent
									 || existingProductMap.Meter.DcuTemperature != newProductMap.Meter.DcuTemperature
									 || existingProductMap.Meter.DcuResets != newProductMap.Meter.DcuResets
									 || existingProductMap.Meter.DcuUpdateDate != newProductMap.Meter.DcuUpdateDate
									 || existingProductMap.Meter.DcuConfigurationDate != newProductMap.Meter.DcuConfigurationDate
									 || existingProductMap.Meter.DcuFirmwareVersion != newProductMap.Meter.DcuFirmwareVersion
									 || existingProductMap.Meter.DcuBluetoothAddress != newProductMap.Meter.DcuBluetoothAddress
									 || existingProductMap.Meter.EntityType != newProductMap.Meter.EntityType
									 || existingProductMap.Meter.ParentEntityType != newProductMap.Meter.ParentEntityType
						  || existingProductMap.Sequence != newProductMap.Sequence
						  || existingProductMap.AdditiveProfileGuid != newProductMap.AdditiveProfileGuid
						  || existingProductMap.ShipToProductID != newProductMap.ShipToProductID
						  || existingProductMap.ShipToProductCode != newProductMap.ShipToProductCode
						  || existingProductMap.ShipToLoadRackDisplayText != newProductMap.ShipToLoadRackDisplayText
						  || existingProductMap.UnavailableInventoryGross != newProductMap.UnavailableInventoryGross
						  || existingProductMap.UnavailableInventoryNet != newProductMap.UnavailableInventoryNet
									 || existingProductMap.EnableRecipe != newProductMap.EnableRecipe
									 || existingProductMap.SpecialInstructions != newProductMap.SpecialInstructions)
								{
									newProductMap.IdentityGuid = existingProductMap.IdentityGuid;
									this.Modify(security, newProductMap);
								}
								else
								{
									processVariables.ModifyCollection(security, newProductMap.IdentityGuid, newProductMap.Permissives.Inputs, existingProductMap.Permissives.Inputs);
									processVariables.ModifyCollection(security, newProductMap.IdentityGuid, newProductMap.Permissives.Outputs, existingProductMap.Permissives.Outputs);
									processVariables.ModifyCollection(security, newProductMap.IdentityGuid, newProductMap.ProcessVariableCollection, existingProductMap.ProcessVariableCollection);
								}

								// ReSharper restore CompareOfFloatsByEqualityOperator
								break;
							}
						}

						if (existingItem == existingProductMapCollection.Count)
						{
							newProductMap.IdentityGuid = this.Add(security, newProductMap);
						}
						else
						{
							existingProductMapCollection.RemoveAt(existingItem);
						}
					}
					else
					{
						newProductMap.IdentityGuid = this.Add(security, newProductMap);
					}
				}
			}

			if (existingProductMapCollection != null)
			{
				foreach (ProductMapClass existingProductMap in existingProductMapCollection)
				{
					this.Purge(security,
						  existingProductMap.IdentityGuid,
						  existingProductMap.Type);
				}
			}
		}

		/// <summary>
		/// Determine if the meter needs to be added, updated, or deleted.
		/// If the meter needs to be added or updated, add it. If it needs to be deleted, 
		/// return the primary key of the meter so we can delete it after updating the product map record
		/// </summary>
		/// <param name="security">Security Information</param>
		/// <param name="productMap"></param>
		/// <returns>The productMap to examine meters for</returns>
		private Guid ModifyMeter(SecurityClass security, ProductMapClass productMap)
		{
			MetersClass meters = new MetersClass();

			Guid meterGuidToBeDeleted = Guid.Empty;

			// Is there a meter associated with this product map? Then determine if we need to
			// add, modify, or delete
			if (productMap.Meter != null)
			{
				MeterClass oldMeter = meters.Get(security, productMap.AssignedToMeterGuid);
				Guid newMeterIdentityGuid = Guid.Empty;

				// Only components can share meters, and the meter can only be shared for the same load arm. 
				// Only try to look for existing meters if the type of map we're modifying is a component
				if (productMap.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP || productMap.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP)
				{
					newMeterIdentityGuid = meters.GetIdentityGuidForLoadArmComponentMeter(security, productMap.Meter.ID, productMap.AssignedToGuid);
				}

				if (oldMeter != null)
				{
					// Are the meters the same or does the new meter not exist and the old meter is only assigned to one product map? Then 
					// we just have to modify the record and our work is done.
					if (oldMeter.IdentityGuid == newMeterIdentityGuid
						|| (newMeterIdentityGuid == Guid.Empty && !meters.AssignedToMoreThanOneProductMap(security, oldMeter.IdentityGuid)))
					{
						meters.Modify(security, productMap.Meter);
						return Guid.Empty;
					}
					// The ID has changed, and it now points to another existing meter in the system. 
					// Delete the old meter if it's not assigned to any other product maps
					else if (!meters.AssignedToMoreThanOneProductMap(security, oldMeter.IdentityGuid))
					{
						productMap.AssignedToMeterGuid = Guid.Empty;
						meterGuidToBeDeleted = oldMeter.IdentityGuid;
					}
				}

				// If the new meter exists, assign it to this product map as well. 
				// If the new meter doesn't exist, add it
				if (newMeterIdentityGuid != Guid.Empty)
				{
					productMap.AssignedToMeterGuid = newMeterIdentityGuid;
				}
				else
				{
					productMap.AssignedToMeterGuid = meters.Add(security, productMap.Meter);
				}
			}
			else if (productMap.AssignedToMeterGuid != Guid.Empty)
			{
				// The user has removed the meter assigned to this map. Delete the meter if it's 
				// not assigned to any other product maps.
				productMap.AssignedToMeterGuid = Guid.Empty;

				if (!meters.AssignedToMoreThanOneProductMap(security, productMap.AssignedToMeterGuid))
				{
					meterGuidToBeDeleted = productMap.AssignedToMeterGuid;
				}
			}

			return meterGuidToBeDeleted;
		}

		void IDependency.Insert(SecurityClass security, BaseDataObject Object, bool preOperation)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (Object == null)
			{
				throw new ArgumentNullException(nameof(Object));
			}

		}

		void IDependency.Update(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (Object == null)
			{
				throw new ArgumentNullException(nameof(Object));
			}

			var tankGroup = Object as TankGroupClass;
			if (tankGroup != null)
			{
				ProductMapCollectionClass productMapCollection = this.EnumerateByAssignedToGuidAndType(security, Guid.Empty, PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP);
				foreach (ProductMapClass productMap in productMapCollection)
				{
					if (productMap.TankOrGroupGuid == tankGroup.IdentityGuid
					&& productMap.AssignedGuid != tankGroup.ProductGuid)
					{
						productMap.AssignedGuid = tankGroup.ProductGuid;
						productMap.AssignedID = tankGroup.ProductID;
						productMap.UpdatedDate = DateTimeOffset.Now;
						productMap.UpdatedBy = security.UserID;
						using (SqlCommand cmd = new SqlCommand())
						{
							productMap.UpdateSQL(cmd);
							this.ConsolidatedDA.ExecuteQuery(security, cmd);
						}
					}
				}

				return;
			}

			var tank = Object as TankClass;
			if (tank != null)
			{
				ProductMapCollectionClass productMapCollection = this.EnumerateByType(security, PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP);
				productMapCollection.AddRange(this.EnumerateByType(security, PRODUCT_MAP_TYPE.PRESET_INJECTOR_MAP));
				productMapCollection.AddRange(this.EnumerateByType(security, PRODUCT_MAP_TYPE.PRESET_FLOW_CONTROLLED_ADDITIVE_MAP));
				productMapCollection.AddRange(this.EnumerateByType(security, PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP));

				foreach (ProductMapClass productMap in productMapCollection)
				{
					if (productMap.TankOrGroupGuid == tank.IdentityGuid)
					{
						if (productMap.AssignedGuid == tank.ProductGuid)
						{
							break;
						}

						if(tank.ProductGuid == Guid.Empty)
						{
							throw new Exception("Tank is assigned to an Arm and product cannot be unassigned from the Tank");
						}

						productMap.AssignedGuid = tank.ProductGuid;
						productMap.AssignedID = tank.ProductID;
						productMap.UpdatedDate = DateTimeOffset.Now;
						productMap.UpdatedBy = security.UserID;
						using (SqlCommand cmd = new SqlCommand())
						{
							productMap.UpdateSQL(cmd);
							this.ConsolidatedDA.ExecuteQuery(security, cmd);
						}
					}
				}
			}
		}

		void IDependency.Purge(SecurityClass security, BaseDataObject Object)
		{

			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (Object == null)
			{
				throw new ArgumentNullException(nameof(Object));
			}

			ProductMapCollectionClass productMapCollection;

			var product = Object as ProductClass;
			if (product != null)
			{
				foreach (PRODUCT_MAP_TYPE productMapType in ProductMapClass.GetValidProductMapTypes())
				{
					productMapCollection = this.EnumerateByAssignedGuidAndType(security, product.IdentityGuid, productMapType);
					foreach (ProductMapClass productMap in productMapCollection)
					{
						// Remove the product map
						this.Purge(security, productMap.IdentityGuid, productMap.Type);
					}
				}
			}
			else
			{
				var additiveProfile = Object as AdditiveProfileClass;
				if (additiveProfile != null)
				{
					// The deletion of ADDITIVE_PROFILE_MAP types is handled by the AdditiveProfiles.Purge method.
					// We must update any other mappings that can be associated with an additive profile. 
					// If they are associated with the additive profile being deleted, we remove the association
					List<PRODUCT_MAP_TYPE> associatedProductMapTypes = new List<PRODUCT_MAP_TYPE>
																								 {
																									  PRODUCT_MAP_TYPE.PRODUCT_COMPANY_MAP,
																									  PRODUCT_MAP_TYPE.PRODUCT_COMPANY_GROUP_MAP
																								 };

					foreach (PRODUCT_MAP_TYPE productMapType in associatedProductMapTypes)
					{
						productMapCollection = this.EnumerateByAdditiveProfileGuid(security, productMapType, additiveProfile.IdentityGuid);
						foreach (ProductMapClass productMap in productMapCollection)
						{
							if (productMap.AdditiveProfileGuid != additiveProfile.IdentityGuid)
							{
								continue;
							}

							productMap.AdditiveProfileGuid = Guid.Empty;
							productMap.UpdatedDate = DateTimeOffset.Now;
							productMap.UpdatedBy = security.UserID;
							using (SqlCommand cmd = new SqlCommand())
							{
								productMap.UpdateSQL(cmd);
								this.ConsolidatedDA.ExecuteQuery(security, cmd);
							}
						}
					}
				}
				else
				{
					var tankGroup = Object as TankGroupClass;
					if (tankGroup != null)
					{
						productMapCollection = this.EnumerateByAssignedToGuidAndType(security, Guid.Empty, PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP);
						foreach (ProductMapClass productMap in productMapCollection)
						{
							if (productMap.TankOrGroupGuid == tankGroup.IdentityGuid)
							{
								productMap.TankOrGroupGuid = Guid.Empty;
								productMap.UpdatedDate = DateTimeOffset.Now;
								productMap.UpdatedBy = security.UserID;
								using (SqlCommand cmd = new SqlCommand())
								{
									productMap.UpdateSQL(cmd);
									this.ConsolidatedDA.ExecuteQuery(security, cmd);
								}
							}
						}
					}
					else
					{
						var company = Object as CompanyClass;
						if (company != null)
						{
							foreach (ProductMapClass map in company.AuthorizedProductCollection)
							{
								using (var cmd = new SqlCommand())
								{
									map.PurgeSQL(cmd);
									this.ConsolidatedDA.ExecuteQuery(security, cmd);
								}
							}

							foreach (ProductMapClass map in company.SupplierAuthorizedProductCollection)
							{
								using (var cmd = new SqlCommand())
								{
									map.PurgeSQL(cmd);
									this.ConsolidatedDA.ExecuteQuery(security, cmd);
								}
							}

							foreach (ProductMapClass map in company.UnavailableInventoryCollection)
							{
								using (var cmd = new SqlCommand())
								{
									map.PurgeSQL(cmd);
									this.ConsolidatedDA.ExecuteQuery(security, cmd);
								}
							}
						}
					}
				}
			}
		}
	}
}
