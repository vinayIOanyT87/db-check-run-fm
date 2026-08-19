/******************************************************************************

	FILE NAME:		Danload6000LoadArmManager.cs


	PURPOSE:			Danload6000LoadArmManagerClass


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------
		06/25/2008	W.Gray		7.3.2.0 - Change to issue Timeout Operation for
										DisplayMessage

		08/18/2008	W.Gray		7.4.5.0 - Change to ProcessStopKey correcting to
										EndTransaction when state is AUTHORIZED, SELECT_RECIPE_PROMPT
										or PRESET_VOLUME_PROMPT and change to EndTransaction to
										issues command when TransactionInProgress or TransactionAuthorized
										(CSI 6090)

		11/24/2008	W.Gray		7.4.6.1 - Revised UpdateReferenceDensity to evalute Site.UseLastKnownGood (CSI 6251)

		07/07/2009	W.Gray		7.4.6.2 - Revised UpdateReferenceDensity to send Standard Density (CSI 4167)
 
		12/14/2009	W.Gray		7.5.1.0 - Revised for Info.MaxFill in SI Units (WI 10074)

*******************************************************************************/

namespace LoadRackLibrary
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Globalization;
    using System.Net;
    using System.Runtime.InteropServices;
    using System.Threading;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;

    using Opc;
    using Opc.Da;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    using Convert = System.Convert;
    using Factory = OpcCom.Factory;
    using Server = Opc.Da.Server;

    /// <summary>
	/// Summary description for Danload6000LoadArmManagerClass.
	/// </summary>
	public class Danload6000LoadArmManagerClass : LoadArmManagerClass
	{
		protected const int NUMBER_OF_RETRIES = 1;
		//protected DisplayMenuParameters	CurrentMenuParameters;
		protected ProcessVariableClass BatchAbortedPV;
		protected ProcessVariableClass BatchEndedPV;
		protected ProcessVariableClass BatchInProgressPV;
		protected ProcessVariableClass BatchAuthorizedPV;
		protected ProcessVariableClass KeyPadDataPendingPV;
		protected ProcessVariableClass PromptTimeoutPV;
		protected ProcessVariableClass TransactionEndRequestedPV;
		protected ProcessVariableClass RecipeSelectedPV;
		protected ProcessVariableClass PresetVolumeEnteredPV;
		protected ProcessVariableClass TransactionInProgressPV;
		protected ProcessVariableClass TransactionAuthorizedPV;
		protected ProcessVariableClass SwingArmStatusPV;
		protected ProcessVariableClass LastKeyPressedPV;
		protected ProcessVariableClass PrimaryAlarmPV;
		protected ProcessVariableClass ManualPV;
		protected int SelectedRecipe;
		protected int PresetVolume;
		protected int TransactionNumber;
		protected int BatchNumber;

		public Danload6000LoadArmManagerClass(
			EventLog EventLog,
			SiteManagerClass SiteManager,
			StationManagerClass StationManager,
			LoadArmClass LoadArm,
			SecurityClass Security)
			: base(EventLog, SiteManager, StationManager, LoadArm, Security)
		{
			if (LoadArm.Enabled == false)
			{
				// If the load arm is disabled, we don't want to set up any process variables for it.
				return;
			}

			string LoadArmURL = LoadArm.ProcessVariableCollection[0].URL;
			string LoadArmProgID = LoadArm.ProcessVariableCollection[0].ProgID;
			string LoadArmOPCItemID = LoadArm.ProcessVariableCollection[0].OPCItemID;

		    this.BatchAbortedPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.BATCH_ABORTED_PV,
				UNIT_TYPE.STATION_UNIT,
				VarEnum.VT_BOOL,
				true,
				LoadArmOPCItemID + ".Status.Batch Aborted",
				LoadArmURL,
				LoadArmProgID);

		    this.BatchEndedPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.BATCH_DONE_PV,
				UNIT_TYPE.STATION_UNIT,
				VarEnum.VT_BOOL,
				true,
				LoadArmOPCItemID + ".Status.Batch Ended",
				LoadArmURL,
				LoadArmProgID);

		    this.BatchAuthorizedPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.BATCH_AUTHORIZED_PV,
				UNIT_TYPE.STATION_UNIT,
				VarEnum.VT_BOOL,
				true,
				LoadArmOPCItemID + ".Status.Batch Authorized",
				LoadArmURL,
				LoadArmProgID);

		    this.BatchInProgressPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.BATCH_IN_PROGRESS_PV,
				UNIT_TYPE.STATION_UNIT,
				VarEnum.VT_BOOL,
				true,
				LoadArmOPCItemID + ".Status.Batch In Progress",
				LoadArmURL,
				LoadArmProgID);

		    this.TransactionAuthorizedPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.TRANSACTION_AUTHORIZED_PV,
				UNIT_TYPE.STATION_UNIT,
				VarEnum.VT_BOOL,
				true,
				LoadArmOPCItemID + ".Status.Transaction Authorized",
				LoadArmURL,
				LoadArmProgID);

		    this.TransactionInProgressPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.TRANSACTION_IN_PROGRESS_PV,
				UNIT_TYPE.STATION_UNIT,
				VarEnum.VT_BOOL,
				true,
				LoadArmOPCItemID + ".Status.Transaction In Progress",
				LoadArmURL,
				LoadArmProgID);

		    this.SwingArmStatusPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.SWING_ARM_STATUS_PV,
				UNIT_TYPE.STATION_UNIT,
				VarEnum.VT_UI1,
				true,
				LoadArmOPCItemID + ".Status.Swing Arm",
				LoadArmURL,
				LoadArmProgID);

		    this.KeyPadDataPendingPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.KEYPAD_DATA_PENDING_PV,
				UNIT_TYPE.STATION_UNIT,
				VarEnum.VT_BOOL,
				true,
				LoadArmOPCItemID + ".Status.Keypad Data Available",
				LoadArmURL,
				LoadArmProgID);

		    this.PromptTimeoutPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.PROMPT_TIMEOUT_PV,
				UNIT_TYPE.STATION_UNIT,
				VarEnum.VT_BOOL,
				true,
				LoadArmOPCItemID + ".Status.Operation Timedout",
				LoadArmURL,
				LoadArmProgID);

		    this.RecipeSelectedPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.RECIPE_SELECTED_PV,
				UNIT_TYPE.STATION_UNIT,
				VarEnum.VT_BOOL,
				true,
				LoadArmOPCItemID + ".Status.Recipe Selected",
				LoadArmURL,
				LoadArmProgID);

		    this.OpcServerManager.AddProcessVariable(this.RecipeSelectedPV);

		    this.PresetVolumeEnteredPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.PRESET_VOLUME_ENTERED_PV,
				UNIT_TYPE.STATION_UNIT,
				VarEnum.VT_BOOL,
				true,
				LoadArmOPCItemID + ".Status.Preset Entered",
				LoadArmURL,
				LoadArmProgID);

		    this.LastKeyPressedPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.GET_KEY_PV,
				UNIT_TYPE.STATION_UNIT,
				VarEnum.VT_UI1,
				true,
				LoadArmOPCItemID + ".Last Key Pressed",
				LoadArmURL,
				LoadArmProgID);

		    this.PrimaryAlarmPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.PRIMARY_ALARM_PV,
				UNIT_TYPE.STATION_UNIT,
				VarEnum.VT_BOOL,
				true,
				LoadArmOPCItemID + ".Status.Primary Alarm",
				LoadArmURL,
				LoadArmProgID);

		    this.ManualPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.MANUAL_PV,
				UNIT_TYPE.STATION_UNIT,
				VarEnum.VT_BOOL,
				true,
				LoadArmOPCItemID + ".Status.Manual",
				LoadArmURL,
				LoadArmProgID);

			try
			{
			    this.OpcServerManager.Read(this.ManualPV);
			}
			catch (Exception e)
			{
				EventLog.WriteEntry("Danload 6000 LoadArmManager Constructor : " + e.Message, EventLogEntryType.Error);
			    this.CommunicationsFailure = true;
			}

			try
			{
			    this.OpcServerManager.Read(this.PrimaryAlarmPV);
			}
			catch (Exception e)
			{
				EventLog.WriteEntry("Danload 6000 LoadArmManager Constructor : " + e.Message, EventLogEntryType.Error);
			    this.CommunicationsFailure = true;
			}

			if (LoadArm.SwingArm)
			{
				try
				{
				    this.OpcServerManager.Read(this.SwingArmStatusPV);
					if (this.SwingArmStatusPV.IsQualityGood) this.LastSwingArmStatus = this.SwingArmStatusPV.ServerValue;
				}
				catch (Exception e)
				{
					EventLog.WriteEntry("Danload 6000 LoadArmManager Constructor : " + e.Message, EventLogEntryType.Error);
				    this.CommunicationsFailure = true;
				}
			}
			else
			{
			    this.SwingArmStatusPV.ServerValue = 1;
			    this.SwingArmStatusPV.OPCQuality = Quality.Good.GetCode();
			}

		    this.OpcServerManager.AddProcessVariable(this.BatchAbortedPV);
		    this.OpcServerManager.AddProcessVariable(this.BatchEndedPV);
		    this.OpcServerManager.AddProcessVariable(this.BatchAuthorizedPV);
		    this.OpcServerManager.AddProcessVariable(this.BatchInProgressPV);
		    this.OpcServerManager.AddProcessVariable(this.TransactionAuthorizedPV);
		    this.OpcServerManager.AddProcessVariable(this.TransactionInProgressPV);
		    this.OpcServerManager.AddProcessVariable(this.KeyPadDataPendingPV);
		    this.OpcServerManager.AddProcessVariable(this.PromptTimeoutPV);
		    this.OpcServerManager.AddProcessVariable(this.PresetVolumeEnteredPV);
		    this.OpcServerManager.AddProcessVariable(this.LastKeyPressedPV);
		    this.OpcServerManager.AddProcessVariable(this.PrimaryAlarmPV);
		    this.OpcServerManager.AddProcessVariable(this.ManualPV);


			// Only monitor Swing Arm if configured as such
			if (LoadArm.SwingArm) this.OpcServerManager.AddProcessVariable(this.SwingArmStatusPV);

		}

		public override int GetRecipeNumber(ProductMapClass recipe)
		{
			int Index = 1;
			foreach (ProductMapClass LoadArmRecipe in this.LoadArm.ProductRecipeCollection)
			{
				if (LoadArmRecipe == recipe)
					break;

				Index++;
			}
			return Index;
		}

		public override ProductMapClass GetRecipeByRecipeNumber(int recipeNumber)
		{
			StationManagerClass StationManager = this.GetStationManager();
			if (StationManager == null)
				return null;

			// RecipeMap defines the authorized recipes
			// RecipeNumber is the recipe in the DANLOAD 6000
			// Index is the recipe in the LoadArm ProductRecipeCollection
			int Item = 0;
			for (int Index = 0; Index < 32; Index++)
			{
				if ((this.Bay(StationManager).RecipeMap & (ulong)1 << Index) != 0)
				{
					Item++;
					if (recipeNumber == Item)
					{
						return this.LoadArm.ProductRecipeCollection[Index];
					}
				}
			}

			return null;
		}


		protected override void OnInvoke(ProcessVariableClass pv)
		{
			if (!pv.IsQualityGood)
			{
				this.CommunicationsFailure = true;
				return;
			}

		    if (this.ManualPV.IsQualityGood && (bool)this.ManualPV.ServerValue)
		    {
		        return;
		    }

			StationManagerClass stationManager = this.GetStationManager();
			if (stationManager == null)
			{
				// Situation can occur when alternate station is disabled
				if (pv.ProcessVariableType == PROCESS_VARIABLE_TYPE.SWING_ARM_STATUS_PV)
				{
				    this.LastSwingArmStatus = pv.ServerValue;
				    this.SetState(LOADARM_STATE.NORMAL);
				}

			    this.ReleaseKeyPad();
				return;
			}

			Monitor.Enter(stationManager);

			try
			{
				switch (pv.ProcessVariableType)
				{
					case PROCESS_VARIABLE_TYPE.TRANSACTION_AUTHORIZED_PV:
						{
							if (pv.IsQualityGood)
							{
								if ((bool)pv.ServerValue)
								{
									if (this.TransactionInProgressPV.IsQualityGood
									&& !((bool)this.TransactionInProgressPV.ServerValue)
									&& this.BatchAuthorizedPV.IsQualityGood
									&& !((bool)this.BatchAuthorizedPV.ServerValue))
									{

										if (stationManager.StationState == StationState.AUTHORIZING
										|| stationManager.StationState == StationState.AUTHORIZED
										|| stationManager.StationState == StationState.TRANSACTION_IN_PROGRESS)
										{
											if (this.IssuePresetVolumePrompt(stationManager))
												return;

										    this.AuthorizeBatch(this.PresetVolume);
										}
										else this.SendEndTransaction();
									}
								}
								else
								{
									if (this.LoadArmState == LOADARM_STATE.INPROGRESS
									|| this.LoadArmState == LOADARM_STATE.AUTHORIZED)
									{
									    this.IssueSelectPrompt(stationManager);
									}
								}
							}
							break;
						}

					case PROCESS_VARIABLE_TYPE.TRANSACTION_IN_PROGRESS_PV:
						{
							if (pv.IsQualityGood
							&& ((bool)pv.ServerValue)
							&& (stationManager.StationState == StationState.AUTHORIZING
							|| stationManager.StationState == StationState.AUTHORIZED
							|| stationManager.StationState == StationState.TRANSACTION_IN_PROGRESS))
							{
							}

							break;
						}

					case PROCESS_VARIABLE_TYPE.BATCH_ABORTED_PV:
						{
							if (pv.IsQualityGood
								&& (bool)pv.ServerValue
								&& (stationManager.StationState == StationState.AUTHORIZING
								|| stationManager.StationState == StationState.AUTHORIZED
								|| stationManager.StationState == StationState.TRANSACTION_IN_PROGRESS))
							{
								if (stationManager.PreloadInProgress)
								{
									if (this.CurrentLineItem != null)
									{
									    this.CurrentLineItem.Status = TransactionStatus.LoadPending;
										foreach (SubLineItemDO subLineItem in this.CurrentLineItem.SubLineItems)
                                        {
                                            if (subLineItem.Status == TransactionStatus.InProgress)
                                            {
                                                subLineItem.Status = TransactionStatus.LoadPending;
                                            }
                                        }
									}
								}
								else
								{
									if (this.NonPreloadEquipmentSelection != ""
										&& this.NonPreloadCompartmentSelection != -1)
									{
										CompartmentInfo info = this.GetCompartmentIfValid(this.NonPreloadEquipmentSelection, this.NonPreloadCompartmentSelection);
										info.Loaded = false;
									}
								}

							    this.EndTransaction();
							}
							break;
						}

					case PROCESS_VARIABLE_TYPE.BATCH_DONE_PV:
						{
							if (pv.IsQualityGood
							&& (bool)pv.ServerValue
							&& (stationManager.StationState == StationState.AUTHORIZING
							|| stationManager.StationState == StationState.AUTHORIZED
							|| stationManager.StationState == StationState.TRANSACTION_IN_PROGRESS))
							{

								LineItemDO lineItem = stationManager.GetLineItem(this.LoadArm.IdentityGuid);

								if (lineItem != null)
								{
									stationManager.UpdateLineItem(lineItem);
									stationManager.CloseOutLineItem(lineItem);
									stationManager.SaveTransaction();
								    this.EndTransaction();
								    this.CurrentLineItem = null;
								    this.IssueSelectPrompt(stationManager);
								}
							}
							break;
						}

					case PROCESS_VARIABLE_TYPE.BATCH_AUTHORIZED_PV:
						{
							if (pv.IsQualityGood
							&& (bool)pv.ServerValue
							&& (stationManager.StationState == StationState.AUTHORIZING
							|| stationManager.StationState == StationState.AUTHORIZED
							|| stationManager.StationState == StationState.TRANSACTION_IN_PROGRESS))
							{
							}
							break;
						}


					case PROCESS_VARIABLE_TYPE.BATCH_IN_PROGRESS_PV:
						{
							if (pv.IsQualityGood
							&& (bool)pv.ServerValue
							&& (stationManager.StationState == StationState.AUTHORIZING
							|| stationManager.StationState == StationState.AUTHORIZED
							|| stationManager.StationState == StationState.TRANSACTION_IN_PROGRESS))
							{
								LineItemDO lineItem = stationManager.GetLineItem(this.LoadArm.IdentityGuid);

								if (lineItem == null)
								{
									if (this.LoadArmState == LOADARM_STATE.AUTHORIZED)
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
								    if (this.LoadArmState == LOADARM_STATE.AUTHORIZED)
								    {
								        this.SetState(LOADARM_STATE.INPROGRESS);
								    }

									stationManager.SetTransactionInProgress();
									stationManager.UpdateLineItem(lineItem);
									stationManager.SaveTransaction();
								}

							    if (stationManager.StationState == StationState.AUTHORIZING
							        || stationManager.StationState == StationState.AUTHORIZED)
							    {
							        stationManager.StationState = StationState.TRANSACTION_IN_PROGRESS;
							    }
							}
							break;
						}

					case PROCESS_VARIABLE_TYPE.SWING_ARM_STATUS_PV:
						{
							if (pv.IsQualityGood)
							{
								if (!pv.ServerValue.Equals(this.LastSwingArmStatus))
								{
									// Close out any LineItem on the Alternate Station
									StationManagerClass alternateStationManager = null;

								    if (this.BayB.StationManager != null && stationManager == this.BayA.StationManager)
								    {
								        alternateStationManager = this.BayB.StationManager;
								    }

								    if (this.BayA.StationManager != null && stationManager == this.BayB.StationManager)
								    {
								        alternateStationManager = this.BayA.StationManager;
								    }

								    LineItemDO lineItem = alternateStationManager?.GetLineItem(this.LoadArm.IdentityGuid);
								    if (lineItem != null)
								    {
								        Monitor.Enter(alternateStationManager);
								        try
								        {
								            alternateStationManager.UpdateLineItem(lineItem);
								            alternateStationManager.CloseOutLineItem(lineItem);
								            alternateStationManager.SaveTransaction();
								        }
								        catch (OpcException e)
								        {
								            this.eventLog.WriteEntry("Accuload III LoadArmManager OnInvoke : " + e.Message, EventLogEntryType.Error);
								            this.CommunicationsFailure = true;
								        }
								        catch (Exception e)
								        {
								            this.eventLog.WriteEntry("Accuload III LoadArmManager OnInvoke : PV = " + pv.OPCItemID + " " + e, EventLogEntryType.Error);
								        }
								        finally
								        {
								            Monitor.Exit(alternateStationManager);
								        }
								    }

								    this.ReleaseKeyPad();
								    this.LastSwingArmStatus = pv.ServerValue;
								    this.SetState(LOADARM_STATE.NORMAL);

								    if (stationManager.StationState == StationState.AUTHORIZING
								        || stationManager.StationState == StationState.AUTHORIZED
								        || stationManager.StationState == StationState.TRANSACTION_IN_PROGRESS)
								    {
								        foreach (ProductMapClass recipe in this.LoadArm.ProductRecipeCollection)
								        {
								            if ((this.Bay(stationManager).RecipeMap & (ulong)0x1 << (this.GetRecipeNumber(recipe) - 1)) == 0) continue;

								            ProductMapClass authorizedProduct = stationManager.GetAuthorizedProduct(recipe.AssignedID);

								            string name = stationManager.GetLoadRackDisplayText(recipe.AssignedGuid);

								            AdditiveProfileClass additiveProfile = null;
								            if (authorizedProduct.AdditiveProfileGuid != Guid.Empty)
								            {
								                additiveProfile = this.GetAdditiveProfile(this.Security, authorizedProduct.AdditiveProfileGuid);
								            }

								            ProductClass product =
								                FMChannelHelper.MakeCall<IProducts, ProductClass>(
								                    x => x.Get(this.Security, authorizedProduct.AssignedGuid));

								            if (!this.UpdateRecipe(name, recipe, product, additiveProfile, this.GetRecipeNumber(recipe)))
								            {
								                if (!this.LogOutOfProgramMode())
								                {
								                    return;
								                }

								                this.DisplayMessage("LoadRack|Update Recipe Error", 0, this.MessageTimeout);
								                return;
								            }
								        }

								        if (!this.LogOutOfProgramMode())
								        {
								            return;
								        }

								        this.ReleaseKeyPad();
								        this.PromptForNextBatch(stationManager, true);
								    }
								    else
								    {
								        // When a swing arm swings to a Station treat like a timeout. 
								        stationManager.ProcessMessageTimeout();
								    }
								}
							}
							break;
						}

					case PROCESS_VARIABLE_TYPE.KEYPAD_DATA_PENDING_PV:
						{
							if (pv.IsQualityGood
							&& (bool)pv.ServerValue
							&& this.ResponsePending)
							{
							    this.ResponsePending = false;

								ProcessVariableClass loadArmPv = this.LoadArm.ProcessVariableCollection[0];
								Item[] items = { new Item(new ItemIdentifier(loadArmPv.OPCItemID + ".Keypad Data")) };
								ItemValueResult[] values = this.OpcServerManager.Read(new URL(loadArmPv.URL), items);
							    if (values[0].Quality == Quality.Good)
							    {
							        string keypadData = values[0].Value.ToString();

							        if (!this.ProcessResponseData(stationManager, keypadData))
							        {
							            stationManager.ProcessResponseData(keypadData);
							        }
							    }
							    else
							    {
							        this.eventLog.WriteEntry("DANLOAD 6000 OnInvoke : Keypad Data OPC Quality Bad " + pv.OPCItemID, EventLogEntryType.Error);
							    }
							}
							break;
						}

					case PROCESS_VARIABLE_TYPE.PROMPT_TIMEOUT_PV:
						{
							if (pv.IsQualityGood
							&& (bool)pv.ServerValue
							&& this.ResponsePending)
							{
							    this.ResponsePending = false;
							    this.CommunicationsFailure = false;

							    if (!this.ProcessMessageTimeout(stationManager))
							    {
							        stationManager.ProcessMessageTimeout();
							    }
							}
							break;
						}

					case PROCESS_VARIABLE_TYPE.RECIPE_SELECTED_PV:
						{
							if (pv.IsQualityGood
							&& (bool)pv.ServerValue
							&& this.LoadArmState == LOADARM_STATE.SELECT_RECIPE_PROMPT)
							{
								ProcessVariableClass loadArmPv = this.LoadArm.ProcessVariableCollection[0];

								ItemValue clearDisplayItemValue = new ItemValue(new ItemIdentifier(loadArmPv.OPCItemID + ".Clear Display"));
							    this.OpcServerManager.Write(new URL(loadArmPv.URL), new[] { clearDisplayItemValue });

								Item requestSelectedRecipeItem = new Item(new ItemIdentifier(loadArmPv.OPCItemID + ".Request Selected Recipe"));
								ItemValueResult[] values = this.OpcServerManager.Read(new URL(loadArmPv.URL), new[] { requestSelectedRecipeItem });
								if (values[0].Quality == Quality.Good)
								{
								    this.SelectedRecipe = Convert.ToInt32(values[0].Value);
								    this.UpdateAdditiveConfiguration(stationManager);
								    this.Authorize(stationManager, this.MaximumPreset);
								}
							}
							break;
						}

					case PROCESS_VARIABLE_TYPE.PRESET_VOLUME_ENTERED_PV:
						{
							if (pv.IsQualityGood
							&& (bool)pv.ServerValue
							&& this.LoadArmState == LOADARM_STATE.PRESET_VOLUME_PROMPT)
							{
								ProcessVariableClass loadArmPv = this.LoadArm.ProcessVariableCollection[0];
								Item requestPresetVolumeItem = new Item(new ItemIdentifier(loadArmPv.OPCItemID + ".Request Preset Volume"));
								ItemValueResult[] values = this.OpcServerManager.Read(new URL(loadArmPv.URL), new[] { requestPresetVolumeItem });
								if (values[0].Quality == Quality.Good)
								{
								    this.PresetVolume = Convert.ToInt32(values[0].Value);
								    this.AuthorizeBatch(this.PresetVolume);
								}
							}
							break;
						}

					case PROCESS_VARIABLE_TYPE.GET_KEY_PV:
						{
						    this.CheckForStopKey();
							break;
						}

					case PROCESS_VARIABLE_TYPE.PRIMARY_ALARM_PV:
						{
							// Display Message or Display Menu may fail if
							// a Primary Alarm exists.  After reset try
							// again.
							if (pv.IsQualityGood
							&& (bool)pv.ServerValue == false
							&& this.CommunicationsFailure)
							{
							    this.CommunicationsFailure = false;

							    if (!this.ProcessMessageTimeout(stationManager))
							    {
							        stationManager.ProcessMessageTimeout();
							    }
							}
							break;
						}

					case PROCESS_VARIABLE_TYPE.MANUAL_PV:
						{
							if (pv.IsQualityGood
							&& (bool)pv.ServerValue == false)
							{
							    this.CommunicationsFailure = false;

							    if (!this.ProcessMessageTimeout(stationManager))
							    {
							        stationManager.ProcessMessageTimeout();
							    }
							}
							break;
						}

					default:
				        this.eventLog.WriteEntry("DANLOAD 6000 LoadArmManager OnInvoke : Unknown PV : " + pv.OPCItemID, EventLogEntryType.Error);
						break;
				}
			}
    		catch (OpcException e)
			{
			    this.eventLog.WriteEntry("DANLOAD 6000 LoadArmManager OnInvoke : PV = " + pv.OPCItemID + " Error = " + e.Message, EventLogEntryType.Error);
			    this.CommunicationsFailure = true;
			}
			catch (Exception e)
			{
			    this.eventLog.WriteEntry("DANLOAD 6000 LoadArmManager OnInvoke : PV = " + pv.OPCItemID + " Error = " + e.Message, EventLogEntryType.Error);
			}
			finally
			{
				Monitor.Exit(stationManager);
			}
		}

		private AdditiveProfileClass GetAdditiveProfile(SecurityClass Security, Guid guid)
		{
			return FMChannelHelper.MakeCall<IAdditiveProfiles, AdditiveProfileClass>(
																	 x =>
																	 x.Get(Security, guid)
																);
		}

		protected void UpdateAdditiveConfiguration(StationManagerClass StationManager)
		{
			ProductMapClass Recipe = null;
			int RecipeNumber = 0;
			for (int Index = 0; Index < 32; Index++)
			{
				if ((this.Bay(StationManager).RecipeMap & (ulong)1 << Index) != 0)
				{
					RecipeNumber++;
					if (RecipeNumber == this.SelectedRecipe)
					{
						Recipe = this.LoadArm.ProductRecipeCollection[Index];
						break;
					}
				}
			}

			if (Recipe == null)
				return;

			ProductMapClass AuthorizedProduct = StationManager.GetAuthorizedProduct(Recipe.AssignedID);

			// If no Additive Profile nothing to do because 
			if (AuthorizedProduct.AdditiveProfileGuid == Guid.Empty)
				return;

			AdditiveProfileClass AdditiveProfile = FMChannelHelper.MakeCall<IAdditiveProfiles, AdditiveProfileClass>(
																	 x =>
																	 x.Get(this.Security, AuthorizedProduct.AdditiveProfileGuid)
																);

			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			ArrayList ItemValues = new ArrayList();

			foreach (ProductMapClass Additive in AdditiveProfile.AdditiveCollection)
			{
				ProductMapClass AdditiveInjector = this.GetAdditive(Additive.AssignedGuid);
				if (AdditiveInjector == null)
					throw new Exception("UpdateAdditiveConfiguration : Additive Injector Not Found");

				ItemValue RateItemValue = new ItemValue(LoadArmPV.OPCItemID + ".Set Program Code Value");
				RateItemValue.Value = ((AdditiveInjector.PresetNumber - 1) * 5 + 140) + " " + Additive._AdditiveRate.Value.ToString("F0");
				ItemValues.Add(RateItemValue);

				ItemValue CycleAmountItemValue = new ItemValue(LoadArmPV.OPCItemID + ".Set Program Code Value");
				CycleAmountItemValue.Value = ((AdditiveInjector.PresetNumber - 1) * 5 + 143) + " " + ((1000 / Additive._AdditiveRate.Value) * Additive._AdditiveCycleVolume.Value * 10000).ToString("F0");
				ItemValues.Add(CycleAmountItemValue);
			}

			try
			{
			    this.OpcServerManager.Write(new URL(LoadArmPV.URL), (ItemValue[])ItemValues.ToArray(typeof(ItemValue)));
			}
			catch (Exception e)
			{
			    this.eventLog.WriteEntry("DANLOAD 6000 LoadArmManager UpdateAdditiveConfiguraiton : " + e.Message, EventLogEntryType.Error);
			}
		}

		protected override string SwingArmPosition
		{
			get
			{
			    if (typeof(byte).IsInstanceOfType(this.SwingArmStatusPV.ServerValue))
			    {
			        if ((byte)this.SwingArmStatusPV.ServerValue == 1)
						return "A";
			        if ((byte)this.SwingArmStatusPV.ServerValue == 2)
			            return "B";
			        return "?";
			    }
			    return "A";
			}
		}


		protected void CheckForStopKey()
		{
			if (this.LastKeyPressedPV.IsQualityGood
			&& (byte)this.LastKeyPressedPV.ServerValue == 111)
			{
				if (this.LoadArmState == LOADARM_STATE.AUTHORIZED
				|| this.LoadArmState == LOADARM_STATE.INPROGRESS
				|| this.LoadArmState == LOADARM_STATE.SELECT_PROMPT)
					return;

			    this.ProcessStopKey();
			}
		}

		protected void ProcessStopKey()
		{
			StationManagerClass StationManager = this.GetStationManager();
			if (StationManager != null && StationManager.IsRemoteAuthorized == false)
			{
				switch (StationManager.StationState)
				{
					// Station States corresponding to control having been turned over to load arm
					case StationState.AUTHORIZING:
					case StationState.AUTHORIZED:
					case StationState.TRANSACTION_IN_PROGRESS:
						{
							switch (this.LoadArmState)
							{
								case LOADARM_STATE.COMPARTMENT_PROMPT:
								case LOADARM_STATE.EQUIPMENTID_PROMPT:
								case LOADARM_STATE.SPLASH_BLEND_COMPONENT_PROMPT:
									{
									    this.CancelPresetting(StationManager);
									    this.IssueSelectPrompt(StationManager);
										break;
									}

								case LOADARM_STATE.AUTHORIZED:
								case LOADARM_STATE.SELECT_RECIPE_PROMPT:
								case LOADARM_STATE.PRESET_VOLUME_PROMPT:
									{
									    this.EndTransaction();
									    this.CancelPresetting(StationManager);
									    this.IssueSelectPrompt(StationManager);
										break;
									}


								case LOADARM_STATE.INPROGRESS:
									break;

								case LOADARM_STATE.BATCH_COMPLETE_PROMPT:
							        this.SetFinishedLoading();
									break;

								case LOADARM_STATE.SELECT_PROMPT:
							        this.SetFinishedLoading();
									break;

								default:
							        this.IssueSelectPrompt(StationManager);
									break;
							}
							break;
						}

					default:
						StationManager.ProcessStopKey();
						break;
				}
			}
		}

		protected override bool IsFlowing()
		{
			return false;
		}


		private void IsPreparedForMessage()
		{
			if (this.ManualPV != null
			&& this.ManualPV.IsQualityGood
			&& (bool)this.ManualPV.ServerValue)
				throw new OpcException("Load Arm is in Manual");

			if (this.PrimaryAlarmPV != null
			&& this.PrimaryAlarmPV.IsQualityGood
			&& (bool)this.PrimaryAlarmPV.ServerValue)
				throw new OpcException("Load Arm Primary Alarm");
		}

		public override int DisplayMessage(string message, int responseLength, int messageTimeout)
		{
			StationManagerClass StationManager = this.GetStationManager();
			if (StationManager == null)
				throw new OpcException("No Station Available");

		    this.IsPreparedForMessage();

			string Message = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.SiteManager.Site.SiteGuid, message)
																);
			// Prefix message with Bay Identifier
			// When Station has Swing Arms provide Bay Prefix
			if (StationManager.HasSwingArms)
			{
				if (this.LoadArm.LoadRackText != "")
					Message = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|Bay") + " " + StationManager.Station.SwingArmPosition + " " + this.LoadArm.LoadRackText + ": " + Message
																);
				else
					Message = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|Bay") + " " + StationManager.Station.SwingArmPosition + ": " + Message
																);
			}
			// Prefix message with LoadArm Identifier
			else if (this.LoadArm.LoadRackText != "")
				Message = this.LoadArm.LoadRackText + ": " + Message;

			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];


			// Maximum DANLOAD 6000 Prompt Length
			if (responseLength > 8)
				responseLength = 8;

			ArrayList Items = new ArrayList();

			ItemValue TimeoutItemValue = new ItemValue(LoadArmPV.OPCItemID + ".Timeout Operation");
			Items.Add(TimeoutItemValue);

			ItemValue ClearDisplayItemValue = new ItemValue(new ItemIdentifier(LoadArmPV.OPCItemID + ".Clear Display"));
			Items.Add(ClearDisplayItemValue);

			ItemValue DisplayMessageItemValue = new ItemValue(LoadArmPV.OPCItemID + ".Display Message");
			DisplayMessageItemValue.Value = responseLength + " 0 " + messageTimeout + " " + Message;
			Items.Add(DisplayMessageItemValue);

		    this.OpcServerManager.Write(new URL(LoadArmPV.URL), (ItemValue[])Items.ToArray(typeof(ItemValue)));

		    this.ResponsePending = true;

			return 0;
		}

		public override void PromptForPIN(string StockMessage, int ResponseLength, int MessageTimeout)
		{
			StationManagerClass StationManager = this.GetStationManager();
			if (StationManager == null)
				throw new OpcException("No Station Available");

		    this.IsPreparedForMessage();

			string Message = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.SiteManager.Site.SiteGuid, StockMessage)
																);

			// Prefix message with Bay Identifier
			// When Station has Swing Arms provide Bay Prefix
			if (StationManager.HasSwingArms)
			{
				if (this.LoadArm.LoadRackText != "")
					Message = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|Bay") + " " + StationManager.Station.SwingArmPosition + " " + this.LoadArm.LoadRackText + ": " + Message
																);

				else
					Message = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|Bay") + " " + StationManager.Station.SwingArmPosition + ": " + Message
																);

			}

				// Prefix message with LoadArm Identifier
			else if (this.LoadArm.LoadRackText != "")
				Message = this.LoadArm.LoadRackText + ": " + Message;

			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];


			// Maximum DANLOAD 6000 Prompt Length
			if (ResponseLength > 8)
				ResponseLength = 8;

			ArrayList Items = new ArrayList();

			ItemValue TimeoutItemValue = new ItemValue(LoadArmPV.OPCItemID + ".Timeout Operation");
			Items.Add(TimeoutItemValue);

			ItemValue ClearDisplayItemValue = new ItemValue(new ItemIdentifier(LoadArmPV.OPCItemID + ".Clear Display"));
			Items.Add(ClearDisplayItemValue);

			ItemValue DisplayMessageItemValue = new ItemValue(LoadArmPV.OPCItemID + ".Display Message");
			DisplayMessageItemValue.Value = ResponseLength + " 1 " + MessageTimeout + " " + Message;
			Items.Add(DisplayMessageItemValue);

		    this.OpcServerManager.Write(new URL(LoadArmPV.URL), (ItemValue[])Items.ToArray(typeof(ItemValue)));

		    this.ResponsePending = true;
		}


		protected virtual int MaxDisplayLineSize
		{
			get { return 128; }
		}

		public override void DisplayMenu(DisplayMenuParameters parameters)
		{
			StationManagerClass StationManager = this.GetStationManager();
			if (StationManager == null)
				throw new OpcException("No Station Available");

		    this.IsPreparedForMessage();

			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];
			string Message = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(StationManager.Station.SiteGuid, parameters.Caption)
																);
			Message += ": ";

			for (int nLoop = 0; nLoop < parameters.Menu.Length; ++nLoop)
			{
				string Value = parameters.Menu[nLoop];

				if (parameters.ApplyDataDictionary)
					Value = this.GetDataDictionaryValueByKey(StationManager.Station.SiteGuid, Value);

				Value = (nLoop + 1) + ". " + Value + " ";

				if (Value.Length + Message.Length < this.MaxDisplayLineSize)
					Message += Value;
			}

			ArrayList Items = new ArrayList();

			ItemValue TimeoutItemValue = new ItemValue(LoadArmPV.OPCItemID + ".Timeout Operation");
			Items.Add(TimeoutItemValue);

			ItemValue ClearDisplayItemValue = new ItemValue(new ItemIdentifier(LoadArmPV.OPCItemID + ".Clear Display"));
			Items.Add(ClearDisplayItemValue);

			ItemValue DisplayMessageItemValue = new ItemValue(LoadArmPV.OPCItemID + ".Display Message");
			DisplayMessageItemValue.Value = "2 0 " + parameters.MenuTimeout + " " + Message;
			Items.Add(DisplayMessageItemValue);

		    this.OpcServerManager.Write(new URL(LoadArmPV.URL), (ItemValue[])Items.ToArray(typeof(ItemValue)));

		    this.CurrentMenuParameters = parameters;

		    this.ResponsePending = true;
		}

		private string GetDataDictionaryValueByKey(Guid guid, string key)
		{
			return FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(guid, key)
																);
		}

		public override void ReadBatchRecipe(
			string BatchNumber,
			Server Server,
			out ItemValueResult Recipe)
		{
			ItemValueResult Value = new ItemValueResult();
			Value.Quality = Quality.Good;

			StationManagerClass StationManager = this.GetStationManager();

			// RecipeMap defines the authorized recipes
			// RecipeNumber is the recipe in the DANLOAD 6000
			// Index is the recipe in the LoadArm ProductRecipeCollection
			int RecipeNumber = 0;
			for (int Index = 0; Index < 32; Index++)
			{
				if ((this.Bay(StationManager).RecipeMap & (ulong)1 << Index) != 0)
				{
					RecipeNumber++;
					if (RecipeNumber == this.SelectedRecipe)
					{
						Value.Value = Index + 1;
						break;
					}
				}
			}

			Recipe = Value;
		}

		public void ReadComponentData(
			Server Server,
			int Component,
			out ItemValueResult GrossTotalizer,
			out ItemValueResult NetTotalizer,
			out ItemValueResult GrossVolume,
			out ItemValueResult NetVolume,
			out ItemValueResult AverageTemperature,
			out ItemValueResult AverageDensity)
		{
			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			string TagPrefix = LoadArmPV.OPCItemID + ".Component Values.";

			ArrayList Items = new ArrayList();

			Items.Add(new Item(new ItemIdentifier(TagPrefix + Component + ".Gross Totalizer")));
			Items.Add(new Item(new ItemIdentifier(TagPrefix + Component + ".Net Totalizer")));
			Items.Add(new Item(new ItemIdentifier(TagPrefix + Component + ".Gross Volume")));
			Items.Add(new Item(new ItemIdentifier(TagPrefix + Component + ".Net Volume")));
			Items.Add(new Item(new ItemIdentifier(TagPrefix + Component + ".Average Temperature")));
			Items.Add(new Item(new ItemIdentifier(TagPrefix + Component + ".Average Density")));

			foreach (Item item in Items)
				item.MaxAgeSpecified = true;

			((Item)Items[0]).MaxAgeSpecified = false;

			ItemValueResult[] Values = Server.Read((Item[])Items.ToArray(typeof(Item)));

			GrossTotalizer = Values[0];
			NetTotalizer = Values[1];
			GrossVolume = Values[2];
			NetVolume = Values[3];
			AverageTemperature = Values[4];
			AverageDensity = Values[5];
		}

		public void ReadBatchData(
			Server Server,
			out ItemValueResult[] ComponentGrossVolume,
			out ItemValueResult[] ComponentNetVolume,
			out ItemValueResult[] ComponentAverageTemperature,
			out ItemValueResult[] ComponentAverageDensity,
			out ItemValueResult[] AdditiveGrossVolume)
		{
			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			string TagPrefix = LoadArmPV.OPCItemID + ".Batch Values.";

			ArrayList Items = new ArrayList();

			for (int Component = 1; Component < 5; Component++)
				Items.Add(new Item(new ItemIdentifier(TagPrefix + "Component." + Component + ".Gross Volume")));

			for (int Component = 1; Component < 5; Component++)
				Items.Add(new Item(new ItemIdentifier(TagPrefix + "Component." + Component + ".Net Volume")));

			for (int Component = 1; Component < 5; Component++)
				Items.Add(new Item(new ItemIdentifier(TagPrefix + "Component." + Component + ".Average Temperature")));

			for (int Component = 1; Component < 5; Component++)
				Items.Add(new Item(new ItemIdentifier(TagPrefix + "Component." + Component + ".Average Density")));

			for (int Additive = 1; Additive < 7; Additive++)
				Items.Add(new Item(new ItemIdentifier(TagPrefix + "Additive." + Additive + ".Gross Volume")));

			foreach (Item item in Items)
				item.MaxAgeSpecified = true;

			((Item)Items[0]).MaxAgeSpecified = false;

			ItemValueResult[] Values = Server.Read((Item[])Items.ToArray(typeof(Item)));

			ComponentGrossVolume = new ItemValueResult[4];
			for (int Component = 0; Component < 4; Component++)
				ComponentGrossVolume[Component] = Values[Component];

			ComponentNetVolume = new ItemValueResult[4];
			for (int Component = 0; Component < 4; Component++)
				ComponentNetVolume[Component] = Values[4 + Component];

			ComponentAverageTemperature = new ItemValueResult[4];
			for (int Component = 0; Component < 4; Component++)
				ComponentAverageTemperature[Component] = Values[8 + Component];

			ComponentAverageDensity = new ItemValueResult[4];
			for (int Component = 0; Component < 4; Component++)
				ComponentAverageDensity[Component] = Values[12 + Component];

			AdditiveGrossVolume = new ItemValueResult[6];
			for (int Additive = 0; Additive < 6; Additive++)
				AdditiveGrossVolume[Additive] = Values[16 + Additive];

		}

		public void ReadComponentNonResettableTotals(
			Server Server,
			out ItemValueResult[] NonResettableGrossVolumes)
		{
			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			string TagPrefix = LoadArmPV.OPCItemID + ".Component Totalizers.";

			ArrayList Items = new ArrayList();

			for (int Component = 1; Component < 5; Component++)
				Items.Add(new Item(new ItemIdentifier(TagPrefix + Component + ".Gross Volume")));

			foreach (Item item in Items)
				item.MaxAgeSpecified = true;

			((Item)Items[0]).MaxAgeSpecified = false;

			ItemValueResult[] Values = Server.Read((Item[])Items.ToArray(typeof(Item)));

			NonResettableGrossVolumes = Values;
		}

		public override void ReadPresetAmount(
			Server Server,
			out ItemValueResult PresetAmount)
		{
			PresetAmount = new ItemValueResult();
			PresetAmount.Value = Convert.ToDouble(this.PresetVolume);
			PresetAmount.Quality = Quality.Good;
		}

		public void ReadAdditiveNonResettableTotals(
			Server Server,
			out ItemValueResult[] NonResettableGrossVolumes)
		{
			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			string TagPrefix = LoadArmPV.OPCItemID + ".Additive Totalizers.";

			ArrayList Items = new ArrayList();

			for (int Additive = 1; Additive < 7; Additive++)
				Items.Add(new Item(new ItemIdentifier(TagPrefix + Additive + ".Gross Volume")));

			foreach (Item item in Items)
				item.MaxAgeSpecified = true;

			((Item)Items[0]).MaxAgeSpecified = false;

			ItemValueResult[] Values = Server.Read((Item[])Items.ToArray(typeof(Item)));

			NonResettableGrossVolumes = Values;
		}

		public override void CaptureMeterValues()
		{
			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			Server Server = new Server(new Factory(), new URL(LoadArmPV.URL));
			NetworkCredential Credentials = null;
			Server.Connect(new ConnectData(Credentials));

			// Components
			ItemValueResult[] ComponentNonResettableTotal;

		    this.ReadComponentNonResettableTotals(
				Server,
				out ComponentNonResettableTotal);

			foreach (ProductMapClass ArmComponent in this.LoadArm.ComponentCollection)
			{
				if (ComponentNonResettableTotal[ArmComponent.PresetNumber - 1].Quality != Quality.Good) this.eventLog.WriteEntry("CaptureMeterValues : Non-Resettable Gross Volume OPC Quality Bad " + ComponentNonResettableTotal[ArmComponent.PresetNumber - 1].ItemName, EventLogEntryType.Error);
				else
					ArmComponent.MeterValue = Convert.ToDouble(ComponentNonResettableTotal[ArmComponent.PresetNumber - 1].Value);
			}

			// Additives
			ItemValueResult[] AdditiveNonResettableTotal;

		    this.ReadAdditiveNonResettableTotals(
				Server,
				out AdditiveNonResettableTotal);

			foreach (ProductMapClass AdditiveInjector in this.LoadArm.AdditiveInjectorCollection)
			{
				if (AdditiveNonResettableTotal[AdditiveInjector.PresetNumber - 1].Quality != Quality.Good) this.eventLog.WriteEntry("CaptureMeterValues : Non-Resettable Gross Volume OPC Quality Bad " + AdditiveNonResettableTotal[AdditiveInjector.PresetNumber - 1].ItemName, EventLogEntryType.Error);
				else
					AdditiveInjector.MeterValue = Convert.ToDouble(AdditiveNonResettableTotal[AdditiveInjector.PresetNumber - 1].Value);
			}

			Server.Disconnect();
			Server.Dispose();
		}


		public override bool IssueSelectRecipePrompt(StationManagerClass StationManager, double MaximumPreset)
		{
			this.MaximumPreset = MaximumPreset;

			int NumberOfAuthorizedRecipes = 0;
			for (int Index = 0; Index < this.LoadArm.ProductRecipeCollection.Count; Index++)
			{
				if ((this.Bay(StationManager).RecipeMap & ((ulong)1 << Index)) != 0)
				{
					NumberOfAuthorizedRecipes++;
				}
			}

			if (NumberOfAuthorizedRecipes <= 1)
			{
			    this.SelectedRecipe = 1;
			    this.UpdateAdditiveConfiguration(StationManager);
				return false;
			}

			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

		    ItemValue promptRecipeItemValue = new ItemValue(LoadArmPV.OPCItemID + ".Prompt Recipe") { Value = 0 };

		    this.OpcServerManager.Write(new URL(LoadArmPV.URL), new[] { promptRecipeItemValue });
		    this.SetState(LOADARM_STATE.SELECT_RECIPE_PROMPT);

			return true;
		}

		public bool IssuePresetVolumePrompt(StationManagerClass stationManager)
		{
			if (stationManager.PreloadInProgress)
			{
				if (this.CurrentLineItem == null)
					throw new Exception("Danload6000LoadArmManager : IssuePresetVolume - no current line item");

			    this.PresetVolume = Convert.ToInt32(this.CurrentLineItem.PresetAmount.Value);
				return false;
			}

			EngineeringUnit volumeUnit = (stationManager.Order.VolumeUnits != 0) ? stationManager.Order.VolumeUnits :
				(stationManager.CurrentTransactionAlias.VolumeUnits != 0) ? stationManager.CurrentTransactionAlias.VolumeUnits : this.SiteManager.Site.VolumeUnits;

		    SIDouble maxPreset = new SIDouble
		                         {
		                             Units = volumeUnit,
		                             SIValue = this.SiteManager.Site._MaximumLoadAmount.SIValue
		                         };

		    if (this.NonPreloadEquipmentSelection != ""
			&& this.NonPreloadCompartmentSelection != -1)
			{

				CompartmentInfo info = this.GetCompartmentIfValid(this.NonPreloadEquipmentSelection, this.NonPreloadCompartmentSelection);

			    SIDouble maxFill = new SIDouble { Units = volumeUnit, SIValue = info.MaxFill };

			    if (maxFill.Value < maxPreset.Value)
					maxPreset.Value = maxFill.Value;
			}

			if (stationManager.Order != null
			&& stationManager.Order.LineItems.Count == 1)
			{
				LineItemDO orderLineItem = stationManager.Order.LineItems[0];

				EngineeringUnit orderVolumeUnits = (orderLineItem.VolumeUnits != 0) ? orderLineItem.VolumeUnits : stationManager.SiteManager.Site.VolumeUnits;

				SIDouble GrossQuantityRemaining = new SIDouble();
				GrossQuantityRemaining.Units = orderVolumeUnits;
				GrossQuantityRemaining.Value = orderLineItem.GrossQuantityRemaining;

				SIDouble NetQuantityRemaining = new SIDouble();
				NetQuantityRemaining.Units = orderVolumeUnits;
				NetQuantityRemaining.Value = orderLineItem.NetQuantityRemaining;

				SIDouble gross = new SIDouble();
				SIDouble net = new SIDouble();
				foreach (LineItemDO TransLineItem in stationManager.Transaction.LineItems)
				{
					orderVolumeUnits = (TransLineItem.VolumeUnits != 0) ? TransLineItem.VolumeUnits : stationManager.SiteManager.Site.VolumeUnits;
					gross.Units = orderVolumeUnits;
					net.Units = orderVolumeUnits;
					gross.Value = TransLineItem.Quantity.Gross;
					net.Value = TransLineItem.Quantity.Net;
					GrossQuantityRemaining.SIValue -= gross.SIValue;
					NetQuantityRemaining.SIValue -= net.SIValue;
				}


				if (stationManager.SiteManager.Site.LoadByNet
					&& maxPreset.SIValue > NetQuantityRemaining.SIValue)
					maxPreset.SIValue = NetQuantityRemaining.SIValue;

				if (!stationManager.SiteManager.Site.LoadByNet
					&& maxPreset.SIValue > GrossQuantityRemaining.SIValue)
					maxPreset.SIValue = GrossQuantityRemaining.SIValue;


			}

			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			ItemValue PromptRecipeItemValue = new ItemValue(LoadArmPV.OPCItemID + ".Prompt Preset Volume");
			PromptRecipeItemValue.Value = maxPreset.Value.ToString("########") + " " + maxPreset.Value.ToString("########") + " 0";

		    this.OpcServerManager.Write(new URL(LoadArmPV.URL), new[] { PromptRecipeItemValue });
		    this.SetState(LOADARM_STATE.PRESET_VOLUME_PROMPT);

			return true;
		}

		public override bool Authorize(StationManagerClass stationManager, double preset)
		{
			try
			{
				ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

				ItemValue AuthorizeItemValue = new ItemValue(new ItemIdentifier(LoadArmPV.OPCItemID + ".Authorize Transaction"));
				AuthorizeItemValue.Value = this.SelectedRecipe + " " + this.SwingArmStatusPV.ServerValue;

			    this.OpcServerManager.Write(new URL(LoadArmPV.URL), new[] { AuthorizeItemValue });

				Item AuthorizeTransactionItem = new Item(new ItemIdentifier(LoadArmPV.OPCItemID + ".Authorize Transaction"));
				ItemValueResult[] Values = this.OpcServerManager.Read(new URL(LoadArmPV.URL), new[] { AuthorizeTransactionItem });
				if (Values[0].Quality == Quality.Good) this.TransactionNumber = Convert.ToInt32(Values[0].Value);

			}
			catch (Exception e)
			{
			    this.eventLog.WriteEntry("DANLOAD 6000 LoadArmManager Authorize : " + e.Message, EventLogEntryType.Error);
			    this.DisplayMessage("LoadRack|Authorize Transaction Error", 0, this.MessageTimeout);
				return false;
			}

			return true;
		}

		public void AuthorizeBatch(double Preset)
		{
			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			ItemValue AuthorizeItemValue = new ItemValue(new ItemIdentifier(LoadArmPV.OPCItemID + ".Authorize Batch"));
			AuthorizeItemValue.Value = Preset.ToString("########") + " " + this.PromptTimeout;

		    this.OpcServerManager.Write(new URL(LoadArmPV.URL), new[] { AuthorizeItemValue });

			Item AuthorizeBatchItem = new Item(new ItemIdentifier(LoadArmPV.OPCItemID + ".Authorize Batch"));
			ItemValueResult[] Values = this.OpcServerManager.Read(new URL(LoadArmPV.URL), new[] { AuthorizeBatchItem });
			if (Values[0].Quality == Quality.Good) this.BatchNumber = Convert.ToInt32(Values[0].Value);

		    this.LoadArmState = LOADARM_STATE.AUTHORIZED;
		}


		public override bool AllocateRecipes(ulong recipeMap)
		{
			StationManagerClass StationManager = this.GetStationManager();
			if (StationManager == null)
				return false;

			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			int Offset = 0;
			int RecipeNumber = 0;

			foreach (ProductMapClass Recipe in this.LoadArm.ProductRecipeCollection)
			{
				if ((recipeMap & (ulong)1 << Offset++) == 0)
					continue;

				ArrayList ItemValues = new ArrayList();

				ProductMapClass AuthorizedProduct = StationManager.GetAuthorizedProduct(Recipe.AssignedID);

				string Name = StationManager.GetLoadRackDisplayText(Recipe.AssignedGuid);

				if (Name.Length > 14)
					Name = Name.Substring(0, 14);

				Name = Name.PadRight(14, ' ');

				ulong AdditiveMask = 0;

				if (AuthorizedProduct.AdditiveProfileGuid != Guid.Empty)
				{
					AdditiveProfileClass AdditiveProfile = this.GetAdditiveProfile(this.Security, AuthorizedProduct.AdditiveProfileGuid);

					foreach (ProductMapClass Additive in AdditiveProfile.AdditiveCollection)
					{
						ProductMapClass AdditiveInjector = this.GetAdditive(Additive.AssignedGuid);
						AdditiveMask |= ((ulong)0x1 << AdditiveInjector.PresetNumber - 1);
					}
				}

				Name += AdditiveMask.ToString("D2");

				ItemValue NameItemValue = new ItemValue(LoadArmPV.OPCItemID + ".Set Program Code Value");
				NameItemValue.Value = (RecipeNumber * 6 + 481) + " " + Name;
				ItemValues.Add(NameItemValue);

				ProductMapCollectionClass RecipeComponentCollection;
				if (Recipe.AssignedProductType == ProductType.BlendProduct)
					RecipeComponentCollection = this.EnumerateByAssignedToGuidAndType(this.Security, Recipe.AssignedGuid, PRODUCT_MAP_TYPE.BLEND_COMPONENT_MAP);
				else
				{
					RecipeComponentCollection = new ProductMapCollectionClass();
					Recipe.BlendPercentage = 100.0;
					RecipeComponentCollection.Add(Recipe);
				}

				for (int ComponentIndex = 0; ComponentIndex < 4; ComponentIndex++)
				{
					ItemValue ComponentPercentageItemValue = new ItemValue(LoadArmPV.OPCItemID + ".Set Program Code Value");
					ComponentPercentageItemValue.Value = (RecipeNumber * 6 + ComponentIndex + 482) + " 0";

					if (ComponentIndex < this.LoadArm.ComponentCollection.Count)
					{
						ProductMapClass loadArmComponent = this.LoadArm.ComponentCollection[ComponentIndex];
						foreach (ProductMapClass blendComponent in RecipeComponentCollection)
						{
							if (loadArmComponent.AssignedGuid == blendComponent.AssignedGuid)
							{
								ComponentPercentageItemValue.Value = (RecipeNumber * 6 + ComponentIndex + 482) + " " + Convert.ToInt16(blendComponent.BlendPercentage * 100).ToString("D4");
								break;
							}
						}
					}

					ItemValues.Add(ComponentPercentageItemValue);
				}

				// Sequence/Low Proportion
			    string sequenceOrLowProportion = ((Recipe.PresetNumber / 1000) % 10)
			                                     + ((Recipe.PresetNumber / 100) % 10)
			                                     + ((Recipe.PresetNumber / 10) % 10)
			                                     + (Recipe.PresetNumber % 10).ToString();

			    ItemValue sequenceOrLowProportionItemValue = new ItemValue(LoadArmPV.OPCItemID + ".Set Program Code Value")
			                                                 {
			                                                     Value = (RecipeNumber * 6 + 486) + " " + sequenceOrLowProportion
			                                                 };
			    ItemValues.Add(sequenceOrLowProportionItemValue);

				try
				{
				    this.OpcServerManager.Write(new URL(LoadArmPV.URL), (ItemValue[])ItemValues.ToArray(typeof(ItemValue)));
				}
				catch (Exception e)
				{
				    this.eventLog.WriteEntry("DANLOAD 6000 LoadArmManager AllocateRecipes : " + e.Message, EventLogEntryType.Error);
					return false;
				}

				RecipeNumber++;
			}

			ItemValue numberOfRecipesItemValue = new ItemValue(LoadArmPV.OPCItemID + ".Set Program Code Value");
			numberOfRecipesItemValue.Value = (480) + " " + RecipeNumber;

			try
			{
			    this.OpcServerManager.Write(new URL(LoadArmPV.URL), new[] { numberOfRecipesItemValue });
			}
			catch (Exception e)
			{
			    this.eventLog.WriteEntry("DANLOAD 6000 LoadArmManager AllocateRecipes : " + e.Message, EventLogEntryType.Error);
				return false;
			}

			return true;
		}

		private ProductMapCollectionClass EnumerateByAssignedToGuidAndType(SecurityClass Security, Guid guid, PRODUCT_MAP_TYPE productMapType)
		{
			return FMChannelHelper.MakeCall<IProductMaps, ProductMapCollectionClass>(
																	 x =>
																	 x.EnumerateByAssignedToGuidAndType(Security, guid, productMapType)
																);
		}

		public override int NumberOfOffsets
		{
			get { return 30; }
		}

		public override void Start()
		{
		}

		public override void Stop()
		{
		}

		public override void Unauthorize()
		{
			StationManagerClass StationManager = this.GetStationManager();
			if (StationManager == null)
				return;

			if (this.LoadArmState == LOADARM_STATE.AUTHORIZED) this.EndBatch();

		    this.SetState(LOADARM_STATE.NORMAL);
		    this.CurrentLineItem = null;
		}


		protected override void ProcessSplashBlendComponentPromptResponse(StationManagerClass StationManager, string Response)
		{
			int nSelection = Convert.ToInt32(Response);

			if (this.CurrentMenuParameters == null
				|| nSelection > this.CurrentMenuParameters.Menu.Length)
			{
			    this.IssueSplashBlendComponentInvalidMessage();
				return;
			}

			if (nSelection == 0)
				Response = StationManagerClass.EscapeString;
			else
				Response = this.CurrentMenuParameters.Menu[nSelection - 1];

			base.ProcessSplashBlendComponentPromptResponse(StationManager, Response);
		}


		public override void SendEndTransaction()
		{
			if (this.TransactionAuthorizedPV.IsQualityGood
			&& (bool)this.TransactionAuthorizedPV.ServerValue)
			{
			    this.IsPreparedForMessage();

				ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

				ArrayList ItemValues = new ArrayList();

				ItemValue EndTransactionItemValue = new ItemValue(new ItemIdentifier(LoadArmPV.OPCItemID + ".End Transaction"));
				EndTransactionItemValue.Value = this.SwingArmStatusPV.ServerValue.ToString();
				ItemValues.Add(EndTransactionItemValue);

				ItemValue ClearDisplayItemValue = new ItemValue(new ItemIdentifier(LoadArmPV.OPCItemID + ".Clear Display"));
				ItemValues.Add(ClearDisplayItemValue);

			    this.OpcServerManager.Write(new URL(LoadArmPV.URL), (ItemValue[])ItemValues.ToArray(typeof(ItemValue)));
			}
		}


		public override bool UpdateReferenceDensity(StationManagerClass stationManager)
		{
			ProcessVariableClass loadArmPv = this.LoadArm.ProcessVariableCollection[0];

			ArrayList itemValues = new ArrayList();

			EngineeringUnit units = (stationManager.CurrentTransactionAlias.DensityUnits != 0) ? stationManager.CurrentTransactionAlias.DensityUnits : stationManager.SiteManager.Site.DensityUnits;
			byte decimalPlaces = (stationManager.CurrentTransactionAlias.DensityUnits != 0) ? stationManager.CurrentTransactionAlias._DensityDecimalPlaces : stationManager.SiteManager.Site._DensityDecimalPlaces;

			foreach (ProductMapClass component in this.LoadArm.ComponentCollection)
			{
				TankClass tank = this.SiteManager.GetTank(component, stationManager.Manager);
				if (tank == null)
				{
				    // Tank Group may not have a market tank in which case no recipes will be enabled
					// for the product.
					if (component.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP)
						continue;
				    return false;
				}

			    ProcessVariableClass densityPv = tank.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.STANDARD_DENSITY_PV];
				if (densityPv == null
				|| (!densityPv.IsQualityGood
				&& !this.SiteManager.Site.UseLastKnownGoodTankData))
					return false;

				// Presently the system expects that the Preset Units will match the Site Units
				// however there is a scale parameter 046 in the Daniels that is set based upon the Units
				double scale = 10;
				if (densityPv.ServerUnits == EngineeringUnit.FmdDegApi)
					scale = 100;
				else if (densityPv.ServerUnits == EngineeringUnit.FmdGcm3)
					scale = 10000;

				int density;
				try
				{
					density = Convert.ToInt32(Convert.ToDouble(densityPv.GetValue(units, decimalPlaces)) * scale);
				}
				catch
				{
					return false;
				}

			    ItemValue writeDensityItem = new ItemValue(loadArmPv.OPCItemID + ".Set Program Code Value")
			                                 {
			                                     Value = ((component.PresetNumber - 1) * 2 + 457) + " " + density
			                                 };

			    itemValues.Add(writeDensityItem);
			}

			try
			{
			    this.OpcServerManager.Write(new URL(loadArmPv.URL), (ItemValue[])itemValues.ToArray(typeof(ItemValue)));
			}
			catch (Exception e)
			{
			    this.eventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
				return false;
			}

			return true;
		}


        // Update the Amount/Cycle and Rate parameters for the Recipe
        /// <summary>
        /// Applies the additive profile to the preset device and updates the display name of the recipe
		/// 
		/// Currently does nothing
        /// </summary>
        /// <param name="name">
        /// Recipe name to be displayed
        /// </param>
        /// <param name="recipe">
        /// Recipe to be updated
        /// </param>
        /// <param name="product">
        /// Blend or Component definition for the recipe
        /// </param>
        /// <param name="additiveProfile">
        /// Additive Profile to apply to this recipe for the current ShipTo
        /// </param>
        /// <param name="deviceRecipeNumber">
        /// Internal recipe identifier on the preset
        /// </param>
        /// <returns>
        /// True
        /// </returns>
        public override bool UpdateRecipe(
			string name,
			ProductMapClass recipe,
            ProductClass product,
			AdditiveProfileClass additiveProfile,
			int deviceRecipe)
		{
			return true;
		}

		public override void ReleaseKeyPad()
		{
			try
			{
			    this.IsPreparedForMessage();

				ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];
				ItemValue TimeoutItemValue = new ItemValue(LoadArmPV.OPCItemID + ".Timeout Operation");
			    this.OpcServerManager.Write(new URL(LoadArmPV.URL), new[] { TimeoutItemValue });
			}
			catch (Exception e)
			{
			    this.eventLog.WriteEntry(e.Message, EventLogEntryType.Error);
			}
		}

		public override bool IsTransactionInProgress()
		{
		    if (this.TransactionAuthorizedPV.IsQualityGood
			&& (bool)this.TransactionAuthorizedPV.ServerValue)
				return true;

		    return false;
		}

        public override void ResetPowerFailAlarm()
		{
			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			// Only required if Arm 1
			if (LoadArmPV.OPCItemID.IndexOf("Arm 1") == -1)
				return;

			// Strip off the last part of the path
			int nLastIndex = LoadArmPV.OPCItemID.LastIndexOf(".");
			string OPCPath = LoadArmPV.OPCItemID.Substring(0, nLastIndex);

			Server Server = new Server(new Factory(), new URL(LoadArmPV.URL));
			Server.Connect();

			ItemValue[] SubItems =	{ new ItemValue(new ItemIdentifier(OPCPath+".Alarms.Reset Power-fail Alarm")),
										new ItemValue(new ItemIdentifier(OPCPath+".Status.Reset Powerfail"))
									};

			Server.Write(SubItems);

			Server.Disconnect();
			Server.Dispose();
		}

		public override void SyncDateAndTime()
		{
			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			ItemValue Item = new ItemValue();

			Item.ItemName = LoadArmPV.OPCItemID + ".Set Date And Time";

			Item.Value = DateTimeOffset.Now.ToString("MM/dd/yy HH:mm:ss");

		    this.OpcServerManager.Write(new URL(LoadArmPV.URL), new[] { Item });
		}

		public override string GetBatchNumber(StationManagerClass stationManager)
		{
			return this.TransactionNumber.ToString("D4") + " " + this.BatchNumber.ToString("D4");
		}


		public override void EndBatch()
		{
		    this.IsPreparedForMessage();

			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			ItemValue EndBatchItemValue = new ItemValue(new ItemIdentifier(LoadArmPV.OPCItemID + ".End Batch"));

			try
			{
			    this.OpcServerManager.Write(new URL(LoadArmPV.URL), new[] { EndBatchItemValue });
			}
			catch (Exception e)
			{
			    this.eventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
			}
		}

		protected void EndTransaction()
		{
			if ((this.TransactionInProgressPV.IsQualityGood
			&& !((bool)this.TransactionInProgressPV.ServerValue))
			&& (this.TransactionAuthorizedPV.IsQualityGood
			&& !((bool)this.TransactionAuthorizedPV.ServerValue)))
				return;

			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			ItemValue EndTransactionItemValue = new ItemValue(new ItemIdentifier(LoadArmPV.OPCItemID + ".End Transaction"));
			EndTransactionItemValue.Value = this.SwingArmStatusPV.ServerValue.ToString();

			try
			{
			    this.OpcServerManager.Write(new URL(LoadArmPV.URL), new[] { EndTransactionItemValue });
			}
			catch (Exception e)
			{
			    this.eventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
			}
		}

      public override bool AllocateRecipes(ulong recipeMap, ulong extendedRecipeMap)
      {
         throw new NotImplementedException();
      }
   }
}
