namespace FMBusinessServices.ServiceClasses
{
	using crypto;
	using DataAccessLayer;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.UtilityObjects;
	using InternalClasses;
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Linq;
	using System.Security;
	using System.ServiceModel;

	/// <summary>
	/// Summary description for Stations.
	/// </summary>
	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class StationsClass : IStations, IDependency
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

	    protected void Validate(StationClass station)
		{
			if (station.ID.Length == 0)
			{
				throw (new Exception("ID Required"));
			}

			if (station.ID == "{None}" || station.ID == "{Unassigned}" || station.ID == "{All}")
			{
				throw new Exception("ID is reserved key word " + station.ID);
			}

			foreach (LoadArmClass loadArm in station.LoadArmCollection)
			{
				if (loadArm.PresetType == PRESET_TYPE.MAX_PRESET_TYPE)
				{
					throw (new Exception("Invalid Load Arm Preset Type"));
				}
			}
		}

		protected Dictionary<Guid, string> UpdateLoadArms(SecurityClass security, StationClass station)
		{
			var loadArms = new LoadArmsClass();
			
			Dictionary<Guid, string> SwingArmStationDict = new Dictionary<Guid, String>();

			LoadArmCollectionClass existingLoadArmCollection = loadArms.EnumerateByStationGuid(security, station.SwingArmPosition, station.IdentityGuid);

			if (station.LoadArmCollection != null)
			{
				foreach (LoadArmClass loadArm in station.LoadArmCollection)
				{
					if (loadArm.IdentityGuid == Guid.Empty)
					{
						if (station.SwingArmPosition == "A")
						{
							loadArm.BayAStationGuid = station.IdentityGuid;
							loadArm.BayAStationID = station.ID;
						}
						else
						{
							loadArm.BayBStationGuid = station.IdentityGuid;
							loadArm.BayBStationID = station.ID;
						}
						loadArms.Add(security, loadArm);
					}
					else
					{
						for (int item = 0; item < existingLoadArmCollection.Count; item++)
						{
							LoadArmClass existingLoadArm = existingLoadArmCollection[item];
							
							if (existingLoadArm.IdentityGuid == loadArm.IdentityGuid)
							{
								// If a StationGuid changes the ArmNumber sequence may need to be
								// corrected in the old station
								if (existingLoadArm.BayAStationGuid != loadArm.BayAStationGuid)
								{
									if (existingLoadArm.BayAStationGuid != Guid.Empty)
									{
										StationClass bayAStation = this.Get(security, existingLoadArm.BayAStationGuid);
										int armNumber = 1;

										foreach (LoadArmClass bayALoadArm in bayAStation.LoadArmCollection)
										{
											if (bayALoadArm.IdentityGuid == existingLoadArm.IdentityGuid)
											{
												continue;
											}

											if (bayALoadArm.BayAArmNumber != armNumber)
											{
												// Determine if this LoadArm is part of the new LoadArmCollection
												// and if so apply the change to it.
												bool found = false;

												foreach (LoadArmClass newLoadArm in station.LoadArmCollection)
												{
													if (newLoadArm.IdentityGuid == bayALoadArm.IdentityGuid)
													{
														newLoadArm.BayAArmNumber = armNumber;

														if (newLoadArm.BayBArmNumber < loadArm.BayBArmNumber)
														{
															loadArms.Modify(security, newLoadArm);
														}

														found = true;
														break;
													}
												}

												if (!found)
												{
													bayALoadArm.BayAArmNumber = armNumber;
													loadArms.Modify(security, bayALoadArm);
												}
											}

											armNumber++;
										}
									}

									if (loadArm.BayAStationGuid != Guid.Empty)
									{
										StationClass bayAStation = this.Get(security, loadArm.BayAStationGuid);
										loadArm.BayAArmNumber = bayAStation.LoadArmCollection.Count + 1;
									}
								}

								if (existingLoadArm.BayBStationGuid != loadArm.BayBStationGuid)
								{
									if (existingLoadArm.BayBStationGuid != Guid.Empty)
									{
										StationClass bayBStation = this.Get(security, existingLoadArm.BayAStationGuid);
										int armNumber = 1;

										foreach (LoadArmClass bayBLoadArm in bayBStation.LoadArmCollection)
										{
											if (bayBLoadArm.IdentityGuid == existingLoadArm.IdentityGuid)
											{
												continue;
											}

											if (bayBLoadArm.BayBArmNumber != armNumber)
											{
												// Determine if this LoadArm is part of the new LoadArmCollection
												// and if so apply the change to it.
												bool found = false;

												foreach (LoadArmClass newLoadArm in station.LoadArmCollection)
												{
													if (newLoadArm.IdentityGuid == bayBLoadArm.IdentityGuid)
													{
														newLoadArm.BayBArmNumber = armNumber;

														if (newLoadArm.BayAArmNumber < loadArm.BayAArmNumber)
														{
															loadArms.Modify(security, newLoadArm);
														}
														
														found = true;
														break;
													}
												}

												if (!found)
												{
													bayBLoadArm.BayBArmNumber = armNumber;
													loadArms.Modify(security, bayBLoadArm);
												}
											}

											armNumber++;
										}
									}

									if (loadArm.BayBStationGuid != Guid.Empty)
									{
										StationClass bayBStation = this.Get(security, loadArm.BayBStationGuid);
										loadArm.BayBArmNumber = bayBStation.LoadArmCollection.Count + 1;
									}
								}

								existingLoadArmCollection.RemoveAt(item);
								break;
							}
						}

						// Possibly the Arms have been resequenced in which case
						// an existing arm may need to be purged and added back
						Guid identityGuid = loadArms.GetIdentityGuid(security,
																loadArm.BayAStationGuid,
																loadArm.BayBStationGuid,
																loadArm.BayAArmNumber,
																loadArm.BayBArmNumber);

						if (identityGuid != Guid.Empty && identityGuid != loadArm.IdentityGuid)
						{
							loadArms.Purge(security, identityGuid);
							int item = 0;

							foreach (LoadArmClass existingLoadArm in existingLoadArmCollection)
							{
								if (existingLoadArm.IdentityGuid == identityGuid)
								{
									existingLoadArmCollection.RemoveAt(item);
									break;
								}

								item++;
							}

							item = 0;

							foreach (LoadArmClass newLoadArm in station.LoadArmCollection)
							{
								if (newLoadArm.IdentityGuid == identityGuid)
								{
									newLoadArm.IdentityGuid = Guid.Empty;
									break;
								}

								item++;
							}
						}

						loadArms.Modify(security, loadArm);
					}

					//Get partner station guid, if it is a swing arm
					if(loadArm.SwingArm)
					{
						if (station.SwingArmPosition == "A")
						{
							if (!SwingArmStationDict.ContainsKey(loadArm.BayBStationGuid))
							{
								SwingArmStationDict.Add(loadArm.BayBStationGuid, loadArm.BayBStationID);
							}
						}
						else
						{
							if (!SwingArmStationDict.ContainsKey(loadArm.BayAStationGuid))
							{
								SwingArmStationDict.Add(loadArm.BayAStationGuid, loadArm.BayAStationID);
							}
						}
					}
				}
			}

			foreach (LoadArmClass loadArm in existingLoadArmCollection)
			{
				if (station.SwingArmPosition == "A")
				{
					loadArm.BayAStationGuid = Guid.Empty;
					loadArm.BayAStationID = station.ID;
					loadArm.BayAArmNumber = 0;
				}
				else
				{
					loadArm.BayBStationGuid = Guid.Empty;
					loadArm.BayBStationID = station.ID;
					loadArm.BayBArmNumber = 0;
				}

				if (loadArm.BayAStationGuid == Guid.Empty && loadArm.BayBStationGuid == Guid.Empty)
				{
					loadArms.Purge(security, loadArm.IdentityGuid);
				}
				else
				{
					loadArm.SwingArm = false;
					loadArms.Modify(security, loadArm);
				}
			}
			return SwingArmStationDict;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, StationClass station)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (station == null)
			{
				throw new ArgumentNullException(nameof(station));
			}

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			if (!this.GetIdentityGuid(security, station.ID).IsEmpty())
			{
				throw (new Exception("Station Exists"));
			}

			station.SiteGuid = security.SiteGuid;

			this.Validate(station);

			station.CreatedDate = DateTimeOffset.Now;
			station.CreatedBy = security.UserID;
			station.UpdatedDate = station.CreatedDate;
			station.UpdatedBy = security.UserID;

			using (var cmd = new SqlCommand())
			{
				station.InsertSql(cmd);
			    this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			Guid stationGuid = this.GetIdentityGuid(security, station.ID);
			station.IdentityGuid = stationGuid;

		    this.UpdateLoadArms(security, station);

			var processVariables = new ProcessVariablesClass();
			processVariables.ModifyCollection(security, station.IdentityGuid, station.ProcessVariableCollection, null);
			processVariables.ModifyCollection(security, station.IdentityGuid, station.StationPermissives.Inputs, null);
			processVariables.ModifyCollection(security, station.IdentityGuid, station.StationPermissives.Outputs, null);

			var qualificationMaps = new QualificationMapsClass();
			qualificationMaps.ModifyCollection(security, station.IdentityGuid, station.ReqQualificationsCollection, null);
			qualificationMaps.ModifyCollection(security, station.IdentityGuid, station.ReqTrainingCollection, null);
			qualificationMaps.ModifyCollection(security, station.IdentityGuid, station.ReqTestsandInspectionsCollection, null);
            qualificationMaps.ModifyCollection(security, station.IdentityGuid, station.ReqLicenseCollection, null);
            qualificationMaps.ModifyCollection(security, station.IdentityGuid, station.ReqEquipmentTagAndLicenseCollection, null);

            return stationGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, StationClass station)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (station == null)
			{
				throw new ArgumentNullException(nameof(station));
			}

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) 
				&& !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA)
			    && !security.HasRight(RIGHT.ENABLEDISABLE_STATIONS))
			{
				throw new FMInsufficientRightsException();
			}

			this.Validate(station);

			// Verify ID does not exist
			Guid stationGuid = this.GetIdentityGuid(security, station.ID);

			if (stationGuid.IsNotEmptyAndNotEqualTo(station.IdentityGuid))
			{
				throw (new Exception("Station Exists"));
			}

			StationClass oldStation = this.Get(security, station.IdentityGuid);

			if (oldStation.IdentityGuid.IsEmpty())
			{
				throw (new Exception("Station Not Found"));
			}

			station.UpdatedDate = DateTimeOffset.Now;
			station.UpdatedBy = security.UserID;

			using (var cmd = new SqlCommand())
			{
				station.UpdateSql(cmd);
			    this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			// Update any partner stations for RewriteDynamicRecipe
			Dictionary<Guid, string> swingArmStationDict = this.UpdateLoadArms(security, station);

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "UPDATE tblStations Set EnableDynamicRecipes = @EnableDynamicRecipes WHERE StationGuid = @StationGuid";
				cmd.CommandType = CommandType.Text;

				foreach (Guid swingArmStationGuid in swingArmStationDict.Keys)
				{
					cmd.Parameters.AddWithValue("@EnableDynamicRecipes", Convert.ToInt32(station.EnableDynamicRecipes));
					cmd.Parameters.AddWithValue("@StationGuid", swingArmStationGuid);
					this.ConsolidatedDA.ExecuteQuery(security, cmd);
					cmd.Parameters.Clear();
				}
			}

			var processVariables = new ProcessVariablesClass();
			processVariables.ModifyCollection(security, station.IdentityGuid, station.ProcessVariableCollection, oldStation.ProcessVariableCollection);
			processVariables.ModifyCollection(security, station.IdentityGuid, station.StationPermissives.Inputs, oldStation.StationPermissives.Inputs);
			processVariables.ModifyCollection(security, station.IdentityGuid, station.StationPermissives.Outputs, oldStation.StationPermissives.Outputs);

			var qualificationMaps = new QualificationMapsClass();
			qualificationMaps.ModifyCollection(security, station.IdentityGuid, station.ReqQualificationsCollection, oldStation.ReqQualificationsCollection);
			qualificationMaps.ModifyCollection(security, station.IdentityGuid, station.ReqTrainingCollection, oldStation.ReqTrainingCollection);
			qualificationMaps.ModifyCollection(security, station.IdentityGuid, station.ReqTestsandInspectionsCollection, oldStation.ReqTestsandInspectionsCollection);
            qualificationMaps.ModifyCollection(security, station.IdentityGuid, station.ReqLicenseCollection, oldStation.ReqLicenseCollection);
            qualificationMaps.ModifyCollection(security, station.IdentityGuid, station.ReqEquipmentTagAndLicenseCollection, oldStation.ReqEquipmentTagAndLicenseCollection);
        }

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid targetStationGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			StationClass station = this.Get(security, targetStationGuid);

			if (station.IdentityGuid.IsEmpty())
			{
				throw (new Exception("Station Not Found"));
			}

			var processVariables = new ProcessVariablesClass();
			processVariables.ModifyCollection(security, station.IdentityGuid, null, station.ProcessVariableCollection);
			processVariables.ModifyCollection(security, station.IdentityGuid, null, station.StationPermissives.Inputs);
			processVariables.ModifyCollection(security, station.IdentityGuid, null, station.StationPermissives.Outputs);

			station.LoadArmCollection = null;
		    this.UpdateLoadArms(security, station);

			// Purge any qualification maps
			var qualificationMaps = new QualificationMapsClass();
			qualificationMaps.ModifyCollection(security, station.IdentityGuid, null, station.ReqQualificationsCollection);
			qualificationMaps.ModifyCollection(security, station.IdentityGuid, null, station.ReqTrainingCollection);
			qualificationMaps.ModifyCollection(security, station.IdentityGuid, null, station.ReqTestsandInspectionsCollection);

			using (var cmd = new SqlCommand())
			{
				station.PurgeSQL(cmd);
			    this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		public StationClass Get(SecurityClass security, Guid targetStationGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

            if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
            && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
            && !security.HasRight(RIGHT.VIEW_LOAD_RACK_DATA)
            && !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA)
            && !security.HasRight(RIGHT.ENABLEDISABLE_STATIONS)
            && !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA)
            && !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			var station = new StationClass { IdentityGuid = targetStationGuid };

			using (var cmd = new SqlCommand())
			{
				station.SelectSQL(cmd, ContextUtil.IsInTransaction);
				station.Load(this.ConsolidatedDA.GetDataSet(cmd, security));
			}

			var processVariables = new ProcessVariablesClass();
			station.ProcessVariableCollection = processVariables.EnumerateByUnit(security, station.IdentityGuid, UNIT_TYPE.STATION_UNIT);
			station.StationPermissives.Inputs = processVariables.EnumerateByUnit(security, station.IdentityGuid, station.StationPermissives.InputUnitType);
			station.StationPermissives.Outputs = processVariables.EnumerateByUnit(security, station.IdentityGuid, station.StationPermissives.OutputUnitType);

			// Special Processing 7.3 to 7.4 to move START_PERMISSIVES to StationPermissives
			var newProcessVariableCollection = new ProcessVariableCollectionClass();
			
			foreach (ProcessVariableClass processVariable in station.ProcessVariableCollection)
			{
				if (processVariable.ProcessVariableType == PROCESS_VARIABLE_TYPE.START_PERMISSIVE_PV)
				{
					processVariable.ProcessVariableType = PROCESS_VARIABLE_TYPE.OUTPUT_PERMISSIVE_PV;
					processVariable.UnitType = UNIT_TYPE.STATION_OUTPUT_PERMISSIVE;
					station.StationPermissives.Outputs.Add(processVariable);
				}
				else
				{
					newProcessVariableCollection.Add(processVariable);
				}
			}

			station.ProcessVariableCollection = newProcessVariableCollection;

			var loadArms = new LoadArmsClass();
			station.LoadArmCollection = loadArms.EnumerateByStationGuid(security, station.SwingArmPosition, station.IdentityGuid);

			// Renumber load arms in case a swing arm has been introduced and the numbering is off
			int index = 1;

			foreach (LoadArmClass loadArm in station.LoadArmCollection)
			{
				if (station.SwingArmPosition == "A")
				{
					loadArm.BayAArmNumber = index++;
				}
				else
				{
					loadArm.BayBArmNumber = index++;
				}
			}

			var qualificationMaps = new QualificationMapsClass();
			station.ReqQualificationsCollection = qualificationMaps.EnumerateByGuidAndType(security, station.IdentityGuid, QUALIFICATION_MAP_TYPE.PERSON_QUALIFICATION_TO_STATION, false);
			station.ReqTrainingCollection = qualificationMaps.EnumerateByGuidAndType(security, station.IdentityGuid, QUALIFICATION_MAP_TYPE.PERSON_TRAINING_TO_STATION, false);
			station.ReqTestsandInspectionsCollection = qualificationMaps.EnumerateByGuidAndType(security, station.IdentityGuid, QUALIFICATION_MAP_TYPE.EQUIPMENT_TEST_AND_INSPECTION_TO_STATION, false);
            station.ReqLicenseCollection = qualificationMaps.EnumerateByGuidAndType(security, station.IdentityGuid, QUALIFICATION_MAP_TYPE.PERSON_LICENSE_TO_STATION, false);
            station.ReqEquipmentTagAndLicenseCollection = qualificationMaps.EnumerateByGuidAndType(security, station.IdentityGuid, QUALIFICATION_MAP_TYPE.EQUIPMENT_TAG_AND_LICENSE_TO_STATION, false);

            return station;
		}

		public int GetTheNextPresetNumber(SecurityClass security, Guid stationGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}


			var station = new StationClass { IdentityGuid = stationGuid };

			using (var cmd = new SqlCommand())
			{
				station.NextPresetNumberSQL(cmd, ContextUtil.IsInTransaction);
				object oPreset = this.ConsolidatedDA.ExecuteScalar(cmd, security);

				if (oPreset == null)
				{
					return 1;
				}
				else
				{
					return Convert.ToInt32(oPreset) + 1;
				}
			}
		}

		public bool IsDynamicRecipesEnabled(SecurityClass security, Guid stationGuid, STATION_TYPE stationType)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			var station = new StationClass { Type = stationType, IdentityGuid = stationGuid };

			using (var cmd = new SqlCommand())
			{
				station.IsDynamicRecipesEnabled(cmd);
				object oEnabled = this.ConsolidatedDA.ExecuteScalar(cmd, security);

				if (oEnabled == null)
				{
					return false;
				}
				else
				{
					return Convert.ToBoolean(oEnabled);
				}
			}
		}

		public List<bool> IsDynamicRecipesEnabledOnPartnerStations(SecurityClass security, Guid stationGuid, List<Guid> partnerStationGuids, STATION_TYPE stationType)
		{
			List<bool> list = new List<bool>();

			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			var station = new StationClass { Type = stationType, IdentityGuid = stationGuid };

			using (var cmd = new SqlCommand())
			{
				station.IsDynamicRecipesEnabledOnPartnerStations(cmd, partnerStationGuids);
				DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);
				
				foreach (DataTable table in set.Tables)
				{
					foreach (DataRow row in table.Rows)
					{
						foreach (DataColumn column in table.Columns)
						{
							bool enabled = (Convert.ToInt32(row[column]) == 1);
							list.Add(enabled);
						}
					}
				}
			}
			return list;
		}
		public Guid GetIdentityGuid(SecurityClass security, string id)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) 
			&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
			&& !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA) 
			&& !security.HasRight(RIGHT.ENABLEDISABLE_STATIONS)			
			&& !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA)
			&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA))

			{
				throw new FMInsufficientRightsException();
			}

			var station = new StationClass { ID = id, SiteGuid = security.SiteGuid };

			using (var cmd = new SqlCommand())
			{
				station.SelectByIDSQL(cmd, ContextUtil.IsInTransaction);
				station.Load(this.ConsolidatedDA.GetDataSet(cmd, security));
				return station.IdentityGuid;
			}
		}

		public StationCollectionClass Enumerate(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
				&& !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA)
				&& !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
				&& !security.HasRight(RIGHT.ENABLEDISABLE_STATIONS)
				&& !security.HasRight(RIGHT.MODIFY_DISPATCH)
				&& !security.HasRight(RIGHT.VIEW_DISPATCH)
				&& !security.HasRight(RIGHT.IMPORT_TRANSACTION))
			{
				throw new FMInsufficientRightsException();
			}

			var station = new StationClass { SiteGuid = security.SiteGuid };

			using (var cmd = new SqlCommand())
			{
				station.EnumerateSQL(cmd);
				DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);

				var stationCollection = new StationCollectionClass();
				DataTable table = set.Tables[0];

				while (table.Rows.Count != 0)
				{
					station = new StationClass();
					station.Load(set);
					stationCollection.Add(station);
					table.Rows.RemoveAt(0);
				}

				return stationCollection;
			}
		}

		public StationCollectionClass EnumerateByType(SecurityClass security, STATION_TYPE stationType)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) 
				&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
			    && !security.HasRight(RIGHT.VIEW_LOAD_RACK_DATA) 
				&& !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA)
			    && !security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA) 
				&& !security.HasRight(RIGHT.VIEW_PERSONNEL_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			var station = new StationClass { Type = stationType, SiteGuid = security.SiteGuid };

			using (var cmd = new SqlCommand())
			{
				station.EnumerateByTypeSQL(cmd);
				DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);

				var stationCollection = new StationCollectionClass();

				var processVariables = new ProcessVariablesClass();
				DataTable table = set.Tables[0];

				while (table.Rows.Count != 0)
				{
					station = new StationClass();
					station.Load(set);
					station.ProcessVariableCollection = processVariables.EnumerateByUnit(security, station.IdentityGuid, UNIT_TYPE.STATION_UNIT);
					stationCollection.Add(station);
					table.Rows.RemoveAt(0);
				}

				return stationCollection;
			}
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
				var entityToSiteMap = (EntityToSiteMapClass)Object;

				if ( entityToSiteMap.TypeID != ENTITY_TYPE.STATION )
				{
					return;
				}

				if (!this.GetIdentityGuid(security, entityToSiteMap.ID).IsEmpty())
				{
					throw (new Exception("Station Exists - " + entityToSiteMap.ID));
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

		    var site = Object as SiteClass;
		    if (site != null)
			{
				StationCollectionClass stationCollection = this.Enumerate(security);
				var entityToSiteMaps = new EntityToSiteMaps();

				foreach (StationClass station in stationCollection)
				{
					if (site.SiteGuid == station.SiteGuid)
					{
					    this.Purge(security, station.IdentityGuid);
					}
					else
					{
						var entityToSiteMp = new EntityToSiteMapClass(station) { SiteGuid = site.SiteGuid };
						entityToSiteMaps.Purge(security, entityToSiteMp);
					}
				}

				return;
			}

		    var entityToSiteMap = Object as EntityToSiteMapClass;
		    if (entityToSiteMap != null)
			{
				var transactionAlias = new TransactionAliasClass();

				if (entityToSiteMap.TypeID == transactionAlias.EntityType)
				{
					StationCollectionClass stationCollection = this.Enumerate(security);

					foreach (StationClass station in stationCollection)
					{
						if (station.SiteGuid != entityToSiteMap.SiteGuid)
						{
							continue;
						}

						if (station.IssueByVolumeTransactionAliasGuid == entityToSiteMap.IdentityGuid
							|| station.IssueByWeightTransactionAliasGuid == entityToSiteMap.IdentityGuid
							|| station.ReceiptByVolumeTransactionAliasGuid == entityToSiteMap.IdentityGuid
							|| station.ReceiptByWeightTransactionAliasGuid == entityToSiteMap.IdentityGuid)
						{
							StationClass completeStation = this.Get(security, station.IdentityGuid);

							if (completeStation.IssueByVolumeTransactionAliasGuid == entityToSiteMap.IdentityGuid)
							{
								completeStation.IssueByVolumeTransactionAliasGuid = Guid.Empty;
								completeStation.IssueByVolumeTransactionAliasID = string.Empty;
							}

							else if (completeStation.IssueByWeightTransactionAliasGuid == entityToSiteMap.IdentityGuid)
							{
								completeStation.IssueByWeightTransactionAliasGuid = Guid.Empty;
								completeStation.IssueByWeightTransactionAliasID = string.Empty;
							}

							else if (completeStation.ReceiptByVolumeTransactionAliasGuid == entityToSiteMap.IdentityGuid)
							{
								completeStation.ReceiptByVolumeTransactionAliasGuid = Guid.Empty;
								completeStation.ReceiptByVolumeTransactionAliasID = string.Empty;
							}

							else if (completeStation.ReceiptByWeightTransactionAliasGuid == entityToSiteMap.IdentityGuid)
							{
								completeStation.ReceiptByWeightTransactionAliasGuid = Guid.Empty;
								completeStation.ReceiptByWeightTransactionAliasID = string.Empty;
							}

							this.Modify(security, completeStation);
						}
					}
				}
			}
		}
	}
}