/******************************************************************************

	FILE NAME:		AcculoadIIILoadArmManager.cs


	PURPOSE:			AcculoadIIILoadArmManagerClass


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------
										7.1.0.1 - CSI 4730 Changed to SetFinishedLoading
										when StopKey and Load Arm State is BATCH_COMPLETE_PROMPT

		07/11/07		W.Gray		7.1.0.2 - CSI 4717 Changed OnInvoke to call CheckForStopKey
										rather than IssueEndBatchPrompt when flowing stops.

		09/27/07		W.Gray		7.1.1.1 - CSI 5242 Change to Add Line Item on Released

		10/16/07		W.Gray		7.1.1.2 - CSI 5283 Changed to disable recipe if Update Recipe fails

		01/02/08		W.Gray		7.1.1.3 - CSI 5437 Change to Complete Line Item when Swing Arm
										Position is changed

		06/12/2008	W.Gray		7.4.5.0 - Change to include ItemName on
										OPC Quality Bad Messages (CSI 5961)

		06/24/2008	W.Gray		7.4.5.1 - Change to incorporate Station InhibitSettingRecipeNames (CSI 5957)

		07/16/2008	W.Gray		7.4.5.2 - Change to not Test CommunicationsFailure flags when
										processing KeyPad data.  This was to insure that a response
										was for a prompt.  A new method has been added where by 
										a ResponsePending flag is set when a prompt is issued.

		08/04/2008	W.Gray		7.4.5.3 - If communications fails during Authorize the Arm may
										have been successfully authorized.  Revised to set LoadArmState to
										LOADARM_STATE.AUTHORIZED and return true regardless. 

		08/06/2008	W.Gray		7.4.5.4 - Revised so that Flowing Is not required to add line item

		08/15/2008	W.Gray		7.4.5.5 - Revised ProcessStopKey so that LOADARM_STATE.END_BATCH_PROMPT
										cause redisplay of the EndBatchMenu. (CSI 6085)

		11/03/2008	W.Gray		7.4.6.0 - Correction to error in Authorize introduced 10/22/2008

		11/05/2008	W.Gray		7.4.6.1 - Correction from 11/03/2008 was inadequate.  DisplayMessage
										can fail and no catch.  (CSI 6268)

		11/07/2008	W.Gray		7.4.6.2 - In Authorize changed not to issue EndBatch on a failure.
										In OnInvoke changed to Inprogress when ever Authorized & Released (CSI 6268)

		11/07/2008	W.Gray		7.4.6.3 - Revised to set DataType in ProcessVariable (CSI 6278)

		11/24/2008	W.Gray		7.4.6.4 - Revised UpdateReferenceDensity to evalute Site.UseLastKnownGood (CSI 6251)

		01/23/2009	W.Gray		7.4.6.5 - Revised to process TransactionDone and to recover from
										communications failure by handling Prompt Timeout when Station is idle (CSI 1092)

		01/27/2009	W.Gray		7.4.6.6 - Revised to OnInvoke to not set status to InProgress if
										AddLineItem fails.

		02/17/2009	W.Gray		7.4.6.7 - Revise OnInvoke to perform more rigorous checks for Batch Start
										and Batch Complete.

		02/19/2009	W.Gray		7.4.6.8 - Revised to attempt to retrieve BatchNumber up to 3 times (CSI 1550)

		03/02/2009	W.Gray		7.4.6.9 - Revised to read CTL in ReadComponentBatchData (CSI 1794)

		03/10/2009	W.Gray		7.4.6.10 - Revised to read RecipePV prior to AddLineItem (CSI 1884)

		03/17/2009	W.Gray		7.4.6.11 - Revised OnInvoke processing for TransactionDone (CSI 2036)

		07/07/2009	W.Gray		7.4.6.12 - Revised to Perform Log Out seperate from UpdateRecipe (4042)

		07/07/2009	W.Gray		7.4.6.13 - Revised to monitor Keypad Data Pending (CSI 4042)

		07/07/2009	W.Gray		7.4.6.14 - Revised IssuePermissiveMessage to ReleaseKeyPad on all Arms

		07/23/2009	W.Gray		7.4.6.15 - Revised OnInvoke processing for Transaction Done
										to confirm Authorized false and Released false.  This was
										to address an occurance of premature completion of line items
										that appears to be only attributable to a spurious signal
										on the Transaction Done.  Accuload Event Logs showed that
										the problem occured when permissive lost signal occured.
										Also change removed check of Released on Batch Done and check
										of Batch Done on Released based on the belief that those prior
										attempts to explain duplicate line items were incorrect.
 
		08/06/2009	W.Gray		7.4.6.16 - Revised to monitor the Arm Status Alarm so that the 
										Station Manager can decide which arm to use for Station Message
										as the first arm without an alarm.  This will enable loading to
										function when an alarm exists (CSI 5208)

		09/20/2009	W.Gray		7.4.6.17 - Revised to implement ProcessPermissiveMessageAcknowledge.
										Corrected problem where arm didn't not properly display IssueSelectPrompt
										after acknowledgement. (WI 5699)
  
		06/2009	W.Gray		7.4.6.18 - Revised to issue Reset Transaction Done when
										Transaction Done occurs and issue Reset Batch Done when
										batch Done occurs.  This serves to make them one shots.

										Revised Transaction Done processing to call IssueFinishedMessage

										Revised Presetting In Progress processing to call TransactionDone 


  
********************************************************************************/

namespace LoadRackLibrary
{
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.LogClient;

	using Opc;
	using Opc.Da;

	using System;
	using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
	using System.Globalization;
    using System.Linq;
    using System.Net;
	using System.Runtime.InteropServices;
	using System.Threading;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	using Convert = System.Convert;
	using Factory = OpcCom.Factory;
	using Server = Opc.Da.Server;

	/// <summary>
	/// Summary description for AcculoadIIILoadArmManagerClass.
	/// </summary>
	// ReSharper disable once InconsistentNaming
	public class AcculoadIIILoadArmManagerClass : LoadArmManagerClass
	{
		//	protected DisplayMenuParameters			CurrentMenuParameters;
		protected ProcessVariableClass RecipePV;
		protected ProcessVariableClass ReleaseKeypadPV;
		protected ProcessVariableClass KeypadDataPV;
		protected ProcessVariableClass KeypadDataPendingPV;
		protected ProcessVariableClass MessageTimeoutPV;
		protected ProcessVariableClass GetKeyPV;
		protected ProcessVariableClass PresettingInProgressPV;
		protected ProcessVariableClass FlowingPV;
		protected ProcessVariableClass LoadArmReleasedPV;
		protected ProcessVariableClass BatchDonePV;
		protected ProcessVariableClass AuthorizedPV;
		protected ProcessVariableClass TransactionInProgressPV;
		protected ProcessVariableClass TransactionDonePV;
		protected ProcessVariableClass SwingArmStatusPV;
		protected ProcessVariableClass PowerFailPV;
		protected ProcessVariableClass PermissiveDelayPV;
		protected ProcessVariableClass AlarmPV;
		protected bool InitialAlarmStatus = true;

		protected const int NUMBER_OF_RETRIES = 1;

		protected int presetArmNumber = 0;

		public AcculoadIIILoadArmManagerClass(
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
			string LoadArmOPCItemID = LoadArm.ProcessVariableCollection[0].OPCItemID;
			string LoadArmProgID = LoadArm.ProcessVariableCollection[0].ProgID;

			this.ReleaseKeypadPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.RELEASE_KEYPAD_PV,
				UNIT_TYPE.LOADARM_UNIT,
				VarEnum.VT_EMPTY,
				false,
				LoadArmOPCItemID + ".Release Keypad and Display",
				LoadArmURL,
				LoadArmProgID);

			this.RecipePV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.RECIPE_PV,
				UNIT_TYPE.LOADARM_UNIT,
				VarEnum.VT_UI2,
				true,
				LoadArmOPCItemID + ".Recipe",
				LoadArmURL,
				LoadArmProgID);

			this.MessageTimeoutPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.PROMPT_TIMEOUT_PV,
				UNIT_TYPE.LOADARM_UNIT,
				VarEnum.VT_BOOL,
				true,
				LoadArmOPCItemID + ".Status.Display Message Time-out",
				LoadArmURL,
				LoadArmProgID);

			this.KeypadDataPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.KEYPAD_DATA_PV,
				UNIT_TYPE.LOADARM_UNIT,
				VarEnum.VT_BSTR,
				true,
				LoadArmOPCItemID + ".Keypad Data",
				LoadArmURL,
				LoadArmProgID);

			this.KeypadDataPendingPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.KEYPAD_DATA_PENDING_PV,
				UNIT_TYPE.LOADARM_UNIT,
				VarEnum.VT_BOOL,
				true,
				LoadArmOPCItemID + ".Status.Keypad Data Pending",
				LoadArmURL,
				LoadArmProgID);

			this.GetKeyPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.GET_KEY_PV,
				UNIT_TYPE.LOADARM_UNIT,
				VarEnum.VT_BSTR,
				true,
				LoadArmOPCItemID + ".Get Key",
				LoadArmURL,
				LoadArmProgID);

			this.AuthorizedPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.AUTHORIZED_PV,
				UNIT_TYPE.LOADARM_UNIT,
				VarEnum.VT_BOOL,
				true,
				LoadArmOPCItemID + ".Status.Authorized",
				LoadArmURL,
				LoadArmProgID);

			this.TransactionInProgressPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.TRANSACTION_IN_PROGRESS_PV,
				UNIT_TYPE.LOADARM_UNIT,
				VarEnum.VT_BOOL,
				true,
				LoadArmOPCItemID + ".Status.Transaction in Progress",
				LoadArmURL,
				LoadArmProgID);

			this.TransactionDonePV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.TRANSACTION_DONE_PV,
				UNIT_TYPE.LOADARM_UNIT,
				VarEnum.VT_BOOL,
				true,
				LoadArmOPCItemID + ".Status.Transaction Done",
				LoadArmURL,
				LoadArmProgID);

			this.BatchDonePV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.BATCH_DONE_PV,
				UNIT_TYPE.LOADARM_UNIT,
				VarEnum.VT_BOOL,
				true,
				LoadArmOPCItemID + ".Status.Batch Done",
				LoadArmURL,
				LoadArmProgID);

			this.FlowingPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.FLOWING_PV,
				UNIT_TYPE.LOADARM_UNIT,
				VarEnum.VT_BOOL,
				true,
				LoadArmOPCItemID + ".Status.Flowing",
				LoadArmURL,
				LoadArmProgID);

			this.LoadArmReleasedPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.LOAD_ARM_RELEASED_PV,
				UNIT_TYPE.LOADARM_UNIT,
				VarEnum.VT_BOOL,
				true,
				LoadArmOPCItemID + ".Status.Released",
				LoadArmURL,
				LoadArmProgID);

			this.PermissiveDelayPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.PERMISSIVE_DELAY_PV,
				UNIT_TYPE.LOADARM_UNIT,
				VarEnum.VT_BOOL,
				true,
				LoadArmOPCItemID + ".Status.Permissive Delay",
				LoadArmURL,
				LoadArmProgID);

			this.AlarmPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.ALARM_PV,
				UNIT_TYPE.LOADARM_UNIT,
				VarEnum.VT_BOOL,
				true,
				LoadArmOPCItemID + ".Status.Alarm",
				LoadArmURL,
				LoadArmProgID);

			if (LoadArm.SwingArm)
			{
				this.SwingArmStatusPV = new ProcessVariableClass(
					PROCESS_VARIABLE_TYPE.SWING_ARM_STATUS_PV,
					UNIT_TYPE.LOADARM_UNIT,
					VarEnum.VT_BSTR,
					true,
					LoadArmOPCItemID + ".Swing Arm Position",
					LoadArmURL,
					LoadArmProgID);

				this.OpcServerManager.AddProcessVariable(this.SwingArmStatusPV);
			}

			if (LoadArm.PresetType == PRESET_TYPE.ACCULOADIII_Q
			|| LoadArm.PresetType == PRESET_TYPE.ACCULOADIII_S
			|| LoadArm.PresetType == PRESET_TYPE.ACCULOADIII_SA)
			{
				this.PresettingInProgressPV = new ProcessVariableClass(
					PROCESS_VARIABLE_TYPE.PRESETTING_IN_PROGRESS_PV,
					UNIT_TYPE.LOADARM_UNIT,
					VarEnum.VT_BOOL,
					true,
					LoadArmOPCItemID + ".Status.Presetting In Progress",
					LoadArmURL,
					LoadArmProgID);

				this.OpcServerManager.AddProcessVariable(this.PresettingInProgressPV);
			}

			int nLastIndex = LoadArm.ProcessVariableCollection[0].OPCItemID.LastIndexOf(".");
			string OPCPath = LoadArm.ProcessVariableCollection[0].OPCItemID.Substring(0, nLastIndex);

			if (StationManager.RegisterPowerfail(OPCPath))
			{
				this.PowerFailPV = new ProcessVariableClass(
									PROCESS_VARIABLE_TYPE.POWER_FAIL_OCCURRED_PV,
									UNIT_TYPE.LOADARM_UNIT,
									VarEnum.VT_BOOL,
									true,
									OPCPath + ".Alarms.System.Powerfail",
									LoadArmURL,
									LoadArmProgID);

				this.OpcServerManager.AddProcessVariable(this.PowerFailPV);
			}


			if (LoadArm.SwingArm)
			{
				try
				{
					this.OpcServerManager.Read(this.SwingArmStatusPV);
					if (this.SwingArmStatusPV.IsQualityGood)
					{
						this.LastSwingArmStatus = this.SwingArmStatusPV.ServerValue;
					}
				}
				catch (Exception e)
				{
					EventLog.WriteEntry("Accuload III LoadArmManager OnInvoke reading Swing Arm: " + e.Message + e.StackTrace, EventLogEntryType.Error);
					this.CommunicationsFailure = true;
				}
			}

			this.OpcServerManager.AddProcessVariable(this.RecipePV);
			this.OpcServerManager.AddProcessVariable(this.MessageTimeoutPV);
			this.OpcServerManager.AddProcessVariable(this.KeypadDataPendingPV);
			this.OpcServerManager.AddProcessVariable(this.GetKeyPV);
			this.OpcServerManager.AddProcessVariable(this.AuthorizedPV);
			this.OpcServerManager.AddProcessVariable(this.FlowingPV);
			this.OpcServerManager.AddProcessVariable(this.BatchDonePV);
			this.OpcServerManager.AddProcessVariable(this.TransactionInProgressPV);
			this.OpcServerManager.AddProcessVariable(this.TransactionDonePV);
			this.OpcServerManager.AddProcessVariable(this.LoadArmReleasedPV);
			this.OpcServerManager.AddProcessVariable(this.PermissiveDelayPV);
			this.OpcServerManager.AddProcessVariable(this.AlarmPV);

			this.OpcServerManager.Read(this.AlarmPV);
		}

		protected internal override bool SuppressLoadFinishedPrompt
		{
			get
			{
				int nLastIndex = this.LoadArm.ProcessVariableCollection[0].OPCItemID.LastIndexOf(".", StringComparison.Ordinal);
				string opcPath = this.LoadArm.ProcessVariableCollection[0].OPCItemID.Substring(0, nLastIndex);
				Item authorizeTransactionItem = new Item(new ItemIdentifier(opcPath + ".Transaction Termination"));

				try
				{
					ItemValueResult[] values = this.OpcServerManager.Read(new URL(this.LoadArm.ProcessVariableCollection[0].URL), new[] { authorizeTransactionItem });

					string pointValue = values[0].Value.ToString();
					if (pointValue.StartsWith("1", StringComparison.InvariantCultureIgnoreCase) == false)
					{
						return true;
					}
				}
				catch (Exception)
				{
					//Unable to determine a specific exception thrown by OPCServerManager.Read, so have to catch them all
				}

				return false;
			}
		}

        /// <summary>
		/// Returns the physical arm number from the preset that this load arm manager communicates with
		/// 
		/// Queries the preset to determine the physical arm number the first time this is called.
		/// Caches the arm number for subsequent calls; this is not expected to change while Terminal Automation
		/// is operating
		/// </summary>
		/// <param name="stationManager">Current station manager controlling this load arm manager</param>
		/// <returns>Physical Arm number from the device</returns>
		protected internal override int GetPresetArmNumber(StationManagerClass stationManager)
        {
            if (this.presetArmNumber != 0)
			{
				return this.presetArmNumber;
			}

            ProcessVariableClass loadArmPv = this.LoadArm.ProcessVariableCollection[0];
            Server server = new Server(new Factory(), new URL(loadArmPv.URL));
            server.Connect(new ConnectData(null));

            string tag = loadArmPv.OPCItemID + ".Preset Arm Number";

            Item[] Items ={ new Item(new ItemIdentifier(tag)),
                            };

            ItemValueResult[] Values = server.Read(Items);

			if (Values[0].Quality == Quality.Good)
			{
				this.presetArmNumber = Convert.ToInt32(Values[0].Value);
				return this.presetArmNumber;
			}
			else
			{
				return 0;
			}
        }

        protected override void OnInvoke(ProcessVariableClass pv)
		{
			StationManagerClass stationManager = this.GetStationManager();
			if (stationManager == null)
			{
				// Situation can occur when alternate station is disabled
				if (pv.ProcessVariableType == PROCESS_VARIABLE_TYPE.SWING_ARM_STATUS_PV)
				{
					this.LastSwingArmStatus = pv.ServerValue;
					this.SetState(LOADARM_STATE.NORMAL);
					this.ReleaseKeyPad();
				}

				return;
			}

			if (pv.IsQualityGood)
			{
				this.CommunicationsFailure = false;
				stationManager.CommunicationsFailure = false;
			}

			Monitor.Enter(stationManager);

			try
			{
				Logger logger = new Logger(StopWatch.Appnames.LoadRackService.ToString());
				LOADARM_STATE initialLoadArmState = this.LoadArmState;
				StationState initialStationState = stationManager.StationState;

				switch (pv.ProcessVariableType)
				{
					case PROCESS_VARIABLE_TYPE.POWER_FAIL_OCCURRED_PV:
						{
							AcculoadIIIStationManagerClass acculoadStation = stationManager as AcculoadIIIStationManagerClass;
							acculoadStation?.ProcessPowerfail(pv);
							break;
						}

					case PROCESS_VARIABLE_TYPE.TRANSACTION_IN_PROGRESS_PV:
						{
							if (pv.IsQualityGood
							&& (bool)pv.ServerValue)
							{
								if (stationManager.StationState != StationState.AUTHORIZING
								&& stationManager.StationState != StationState.AUTHORIZED
								&& stationManager.StationState != StationState.TRANSACTION_IN_PROGRESS)
								{
									stationManager.CancelUnauthorizedTransaction();
								}
								else if (stationManager.StationState == StationState.AUTHORIZING
								|| stationManager.StationState == StationState.AUTHORIZED)
								{
									stationManager.StationState = StationState.TRANSACTION_IN_PROGRESS;
								}
							}
							break;
						}

					case PROCESS_VARIABLE_TYPE.RECIPE_PV:
						{
							bool authorized = false;
							int downloadedRecipeNumber = 0;
							ProductMapClass configuredRecipe = null;

							if (this.RecipePV.IsQualityGood)
							{
								downloadedRecipeNumber = Convert.ToInt32(this.RecipePV.ServerValue);
								if (this.GetStationManager().Station.Type == STATION_TYPE.LOAD_RACK)
								{
									try
									{
										configuredRecipe = this.GetStationManager().RecipeInternalNumberMap[downloadedRecipeNumber];
									}
									catch (KeyNotFoundException knfe)
									{
										_ = knfe;

										if (this.GetStationManager().StationState != StationState.IDLE)
										{
											// When station is idle, we'll have no recipes active  in the stationmanager.  Any report of a recipe number
											// from the Accuload will fall to here in that case.  For idle station, we can safely ignore.  We log and report
											// at other times.
											string msg = $"Unexpected device recipe {downloadedRecipeNumber} on arm {this.GetArmNumber(stationManager)}";

											this.GetStationManager().WriteLogDataToCommFile(msg, StationManagerClass.CommLogDirection.None);

											this.eventLog.WriteEntry(msg, EventLogEntryType.Error);
										}
									}
								}
								else
								{
									// expect Station type OFFLOAD
									configuredRecipe = this.GetRecipeByRecipeNumber(downloadedRecipeNumber);
								}
							}

							if (this.AuthorizedPV.IsQualityGood)
							{
								authorized = Convert.ToBoolean(this.AuthorizedPV.ServerValue);
							}

							this.UpdatePermissives(stationManager, configuredRecipe, authorized);
							break;
						}

					case PROCESS_VARIABLE_TYPE.FLOWING_PV:
						{
							if (this.FlowingPV.IsQualityGood
							&& (bool)this.FlowingPV.ServerValue == false)
							{ 
								switch (this.LoadArmState)// == LOADARM_STATE.INPROGRESS)
								{
									case LOADARM_STATE.INPROGRESS:
										this.OpcServerManager.Read(this.BatchDonePV);
										if (this.BatchDonePV.IsQualityGood
										&& (bool)this.BatchDonePV.ServerValue == false)
										{
											this.CheckForStopKey();
										}
										break;
									case LOADARM_STATE.NORMAL:
										this.OpcServerManager.Read(this.BatchDonePV);
										if (this.BatchDonePV.IsQualityGood
										&& (bool)this.BatchDonePV.ServerValue == false)
										{
											if (!this.GetStationManager()?.SiteManager.StationPermissive ?? false)
											{
												// Flow most likely stopped because we lost a site permissive and issued a stop
												// Go ahead and end the batch.
												this.EndBatch();
											}
										}
										break;
								}
							}
							break;
						}

					case PROCESS_VARIABLE_TYPE.PERMISSIVE_DELAY_PV:
					case PROCESS_VARIABLE_TYPE.TRANSACTION_DONE_PV:
					case PROCESS_VARIABLE_TYPE.AUTHORIZED_PV:
					case PROCESS_VARIABLE_TYPE.LOAD_ARM_RELEASED_PV:
					case PROCESS_VARIABLE_TYPE.BATCH_DONE_PV:
						{
							// Process only when all flags are valid
							if (!this.TransactionDonePV.IsQualityGood
							|| !this.LoadArmReleasedPV.IsQualityGood
							|| !this.BatchDonePV.IsQualityGood
							|| !this.AuthorizedPV.IsQualityGood
							|| !this.PermissiveDelayPV.IsQualityGood
							|| (this.PresettingInProgressPV != null
							&& !this.PresettingInProgressPV.IsQualityGood))
							{
								break;
							}

							if ((bool)this.TransactionDonePV.ServerValue
							&& (bool)this.AuthorizedPV.ServerValue == false
							&& (bool)this.LoadArmReleasedPV.ServerValue == false)
							{
								bool updateLineItemSuccessful = true;

								if (stationManager.StationState == StationState.AUTHORIZING
								|| stationManager.StationState == StationState.AUTHORIZED
								|| stationManager.StationState == StationState.TRANSACTION_IN_PROGRESS)
								{
									LineItemDO lineItem = stationManager.GetLineItem(this.LoadArm.IdentityGuid);

									if (lineItem != null)
									{
										try
										{
											stationManager.UpdateLineItem(lineItem);
											stationManager.CloseOutLineItem(lineItem);
										}
										catch (Exception e) // catching Exception because the above two functions may throw Exception
										{
											updateLineItemSuccessful = false;

											this.eventLog.WriteEntry("Accuload III LoadArmManager OnInvoke : PV = " + pv.OPCItemID + " " + e + e.StackTrace, EventLogEntryType.Error);
										}
										stationManager.SaveTransaction();
									}

									if (updateLineItemSuccessful)
									{
										this.IssueFinishedMessage();

										stationManager.TransactionDone();
									}
									else
									{
										stationManager.StationState = StationState.RESET_ON_TIMEOUT;
										stationManager.DisplayMessage("LoadRack|Save Transaction LineItem Failure", null, 0, this.MessageTimeout);
									}
								}

								ProcessVariableClass loadArmPv = this.LoadArm.ProcessVariableCollection[0];
								ItemValue[] subItems = { new ItemValue(new ItemIdentifier(loadArmPv.OPCItemID + ".Status.Reset Transaction Done")) };
								this.OpcServerManager.Write(new URL(loadArmPv.URL), subItems);
							}

							if (pv.ProcessVariableType == PROCESS_VARIABLE_TYPE.AUTHORIZED_PV)
							{
								int downloadedRecipeNumber = 0;
								ProductMapClass configuredRecipe = null;

								bool authorized = Convert.ToBoolean(this.AuthorizedPV.ServerValue);

								// When authorized is true, the Recipe can be reset to Zero
								// immediately afterward so read it to get current value
								if (authorized)
								{
									if (this.LoadArmState != LOADARM_STATE.END_BATCH_PROMPT
									&& this.LoadArmState != LOADARM_STATE.INPROGRESS
									&& this.LoadArmState != LOADARM_STATE.AUTHORIZED_PERMISSIVE_PROMPT)
									{
										this.LoadArmState = LOADARM_STATE.AUTHORIZED;
									}

									this.OpcServerManager.Read(this.RecipePV);
								}

								if (this.RecipePV.IsQualityGood)
								{
									downloadedRecipeNumber = Convert.ToInt32(this.RecipePV.ServerValue);
                                    if (this.GetStationManager().Station.Type == STATION_TYPE.LOAD_RACK)
                                    {
                                        try
                                        {
                                            configuredRecipe = this.GetStationManager().RecipeInternalNumberMap[downloadedRecipeNumber];
                                        }
                                        catch (KeyNotFoundException knfe)
                                        {
                                            _ = knfe;

                                            if (this.GetStationManager().StationState != StationState.IDLE)
                                            {
                                                // When station is idle, we'll have no recipes active  in the stationmanager.  Any report of a recipe number
                                                // from the Accuload will fall to here in that case.  For idle station, we can safely ignore.  We log and report
                                                // at other times.
                                                string msg = $"Unexpected device recipe {downloadedRecipeNumber} on arm {this.GetArmNumber(stationManager)}";

                                                this.GetStationManager().WriteLogDataToCommFile(msg, StationManagerClass.CommLogDirection.None);

                                                this.eventLog.WriteEntry(msg, EventLogEntryType.Error);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // expect Station type OFFLOAD
                                        configuredRecipe = this.GetRecipeByRecipeNumber(downloadedRecipeNumber);
                                    }
                                }

                                this.UpdatePermissives(stationManager, configuredRecipe, authorized);
							}

							// Check for Possible Stop Key
							if ((bool)this.LoadArmReleasedPV.ServerValue == false
							&& (bool)this.BatchDonePV.ServerValue == false
							&& (bool)this.AuthorizedPV.ServerValue
							&& this.LoadArmState == LOADARM_STATE.INPROGRESS)
							{
								// Verify the Batch Done is false
								this.OpcServerManager.Read(this.BatchDonePV);
								if (this.BatchDonePV.IsQualityGood
								&& (bool)this.BatchDonePV.ServerValue == false)
								{
									this.CheckForStopKey();
								}
							}

							// Check for Possible End of Batch
							if ((bool)this.LoadArmReleasedPV.ServerValue == false
							&& (bool)this.BatchDonePV.ServerValue
							&& (bool)this.AuthorizedPV.ServerValue == false)
							{
								bool updateLineItemSuccessful = true;

								if (stationManager.StationState == StationState.AUTHORIZING
								|| stationManager.StationState == StationState.AUTHORIZED
								|| stationManager.StationState == StationState.TRANSACTION_IN_PROGRESS)
								{
									LineItemDO lineItem = stationManager.GetLineItem(this.LoadArm.IdentityGuid);

									if (lineItem != null)
									{
										try
										{
											stationManager.UpdateLineItem(lineItem);
											stationManager.CloseOutLineItem(lineItem);
										}
										catch (Exception e) // catching Exception because the above two functions may throw Exception
										{
											updateLineItemSuccessful = false;

											this.eventLog.WriteEntry("Accuload III LoadArmManager OnInvoke : PV = " + pv.OPCItemID + " " + e + e.StackTrace, EventLogEntryType.Error);
										}
										stationManager.SaveTransaction();

										// At this point, we need to issue a Batch Complete message to allow the driver
										// to load another batch, view the last batch status, or indicate they are finished
										// with this load arm.
										// Need to check the Transaction Termination OPC value; if it is present and it is not "1 Comm Only", then immediately
										// complete the transaction.
										if (updateLineItemSuccessful)
										{
											this.IssueBatchCompletePrompt();
										}
										else
										{
											stationManager.StationState = StationState.RESET_ON_TIMEOUT;
											stationManager.DisplayMessage("LoadRack|Save Transaction LineItem Failure", null, 0, this.MessageTimeout);
										}
									}
								}

								ProcessVariableClass loadArmPv = this.LoadArm.ProcessVariableCollection[0];
								ItemValue[] subItems = { new ItemValue(new ItemIdentifier(loadArmPv.OPCItemID + ".Status.Reset Batch Done")) };
								this.OpcServerManager.Write(new URL(loadArmPv.URL), subItems);
							}

							// Check for Possible Batch Start
							if (((bool)this.LoadArmReleasedPV.ServerValue
							|| ((bool)this.PermissiveDelayPV.ServerValue
							&& (this.PresettingInProgressPV == null
							|| (bool)this.PresettingInProgressPV.ServerValue == false)))
							&& (bool)this.AuthorizedPV.ServerValue
							&& (bool)this.BatchDonePV.ServerValue == false
							&& (stationManager.StationState == StationState.AUTHORIZING
							|| stationManager.StationState == StationState.AUTHORIZED
							|| stationManager.StationState == StationState.TRANSACTION_IN_PROGRESS))
							{

								LineItemDO lineItem = stationManager.GetLineItem(this.LoadArm.IdentityGuid);

								if (lineItem == null)
								{
									try
									{
										stationManager.AddLineItem(this.LoadArm.IdentityGuid);
										if (this.LoadArmState == LOADARM_STATE.AUTHORIZED_PERMISSIVE_PROMPT)
										{
											this.SetState(LOADARM_STATE.INPROGRESS_PERMISSIVE_PROMPT);
										}
										else
										{
											this.SetState(LOADARM_STATE.INPROGRESS);
										}

										stationManager.SaveTransaction();
									}
									catch (Exception e)
									{
										this.eventLog.WriteEntry("Accuload III LoadArmManager OnInvoke : Failed To Add Line Item - " + e.Message + e.StackTrace, EventLogEntryType.Error);
										this.Unauthorize();
										this.SetState(LOADARM_STATE.SELECT_PROMPT);
										this.DisplayMessage("LoadRack|Add Line Item Error", 0, this.MessageTimeout);
									}
								}
								else
								{
									if (this.LoadArmState != LOADARM_STATE.END_BATCH_PROMPT)
									{
										this.SetState(LOADARM_STATE.INPROGRESS);
									}

									stationManager.SetTransactionInProgress();
									stationManager.UpdateLineItem(lineItem);
									stationManager.SaveTransaction();
								}

								stationManager.StationState = StationState.TRANSACTION_IN_PROGRESS;
							}

							// Check for possible permissive delay
							if ((bool)this.PermissiveDelayPV.ServerValue
							&& (this.LoadArmState == LOADARM_STATE.AUTHORIZED
							|| this.LoadArmState == LOADARM_STATE.INPROGRESS))
							{
								this.IssuePermissiveMessage(stationManager);
							}

							break;
						}

					case PROCESS_VARIABLE_TYPE.GET_KEY_PV:
						{
							this.CheckForStopKey();
							break;

						}

					case PROCESS_VARIABLE_TYPE.PRESETTING_IN_PROGRESS_PV:
						{
							if (this.LoadArmState == LOADARM_STATE.AUTHORIZED
							&& pv.IsQualityGood)
							{
								if ((bool)pv.ServerValue == false)
								{
									this.OpcServerManager.Read(this.LoadArmReleasedPV);

									if (this.LoadArmReleasedPV.IsQualityGood
									&& (bool)this.LoadArmReleasedPV.ServerValue == false
									&& this.PermissiveDelayPV.IsQualityGood
									&& (bool)this.PermissiveDelayPV.ServerValue == false)
									{
										this.CancelPresetting(stationManager);
										this.EndBatch();
										this.IssueSelectPrompt(stationManager);
										stationManager.TransactionDone();
									}
								}
								else
								{
									this.IssuePermissiveMessage(stationManager);
								}
							}
							break;
						}

					case PROCESS_VARIABLE_TYPE.SWING_ARM_STATUS_PV:
						{
							if (pv.IsQualityGood)
							{
								if (pv.ServerValue as string != this.LastSwingArmStatus as string)
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
												this.eventLog.WriteEntry("Accuload III LoadArmManager OnInvoke : " + e.Message + e.StackTrace, EventLogEntryType.Error);
												this.CommunicationsFailure = true;
										}
										catch (Exception e)
										{
												this.eventLog.WriteEntry("Accuload III LoadArmManager OnInvoke : PV = " + pv.OPCItemID + " " + e + e.StackTrace, EventLogEntryType.Error);
										}
										finally
										{
												Monitor.Exit(alternateStationManager);
										}
									}

									// Stop batch on this arm that was authorized before it swung to this station/bay ??
									if (this.AuthorizedPV.IsQualityGood
									&& (bool)this.AuthorizedPV.ServerValue)
									{
										this.EndBatch();
									}

									// Stop transaction in prgress on this arm from before it swung to this station/bay ??
									if (this.TransactionInProgressPV.IsQualityGood
									&& (bool)this.TransactionInProgressPV.ServerValue)
									{
										this.SendEndTransaction();
									}

									// If using dynamic recipes, clear any recipes that had been assigned to this arm
									// Need to do this via the station we came from as that station needs to update its recipe mapping
									if (alternateStationManager != null)
									{
										this.ClearArmProducts(alternateStationManager);
									}

									this.ReleaseKeyPad();
									this.LastSwingArmStatus = pv.ServerValue;
									this.SetState(LOADARM_STATE.NORMAL);

									if (stationManager.StationState == StationState.AUTHORIZING
										|| stationManager.StationState == StationState.AUTHORIZED
										|| stationManager.StationState == StationState.TRANSACTION_IN_PROGRESS)
									{
										foreach (ProductMapClass recipe in this.AvailableRecipeCollection)
										{
											int presetNumber = this.GetStationManager().RecipeInternalNumberMap.FirstOrDefault(x => x.Value.AssignedGuid == recipe.AssignedGuid).Key;
											// Need to resolve back from preset recipe number 
											// Note that in the dynamic recipe case, we expect that the RecipeMap will be all zeros at this point and will be adding to recipe map later
											// For static though, we rely on the RecipeMap bit already being set
											if (!stationManager.Station.EnableDynamicRecipes && (this.Bay(stationManager).RecipeMap & (ulong)0x1 << (presetNumber - 1)) == 0)
											{
												continue;
											}

											ProductMapClass authorizedProduct = stationManager.GetAuthorizedProduct(recipe.AssignedID);

											string name = stationManager.GetLoadRackDisplayText(recipe.AssignedGuid);

											AdditiveProfileClass additiveProfile = null;

											if (authorizedProduct != null)
											{
												if (authorizedProduct.AdditiveProfileGuid != Guid.Empty)
												{
													additiveProfile = this.GetAdditiveProfile(this.Security, authorizedProduct.AdditiveProfileGuid);
												}

												ProductClass product =
													FMChannelHelper.MakeCall<IProducts, ProductClass>(
														x => x.Get(this.Security, authorizedProduct.AssignedGuid));

												presetNumber = this.GetStationManager().WriteSingleRecipe(this, recipe);
												if (presetNumber > 0)
												{
                                                    try
                                                    {
                                                        stationManager.RecipeInternalNumberMap.Add(presetNumber, recipe);
                                                    }
                                                    catch (ArgumentException ae)
                                                    {
                                                        _ = ae;
                                                        this.DisplayMessageWithAcknowledge("LoadRack|Recipe Write Error");
                                                        stationManager.WriteLogDataToCommFile("Error writing recipe to preset: duplicate internal recipe number", StationManagerClass.CommLogDirection.None);

                                                        this.eventLog.WriteEntry($"Error writing recipe {recipe.ID} to preset# {presetNumber}, duplicate internal recipe number", EventLogEntryType.Error);
                                                        return;
                                                    }

                                                    this.Bay(stationManager).RecipeMap |= 0x1UL << (presetNumber - 1);
													// TODO: Fix this when we handle dynamic recipe support on swing arms
													if (!this.UpdateRecipe(name, recipe, product, additiveProfile, presetNumber))
													{
														this.Bay(stationManager).RecipeMap ^= 0x1UL << (presetNumber - 1);
													}
												}
											}
											else
											{
												this.eventLog.WriteEntry($"Recipe {recipe.AssignedID} does not have any authorized products ", EventLogEntryType.Error);
											}
										}

										if (!this.LogOutOfProgramMode())
										{
												return;
										}

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

					// Nothing to do at this point
					case PROCESS_VARIABLE_TYPE.RELEASE_KEYPAD_PV:
						break;

					case PROCESS_VARIABLE_TYPE.KEYPAD_DATA_PENDING_PV:

						if (pv.IsQualityGood
						&& (bool)pv.ServerValue
						&& this.ResponsePending)
						{
							this.OpcServerManager.Read(this.KeypadDataPV);
							if (this.KeypadDataPV.IsQualityGood)
							{
								this.ResponsePending = false;

								if (!this.ProcessResponseData(stationManager, this.KeypadDataPV.ServerValue as string))
								{
									stationManager.ProcessResponseData(this.KeypadDataPV.ServerValue as string);
								}
							}
						}
						break;

					case PROCESS_VARIABLE_TYPE.PROMPT_TIMEOUT_PV:
						if (pv.IsQualityGood
						&& ((bool)pv.ServerValue
						&& this.ResponsePending)
						|| stationManager.StationState == StationState.IDLE)
						{
							this.ResponsePending = false;

							if (!this.ProcessMessageTimeout(stationManager))
							{
								stationManager.ProcessMessageTimeout();
							}
						}
						break;

					case PROCESS_VARIABLE_TYPE.ALARM_PV:
						if (pv.IsQualityGood
						&& !(bool)pv.ServerValue
						&& !this.InitialAlarmStatus)
						{
							if (stationManager.StationState == StationState.AUTHORIZING
							|| stationManager.StationState == StationState.AUTHORIZED
							|| stationManager.StationState == StationState.TRANSACTION_IN_PROGRESS)
							{
								if (this.AuthorizedPV.IsQualityGood && !(bool)this.AuthorizedPV.ServerValue)
								{
									this.PromptForNextBatch(stationManager, true);
								}
							}
							else
							{
								if (!this.ProcessMessageTimeout(stationManager))
								{
									stationManager.ProcessMessageTimeout();
								}
							}
						}

						this.InitialAlarmStatus = false;

						break;

					default:
						base.OnInvoke(pv);
						break;
				}

				if (initialLoadArmState != this.LoadArmState
				|| initialStationState != stationManager.StationState)
				{
					int armNumber = stationManager.Station.SwingArmPosition == "A" ? this.LoadArm.BayAArmNumber : this.LoadArm.BayBArmNumber;
					StationState stationState = stationManager.StationState;
					logger.Info("Accuload III Post OnInvoke : Arm " + armNumber.ToString(CultureInfo.InvariantCulture) + " State = " + this.LoadArmState +
								"; Station State = " + stationState + "; PV " + pv.ProcessVariableType + "/" + pv.OPCItemID);
				}
			}
			catch (OpcException e)
			{
				this.eventLog.WriteEntry("Accuload III LoadArmManager OnInvoke : " + e.Message + e.StackTrace, EventLogEntryType.Error);
				this.CommunicationsFailure = true;
			}
			catch (Exception e)
			{
				this.eventLog.WriteEntry("Accuload III LoadArmManager OnInvoke : PV = " + pv.OPCItemID + " " + e + e.StackTrace, EventLogEntryType.Error);
			}
			finally
			{
				Monitor.Exit(stationManager);
			}
		}

		private AdditiveProfileClass GetAdditiveProfile(SecurityClass security, Guid guid)
		{
			return FMChannelHelper.MakeCall<IAdditiveProfiles, AdditiveProfileClass>(
																	x =>
																	x.Get(security, guid)
																);
		}

		public override void IssuePermissiveMessage(StationManagerClass stationManager)
		{
			
			if (stationManager == null)
			{
				return;
			}

			string message = this.GetPermissiveMessage(stationManager);

			if (message != ""
			&& ((this.PresettingInProgressPV != null
			&& this.PresettingInProgressPV.IsQualityGood
			&& (bool)this.PresettingInProgressPV.ServerValue)
			|| (this.PermissiveDelayPV.IsQualityGood
			&& (bool)this.PermissiveDelayPV.ServerValue)))
			{
				this.DisplayMessage(message + " " + stationManager.AcknowledgementMessage, stationManager.AcknowledgementResponseLength, 999);
				if (this.LoadArmState == LOADARM_STATE.AUTHORIZED)
				{
					this.SetState(LOADARM_STATE.AUTHORIZED_PERMISSIVE_PROMPT);
				}
				else if (this.LoadArmState == LOADARM_STATE.INPROGRESS)
				{
					this.SetState(LOADARM_STATE.INPROGRESS_PERMISSIVE_PROMPT);
				}
			}
		}

		public override void ProcessPermissiveMessageAcknowledge(StationManagerClass stationManager, string response)
		{
			foreach (LoadArmManagerClass loadArmManager in stationManager.LoadArmManagerCollection)
			{
				if (loadArmManager.IsInAlarm)
				{
					continue;
				}

				if (loadArmManager.GetStationManager() != stationManager)
				{
					continue;
				}

				if (loadArmManager.LoadArmState == LOADARM_STATE.INPROGRESS_PERMISSIVE_PROMPT)
				{
					loadArmManager.SetState(LOADARM_STATE.INPROGRESS);
					loadArmManager.ReleaseKeyPad();
				}
				else if (loadArmManager.LoadArmState == LOADARM_STATE.AUTHORIZED_PERMISSIVE_PROMPT)
				{
					loadArmManager.CancelPresetting(stationManager);
					loadArmManager.EndBatch();
					loadArmManager.IssueSelectPrompt(stationManager);
				}
			}
		}

		protected override string SwingArmPosition
		{
			get
			{
				var s = this.SwingArmStatusPV.ServerValue as string;
				return s ?? "A";
			}
		}

		public override bool IsInAlarm
		{
			get
			{
				if (this.AlarmPV != null
				&& this.AlarmPV.IsQualityGood
				&& this.AlarmPV.ServerValue is bool)
				{
					return (bool)this.AlarmPV.ServerValue;
				}
				else
				{
					return false;
				}
			}
		}

		protected void CheckForStopKey()
		{
			if (this.GetKeyPV != null
			&& this.GetKeyPV.IsQualityGood
			&& this.IsActiveStopKey(this.GetKeyPV.ServerValue as string)
			&& !this.IsFlowing()
			&& !this.IsReleased())
			{
				this.ProcessStopKey();
			}
		}


		protected virtual bool IsActiveStopKey(string value)
		{
			return value == "*S1";
		}


		protected void ProcessStopKey()
		{
			StationManagerClass stationManager = this.GetStationManager();
			if (stationManager != null && stationManager.IsRemoteAuthorized == false)
			{
				switch (stationManager.StationState)
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
								case LOADARM_STATE.AUTHORIZED:
								case LOADARM_STATE.AUTHORIZED_PERMISSIVE_PROMPT:
									{
										this.CancelPresetting(stationManager);

										if (this.AuthorizedPV.IsQualityGood
										&& ((bool)this.AuthorizedPV.ServerValue))
										{
											this.EndBatch();
										}

										this.IssueSelectPrompt(stationManager);
										break;
									}

								case LOADARM_STATE.INPROGRESS:
								case LOADARM_STATE.INPROGRESS_PERMISSIVE_PROMPT:
								case LOADARM_STATE.END_BATCH_PROMPT:
									this.IssueEndBatchPrompt();
									break;

								case LOADARM_STATE.BATCH_COMPLETE_PROMPT:
									this.SetFinishedLoading();
									break;

								case LOADARM_STATE.SELECT_PROMPT:
									this.SetFinishedLoading();
									break;


								default:
									this.IssueSelectPrompt(stationManager);
									break;
							}
							break;
						}

					default:
						stationManager.ProcessStopKey();
						break;
				}
			}
		}

		protected override bool IsFlowing()
		{
			return (this.FlowingPV.IsQualityGood && (bool)this.FlowingPV.ServerValue);
		}

		protected bool IsReleased()
		{
			return (this.LoadArmReleasedPV.IsQualityGood && (bool)this.LoadArmReleasedPV.ServerValue);
		}

		protected virtual string[] MenuWriteTags
		{
			get
			{
				string[] val = { ".Write First Line With Prompt",
										".Write Second Line",
										".Write Third Line",
										".Write Fourth Line" };

				return val;

			}

		}

		public override int DisplayMessage(string message, int responseLength, int messageTimeout)
		{
			if (this.IsInAlarm)
			{
				return 0;
			}

			StationManagerClass stationManager = this.GetStationManager();
			if (stationManager == null)
			{
				throw new OpcException("No Station Available");
			}

			if (messageTimeout > 999)
			{
				messageTimeout = 999;
			}

			// Always use variable length input strings
			if (responseLength > 0 && responseLength < 40)
			{
				responseLength += 40;
			}

			string translatedMessage = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	x =>
																	x.Get(this.SiteManager.Site.SiteGuid, message)
																);

			stationManager.WriteLogDataToCommFile(translatedMessage, true);

			// Prefix message ith Bay Identifier
			// When Station has Swing Arms provide Bay Prefix
			if (stationManager.HasSwingArms)
			{
				translatedMessage = this.LoadArm.LoadRackText != ""
					? FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	x =>
																	// ReSharper disable once AccessToModifiedClosure
																	x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|Bay") + " " + stationManager.Station.SwingArmPosition + " " + this.LoadArm.LoadRackText + ": " + translatedMessage
																)
					: FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	x =>
																	// ReSharper disable once AccessToModifiedClosure
																	x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|Bay") + " " + stationManager.Station.SwingArmPosition + ": " + translatedMessage
																);
			}

			// Prefix message with LoadArm Identifier
			else if (this.LoadArm.LoadRackText != "")
			{
				translatedMessage = this.LoadArm.LoadRackText + ": " + translatedMessage;
			}

			string[] writeTags = this.MenuWriteTags;

			char[] separator = { ' ' };
			string[] strings = translatedMessage.Split(separator);
			string[] lines = new string[4];
			int lineIndex = 0;
			foreach (string subMessage in strings)
			{
				if (subMessage.Length > this.MaxDisplayLineSize)
				{
					break;
				}

				if (lines[lineIndex] == null)
				{
					lines[lineIndex] = subMessage;
				}
				else if (lineIndex == lines.Length - 1
					|| lines[lineIndex + 1] == null)
				{
					if (lines[lineIndex].Length + subMessage.Length + 1 <= this.MaxDisplayLineSize)
					{
						lines[lineIndex] = lines[lineIndex] + " " + subMessage;
					}
					else
					{
						lineIndex++;
						if (lineIndex > lines.Length - 1)
						{
							break;
						}

						lines[lineIndex] = subMessage;
					}
				}

				else
				{
					lineIndex++;
					if (lines[lineIndex].Length + subMessage.Length + 1 < this.MaxDisplayLineSize)
					{
						lines[lineIndex] = lines[lineIndex] + " " + subMessage;
					}
					else
					{
						break;
					}
				}
			}

			ArrayList itemValues = new ArrayList();

			var LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			lineIndex = 0;
			foreach (string line in lines)
			{
				if (line == null)
				{
					break;
				}

				// Per Accuload III/IV documentation, last prompt command sent governs the response terminator and length
				// Therefore, we send the response terminator and length on ALL lines
				ItemValue writeLine = new ItemValue(LoadArmPV.OPCItemID + writeTags[lineIndex])
											{
													Value = " " + messageTimeout.ToString("D3") + " " + line + "&" + responseLength.ToString("D2")
											};

				itemValues.Add(writeLine);
				lineIndex++;
			}

			// Send the lot to the OPCServer
			this.OpcServerManager.Write(new URL(LoadArmPV.URL), (ItemValue[])itemValues.ToArray(typeof(ItemValue)));

			this.ResponsePending = true;

			return lineIndex;
		}

		// There is a descrepency.  Documentation says 30 characters but it is actually 29
		protected virtual int MaxDisplayLineSize => 29;

		public override void DisplayMenu(DisplayMenuParameters parameters)
		{
			if (this.IsInAlarm)
			{
				return;
			}

			StationManagerClass stationManager = this.GetStationManager();
			if (stationManager == null)
			{
				return;
			}

			stationManager.WriteMenuLogDataToCommFile(parameters);

			if (parameters.MenuTimeout > 999)
			{
				parameters.MenuTimeout = 999;
			}

			string[] writeTags = this.MenuWriteTags;

			ArrayList itemValues = new ArrayList();

			ProcessVariableClass loadArmPv = this.LoadArm.ProcessVariableCollection[0];

			// Write the title line
			int numberOfLines = this.DisplayMessage(parameters.Caption, 1, this.PromptTimeout);

			for (int nLoop = 0; nLoop < parameters.Menu.Length && nLoop < writeTags.Length - numberOfLines; ++nLoop)
			{
				ItemValue writeListItem = new ItemValue(loadArmPv.OPCItemID + writeTags[nLoop + numberOfLines]);

				string value = parameters.Menu[nLoop];
				if (parameters.ApplyDataDictionary)
				{
					value = this.GetDictionaryValueByKey(stationManager.Station.SiteGuid, value);
				}

				value = (nLoop + 1) + ". " + value + "                                       ";
				if (value.Length > this.MaxDisplayLineSize)
				{
					value = value.Substring(0, this.MaxDisplayLineSize);
				}

					// Add in the response & timeout.  For a menu, response is expected to be 0 or 1 characters
					// Per Accuload III/IV documentation, last prompt command sent governs the response terminator and length
					// Therefore, we send the response terminator and length on ALL lines
					writeListItem.Value = " " + parameters.MenuTimeout.ToString("D3") + " " + value + "&41";

				itemValues.Add(writeListItem);

			}

			// Send the lot to the OPCServer
			this.OpcServerManager.Write(new URL(loadArmPv.URL), (ItemValue[])itemValues.ToArray(typeof(ItemValue)));

			this.CurrentMenuParameters = parameters;
		}

		private string GetDictionaryValueByKey(Guid guid, string key)
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
			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			string TagPrefix = LoadArmPV.OPCItemID + ".Dynamic Values.Batch." + BatchNumber + ".";

			Item[] Items ={	new Item(new ItemIdentifier(TagPrefix+"Recipe")),
							};

			ItemValueResult[] Values = Server.Read(Items);

			Recipe = Values[0];
		}

		public void ReadAdditiveProductsUsingInjector(
			int AdditiveNumber,
			Server Server,
			out ItemValueResult ProductsUsingInjector)
		{
			this.OpcServerManager.Read(this.RecipePV);

			if (!this.RecipePV.IsQualityGood)
			{
				throw new Exception("ReadAdditiveProductsUsingInjector: Recipe OPC Quality Bad " + this.RecipePV.OPCItemID);
			}

			// The OPCTagID Prefix for Recipes will be the Prefix for the Arm
			// with the last Tag stripped off.  Typically Rack X or Rack X.Station Y
			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];
			char[] Seperators = { '.' };
			string[] OPCItemIDStrings = LoadArmPV.OPCItemID.Split(Seperators);
			string OPCItemID = "";
			for (int Index = 0; Index < OPCItemIDStrings.Length - 1; Index++)
			{
				OPCItemID = OPCItemID + OPCItemIDStrings[Index] + ".";
			}

			Item[] Items ={	new Item(new ItemIdentifier(	OPCItemID+
																			"Recipes."+
																			Convert.ToInt32(this.RecipePV.ServerValue).ToString("D2")+
																			".Additive Injector "+
																			AdditiveNumber.ToString("D2")+
																			" Products Using Injector"))
								};


			ItemValueResult[] Values = Server.Read(Items);

			ProductsUsingInjector = Values[0];
		}

		/// <summary>
		/// Read individual batch data from the OPC Server.
		/// </summary>
		/// <param name="batchNumber">
		/// batch number to retrieve data from.
		/// </param>
		/// <param name="server">
		/// OPC Server reference.
		/// </param>
		/// <param name="grossVolume">
		/// Gross Volume OPC result.
		/// </param>
		/// <param name="temperature">
		/// The temperature OPC result.
		/// </param>
		/// <param name="netVolume">
		/// The net volume OPC result.
		/// </param>
		public void ReadBatchData(
			string batchNumber,
			Server server,
			out ItemValueResult grossVolume,
			out ItemValueResult temperature,
			out ItemValueResult netVolume,
			out ItemValueResult pressure)
		{
			ProcessVariableClass loadArmPv = this.LoadArm.ProcessVariableCollection[0];

			string tagPrefix = loadArmPv.OPCItemID + ".Dynamic Values.Batch." + batchNumber + ".";

			Item[] items ={	new Item(new ItemIdentifier(tagPrefix+"Gross Volume")),
									new Item(new ItemIdentifier(tagPrefix+"Average Temperature")),
									new Item(new ItemIdentifier(tagPrefix+"GST Volume")),
									new Item(new ItemIdentifier(tagPrefix+"Average Pressure"))
							};

			ItemValueResult[] values = server.Read(items);

			grossVolume = values[0];
			temperature = values[1];
			netVolume = values[2];
			pressure = values[3];
		}

		/// <summary>
		/// Read individual batch mass from the OPC server.
		/// </summary>
		/// <param name="batchNumber">
		/// batch number to retrieve data from.
		/// </param>
		/// <param name="server">
		/// OPC Server reference.
		/// </param>
		/// <param name="mass">
		/// The mass OPC result.
		/// </param>
		internal void ReadBatchMass(string batchNumber, Server server, out ItemValueResult mass)
		{
				ProcessVariableClass loadArmPv = this.LoadArm.ProcessVariableCollection[0];

				string tagPrefix = loadArmPv.OPCItemID + ".Dynamic Values.Batch." + batchNumber + ".";

				Item[] items =
					{
						new Item(new ItemIdentifier(tagPrefix + "Mass Total"))
					};

				ItemValueResult[] values = server.Read(items);

				mass = values[0];
		}

		public override void ReadPresetAmount(
			Server Server,
			out ItemValueResult PresetAmount)
		{
			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			Item[] Items = { new Item(new ItemIdentifier(LoadArmPV.OPCItemID + ".Preset Amount")) };

			ItemValueResult[] Values = Server.Read(Items);

			PresetAmount = Values[0];
		}

		internal virtual void ReadComponentBatchData(
			int productNumber,
			Server server,
			out ItemValueResult grossVolume,
			out ItemValueResult standardDensity,
			out ItemValueResult temperature,
			out ItemValueResult netVolume,
			out ItemValueResult ctpl,
			out ItemValueResult pressure)
		{
			ProcessVariableClass loadArmPv = this.LoadArm.ProcessVariableCollection[0];

			string tagPrefix = loadArmPv.OPCItemID + ".Dynamic Values.Product " + productNumber.ToString() + ".Batch.";

			Item[] SubItems ={	new Item(new ItemIdentifier(tagPrefix+"Gross Volume")),
										new Item(new ItemIdentifier(tagPrefix+"Average Reference Density")),
										new Item(new ItemIdentifier(tagPrefix+"Average Temperature")),
										new Item(new ItemIdentifier(tagPrefix+"GSV Volume")),
										new Item(new ItemIdentifier(tagPrefix+"Average CTPL")),
										new Item(new ItemIdentifier(tagPrefix+"Average Pressure"))
									};

			ItemValueResult[] values = server.Read(SubItems);

			grossVolume = values[0];
			standardDensity = values[1];
			temperature = values[2];
			netVolume = values[3];
			ctpl = values[4];
			pressure = values[5];
		}

		internal void ReadComponentBatchMass(int productNumber, Server server, out ItemValueResult mass)
		{
				ProcessVariableClass loadArmPv = this.LoadArm.ProcessVariableCollection[0];

				string tagPrefix = loadArmPv.OPCItemID + ".Dynamic Values.Product " + productNumber.ToString(CultureInfo.InvariantCulture) + ".Batch.";

				Item[] subItems =
					{
						new Item(new ItemIdentifier(tagPrefix + "Mass Total"))
					};

				ItemValueResult[] values = server.Read(subItems);

				mass = values[0];
		}

		public virtual void ReadProductNonResettableTotal(
			int productNumber,
			Server server,
			out ItemValueResult nonResettableGrossVolume)
		{
			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			Item[] SubItems ={	new Item(new ItemIdentifier(LoadArmPV.OPCItemID+".Non-Resettable Totals.Product "+productNumber.ToString()+".Gross Volume"))
								};

			ItemValueResult[] Values = server.Read(SubItems);

			nonResettableGrossVolume = Values[0];
		}



		public void ReadBatchAdditiveData(
			string BatchNumber,
			int AdditiveNumber,
			Server Server,
			out ItemValueResult AdditiveVolume)
		{
			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			Item[] SubItems =	{	new Item(new ItemIdentifier(LoadArmPV.OPCItemID+".Dynamic Values.Batch."+BatchNumber+".Additives."+AdditiveNumber.ToString("D2")+".Volume"))
									};

			ItemValueResult[] Values = Server.Read(SubItems);

			AdditiveVolume = Values[0];
		}

		public void ReadAdditiveNonResettableTotal(
			int AdditiveNumber,
			Server Server,
			out ItemValueResult NonResettableGrossVolume)
		{
			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			Item[] SubItems =	{	new Item(new ItemIdentifier(LoadArmPV.OPCItemID+".Non-Resettable Totals.Additives."+AdditiveNumber.ToString("D2")+".Volume"))
									};

			ItemValueResult[] Values = Server.Read(SubItems);

			NonResettableGrossVolume = Values[0];
		}

		public void ReadBatchFlowControlledAdditiveData(
				int additiveNumber,
				Server server,
				out ItemValueResult grossVolume,
				out ItemValueResult temperature,
				out ItemValueResult netVolume,
				out ItemValueResult ctl)
		{
				ProcessVariableClass loadArmPv = this.LoadArm.ProcessVariableCollection[0];

				string tagPrefix = loadArmPv.OPCItemID + ".Dynamic Values.Flow Controlled Additives." + additiveNumber.ToString("D2", CultureInfo.InvariantCulture) + ".Batch.";

				Item[] subItems =
				{
					new Item(new ItemIdentifier(tagPrefix + "Gross Volume")),
					new Item(new ItemIdentifier(tagPrefix + "Average Temperature")),
					new Item(new ItemIdentifier(tagPrefix + "GST Volume")),
					new Item(new ItemIdentifier(tagPrefix + "Average CTL"))
				};

				ItemValueResult[] values = server.Read(subItems);

				grossVolume = values[0];
				temperature = values[1];
				netVolume = values[2];
				ctl = values[3];
		}

		public void ReadBatchFlowControlledAdditiveMass(int additiveNumber, Server server, out ItemValueResult mass)
		{
				ProcessVariableClass loadArmPv = this.LoadArm.ProcessVariableCollection[0];

				string tagPrefix = loadArmPv.OPCItemID + ".Dynamic Values.Flow Controlled Additives." + additiveNumber.ToString("D2", CultureInfo.InvariantCulture) + ".Batch.";

				Item[] subItems =
					{
						new Item(new ItemIdentifier(tagPrefix + "Mass Total"))
					};

				ItemValueResult[] values = server.Read(subItems);

				mass = values[0];
		}

		public override bool Authorize(StationManagerClass stationManager, double preset)
		{
			if (stationManager.Station.Type == STATION_TYPE.OFF_LOADING)
			{
				ProductMapClass component = this.LoadArm.ComponentCollection[0];

				TankClass tank = this.SiteManager.GetTank(component, stationManager.Manager);
				if (tank == null)
				{
					this.eventLog.WriteEntry("Authorize : Invalid Tank " + component.TankOrGroupID, EventLogEntryType.Error);
					this.DisplayMessage("LoadRack|Invalid Tank" + component.TankOrGroupID, 0, 999);
					Thread.Sleep(3000);
					stationManager.StationState = StationState.IDLE;
					FMChannelHelper.MakeCall<IAlarmAndEventLogs>(alarmAndEventLogs => alarmAndEventLogs.Add(this.Security, this.LoadArm.NoTankCapcityReminingEventString(stationManager.Station.ID, component.TankOrGroupID)));
					return false;
				}

				ProcessVariableClass tankVariable = stationManager.SiteManager.Site.LoadByNet
					? tank.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.REMAINING_NET_VOLUME_PV]
					: tank.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.REMAINING_GROSS_VOLUME_PV];
				if (tankVariable == null || (!tankVariable.IsQualityGood && !this.SiteManager.Site.UseLastKnownGoodTankData))
				{
					this.eventLog.WriteEntry("Authorize : remaining gross volume OPC Quality Bad " + ((tankVariable == null) ? "*undefined*" : tankVariable.OPCItemID), EventLogEntryType.Error);
					this.DisplayMessage("LoadRack|Remaining Volume OPC Quality Bad", 0, 999);
					Thread.Sleep(3000);
					stationManager.StationState = StationState.IDLE;
					FMChannelHelper.MakeCall<IAlarmAndEventLogs>(alarmAndEventLogs => alarmAndEventLogs.Add(this.Security, this.LoadArm.NoTankCapcityReminingEventString(stationManager.Station.ID, tank.ID)));
					return false;
				}

				EngineeringUnit units = (stationManager.CurrentTransactionAlias.VolumeUnits != 0)
									? stationManager.CurrentTransactionAlias.VolumeUnits
									: stationManager.SiteManager.Site.VolumeUnits;
				var remainingVolume = new SIDouble { Units = units, SIValue = System.Convert.ToDouble(tankVariable.SIValue) };
				if (preset > remainingVolume.Value)
				{
					this.DisplayMessage("LoadRack|No Tank Capcity Remining", 0, 999);
					Thread.Sleep(3000);
					stationManager.StationState = StationState.IDLE;
					FMChannelHelper.MakeCall<IAlarmAndEventLogs>(alarmAndEventLogs => alarmAndEventLogs.Add(this.Security, this.LoadArm.NoTankCapcityReminingEventString(stationManager.Station.ID, tank.ID)));
					return false;
				}
			}

			ProcessVariableClass loadArmPv = this.LoadArm.ProcessVariableCollection[0];
				ProcessVariableClass authorizeTransaction = new ProcessVariableClass
																		{
																				URL = loadArmPv.URL,
																				OPCItemID = loadArmPv.OPCItemID + ".Authorize And Set Batch Amount"
																		};


				if (stationManager.Station.Type == STATION_TYPE.OFF_LOADING)
			{
				double offLoadPresetAmount = stationManager.OffLoadPresetAmount;
				authorizeTransaction.ServerValue = offLoadPresetAmount.ToString(CultureInfo.InvariantCulture);
			}
			else
			{
				authorizeTransaction.ServerValue = stationManager.Station.SetDefaultPresetToZero ? "0" : preset.ToString(CultureInfo.InvariantCulture);
			}

			StopWatch timer = new StopWatch(StopWatch.Appnames.LoadRackService, "Authorize : " + loadArmPv.OPCItemID);
			try
			{
				this.OpcServerManager.Write(authorizeTransaction);
			}
			catch (Exception e)
			{
				this.eventLog.WriteEntry("Accuload III LoadArmManager Authorize : " + e.Message + e.StackTrace, EventLogEntryType.Error);

				// Typical Errors are excessive preset or max batches.	
				try
				{
					this.SetState(LOADARM_STATE.SELECT_PROMPT);
					this.DisplayMessage("LoadRack|Authorize To Preset Error", 0, this.MessageTimeout);
					return true;
				}
				catch (Exception e1)
				{
					this.eventLog.WriteEntry("Accuload III LoadArmManager Authorize : " + e1.Message + e1.StackTrace, EventLogEntryType.Error);
				}
			}
			finally
			{
				timer.Stop();
			}

			this.SetState(LOADARM_STATE.AUTHORIZED);
			return true;
		}

		public override bool AllocateRecipes(ulong recipeMap, ulong extendedRecipeMap)
		{
			ProcessVariableClass loadArmPv = this.LoadArm.ProcessVariableCollection[0];

			ProcessVariableClass allocateRecipes = new ProcessVariableClass
																{
																	URL = loadArmPv.URL,
																	OPCItemID = loadArmPv.OPCItemID + ".Allocate Recipes",
																	ServerValue = ""
																};

			for (int offset = 0; offset < this.NumberOfOffsets; offset++)
			{
				ulong mapCharacter = (recipeMap >> offset * 4) & 0xF;
				allocateRecipes.ServerValue += mapCharacter.ToString("X1");
			}

			StopWatch timer = new StopWatch(StopWatch.Appnames.LoadRackService, "Allocate Recipes : " + loadArmPv.OPCItemID);
			try
			{
				OpcServerManager.Write(allocateRecipes);
                try
                {
                    OpcServerManager.Read(RecipePV);
                }
                catch (Exception)
            {
					// Do nothing here; the read above is to attempt to force the subscription to update
					// If it fails, we're able to continue.
                }
			}
			catch (Exception e)
			{
				this.eventLog.WriteEntry("Accuload III LoadArmManager AllocateRecipes : " + e.Message + e.StackTrace, EventLogEntryType.Error);
				return false;
			}
			finally
			{
				timer.Stop();
			}

			return true;
		}

		public override int NumberOfOffsets => 13;

		public override void Start()
		{
			ProcessVariableClass loadArmPv = this.LoadArm.ProcessVariableCollection[0];
				ProcessVariableClass endTransaction = new ProcessVariableClass
																{
																		URL = loadArmPv.URL,
																		OPCItemID = loadArmPv.OPCItemID + ".Start"
																};
				this.OpcServerManager.Write(endTransaction);
		}

		public override void Stop()
		{
			this.SetState(LOADARM_STATE.STOPPING);

			if (!this.LoadArmReleasedPV.IsQualityGood)
			{
				return;
			}

			if (!(bool)this.LoadArmReleasedPV.ServerValue)
			{
				return;
			}

			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];
			ItemValue[] SubItems = { new ItemValue(new ItemIdentifier(LoadArmPV.OPCItemID + ".Stop")) };
			this.OpcServerManager.Write(new URL(LoadArmPV.URL), SubItems);

			for (int Iterator = 0; Iterator < MaxDelayForNoflowSeconds; Iterator++)
			{
				this.OpcServerManager.Read(this.FlowingPV);

				if (this.LoadArmReleasedPV.IsQualityGood
				&& (bool)this.LoadArmReleasedPV.ServerValue == false)
				{
					break;
				}

				Thread.Sleep(1000);
			}
		}

		public override void Unauthorize()
		{
			StationManagerClass StationManager = this.GetStationManager();
			if (StationManager == null)
			{
				return;
			}

			if (this.LoadArmState == LOADARM_STATE.AUTHORIZED
			|| this.LoadArmState == LOADARM_STATE.AUTHORIZED_PERMISSIVE_PROMPT
			|| this.LoadArmState == LOADARM_STATE.INPROGRESS
			|| this.LoadArmState == LOADARM_STATE.INPROGRESS_PERMISSIVE_PROMPT)
			{
				this.Stop();
				// this.EndBatch(); // not going to work here, EndBatch will fail while product is still flowing, and product flow doesn't stop immediately
			}

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

			Response = nSelection == 0 ? StationManagerClass.EscapeString : this.CurrentMenuParameters.Menu[nSelection - 1];

			base.ProcessSplashBlendComponentPromptResponse(StationManager, Response);
		}


		public override void SendEndTransaction()
		{
			ProcessVariableClass loadArmPv = this.LoadArm.ProcessVariableCollection[0];
			ProcessVariableClass endTransaction = new ProcessVariableClass
																{
																	URL = loadArmPv.URL,
																	OPCItemID = loadArmPv.OPCItemID + ".End Transaction"
																};
			this.OpcServerManager.Write(endTransaction);
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
					{
						continue;
					}

						return false;
				}

				ProcessVariableClass densityPv = tank.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.STANDARD_DENSITY_PV];
				if (densityPv == null
				|| (!densityPv.IsQualityGood
				&& !this.SiteManager.Site.UseLastKnownGoodTankData))
				{
					return false;
				}

				StopWatch timer = new StopWatch(StopWatch.Appnames.LoadRackService, "Update Reference Density : " + loadArmPv.OPCItemID);

				double density;
				try
				{
					density = Convert.ToDouble(densityPv.GetValue(units, decimalPlaces));
				}
				catch
				{
					return false;
				}
				finally
				{
					timer.Stop();
				}

				ItemValue writeDensityItem = new ItemValue(loadArmPv.OPCItemID + ".Program Code Change")
														{
															Value = "P" + component.PresetNumber.ToString("D1") + " 412 " + density.ToString("F")
														};

				itemValues.Add(writeDensityItem);
			}

			itemValues.Add(new ItemValue(loadArmPv.OPCItemID + ".Log Out of Program Mode"));

			try
			{
				this.OpcServerManager.Write(new URL(loadArmPv.URL), (ItemValue[])itemValues.ToArray(typeof(ItemValue)));
					Thread.Sleep(1000);
			}
			catch (Exception e)
			{
				this.eventLog.WriteEntry(e + e.StackTrace, EventLogEntryType.Error);
				return false;
			}

			return true;
		}

		public override bool UpdateMaximumPreset(StationManagerClass stationManager)
		{
			ArrayList itemValues = new ArrayList();

			double maxPreset = stationManager.SiteManager.Site._MaximumLoadAmount.Value;

			ProcessVariableClass loadArmPv = this.LoadArm.ProcessVariableCollection[0];

			ItemValue writeMaximumPresetItem = new ItemValue(loadArmPv.OPCItemID + ".Program Code Change")
															{
																Value = "SY 311 " + maxPreset.ToString("F")
															};

			itemValues.Add(writeMaximumPresetItem);

			try
			{
				this.OpcServerManager.Write(new URL(loadArmPv.URL), (ItemValue[])itemValues.ToArray(typeof(ItemValue)));
			}
			catch (Exception e)
			{
				this.eventLog.WriteEntry(e + e.StackTrace, EventLogEntryType.Error);
				return false;
			}

			return true;
		}

        // Update the Amount/Cycle and Rate parameters for the Recipe
        /// <summary>
        /// Applies the additive profile to the preset device and updates the display name of the recipe
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
        /// True is successful; false on error/failure
        /// </returns>
        public override bool UpdateRecipe(
			string name,
			ProductMapClass recipe,
				ProductClass product,
				AdditiveProfileClass additiveProfile,
				int deviceRecipeNumber)
		{
			StationManagerClass stationManager = this.GetStationManager();
			if (stationManager == null)
			{
				throw new Exception("Update Recipe : No Station Manager");
			}

			// Build a collection of Addtive Injectors that must be updated in the recipe
			ProductMapCollectionClass additiveInjectorCollection = new ProductMapCollectionClass();

			// When Additive Profile, add each Injector that is part of the profile
			if (additiveProfile != null)
			{
				foreach (ProductMapClass additive in additiveProfile.AdditiveCollection)
				{
					ProductMapClass additiveInjector = this.GetAdditive(additive.AssignedGuid);
					if (additiveInjector == null)
					{
						throw new Exception("Update Recipe : Additive not found in LoadArm Configuration");
					}

					// Make sure each has a unique preset number
					bool found = false;
					foreach (ProductMapClass existingAdditiveInjector in additiveInjectorCollection)
					{
						if (additiveInjector.PresetNumber == existingAdditiveInjector.PresetNumber)
						{
							found = true;
							break;
						}
					}

					if (found)
					{
						throw new Exception("Update Recipe : Additive not unique in LoadArm Configuration");
					}

					additiveInjectorCollection.Add(additiveInjector);
				}
			}

			// Add any flow controlled additives
			foreach (ProductMapClass additive in this.LoadArm.FlowControlledAdditiveCollection)
			{
				additive._AdditiveCycleVolume.Value = 0.0;

				ProductMapClass component = null;
				if (product.ProductType == ProductType.BlendProduct)
				{
					component = product.ComponentCollection.Find(x => x.AssignedGuid == additive.AssignedGuid);
				}

				additive._AdditiveRate.Value = component != null ? component.BlendPercentage : 0.0;

				additiveInjectorCollection.Add(additive);
			}

			// Add any remaining Injectors with unique Preset Numbers
			foreach (ProductMapClass additiveInjector in this.LoadArm.AdditiveInjectorCollection)
			{
				bool found = false;
				foreach (ProductMapClass existingAdditiveInjector in additiveInjectorCollection)
				{
					if (additiveInjector.PresetNumber == existingAdditiveInjector.PresetNumber)
					{
						found = true;
						break;
					}
				}

				if (!found)
				{
					additiveInjectorCollection.Add(additiveInjector);
				}
			}


			ProcessVariableClass loadArmPv = this.LoadArm.ProcessVariableCollection[0];

			ArrayList items = new ArrayList();

			if (!stationManager.Station.InhibitSettingRecipeNames)
			{
				ItemValue nameItem = new ItemValue(loadArmPv.OPCItemID + ".Program Code Change");

				if (name.Length > 9)
				{
					name = name.Substring(0, 9);
				}

				nameItem.Value = deviceRecipeNumber.ToString("D2") + " 002 " + name;

				items.Add(nameItem);
			}

			foreach (ProductMapClass additiveInjector in additiveInjectorCollection)
			{
				ItemValue cycleItem = new ItemValue(loadArmPv.OPCItemID + ".Program Code Change");

				ItemValue rateItem = new ItemValue(loadArmPv.OPCItemID + ".Program Code Change");


				cycleItem.Value = deviceRecipeNumber.ToString("D2") + " " + ((additiveInjector.PresetNumber - 1) * 3 + 17).ToString("D3") + " 0.0";
				rateItem.Value = deviceRecipeNumber.ToString("D2") + " " + ((additiveInjector.PresetNumber - 1) * 3 + 18).ToString("D3") + " 0.0";

				if (additiveInjector.Type == PRODUCT_MAP_TYPE.PRESET_INJECTOR_MAP)
				{
					ProductMapClass additive = additiveProfile?.AdditiveCollection.Find(x => x.AssignedGuid == additiveInjector.AssignedGuid);
					if (additive != null)
					{
						var cycleVolume = StationManagerClass.Convert(additive._AdditiveCycleVolume.SIValue, EngineeringUnit.FmvMeter3, this.SiteManager.Site.AdditiveProfileCycleAmountUnits);
						var cycleRate = StationManagerClass.Convert(additive._AdditiveRate.SIValue, EngineeringUnit.FmvMeter3, this.SiteManager.Site.AdditiveProfileRateUnits);

						cycleItem.Value = deviceRecipeNumber.ToString("D2") + " "
													+ ((additiveInjector.PresetNumber - 1) * 3 + 17).ToString("D3") + " "
													+ cycleVolume.ToString("F");

						rateItem.Value = deviceRecipeNumber.ToString("D2") + " "
											+ ((additiveInjector.PresetNumber - 1) * 3 + 18).ToString("D3") + " "
											+ cycleRate.ToString("F");
					}
				}
				else
				{
						// Flow Controlled Additive
						rateItem.Value = deviceRecipeNumber.ToString("D2") + " " + (((additiveInjector.PresetNumber - 1) * 3) + 18).ToString("D3") + " " + additiveInjector._AdditiveRate.Value.ToString("F");
						items.Add(rateItem);
				}

				items.Add(cycleItem);
				items.Add(rateItem);
			}

			if (items.Count == 0)
			{
				return true;
			}

			StopWatch timer = new StopWatch(StopWatch.Appnames.LoadRackService, "Update Recipe : " + loadArmPv.OPCItemID + " " + recipe.AssignedID);

			try
			{
				this.OpcServerManager.Write(new URL(loadArmPv.URL), (ItemValue[])items.ToArray(typeof(ItemValue)));
			}
			catch (Exception e)
			{
				this.eventLog.WriteEntry(e + e.StackTrace, EventLogEntryType.Error);
				return false;
			}
			finally
			{
				timer.Stop();
			}

			return true;
		}

		public override bool LogOutOfProgramMode()
		{
			ProcessVariableClass loadArmPv = this.LoadArm.ProcessVariableCollection[0];

			ItemValue[] items = { new ItemValue(loadArmPv.OPCItemID + ".Log Out of Program Mode") };

			try
			{
				this.OpcServerManager.Write(new URL(loadArmPv.URL), items);
					Thread.Sleep(1000);
			}
			catch (Exception e)
			{
				this.eventLog.WriteEntry(e + e.StackTrace, EventLogEntryType.Error);
				return false;
			}

			return true;
		}


		public override void ReleaseKeyPad()
		{
			try
			{
				this.OpcServerManager.Write(this.ReleaseKeypadPV);
			}
			catch (Exception e)
			{
				this.eventLog.WriteEntry(e.Message + e.StackTrace, EventLogEntryType.Error);
			}
		}

		public override bool IsTransactionInProgress()
		{
			// Read the status flag for this arm and check for Transaction In Progress

			ProcessVariableClass loadArmPv = this.LoadArm.ProcessVariableCollection[0];

			Server server = new Server(new Factory(), new URL(loadArmPv.URL));
			server.Connect();

			Item[] subItems = { new Item(new ItemIdentifier(loadArmPv.OPCItemID + ".Status.Transaction In Progress")) };

			ItemValueResult[] values = server.Read(subItems);

			server.Disconnect();
			server.Dispose();

			return Convert.ToBoolean(values[0].Value);

		}

		public override void ResetPowerFailAlarm()
		{
			ProcessVariableClass loadArmPv = this.LoadArm.ProcessVariableCollection[0];

			// Only required if Arm 1
			if (loadArmPv.OPCItemID.IndexOf("Arm 1", StringComparison.Ordinal) == -1)
			{
				return;
			}

			// Strip off the last part of the path
			int nLastIndex = loadArmPv.OPCItemID.LastIndexOf(".", StringComparison.Ordinal);
			string opcPath = loadArmPv.OPCItemID.Substring(0, nLastIndex);

			Server server = new Server(new Factory(), new URL(loadArmPv.URL));
			server.Connect();

			ItemValue[] subItems =	
					{
						new ItemValue(new ItemIdentifier(opcPath+".Alarms.Reset Power-fail Alarm")),
					new ItemValue(new ItemIdentifier(opcPath+".Status.Reset Powerfail"))
				};

			server.Write(subItems);

			server.Disconnect();
			server.Dispose();
		}

		public override void ResetCommunicationsFailAlarm()
		{
				ProcessVariableClass loadArmPv = this.LoadArm.ProcessVariableCollection[0];

				// Only required if Arm 1
				if (loadArmPv.OPCItemID.IndexOf("Arm 1", StringComparison.Ordinal) == -1)
				{
					return;
				}

				// Strip off the last part of the path
				int lastIndex = loadArmPv.OPCItemID.LastIndexOf(".", StringComparison.Ordinal);
				string opcPath = loadArmPv.OPCItemID.Substring(0, lastIndex);

				var server = new Server(new OpcCom.Factory(), new URL(loadArmPv.URL));
				server.Connect();

				ItemValue[] subItems =
					{
						new ItemValue(new ItemIdentifier(opcPath + ".Alarms.Reset Communications Failure"))
					};

				server.Write(subItems);

				server.Disconnect();
				server.Dispose();
		}

		public override void SetFocus()
		{
			ProcessVariableClass loadArmPv = this.LoadArm.ProcessVariableCollection[0];

			Server server = new Server(new Factory(), new URL(loadArmPv.URL));
			server.Connect();

			ItemValue[] subItems =	{ new ItemValue(new ItemIdentifier(loadArmPv.OPCItemID+".Force Full Screen View"))
											};

			server.Write(subItems);

			server.Disconnect();
			server.Dispose();
		}

		public override void SyncDateAndTime()
		{
			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			// Only required if Arm 1
			if (LoadArmPV.OPCItemID.IndexOf("Arm 1") == -1)
			{
				return;
			}

			StationManagerClass StationManager = this.GetStationManager();
			if (StationManager == null)
			{
				return;
			}

			// Get the date/time value for the device
			string sDateTimeValue = StationManager.GetDateTimeSettingCommand();

			if (sDateTimeValue != "" && sDateTimeValue != null)
			{

				// Strip off the last part of the path
				int nLastIndex = LoadArmPV.OPCItemID.LastIndexOf(".");
				string OPCPath = LoadArmPV.OPCItemID.Substring(0, nLastIndex);

				// Create an itemvalue object and fill it
				ItemValue IV = new ItemValue
				{
					ItemName = OPCPath + ".Set Date And Time",
					Value = sDateTimeValue
				};

				ItemValue[] ItemValues = { IV };

				this.OpcServerManager.Write(new URL(LoadArmPV.URL), ItemValues);
			}

		}

		public override string GetBatchNumber(StationManagerClass stationManager)
		{
			// Read the status flag for this arm and check for Transaction In Progress

			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			// Strip off the last part of the path
			string OPCPath = LoadArmPV.OPCItemID;

			Server Server = new Server(new Factory(), new URL(LoadArmPV.URL));
			Server.Connect();

			Item[] SubItems = { new Item(new ItemIdentifier(OPCPath + ".Status.Get Batch Number")) };

			int retry = 0;
			ItemValueResult[] values = null;

			while (retry < 3)
			{
				values = Server.Read(SubItems);

				if (values[0].Quality == Quality.Good)
				{
					break;
				}

				retry++;
				
                // we need to delay for 100mSec
				Thread.Sleep(100);
			}

			Server.Disconnect();
			Server.Dispose();

		    if (retry == 3)
		    {
		        if (values != null)
		        {
		            throw new Exception("GetBatchNumber : Batch Number OPC Quality Bad " + values[0].ItemName);
		        }
		        
                throw new Exception("GetBatchNumber : Batch Number OPC Quality Bad  - no values read");
		    }

			return (Convert.ToString(values[0].Value));
		}


		public override void EndBatch()
		{
			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];
			ItemValue[] SubItems = { new ItemValue(new ItemIdentifier(LoadArmPV.OPCItemID + ".End Batch")) };
			this.OpcServerManager.Write(new URL(LoadArmPV.URL), SubItems);
		}

		public override void CaptureMeterValues()
		{
			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			StopWatch timer = new StopWatch(StopWatch.Appnames.LoadRackService, "Capture Meter Values : " + LoadArmPV.OPCItemID);

			try
			{

				Server Server = new Server(new Factory(), new URL(LoadArmPV.URL));
				NetworkCredential Credentials = null;
				Server.Connect(new ConnectData(Credentials));

				// Components
				foreach (ProductMapClass ArmComponent in this.LoadArm.ComponentCollection)
				{
					ItemValueResult NonResettableGrossVolume;

					this.ReadProductNonResettableTotal(
						ArmComponent.PresetNumber,
						Server,
						out NonResettableGrossVolume);

					if (NonResettableGrossVolume.Quality != Quality.Good)
					{
						this.eventLog.WriteEntry("CaptureMeterValues : Non-Resettable Gross Volume OPC Quality Bad " + NonResettableGrossVolume.ItemName, EventLogEntryType.Error);
					}
					else
					{
						ArmComponent.MeterValue = Convert.ToDouble(NonResettableGrossVolume.Value);
					}
				}

				// Additives
				foreach (ProductMapClass AdditiveInjector in this.LoadArm.AdditiveInjectorCollection)
				{
					ItemValueResult NonResettableGrossVolume;

					this.ReadAdditiveNonResettableTotal(
						AdditiveInjector.PresetNumber,
						Server,
						out NonResettableGrossVolume);

					if (NonResettableGrossVolume.Quality != Quality.Good)
					{
						this.eventLog.WriteEntry("CaptureMeterValues : Non-Resettable Gross Volume OPC Quality Bad " + NonResettableGrossVolume.ItemName, EventLogEntryType.Error);
					}
					else
					{
						AdditiveInjector.MeterValue = Convert.ToDouble(NonResettableGrossVolume.Value);
					}
				}

				Server.Disconnect();
				Server.Dispose();
			}

			finally
			{
				timer.Stop();
			}
		}

        /// <summary>
        /// This function clears all products on the physical preset assigned to the arm,
        /// as determined by the RecipeMap.  It also removes the recipe from the specified stations 
        /// recipe map
        /// </summary>
        /// <param name="stationManager">StationManager to clear recipe mapping from</param>
        protected override void ClearArmProducts(StationManagerClass stationManager)
        {
			if (stationManager == null)
			{
				return;
			}

            if (!stationManager.Station.EnableDynamicRecipes)
			{
				// if Dynamic Recipes is not enabled, do nothing
				return;
			}

			int numberOfUsableRecipes = AcculoadIIIStationManagerClass.MaxRecipes - stationManager.PhysicalArmsOnPreset;
			for (int recipeMapPosition = 1; recipeMapPosition <= numberOfUsableRecipes; recipeMapPosition++)
			{
				if ((this.Bay(stationManager).RecipeMap & (1UL << (recipeMapPosition - 1))) > 0)
				{
					stationManager.ClearSingleRecipe(recipeMapPosition);
					this.Bay(stationManager).RecipeMap ^= 1UL << (recipeMapPosition - 1);
				}
			}

			this.LogOutOfProgramMode();
        }
    }
}
