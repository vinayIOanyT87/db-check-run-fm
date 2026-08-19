/******************************************************************************

	FILE NAME:		MultiloadIISMPLoadArmManager.cs


	PURPOSE:			MultiloadIISMPLoadArmManagerClass


	COMMENTS:

		Copyright (C) Varec, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------
		06/12/2008	W.Gray		7.4.5.0 - Change to include ItemName on
										OPC Quality Bad Messages (CSI 5961)

		11/24/2008	W.Gray		7.4.6.1 - Revised UpdateReferenceDensity to evalute Site.UseLastKnownGood (CSI 6251)

*******************************************************************************/

namespace LoadRackLibrary
{
	using System;
	using System.Collections;
	using System.Diagnostics;
	using System.Globalization;
	using System.Runtime.InteropServices;
	using System.Text;
	using System.Threading;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.LogClient;

	using Opc;
	using Opc.Da;

	/// <summary>
	/// Summary description for MultiloadIISMPLoadArmManagerClass.
	/// </summary>
	public class MultiloadIISMPLoadArmManagerClass : MultiloadIILoadArmManagerClass
	{
		public MultiloadIISMPLoadArmManagerClass(
			 EventLog eventLog,
			 SiteManagerClass siteManager,
			 StationManagerClass stationManager,
			 LoadArmClass loadArm,
			 SecurityClass security)
			 : base(eventLog, siteManager, stationManager, loadArm, security)
		{
		}

		public override bool ProcessMessageTimeout(StationManagerClass stationManager)
		{
			switch (this.LoadArmState)
			{
				case LOADARM_STATE.FINISHED_WITH_NO_PRODUCTS_TO_LOAD:
					return false;
				default:
					return base.ProcessMessageTimeout(stationManager);
			}
		}

		public override bool EnablePreset(StationManagerClass stationManager, bool showNoProductsMessage)
		{
			if (!stationManager.PreloadInProgress
			&& Bay(stationManager).RecipeMap == 0
			&& Bay(stationManager).ExtendedRecipeMap == 0)
			{
				this.IssueNoProductsToLoadMessage(true);
				return false;
			}

			this.SetState(LOADARM_STATE.PRESET_ENABLED);

			var terminalCommand = new ItemValue(this.StationPV.OPCItemID + ".Terminal Command") { Value = Esc + "A" };

			if (!string.IsNullOrEmpty(this.GetPermissiveMessage(stationManager)))
			{
				// arm should be authorized, just blocked by a permissive (which may be resolved)
				OpcServerManager.Write(new URL(this.StationPV.URL), new[] { terminalCommand });
				this.IssuePermissiveMessage(stationManager);

				return true;
			}

			OpcServerManager.Write(new URL(this.StationPV.URL), new[] { terminalCommand });

			ProcessVariableClass loadArmPv = LoadArm.ProcessVariableCollection[0];
			var enablePreset = new ItemValue(this.StationPV.OPCItemID + ".Enable Preset") { Value = "2" };

			OpcServerManager.Write(new URL(loadArmPv.URL), new[] { enablePreset });

			return true;
		}

		protected override void DisablePreset()
		{
			ProcessVariableClass loadArmPv = LoadArm.ProcessVariableCollection[0];
			var enablePreset = new ItemValue(this.StationPV.OPCItemID + ".Enable Preset") { Value = "0" };
			OpcServerManager.Write(new URL(loadArmPv.URL), new[] { enablePreset });
		}

		public override bool Authorize(StationManagerClass stationManager, double preset)
		{
			ProcessVariableClass loadArmPv = LoadArm.ProcessVariableCollection[0];
			var authorizePreset = new ItemValue(this.StationPV.OPCItemID + ".Authorize Preset");

			// authorize the preset
			var multiloadSmpStationManager = (MultiloadIISMPStationManagerClass)GetStationManager();
			if (multiloadSmpStationManager == null)
			{
				throw new OpcException("No Station Available");
			}

			this.SetState(LOADARM_STATE.AUTHORIZED);

			this.IssuePermissiveMessage(multiloadSmpStationManager);
			bool passedPermissives = this.LoadArmState == LOADARM_STATE.AUTHORIZED;

			authorizePreset.Value = passedPermissives ? "1" : "0";   // authorize bit
			authorizePreset.Value += this.CurrentRecipe.PresetNumber.ToString("D3", CultureInfo.InvariantCulture); // selected product number
			authorizePreset.Value += System.Convert.ToInt32(preset).ToString("D9"); // preset volume
			authorizePreset.Value += "01";   // compartment number

			OpcServerManager.Write(new URL(loadArmPv.URL), new[] { authorizePreset });

			this.UpdatePermissives(stationManager, this.CurrentRecipe, passedPermissives);

			return true;
		}

		protected override void InitializeLoadArmPvs()
		{
			string loadArmUrl = this.LoadArm.ProcessVariableCollection[0].URL;
			string loadArmProgID = this.LoadArm.ProcessVariableCollection[0].ProgID;

			this.LoadArmStatePV = new ProcessVariableClass(
				 PROCESS_VARIABLE_TYPE.LOAD_ARM_STATE_PV,
				 UNIT_TYPE.LOADARM_UNIT,
				 VarEnum.VT_UI2,
				 true,
				 this.StationPV.OPCItemID + ".Preset State", // station level tag for SMP
				 loadArmUrl,
				 loadArmProgID);

			this.OpcServerManager.AddProcessVariable(this.LoadArmStatePV);

			this.RemoteMsgActivePV = new ProcessVariableClass(
				 PROCESS_VARIABLE_TYPE.LOADARM_PV,
				 UNIT_TYPE.LOADARM_UNIT,
				 VarEnum.VT_BOOL,
				 true,
				 this.StationPV.OPCItemID + ".Preset Status.Remote Message", // station level tag for SMP
				 loadArmUrl,
				 loadArmProgID);

			this.OpcServerManager.AddProcessVariable(this.RemoteMsgActivePV);

			this.AuthorizedPV = new ProcessVariableClass(
				 PROCESS_VARIABLE_TYPE.LOADARM_PV,
				 UNIT_TYPE.LOADARM_UNIT,
				 VarEnum.VT_BOOL,
				 true,
				 this.StationPV.OPCItemID + ".Preset Status.Authorized", // station level tag for SMP
				 loadArmUrl,
				 loadArmProgID);

			this.OpcServerManager.AddProcessVariable(this.AuthorizedPV);

			this.HostEnabledPV = new ProcessVariableClass(
				 PROCESS_VARIABLE_TYPE.LOADARM_PV,
				 UNIT_TYPE.LOADARM_UNIT,
				 VarEnum.VT_BOOL,
				 true,
				 this.StationPV.OPCItemID + ".Preset Status.Host Enabled", // station level tag for SMP
				 loadArmUrl,
				 loadArmProgID);

			this.OpcServerManager.AddProcessVariable(this.HostEnabledPV);
		}

		public override void IssuePermissiveMessage(StationManagerClass stationManager)
		{
			if (stationManager == null || this.LoadArmStatePV == null)
			{
				return;
			}

			var logger = new Logger(StopWatch.Appnames.LoadRackService.ToString());
			logger.Debug("In IssuePermissiveMessage; RemoteMsgActivePV = " + this.RemoteMsgActivePV.ServerValue + "; LoadArmStatePV = " + this.LoadArmStatePV.ServerValue);

			if (this.LoadArmStatePV.IsQualityGood
				 && ((ushort)this.LoadArmStatePV.ServerValue == (ushort)PresetState.Alarm
						  || (ushort)this.LoadArmStatePV.ServerValue == (ushort)PresetState.NotAuth
						  || (ushort)this.LoadArmStatePV.ServerValue == (ushort)PresetState.Idle))
			{
				string message = GetPermissiveMessage(stationManager);

				if (!string.IsNullOrEmpty(message))
				{
					ProcessVariableClass loadArmPv = LoadArm.ProcessVariableCollection[0];
					var armUnavailable = new ItemValue(this.StationPV.OPCItemID + ".Enable Preset") { Value = "1" };

					string permissiveHeader = stationManager.GetDataDictionaryValueByKey(SiteManager.Site.SiteGuid, "[LoadRack|Permissive]");
					var presetMessage = new ItemValue(this.StationPV.OPCItemID + ".Preset Message")
					{
						Value = permissiveHeader.PadRight(10, ' ').Substring(0, 10) + message.PadRight(39, ' ').Substring(0, 39)
					};
					OpcServerManager.Write(new URL(loadArmPv.URL), new[] { armUnavailable, presetMessage });
					switch (this.LoadArmState)
					{
						case LOADARM_STATE.AUTHORIZED:
							this.SetState(LOADARM_STATE.AUTHORIZED_PERMISSIVE_PROMPT);
							break;
						case LOADARM_STATE.INPROGRESS:
							this.SetState(LOADARM_STATE.INPROGRESS_PERMISSIVE_PROMPT);
							break;
						case LOADARM_STATE.PRESET_ENABLED:
							this.SetState(LOADARM_STATE.PRESET_ENABLED_PERMISSIVE_PROMPT);
							break;
					}
				}
			}
		}

		protected override void BatchComplete()
		{
			ProcessVariableClass loadArmPv = LoadArm.ProcessVariableCollection[0];
			var batchComplete = new ItemValue(this.StationPV.OPCItemID + ".Batch Complete");
			OpcServerManager.Write(new URL(loadArmPv.URL), new[] { batchComplete });

			this.UpdatePermissives(this.GetStationManager(), this.CurrentRecipe, false);
		}

		protected override void BatchEnd()
		{
			ProcessVariableClass loadArmPv = LoadArm.ProcessVariableCollection[0];
			var batchEnd = new ItemValue(this.StationPV.OPCItemID + ".Batch End");
			OpcServerManager.Write(new URL(loadArmPv.URL), new[] { batchEnd });

			this.UpdatePermissives(this.GetStationManager(), this.CurrentRecipe, false);
		}

		public override void EndBatch()
		{
			ProcessVariableClass loadArmPv = LoadArm.ProcessVariableCollection[0];
			var endBatch = new ItemValue(this.StationPV.OPCItemID + ".End Batch");
			OpcServerManager.Write(new URL(loadArmPv.URL), new[] { endBatch });

			if ((this.GetStationManager()?.Station.Type ?? STATION_TYPE.MAX_STATION_TYPE) != STATION_TYPE.LOAD_RACK)
			{
				// For offload stations, we should end the transaction once we end the batch.
				// If GetStationManager() returned null, then something BAD happened, and we should also end the transaction.
				// Load rack station is the only other reasonable possibility; in that case we allow the transaction to continue.
				this.SendEndTransaction();
			}
		}

		public override void Start()
		{
			ProcessVariableClass loadArmPv = LoadArm.ProcessVariableCollection[0];
			ItemValue[] itemValues = { new ItemValue(new ItemIdentifier(this.StationPV.OPCItemID + ".Start")) };
			OpcServerManager.Write(new URL(loadArmPv.URL), itemValues);
		}

		public override void Stop()
		{
			ProcessVariableClass loadArmPv = LoadArm.ProcessVariableCollection[0];
			ItemValue[] itemValues = { new ItemValue(new ItemIdentifier(this.StationPV.OPCItemID + ".Stop")) };
			OpcServerManager.Write(new URL(loadArmPv.URL), itemValues);
		}

		public override void ReadNonResettableTotals(
			 Opc.Da.Server server,
			 out ItemValueResult[] nonResettableGrossVolumes)
		{
			var items = new ArrayList();

			string tagPrefix = this.StationPV.OPCItemID + ".Gross Totalizer.Component.";

			items.Add(new Item(new ItemIdentifier(tagPrefix + "1")));

			tagPrefix = this.StationPV.OPCItemID + ".Gross Totalizer.Additive.";

			for (int additive = 0; additive < 3; additive++)
			{
				items.Add(new Item(new ItemIdentifier(tagPrefix + additive.ToString(CultureInfo.InvariantCulture))));
			}

			foreach (Item item in items)
			{
				item.MaxAgeSpecified = true;
			}

			 ((Item)items[0]).MaxAgeSpecified = false;

			ItemValueResult[] values = server.Read((Item[])items.ToArray(typeof(Item)));

			nonResettableGrossVolumes = values;
		}

		public override void ReadComponentData(
			 Opc.Da.Server server,
			 ProductMapClass component,
			 out ItemValueResult grossVolume,
			 out ItemValueResult netVolume,
			 out ItemValueResult averageTemperature,
			 out ItemValueResult averageDensity)
		{
			string tagPrefix = this.StationPV.OPCItemID + ".Batch.";

			var items = new ArrayList
								 {
									  new Item(new ItemIdentifier(tagPrefix + "Gross Volume")),
									  new Item(new ItemIdentifier(tagPrefix + "Net Volume")),
									  new Item(new ItemIdentifier(tagPrefix + "Average Temperature")),
									  new Item(new ItemIdentifier(tagPrefix + "Average Density"))
								 };

			foreach (Item item in items)
			{
				item.MaxAgeSpecified = true;
			}

			 ((Item)items[0]).MaxAgeSpecified = false;

			ItemValueResult[] values = server.Read((Item[])items.ToArray(typeof(Item)));

			grossVolume = values[0];
			netVolume = values[1];
			averageTemperature = values[2];
			averageDensity = values[3];
		}

		public override void ReadAdditiveData(
			 Opc.Da.Server server,
			 ProductMapClass additive,
			 out ItemValueResult grossVolume)
		{
			string tagPrefix = this.StationPV.OPCItemID + ".Batch.Additive.";

			int position = 0;
			bool found = false;

			// Need to find the component's out of those defined on the arm
			foreach (ProductMapClass armAdditive in this.LoadArm.AdditiveInjectorCollection)
			{
				position++;
				if (armAdditive.AssignedGuid == additive.AssignedGuid)
				{
					found = true;
					break;
				}
			}

			if (found)
			{
				var items = new ArrayList
										{
											 new Item(new ItemIdentifier(tagPrefix + position.ToString(CultureInfo.InvariantCulture) + ".Gross Volume")),
										};

				foreach (Item item in items)
				{
					item.MaxAgeSpecified = true;
				}

				 ((Item)items[0]).MaxAgeSpecified = false;

				ItemValueResult[] values = server.Read((Item[])items.ToArray(typeof(Item)));

				grossVolume = values[0];
			}
			else
			{
				grossVolume = new ItemValueResult(new ItemValue { Quality = Quality.Bad, Value = 0.0 });
			}
		}

		protected override void OnInvoke(ProcessVariableClass pv)
		{
			StationManagerClass stationManager = this.GetStationManager();
			if (stationManager == null)
			{
				return;
			}

			Monitor.Enter(stationManager);

			try
			{
				switch (pv.ProcessVariableType)
				{
					case PROCESS_VARIABLE_TYPE.INPUT_PERMISSIVE_PV:
						{
							var logger = new Logger(StopWatch.Appnames.LoadRackService.ToString());
							logger.Debug("Input Permissive change - Arm " + this.GetArmNumber(this.GetStationManager()).ToString(CultureInfo.InvariantCulture) + ", Arm State " + this.LoadArmState.ToString());
							if (this.LoadArmState == LOADARM_STATE.PRESET_ENABLED_PERMISSIVE_PROMPT)
							{
								// Try enabling the arm now.  EnablePreset will check permissives again for us
								logger.Debug("Calling EnablePreset for Arm " + this.GetArmNumber(this.GetStationManager()).ToString(CultureInfo.InvariantCulture));
								this.EnablePreset(this.GetStationManager(), true);
							}

							base.OnInvoke(pv);
							break;
						}

					case PROCESS_VARIABLE_TYPE.LOADARM_PV:
						{
							if (pv.OPCItemID.EndsWith("RCU Status"))
							{
								if (pv.IsQualityGood)
								{
									char rcuStatus = System.Convert.ToChar(pv.ServerValue);
									switch (rcuStatus)
									{
										// Idle
										case '0':
											if (!this.ProcessMessageTimeout(stationManager))
											{
												stationManager.ProcessMessageTimeout();
											}

											break;

										// Transaction Done
										case 'D':
											{
												this.SetState(LOADARM_STATE.NORMAL);

												switch (stationManager.StationState)
												{
													case StationState.AUTHORIZED:
													case StationState.AUTHORIZING:
													case StationState.TRANSACTION_IN_PROGRESS:
														stationManager.StationState = StationState.TRANSACTION_IN_PROGRESS;
														this.SetFinishedLoading();
														break;
												}

												break;
											}

										// Preset 1 Preset Request
										// There's only one Preset/Arm on the SMP, so this will be the right one
										case 'a':
											if (stationManager.Station.Type == STATION_TYPE.LOAD_RACK)
											{
												this.PromptForNextBatch(stationManager, true);
											}
											break;

										// transaction authorized
										case 'T':
											switch (stationManager.StationState)
											{
												case StationState.TRANSACTION_IN_PROGRESS:
												case StationState.AUTHORIZED:
												case StationState.AUTHORIZING:
												case StationState.IDLE:
												case StationState.ENTER_DRIVER_ID_PROMPT:
													break;
												default:
													this.EndTransaction();
													break;
											}

											break;
									}
								}
							}
							else if (pv.OPCItemID.EndsWith("Preset Status.Host Enabled"))
							{
								if (pv.IsQualityGood
								&& (bool)pv.ServerValue
								&& this.LoadArmState == LOADARM_STATE.NORMAL)
								{
									ProcessVariableClass loadArmPv = LoadArm.ProcessVariableCollection[0];
									var enablePreset = new ItemValue(this.StationPV.OPCItemID + ".Enable Preset") { Value = "0" };
									OpcServerManager.Write(new URL(loadArmPv.URL), new[] { enablePreset });
								}
							}
							else if (pv.OPCItemID.EndsWith("Preset Status.Remote Message"))
							{
								if (pv.IsQualityGood && !((bool)pv.ServerValue))
								{
									this.IssuePermissiveMessage(stationManager);
								}
							}

							break;
						}

					case PROCESS_VARIABLE_TYPE.LOAD_ARM_STATE_PV:
						{
							if (pv.IsQualityGood)
							{
								if (this.CommunicationsFailure)
								{
									this.CommunicationsFailure = false;
								}

								switch ((ushort)pv.ServerValue)
								{
									case (ushort)PresetState.Idle:
										{
											break;
										}

									case (ushort)PresetState.Alarm:
										{
											if (this.RemoteMsgActivePV.IsQualityGood && (bool)this.RemoteMsgActivePV.ServerValue)
											{
												break;
											}

											this.IssuePermissiveMessage(stationManager);
											break;
										}

									case (ushort)PresetState.Start:
									case (ushort)PresetState.LowFlow:
									case (ushort)PresetState.HighFlow:
										{
											if (stationManager.StationState == StationState.AUTHORIZING
												 || stationManager.StationState == StationState.AUTHORIZED
												 || stationManager.StationState == StationState.TRANSACTION_IN_PROGRESS)
											{
												LineItemDO lineItem = stationManager.GetLineItem(this.LoadArm.IdentityGuid);

												if (lineItem == null)
												{
													if (this.LoadArmState == LOADARM_STATE.AUTHORIZED
														 || this.LoadArmState == LOADARM_STATE.AUTHORIZED_PERMISSIVE_PROMPT)
													{
														this.SetState(LOADARM_STATE.INPROGRESS);

														if (stationManager.AddLineItem(this.LoadArm.IdentityGuid) != null)
														{
															stationManager.SaveTransaction();
														}
													}
												}
												else
												{
													if (this.LoadArmState == LOADARM_STATE.AUTHORIZED
														 || this.LoadArmState == LOADARM_STATE.AUTHORIZED_PERMISSIVE_PROMPT)
													{
														this.SetState(LOADARM_STATE.INPROGRESS);
													}

													stationManager.SetTransactionInProgress();
													stationManager.UpdateLineItem(lineItem);
													stationManager.SaveTransaction();
												}

												stationManager.StationState = StationState.TRANSACTION_IN_PROGRESS;
											}

											break;
										}

									case (ushort)PresetState.Preset:
										{
											this.SetState(LOADARM_STATE.AUTHORIZED);
											break;
										}

									case (ushort)PresetState.Stop:
										{
											break;
										}

									case (ushort)PresetState.Complete:
										{
											LineItemDO lineItem = stationManager.GetLineItem(this.LoadArm.IdentityGuid);

											if (lineItem != null)
											{
												stationManager.UpdateLineItem(lineItem);
												stationManager.SaveTransaction();
												this.BatchEnd();
												this.SetState(LOADARM_STATE.BATCH_COMPLETE);
											}

											break;
										}

									case (ushort)PresetState.EndOfBatch:
										{
											if (this.LoadArmState == LOADARM_STATE.AUTHORIZED)
											{
												this.CancelPresetting(stationManager);
												stationManager.UpdatePermissives(false);

												this.NonPreloadEquipmentSelection = string.Empty;
												this.NonPreloadCompartmentSelection = -1;

												this.EndBatch();
												this.SetState(LOADARM_STATE.PRESET_ENABLED);
											}
											else if (this.LoadArmState == LOADARM_STATE.BATCH_COMPLETE)
											{
												LineItemDO lineItem = stationManager.GetLineItem(LoadArm.IdentityGuid);

												stationManager.UpdateLineItem(lineItem);
												stationManager.CloseOutLineItem(lineItem);
												stationManager.SaveTransaction();
												stationManager.UpdatePermissives(false);

												this.NonPreloadEquipmentSelection = string.Empty;
												this.NonPreloadCompartmentSelection = -1;

												this.EndBatch();
											}
											else if (this.LoadArmState == LOADARM_STATE.INPROGRESS)
											{
												LineItemDO lineItem = stationManager.GetLineItem(LoadArm.IdentityGuid);

												if (lineItem != null)
												{
													stationManager.UpdateLineItem(lineItem);
													stationManager.CloseOutLineItem(lineItem);
													stationManager.SaveTransaction();
													stationManager.UpdatePermissives(false);

													this.NonPreloadEquipmentSelection = string.Empty;
													this.NonPreloadCompartmentSelection = -1;

													this.SetState(LOADARM_STATE.BATCH_COMPLETE);
												}

												this.EndBatch();
											}

											break;
										}

									case (ushort)PresetState.NotAuth:
										{
											if (this.HostEnabledPV.IsQualityGood && (bool)this.HostEnabledPV.ServerValue)
											{
												this.DisablePreset();
											}

											break;
										}
								}
							}
							else
							{
								this.CommunicationsFailure = true;
							}

							break;
						}

					default:
						base.OnInvoke(pv);
						break;
				}
			}
			catch (OpcException e)
			{
				this.eventLog.WriteEntry("Multiload II SMP LoadArmManager OnInvoke : PV = " + pv.OPCItemID + "\n" + e.Message + "\n" + e.StackTrace, EventLogEntryType.Error);
				this.CommunicationsFailure = true;
			}
			catch (Exception e)
			{
				this.eventLog.WriteEntry("Multiload II SMP LoadArmManager OnInvoke : PV = " + pv.OPCItemID + " " + e + "\n" + e.StackTrace, EventLogEntryType.Error);
			}
			finally
			{
				Monitor.Exit(stationManager);
			}
		}

		public override bool CheckLoadArmAuthorizedProducts()
		{
			string[] productID = { "000", "000", "000", "000", "000", "000", "000", "000" };
			string[] additiveID = { "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000" };
			string[] recipeID =
			  {
						"000", "000", "000", "000", "000", "000", "000", "000",
						  "000", "000", "000", "000", "000", "000", "000", "000",
						  "000", "000", "000", "000", "000", "000", "000", "000",
						  "000", "000", "000", "000", "000", "000", "000", "000",
						  "000"
				  };

			// this routine will check the authorized products to the preset for this arm
			// set the preset name based on the configured arm name from the OPC configuration
			// the end of the assignment has to be .Arm 1 or .Arm 2 or so on
			var multiloadStationManager = (MultiloadIIStationManagerClass)GetStationManager();
			if (multiloadStationManager == null)
			{
				throw new OpcException("No Station Available");
			}

			string opcItemId = this.StationPV.OPCItemID + ".Arm Product Configuration";

			int loop;
			int compIndex;
			for (loop = 0; loop < this.LoadArm.ComponentCollection.Count && loop < productID.Length; loop++)
			{
				compIndex = this.LoadArm.ComponentCollection[loop].PresetNumber;
				if (compIndex >= 0)
				{
					productID[loop] = compIndex.ToString("D3", CultureInfo.InvariantCulture);
				}
			}

			for (loop = 0; loop < this.LoadArm.AdditiveInjectorCollection.Count && loop < additiveID.Length; loop++)
			{
				compIndex = this.LoadArm.AdditiveInjectorCollection[loop].PresetNumber;
				if (compIndex >= 0)
				{
					additiveID[loop] = compIndex.ToString("D3", CultureInfo.InvariantCulture);
				}
			}

			for (loop = 0; loop < this.LoadArm.ProductRecipeCollection.Count && loop < recipeID.Length; loop++)
			{
				compIndex = this.LoadArm.ProductRecipeCollection[loop].PresetNumber;
				if (compIndex >= 0)
				{
					recipeID[loop] = compIndex.ToString("D3", CultureInfo.InvariantCulture);
				}
			}

			ItemValueResult[] values = OpcServerManager.Read(
				 new URL(this.LoadArm.ProcessVariableCollection[0].URL),
				 new[] { new Item(new ItemIdentifier(opcItemId)) });

			if (values[0].Quality != Quality.Good)
			{
				return false;
			}

			var deviceProductDefinition = values[0].Value as string;
			if (string.IsNullOrEmpty(deviceProductDefinition))
			{
				return false;
			}

			// Build the component/additive/product configuration section
			var configurationStringBuilder = new StringBuilder();
			int totalEntries = 0;

			// ReSharper disable ForCanBeConvertedToForeach
			for (int index = 0; index < productID.Length; index++)
			{
				configurationStringBuilder.Append(productID[index]);
				totalEntries++;
			}

			for (int index = 0; index < additiveID.Length; index++)
			{
				configurationStringBuilder.Append(additiveID[index]);
				totalEntries++;
			}

			for (int index = 0; index < recipeID.Length; index++)
			{
				configurationStringBuilder.Append(recipeID[index]);
				totalEntries++;
			}

			// ReSharper restore ForCanBeConvertedToForeach
			string configurationSection = configurationStringBuilder.ToString();

			if (string.CompareOrdinal(configurationSection, 0, deviceProductDefinition, 10, totalEntries * 3) != 0)
			{
				// not a match
				return false;
			}

			return true;
		}

		protected override void IssueNoProductsToLoadMessage(bool finished)
		{
			if (finished)
			{
				this.SetState(LOADARM_STATE.FINISHED_WITH_NO_PRODUCTS_TO_LOAD);
				this.DisplayMessage("LoadRack|No Products to Load on Arm", 0, this.PromptTimeout);
			}
			else
			{
				this.SetState(LOADARM_STATE.NO_PRODUCTS_TO_LOAD);
				this.DisplayMessage("LoadRack|No Products to Load on Arm", 0);
			}
		}

		public override bool IsStatusForThisLoadArm(char rcuStatus)
		{
			// Some statuses are full station/all arms
			switch (rcuStatus)
			{
				case '0':
				case '4':
				case '9':
				case '%':
				case 'A':
				case 'B':
				case 'C':
				case 'D':
				case 'E':
				case 'P':
				case 'R':
				case 'T':
				case '?':
				case '!':
				case 'I':
				case 'N':
					return true;
				case 'a': // this is the status for Arm 1.  This is the only arm/Preset we support for the SMP
					return true;
				default:
					return false;
			}
		}
	}
}
