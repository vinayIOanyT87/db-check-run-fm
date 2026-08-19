/******************************************************************************

	FILE NAME:		MultiloadIILoadArmManager.cs


	PURPOSE:			MultiloadIILoadArmManagerClass


	COMMENTS:

		Copyright (C) Varec, Inc. Norcross, GA, USA, 2009

		This file shall not be copied or reproduced in any form without
				the express written consent of Varec Inc.


	AUTHOR(S):	B. Schaal


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------

*******************************************************************************/

namespace LoadRackLibrary
{
	using System;
	using System.Collections;
	using System.Diagnostics;
	using System.Runtime.InteropServices;
	using System.Threading;
	using System.Globalization;

	using FMBusinessObjects.DataObjects;

	using Opc;
	using Opc.Da;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	using Convert = System.Convert;
	using System.Text;
	using FMBusinessObjects.LogClient;

	public class MultiloadIILoadArmManagerClass : LoadArmManagerClass
	{
		protected static readonly string Esc = Convert.ToChar(27).ToString(CultureInfo.InvariantCulture);

		protected ManualResetEvent KillEvent = null;
		protected Thread ScanThread = null;
		protected int MessageTimer = 0;
		protected ProcessVariableClass LoadArmStatePV;
		protected ProcessVariableClass RemoteMsgActivePV;
		protected ProcessVariableClass RcuStatusPV;
		protected ProcessVariableClass HostEnabledPV;
		protected ProcessVariableClass AuthorizedPV;
		protected int SelectedRecipe;
		protected ProcessVariableClass StationPV;

		public MultiloadIILoadArmManagerClass(
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
			this.StationPV = StationManager.Station.ProcessVariableCollection[0];

			string StationOPCPath = this.StationPV.OPCItemID;

			this.RcuStatusPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.LOADARM_PV,
				UNIT_TYPE.LOADARM_UNIT,
				VarEnum.VT_UI1,
				true,
				StationOPCPath + ".RCU Status",
				LoadArmURL,
				LoadArmProgID);

			this.OpcServerManager.AddProcessVariable(this.RcuStatusPV);

			ThreadStart scanStart = this.ScanDataThread;
			this.KillEvent = new ManualResetEvent(false);
			this.ScanThread = new Thread(scanStart);
			this.ScanThread.Start();
			this.ScanThread.Priority = ThreadPriority.AboveNormal;
		}

		~MultiloadIILoadArmManagerClass()
		{
			this.Dispose();
		}

		protected enum PresetState : ushort
		{
			Idle = 0,
			LowFlow = 1,
			HighFlow = 2,
			FirstTrip = 3,
			SecondTrip = 4,
			FinalTrip = 5,
			Start = 6,
			Alarm = 7,
			Complete = 8,
			NotAuth = 9,
			WaitTms = 10,
			Auth = 11,
			Preset = 12,
			Disabled = 13,
			Stop = 14,
			RemoteMsg = 15,
			EndOfBatch = 16,
			Archiving = 17,
			Clearing = 18,
			TransDone = 19
		}

		public override LOADARM_STATE LoadArmState
		{
			get
			{
				return base.LoadArmState;
			}

			set
			{
				string logString = this.GetStationManager().Station.ID + " "
										 + this.GetArmNumber(this.GetStationManager()).ToString(CultureInfo.InvariantCulture)
										 + ":  Changing state from " + base.LoadArmState + " to " + value;
				this.GetStationManager().WriteLogDataToCommFile(logString, StationManagerClass.CommLogDirection.None);
				base.LoadArmState = value;
			}
		}

		public override void Dispose()
		{
			if (!this.AlreadyDisposed)
			{
				try
				{
					ItemValue FunctionHostDown = new ItemValue(this.StationPV.OPCItemID + ".FHOSTDOWN");
					ItemValue TerminalCommand = new ItemValue(this.StationPV.OPCItemID + ".Terminal Command")
					{
						Value = Esc + "R"
					};
					this.OpcServerManager.Write(new URL(this.StationPV.URL), new ItemValue[] { FunctionHostDown, TerminalCommand });
				}
				catch (Exception e)
				{
					this.eventLog.WriteEntry("MultiloadIILoaddArmManager Dispose : " + e.Message);
				}

				base.Dispose();

				// Terminate the Scan Thread
				this.KillEvent?.Set();
				this.ScanThread?.Join();

				GC.SuppressFinalize(this);
				this.AlreadyDisposed = true;
			}
		}

		public void ScanDataThread()
		{
			this.InitializeLoadArmPvs();

			while (!this.KillEvent.WaitOne(1000, true))
			{
				StationManagerClass stationManager = this.GetStationManager();
				if (stationManager == null)
					return;

				Monitor.Enter(stationManager);

				try
				{
					if (this.MessageTimer > 0)
					{
						this.MessageTimer--;
						if (this.MessageTimer == 0)
						{
							if (!this.ProcessMessageTimeout(stationManager))
								stationManager.ProcessMessageTimeout();
						}
					}
				}
				catch (Exception e)
				{
					this.eventLog.WriteEntry("MultiloadIILoaddArmManager Scan : " + e.Message);
				}

				Monitor.Exit(stationManager);
			}
		}

		protected virtual void InitializeLoadArmPvs()
		{
			string loadArmUrl = this.LoadArm.ProcessVariableCollection[0].URL;
			string loadArmProgID = this.LoadArm.ProcessVariableCollection[0].ProgID;
			string loadArmOpcItemID = this.LoadArm.ProcessVariableCollection[0].OPCItemID;

			this.LoadArmStatePV = new ProcessVariableClass(
				 PROCESS_VARIABLE_TYPE.LOAD_ARM_STATE_PV,
				 UNIT_TYPE.LOADARM_UNIT,
				 VarEnum.VT_UI2,
				 true,
				 loadArmOpcItemID + ".Preset State", // arm level tag
				 loadArmUrl,
				 loadArmProgID);

			this.OpcServerManager.AddProcessVariable(this.LoadArmStatePV);

			this.RemoteMsgActivePV = new ProcessVariableClass(
				 PROCESS_VARIABLE_TYPE.LOADARM_PV,
				 UNIT_TYPE.LOADARM_UNIT,
				 VarEnum.VT_BOOL,
				 true,
				 loadArmOpcItemID + ".Preset Status.Remote Message", // arm level tag
				 loadArmUrl,
				 loadArmProgID);

			this.OpcServerManager.AddProcessVariable(this.RemoteMsgActivePV);

			this.AuthorizedPV = new ProcessVariableClass(
				 PROCESS_VARIABLE_TYPE.LOADARM_PV,
				 UNIT_TYPE.LOADARM_UNIT,
				 VarEnum.VT_BOOL,
				 true,
				 loadArmOpcItemID + ".Preset Status.Authorized", // arm level tag
				 loadArmUrl,
				 loadArmProgID);

			this.OpcServerManager.AddProcessVariable(this.AuthorizedPV);

			this.HostEnabledPV = new ProcessVariableClass(
				 PROCESS_VARIABLE_TYPE.LOADARM_PV,
				 UNIT_TYPE.LOADARM_UNIT,
				 VarEnum.VT_BOOL,
				 true,
				 loadArmOpcItemID + ".Preset Status.Host Enabled", // arm level tag
				 loadArmUrl,
				 loadArmProgID);

			this.OpcServerManager.AddProcessVariable(this.HostEnabledPV);
		}

		public override bool AllocateRecipes(ulong recipeMap, ulong extendedRecipeMap)
		{
			StationManagerClass stationManager = this.GetStationManager();
			this.Bay(stationManager).RecipeMap = recipeMap;
			this.Bay(stationManager).ExtendedRecipeMap = extendedRecipeMap;
			return true;
		}

		public override void DisplayMenu(DisplayMenuParameters parameters)
		{
			StationManagerClass stationManager = this.GetStationManager();
			if (stationManager == null)
			{
				throw new OpcException("No Station Available");
			}

			stationManager.DisplayMenu(parameters);
		}

		public override int DisplayMessage(string stockMessage, int responseLength, int messageTimeout)
		{
			// all messages must be handled by the station class
			const string DefaultResponse = "None";
			StationManagerClass stationManager = GetStationManager();
			if (stationManager == null)
			{
				throw new OpcException("No Station Available");
			}

			int lineNumber = stationManager.DisplayMessage(stockMessage, DefaultResponse, responseLength, messageTimeout, false);
			return lineNumber;
		}

		protected override void ProcessNonPreloadEquipmentResponse(StationManagerClass stationManager, string response)
		{
			if (response == StationManagerClass.EscapeString)
			{
				this.TerminalCommandAuthorize();
				this.SetState(LOADARM_STATE.PRESET_ENABLED);
				return;
			}

			// Get the value of the response
			int selection = -1;

			if (!string.IsNullOrEmpty(response))
			{
				try
				{
					selection = Convert.ToInt32(response);
				}
				catch
				{
					return;
				}
			}

			// Zero cancels
			if (selection == 0)
			{
				this.TerminalCommandAuthorize();
				this.SetState(LOADARM_STATE.PRESET_ENABLED);
				return;
			}

			// Process the selection
			this.ValidateNonPreloadEquipmentResponse(selection);
		}

		protected override void ProcessEquipmentResponse(StationManagerClass stationManager, string response)
		{
			if (response == StationManagerClass.EscapeString)
			{
				this.TerminalCommandAuthorize();
				this.SetState(LOADARM_STATE.PRESET_ENABLED);
				return;
			}

			// Get the value of the response
			int selection = -1;
			if (!string.IsNullOrEmpty(response))
			{
				try
				{
					selection = Convert.ToInt32(response);
				}
				catch
				{
					return;
				}
			}

			// Zero cancels
			if (selection == 0)
			{
				this.TerminalCommandAuthorize();
				this.SetState(LOADARM_STATE.PRESET_ENABLED);
				return;
			}

			// Check the validity of the selection
			if (selection < 0)
			{
				this.SetState(LOADARM_STATE.INVALID_COMPARTMENT_SELECTION_MSG);
				this.DisplayMessage("[LoadRack|Invalid Selection]", 0);
				return;
			}

			// Process selection
			this.ValidateEquipmentResponse(stationManager, selection);
		}

		protected override void ProcessCompartmentResponse(StationManagerClass stationManager, string response)
		{
			if (response == StationManagerClass.EscapeString)
			{
				if (this.NeedEquipmentPrompt(stationManager))
				{
					this.IssueEquipmentPrompt(stationManager);
				}
				else
				{
					this.TerminalCommandAuthorize();
					this.SetState(LOADARM_STATE.PRESET_ENABLED);
				}

				return;
			}

			// Get the value of the response
			int selection = -1;
			if (!string.IsNullOrEmpty(response))
			{
				try
				{
					selection = Convert.ToInt32(response);
				}
				catch
				{
					this.SetState(LOADARM_STATE.INVALID_COMPARTMENT_SELECTION_MSG);
					this.DisplayMessage("[LoadRack|Invalid Selection]", 0);
					return;
				}
			}

			// Zero cancels
			if (selection == 0)
			{
				this.TerminalCommandAuthorize();
				this.SetState(LOADARM_STATE.PRESET_ENABLED);
				return;
			}

			// Check the validity of the selection
			if (selection < 0)
			{
				this.SetState(LOADARM_STATE.INVALID_COMPARTMENT_SELECTION_MSG);
				this.DisplayMessage("[LoadRack|Invalid Selection]", 0);
				return;
			}

			// Process selection
			this.ValidateCompartmentResponse(stationManager, selection);
		}

		public override bool IssuePresetPrompt(StationManagerClass stationManager, double preset)
		{
			this.MaximumPreset = preset;
			this.SetState(LOADARM_STATE.PRESET_VOLUME_PROMPT);
			this.DisplayMessage("[LoadRack|Enter] " + stationManager.GetLoadRackDisplayText(CurrentRecipe.AssignedGuid) + " [LoadRack|Preset], [LoadRack|Maximum] " + this.MaximumPreset, 10, this.PromptTimeout);
			return true;
		}

		public override void ProcessPresetResponse(StationManagerClass stationManager, string response)
		{
			if (response == StationManagerClass.EscapeString)
			{
				if (stationManager.PreloadInProgress && stationManager.AvailableLoadArms == 1
					 && Bay(stationManager).PreLoads.Count == 1)
				{
					this.TerminalCommandAuthorize();
				}
				else
				{
					if (!this.IssueSelectRecipePrompt(stationManager, this.MaximumPreset))
					{
						if (stationManager.SiteManager.Site.PromptForCompartment)
						{
							this.IssueCompartmentPrompt(stationManager);
						}
						else
						{
							this.IssueSelectPrompt(stationManager);
						}
					}
				}

				return;
			}

			if (string.IsNullOrEmpty(response))
			{
				this.CurrentPreset = stationManager.Station.SetDefaultPresetToZero ? 0 : Convert.ToInt32(this.MaximumPreset);
			}
			else
			{
				try
				{
					this.CurrentPreset = Convert.ToInt32(response);
				}
				catch
				{
					this.IssuePresetInvalidMessage();
					return;
				}
			}

			if (this.CurrentPreset > this.MaximumPreset)
			{
				this.IssuePresetInvalidMessage();
				return;
			}

			this.Authorize(stationManager, this.CurrentPreset);
		}

		public override bool IssueSelectRecipePrompt(StationManagerClass stationManager, double maximumPreset)
		{
			this.MaximumPreset = maximumPreset;

			int numberOfAuthorizedRecipes = 0;
			for (int index = 0; index < AvailableRecipeCollection.Count; index++)
			{
				int recipeNumber = AvailableRecipeCollection[index].PresetNumber;
				if (recipeNumber >= 64)
				{
					if ((Bay(stationManager).ExtendedRecipeMap & ((ulong)1 << ((recipeNumber - 64) - 1))) != 0)
					{
						this.CurrentRecipe = this.AvailableRecipeCollection[index];
						numberOfAuthorizedRecipes++;
					}
				}
				else
				{
					if ((Bay(stationManager).RecipeMap & ((ulong)1 << (recipeNumber - 1))) != 0)
					{
						this.CurrentRecipe = this.AvailableRecipeCollection[index];
						numberOfAuthorizedRecipes++;
					}
				}
			}

			if (numberOfAuthorizedRecipes <= 1)
			{
				return false;
			}

			var menu = new string[numberOfAuthorizedRecipes];

			numberOfAuthorizedRecipes = 0;
			for (int index = 0; index < AvailableRecipeCollection.Count; index++)
			{
				int recipeNumber = AvailableRecipeCollection[index].PresetNumber;
				if (recipeNumber >= 64)
				{
					if ((Bay(stationManager).ExtendedRecipeMap & ((ulong)1 << ((recipeNumber - 64) - 1))) != 0)
					{
						ProductMapClass recipe = AvailableRecipeCollection[index];

						menu[numberOfAuthorizedRecipes] = stationManager.GetLoadRackDisplayText(recipe.AssignedGuid);
						numberOfAuthorizedRecipes++;
					}
				}
				else
				{
					if ((Bay(stationManager).RecipeMap & ((ulong)1 << (recipeNumber - 1))) != 0)
					{
						ProductMapClass recipe = AvailableRecipeCollection[index];

						menu[numberOfAuthorizedRecipes] = stationManager.GetLoadRackDisplayText(recipe.AssignedGuid);
						numberOfAuthorizedRecipes++;
					}
				}
			}

			var parameters = new DisplayMenuParameters("LoadRack|Select Recipe", menu, false, -1, PromptTimeout);

			this.SetState(LOADARM_STATE.SELECT_RECIPE_PROMPT);

			this.CurrentMenuParameters = parameters;

			this.DisplayMenu(parameters);

			return true;
		}

		protected override void ProcessSelectRecipeResponse(StationManagerClass stationManager, string response)
		{
			int selection;

			try
			{
				selection = Convert.ToInt32(response);
			}
			catch (FormatException)
			{
				selection = 0;
			}
			catch (OverflowException)
			{
				selection = 0;
			}

			if (response == StationManagerClass.EscapeString || selection == 0)
			{
				if (stationManager.PreloadInProgress && stationManager.AvailableLoadArms == 1
					 && Bay(stationManager).PreLoads.Count == 1)
				{
					this.TerminalCommandAuthorize();
				}
				else if (stationManager.SiteManager.Site.PromptForCompartment)
				{
					this.IssueCompartmentPrompt(stationManager);
				}
				else
				{
					this.IssueSelectRecipePrompt(stationManager, this.MaximumPreset);
				}

				return;
			}

			if (this.CurrentMenuParameters == null
			|| selection > CurrentMenuParameters.Menu.Length)
			{
				this.IssueRecipeInvalidMessage();
				return;
			}

			string displayProduct = this.CurrentMenuParameters.Menu[selection - 1];
			string productId = string.Empty;
			foreach (ProductMapClass availableRecipe in this.AvailableRecipeCollection)
			{
				if (displayProduct == StationManagerClass.GetLoadRackDisplayText(availableRecipe))
				{
					productId = availableRecipe.AssignedID;
				}
			}

			if (string.IsNullOrEmpty(productId))
			{
				this.IssueRecipeInvalidMessage();
				return;
			}

			base.ProcessSelectRecipeResponse(stationManager, productId);
		}

		protected override void ProcessSplashBlendComponentPromptResponse(StationManagerClass stationManager, string response)
		{
			int selection = Convert.ToInt32(response);

			if (this.CurrentMenuParameters == null
				|| selection > CurrentMenuParameters.Menu.Length)
			{
				this.IssueSplashBlendComponentInvalidMessage();
				return;
			}

			if (selection == 0)
			{
				response = StationManagerClass.EscapeString;
			}
			else
			{
				response = this.CurrentMenuParameters.Menu[selection - 1];
			}

			base.ProcessSplashBlendComponentPromptResponse(stationManager, response);
		}

		public override void ReadBatchRecipe(
			string batchNumber,
			Opc.Da.Server server,
			out ItemValueResult recipe)
		{
			var value = new ItemValueResult();

			if (this.CurrentRecipe == null)
			{
				value.Quality = Quality.Bad;
			}
			else
			{
				value.Quality = Quality.Good;
				value.Value = CurrentRecipe.PresetNumber;
			}

			recipe = value;
		}

		public override bool IsTransactionInProgress()
		{
			return false;
		}

		public void TerminalCommandAuthorize()
		{
			var terminalCommand = new ItemValue(this.StationPV.OPCItemID + ".Terminal Command") { Value = Esc + "A" };
			OpcServerManager.Write(new URL(this.StationPV.URL), new[] { terminalCommand });
		}

		public override void SetFinishedLoading()
		{
			try
			{
				this.DoFinishedLoadingProcessing();

				this.SetState(LOADARM_STATE.FINISHED);

				StationManagerClass stationManager = this.GetStationManager();
				if (stationManager == null)
				{
					return;
				}

				stationManager.EvaluateLoadArmStatus();

				this.UpdatePermissives(stationManager, this.CurrentRecipe, false);

				if (stationManager.StationState != StationState.IDLE
					 && stationManager.StationState != StationState.BROKEN_BLEND
					 && stationManager.StationState != StationState.IMPROPER_ADDITIZATION
					 && stationManager.StationState != StationState.ENTER_DRIVER_ID_PROMPT)
				{
					if (stationManager.IsRemoteAuthorized
						 || stationManager.TransactionSupportsMultipleLineItems == false
						 || stationManager.bInRecircMode)
					{
						// Allow the card pull to end the transaction.  Going to IssueSelectPrompt will start infinite recursion.
						return;
					}

					OpcServerManager.Read(this.RcuStatusPV);
					if (this.RcuStatusPV.IsQualityGood
					&& ('D' == Convert.ToChar(this.RcuStatusPV.ServerValue)
						 || 'C' == Convert.ToChar(this.RcuStatusPV.ServerValue)))
					{
						// Card was pulled or transaction was otherwise ended at the device by driver action
						// we're on the way out.
						return;
					}

					this.IssueSelectPrompt(stationManager);
				}
			}
			catch (Exception e)
			{
				this.eventLog.WriteEntry("LoadArmManager SetFinishedLoading : " + e.Message + "\n" + e.StackTrace, EventLogEntryType.Error);
				this.SetState(LOADARM_STATE.SELECT_PROMPT);
				this.DisplayMessage("LoadRack|Finished Loading Error", 0);
			}
		}

		public override void IssueSelectPrompt(StationManagerClass stationManager)
		{
			// If we were authorized by remote control, do not prompt for batch complete
			stationManager.UpdatePermissives(false);
			this.UpdatePermissives(stationManager, this.CurrentRecipe, false);

			if (stationManager.IsRemoteAuthorized
				 || stationManager.TransactionSupportsMultipleLineItems == false
				 || stationManager.bInRecircMode)
			{
				this.SetFinishedLoading();
				return;
			}

			// For the Multiload II, just return to the Arm Select screen
			this.NonPreloadEquipmentSelection = string.Empty;
			this.NonPreloadCompartmentSelection = -1;

			this.TerminalCommandAuthorize();
			this.SetState(LOADARM_STATE.PRESET_ENABLED);
		}

		public override void IssueBatchCompletePrompt()
		{
			StationManagerClass stationManager = this.GetStationManager();
			if (stationManager == null)
			{
				return;
			}

			// If we were authorized by remote control, do not prompt for batch complete
			stationManager.UpdatePermissives(false);

			if (stationManager.IsRemoteAuthorized
				 || stationManager.TransactionSupportsMultipleLineItems == false
				 || stationManager.bInRecircMode)
			{
				this.SetFinishedLoading();
				return;
			}

			// For the Multiload II, just return to the Arm Select screen
			this.NonPreloadEquipmentSelection = string.Empty;
			this.NonPreloadCompartmentSelection = -1;

			// For loading, we allow another batch.
			// for offloading, we only allow a single offload at a time, so we need to end the transaction
			if (stationManager.Station.Type == STATION_TYPE.LOAD_RACK)
			{
				this.TerminalCommandAuthorize();
			}
			else
			{
				this.SendEndTransaction();
			}
			this.SetState(LOADARM_STATE.PRESET_ENABLED);
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

			OpcServerManager.Read(this.RcuStatusPV); // Force a status read to make sure we have the latest
			char rcuStatus = Convert.ToChar(this.RcuStatusPV.ServerValue);
			var terminalCommand = new ItemValue(this.StationPV.OPCItemID + ".Terminal Command") { Value = Esc + "A" };

			if (!string.IsNullOrEmpty(this.GetPermissiveMessage(stationManager)))
			{
				// arm should be authorized, just blocked by a permissive (which may be resolved)
				// Don't send a terminal authorize if we're already authorized (status 'T')
				if (rcuStatus != 'T')
				{
					OpcServerManager.Write(new URL(this.StationPV.URL), new[] { terminalCommand });
				}
				this.IssuePermissiveMessage(stationManager);

				return true;
			}

			// Don't send a terminal authorize if we're already authorized (status 'T')
			if (rcuStatus != 'T')
			{
				OpcServerManager.Write(new URL(this.StationPV.URL), new[] { terminalCommand });
			}

			ProcessVariableClass loadArmPv = LoadArm.ProcessVariableCollection[0];
			var enablePreset = new ItemValue(loadArmPv.OPCItemID + ".Enable Preset") { Value = "2" };

			OpcServerManager.Write(new URL(loadArmPv.URL), new[] { enablePreset });

			return true;
		}

		protected virtual void DisablePreset()
		{
			ProcessVariableClass loadArmPv = LoadArm.ProcessVariableCollection[0];
			var enablePreset = new ItemValue(loadArmPv.OPCItemID + ".Enable Preset") { Value = "0" };
			OpcServerManager.Write(new URL(loadArmPv.URL), new[] { enablePreset });
		}

		public override bool Authorize(StationManagerClass stationManager, double preset)
		{
			ProcessVariableClass loadArmPv = LoadArm.ProcessVariableCollection[0];
			var authorizePreset = new ItemValue(loadArmPv.OPCItemID + ".Authorize Preset");

			// authorize the preset
			var multiloadStationManager = (MultiloadIIStationManagerClass)GetStationManager();
			if (multiloadStationManager == null)
			{
				throw new OpcException("No Station Available");
			}

			this.SetState(LOADARM_STATE.AUTHORIZED);

			this.IssuePermissiveMessage(multiloadStationManager);
			bool passedPermissives = this.LoadArmState == LOADARM_STATE.AUTHORIZED;

			authorizePreset.Value = passedPermissives ? "1" : "0";   // authorize bit
			authorizePreset.Value += this.CurrentRecipe.PresetNumber.ToString("D3", CultureInfo.InvariantCulture); // selected product number
			authorizePreset.Value += Convert.ToInt32(preset).ToString("D9");  // preset volume
			authorizePreset.Value += "01";   // compartment number

			OpcServerManager.Write(new URL(loadArmPv.URL), new[] { authorizePreset });

			this.UpdatePermissives(stationManager, this.CurrentRecipe, passedPermissives);

			return true;
		}

		protected override void DoFinishedLoadingProcessing(StationManagerClass stationManager, bool transactionComplete, LOADARM_STATE state)
		{
			this.ReleaseKeyPad();
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
					var armUnavailable = new ItemValue(loadArmPv.OPCItemID + ".Enable Preset") { Value = "1" };

					string permissiveHeader = stationManager.GetDataDictionaryValueByKey(SiteManager.Site.SiteGuid, "[LoadRack|Permissive]");
					var presetMessage = new ItemValue(loadArmPv.OPCItemID + ".Preset Message")
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
					//// loadArmManager.EndBatch();
					loadArmManager.IssueSelectPrompt(stationManager);
				}
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
									char rcuStatus = Convert.ToChar(pv.ServerValue);
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
										case 'a':
											{
												string tempArmName = LoadArm.ProcessVariableCollection[0].OPCItemID;
												if (tempArmName.IndexOf("Arm 1", StringComparison.Ordinal) > 0
													 && tempArmName.IndexOf("Arm 10", StringComparison.Ordinal) == -1
													 && tempArmName.IndexOf("Arm 11", StringComparison.Ordinal) == -1)
												{
													this.PromptForNextBatch(stationManager, true);
												}

												break;
											}

										// Preset 2 Preset Request
										case 'b':
											{
												string tempArmName = LoadArm.ProcessVariableCollection[0].OPCItemID;
												if (tempArmName.IndexOf("Arm 2", StringComparison.Ordinal) > 0)
												{
													this.PromptForNextBatch(stationManager, true);
												}

												break;
											}

										// Preset 3 Preset Request
										case 'c':
											{
												string tempArmName = LoadArm.ProcessVariableCollection[0].OPCItemID;
												if (tempArmName.IndexOf("Arm 3", StringComparison.Ordinal) > 0)
												{
													this.PromptForNextBatch(stationManager, true);
												}

												break;
											}

										// Preset 4 Preset Request
										case 'd':
											{
												string tempArmName = LoadArm.ProcessVariableCollection[0].OPCItemID;
												if (tempArmName.IndexOf("Arm 4", StringComparison.Ordinal) > 0)
												{
													this.PromptForNextBatch(stationManager, true);
												}

												break;
											}

										// Preset 5 Preset Request
										case 'e':
											{
												string tempArmName = LoadArm.ProcessVariableCollection[0].OPCItemID;
												if (tempArmName.IndexOf("Arm 5", StringComparison.Ordinal) > 0)
												{
													this.PromptForNextBatch(stationManager, true);
												}

												break;
											}

										// Preset 6 Preset Request
										case 'f':
											{
												string tempArmName = LoadArm.ProcessVariableCollection[0].OPCItemID;
												if (tempArmName.IndexOf("Arm 6", StringComparison.Ordinal) > 0)
												{
													this.PromptForNextBatch(stationManager, true);
												}

												break;
											}

										// Preset 7 Preset Request
										case 'g':
											{
												string tempArmName = LoadArm.ProcessVariableCollection[0].OPCItemID;
												if (tempArmName.IndexOf("Arm 7", StringComparison.Ordinal) > 0)
												{
													this.PromptForNextBatch(stationManager, true);
												}

												break;
											}

										// Preset 8 Preset Request
										case 'h':
											{
												string tempArmName = LoadArm.ProcessVariableCollection[0].OPCItemID;
												if (tempArmName.IndexOf("Arm 8", StringComparison.Ordinal) > 0)
												{
													this.PromptForNextBatch(stationManager, true);
												}

												break;
											}

										// Preset 9 Preset Request
										case 'i':
											{
												string tempArmName = LoadArm.ProcessVariableCollection[0].OPCItemID;
												if (tempArmName.IndexOf("Arm 9", StringComparison.Ordinal) > 0)
												{
													this.PromptForNextBatch(stationManager, true);
												}

												break;
											}

										// Preset 10 Preset Request
										case 'j':
											{
												string tempArmName = LoadArm.ProcessVariableCollection[0].OPCItemID;
												if (tempArmName.IndexOf("Arm 10", StringComparison.Ordinal) > 0)
												{
													this.PromptForNextBatch(stationManager, true);
												}

												break;
											}

										// Preset 11 Preset Request
										case 'k':
											{
												string tempArmName = LoadArm.ProcessVariableCollection[0].OPCItemID;
												if (tempArmName.IndexOf("Arm 11", StringComparison.Ordinal) > 0)
												{
													this.PromptForNextBatch(stationManager, true);
												}

												break;
											}

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
									var enablePreset = new ItemValue(loadArmPv.OPCItemID + ".Enable Preset") { Value = "0" };
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
												LineItemDO lineItem = stationManager.GetLineItem(this.LoadArm.IdentityGuid);

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
				this.eventLog.WriteEntry("Multiload II LoadArmManager OnInvoke : " + e.Message, EventLogEntryType.Error);
				this.CommunicationsFailure = true;
			}
			catch (Exception e)
			{
				this.eventLog.WriteEntry("Multiload II LoadArmManager OnInvoke : PV = " + pv.OPCItemID + " " + e + "\n" + e.StackTrace, EventLogEntryType.Error);
			}
			finally
			{
				Monitor.Exit(stationManager);
			}
		}

		protected virtual void BatchComplete()
		{
			ProcessVariableClass loadArmPv = LoadArm.ProcessVariableCollection[0];
			var batchComplete = new ItemValue(loadArmPv.OPCItemID + ".Batch Complete");
			OpcServerManager.Write(new URL(loadArmPv.URL), new[] { batchComplete });

			this.UpdatePermissives(this.GetStationManager(), this.CurrentRecipe, false);
		}

		protected virtual void BatchEnd()
		{
			ProcessVariableClass loadArmPv = LoadArm.ProcessVariableCollection[0];
			var batchEnd = new ItemValue(loadArmPv.OPCItemID + ".Batch End");
			OpcServerManager.Write(new URL(loadArmPv.URL), new[] { batchEnd });

			this.UpdatePermissives(this.GetStationManager(), this.CurrentRecipe, false);
		}

		public override void SendEndTransaction()
		{
			this.EndTransaction();
		}

		public override void EndBatch()
		{
			ProcessVariableClass loadArmPv = LoadArm.ProcessVariableCollection[0];
			var endBatch = new ItemValue(loadArmPv.OPCItemID + ".End Batch");
			OpcServerManager.Write(new URL(loadArmPv.URL), new[] { endBatch });

			if ((this.GetStationManager()?.Station.Type ?? STATION_TYPE.MAX_STATION_TYPE) != STATION_TYPE.LOAD_RACK)
			{
				// For offload stations, we should end the transaction once we end the batch.
				// If GetStationManager() returned null, then something BAD happened, and we should also end the transaction.
				// Load rack station is the only other reasonable possibility; in that case we allow the transaction to continue.
				this.SendEndTransaction();
			}
		}

		protected override void ProcessEndBatchPromptResponse(StationManagerClass stationManager, string response)
		{
			if (response == "1")
			{
				try
				{
					this.ReleaseKeyPad();
					this.BatchComplete();
					return;
				}
				catch (Exception e)
				{
					this.eventLog.WriteEntry("LoadArmManager : ProcessEndBatchPromptResonse " + e.Message, EventLogEntryType.Error);
				}
			}

			this.ReleaseKeyPad();
			this.SetState(LOADARM_STATE.INPROGRESS);
		}

		public void EndTransaction()
		{
			OpcServerManager.Read(this.RcuStatusPV);
			if (this.RcuStatusPV.IsQualityGood
			&& 'D' != Convert.ToChar(this.RcuStatusPV.ServerValue)
				&& 'C' != Convert.ToChar(this.RcuStatusPV.ServerValue))
			{
				var endTransaction = new ItemValue(this.StationPV.OPCItemID + ".End Transaction");
				OpcServerManager.Write(new URL(this.StationPV.URL), new[] { endTransaction });
			}
		}

		public override void ReleaseKeyPad()
		{
			try
			{
				var terminalCommand = new ItemValue(this.StationPV.OPCItemID + ".Terminal Command") { Value = Esc + "R" };
				OpcServerManager.Write(new URL(this.StationPV.URL), new[] { terminalCommand });
			}
			catch (Exception e)
			{
				this.eventLog.WriteEntry(e.Message, EventLogEntryType.Error);
			}
		}

		public override bool ProcessMessageTimeout(StationManagerClass stationManager)
		{
			switch (this.LoadArmState)
			{
				case LOADARM_STATE.COMPARTMENT_ALREADY_LOADED_MSG:
				case LOADARM_STATE.NO_PRODUCTS_TO_LOAD:
				case LOADARM_STATE.NO_EQUIPMENT_TO_LOAD_MSG:
				case LOADARM_STATE.NO_COMPARTMENTS_TO_LOAD_MSG:
				case LOADARM_STATE.MAXIMUM_PRESET_LESS_THAN_OR_EQUAL_ZERO:
				case LOADARM_STATE.PRESET_ENABLED:
					this.TerminalCommandAuthorize();
					return true;
				default:
					return base.ProcessMessageTimeout(stationManager);
			}
		}

		public override void Start()
		{
			ProcessVariableClass loadArmPv = LoadArm.ProcessVariableCollection[0];
			ItemValue[] itemValues = { new ItemValue(new ItemIdentifier(loadArmPv.OPCItemID + ".Start")) };
			OpcServerManager.Write(new URL(loadArmPv.URL), itemValues);
		}

		public override void Stop()
		{
			ProcessVariableClass loadArmPv = LoadArm.ProcessVariableCollection[0];
			ItemValue[] itemValues = { new ItemValue(new ItemIdentifier(loadArmPv.OPCItemID + ".Stop")) };
			OpcServerManager.Write(new URL(loadArmPv.URL), itemValues);
		}

		public override void Unauthorize()
		{
			StationManagerClass stationManager = GetStationManager();
			if (stationManager == null)
			{
				// TODO: Check if we need to do something here
				return;
			}

			this.UpdatePermissives(this.GetStationManager(), this.CurrentRecipe, false);
		}

        /// <summary>
        /// Applies the additive profile to the preset device and updates the display name of the recipe
		/// 
		/// Does nothing as the additive configuration gets set elsewhere
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
        /// true
        /// </returns>
        public override bool UpdateRecipe(
			string name,
			ProductMapClass recipe,
			ProductClass product,
			AdditiveProfileClass additiveProfile,
			int deviceRecipeNumber)
		{
			return true;
		}

		public override void ReadPresetAmount(
			Opc.Da.Server server,
			out ItemValueResult presetAmount)
		{
			presetAmount = new ItemValueResult { Value = Convert.ToDouble(this.CurrentPreset), Quality = Quality.Good };
		}

		public override void CaptureMeterValues()
		{
			ProcessVariableClass loadArmPv = LoadArm.ProcessVariableCollection[0];

			var server = new Opc.Da.Server(new OpcCom.Factory(), new URL(loadArmPv.URL));
			server.Connect(new ConnectData(null));

			ItemValueResult[] nonResettableTotal;

			this.ReadNonResettableTotals(
				server,
				out nonResettableTotal);

			// Component
			if (nonResettableTotal[0].Quality != Quality.Good)
			{
				this.eventLog.WriteEntry(
					 "CaptureMeterValues : Non-Resettable Gross Volume OPC Quality Bad " + nonResettableTotal[0].ItemName,
					 EventLogEntryType.Error);
			}
			else
			{
				for (int index = 0; index < this.LoadArm.ComponentCollection.Count; index++)
				{
					this.LoadArm.ComponentCollection[index].MeterValue = Convert.ToDouble(nonResettableTotal[index].Value);
				}
			}

			// Additives
			for (int item = 0; item < this.LoadArm.AdditiveInjectorCollection.Count; item++)
			{
				if (nonResettableTotal[item + this.LoadArm.ComponentCollection.Count].Quality != Quality.Good)
				{
					this.eventLog.WriteEntry(
						 "CaptureMeterValues : Non-Resettable Gross Volume OPC Quality Bad " + nonResettableTotal[item].ItemName,
						 EventLogEntryType.Error);
				}
				else
				{
					this.LoadArm.AdditiveInjectorCollection[item].MeterValue = Convert.ToDouble(nonResettableTotal[item + this.LoadArm.ComponentCollection.Count].Value);
				}
			}

			server.Disconnect();
			server.Dispose();
		}

		public virtual void ReadNonResettableTotals(
			Opc.Da.Server server,
			out ItemValueResult[] nonResettableGrossVolumes)
		{
			ProcessVariableClass loadArmPv = LoadArm.ProcessVariableCollection[0];

			var items = new ArrayList();

			string tagPrefix = loadArmPv.OPCItemID + ".Gross Totalizer.Component.";

			for (int index = 0; index < this.LoadArm.ComponentCollection.Count; index++)
			{
				items.Add(new Item(new ItemIdentifier(tagPrefix + (index + 1).ToString(CultureInfo.InvariantCulture))));
			}

			tagPrefix = loadArmPv.OPCItemID + ".Gross Totalizer.Additive.";

			for (int additive = 0; additive < this.LoadArm.AdditiveInjectorCollection.Count; additive++)
			{
				items.Add(new Item(new ItemIdentifier(tagPrefix + (additive + 1).ToString(CultureInfo.InvariantCulture))));
			}

			foreach (Item item in items)
			{
				item.MaxAgeSpecified = true;
			}

			((Item)items[0]).MaxAgeSpecified = false;

			ItemValueResult[] values = server.Read((Item[])items.ToArray(typeof(Item)));

			nonResettableGrossVolumes = values;
		}

		public virtual void ReadComponentData(
			Opc.Da.Server server,
			ProductMapClass component,
			out ItemValueResult grossVolume,
			out ItemValueResult netVolume,
			out ItemValueResult averageTemperature,
			out ItemValueResult averageDensity)
		{
			ProcessVariableClass loadArmPv = LoadArm.ProcessVariableCollection[0];

			string tagPrefix = loadArmPv.OPCItemID + ".Batch.";

			int position = 0;
			bool found = false;

			// Need to find the component's out of those defined on the arm
			foreach (ProductMapClass armComponent in this.LoadArm.ComponentCollection)
			{
				position++;
				if (armComponent.AssignedGuid == component.AssignedGuid)
				{
					found = true;
					break;
				}
			}

			if (found)
			{
				var items = new ArrayList
										{
											 new Item(new ItemIdentifier(tagPrefix + "Gross Volume." + position.ToString(CultureInfo.InvariantCulture))),
											 new Item(new ItemIdentifier(tagPrefix + "Net Volume." + position.ToString(CultureInfo.InvariantCulture))),
											 new Item(new ItemIdentifier(tagPrefix + "Average Temperature." + position.ToString(CultureInfo.InvariantCulture))),
											 new Item(new ItemIdentifier(tagPrefix + "Average Density." + position.ToString(CultureInfo.InvariantCulture)))
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
			else
			{
				grossVolume = new ItemValueResult(new ItemValue { Quality = Quality.Bad, Value = 0.0 });
				netVolume = new ItemValueResult(new ItemValue { Quality = Quality.Bad, Value = 0.0 });
				averageTemperature = new ItemValueResult(new ItemValue { Quality = Quality.Bad, Value = 0.0 });
				averageDensity = new ItemValueResult(new ItemValue { Quality = Quality.Bad, Value = 0.0 });
			}
		}

		public virtual void ReadAdditiveData(
			 Opc.Da.Server server,
			 ProductMapClass additive,
			 out ItemValueResult grossVolume)
		{
			ProcessVariableClass loadArmPv = this.LoadArm.ProcessVariableCollection[0];

			string tagPrefix = loadArmPv.OPCItemID + ".Batch.Additive.";

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

		public override bool UpdateReferenceDensity(StationManagerClass stationManager)
		{
			var itemValues = new ArrayList();

			for (int item = 0; item < this.LoadArm.ComponentCollection.Count; item++)
			{
				ProductMapClass component = LoadArm.ComponentCollection[item];
				TankClass tank = SiteManager.GetTank(component, stationManager.Manager);
				if (tank == null)
				{
					// Tank Group may not have a market tank in which case no recipes will be enabled
					// for the product.
					return component.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP;
				}

				ProcessVariableClass densityPv =
						  tank.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.STANDARD_DENSITY_PV];
				if (densityPv == null || (!densityPv.IsQualityGood && !SiteManager.Site.UseLastKnownGoodTankData))
				{
					return false;
				}

				// Multiload specifically wants API gravity and one decimal place for register 103043
				// CU_UNIT units = (stationManager.CurrentTransactionAlias.DensityUnits != 0)
				//                    ? stationManager.CurrentTransactionAlias.DensityUnits
				//                    : stationManager.SiteManager.Site.DensityUnits;
				// byte decimalPlaces = (stationManager.CurrentTransactionAlias.DensityUnits != 0)
				//                         ? stationManager.CurrentTransactionAlias._DensityDecimalPlaces
				//                         : stationManager.SiteManager.Site._DensityDecimalPlaces;
				const EngineeringUnit Units = EngineeringUnit.FmdDegApi;
				const byte DecimalPlaces = 1;
				const double Scaling = 10;

				double density;
				try
				{
					density = Convert.ToDouble(densityPv.GetValue(Units, DecimalPlaces));
				}
				catch
				{
					return false;
				}

				density *= Scaling;

				var writeDensityItem = new ItemValue(this.StationPV.OPCItemID + ".Write Register")
				{
					Value =
															  "103043"
															  + (this.GetArmNumber(stationManager) - 1).ToString("D3", CultureInfo.InvariantCulture)
															  + item.ToString("D3", CultureInfo.InvariantCulture)
															  + Convert.ToInt32(density).ToString(CultureInfo.InvariantCulture)
				};

				itemValues.Add(writeDensityItem);

				try
				{
					OpcServerManager.Write(
						 new URL(this.StationPV.URL),
						 (ItemValue[])itemValues.ToArray(typeof(ItemValue)));
				}
				catch (Exception e)
				{
					this.eventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
					return false;
				}
			}

			return true;
		}

		public bool LoadArmProcessResponseData(StationManagerClass stationManager, string keypadData)
		{
			return this.ProcessResponseData(stationManager, keypadData);
		}

		public virtual bool CheckLoadArmAuthorizedProducts()
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
			string opcItemId = LoadArm.ProcessVariableCollection[0].OPCItemID + ".Arm Product Configuration";

			var multiloadStationManager = (MultiloadIIStationManagerClass)GetStationManager();
			if (multiloadStationManager == null)
			{
				throw new OpcException("No Station Available");
			}

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

			string configurationSection = configurationStringBuilder.ToString();

			if (string.CompareOrdinal(configurationSection, 0, deviceProductDefinition, 10, totalEntries * 3) != 0)
			{
				// not a match
				var logger = new Logger(StopWatch.Appnames.LoadRackService.ToString());
				logger.Debug("Arm Configuration mismatch!!!");
				logger.Debug("Expected configuration: " + configurationSection.Substring(0, totalEntries * 3));

				logger.Debug("Actual configuration:   " + deviceProductDefinition.Substring(10, totalEntries * 3));
				return false;
			}

			return true;
		}

		public int GetComponentPosition(ProductMapClass component)
		{
			for (int position = 0; position < this.LoadArm.ComponentCollection.Count; position++)
			{
				if (component.AssignedGuid == this.LoadArm.ComponentCollection[position].AssignedGuid)
				{
					return position;
				}
			}

			return -1;
		}

		public int GetAdditivePosition(ProductMapClass additive)
		{
			for (int position = 0; position < this.LoadArm.AdditiveInjectorCollection.Count; position++)
			{
				if (additive.AssignedGuid == this.LoadArm.AdditiveInjectorCollection[position].AssignedGuid)
				{
					return position;
				}
			}

			return -1;
		}

		protected override void IssueNoProductsToLoadMessage(bool finished)
		{
			/*
			var dataDictionaries = new DataDictionariesClass();
			string header = dataDictionaries.Get(SiteManager.Site.SiteIndex, "LoadRack|No Products");
			string message = dataDictionaries.Get(SiteManager.Site.SiteIndex, "LoadRack|No Products to Load on Arm");

			ProcessVariableClass loadArmPv = LoadArm.ProcessVariableCollection[0];
			var presetMessage = new ItemValue(loadArmPv.OPCItemID + ".Preset Message")
			{
				 Value = header.PadRight(10).Substring(0, 10) + message.PadRight(39).Substring(0, 39)
			};
			OpcServerManager.Write(new URL(loadArmPv.URL), new[] { presetMessage });
			 */

			// Do nothing; arm should show Not Auth
		}

		public virtual bool IsStatusForThisLoadArm(char rcuStatus)
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
			}

			// get device arm number for this LoadArmManager
			string loadArmOpcItemID = this.LoadArm.ProcessVariableCollection[0].OPCItemID;
			int armLeafIndex = loadArmOpcItemID.LastIndexOf(".Arm ", StringComparison.Ordinal);

			if (armLeafIndex == -1)
			{
				return false;
			}

			int armNumber;
			try
			{
				armNumber = Convert.ToInt32(loadArmOpcItemID.Substring(armLeafIndex + ".Arm ".Length));
			}
			catch (FormatException)
			{
				return false;
			}
			catch (OverflowException)
			{
				return false;
			}

			char presetCode = Convert.ToChar(0x60 + armNumber);
			return presetCode == rcuStatus;
		}
	}
}
