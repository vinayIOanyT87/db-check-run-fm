/******************************************************************************

	FILE NAME:		Contrec1010LoadArmManagerClass.cs


	PURPOSE:			Contrec1010LoadArmManagerClass


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


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
    using System.Net;
    using System.Runtime.InteropServices;
    using System.Threading;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;

    using Opc;
    using Opc.Da;

    using Convert = System.Convert;
    using Factory = OpcCom.Factory;
    using Server = Opc.Da.Server;

    public class Contrec1010LoadArmManagerClass : LoadArmManagerClass
	{
		protected const int NUMBER_OF_RETRIES = 1;
		protected ProcessVariableClass BatchInProgressPV;
		protected ProcessVariableClass CommandFieldPV;
		protected ProcessVariableClass MessageTimeoutPV;
		protected ProcessVariableClass ContrecStatusPV;
		protected ProcessVariableClass Arm1Status;
		protected ProcessVariableClass Arm2Status;
		protected ProcessVariableClass Arm3Status;
		protected ProcessVariableClass Arm4Status;
		protected int SelectedRecipe;
		protected double SelectedPresetQuantity;
		protected int iLoadArmInUse;
		protected bool bLoadArm1InUse = false;
		protected bool bLoadArm2InUse = false;
		protected bool bLoadArm3InUse = false;
		protected bool bLoadArm4InUse = false;
		protected int iContrecTransActionNumber;
		protected int SelectedCompartment;
		protected bool bInitialized;
		protected double OffLoadDensity = 0.0;
		protected bool EmergencyStopPressed = false;

		public Contrec1010LoadArmManagerClass(
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

			this.CommandFieldPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.LOAD_ARM_STATE_PV,
				UNIT_TYPE.LOADARM_UNIT,
				VarEnum.VT_BSTR,
				true,
				LoadArmOPCItemID + ".Command Field",
				LoadArmURL,
				LoadArmProgID);

			this.MessageTimeoutPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.PROMPT_TIMEOUT_PV,
				UNIT_TYPE.LOADARM_UNIT,
				VarEnum.VT_BOOL,
				true,
				LoadArmOPCItemID + ".Display Message Time-out",
				LoadArmURL,
				LoadArmProgID);

			this.ContrecStatusPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.TRANSACTION_IN_PROGRESS_PV,
				UNIT_TYPE.LOADARM_UNIT,
				VarEnum.VT_I4,
				true,
				LoadArmOPCItemID + ".Status.1010 System Status",
				LoadArmURL,
				LoadArmProgID);

			this.BatchInProgressPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.BATCH_IN_PROGRESS_PV,
				UNIT_TYPE.STATION_UNIT,
				VarEnum.VT_I4,
				true,
				LoadArmOPCItemID + ".Status.Batch In Progress",
				LoadArmURL,
				LoadArmProgID);

			this.Arm1Status = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.LEVEL_PV,
				UNIT_TYPE.LOADARM_UNIT,
				VarEnum.VT_BSTR,
				true,
				LoadArmOPCItemID + ".Arms.Arm 1.Arm Status",
				LoadArmURL,
				LoadArmProgID);

			this.Arm2Status = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.TEMPERATURE_PV,
				UNIT_TYPE.LOADARM_UNIT,
				VarEnum.VT_BSTR,
				true,
				LoadArmOPCItemID + ".Arms.Arm 2.Arm Status",
				LoadArmURL,
				LoadArmProgID);

			this.Arm3Status = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.GROSS_VOLUME_PV,
				UNIT_TYPE.LOADARM_UNIT,
				VarEnum.VT_BSTR,
				true,
				LoadArmOPCItemID + ".Arms.Arm 3.Arm Status",
				LoadArmURL,
				LoadArmProgID);

			this.Arm4Status = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.NET_VOLUME_PV,
				UNIT_TYPE.LOADARM_UNIT,
				VarEnum.VT_BSTR,
				true,
				LoadArmOPCItemID + ".Arms.Arm 4.Arm Status",
				LoadArmURL,
				LoadArmProgID);


			this.OpcServerManager.AddProcessVariable(this.CommandFieldPV);
			this.OpcServerManager.AddProcessVariable(this.MessageTimeoutPV);
			this.OpcServerManager.AddProcessVariable(this.ContrecStatusPV);
			this.OpcServerManager.AddProcessVariable(this.BatchInProgressPV);

			this.OpcServerManager.AddProcessVariable(this.Arm1Status);
			this.OpcServerManager.AddProcessVariable(this.Arm2Status);
			this.OpcServerManager.AddProcessVariable(this.Arm3Status);
			this.OpcServerManager.AddProcessVariable(this.Arm4Status);
			this.iContrecTransActionNumber = 1;
			this.iLoadArmInUse = 1;
			this.SelectedCompartment = 0;
			this.bInitialized = false;
			this.EmergencyStopPressed = false;
		}

		public override void Start()
		{
		}

		public override void Stop()
		{
			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			this.SendEndTransaction();

			Thread.Sleep(5000);

			ProcessVariableClass ClearDisplay = new ProcessVariableClass();
			ClearDisplay.URL = LoadArmPV.URL;
			ClearDisplay.OPCItemID = LoadArmPV.OPCItemID + ".Clear Display";
			ClearDisplay.ServerValue = true;
			this.OpcServerManager.Write(ClearDisplay);

			this.ClearContrecDisplay(true, true);
			Thread.Sleep(10000);
		}

		public override bool AllocateRecipes(ulong recipeMap)
		{
			return true;
		}

		public override void Unauthorize()
		{
			StationManagerClass StationManager = this.GetStationManager();
			if (StationManager == null)
				return;

			if (this.LoadArmState == LOADARM_STATE.INPROGRESS)
				this.Stop();

			this.SetState(LOADARM_STATE.NORMAL);
			this.CurrentLineItem = null;
		}

        /// <summary>
        /// Applies the additive profile to the preset device and updates the display name of the recipe
		/// 
		/// Does nothing as additives on the Contrec 1010 are not currently supported
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
			int deviceRecipe)
		{
			return true;
		}

		public override int DisplayMessage(string message, int responseLength, int messageTimeout)
		{
			StationManagerClass StationManager = this.GetStationManager();
			if (StationManager == null)
				throw new OpcException("No Station Available");

			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			ArrayList Items = new ArrayList();

			this.SetMessageTimeout(messageTimeout);

			// use the get answer prompt to get the response
			string Prompt = "";
			Prompt = this.FormatContrecPrompt(message, true);
			ItemValue DisplayMessageItemValue = new ItemValue(LoadArmPV.OPCItemID + ".Get Answer Prompt");
			DisplayMessageItemValue.Value = Prompt;
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

			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			this.SetMessageTimeout(MessageTimeout);

			string Prompt = "";
			Prompt = this.FormatContrecPrompt(StockMessage, true);

			ArrayList Items = new ArrayList();

			ItemValue DisplayMessageItemValue = new ItemValue(LoadArmPV.OPCItemID + ".Hidden Message Prompt");
			DisplayMessageItemValue.Value = Prompt;
			Items.Add(DisplayMessageItemValue);

			this.OpcServerManager.Write(new URL(LoadArmPV.URL), (ItemValue[])Items.ToArray(typeof(ItemValue)));

			this.ResponsePending = true;

		}

		public void ClearContrecDisplay(bool bSetInitialMessage, bool bSetToWait)
		{
			StationManagerClass StationManager = this.GetStationManager();
			if (StationManager == null)
				throw new OpcException("No Station Available");

			int ArmNumber = this.GetArmNumber(StationManager);

			if (ArmNumber != this.iLoadArmInUse)
				return;

			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];
			// now set the initial message prompt on the screen
			if (bSetInitialMessage == true || bSetToWait == true)
			{
				if (bSetToWait == true)
					this.SetContrecPrompt(true);
				else
					this.SetContrecPrompt(false);
			}

			ProcessVariableClass ClearDisplay = new ProcessVariableClass();
			ClearDisplay.URL = LoadArmPV.URL;
			ClearDisplay.OPCItemID = LoadArmPV.OPCItemID + ".Clear Display";
			ClearDisplay.ServerValue = true;
			this.OpcServerManager.Write(ClearDisplay);


		}

		public string FormatContrecPrompt(string StockMessage, bool bUseAnswerPrompt)
		{
			string Prompt = "";
			int iLoop;
			int iNumberofCharacters = 0;
			bool bStart = false;
			bool bLookForSpace = false;
			bool bLastCharacterWasSpace = false;

			// we need to reformat the message. The Contrec supports eight lines at 30 characters and we use the '|' as the line seperator
			// we need to remove the [] from the message
			Prompt = "|Varec Terminal Automation|";
			if (bUseAnswerPrompt)
				Prompt += "|";
			for (iLoop = 0; iLoop < StockMessage.Length; iLoop++)
			{
				if (iNumberofCharacters > 20)
				{
					bLookForSpace = true;
				}
				if (bLookForSpace && StockMessage[iLoop] == ' ')
				{
					bLookForSpace = false;
					iNumberofCharacters = 0;
					Prompt += '|';
				}
				if (StockMessage[iLoop] == '|')
					bStart = true;
				else if (StockMessage[iLoop] == ']' || StockMessage[iLoop] == '[')
				{
					bStart = false;
					if (bLastCharacterWasSpace == false)
					{
						Prompt += " ";
						++iNumberofCharacters;
					}
					bLastCharacterWasSpace = true;
				}
				else if (bStart)
				{
					if (bLastCharacterWasSpace &&
						StockMessage[iLoop] == ' ')
					{
					}
					else
					{
						bLastCharacterWasSpace = false;
						Prompt += StockMessage[iLoop];
						++iNumberofCharacters;
					}
				}
			}

			return Prompt;
		}

		public void SetMessageTimeout(int MessageTimeoutValue)
		{
			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];
			ProcessVariableClass MessageTimeout = new ProcessVariableClass();
			MessageTimeout.URL = LoadArmPV.URL;
			MessageTimeout.OPCItemID = LoadArmPV.OPCItemID + ".Set Message Time-out";
			MessageTimeout.ServerValue = MessageTimeoutValue;
			this.OpcServerManager.Write(MessageTimeout);
		}

		public override void DisplayMenu(DisplayMenuParameters parameters)
		{
			StationManagerClass StationManager = this.GetStationManager();
			if (StationManager == null)
				throw new OpcException("No Station Available");

			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			ArrayList Items = new ArrayList();

			// use the get answer prompt to get the response
			string Prompt = "";
			Prompt = this.FormatContrecMenuPrompt(parameters);
			ItemValue DisplayMessageItemValue = new ItemValue(LoadArmPV.OPCItemID + ".Get Answer Prompt");
			DisplayMessageItemValue.Value = Prompt;
			Items.Add(DisplayMessageItemValue);

			this.OpcServerManager.Write(new URL(LoadArmPV.URL), (ItemValue[])Items.ToArray(typeof(ItemValue)));

			this.ResponsePending = true;

			return;
		}

		public string FormatContrecMenuPrompt(DisplayMenuParameters Parameters)
		{
			StationManagerClass StationManager = this.GetStationManager();
			if (StationManager == null)
				throw new OpcException("No Station Available");


			this.SetMessageTimeout(Parameters.MenuTimeout);

			// the menu can consist of a header and up to 6 menu options.
			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			string Message = "|Varec Terminal Automation|";
			int iMaximumLength = 0;
			Message += FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(StationManager.Station.SiteGuid, Parameters.Caption)
																);
			int iCommandNumber;

			// first determine the maximum length of the menus
			for (int nLoop = 0; nLoop < Parameters.Menu.Length; ++nLoop)
			{
				string Value = Parameters.Menu[nLoop];

				if (Parameters.ApplyDataDictionary)
					Value = this.GetDataDictionaryValueByKey(StationManager.Station.SiteGuid, Value);

				if (Value.Length /*+ 4*/ > iMaximumLength)
					iMaximumLength = Value.Length /*+ 4*/;
			}

			for (int nLoop = 0; nLoop < Parameters.Menu.Length; ++nLoop)
			{
				string Value = Parameters.Menu[nLoop];

				if (Parameters.ApplyDataDictionary)
					Value = this.GetDataDictionaryValueByKey(StationManager.Station.SiteGuid, Value);

				if (Value.Length /*+ 4*/ < iMaximumLength)
				{
					for (int nLoop1 = 0; nLoop1 < (iMaximumLength - (Value.Length /*+ 4*/)); nLoop1++)
					{
						Value += " ";
					}
				}
				iCommandNumber = nLoop + 1;
				Message += '|';
				Message += iCommandNumber.ToString() + " - ";
				Message += Value;
			}

			return Message;
		}

		private string GetDataDictionaryValueByKey(Guid guid, string key)
		{
			return FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(guid, key)
																);
		}

		public override bool IsTransactionInProgress()
		{
			StationManagerClass StationManager = this.GetStationManager();
			if (StationManager == null)
				return false;
			int ArmNumber = this.GetArmNumber(StationManager);
			if (ArmNumber == 1 &&
				this.bLoadArm1InUse == true)
				return true;
			else if (ArmNumber == 2 &&
				this.bLoadArm2InUse == true)
				return true;
			else if (ArmNumber == 3 &&
				this.bLoadArm3InUse == true)
				return true;
			else if (ArmNumber == 4 &&
				this.bLoadArm4InUse == true)
				return true;
			return false;
		}

		public override void SendEndTransaction()
		{
			Contrec1010StationManagerClass StationManager = (Contrec1010StationManagerClass)this.GetStationManager();
			if (StationManager == null)
				return;

			int ArmNumber = this.GetArmNumber(StationManager);

			if (ArmNumber != this.iLoadArmInUse)
				return;

			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];
			ProcessVariableClass TerminateTransaction = new ProcessVariableClass();
			TerminateTransaction.URL = LoadArmPV.URL;
			TerminateTransaction.OPCItemID = LoadArmPV.OPCItemID + ".Terminate Transaction";
			TerminateTransaction.ServerValue = true;
			this.OpcServerManager.Write(TerminateTransaction);

			this.ClearContrecDisplay(true, false);

			if (StationManager.StationState == StationState.IDLE
				&& StationManager.ShuttingDown == false)
			{
				if (StationManager.Station.CardReader)
					StationManager.IssueDriverCardPrompt();
				else if (StationManager.Station.TouchKeyReader)
					StationManager.IssueTouchKeyPleaseCardIn();
				else
					StationManager.IssueDriverIDPrompt();
			}
		}

		public override void ReleaseKeyPad()
		{
			// this is used to reset the device by issueing a clear command
			this.SendEndTransaction();
		}

		protected override void OnInvoke(ProcessVariableClass pv)
		{
			Contrec1010StationManagerClass StationManager = (Contrec1010StationManagerClass)this.GetStationManager();
			if (StationManager == null)
			{
				return;
			}

			Monitor.Enter(StationManager);

			try
			{
				if (pv.IsQualityGood)
				{
					if (this.CommunicationsFailure ||
						StationManager.CommunicationsFailure || !this.bInitialized)
					{
						int ArmNumber = this.GetArmNumber(StationManager);
						if (ArmNumber == this.iLoadArmInUse)
						{
							this.SendTransactionComplete();
							StationManager.CommunicationsFailure = false;
							// new code
							if (StationManager.Station.CardReader)
								StationManager.IssueDriverCardPrompt();
							else if (StationManager.Station.TouchKeyReader)
								StationManager.IssueTouchKeyPleaseCardIn();
							else
								StationManager.IssueDriverIDPrompt();
							// end new code
						}
						this.CommunicationsFailure = false;
						this.bInitialized = true;
					}
				}

				switch (pv.ProcessVariableType)
				{
					case PROCESS_VARIABLE_TYPE.LOAD_ARM_STATE_PV:
						{
							if (pv.IsQualityGood)
							{
								if (pv.ServerValue.ToString() == "AA")
								{
									if (this.ResponsePending == true)
									{
										bool bLoadingTerminated = false;
										ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

										Item[] Items ={	new Item(new ItemIdentifier(LoadArmPV.OPCItemID+".Entered Data")),
							                     };

										ItemValueResult[] Values = this.OpcServerManager.Read(new URL(LoadArmPV.URL), Items);
										if (Values[0].Quality == Quality.Good)
										{
											string KeypadData = Values[0].Value.ToString();
											this.ResponsePending = false;

											// the loadrack expects the yes no to be sent as Yes or No. The contrec returns these as YES and NO.
											// change the formatting but only for these two responses
											if (KeypadData.ToString().ToUpper() == "YES")
												KeypadData = "Yes";
											else if (KeypadData.ToString().ToUpper() == "NO")
											{
												if (StationManager.StationState == StationState.VERIFY_SHIPTO_MSG ||
													StationManager.StationState == StationState.PROMPT_FOR_RETURNS)
													KeypadData = "No";
												else
													KeypadData = StationManagerClass.EscapeString;
											}
											else if (this.LoadArmState == LOADARM_STATE.SELECT_PROMPT)
											{
												if (KeypadData[0] == '2') // terminate loading
												{
													this.bLoadArm1InUse = false;
													this.bLoadArm2InUse = false;
													this.bLoadArm3InUse = false;
													this.bLoadArm4InUse = false;
													bLoadingTerminated = true;
												}
											}
											else if (this.LoadArmState == LOADARM_STATE.COMPARTMENT_PROMPT)
											{
												// we need to store this in the object incase the user changes load arms during the load
												this.SelectedCompartment = Convert.ToInt32(KeypadData[0].ToString());
												if (this.SelectedCompartment > 0)
													StationManager.SetSelectedCompartMentNumber(this.SelectedCompartment);
											}
											if (StationManager.StationState == StationState.PROMPT_FOR_RETURNS)
												StationManager.ProcessResponseData(KeypadData);
											else
											{
												if (!this.ProcessResponseData(StationManager, KeypadData))
													StationManager.ProcessResponseData(KeypadData);
											}

											if (bLoadingTerminated == true)
											{
												bLoadingTerminated = false;
												if (StationManager.Station.CardReader)
													StationManager.IssueDriverCardPrompt();
												else if (StationManager.Station.TouchKeyReader)
													StationManager.IssueTouchKeyPleaseCardIn();
												else
													StationManager.IssueDriverIDPrompt();
											}

										}
										else
											this.eventLog.WriteEntry("Contrec OnInvoke : Keypad Data Bad " + pv.OPCItemID, EventLogEntryType.Error);

									}

								}
								else if (pv.ServerValue.ToString() == "RL" &&
									StationManager.StationState == StationState.AUTHORIZED)
								{
									this.IssueLoadNumberTransactiont();
								}
								else if (pv.ServerValue.ToString() == "RC" &&
									StationManager.StationState == StationState.AUTHORIZED)
								{
									this.SetPresetValuePrompt();
								}
								else if (pv.ServerValue.ToString() == "SS" &&
									StationManager.StationState == StationState.AUTHORIZED)
								{
									int ArmNumber = this.GetArmNumber(StationManager);
									if (this.LoadArmState == LOADARM_STATE.PRESET_VOLUME_PROMPT &&
										ArmNumber == this.iLoadArmInUse)
										this.SetMessageControlled();
									else if (this.LoadArmState == LOADARM_STATE.UNLOAD_VOLUME_PROMPT &&
										ArmNumber == this.iLoadArmInUse)
									{
										this.SetUnloadMessageControlled();
										this.SetState(LOADARM_STATE.AUTHORIZED);
									}
									else if (this.LoadArmState == LOADARM_STATE.METERRECIRC_VOLUME_PROMPT &&
										ArmNumber == this.iLoadArmInUse)
									{
										this.SetMeterRecircMessageControlled();
										this.SetState(LOADARM_STATE.AUTHORIZED);
									}
								}
								else if (pv.ServerValue.ToString() == "PL" &&
									StationManager.StationState == StationState.AUTHORIZED)
								{
								}
							}
							else if (!pv.IsQualityGood)
							{
								this.SendEndTransaction();
							}

							break;
						}
					case PROCESS_VARIABLE_TYPE.TRANSACTION_IN_PROGRESS_PV:
						{
							if (pv.IsQualityGood)
							{
								int iTempValue = Convert.ToInt32(pv.ServerValue);

								if ((iTempValue & 0x40) == 0x40)
									this.EmergencyStopPressed = true;
								else if ((iTempValue & 0x40) != 0x40 &&
										this.EmergencyStopPressed == true)
								{
									this.EmergencyStopPressed = false;
									int ArmNumber = this.GetArmNumber(StationManager);
									if (ArmNumber == this.iLoadArmInUse)
									{
										if (StationManager.Station.CardReader)
											StationManager.IssueDriverCardPrompt();
										else if (StationManager.Station.TouchKeyReader)
											StationManager.IssueTouchKeyPleaseCardIn();
										else
											StationManager.IssueDriverIDPrompt();
									}
								}
								if ((iTempValue & 0x80) == 0 &&
									StationManager.StationState != StationState.IDLE &&
									StationManager.StationState != StationState.ENTER_ORDER_PROMPT)
								{
									int ArmNumber = this.GetArmNumber(StationManager);
									if (ArmNumber == this.iLoadArmInUse &&
										this.LoadArmState != LOADARM_STATE.SELECT_PROMPT)
									{
										StationManager.ResetStationDevice();
										// new code
										if (StationManager.Station.CardReader)
											StationManager.IssueDriverCardPrompt();
										else if (StationManager.Station.TouchKeyReader)
											StationManager.IssueTouchKeyPleaseCardIn();
										else
											StationManager.IssueDriverIDPrompt();
										// end new code
									}
								}
								if ((iTempValue & 0x10) == 0x10)
								{
									int ArmNumber = this.GetArmNumber(StationManager);
									if (ArmNumber == this.iLoadArmInUse)
										this.ClearPowerFailureFlag();
								}
							}
							break;
						}
					case PROCESS_VARIABLE_TYPE.LEVEL_PV:	// this is actually arm1 status
						{
							int ArmNumber = this.GetArmNumber(StationManager);
							if (ArmNumber != 1 ||
								this.bLoadArm1InUse != true)
								break;

							if (pv.IsQualityGood)
							{
								if (pv.ServerValue.ToString() == "Batch Complete" ||
									pv.ServerValue.ToString() == "Batch Error")
								{
									if (pv.ServerValue.ToString() == "Batch Error")
										Thread.Sleep(10000);
									if (this.LoadArmState == LOADARM_STATE.INPROGRESS)
									{
										LineItemDO LineItem = StationManager.GetLineItem(this.LoadArm.IdentityGuid);

										if (LineItem != null)
										{
											StationManager.UpdateLineItem(LineItem);
											StationManager.CloseOutLineItem(LineItem);
											if (StationManager.InRecircMode == false)
												StationManager.SaveTransaction();
										}
										this.SetState(LOADARM_STATE.FINISHED);
										this.bLoadArm1InUse = false;
										this.SendTransactionComplete();
										Thread.Sleep(3000); // delay required to allow nvram to update per contrec
										this.ClearContrecDisplay(false, true);
										Thread.Sleep(1000); // delay required to allow nvram to update per contrec
															//										SetFinishedLoading();
															//										Thread.Sleep(3000);

										this.IssueSelectPrompt(StationManager);
										this.CheckForSingleLineitem(StationManager);
									}
								}
								else if (pv.ServerValue.ToString() == "Loading")
								{
									if (this.LoadArmState != LOADARM_STATE.INPROGRESS)
										this.SetState(LOADARM_STATE.INPROGRESS);
								}
							}
							break;
						}
					case PROCESS_VARIABLE_TYPE.TEMPERATURE_PV:	// this is actually arm2 status
						{
							int ArmNumber = this.GetArmNumber(StationManager);
							if (ArmNumber != 2 ||
								this.bLoadArm2InUse != true)
								break;

							if (pv.IsQualityGood)
							{
								if (pv.ServerValue.ToString() == "Batch Complete" ||
									pv.ServerValue.ToString() == "Batch Error")
								{
									if (pv.ServerValue.ToString() == "Batch Error")
										Thread.Sleep(10000);
									if (this.LoadArmState == LOADARM_STATE.INPROGRESS)
									{
										LineItemDO LineItem = StationManager.GetLineItem(this.LoadArm.IdentityGuid);

										if (LineItem != null)
										{
											StationManager.UpdateLineItem(LineItem);
											StationManager.CloseOutLineItem(LineItem);
											if (StationManager.InRecircMode == false)
												StationManager.SaveTransaction();
										}
										this.SetState(LOADARM_STATE.FINISHED);
										this.bLoadArm2InUse = false;
										this.SendTransactionComplete();
										Thread.Sleep(3000); // delay required to allow nvram to update per contrec
										this.ClearContrecDisplay(false, true);
										Thread.Sleep(1000); // delay required to allow nvram to update per contrec
															//										SetFinishedLoading();
															//										Thread.Sleep(3000);

										this.IssueSelectPrompt(StationManager);
										this.CheckForSingleLineitem(StationManager);
									}
								}
								else if (pv.ServerValue.ToString() == "Loading")
								{
									if (this.LoadArmState != LOADARM_STATE.INPROGRESS)
										this.SetState(LOADARM_STATE.INPROGRESS);
								}
							}
							break;
						}
					case PROCESS_VARIABLE_TYPE.GROSS_VOLUME_PV:	// this is actually arm3 status
						{
							int ArmNumber = this.GetArmNumber(StationManager);
							if (ArmNumber != 3 ||
								this.bLoadArm3InUse != true)
								break;

							if (pv.IsQualityGood)
							{
								if (pv.ServerValue.ToString() == "Batch Complete" ||
									pv.ServerValue.ToString() == "Batch Error")
								{
									if (pv.ServerValue.ToString() == "Batch Error")
										Thread.Sleep(10000);
									if (this.LoadArmState == LOADARM_STATE.INPROGRESS)
									{
										LineItemDO LineItem = StationManager.GetLineItem(this.LoadArm.IdentityGuid);

										if (LineItem != null)
										{
											StationManager.UpdateLineItem(LineItem);
											StationManager.CloseOutLineItem(LineItem);
											if (StationManager.InRecircMode == false)
												StationManager.SaveTransaction();
										}
										this.SetState(LOADARM_STATE.FINISHED);
										this.bLoadArm3InUse = false;
										this.SendTransactionComplete();
										Thread.Sleep(3000); // delay required to allow nvram to update per contrec
										this.ClearContrecDisplay(false, true);
										Thread.Sleep(1000); // delay required to allow nvram to update per contrec
															//										SetFinishedLoading();
															//										Thread.Sleep(3000);

										this.IssueSelectPrompt(StationManager);
										this.CheckForSingleLineitem(StationManager);
									}
								}
								else if (pv.ServerValue.ToString() == "Loading")
								{
									if (this.LoadArmState != LOADARM_STATE.INPROGRESS)
										this.SetState(LOADARM_STATE.INPROGRESS);
								}
							}
							break;
						}
					case PROCESS_VARIABLE_TYPE.NET_VOLUME_PV:	// this is actually arm4 status
						{
							int ArmNumber = this.GetArmNumber(StationManager);
							if (ArmNumber != 4 ||
								this.bLoadArm4InUse != true)
								break;

							if (pv.IsQualityGood)
							{
								if (pv.ServerValue.ToString() == "Batch Complete" ||
									pv.ServerValue.ToString() == "Batch Error")
								{
									if (pv.ServerValue.ToString() == "Batch Error")
										Thread.Sleep(10000);
									if (this.LoadArmState == LOADARM_STATE.INPROGRESS)
									{
										LineItemDO LineItem = StationManager.GetLineItem(this.LoadArm.IdentityGuid);

										if (LineItem != null)
										{
											StationManager.UpdateLineItem(LineItem);
											StationManager.CloseOutLineItem(LineItem);
											if (StationManager.InRecircMode == false)
												StationManager.SaveTransaction();
										}
										this.SetState(LOADARM_STATE.FINISHED);
										this.bLoadArm4InUse = false;
										this.SendTransactionComplete();
										Thread.Sleep(3000); // delay required to allow nvram to update per contrec
										this.ClearContrecDisplay(false, true);
										Thread.Sleep(1000); // delay required to allow nvram to update per contrec
															//										SetFinishedLoading();
															//										Thread.Sleep(3000);

										this.IssueSelectPrompt(StationManager);
										this.CheckForSingleLineitem(StationManager);
									}
								}
								else if (pv.ServerValue.ToString() == "Loading")
								{
									if (this.LoadArmState != LOADARM_STATE.INPROGRESS)
										this.SetState(LOADARM_STATE.INPROGRESS);
								}
							}
							break;
						}
					case PROCESS_VARIABLE_TYPE.PROMPT_TIMEOUT_PV:
						{
							if (pv.IsQualityGood
							&& (bool)pv.ServerValue)
							{
								this.ResponsePending = false;

								if (!this.ProcessMessageTimeout(StationManager))
								{
									StationManager.ProcessMessageTimeout();
								}
								if (StationManager.StationState == StationState.IDLE)
								{
									if (StationManager.Station.CardReader)
										StationManager.IssueDriverCardPrompt();
									else if (StationManager.Station.TouchKeyReader)
										StationManager.IssueTouchKeyPleaseCardIn();
									else
										StationManager.IssueDriverIDPrompt();
								}
							}
							break;
						}
					case PROCESS_VARIABLE_TYPE.BATCH_IN_PROGRESS_PV:
						{
							int iServerIntValue = 0;
							if (pv.IsQualityGood)
							{
								if (pv.ServerValue.ToString() != "1" &&
									pv.ServerValue.ToString() != "2" &&
									pv.ServerValue.ToString() != "3" &&
									pv.ServerValue.ToString() != "4")
									break;
								if (StationManager.StationState == StationState.AUTHORIZING
								|| StationManager.StationState == StationState.AUTHORIZED
								|| StationManager.StationState == StationState.TRANSACTION_IN_PROGRESS)
								{
									iServerIntValue = Convert.ToInt32(pv.ServerValue.ToString());
									this.DetermineArmandPresetAmount(iServerIntValue);

									int ArmNumber = this.GetArmNumber(StationManager);
									if (ArmNumber == 1 && this.bLoadArm1InUse == false)
									{
										Thread.Sleep(5000);
										this.Unauthorize();
										this.SendEndTransaction();
										break;
									}
									else if (ArmNumber == 2 && this.bLoadArm2InUse == false)
									{
										Thread.Sleep(5000);
										this.Unauthorize();
										this.SendEndTransaction();
										break;
									}
									else if (ArmNumber == 3 && this.bLoadArm3InUse == false)
									{
										Thread.Sleep(5000);
										this.Unauthorize();
										this.SendEndTransaction();
										break;
									}
									else if (ArmNumber == 4 && this.bLoadArm4InUse == false)
									{
										Thread.Sleep(5000);
										this.Unauthorize();
										this.SendEndTransaction();
										break;
									}
									LineItemDO LineItem = StationManager.GetLineItem(this.LoadArm.IdentityGuid);

									if (LineItem == null)
									{
										this.SetState(LOADARM_STATE.AUTHORIZED);
										if (this.LoadArmState == LOADARM_STATE.AUTHORIZED
										|| this.LoadArmState == LOADARM_STATE.AUTHORIZED_PERMISSIVE_PROMPT
										|| this.LoadArmState == LOADARM_STATE.PRESET_VOLUME_PROMPT)
										{
											this.SetState(LOADARM_STATE.INPROGRESS);

											if (StationManager.AddLineItem(this.LoadArm.IdentityGuid) != null &&
												StationManager.InRecircMode == false)
												StationManager.SaveTransaction();
										}
									}
									else if (this.LoadArmState != LOADARM_STATE.INPROGRESS)
										this.SetState(LOADARM_STATE.INPROGRESS);
								}
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
				this.eventLog.WriteEntry("Contrec LoadArmManager OnInvoke : " + e.Message, EventLogEntryType.Error);
				//            CommunicationsFailure = true;
			}

			catch (Exception e)
			{
				this.eventLog.WriteEntry("Contrec LoadArmManager OnInvoke : PV Type = " + pv.ProcessVariableType.ToString() + " " + e.ToString(), EventLogEntryType.Error);
			}
			finally
			{
				Monitor.Exit(StationManager);
			}
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
			return true;
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

				ItemValue RateItemValue = new ItemValue(LoadArmPV.OPCItemID + ".Set Program Code key");
				RateItemValue.Value = ((AdditiveInjector.PresetNumber - 1) * 5 + 140).ToString() + " " + Additive._AdditiveRate.Value.ToString("F0");
				ItemValues.Add(RateItemValue);

				ItemValue CycleAmountItemValue = new ItemValue(LoadArmPV.OPCItemID + ".Set Program Code key");
				CycleAmountItemValue.Value = ((AdditiveInjector.PresetNumber - 1) * 5 + 143).ToString() + " " + ((1000 / Additive._AdditiveRate.Value) * Additive._AdditiveCycleVolume.Value * 10000).ToString("F0");
				ItemValues.Add(CycleAmountItemValue);
			}

			try
			{
				this.OpcServerManager.Write(new URL(LoadArmPV.URL), (ItemValue[])ItemValues.ToArray(typeof(ItemValue)));
			}
			catch (Exception e)
			{
				this.eventLog.WriteEntry("Contrec LoadArmManager UpdateAdditiveConfiguraiton : " + e.Message, EventLogEntryType.Error);
			}
		}

		public override bool IssuePresetPrompt(StationManagerClass StationManager, double Preset)
		{
			int ArmNumber = this.GetArmNumber(StationManager);
			if (ArmNumber != this.iLoadArmInUse)
				return true;

            this.MaximumPreset = Preset;
			this.SetState(LOADARM_STATE.PRESET_VOLUME_PROMPT);

			this.DisplayMessage("[LoadRack|Enter] " + "|" + " [LoadRack|Preset], [LoadRack|Maximum] " + "|" + this.MaximumPreset, 10, this.PromptTimeout);

			return true;
		}

		public void SetPresetValuePrompt()
		{
			double dPresetMax = 0.0;
			StationManagerClass StationManager = this.GetStationManager();
			if (StationManager == null)
				return;
			int ArmNumber = this.GetArmNumber(StationManager);
			if (ArmNumber != this.iLoadArmInUse)
				return;

			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];
			ProcessVariableClass PresetParameters = new ProcessVariableClass();
			PresetParameters.URL = LoadArmPV.URL;
			PresetParameters.OPCItemID = LoadArmPV.OPCItemID + ".Compartment Response";

			if (StationManager.Station.Type == STATION_TYPE.OFF_LOADING)
				dPresetMax = this.SelectedPresetQuantity + 10.0;
			else
				dPresetMax = this.MaximumPreset;

			PresetParameters.ServerValue = this.SelectedPresetQuantity.ToString() + "@" + dPresetMax.ToString();

			this.OpcServerManager.Write(PresetParameters);

		}

		protected override bool IssueCompartmentPrompt(StationManagerClass stationManager)
		{
			if (stationManager == null)
			{
				return false;
			}

			int ArmNumber = this.GetArmNumber(stationManager);
			if (ArmNumber != this.iLoadArmInUse)
			{
				// we only send messages to arm 1 but we still need to authorize all the arms
				stationManager.AuthorizeLoadArm(this, null, stationManager.SiteManager.Site._MaximumLoadAmount.Value, 0x0);
				return false;
			}

			return base.IssueCompartmentPrompt(stationManager);
		}

		public void IssueLoadNumberTransactiont()
		{
			StationManagerClass StationManager = this.GetStationManager();
			if (StationManager == null)
				return;
			StationManager.StationState = StationState.AUTHORIZED;

			this.ClearContrecDisplay(false, true);
			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];
			ProcessVariableClass LoadNumberTransaction = new ProcessVariableClass();
			LoadNumberTransaction.URL = LoadArmPV.URL;
			LoadNumberTransaction.OPCItemID = LoadArmPV.OPCItemID + ".Load Number Response";

			LoadNumberTransaction.ServerValue = this.iContrecTransActionNumber.ToString();
			++this.iContrecTransActionNumber;

			this.OpcServerManager.Write(LoadNumberTransaction);
			this.Authorize(StationManager, this.CurrentPreset);
			return;
		}

		public override void ProcessPresetResponse(StationManagerClass StationManager, string Response)
		{
			if (Response == StationManagerClass.EscapeString)
			{
				if (!this.IssueSelectRecipePrompt(StationManager, this.MaximumPreset))
				{
					this.IssueSelectPrompt(StationManager);
				}
				return;
			}
			try
			{
				if (Response.ToUpper() == "YES")	// user just pressed enter so use the maximum
					Response = this.MaximumPreset.ToString();
				this.SelectedPresetQuantity = Convert.ToDouble(Response);
			}
			catch
			{
				this.IssuePresetInvalidMessage();
				return;
			}

			if (this.CurrentPreset > this.MaximumPreset)
			{
				this.IssuePresetInvalidMessage();
				return;
			}

			if (StationManager.SiteManager.Site.PromptForReturns)
			{
				StationManager.IssuePromptForReturnsPrompt();//StationManager.StationState = STATION_STATE.PROMPT_FOR_RETURNS;
			}
			else
			{
				this.IssueLoadNumberTransactiont();
			}
		}

		public void SendTransactionComplete()
		{
			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];
			ProcessVariableClass TransactionComplete = new ProcessVariableClass();
			TransactionComplete.URL = LoadArmPV.URL;
			TransactionComplete.OPCItemID = LoadArmPV.OPCItemID + ".Transaction Complete";
			TransactionComplete.ServerValue = true;
			this.OpcServerManager.Write(TransactionComplete);
			this.SetState(LOADARM_STATE.NORMAL);
		}

		public override void EndBatch()
		{
			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];
			ItemValue EndBatch = new ItemValue(LoadArmPV.OPCItemID + ".End Batch");
			this.OpcServerManager.Write(new URL(LoadArmPV.URL), new ItemValue[] { EndBatch });
		}

		public override void ReadBatchRecipe(
			string BatchNumber,
			Server Server,
			out ItemValueResult Recipe)
		{
			ItemValueResult Value = new ItemValueResult();

			Value.Quality = Quality.Good;
			Value.Value = this.SelectedRecipe;// CurrentRecipe.PresetNumber;

			Recipe = Value;
		}

		public override void ReadPresetAmount(
		Server Server,
		out ItemValueResult PresetAmount)
		{
			PresetAmount = new ItemValueResult();
			PresetAmount.Value = Convert.ToDouble(this.CurrentPreset);
			PresetAmount.Quality = Quality.Good;
		}

		public void DetermineArmandPresetAmount(int iServerIntValue)
		{
			string LoadArmURL = this.LoadArm.ProcessVariableCollection[0].URL;
			string LoadArmProgID = this.LoadArm.ProcessVariableCollection[0].ProgID;
			string LoadArmOPCItemID = this.LoadArm.ProcessVariableCollection[0].OPCItemID;
			int iOldLoadArmInUse = this.iLoadArmInUse;

			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			Item[] Items ={new Item(new ItemIdentifier(LoadArmPV.OPCItemID+".Arms.Arm 1.Preset Amount")),
				new Item(new ItemIdentifier(LoadArmPV.OPCItemID+".Arms.Arm 2.Preset Amount")),
				new Item(new ItemIdentifier(LoadArmPV.OPCItemID+".Arms.Arm 3.Preset Amount")),
				new Item(new ItemIdentifier(LoadArmPV.OPCItemID+".Arms.Arm 4.Preset Amount")),
				new Item(new ItemIdentifier(LoadArmPV.OPCItemID+".Arms.Arm 1.Arm Status")),
				new Item(new ItemIdentifier(LoadArmPV.OPCItemID+".Arms.Arm 2.Arm Status")),
				new Item(new ItemIdentifier(LoadArmPV.OPCItemID+".Arms.Arm 3.Arm Status")),
				new Item(new ItemIdentifier(LoadArmPV.OPCItemID+".Arms.Arm 4.Arm Status"))
				};

			ItemValueResult[] Values = this.OpcServerManager.Read(new URL(LoadArmPV.URL), Items);

			Contrec1010StationManagerClass StationManager = (Contrec1010StationManagerClass)this.GetStationManager();
			if (StationManager == null)
				return;

			int ArmNumber = this.GetArmNumber(StationManager);
			// determine the load arms currently being used
			if ((iServerIntValue & 0x01) != 0)
			{
				this.bLoadArm1InUse = true;
				this.iLoadArmInUse = 1;
			}
			if ((iServerIntValue & 0x02) != 0)
			{
				this.bLoadArm2InUse = true;
				this.iLoadArmInUse = 2;
			}
			if ((iServerIntValue & 0x04) != 0)
			{
				this.bLoadArm3InUse = true;
				this.iLoadArmInUse = 3;
			}
			if ((iServerIntValue & 0x08) != 0)
			{
				this.bLoadArm4InUse = true;
				this.iLoadArmInUse = 4;
			}

			if ((iOldLoadArmInUse != this.iLoadArmInUse) &&
				(ArmNumber == iOldLoadArmInUse))
			{
				this.SetState(LOADARM_STATE.NORMAL);
			}
			if ((iOldLoadArmInUse != this.iLoadArmInUse) &&
				(ArmNumber == this.iLoadArmInUse))
			{
				this.SelectedCompartment = StationManager.GetSelectedCompartMentNumber();
			}

			this.CurrentPreset = Convert.ToInt32(Values[this.iLoadArmInUse - 1].Value.ToString());

		}

		public override void CaptureMeterValues()
		{
			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			Server Server = new Server(new Factory(), new URL(LoadArmPV.URL));
			NetworkCredential Credentials = null;
			Server.Connect(new ConnectData(Credentials));

			ItemValueResult[] NonResettableTotal;

			this.ReadNonResettableTotals(
				Server,
				out NonResettableTotal);


			// Component
			if (NonResettableTotal[0].Quality != Quality.Good)
				this.eventLog.WriteEntry("CaptureMeterValues : Non-Resettable Gross Volume OPC Quality Bad " + NonResettableTotal[0].ItemName, EventLogEntryType.Error);
			else
				this.LoadArm.ComponentCollection[0].MeterValue = Convert.ToDouble(NonResettableTotal[0].Value);

			// Additives
			int Item = 0;
			foreach (ProductMapClass AdditiveInjector in this.LoadArm.AdditiveInjectorCollection)
			{
				Item++;
				if (NonResettableTotal[Item].Quality != Quality.Good)
					this.eventLog.WriteEntry("CaptureMeterValues : Non-Resettable Gross Volume OPC Quality Bad " + NonResettableTotal[Item].ItemName, EventLogEntryType.Error);
				else
					AdditiveInjector.MeterValue = Convert.ToDouble(NonResettableTotal[Item].Value);
			}

			Server.Disconnect();
			Server.Dispose();
		}

		public void ReadNonResettableTotals(
			Server Server,
			out ItemValueResult[] NonResettableGrossVolumes)
		{
			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			ArrayList Items = new ArrayList();

			for (int iArm = 1; iArm <= 4; iArm++)
			{
				string TagPrefix = LoadArmPV.OPCItemID + ".Arm Totals.Arm " + iArm.ToString();

				string OPCTagPrefix = TagPrefix + ".Accumulated Gross Volume";

				Items.Add(new Item(new ItemIdentifier(OPCTagPrefix)));
			}

			foreach (Item item in Items)
				item.MaxAgeSpecified = true;

			((Item)Items[0]).MaxAgeSpecified = false;

			ItemValueResult[] Values = Server.Read((Item[])Items.ToArray(typeof(Item)));

			NonResettableGrossVolumes = Values;
		}

		public void GetOffLoadDensity(out double Density)
		{
			Density = this.OffLoadDensity;
		}

		public void ReadComponentData(
			Server Server,
			out ItemValueResult GrossVolume,
			out ItemValueResult NetVolume,
			out ItemValueResult AverageTemperature,
			out ItemValueResult AverageDensity)
		{
			StationManagerClass StationManager = this.GetStationManager();
			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			string TagPrefix = "";// = LoadArmPV.OPCItemID + ".Arm Totals.Arm " + iLoadArmInUse.ToString();
			int ArmNumber = this.GetArmNumber(StationManager);
			if (this.bLoadArm1InUse == true && ArmNumber == 1)
				TagPrefix = LoadArmPV.OPCItemID + ".Arm Totals.Arm " + "1";
			else if (this.bLoadArm2InUse == true && ArmNumber == 2)
				TagPrefix = LoadArmPV.OPCItemID + ".Arm Totals.Arm " + "2";
			else if (this.bLoadArm3InUse == true && ArmNumber == 3)
				TagPrefix = LoadArmPV.OPCItemID + ".Arm Totals.Arm " + "3";
			else if (this.bLoadArm4InUse == true && ArmNumber == 4)
				TagPrefix = LoadArmPV.OPCItemID + ".Arm Totals.Arm " + "4";



			ArrayList Items = new ArrayList();

			Items.Add(new Item(new ItemIdentifier(TagPrefix + ".Batch Gross Volume")));
			Items.Add(new Item(new ItemIdentifier(TagPrefix + ".Batch Net Volume")));
			Items.Add(new Item(new ItemIdentifier(TagPrefix + ".Batch Average Temperature")));
			Items.Add(new Item(new ItemIdentifier(TagPrefix + ".Batch Product Density")));

			foreach (Item item in Items)
				item.MaxAgeSpecified = true;

			((Item)Items[0]).MaxAgeSpecified = false;

			ItemValueResult[] Values = Server.Read((Item[])Items.ToArray(typeof(Item)));

			GrossVolume = Values[0];
			NetVolume = Values[1];
			AverageTemperature = Values[2];
			AverageDensity = Values[3];
		}

		public void GetCompartmentNumberMultipleArms(Server Server,
													out ItemValueResult CompartmentNumber)
		{
			int Selection;
			StationManagerClass StationManager = this.GetStationManager();
			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			string TagPrefix = "";
			int ArmNumber = this.GetArmNumber(StationManager);
			if (this.bLoadArm1InUse == true && ArmNumber == 1)
				TagPrefix = LoadArmPV.OPCItemID + ".Arm Totals.Arm " + "1";
			else if (this.bLoadArm2InUse == true && ArmNumber == 2)
				TagPrefix = LoadArmPV.OPCItemID + ".Arm Totals.Arm " + "2";
			else if (this.bLoadArm3InUse == true && ArmNumber == 3)
				TagPrefix = LoadArmPV.OPCItemID + ".Arm Totals.Arm " + "3";
			else if (this.bLoadArm4InUse == true && ArmNumber == 4)
				TagPrefix = LoadArmPV.OPCItemID + ".Arm Totals.Arm " + "4";

			ArrayList Items = new ArrayList();

			Items.Add(new Item(new ItemIdentifier(TagPrefix + ".Batch Compartment Number")));

			((Item)Items[0]).MaxAgeSpecified = false;

			ItemValueResult[] Values = Server.Read((Item[])Items.ToArray(typeof(Item)));

			if (Values[0].Quality == Quality.Good)
			{
				// Validate in the context of the selected equipment
				Selection = Convert.ToInt32(Values[0].Value);
				this.NonPreloadCompartmentSelection = Selection;
				if (this.NonPreloadEquipmentSelection != "")
				{
					CompartmentInfo Info = this.GetCompartmentIfValid(this.NonPreloadEquipmentSelection, Selection);
					if (Info != null)
					{
						Info.Loaded = true;
					}
				}
			}
			CompartmentNumber = Values[0];
		}

		public void SetMessageControlled()
		{
			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			// now set the initial message prompt on the screen
			ProcessVariableClass InitialMessage = new ProcessVariableClass();
			InitialMessage.URL = LoadArmPV.URL;
			InitialMessage.OPCItemID = LoadArmPV.OPCItemID + ".Initial Message Controlled";
			InitialMessage.ServerValue = "Verify Foot Valve is Open@Press Enter to Continue";
			this.OpcServerManager.Write(InitialMessage);

			ProcessVariableClass ClearDisplay = new ProcessVariableClass();
			ClearDisplay.URL = LoadArmPV.URL;
			ClearDisplay.OPCItemID = LoadArmPV.OPCItemID + ".Clear Display";
			ClearDisplay.ServerValue = true;
			this.OpcServerManager.Write(ClearDisplay);
		}

		public void SetUnloadMessageControlled()
		{
			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			// now set the initial message prompt on the screen
			ProcessVariableClass InitialMessage = new ProcessVariableClass();
			InitialMessage.URL = LoadArmPV.URL;
			InitialMessage.OPCItemID = LoadArmPV.OPCItemID + ".Initial Message Controlled";
			InitialMessage.ServerValue = "Verify Earth is Connected@Press Enter to Continue";
			this.OpcServerManager.Write(InitialMessage);

			ProcessVariableClass ClearDisplay = new ProcessVariableClass();
			ClearDisplay.URL = LoadArmPV.URL;
			ClearDisplay.OPCItemID = LoadArmPV.OPCItemID + ".Clear Display";
			ClearDisplay.ServerValue = true;
			this.OpcServerManager.Write(ClearDisplay);
		}

		public void SetMeterRecircMessageControlled()
		{
			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			// now set the initial message prompt on the screen
			ProcessVariableClass InitialMessage = new ProcessVariableClass();
			InitialMessage.URL = LoadArmPV.URL;
			InitialMessage.OPCItemID = LoadArmPV.OPCItemID + ".Initial Message Controlled";
			InitialMessage.ServerValue = "Verify Recirc Connections@Press Enter to Continue";
			this.OpcServerManager.Write(InitialMessage);

			ProcessVariableClass ClearDisplay = new ProcessVariableClass();
			ClearDisplay.URL = LoadArmPV.URL;
			ClearDisplay.OPCItemID = LoadArmPV.OPCItemID + ".Clear Display";
			ClearDisplay.ServerValue = true;
			this.OpcServerManager.Write(ClearDisplay);
		}

		public void ClearPowerFailureFlag()
		{
			//			ProcessVariableClass LoadArmPV = LoadArm.ProcessVariableCollection[0];

			// clear the power failure bit
			//			ProcessVariableClass ClearPowerFailure = new ProcessVariableClass();
			//			ClearPowerFailure.URL = LoadArmPV.URL;
			//			ClearPowerFailure.OPCItemID = LoadArmPV.OPCItemID + ".Manager Reset";
			//			ClearPowerFailure.ServerValue = 0;
			//			OPCServerManager.Write(ClearPowerFailure);

		}

		public int GetLoadArmInUse()
		{
			return this.iLoadArmInUse;
		}

		public void SetContrecPrompt(bool bSetToPleaseWait)
		{
			bool bExitLoop = false;
			int iRetryCounter = 10;

			StationManagerClass StationManager = this.GetStationManager();
			if (StationManager == null)
				throw new OpcException("No Station Available");

			int ArmNumber = this.GetArmNumber(StationManager);

			if (ArmNumber != this.iLoadArmInUse)
				return;

			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];
			// now set the initial message prompt on the screen
			ProcessVariableClass InitialMessage = new ProcessVariableClass();
			InitialMessage.URL = LoadArmPV.URL;
			InitialMessage.OPCItemID = LoadArmPV.OPCItemID + ".Initial Message";
			if (bSetToPleaseWait)
				InitialMessage.ServerValue = "Please Wait@ ";
			else
				InitialMessage.ServerValue = "Varec Terminal Automation@ ";

			Item[] Items ={	new Item(new ItemIdentifier(LoadArmPV.OPCItemID+".Command Field")),
							                     };
			while (bExitLoop == false)
			{

				ItemValueResult[] Values = this.OpcServerManager.Read(new URL(LoadArmPV.URL), Items);
				if (Values[0].Quality == Quality.Good)
				{
					if (Values[0].Value.ToString() == "SS" ||
						Values[0].Value.ToString() == "AA" ||
						Values[0].Value.ToString() == "RL")
						break;
				}
				if (iRetryCounter <= 0)
					break;
				--iRetryCounter;
				Thread.Sleep(1000);
			}

			this.OpcServerManager.Write(InitialMessage);
		}

		public int GetCompartmentNumber()
		{
			// Validate in the context of the selected equipment
			if (this.SelectedCompartment == 0)
				this.NonPreloadCompartmentSelection = -1;
			else
				this.NonPreloadCompartmentSelection = this.SelectedCompartment;
			if (this.NonPreloadEquipmentSelection != "" &&
				this.NonPreloadCompartmentSelection != -1)
			{
				CompartmentInfo Info = this.GetCompartmentIfValid(this.NonPreloadEquipmentSelection, this.SelectedCompartment);
				if (Info != null)
				{
					Info.Loaded = true;
				}
			}
			return this.SelectedCompartment;
		}

		public void SetDensityInUnit(string Density)
		{
			ProcessVariableClass LoadArmPV = this.LoadArm.ProcessVariableCollection[0];

			StationManagerClass StationManager = this.GetStationManager();
			if (StationManager == null)
				throw new OpcException("No Station Available");

			this.OffLoadDensity = Convert.ToDouble(Density);
			int ArmNumber = this.GetArmNumber(StationManager);

			ProcessVariableClass SetArmDensity = new ProcessVariableClass();
			SetArmDensity.URL = LoadArmPV.URL;
			if (ArmNumber == 1)
				SetArmDensity.OPCItemID = LoadArmPV.OPCItemID + ".Arms.Arm 1.Arm Density";
			else if (ArmNumber == 2)
				SetArmDensity.OPCItemID = LoadArmPV.OPCItemID + ".Arms.Arm 2.Arm Density";
			else if (ArmNumber == 3)
				SetArmDensity.OPCItemID = LoadArmPV.OPCItemID + ".Arms.Arm 3.Arm Density";
			else if (ArmNumber == 4)
				SetArmDensity.OPCItemID = LoadArmPV.OPCItemID + ".Arms.Arm 4.Arm Density";
			SetArmDensity.ServerValue = Density;
			this.OpcServerManager.Write(SetArmDensity);
			Thread.Sleep(3000);
		}

		public void CheckForSingleLineitem(Contrec1010StationManagerClass StationManager)
		{
			if (StationManager.TransactionSupportsMultipleLineItems == false
				|| StationManager.Station.Type == STATION_TYPE.OFF_LOADING
				|| StationManager.InRecircMode == true)
			{
				this.SetFinishedLoading();
				this.bLoadArm1InUse = false;
				this.bLoadArm2InUse = false;
				this.bLoadArm3InUse = false;
				this.bLoadArm4InUse = false;
				if (StationManager.Station.CardReader)
					StationManager.IssueDriverCardPrompt();
				else if (StationManager.Station.TouchKeyReader)
					StationManager.IssueTouchKeyPleaseCardIn();
				else
					StationManager.IssueDriverIDPrompt();
			}
		}

		public void SetUnloadPresetAmount(string Response)
		{
			Contrec1010StationManagerClass StationManager = (Contrec1010StationManagerClass)this.GetStationManager();
			if (StationManager == null)
			{
				return;
			}
			try
			{
				if (Response.ToUpper() == "YES")	// user just pressed enter so use the maximum
					Response = this.MaximumPreset.ToString();
				this.SelectedPresetQuantity = Convert.ToDouble(Response);
			}
			catch
			{
				this.IssuePresetInvalidMessage();
				return;
			}
			this.SelectedRecipe = 1;
			this.UpdateAdditiveConfiguration(StationManager);
			StationManager.CurrentTransactionAlias = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(
																	 x =>
																	 x.Get(this.Security, StationManager.Station.ReceiptByVolumeTransactionAliasGuid, false)
																);

			StationManager.InitializeTransaction();

			this.ClearContrecDisplay(false, true);

			return;
		}

		public void SetMeterRecircPresetAmount(string Response)
		{
			Contrec1010StationManagerClass StationManager = (Contrec1010StationManagerClass)this.GetStationManager();
			if (StationManager == null)
			{
				return;
			}
			try
			{
				if (Response.ToUpper() == "YES")	// user just pressed enter so use the maximum
					Response = this.MaximumPreset.ToString();
				this.SelectedPresetQuantity = Convert.ToDouble(Response);
			}
			catch
			{
				this.IssuePresetInvalidMessage();
				return;
			}
			this.SelectedRecipe = 1;
			this.UpdateAdditiveConfiguration(StationManager);

			StationManager.CurrentTransactionAlias = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(
																	 x =>
																	 x.Get(this.Security, StationManager.Station.ReceiptByVolumeTransactionAliasGuid, false)
																);

			StationManager.InitializeTransaction();
			this.ClearContrecDisplay(false, true);

			return;
		}

      public override bool AllocateRecipes(ulong recipeMap, ulong extendedRecipeMap)
      {
         throw new NotImplementedException();
      }
   }
}

