using System;
using System.Data;
using System.Security;
using System.ServiceModel;
using System.Data.SqlClient;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessServices.DataAccessLayer;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	using FMBusinessObjects.Exceptions;

	/// <summary>
	/// Summary description for LoadArms.
	/// </summary>
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class LoadArmsClass : ILoadArms
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

	    [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, LoadArmClass loadArm)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

			if (loadArm == null)
				throw new ArgumentNullException(nameof(loadArm));

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
				throw new FMInsufficientRightsException();

			if (this.GetIdentityGuid(security,
							loadArm.BayAStationGuid,
							loadArm.BayBStationGuid,
							loadArm.BayAArmNumber,
							loadArm.BayBArmNumber) != Guid.Empty)
				throw (new Exception("Load Arm Exists"));

			loadArm.CreatedDate = DateTimeOffset.Now;
			loadArm.CreatedBy = security.UserID;
			loadArm.UpdatedDate = loadArm.CreatedDate;
			loadArm.UpdatedBy = security.UserID;

			using (SqlCommand cmd = new SqlCommand())
			{
				loadArm.IdentityGuid = Guid.NewGuid();
				loadArm.InsertSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}


			ProductMapsClass productMaps = new ProductMapsClass();
			productMaps.ModifyCollection(security, loadArm.IdentityGuid, loadArm.ID, false, loadArm.ProductRecipeCollection, null);
			productMaps.ModifyCollection(security, loadArm.IdentityGuid, loadArm.ID, false, loadArm.AdditiveInjectorCollection, null);
			productMaps.ModifyCollection(security, loadArm.IdentityGuid, loadArm.ID, false, loadArm.ComponentCollection, null);
			productMaps.ModifyCollection(security, loadArm.IdentityGuid, loadArm.ID, false, loadArm.ExternalComponentCollection, null);
            productMaps.ModifyCollection(security, loadArm.IdentityGuid, loadArm.ID, false, loadArm.FlowControlledAdditiveCollection, null);
            productMaps.ModifyCollection(security, loadArm.IdentityGuid, loadArm.ID, false, loadArm.OffloadExternalProductCollection, null);

            ProcessVariablesClass processVariables = new ProcessVariablesClass();
			processVariables.ModifyCollection(security, loadArm.IdentityGuid, loadArm.ProcessVariableCollection, null);
			processVariables.ModifyCollection(security, loadArm.IdentityGuid, loadArm.LoadArmPermissives.Inputs, null);
			processVariables.ModifyCollection(security, loadArm.IdentityGuid, loadArm.LoadArmPermissives.Outputs, null);
			processVariables.ModifyCollection(security, loadArm.IdentityGuid, loadArm.NoAdditivePermissives.Inputs, null);
			processVariables.ModifyCollection(security, loadArm.IdentityGuid, loadArm.NoAdditivePermissives.Outputs, null);

			return loadArm.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, LoadArmClass loadArm)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

			if (loadArm == null)
				throw new ArgumentNullException(nameof(loadArm));

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
			&& !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA)
			&& !security.HasRight(RIGHT.ENABLEDISABLE_STATIONS))
				throw new FMInsufficientRightsException();

			Guid identityGuid = this.GetIdentityGuid(security,
										loadArm.BayAStationGuid,
										loadArm.BayBStationGuid,
										loadArm.BayAArmNumber,
										loadArm.BayBArmNumber);
			if (identityGuid != Guid.Empty
			&& identityGuid != loadArm.IdentityGuid)
				throw (new Exception("Load Arm Exists"));

			LoadArmClass oldLoadArm = this.Get(security, loadArm.IdentityGuid);
			if (oldLoadArm.IdentityGuid == Guid.Empty)
				throw (new Exception("Load Arm Not Found"));


			loadArm.CreatedDate = DateTimeOffset.Now;
			loadArm.UpdatedBy = security.UserID;
			using (SqlCommand cmd = new SqlCommand())
			{
				loadArm.UpdateSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}



			ProductMapsClass productMaps = new ProductMapsClass();
			productMaps.ModifyCollection(security, loadArm.IdentityGuid, loadArm.ID, false, loadArm.ProductRecipeCollection, oldLoadArm.ProductRecipeCollection);
			productMaps.ModifyCollection(security, loadArm.IdentityGuid, loadArm.ID, false, loadArm.AdditiveInjectorCollection, oldLoadArm.AdditiveInjectorCollection);
			productMaps.ModifyCollection(security, loadArm.IdentityGuid, loadArm.ID, false, loadArm.ComponentCollection, oldLoadArm.ComponentCollection);
			productMaps.ModifyCollection(security, loadArm.IdentityGuid, loadArm.ID, false, loadArm.ExternalComponentCollection, oldLoadArm.ExternalComponentCollection);
            productMaps.ModifyCollection(security, loadArm.IdentityGuid, loadArm.ID, false, loadArm.FlowControlledAdditiveCollection, oldLoadArm.FlowControlledAdditiveCollection);
            productMaps.ModifyCollection(security, loadArm.IdentityGuid, loadArm.ID, false, loadArm.OffloadExternalProductCollection, oldLoadArm.OffloadExternalProductCollection);

            ProcessVariablesClass processVariables = new ProcessVariablesClass();
			processVariables.ModifyCollection(security, loadArm.IdentityGuid, loadArm.ProcessVariableCollection, oldLoadArm.ProcessVariableCollection);
			processVariables.ModifyCollection(security, loadArm.IdentityGuid, loadArm.LoadArmPermissives.Inputs, oldLoadArm.LoadArmPermissives.Inputs);
			processVariables.ModifyCollection(security, loadArm.IdentityGuid, loadArm.LoadArmPermissives.Outputs, oldLoadArm.LoadArmPermissives.Outputs);
			processVariables.ModifyCollection(security, loadArm.IdentityGuid, loadArm.NoAdditivePermissives.Inputs, oldLoadArm.NoAdditivePermissives.Inputs);
			processVariables.ModifyCollection(security, loadArm.IdentityGuid, loadArm.NoAdditivePermissives.Outputs, oldLoadArm.NoAdditivePermissives.Outputs);
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid loadArmGuid)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
				throw new FMInsufficientRightsException();

			LoadArmClass loadArm = this.Get(security, loadArmGuid);
			if (loadArm.IdentityGuid == Guid.Empty)
				throw (new Exception("Load Arm Not Found"));

			ProcessVariablesClass processVariables = new ProcessVariablesClass();
			processVariables.ModifyCollection(security, loadArm.IdentityGuid, null, loadArm.ProcessVariableCollection);
			processVariables.ModifyCollection(security, loadArm.IdentityGuid, null, loadArm.LoadArmPermissives.Inputs);
			processVariables.ModifyCollection(security, loadArm.IdentityGuid, null, loadArm.LoadArmPermissives.Outputs);
			processVariables.ModifyCollection(security, loadArm.IdentityGuid, null, loadArm.NoAdditivePermissives.Inputs);
			processVariables.ModifyCollection(security, loadArm.IdentityGuid, null, loadArm.NoAdditivePermissives.Outputs);

			ProductMapsClass productMaps = new ProductMapsClass();
			productMaps.ModifyCollection(security, loadArm.IdentityGuid, loadArm.ID, false, null, loadArm.ProductRecipeCollection);
			productMaps.ModifyCollection(security, loadArm.IdentityGuid, loadArm.ID, false, null, loadArm.AdditiveInjectorCollection);
			productMaps.ModifyCollection(security, loadArm.IdentityGuid, loadArm.ID, false, null, loadArm.ComponentCollection);
			productMaps.ModifyCollection(security, loadArm.IdentityGuid, loadArm.ID, false, null, loadArm.ExternalComponentCollection);
            productMaps.ModifyCollection(security, loadArm.IdentityGuid, loadArm.ID, false, null, loadArm.FlowControlledAdditiveCollection);
            productMaps.ModifyCollection(security, loadArm.IdentityGuid, loadArm.ID, false, null, loadArm.OffloadExternalProductCollection);

            using (SqlCommand cmd = new SqlCommand())
			{
				loadArm.PurgeSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		public LoadArmClass Get(SecurityClass security, Guid loadArmGuid)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
			&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
			&& !security.HasRight(RIGHT.VIEW_LOAD_RACK_DATA)
			&& !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA)
			&& !security.HasRight(RIGHT.ENABLEDISABLE_STATIONS))
				throw new FMInsufficientRightsException();

		    LoadArmClass loadArm = new LoadArmClass { IdentityGuid = loadArmGuid };
		    using (SqlCommand cmd = new SqlCommand())
			{
				loadArm.SelectSQL(cmd, ContextUtil.IsInTransaction);
				loadArm.Load(this.ConsolidatedDA.GetDataSet(cmd, security));
			}

			ProductMapsClass productMaps = new ProductMapsClass();
			loadArm.ProductRecipeCollection = productMaps.EnumerateByAssignedToGuidAndType(security, loadArm.IdentityGuid, PRODUCT_MAP_TYPE.PRESET_RECIPE_MAP);
			loadArm.AdditiveInjectorCollection = productMaps.EnumerateByAssignedToGuidAndType(security, loadArm.IdentityGuid, PRODUCT_MAP_TYPE.PRESET_INJECTOR_MAP);
			loadArm.ComponentCollection = productMaps.EnumerateByAssignedToGuidAndType(security, loadArm.IdentityGuid, PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP);
			loadArm.ExternalComponentCollection = productMaps.EnumerateByAssignedToGuidAndType(security, loadArm.IdentityGuid, PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP);
            loadArm.FlowControlledAdditiveCollection = productMaps.EnumerateByAssignedToGuidAndType(security, loadArm.IdentityGuid, PRODUCT_MAP_TYPE.PRESET_FLOW_CONTROLLED_ADDITIVE_MAP);
            loadArm.OffloadExternalProductCollection = productMaps.EnumerateByAssignedToGuidAndType(security, loadArm.IdentityGuid, PRODUCT_MAP_TYPE.OFFLOAD_EXTERNAL_METER_MAP);

            // Merge the ComponentTankGroupCollection ordering by Preset Number
            ProductMapCollectionClass componentTankGroupCollection = productMaps.EnumerateByAssignedToGuidAndType(security, loadArm.IdentityGuid, PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP);
			foreach (ProductMapClass componentTankGroup in componentTankGroupCollection)
			{
				bool inserted = false;

				foreach (ProductMapClass componentTank in loadArm.ComponentCollection)
				{

					if (componentTank.PresetNumber > componentTankGroup.PresetNumber)
					{
						loadArm.ComponentCollection.Insert(loadArm.ComponentCollection.IndexOf(componentTank), componentTankGroup);
						inserted = true;
						break;
					}
				}

				if (!inserted)
					loadArm.ComponentCollection.Add(componentTankGroup);
			}

            // Merge the ComponentTankGroupCollection ordering by Preset Number
            ProductMapCollectionClass externalComponentTankGroupCollection = productMaps.EnumerateByAssignedToGuidAndType(security, loadArm.IdentityGuid, PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP);
            foreach (ProductMapClass externalComponentTankGroup in externalComponentTankGroupCollection)
            {
                bool inserted = false;

                foreach (ProductMapClass externalComponentTank in loadArm.ExternalComponentCollection)
                {

                    if (externalComponentTank.PresetNumber > externalComponentTankGroup.PresetNumber)
                    {
                        loadArm.ExternalComponentCollection.Insert(loadArm.ExternalComponentCollection.IndexOf(externalComponentTank), externalComponentTankGroup);
                        inserted = true;
                        break;
                    }
                }

                if (!inserted)
                    loadArm.ExternalComponentCollection.Add(externalComponentTankGroup);
            }

            ProcessVariablesClass processVariables = new ProcessVariablesClass();
			loadArm.ProcessVariableCollection = processVariables.EnumerateByUnit(security, loadArm.IdentityGuid, UNIT_TYPE.LOADARM_UNIT);
			loadArm.LoadArmPermissives.Inputs = processVariables.EnumerateByUnit(security, loadArm.IdentityGuid, loadArm.LoadArmPermissives.InputUnitType);
			loadArm.LoadArmPermissives.Outputs = processVariables.EnumerateByUnit(security, loadArm.IdentityGuid, loadArm.LoadArmPermissives.OutputUnitType);
			loadArm.NoAdditivePermissives.Inputs = processVariables.EnumerateByUnit(security, loadArm.IdentityGuid, loadArm.NoAdditivePermissives.InputUnitType);
			loadArm.NoAdditivePermissives.Outputs = processVariables.EnumerateByUnit(security, loadArm.IdentityGuid, loadArm.NoAdditivePermissives.OutputUnitType);

			return loadArm;
		}

		public Guid GetIdentityGuid(SecurityClass security,
									Guid bayAStationGuid,
									Guid bayBStationGuid,
									int bayAArmNumber,
									int bayBArmNumber)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
			&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
			&& !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA)
			&& !security.HasRight(RIGHT.ENABLEDISABLE_STATIONS))
				throw new FMInsufficientRightsException();

		    LoadArmClass loadArm = new LoadArmClass
		                               {
		                                   BayAStationGuid = bayAStationGuid,
		                                   BayBStationGuid = bayBStationGuid,
		                                   BayAArmNumber = bayAArmNumber,
		                                   BayBArmNumber = bayBArmNumber
		                               };

		    using (SqlCommand cmd = new SqlCommand())
			{
				loadArm.SelectByStationGuidsAndArmNumbersSQL(cmd, ContextUtil.IsInTransaction);
				loadArm.Load(this.ConsolidatedDA.GetDataSet(cmd, security));
			}
			return loadArm.IdentityGuid;
		}

		public LoadArmCollectionClass EnumerateByStationGuid(SecurityClass security, string swingArmPosition, Guid stationGuid)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));

			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
			&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
			&& !security.HasRight(RIGHT.VIEW_LOAD_RACK_DATA)
			&& !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA)
			&& !security.HasRight(RIGHT.ENABLEDISABLE_STATIONS))
				throw new FMInsufficientRightsException();

			LoadArmClass loadArm = new LoadArmClass();
			
			DataSet set;
			using (SqlCommand cmd = new SqlCommand())
			{
				loadArm.EnumerateByStationGuidSQL(cmd, (swingArmPosition == "A"), stationGuid, ContextUtil.IsInTransaction);
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}


			LoadArmCollectionClass loadArmCollection = new LoadArmCollectionClass();
			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				loadArm = new LoadArmClass();
				loadArm.Load(set);

				loadArm = this.Get(security, loadArm.IdentityGuid);

				loadArmCollection.Add(loadArm);
				table.Rows.RemoveAt(0);
			}

			return loadArmCollection;
		}
	}
}
