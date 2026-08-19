using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.Web;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessServices.DataAccessLayer;
using FMBusinessServices.InternalClasses;
using System.Data.SqlClient;

namespace FMBusinessServices.ServiceClasses
{
	using FMBusinessObjects.Exceptions;

	/// <summary>
	/// Summary description for ProcessVariablesClass.
	/// </summary>
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class ProcessVariablesClass : IDependency, IProcessVariables
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		public ProcessVariablesClass()
		{
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, ProcessVariableClass ProcessVariable)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (ProcessVariable == null)
				throw new ArgumentNullException("ProcessVariable");

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) && !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA)
			    && !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) && !security.HasRight(RIGHT.MODIFY_COMPANY_DATA)
			    && !security.HasRight(RIGHT.MODIFY_PRODUCTS) && !security.HasRight(RIGHT.MODIFY_TANK_DATA)
			    && !security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS) && !security.HasRight(RIGHT.MODIFY_QUALITY_TESTS)
			    && !security.HasRight(RIGHT.MODIFY_DISPATCH) && !security.HasRight(RIGHT.VIEW_DISPATCH)
			    && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA) && !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			if (Guid.Empty != this.GetIdentityGuid(security,
									ProcessVariable.ProcessVariableType,
									ProcessVariable.InstanceNumber,
									ProcessVariable.UnitGuid,
									ProcessVariable.UnitType))
				throw (new Exception("Process Variable Exists"));

			ProcessVariable.SiteGuid = security.SiteGuid;
			ProcessVariable.CreatedDate = DateTimeOffset.Now;
			ProcessVariable.CreatedBy = security.UserID;
			ProcessVariable.UpdatedDate = ProcessVariable.CreatedDate;
			ProcessVariable.UpdatedBy = security.UserID;
			ProcessVariable.IdentityGuid = Guid.NewGuid();

			// if URL is non NULL then get Client Guid
			if (ProcessVariable.URL != "")
			{
				OPCConnectionsClass OPCConnections = new OPCConnectionsClass();
				ProcessVariable.OPCConnectionGuid = OPCConnections.GetIdentityGuid(security, ProcessVariable.URL);
				if (ProcessVariable.OPCConnectionGuid == Guid.Empty)
				{
					OPCConnectionClass OPCConnection = new OPCConnectionClass();
					OPCConnection.URL = ProcessVariable.URL;
					OPCConnection.ProgID = ProcessVariable.ProgID;
					ProcessVariable.OPCConnectionGuid = OPCConnections.Add(security, OPCConnection);
				}
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				ProcessVariable.InsertSQLCmd(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			return ProcessVariable.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, DATA_TYPE Type, ProcessVariableClass ProcessVariable)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (ProcessVariable == null)
				throw new ArgumentNullException("ProcessVariable");

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) && !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA)
			    && !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) && !security.HasRight(RIGHT.MODIFY_COMPANY_DATA)
			    && !security.HasRight(RIGHT.MODIFY_PRODUCTS) && !security.HasRight(RIGHT.MODIFY_TANK_DATA)
			    && !security.HasRight(RIGHT.ENABLEDISABLE_STATIONS) && !security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS)
			    && !security.HasRight(RIGHT.MODIFY_QUALITY_TESTS) && !security.HasRight(RIGHT.MODIFY_DISPATCH)
			    && !security.HasRight(RIGHT.VIEW_DISPATCH) && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA)
			    && !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			Guid oldOPCGuid = Guid.Empty;

			if (Type != DATA_TYPE.DYNAMIC)
			{
				// Verify does not exist
				Guid identityGuid = this.GetIdentityGuid(security,
											ProcessVariable.ProcessVariableType,
											ProcessVariable.InstanceNumber,
											ProcessVariable.UnitGuid,
											ProcessVariable.UnitType);

				if (identityGuid != Guid.Empty
				&& identityGuid != ProcessVariable.IdentityGuid)
					throw (new Exception("Process Variable Exists"));


				ProcessVariableClass OldProcessVariable = this.Get(security, ProcessVariable.IdentityGuid, ProcessVariable.UnitType);
				if (OldProcessVariable.IdentityGuid == Guid.Empty)
					throw (new Exception("Process Variable Not Found"));

				Guid OPCConnectionGuid = Guid.Empty;
				OPCConnectionsClass OPCConnections = new OPCConnectionsClass();
				if (ProcessVariable.URL != "")
				{
					OPCConnectionGuid = OPCConnections.GetIdentityGuid(security, ProcessVariable.URL);
					if (OPCConnectionGuid == Guid.Empty)
					{
						OPCConnectionClass OPCConnection = new OPCConnectionClass();
						OPCConnection.URL = ProcessVariable.URL;
						OPCConnection.ProgID = ProcessVariable.ProgID;
						OPCConnectionGuid = OPCConnections.Add(security, OPCConnection);
					}
				}

				// indicates a change in assignment
				if (ProcessVariable.OPCConnectionGuid != Guid.Empty
				&& ProcessVariable.OPCConnectionGuid != OPCConnectionGuid)
				{
					oldOPCGuid = ProcessVariable.OPCConnectionGuid;
				}

				ProcessVariable.OPCConnectionGuid = OPCConnectionGuid;

			}

			ProcessVariable.UpdatedDate = DateTimeOffset.Now;
			ProcessVariable.UpdatedBy = security.UserID;
			using (SqlCommand cmd = ProcessVariable.UpdateSQLCmd(Type))
			{
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			if (oldOPCGuid != Guid.Empty)
			{
				this.PurgeOPCIfNotReferenced(security, oldOPCGuid);
			}
		}


		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid ProcessVariableGuid, UNIT_TYPE targetUnitType)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) && !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA)
			    && !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) && !security.HasRight(RIGHT.MODIFY_COMPANY_DATA)
			    && !security.HasRight(RIGHT.MODIFY_PRODUCTS) && !security.HasRight(RIGHT.MODIFY_TANK_DATA)
			    && !security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS) && !security.HasRight(RIGHT.MODIFY_QUALITY_TESTS)
			    && !security.HasRight(RIGHT.MODIFY_DISPATCH) && !security.HasRight(RIGHT.VIEW_DISPATCH)
			    && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA) && !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			ProcessVariableClass ProcessVariable = this.Get(security, ProcessVariableGuid, targetUnitType);
			if (ProcessVariable.IdentityGuid == Guid.Empty)
				throw (new Exception("Process Variable Not Found"));

			Guid oldOPCGUid = ProcessVariable.OPCConnectionGuid;
			using (SqlCommand cmd = ProcessVariable.PurgeSQLCmd)
			{
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			if (oldOPCGUid != Guid.Empty)
			{
				this.PurgeOPCIfNotReferenced(security, oldOPCGUid);
			}

		}


		public ProcessVariableClass Get(SecurityClass security, Guid ProcessVariableGuid, UNIT_TYPE targetUnitType)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
			&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
			&& !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA)
			&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
			&& !security.HasRight(RIGHT.MODIFY_COMPANY_DATA)
			&& !security.HasRight(RIGHT.MODIFY_PRODUCTS)
			&& !security.HasRight(RIGHT.MODIFY_TANK_DATA) && !security.HasRight(RIGHT.VIEW_TANK_DATA)
			&& !security.HasRight(RIGHT.ENABLEDISABLE_STATIONS)
			&& !security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS)
			&& !security.HasRight(RIGHT.MODIFY_QUALITY_TESTS)
			&& !security.HasRight(RIGHT.VIEW_QUALITY_TESTS)
			&& !security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS)
			&& !security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA)
			&& !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
			&& !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA)
			&& !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
			&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
			&& !security.HasRight(RIGHT.MODIFY_FUEL_CARD_DATA)
			&& !security.HasRight(RIGHT.ADD_QUALITYTAG_RECORD)
			&& !security.HasRight(RIGHT.VIEW_QUALITYTAG_RECORD)
			&& !security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD)
			&& !security.HasRight(RIGHT.VIEW_DISPATCH)
			&& !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA)
			&& !security.HasRight(RIGHT.MODIFY_DISPATCH)
			&& !security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD)
			&& !security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD)
			&& !security.HasRight(RIGHT.VIEW_MAINTENANCE_RECORD)
			&& !security.HasRight(RIGHT.VIEW_ASSET_TRACKING_DEVICES)
			&& !security.HasRight(RIGHT.MODIFY_ASSET_TRACKING_DEVICES))
			{
				throw new FMInsufficientRightsException();
			}

			ProcessVariableClass ProcessVariable = new ProcessVariableClass();
			ProcessVariable.IdentityGuid = ProcessVariableGuid;
			ProcessVariable.UnitType = targetUnitType;
			using (SqlCommand cmd = ProcessVariable.SelectSQLCmd(ContextUtil.IsInTransaction))
			{
				ProcessVariable.Load(this.ConsolidatedDA.GetDataSet(cmd, security));
			}
			return ProcessVariable;
		}


		public Guid GetIdentityGuid(SecurityClass security,
									PROCESS_VARIABLE_TYPE ProcessVariableType,
									int InstanceNumber,
									Guid UnitGuid,
									UNIT_TYPE UnitType)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
			&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
			&& !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA)
			&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
			&& !security.HasRight(RIGHT.MODIFY_COMPANY_DATA)
			&& !security.HasRight(RIGHT.MODIFY_PRODUCTS)
			&& !security.HasRight(RIGHT.MODIFY_TANK_DATA) && !security.HasRight(RIGHT.VIEW_TANK_DATA)
			&& !security.HasRight(RIGHT.ENABLEDISABLE_STATIONS)
			&& !security.HasRight(RIGHT.VIEW_QUALITY_TESTS)
			&& !security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS)
			&& !security.HasRight(RIGHT.MODIFY_QUALITY_TESTS)
			&& !security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA)
			&& !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
			&& !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA)
			&& !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
			&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
			&& !security.HasRight(RIGHT.MODIFY_FUEL_CARD_DATA)
			&& !security.HasRight(RIGHT.ADD_QUALITYTAG_RECORD)
			&& !security.HasRight(RIGHT.VIEW_QUALITYTAG_RECORD)
			&& !security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD)
			&& !security.HasRight(RIGHT.MODIFY_DISPATCH)
			&& !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA)
			&& !security.HasRight(RIGHT.VIEW_DISPATCH)
			&& !security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD)
			&& !security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD)
			&& !security.HasRight(RIGHT.VIEW_MAINTENANCE_RECORD)
			&& !security.HasRight(RIGHT.VIEW_ASSET_TRACKING_DEVICES)
			&& !security.HasRight(RIGHT.MODIFY_ASSET_TRACKING_DEVICES))
			{
				throw new FMInsufficientRightsException();
			}

			ProcessVariableClass ProcessVariable = new ProcessVariableClass();
			ProcessVariable.ProcessVariableType = ProcessVariableType;
			ProcessVariable.InstanceNumber = InstanceNumber;
			ProcessVariable.UnitGuid = UnitGuid;
			ProcessVariable.UnitType = UnitType;
			ProcessVariable.SiteGuid = security.SiteGuid;
			using (SqlCommand cmd = ProcessVariable.SelectByTypeInstanceUnitSQLCmd(ContextUtil.IsInTransaction))
			{
				ProcessVariable.Load(this.ConsolidatedDA.GetDataSet(cmd, security));
			}
			return ProcessVariable.IdentityGuid;
		}

		public ProcessVariableCollectionClass EnumerateByUnit(SecurityClass security,
																					Guid UnitGuid,
																					UNIT_TYPE targetUnitType)
		{
			// No security check here, all users must be able to get site which
			// includes Site Process Variables
			ProcessVariableClass ProcessVariable = new ProcessVariableClass();
			ProcessVariable.UnitGuid = UnitGuid;
			ProcessVariable.UnitType = targetUnitType;
			ProcessVariable.SiteGuid = security.SiteGuid;

			DataSet Set = null;
			using (SqlCommand cmd = ProcessVariable.EnumerateByUnitSQLCmd(ContextUtil.IsInTransaction))
			{
				Set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}
			ProcessVariableCollectionClass ProcessVariableCollection = new ProcessVariableCollectionClass();

			DataTable Table = Set.Tables[0];
			while (Table.Rows.Count != 0)
			{
				ProcessVariable = new ProcessVariableClass();
				ProcessVariable.UnitType = targetUnitType;
				ProcessVariable.Load(Set);
				ProcessVariableCollection.Add(ProcessVariable);
				Table.Rows.RemoveAt(0);
			}

			return ProcessVariableCollection;
		}

		/// <summary>
		/// This is used by Modify and Purge method, when no more references are found, it deletes the OPCConnection record.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="targetGuid"></param>
		private void PurgeOPCIfNotReferenced(SecurityClass security, Guid targetGuid)
		{
			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
			&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
				throw new FMInsufficientRightsException();


			ProcessVariableClass ProcessVariable = new ProcessVariableClass();
			ProcessVariable.OPCConnectionGuid = targetGuid;
			ProcessVariable.SiteGuid = security.SiteGuid;
			using (SqlCommand cmd = ProcessVariable.FindOPCConnectionReferenceCount(ContextUtil.IsInTransaction))
			{
				int referenceCount = (int)this.ConsolidatedDA.GetDataSet(cmd, security).Tables[0].Rows[0][0];
				if (referenceCount == 0)
				{
					(new OPCConnectionsClass()).Purge(security, targetGuid);
				}
			}

		}

		public ProcessVariableCollectionClass EnumerateByMessageApplicationStringGuid(SecurityClass security, Guid MessageGuid, UNIT_TYPE unitType)
		{
			ProcessVariableClass ProcessVariable = new ProcessVariableClass();
			ProcessVariable.MessageApplicationStringGuid = MessageGuid;
			ProcessVariable.UnitType = unitType;

			DataSet Set = null;
			using (SqlCommand cmd = ProcessVariable.EnumerateByMessageApplicationStringGuidSQLCmd)
			{
				Set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}
			ProcessVariableCollectionClass ProcessVariableCollection = new ProcessVariableCollectionClass();

			DataTable Table = Set.Tables[0];
			while (Table.Rows.Count != 0)
			{
				ProcessVariable = new ProcessVariableClass();
				ProcessVariable.UnitType = unitType;
				ProcessVariable.Load(Set);
				ProcessVariableCollection.Add(ProcessVariable);
				Table.Rows.RemoveAt(0);
			}

			return ProcessVariableCollection;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void ModifyCollection(SecurityClass security,
												Guid guid,
												ProcessVariableCollectionClass NewProcessVariableCollection,
												ProcessVariableCollectionClass ExistingProcessVariableCollection)
		{
			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) && !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA)
			    && !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) && !security.HasRight(RIGHT.MODIFY_COMPANY_DATA)
			    && !security.HasRight(RIGHT.MODIFY_PRODUCTS) && !security.HasRight(RIGHT.MODIFY_TANK_DATA)
			    && !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT) && !security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES)
			    && !security.HasRight(RIGHT.ENABLEDISABLE_STATIONS) && !security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS)
			    && !security.HasRight(RIGHT.MODIFY_QUALITY_TESTS) && !security.HasRight(RIGHT.MODIFY_DISPATCH)
			    && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA) && !security.HasRight(RIGHT.VIEW_DISPATCH)
			    && !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			if (NewProcessVariableCollection != null)
			{
				if (NewProcessVariableCollection.Equals(ExistingProcessVariableCollection))
					return;

				foreach (ProcessVariableClass ProcessVariable in NewProcessVariableCollection)
				{
					ProcessVariable.UnitGuid = guid;

					if (ExistingProcessVariableCollection != null)
					{
						int Item;
						for (Item = 0; Item < ExistingProcessVariableCollection.Count; Item++)
						{
							ProcessVariableClass ExistingProcessVariable = ExistingProcessVariableCollection[Item];
							if (ExistingProcessVariable.IdentityGuid == ProcessVariable.IdentityGuid

							// case where new variable is added and the Instance Numbers are resequenced.
							|| (ExistingProcessVariable.IdentityGuid != Guid.Empty
							&& ExistingProcessVariable.ProcessVariableType == ProcessVariable.ProcessVariableType
							&& ExistingProcessVariable.InstanceNumber == ProcessVariable.InstanceNumber))
							{
								if (ExistingProcessVariable.IdentityGuid != ProcessVariable.IdentityGuid
								|| ExistingProcessVariable.ProcessVariableType != ProcessVariable.ProcessVariableType
								|| ExistingProcessVariable.InstanceNumber != ProcessVariable.InstanceNumber
								|| ExistingProcessVariable.UnitGuid != ProcessVariable.UnitGuid
								|| ExistingProcessVariable.UnitType != ProcessVariable.UnitType
								|| ExistingProcessVariable.OPCItemID != ProcessVariable.OPCItemID
								|| ExistingProcessVariable.DataType != ProcessVariable.DataType
								|| ExistingProcessVariable.ServerUnits != ProcessVariable.ServerUnits
								|| ExistingProcessVariable.OPCQuality != ProcessVariable.OPCQuality
								|| ExistingProcessVariable.SIValue != ProcessVariable.SIValue
								|| ExistingProcessVariable.DateTimeStamp != ProcessVariable.DateTimeStamp
								|| ExistingProcessVariable.siMaximum != ProcessVariable.siMaximum
								|| ExistingProcessVariable.siMinimum != ProcessVariable.siMinimum
								|| ExistingProcessVariable.DataTypeEnabled != ProcessVariable.DataTypeEnabled
								|| ExistingProcessVariable.Input != ProcessVariable.Input
								|| ExistingProcessVariable.InputEnabled != ProcessVariable.InputEnabled
								|| ExistingProcessVariable.MessageApplicationStringGuid != ProcessVariable.MessageApplicationStringGuid
								|| ExistingProcessVariable.URL != ProcessVariable.URL)
								{
									ProcessVariable.IdentityGuid = ExistingProcessVariable.IdentityGuid;
									this.Modify(security, DATA_TYPE.CONFIG, ProcessVariable);
									if (!ProcessVariable.Input)
										this.Modify(security, DATA_TYPE.DYNAMIC, ProcessVariable);
								}
								break;
							}
						}

						if (Item == ExistingProcessVariableCollection.Count)
							this.Add(security, ProcessVariable);
						else
							ExistingProcessVariableCollection.Remove(Item);
					}
					else
						this.Add(security, ProcessVariable);
				}
			}

			if (ExistingProcessVariableCollection != null)
			{
				foreach (ProcessVariableClass ProcessVariable in ExistingProcessVariableCollection)
				{
					this.Purge(security, ProcessVariable.IdentityGuid, ProcessVariable.UnitType);
				}
			}
		}

		public bool ProcessVariableAlreadyUsed(SecurityClass security, ProcessVariableClass pv)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (pv == null)
			{
				throw new ArgumentNullException("pv");
			}

			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
			&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
			&& !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA)
			&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
			&& !security.HasRight(RIGHT.MODIFY_COMPANY_DATA)
			&& !security.HasRight(RIGHT.MODIFY_PRODUCTS)
			&& !security.HasRight(RIGHT.ENABLEDISABLE_STATIONS)
			&& !security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS)
			&& !security.HasRight(RIGHT.MODIFY_QUALITY_TESTS))
			{
				throw new UnauthorizedAccessException("Access Denied");
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				pv.FindByUrlAndItemSql(cmd, pv.URL, pv.OPCItemID, pv.IdentityGuid);
				DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);
				if (set != null && set.Tables.Count > 0)
				{
					if (set.Tables[0].Rows.Count > 0)
					{
						return true;
					}
				}
			}

			return false;
		}

		#region IDependency Members

		void IDependency.Insert(SecurityClass security, BaseDataObject Object, bool preOperation)
		{
		}

		void IDependency.Update(SecurityClass security, BaseDataObject Object)
		{
		}

		void IDependency.Purge(SecurityClass security, BaseDataObject Object)
		{
			if (typeof(ApplicationStringClass).IsInstanceOfType(Object))
			{
				ApplicationStringClass ApplicationString = (ApplicationStringClass)Object;

				ArrayList unitTypes = ProcessVariableClass.GetProcessVariableUnitTypes();

				foreach (UNIT_TYPE unitType in unitTypes)
				{
					ProcessVariableCollectionClass PVCollection = this.EnumerateByMessageApplicationStringGuid(security, ApplicationString.IdentityGuid, unitType);
					foreach (ProcessVariableClass PV in PVCollection)
					{
						PV.MessageApplicationStringGuid = Guid.Empty;
						PV.MessageID = "";
						this.Modify(security, DATA_TYPE.CONFIG, PV);
					}
				}
			}
		}

		#endregion
	}

}
