namespace FMBusinessServices.ServiceClasses
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.ServiceModel;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Exceptions;

    using FMBusinessServices.DataAccessLayer;
    using FMBusinessServices.InternalClasses;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    using IsolationLevel = System.Transactions.IsolationLevel;

    /// <summary>
	/// Summary description for TanksClass.
	/// </summary>
	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = IsolationLevel.ReadCommitted)]
	public class TanksClass : ITanks, IDependency
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

	    protected void Validate(TankClass tank)
		{
			if (tank.ID == "")
			{
				throw new Exception("ID Required");
			}

			if (tank.ID == "{None}"
			|| tank.ID == "{Unassigned}"
			|| tank.ID == "{All}")
			{
				throw new Exception("ID is reserved key word " + tank.ID);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, TankClass tank)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (tank == null)
			{
				throw new ArgumentNullException(nameof(tank));
			}

			if (!security.HasRight(RIGHT.MODIFY_TANK_DATA) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

		    this.Validate(tank);

			if (Guid.Empty != this.GetIdentityGuid(security, tank.ID))
			{
				throw new Exception("Tank Exists");
			}

			tank.SiteGuid = security.SiteGuid;
			tank.CreatedDate = DateTimeOffset.Now;
			tank.CreatedBy = security.UserID;
			tank.UpdatedDate = tank.CreatedDate;
			tank.UpdatedBy = security.UserID;
			tank.IdentityGuid = Guid.NewGuid();

			using (SqlCommand cmd = new SqlCommand())
			{
				tank.InsertSQL(cmd);
			    this.ConsolidatedDA.ExecuteQuery(security, cmd);

				ProcessVariablesClass processVariables = new ProcessVariablesClass();
				processVariables.ModifyCollection(security, tank.IdentityGuid, tank.ProcessVariableCollection, null);			
			}

			// Add any meters associated with this tank, 
			// and a record to map.tblMeterToTank to indicate the relationship between the meter and tank.
			foreach (MeterClass meter in tank.Meters)
			{
				MetersClass meters = new MetersClass();

				meters.Add(security, meter);
				meters.AddTankMap(security, meter, tank.IdentityGuid);
			}

			return tank.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, TankClass tank)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (tank == null)
			{
				throw new ArgumentNullException(nameof(tank));
			}

			if (!security.HasRight(RIGHT.MODIFY_TANK_DATA) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) 
				&& !security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS) && !security.HasRight(RIGHT.MODIFY_QUALITY_TESTS))
			{
				throw new FMInsufficientRightsException();
			}

		    this.Validate(tank);

			// Verify ID does not exist
			Guid identityGuid = this.GetIdentityGuid(security, tank.ID);
			if (identityGuid != Guid.Empty
			&& identityGuid != tank.IdentityGuid)
			{
				throw new Exception("Tank Exists");
			}

			TankClass oldTank = this.Get(security, tank.IdentityGuid);
			if (oldTank.IdentityGuid == Guid.Empty)
			{
				throw new Exception("Tank Not Found");
			}


			// Temporariy logic to remove TANK_OPERATION_PV for tanks created prior to this feature
			foreach (ProcessVariableClass processVariable in oldTank.ProcessVariableCollection)
			{
				if (processVariable.ProcessVariableType == PROCESS_VARIABLE_TYPE.TANK_OPERATION_PV
				&& processVariable.IdentityGuid == Guid.Empty)
				{
					oldTank.ProcessVariableCollection.Remove(processVariable);
					break;
				}
			}

			// Temporariy logic to remove TANK_VAPOR_PRESSURE_PV for tanks created prior to this feature
			foreach (ProcessVariableClass processVariable in oldTank.ProcessVariableCollection)
			{
				if (processVariable.ProcessVariableType == PROCESS_VARIABLE_TYPE.VAPOR_PRESSURE_PV
				&& processVariable.IdentityGuid == Guid.Empty)
				{
					oldTank.ProcessVariableCollection.Remove(processVariable);
					break;
				}
			}

			// Temporariy logic to remove AVAILABLE_NET_VOLUME_PV for tanks created prior to this feature
			foreach (ProcessVariableClass processVariable in oldTank.ProcessVariableCollection)
			{
				if (processVariable.ProcessVariableType == PROCESS_VARIABLE_TYPE.AVAILABLE_NET_VOLUME_PV
					&& processVariable.IdentityGuid == Guid.Empty)
				{
					oldTank.ProcessVariableCollection.Remove(processVariable);
					break;
				}
			}

			// Temporariy logic to remove REMAINING_NET_VOLUME_PV for tanks created prior to this feature
			foreach (ProcessVariableClass processVariable in oldTank.ProcessVariableCollection)
			{
				if (processVariable.ProcessVariableType == PROCESS_VARIABLE_TYPE.REMAINING_NET_VOLUME_PV
					&& processVariable.IdentityGuid == Guid.Empty)
				{
					oldTank.ProcessVariableCollection.Remove(processVariable);
					break;
				}
			}

			// Temporariy logic to remove TANK_STATUS_PV for tanks created prior to this feature
			foreach (ProcessVariableClass processVariable in oldTank.ProcessVariableCollection)
			{
				if (processVariable.ProcessVariableType == PROCESS_VARIABLE_TYPE.TANK_STATUS_PV
				&& processVariable.IdentityGuid == Guid.Empty)
				{
					oldTank.ProcessVariableCollection.Remove(processVariable);
					break;
				}
			}

			DependenciesClass dependencies = new DependenciesClass(security);
			dependencies.Update(security, tank);

			tank.UpdatedDate = DateTimeOffset.Now;
			tank.UpdatedBy = security.UserID;
			using (SqlCommand cmd = new SqlCommand())
			{
				tank.UpdateSQL(cmd);
			    this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			ProcessVariablesClass processVariables = new ProcessVariablesClass();
			processVariables.ModifyCollection(security, tank.IdentityGuid, tank.ProcessVariableCollection, oldTank.ProcessVariableCollection);

		    this.UpdateMeters(security, tank, oldTank);
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid tankGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.MODIFY_TANK_DATA) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			TankClass tank = this.Get(security, tankGuid);
			if (tank.IdentityGuid == Guid.Empty)
			{
				throw new Exception("Tank Exists");
			}

			// Purge Dependencies
			DependenciesClass dependencies = new DependenciesClass(security);
			dependencies.Purge(security, tank);

			ProcessVariablesClass processVariables = new ProcessVariablesClass();
			processVariables.ModifyCollection(security, tank.IdentityGuid, null, tank.ProcessVariableCollection);

		    this.UpdateMeters(security, null, tank);

			using (SqlCommand cmd = new SqlCommand())
			{
				tank.PurgeSQL(cmd);
			    this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}			
		}

		public TankClass Get(SecurityClass security, Guid tankGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_TANK_DATA)
				&& !security.HasRight(RIGHT.MODIFY_TANK_DATA)
				&& !security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
			    && !security.HasRight(RIGHT.VIEW_INVENTORY_DATA)
				&& !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA)
			    && !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
			    && !security.HasRight(RIGHT.VIEW_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS)
			    && !security.HasRight(RIGHT.MODIFY_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.MODIFY_DISPATCH)
			    && !security.HasRight(RIGHT.VIEW_DISPATCH)
				&& !security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD)
				&& !security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD)
				&& !security.HasRight(RIGHT.VIEW_MAINTENANCE_RECORD)
                && !security.HasRight(RIGHT.VIEW_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.CREATE_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.VIEW_ORDERS)
                && !security.HasRight(RIGHT.MODIFY_ORDERS)
                && !security.HasRight(RIGHT.CREATE_ORDERS)
                && !security.HasRight(RIGHT.VIEW_MAPS))
			{
				throw new FMInsufficientRightsException();
			}

		    TankClass tank = new TankClass { IdentityGuid = tankGuid };
		    using (SqlCommand cmd = new SqlCommand())
			{
				tank.SelectSQL(cmd, ContextUtil.IsInTransaction);
				tank.Load(this.ConsolidatedDA.GetDataSet(cmd, security));
			}

			if (tank.IdentityGuid != Guid.Empty)
			{
				MetersClass meters = new MetersClass();
				tank.Meters = meters.EnumerateByTank(security, tank.IdentityGuid);
			}

			ProcessVariablesClass processVariables = new ProcessVariablesClass();
			if (tank.IdentityGuid != Guid.Empty)
			{
				tank.ProcessVariableCollection = processVariables.EnumerateByUnit(security, tank.IdentityGuid, UNIT_TYPE.TANK_UNIT);
			}

			// Temporary logid to add TANK_STATUS_PV for tanks create prior to this feature
			bool foundTankStatus = false;
			foreach (ProcessVariableClass processVariable in tank.ProcessVariableCollection)
			{
				if (processVariable.ProcessVariableType == PROCESS_VARIABLE_TYPE.TANK_STATUS_PV)
				{
					foundTankStatus = true;
					break;
				}
			}

			if (!foundTankStatus)
			{
				ProcessVariableClass levelPv = tank.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.LEVEL_PV];
			    ProcessVariableClass tankStatusPv = new ProcessVariableClass(
			        PROCESS_VARIABLE_TYPE.TANK_STATUS_PV,
			        UNIT_TYPE.TANK_UNIT,
			        VarEnum.VT_BSTR,
			        true,
			        "",
			        "",
			        "") { UnitGuid = tank.IdentityGuid };
			    tankStatusPv.SetMaximum("", 0);
				tankStatusPv.SetMinimum("", 0);
				if (levelPv?.OPCItemID != null)
				{
					tankStatusPv.ProgID = levelPv.ProgID;
					tankStatusPv.URL = levelPv.URL;
					tankStatusPv.OPCItemID = levelPv.OPCItemID.Replace("Level", "Tank Status");
				}
				tank.ProcessVariableCollection.Add(tankStatusPv);

				tankStatusPv.IdentityGuid = processVariables.Add(security, tankStatusPv);
			}

			// Temporary logic to add TANK_VAPOR_PRESSURE_PV for tanks created prior to this feature
			bool foundTankVaporPressure = false;
			foreach (ProcessVariableClass processVariable in tank.ProcessVariableCollection)
			{
				if (processVariable.ProcessVariableType == PROCESS_VARIABLE_TYPE.VAPOR_PRESSURE_PV)
				{
					foundTankVaporPressure = true;
					break;
				}
			}

			if (!foundTankVaporPressure)
			{
				ProcessVariableClass levelPv = tank.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.LEVEL_PV];
			    ProcessVariableClass vaporPressurePv = new ProcessVariableClass(
			        PROCESS_VARIABLE_TYPE.VAPOR_PRESSURE_PV,
			        UNIT_TYPE.TANK_UNIT,
			        VarEnum.VT_R8,
			        true,
			        "",
			        "",
			        "") { DataType = VarEnum.VT_R8, UnitGuid = tank.IdentityGuid, ServerUnits = EngineeringUnit.FmpPsi };
			    vaporPressurePv.SetMaximum(20.0, EngineeringUnit.FmpPsi);
				vaporPressurePv.SetMinimum(0.0, EngineeringUnit.FmpPsi);
				if (levelPv?.OPCItemID != null)
				{
					vaporPressurePv.ProgID = levelPv.ProgID;
					vaporPressurePv.URL = levelPv.URL;
					vaporPressurePv.OPCItemID = levelPv.OPCItemID.Replace("Level", "Vapor Press P3");
				}
				tank.ProcessVariableCollection.Add(vaporPressurePv);

				vaporPressurePv.IdentityGuid = processVariables.Add(security, vaporPressurePv);
			}



			// Temporariy logic to add TANK_OPERATION_PV for tanks created prior to this feature
			bool foundTankOperation = false;
			foreach (ProcessVariableClass processVariable in tank.ProcessVariableCollection)
			{
				if (processVariable.ProcessVariableType == PROCESS_VARIABLE_TYPE.TANK_OPERATION_PV)
				{
					foundTankOperation = true;
					break;
				}
			}

			if (!foundTankOperation)
			{
			    ProcessVariableClass tankOperationPv = new ProcessVariableClass(
			        PROCESS_VARIABLE_TYPE.TANK_OPERATION_PV,
			        UNIT_TYPE.TANK_UNIT,
			        VarEnum.VT_BSTR,
			        true,
			        "",
			        "",
			        "") { DataType = VarEnum.VT_BSTR, UnitGuid = tank.IdentityGuid };
			    tankOperationPv.SetMaximum("", 0);
				tankOperationPv.SetMinimum("", 0);
				tank.ProcessVariableCollection.Add(tankOperationPv);

				tankOperationPv.IdentityGuid = processVariables.Add(security, tankOperationPv);
			}

			// Temporariy logic to add AVAILABLE_NET_VOLUME_PV & REMAINING_NET_VOLUME_PV
			// for tanks created prior to this feature
			bool foundAvailableNetVolume = false;
			foreach (ProcessVariableClass processVariable in tank.ProcessVariableCollection)
			{
				if (processVariable.ProcessVariableType == PROCESS_VARIABLE_TYPE.AVAILABLE_NET_VOLUME_PV)
				{
					foundAvailableNetVolume = true;
					break;
				}
			}

			if (!foundAvailableNetVolume)
			{
				ProcessVariableClass availableGrossVolumePv = tank.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.AVAILABLE_GROSS_VOLUME_PV];

			    ProcessVariableClass availableNetVolumePv =
			        new ProcessVariableClass(
			            PROCESS_VARIABLE_TYPE.AVAILABLE_NET_VOLUME_PV,
			            UNIT_TYPE.TANK_UNIT,
			            VarEnum.VT_BSTR,
			            true,
			            "",
			            "",
			            "") { DataType = VarEnum.VT_R8, UnitGuid = tank.IdentityGuid, ServerUnits = EngineeringUnit.FmvUsGal };
			    availableNetVolumePv.SetMaximum(10000.0, EngineeringUnit.FmvUsGal);
				availableNetVolumePv.SetMinimum(0.0, EngineeringUnit.FmvUsGal);
				if (availableGrossVolumePv.OPCItemID != "")
				{
					availableNetVolumePv.ServerUnits = availableGrossVolumePv.ServerUnits;
					availableNetVolumePv.SetMaximum(availableGrossVolumePv.GetMaximum(EngineeringUnit.FmvUsGal, 0), EngineeringUnit.FmvUsGal);
					availableNetVolumePv.ProgID = availableGrossVolumePv.ProgID;
					availableNetVolumePv.URL = availableGrossVolumePv.URL;
					availableNetVolumePv.OPCItemID = availableGrossVolumePv.OPCItemID.Replace("Vol. Available Gross", "Volume Available Net");
				}

				tank.ProcessVariableCollection.Add(availableNetVolumePv);

				ProcessVariableClass remainingGrossVolumePv = tank.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.REMAINING_GROSS_VOLUME_PV];

			    ProcessVariableClass remainingNetVolumePv =
			        new ProcessVariableClass(
			            PROCESS_VARIABLE_TYPE.REMAINING_NET_VOLUME_PV,
			            UNIT_TYPE.TANK_UNIT,
			            VarEnum.VT_BSTR,
			            true,
			            "",
			            "",
			            "") { DataType = VarEnum.VT_R8, UnitGuid = tank.IdentityGuid, ServerUnits = EngineeringUnit.FmvUsGal };
			    remainingNetVolumePv.SetMaximum(10000.0, EngineeringUnit.FmvUsGal);
				remainingNetVolumePv.SetMinimum(0.0, EngineeringUnit.FmvUsGal);
				if (availableGrossVolumePv.OPCItemID != "")
				{
					remainingNetVolumePv.ServerUnits = remainingGrossVolumePv.ServerUnits;
					remainingNetVolumePv.SetMaximum(remainingGrossVolumePv.GetMaximum(EngineeringUnit.FmvUsGal, 0), EngineeringUnit.FmvUsGal);
					remainingNetVolumePv.ProgID = remainingGrossVolumePv.ProgID;
					remainingNetVolumePv.URL = remainingGrossVolumePv.URL;
					remainingNetVolumePv.OPCItemID = remainingGrossVolumePv.OPCItemID.Replace("Vol. Remaining Gross", "Volume Remaining Net");
				}

				tank.ProcessVariableCollection.Add(remainingNetVolumePv);

				availableNetVolumePv.IdentityGuid = processVariables.Add(security, availableNetVolumePv);
				remainingNetVolumePv.IdentityGuid = processVariables.Add(security, remainingNetVolumePv);
			}

			return tank;
		}

		public Guid GetIdentityGuid(SecurityClass security, string id)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_TANK_DATA)
				&& !security.HasRight(RIGHT.MODIFY_TANK_DATA)
				&& !security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
			    && !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA)
				&& !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
			    && !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
				&& !security.HasRight(RIGHT.VIEW_QUALITY_TESTS)
			    && !security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.MODIFY_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.ADD_QUALITYTAG_RECORD)
				&& !security.HasRight(RIGHT.VIEW_QUALITYTAG_RECORD)
				&& !security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD)
			    && !security.HasRight(RIGHT.MODIFY_DISPATCH)
				&& !security.HasRight(RIGHT.VIEW_DISPATCH)
                && !security.HasRight(RIGHT.VIEW_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.CREATE_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.VIEW_ORDERS)
                && !security.HasRight(RIGHT.MODIFY_ORDERS)
                && !security.HasRight(RIGHT.CREATE_ORDERS)
                && !security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD)
				&& !security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD)
				&& !security.HasRight(RIGHT.VIEW_MAINTENANCE_RECORD))
			{
				throw new FMInsufficientRightsException();
			}

		    TankClass tank = new TankClass { ID = id, SiteGuid = security.SiteGuid };
		    using (SqlCommand cmd = new SqlCommand())
			{
				tank.SelectByIDSQL(cmd, ContextUtil.IsInTransaction);
				tank.Load(this.ConsolidatedDA.GetDataSet(cmd, security));
			}

			return tank.IdentityGuid;
		}

		public DataSet EnumerateForPhysicalInventory(SecurityClass security, bool hideHiddenTanks = false)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_TANK_DATA) && !security.HasRight(RIGHT.MODIFY_TANK_DATA)
				&& !security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.VIEW_INVENTORY_DATA)
				&& !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA)
				&& !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
				&& !security.HasRight(RIGHT.VIEW_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.MODIFY_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.ADD_QUALITYTAG_RECORD)
				&& !security.HasRight(RIGHT.VIEW_QUALITYTAG_RECORD)
				&& !security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD)
				&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
				&& !security.HasRight(RIGHT.MODIFY_DISPATCH)
				&& !security.HasRight(RIGHT.VIEW_DISPATCH)
				&& !security.HasRight(RIGHT.VIEW_SUPPLY_ORDERS)
				&& !security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS)
				&& !security.HasRight(RIGHT.CREATE_SUPPLY_ORDERS)
				&& !security.HasRight(RIGHT.VIEW_ORDERS)
				&& !security.HasRight(RIGHT.MODIFY_ORDERS)
				&& !security.HasRight(RIGHT.CREATE_ORDERS)
				&& !security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD)
				&& !security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD)
				&& !security.HasRight(RIGHT.VIEW_MAINTENANCE_RECORD))
			{
				throw new FMInsufficientRightsException();
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandText = 
					@"select t.TankGuid, t.TankId, p.ProductGuid, p.ProductId
from dbo.tblTanks t
left join dbo.tblProducts p on t.ProductGuid = p.ProductGuid
where t.SiteGuid = @SiteGuid
order by t.TankId";
				cmd.Parameters.AddWithValue("@SiteGuid", security.SiteGuid);
				DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);

				return set;
			}
		}

		public TankCollectionClass Enumerate(SecurityClass security, bool hideHiddenTanks = false )
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_TANK_DATA) && !security.HasRight(RIGHT.MODIFY_TANK_DATA)
				&& !security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.VIEW_INVENTORY_DATA)
				&& !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA)
				&& !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
				&& !security.HasRight(RIGHT.VIEW_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.MODIFY_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.ADD_QUALITYTAG_RECORD)
				&& !security.HasRight(RIGHT.VIEW_QUALITYTAG_RECORD)
				&& !security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD)
				&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
				&& !security.HasRight(RIGHT.MODIFY_DISPATCH)
				&& !security.HasRight(RIGHT.VIEW_DISPATCH)
                && !security.HasRight(RIGHT.VIEW_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.CREATE_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.VIEW_ORDERS)
                && !security.HasRight(RIGHT.MODIFY_ORDERS)
                && !security.HasRight(RIGHT.CREATE_ORDERS)
                && !security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD)
				&& !security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD)
				&& !security.HasRight(RIGHT.VIEW_MAINTENANCE_RECORD))
			{
				throw new FMInsufficientRightsException();
			}

		    TankClass tank = new TankClass { SiteGuid = security.SiteGuid };
		    using (SqlCommand cmd = new SqlCommand())
			{
				tank.EnumerateSQL(cmd, hideHiddenTanks);
				DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);
				TankCollectionClass tankCollection = new TankCollectionClass();

				DataTable table = set.Tables[0];
				while (table.Rows.Count != 0)
				{
					tank = new TankClass();
					tank.Load(set);
					tankCollection.Add(tank);
					table.Rows.RemoveAt(0);
				}

				return tankCollection;
			}
		}

        public TankCollectionClass EnumerateAuthorized(SecurityClass security, bool hideHiddenTanks = false)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            if (!security.HasRight(RIGHT.VIEW_TANK_DATA) && !security.HasRight(RIGHT.MODIFY_TANK_DATA)
                && !security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
                && !security.HasRight(RIGHT.VIEW_INVENTORY_DATA)
                && !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA)
                && !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
                && !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
                && !security.HasRight(RIGHT.VIEW_QUALITY_TESTS)
                && !security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS)
                && !security.HasRight(RIGHT.MODIFY_QUALITY_TESTS)
                && !security.HasRight(RIGHT.ADD_QUALITYTAG_RECORD)
                && !security.HasRight(RIGHT.VIEW_QUALITYTAG_RECORD)
                && !security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD)
                && !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
                && !security.HasRight(RIGHT.MODIFY_DISPATCH)
                && !security.HasRight(RIGHT.VIEW_DISPATCH)
                && !security.HasRight(RIGHT.VIEW_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.CREATE_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.VIEW_ORDERS)
                && !security.HasRight(RIGHT.MODIFY_ORDERS)
                && !security.HasRight(RIGHT.CREATE_ORDERS)
                && !security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD)
                && !security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD)
                && !security.HasRight(RIGHT.VIEW_MAINTENANCE_RECORD))
            {
                throw new FMInsufficientRightsException();
            }

            TankClass tank = new TankClass { SiteGuid = security.SiteGuid };
            using (SqlCommand cmd = new SqlCommand())
            {
                tank.EnumerateAuthorizedSqlCmd(security, hideHiddenTanks);
                DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);
                TankCollectionClass tankCollection = new TankCollectionClass();

                DataTable table = set.Tables[0];
                while (table.Rows.Count != 0)
                {
                    tank = new TankClass();
                    tank.Load(set);
                    tankCollection.Add(tank);
                    table.Rows.RemoveAt(0);
                }

                return tankCollection;
            }
        }

        /// <summary>
        /// This method will return all tanks that have coordinates and base on
        /// the current site.
        /// </summary>
        /// <param name="security">The security object.</param>
        /// <returns>Returns a collection of tanks that have coordinates.</returns>
        public TankCollectionClass EnumerateWhereCoordinatesExist(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (security.HasRight(RIGHT.VIEW_MAPS) == false)
			{
				throw new FMInsufficientRightsException();
			}

			using (var sqlCommand = new SqlCommand())
			{
				var tankClass = new TankClass();
				tankClass.EnumerateWhereCoordinatesExistSql(sqlCommand, security.SiteGuid);

				DataSet dataSet = this.ConsolidatedDA.GetDataSet(sqlCommand, security);

				if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
				{
					return null;
				}

				var tankCollection = new TankCollectionClass();

				DataTable table = dataSet.Tables[0];

				while (table.Rows.Count != 0)
				{
					tankClass = new TankClass();
					tankClass.Load(dataSet);
					tankCollection.Add(tankClass);

					table.Rows.RemoveAt(0);
				}

				return tankCollection;
			}
		}

		public TankCollectionClass EnumerateTanksWithoutQualityTag(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
				&& !security.HasRight(RIGHT.VIEW_DISPATCH)
				&& !security.HasRight(RIGHT.ADD_QUALITYTAG_RECORD)
				&& !security.HasRight(RIGHT.VIEW_QUALITYTAG_RECORD)
				&& !security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD))
			{
				throw new FMInsufficientRightsException();
			}

		    TankClass tank = new TankClass { SiteGuid = security.SiteGuid };
		    using (SqlCommand cmd = new SqlCommand())
			{
				tank.EnumerateTanksWithoutQualityTagSQL(cmd, security);
				DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);
				TankCollectionClass tankCollection = new TankCollectionClass();

				DataTable table = set.Tables[0];
				while (table.Rows.Count != 0)
				{
					tank = new TankClass();
					tank.Load(set);
					tankCollection.Add(tank);
					table.Rows.RemoveAt(0);
				}

				return tankCollection;
			}
		}

	    /// <summary>
	    /// This method will return the tanks by a filter criterion.
	    /// </summary>
	    /// <param name="security"></param>
	    /// <param name="filter"></param>
	    /// <param name="hideHiddenTanks">If true, only tanks that are not marked has hidden will be returned</param>
	    /// <returns></returns>
	    public TankCollectionClass EnumerateByFilter(SecurityClass security, string filter, bool hideHiddenTanks = false)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_TANK_DATA) && !security.HasRight(RIGHT.MODIFY_TANK_DATA)
				&& !security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.VIEW_INVENTORY_DATA)
				&& !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA)
				&& !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
				&& !security.HasRight(RIGHT.MODIFY_DISPATCH)
				&& !security.HasRight(RIGHT.VIEW_DISPATCH))
			{
				throw new FMInsufficientRightsException();
			}

	        TankClass tank = new TankClass { SiteGuid = security.SiteGuid };
	        using (SqlCommand cmd = new SqlCommand())
			{
				tank.EnumerateByFilterSQL(cmd, filter, hideHiddenTanks);
				DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);
				TankCollectionClass tankCollection = new TankCollectionClass();

				DataTable table = set.Tables[0];
				while (table.Rows.Count != 0)
				{
					tank = new TankClass();
					tank.Load(set);
					tankCollection.Add(tank);
					table.Rows.RemoveAt(0);
				}

				return tankCollection;
			}
		}

        public TankCollectionClass EnumerateByProduct(SecurityClass security, Guid productGuid, bool hideHiddenTanks = false)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_TANK_DATA) && !security.HasRight(RIGHT.MODIFY_TANK_DATA)
				&& !security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
			    && !security.HasRight(RIGHT.VIEW_PRODUCTS) && !security.HasRight(RIGHT.MODIFY_PRODUCTS)
			    && !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA) && !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA)
			    && !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING) && !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
			    && !security.HasRight(RIGHT.MODIFY_DISPATCH) && !security.HasRight(RIGHT.VIEW_DISPATCH)
                && !security.HasRight(RIGHT.VIEW_SUPPLY_ORDERS) && !security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.CREATE_SUPPLY_ORDERS) && !security.HasRight(RIGHT.VIEW_ORDERS)
                && !security.HasRight(RIGHT.MODIFY_ORDERS) && !security.HasRight(RIGHT.CREATE_ORDERS)
                && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

            TankClass tank = new TankClass { ProductGuid = productGuid };
            using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "usp_EnumerateTankByProduct";
				cmd.Parameters.Clear();
				cmd.Parameters.AddWithValue("@ProductGuid", productGuid);
				cmd.Parameters.AddWithValue("@SiteGuid", security.SiteGuid);
				cmd.Parameters.AddWithValue("@HideHiddenTanks", hideHiddenTanks);

				DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);
				TankCollectionClass tankCollection = new TankCollectionClass();

				DataTable table = set.Tables[0];
				while (table.Rows.Count != 0)
				{
					tank = new TankClass();
					tank.Load(set);
					tankCollection.Add(tank);
					table.Rows.RemoveAt(0);
				}

				return tankCollection;
			}
		}

	    /// <summary>
	    /// This method will return tanks by product and filter.
	    /// </summary>
	    /// <param name="security"></param>
	    /// <param name="productGuid"></param>
	    /// <param name="filter"></param>
	    /// <param name="hideHiddenTanks">If true, only tanks not marked as hidden will be returned</param>
	    /// <returns></returns>
	    public TankCollectionClass EnumerateByProductAndFilter(SecurityClass security, Guid productGuid, string filter, bool hideHiddenTanks = false)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_TANK_DATA) && !security.HasRight(RIGHT.MODIFY_TANK_DATA)
				&& !security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
			    && !security.HasRight(RIGHT.VIEW_PRODUCTS) && !security.HasRight(RIGHT.MODIFY_PRODUCTS)
			    && !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA) && !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
			    && !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) && !security.HasRight(RIGHT.MODIFY_DISPATCH)
                && !security.HasRight(RIGHT.VIEW_SUPPLY_ORDERS) && !security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.CREATE_SUPPLY_ORDERS) && !security.HasRight(RIGHT.VIEW_ORDERS)
                && !security.HasRight(RIGHT.MODIFY_ORDERS) && !security.HasRight(RIGHT.CREATE_ORDERS)
                && !security.HasRight(RIGHT.VIEW_DISPATCH))
			{
				throw new FMInsufficientRightsException();
			}

	        TankClass tank = new TankClass { ProductGuid = productGuid };
	        using (SqlCommand cmd = new SqlCommand())
			{
                tank.EnumerateByProductAndFilterSQL(cmd, security, filter, hideHiddenTanks: hideHiddenTanks);
				DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);
				TankCollectionClass tankCollection = new TankCollectionClass();

				DataTable table = set.Tables[0];
				while (table.Rows.Count != 0)
				{
					tank = new TankClass();
					tank.Load(set);
					tankCollection.Add(tank);
					table.Rows.RemoveAt(0);
				}

				return tankCollection;
			}
		}

		public TankCollectionClass EnumerateByManager(SecurityClass security, Guid managerGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_TANK_DATA) && !security.HasRight(RIGHT.MODIFY_TANK_DATA)
				&& !security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
			    && !security.HasRight(RIGHT.VIEW_COMPANY_DATA) && !security.HasRight(RIGHT.MODIFY_COMPANY_DATA)
			    && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

		    TankClass tank = new TankClass { ManagerGuid = managerGuid };
		    using (SqlCommand cmd = new SqlCommand())
			{
				tank.EnumerateByManagerSQL(cmd);
				DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);
				TankCollectionClass tankCollection = new TankCollectionClass();

				DataTable table = set.Tables[0];
				while (table.Rows.Count != 0)
				{
					tank = new TankClass();
					tank.Load(set);
					tankCollection.Add(tank);
					table.Rows.RemoveAt(0);
				}

				return tankCollection;
			}
		}

        /// <summary>
        /// Get only basic information like the ID and TankGuid for all tanks for a specified site
        /// </summary>
        /// <param name="security">Contains Security information</param>
        /// <returns>A collection of tanks for the specified site with only basic information populated</returns>
		public TankCollectionClass EnumerateBasicInformation(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_TANK_DATA) && !security.HasRight(RIGHT.MODIFY_TANK_DATA)
				&& !security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.VIEW_MAINTENANCE_RECORD) && !security.HasRight(RIGHT.VIEW_COMPANY_DATA)
			    && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA) && !security.HasRight(RIGHT.MODIFY_COMPANY_DATA)
				&& !security.HasRight(RIGHT.IMPORT_TRANSACTION))
			{
				throw new FMInsufficientRightsException();
			}
		
			using (SqlCommand cmd = new SqlCommand())
			{
                TankClass tank = new TankClass();
                tank.EnumerateBasicInformation(security, cmd);
				DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);
				TankCollectionClass tankCollection = new TankCollectionClass();

			    if (set == null || set.Tables.Count <= 0)
			    {
			        return tankCollection;
			    }

                DataTable table = set.Tables[0];

			    while (table.Rows.Count != 0)
			    {
			        tank = new TankClass();
			        DataRow row = table.Rows[0];
			        tank.LoadBasicInformation(row);
			        tankCollection.Add(tank);
			        table.Rows.RemoveAt(0);
			    }

			    return tankCollection;
			}
		}

		/// <summary>
		/// Get only basic information like the ID and TankGuid for all tanks for a specified site
		/// </summary>
		/// <param name="security">Contains Security information</param>
		/// <param name="assetTrackingDeviceId">The asset tracking ID that the tank is linked to.</param>
		/// <returns>A collection of tanks for the specified site with only basic information populated</returns>
		public TankCollectionClass EnumerateBasicInfoLinkedToAssetTrackingDevices(SecurityClass security, string assetTrackingDeviceId)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_TANK_DATA) 
				&& !security.HasRight(RIGHT.MODIFY_TANK_DATA)
				&& !security.HasRight(RIGHT.VIEW_ASSET_TRACKING_DEVICES) 
				&& !security.HasRight(RIGHT.MODIFY_ASSET_TRACKING_DEVICES))
			{
				throw new FMInsufficientRightsException();
			}

			using (SqlCommand sqlCommand = new SqlCommand())
			{
				var tankCollection	= new TankCollectionClass();
				var tank			= new TankClass();

				tank.EnumerateBasicInfoLinkedToAssetTrackingDevicesSQL(sqlCommand, assetTrackingDeviceId);
				DataSet dataSet = this.ConsolidatedDA.GetDataSet(sqlCommand, security);

				if (dataSet == null || dataSet.Tables.Count <= 0 || dataSet.Tables[0].Rows.Count <= 0)
				{
					return tankCollection;
				}

				DataTable table = dataSet.Tables[0];

				foreach (DataRow row in table.Rows)
				{
					tank = new TankClass();
					tank.LoadBasicInformation(row);
					tankCollection.Add(tank);
				}

				return tankCollection;
			}
		}

		/// <summary>
		/// This method will return the number of tank configuration number being used based on the asset
		/// tracking device GUID and tank configuration number.
		/// </summary>
		/// <param name="security">Contains Security information</param>
		/// <param name="tankGuid">Current Tank GUID.</param>
		/// <param name="assetTrackingDeviceGuid">The asset tracking GUID that the tank is linked to.</param>
		/// <param name="tankConfigurationNumber">The tank configuration number.</param>
		/// <returns>Returns the number of tank configuration number being used for the selected number and device.</returns>
		public int TankConfigurationNumberBeingUsed(SecurityClass security, Guid tankGuid, Guid assetTrackingDeviceGuid, int tankConfigurationNumber)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_TANK_DATA)
				&& !security.HasRight(RIGHT.MODIFY_TANK_DATA)
				&& !security.HasRight(RIGHT.VIEW_ASSET_TRACKING_DEVICES)
				&& !security.HasRight(RIGHT.MODIFY_ASSET_TRACKING_DEVICES))
			{
				throw new FMInsufficientRightsException();
			}

			using (SqlCommand sqlCommand = new SqlCommand())
			{
				int tankConfigurationNumberBeingUsed = 0;
				var tank = new TankClass();

				tank.TankConfigurationNumberBeingUsedSQL(sqlCommand, tankGuid, assetTrackingDeviceGuid, tankConfigurationNumber);
				DataSet dataSet = this.ConsolidatedDA.GetDataSet(sqlCommand, security);

				if (dataSet == null || dataSet.Tables.Count <= 0 || dataSet.Tables[0].Rows.Count <= 0)
				{
					return tankConfigurationNumberBeingUsed;
				}

				DataTable table = dataSet.Tables[0];
				DataRow row = table.Rows[0];

				tankConfigurationNumberBeingUsed = row.IsNull("UseCount") ? 0 : (int)row["UseCount"];

				return tankConfigurationNumberBeingUsed;
			}
		}

		/// <summary>
		/// Compare the existing tank's meters to the tank we are adding, updating, or deleting
		/// and add, update, or delete metes appropriately.
		/// </summary>
		/// <param name="security">Security Information</param>
		/// <param name="tank"> The tank that is being updated, inserted, or deleted</param>
		/// <param name="oldTank"> The tank as it existed in the database before the user's action</param>
		private void UpdateMeters(SecurityClass security, TankClass tank, TankClass oldTank)
		{
			MetersClass meters = new MetersClass();

			// If the new tank parameter was null, that means we're deleting the tank.
			// If it's not null, that means we may have to add, update, or delete meters.
			if (tank != null)
			{
				foreach (MeterClass meter in tank.Meters)
				{
					if (oldTank != null)
					{
						// If the meter belonging to the new tank has no identity guid, 
						// it is new and needs to be added to the database
						if (meter.IdentityGuid == Guid.Empty)
						{
							meters.Add(security, meter);
							meters.AddTankMap(security, meter, tank.IdentityGuid);
						}
						else
						{
							// If we can find a meter with the same identity guid in the old tank's set of meters, 
							// the meter needs to be updated. After the update we remove it from the old tank so we know not to delete it later
							foreach (MeterClass oldMeter in oldTank.Meters)
							{
								if (oldMeter.IdentityGuid == meter.IdentityGuid)
								{
									meters.Modify(security, meter);
									oldTank.Meters.Remove(oldMeter);
									break;
								}
							}
						}
					}				
				}
			}

			// Delete any meters that are still present on the old tank object, 
			// because they weren't found in the new tank.
			if (oldTank != null)
			{
				foreach (MeterClass oldMeter in oldTank.Meters)
				{
					meters.Purge(security, oldMeter.IdentityGuid);
				}
			}
		}

		public ProcessVariableCollectionClass GetProcessVariables(SecurityClass security, Guid tankGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}
			if (tankGuid == Guid.Empty)
			{
				throw new ArgumentException($"Must supply a non-zero {nameof(tankGuid)}", nameof(tankGuid));
			}

			if (!security.HasRight(RIGHT.VIEW_TANK_DATA)
				&& !security.HasRight(RIGHT.MODIFY_TANK_DATA)
				&& !security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.VIEW_INVENTORY_DATA)
				&& !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA)
				&& !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
				&& !security.HasRight(RIGHT.VIEW_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.MODIFY_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.MODIFY_DISPATCH)
				&& !security.HasRight(RIGHT.VIEW_DISPATCH)
				&& !security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD)
				&& !security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD)
				&& !security.HasRight(RIGHT.VIEW_MAINTENANCE_RECORD)
				&& !security.HasRight(RIGHT.VIEW_SUPPLY_ORDERS)
				&& !security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS)
				&& !security.HasRight(RIGHT.CREATE_SUPPLY_ORDERS)
				&& !security.HasRight(RIGHT.VIEW_ORDERS)
				&& !security.HasRight(RIGHT.MODIFY_ORDERS)
				&& !security.HasRight(RIGHT.CREATE_ORDERS)
				&& !security.HasRight(RIGHT.VIEW_MAPS))
			{
				throw new FMInsufficientRightsException();
			}

			ProcessVariableCollectionClass processVariableCollection = new ProcessVariableCollectionClass();
			ProcessVariablesClass processVariables = new ProcessVariablesClass();
			processVariableCollection = processVariables.EnumerateByUnit(security, tankGuid, UNIT_TYPE.TANK_UNIT);
			return processVariableCollection;
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

			if (preOperation && Object is EntityToSiteMapClass)
			{
				EntityToSiteMapClass entityToSiteMap = (EntityToSiteMapClass)Object;

				if ( entityToSiteMap.TypeID != ENTITY_TYPE.TANK )
				{
					return;
				}

				if ( Guid.Empty != this.GetIdentityGuid( security, entityToSiteMap.ID ) )
				{
					throw new Exception("Tank Exists - " + entityToSiteMap.ID);
				}
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

			// Deleted/Undelete Groups
			if (Object is SiteClass)
			{
				TankCollectionClass tankCollection = this.Enumerate(security);
				foreach (TankClass tank in tankCollection)
				{
				    this.Purge(security, tank.IdentityGuid);
				}
				return;
			}

		    var entityToSiteMap = Object as EntityToSiteMapClass;
		    if (entityToSiteMap != null)
			{
				if ( entityToSiteMap.TypeID == ENTITY_TYPE.PRODUCT )
				{
					TankCollectionClass tankCollection = this.EnumerateByProduct(security, entityToSiteMap.IdentityGuid);
					foreach (TankClass tank in tankCollection)
					{
						tank.Load(this.Get(security, tank.IdentityGuid));
						tank.ProductGuid = Guid.Empty;
						tank.ProductID = "{None}";
					    this.Modify(security, tank);
					}
					return;
				}

				if ( entityToSiteMap.TypeID == ENTITY_TYPE.COMPANY )
				{
					TankCollectionClass tankCollection = this.EnumerateByManager(security, entityToSiteMap.IdentityGuid);
					foreach (TankClass tank in tankCollection)
					{
						tank.Load(this.Get(security, tank.IdentityGuid));
						tank.ManagerGuid = Guid.Empty;
						tank.ManagerID = "{None}";
					    this.Modify(security, tank);
					}
				}
			}
		}
	}
}